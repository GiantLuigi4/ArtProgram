#version 130
precision highp float;
precision highp int;
precision highp sampler2D;

uniform sampler2D Texture; // uniform-Texture
in vec2 TexCoord; // input-TexCoord

out vec4 FragColor;

void main() {
    FragColor = texelFetch(Texture, ivec2(textureSize(Texture, 0) * TexCoord), 0);
}
