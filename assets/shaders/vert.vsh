#version 330
precision highp float;
precision highp int;
precision highp sampler2D;

layout (location = 0) in vec3 position;

out vec2 TexCoord;

void main() {
    gl_Position = vec4(position, 1.0);
    TexCoord = (position.xy + 1) / 2;
}
