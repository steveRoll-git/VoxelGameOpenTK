using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGameOpenTK;

using static ResourceUtils;

internal class Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : GameWindow(gameWindowSettings, nativeWindowSettings)
{
    private Mesh cubeMesh;
    private Shader shader;
    private Matrix4x4 projectionMatrix;
    private Camera camera;

    private float time = 0;

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.Enable(EnableCap.DepthTest);

        var vertexFormat = new VertexFormat(
            new(VertexAttribute.Position, 3),
            new(VertexAttribute.TexCoords, 2)
        );

        cubeMesh = new Mesh(vertexFormat, new float[,] {
            { 0.5f, 0.5f, -0.5f, 0, 0 },
            { 0.5f, -0.5f, -0.5f, 0, 1 },
            { -0.5f, -0.5f, -0.5f, 1, 1 },
            { -0.5f, 0.5f, -0.5f, 1, 0 },
            { 0.5f, 0.5f, 0.5f, 0, 0 },
            { 0.5f, -0.5f, 0.5f, 0, 1 },
            { -0.5f, -0.5f, 0.5f, 1, 1 },
            { -0.5f, 0.5f, 0.5f, 1, 0 },
        }, [
            0, 1, 3,
            1, 2, 3,
            4, 5, 7,
            5, 6, 7,
        ]);

        camera = new Camera
        {
            Position = new(0, 0, 2)
        };

        shader = new Shader(ReadResource("shaders.default.vert"), ReadResource("shaders.default.frag"));

        projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 2 * 0.7f, (float)ClientSize.X / ClientSize.Y, 0.1f, 100);

        shader.SendMat4("projection", projectionMatrix);
        shader.SendMat4("view", camera.GetView());
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        time += (float)args.Time;
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        shader.Use();
        cubeMesh.Draw(Matrix4x4.CreateRotationZ(time) * Matrix4x4.CreateRotationY(time / 2));

        SwapBuffers();
    }
}
