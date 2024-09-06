using Engine;
using System.Numerics;

namespace VoxelGameOpenTK;

internal class Map
{
    static readonly VertexFormat vertexFormat = new(
        new(VertexAttribute.Position, 3),
        new(VertexAttribute.TexCoords, 2));

    internal record struct FaceRotation(Vector3 Normal, Vector3[] RotatedVerts);
    static readonly FaceRotation[] faceRotations;

    static Map()
    {
        // For each rotation, we generate the normal that points towards it, and transformed vertices of that normal's face.
        Quaternion[] rotations =
        [
            Quaternion.CreateFromYawPitchRoll(0, 0, 0),
            Quaternion.CreateFromYawPitchRoll(MathF.PI / 2, 0, 0),
            Quaternion.CreateFromYawPitchRoll(MathF.PI, 0, 0),
            Quaternion.CreateFromYawPitchRoll(-MathF.PI / 2, 0, 0),
            Quaternion.CreateFromYawPitchRoll(0, 0, MathF.PI / 2),
            Quaternion.CreateFromYawPitchRoll(0, 0, -MathF.PI / 2),
        ];
        Vector3[] templateFace =
        [
            new(1, 0, 0),
            new(1, 1, 0),
            new(1, 1, 1),
            new(1, 0, 1),
        ];
        faceRotations = rotations.Select(r =>
        {
            var matrix = Matrix4x4.CreateTranslation(-0.5f, -0.5f, -0.5f) * Matrix4x4.CreateFromQuaternion(r) * Matrix4x4.CreateTranslation(0.5f, 0.5f, 0.5f);
            var normal = Vector3.Transform(Vector3.UnitX, r);
            var rotatedVerts = templateFace.Select(v => Vector3.Transform(v, matrix).Round()).ToArray();
            return new FaceRotation(normal.Round(), rotatedVerts);
        }).ToArray();
    }

    public bool[,,] Voxels;

    public bool IsInRange(int x, int y, int z)
    {
        return
            z >= 0 && z < Voxels.GetLength(0) &&
            x >= 0 && x < Voxels.GetLength(1) &&
            y >= 0 && y < Voxels.GetLength(2);
    }

    public bool GetVoxel(int x, int y, int z)
    {
        if (!IsInRange(x, y, z))
        {
            return false;
        }
        return Voxels[z, y, x];
    }

    public Mesh GenerateMesh()
    {
        // When a vertex is created, it's stored in this dictionary so that its index can be reused if needed.
        Dictionary<Vector3, uint> vertices = [];
        uint lastVertex = 0;
        List<uint> indices = [];

        for (int z = 0; z < Voxels.GetLength(0); z++)
        {
            for (int y = 0; y < Voxels.GetLength(1); y++)
            {
                for (int x = 0; x < Voxels.GetLength(2); x++)
                {
                    var cell = GetVoxel(x, y, z);

                    if (cell)
                    {
                        foreach (var face in faceRotations)
                        {
                            if (GetVoxel(x + (int)face.Normal.X, y + (int)face.Normal.Y, z + (int)face.Normal.Z))
                            {
                                // Don't generate faces that are adjacent to another voxel.
                                continue;
                            }
                            var faceIndices = new uint[4];
                            for (int i = 0; i < 4; i++)
                            {
                                var vert = face.RotatedVerts[i];
                                var newPos = vert + new Vector3(x, y, z);
                                if (vertices.TryGetValue(newPos, out uint existingIndex))
                                {
                                    faceIndices[i] = existingIndex;
                                }
                                else
                                {
                                    faceIndices[i] = lastVertex;
                                    vertices.Add(newPos, lastVertex);
                                    lastVertex++;
                                }
                            }
                            indices.Add(faceIndices[0]);
                            indices.Add(faceIndices[1]);
                            indices.Add(faceIndices[3]);
                            indices.Add(faceIndices[1]);
                            indices.Add(faceIndices[2]);
                            indices.Add(faceIndices[3]);
                        }
                    }
                }
            }
        }

        var verticesArray = new float[vertices.Count, vertexFormat.numComponents];
        foreach (var (vertex, i) in vertices.Select((i, v) => (i, v)))
        {
            verticesArray[i, 0] = vertex.Key.X;
            verticesArray[i, 1] = vertex.Key.Y;
            verticesArray[i, 2] = vertex.Key.Z;
        }
        return new Mesh(vertexFormat, verticesArray, [.. indices]);
    }
}