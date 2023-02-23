--Vertex
#version 430
in vec3 InPosition;
in vec4 InColor;

smooth out vec4 Color;

uniform mat4 ModelViewProjectionMatrix;

void main()
{
	gl_Position = ModelViewProjectionMatrix * vec4(InPosition, 1);
	Color = InColor;
}

--Fragment
#version 430
smooth in vec4 Color;

out vec4 FragColor;

void main()
{
	FragColor = Color;
}