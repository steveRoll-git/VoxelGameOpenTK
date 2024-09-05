using System.Text;
using System.Numerics;
using OpenTK.Graphics.OpenGL4;

namespace Engine;

public class Shader
{
    private readonly int programHandle;

    public static Shader? CurrentShader { get; private set; }

    public Shader(string vertexCode, string fragmentCode)
    {
        vertexCode = PreprocessShaderCode(vertexCode);
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexCode);

        GL.CompileShader(vertexShader);

        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int vStatus);
        if (vStatus != (int)All.True)
            throw new Exception("Vertex shader failed to compile: " + GL.GetShaderInfoLog(vertexShader));

        fragmentCode = PreprocessShaderCode(fragmentCode);
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentCode);

        GL.CompileShader(fragmentShader);

        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int fStatus);
        if (fStatus != (int)All.True)
            throw new Exception("Fragment shader failed to compile: " + GL.GetShaderInfoLog(fragmentShader));

        programHandle = GL.CreateProgram();

        GL.AttachShader(programHandle, vertexShader);
        GL.AttachShader(programHandle, fragmentShader);

        GL.LinkProgram(programHandle);

        GL.GetProgram(programHandle, GetProgramParameterName.LinkStatus, out int lStatus);
        if (lStatus != (int)All.True)
            throw new Exception("Program failed to link: " + GL.GetProgramInfoLog(programHandle));

        GL.DetachShader(programHandle, vertexShader);
        GL.DetachShader(programHandle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public void Use()
    {
        if (CurrentShader == this)
        {
            return;
        }
        GL.UseProgram(programHandle);
        CurrentShader = this;
    }

    public void SendMat4(string uniform, Matrix4x4 mat4)
    {
        Use();
        var location = GL.GetUniformLocation(programHandle, uniform);
        unsafe
        {
            GL.UniformMatrix4(location, 1, false, (float*)&mat4);
        }
    }

    public static string PreprocessShaderCode(string code)
    {
        const string VERSION_DIRECTIVE = "#version 330 core";
        var sb = new StringBuilder();
        sb.AppendLine(VERSION_DIRECTIVE);
        foreach (var attribute in Enum.GetValues<VertexAttribute>())
        {
            sb.AppendLine($"#define ATTRIBUTE_LOCATION_{Enum.GetName(attribute)!.ToUpper()} {(uint)attribute}");
        }
        sb.AppendLine("#line 1");
        sb.Append(code.Replace(VERSION_DIRECTIVE, ""));
        return sb.ToString();
    }
}
