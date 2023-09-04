#version 130
uniform vec4 Color; // uniform-Color

in vec2 TexCoord;

uniform sampler2D Texture;
uniform ivec2 Resolution;
uniform ivec2 CanvasSize;
uniform vec2 Offset;
uniform float Scale;

out vec4 FragColor;

void main() {
    vec2 center = Resolution / 2.0;
    vec2 crd = TexCoord * Resolution;
    crd -= center;
    crd += Offset;
    crd /= Scale;
    crd += CanvasSize / 2.0;

    if (crd.x > CanvasSize.x) discard;
    if (crd.x < 0) discard;
    if (crd.y > CanvasSize.y) discard;
    if (crd.y < 0) discard;

    FragColor = texelFetch(Texture, ivec2(crd), 0);
}
