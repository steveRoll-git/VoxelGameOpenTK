#version 330 core

layout (location = ATTRIBUTE_LOCATION_POSITION) in vec3 position;
layout (location = ATTRIBUTE_LOCATION_TEXCOORDS) in vec2 texCoords;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 transform;

out vec2 frag_texCoords;
out vec3 modelCoords;

void main()
{
    gl_Position = projection * view * transform * vec4(position, 1.0);
    modelCoords = position;
    frag_texCoords = texCoords;
}