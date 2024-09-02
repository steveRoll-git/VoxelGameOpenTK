using OpenTK.Graphics.OpenGL4;
using System.Numerics;

namespace VoxelGameOpenTK;

internal class Mesh
{
    private readonly int vao;
    private readonly int vbo;
    private readonly int ebo;

    private readonly int vertexCount;

    public Mesh(VertexFormat vertexFormat, float[,] vertices, uint[] indices)
    {
        vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

        if (vertices.GetLength(1) != vertexFormat.numComponents)
        {
            throw new Exception("Number of components in vertex array doesn't match the vertex format");
        }

        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);


        ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);


        var offset = 0;
        foreach (var attribute in vertexFormat.attributes)
        {

            var location = (int)attribute.Attribute;
            GL.EnableVertexAttribArray(location);
            GL.VertexAttribPointer(location, attribute.Size, VertexAttribPointerType.Float, false, vertexFormat.Stride, offset);
            offset += attribute.Size * sizeof(float);
        }

        vertexCount = indices.Length;

        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
    }

    public void Draw()
    {
        Draw(Matrix4x4.Identity);
    }

    public void Draw(Matrix4x4 transform)
    {
        GL.BindVertexArray(vao);
        Shader.CurrentShader.SendMat4("transform", transform);
        GL.DrawElements(PrimitiveType.Triangles, vertexCount, DrawElementsType.UnsignedInt, 0);
    }
}
