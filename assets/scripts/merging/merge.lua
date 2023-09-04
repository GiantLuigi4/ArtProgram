function blit(lower, upper, dst, alpha)
    dst.clear()
    
    -- get the required shader
    sdr = Gl.getShader("layer/merge")
    sdr.bind()

    -- setup shader
    Gl.setupUniforms(sdr)
    sdr.setUniformF("Alpha", alpha)

    -- bind textures
    dst.bindWrite(sdr, "Output", 0)
    lower.bind(sdr, "Tex0", 1)
    upper.bind(sdr, "Tex1", 2)

    -- draw to destination texture
    Gl.blit(sdr, dst)
    sdr.unbind()
end

return {
    args = {
        {
            name = "alpha",
            type = "double",
            min = 0, max = 1,
            default = 1
        },
        {
            name = " blendFunc",
            type = "string",
            default = "alpha",
            options = {"alpha", "additive", "multiplicative"}
        }
    }
}
