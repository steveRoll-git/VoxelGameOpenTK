using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using VoxelGameOpenTK;

var nativeWindowSettings = new NativeWindowSettings()
{
    ClientSize = new Vector2i(800, 600),
    // This is needed to run on macos
    Flags = ContextFlags.ForwardCompatible,
    Vsync = VSyncMode.On,
};

using var window = new Window(GameWindowSettings.Default, nativeWindowSettings);
window.Run();