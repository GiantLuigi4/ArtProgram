using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtProgram.display;
using ArtProgram.display.shader;
using ArtProgram.scripting.lib;
using ArtProgram.scripting.obj;
using OpenGL;

namespace ArtProgram.layer {
    public class TextureLayer {
        public Texture texture;
        private Texture? interlace;
        private Texture? interlaced;

        public TextureLayer(int width, int height) {
            this.texture = new Texture(width, height, true);
        }

        public Texture GetTexture(Texture swap0, Texture swap1) {
            if (interlace != null) {
                interlaced.BindWrite();
                Gl.ClearColor(0, 0, 0, 0f);
                Gl.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

                BaseShader sdr = RenderHelper.INSTANCE.MERGE_FRAG;
                sdr.Bind();

                Gl.Uniform2(sdr.GetUniform("Resolution"), (float) interlaced.Width, interlaced.Height);
                Gl.Uniform1(sdr.GetUniform("Alpha"), 1f);

                sdr.BindOutput(interlaced, "Output", 0);

                Gl.ActiveTexture(TextureUnit.Texture0 + 1);
                Gl.BindTexture(TextureTarget.Texture2d, texture.tex);
                Gl.Uniform1(sdr.GetUniform("Tex0"), 1);

                Gl.ActiveTexture(TextureUnit.Texture0 + 2);
                Gl.BindTexture(TextureTarget.Texture2d, interlace.tex);
                Gl.Uniform1(sdr.GetUniform("Tex1"), 2);

                GlLib.INSTANCE.blit(new LuaShader(sdr), new LuaTexture(interlaced));

                Gl.UseProgram(0);
                
                return interlaced;
            }

            return texture;
        }

        public void MergeInterlace() {
            interlaced.BindWrite();
            Gl.ClearColor(0, 0, 0, 0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            BaseShader sdr = RenderHelper.INSTANCE.MERGE_SHADER;
            sdr.Bind();

            Gl.Uniform2(sdr.GetUniform("Resolution"), (float) interlaced.Width, interlaced.Height);
            Gl.Uniform1(sdr.GetUniform("Alpha"), 1f);

            sdr.BindOutput(interlaced, "Output", 0);

            Gl.ActiveTexture(TextureUnit.Texture0 + 1);
            Gl.BindTexture(TextureTarget.Texture2d, texture.tex);
            Gl.Uniform1(sdr.GetUniform("Tex0"), 1);

            Gl.ActiveTexture(TextureUnit.Texture0 + 2);
            Gl.BindTexture(TextureTarget.Texture2d, interlace.tex);
            Gl.Uniform1(sdr.GetUniform("Tex1"), 2);

            GlLib.INSTANCE.blit(new LuaShader(sdr), new LuaTexture(interlaced));

            Gl.UseProgram(0);

            var swap = texture;
            texture = interlaced;
            interlaced = swap;
        }

        public void Interlace(Texture? texture) {
            if (texture == null) {
                interlace = null;
                interlaced.Delete();
            } else {
                interlace = texture;
                interlaced = new Texture(interlace.Width, interlace.Height, true);
            }
        }

        public void Clear(float r, float g, float b, float a) {
            texture.BindWrite();
            Gl.ClearColor(r, g, b, a);
            Gl.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
        }
    }
}
