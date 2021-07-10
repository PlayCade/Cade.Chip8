#version 450
layout(location = 0) in vec4 fsin_Colour;
layout(location = 0) out vec4 fsout_Colour;

void main()
{
    fsout_Colour = fsin_Colour;
}