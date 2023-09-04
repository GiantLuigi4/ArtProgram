using OpenGL;

namespace ArtProgram.display {
    public class Texture {
        public uint tex;
        public uint fbo;
        public uint rbo;
    	public readonly int Width, Height;

        bool EnableFbo;

        public Texture(int width, int height, bool enableFbo) {
            this.Width = width;
            this.Height = height;
    		this.EnableFbo = enableFbo;
            Create();

            BindWrite();
            Gl.ClearColor(0f, 0, 0, 0);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
        
        public void BindWrite() {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
        }

        private void Create() {
            this.tex = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, tex);

            Gl.TexParameterI(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, new int[]{ Gl.CLAMP_TO_EDGE });
            Gl.TexParameterI(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, new int[]{ Gl.CLAMP_TO_EDGE });

            Gl.TexParameterI(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, new int[]{ Gl.LINEAR });
            Gl.TexParameterI(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, new int[]{ Gl.LINEAR });

            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba16, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, 0);

            if (EnableFbo) {
                rbo = Gl.GenRenderbuffer();
                fbo = Gl.GenFramebuffer();

                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
                Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, tex, 0);

                Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
                
                Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent32, Width, Height);
                Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rbo);

			    Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }

            Gl.BindTexture(TextureTarget.Texture2d, 0);
        }

        public void Delete() {
            Gl.DeleteTextures(tex);
            if (EnableFbo) {
                Gl.DeleteFramebuffers(fbo);
                Gl.DeleteRenderbuffers(rbo);
            }
        }
    }
}