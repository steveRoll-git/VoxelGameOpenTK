#version 330 core

in vec2 frag_texCoords;

out vec4 out_color;

void main()
{
    out_color = vec4(frag_texCoords.x, frag_texCoords.y, 0, 1.0);
}