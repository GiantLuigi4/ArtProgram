#version 430
precision highp float;
precision highp int;
precision highp sampler2D;

//uniform vec2 Resolution; // uniform-Resolution

#ifdef COMPUTE
    layout (local_size_x = 32, local_size_y = 32, local_size_z = 1) in;
    layout(rgba16, binding = 0) uniform image2D Output;
#else
    in vec2 TexCoord;
    out vec4 color;
#endif
uniform vec2 Resolution; // uniform-Resolution

layout(binding = 1) uniform sampler2D Texture; // uniform-Texture

void main() {
    #ifdef COMPUTE
        ivec2 coord = ivec2(gl_GlobalInvocationID.xy);
    #else
        ivec2 coord = ivec2(TexCoord * Resolution);
    #endif

    #ifdef COMPUTE
        imageStore(Output, coord, texelFetch(Texture, coord, 0));
    #else
        color = texelFetch(Texture, coord, 0);
    #endif
}
