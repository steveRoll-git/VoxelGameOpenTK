namespace VoxelGameOpenTK;

internal record AttributeDefinition(VertexAttribute Attribute, int Size);

internal class VertexFormat(params AttributeDefinition[] attributes)
{
    public readonly int numComponents = attributes.Sum(a => a.Size);
    public int Stride => numComponents * sizeof(float);
    public readonly AttributeDefinition[] attributes = attributes;
}
