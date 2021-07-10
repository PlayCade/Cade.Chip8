#version 450

layout(location = 0) in vec2 Position;
layout(location = 1) in vec4 Colour;

layout(location = 0) out vec4 fsin_Colour;

void main()
{
    fsin_Colour = Colour;
    gl_Position = vec4(Position, 0, 1);
}