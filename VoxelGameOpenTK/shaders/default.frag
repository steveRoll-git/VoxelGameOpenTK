#version 330 core

in vec2 frag_texCoords;
in vec3 modelCoords;

out vec4 out_color;

void main()
{
    out_color = vec4(abs(modelCoords), 1.0);
}