using Engine;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Numerics;

namespace VoxelGameOpenTK;

using static ResourceUtils;

internal class Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : GameWindow(gameWindowSettings, nativeWindowSettings)
{
    private Mesh cubeMesh;
    private Shader shader;
    private Matrix4x4 projectionMatrix;
    private Camera camera;
    private Map map;
    private Mesh mapMesh;

    private float time = 0;

    private float sensitivity = 0.005f;

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);

        var vertexFormat = new VertexFormat(
            new(VertexAttribute.Position, 3),
            new(VertexAttribute.TexCoords, 2)
        );

        cubeMesh = new Mesh(vertexFormat, new float[,] {
            {  0.5f,  0.5f, -0.5f, 0, 0 },
            {  0.5f, -0.5f, -0.5f, 0, 1 },
            { -0.5f, -0.5f, -0.5f, 1, 1 },
            { -0.5f,  0.5f, -0.5f, 1, 0 },
            {  0.5f,  0.5f,  0.5f, 0, 0 },
            {  0.5f, -0.5f,  0.5f, 0, 1 },
            { -0.5f, -0.5f,  0.5f, 1, 1 },
            { -0.5f,  0.5f,  0.5f, 1, 0 },
        }, [
            0, 1, 3,
            1, 2, 3,
            5, 4, 7,
            6, 5, 7,
            1, 0, 4,
            5, 1, 4,
            6, 7, 3,
            2, 6, 3,
            4, 0, 3,
            7, 4, 3,
            1, 5, 6,
            2, 1, 6,
        ]);

        camera = new Camera
        {
            Position = new(0, 0, 2)
        };

        map = new Map
        {
            Voxels = new bool[,,] {
                {
                    { true, false, true },
                    { false, false, false },
                    { false, true, false },
                },
                {
                    { false, false, true },
                    { false, false, false },
                    { true, true, true},
                },
                {
                    { true, false, false },
                    { true, false, false },
                    { true, true, true},
                },
            }
        };

        mapMesh = map.GenerateMesh();

        shader = new Shader(ReadResource("shaders.default.vert"), ReadResource("shaders.default.frag"));

        projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 2 * 0.7f, (float)ClientSize.X / ClientSize.Y, 0.1f, 100);

        shader.SendMat4("projection", projectionMatrix);
        UpdateView();

        CursorState = CursorState.Grabbed;
        RawMouseInput = true;

        NewInputFrame();
    }

    private void UpdateView()
    {
        shader.SendMat4("view", camera.GetView());
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        time += (float)args.Time;

        var input = KeyboardState;

        if (input.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        Vector3 movement = new();
        if (input.IsKeyDown(Keys.W))
        {
            movement.Z -= 1;
        }
        else if (input.IsKeyDown(Keys.S))
        {
            movement.Z += 1;
        }
        if (input.IsKeyDown(Keys.D))
        {
            movement.X += 1;
        }
        else
        if (input.IsKeyDown(Keys.A))
        {
            movement.X -= 1;
        }
        movement = Vector3.Transform(movement, Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, camera.RotationY));
        if (input.IsKeyDown(Keys.Space))
        {
            movement.Y += 1;
        }
        else if (input.IsKeyDown(Keys.LeftControl))
        {
            movement.Y -= 1;
        }
        camera.Position += movement * (float)args.Time;

        var mouse = MouseState;
        camera.RotationY -= mouse.Delta.X * sensitivity;
        camera.RotationX -= mouse.Delta.Y * sensitivity;
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        UpdateView();

        shader.Use();
        mapMesh.Draw();

        SwapBuffers();
    }
}
