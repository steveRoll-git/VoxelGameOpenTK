using System.Numerics;

namespace VoxelGameOpenTK;
internal class Camera
{
    public Vector3 Position;
    public float RotationX;
    public float RotationY;

    public Matrix4x4 GetView()
    {
        Matrix4x4.Invert(Matrix4x4.CreateRotationX(RotationX) * Matrix4x4.CreateRotationY(RotationY) * Matrix4x4.CreateTranslation(Position), out var inverse);
        return inverse;
    }
}
