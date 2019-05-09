using System;

namespace CastFramework
{
    public partial class Texture2D : Resource
    {
        internal Texture Texture {get; private set; }

        internal TextureFlags TexFlags {get; private set; }

        private void ImplInitialize(Pixmap pixmap)
        {
            this.ImplUpdateTexProperties();

            this.Texture = Texture.Create2D(
                pixmap.Width,
                pixmap.Height,
                false,
                0,
                TextureFormat.BGRA8,
                TexFlags,
                MemoryBlock.FromArray(pixmap.PixelData)
            );
        }

        private void ImplUpdateTexProperties()
        {
            var tex_flags = TextureFlags.None;

            if (!Tiled) tex_flags = TextureFlags.ClampU | TextureFlags.ClampV;

            if (!Filtered) tex_flags |= TextureFlags.MinFilterPoint | TextureFlags.MagFilterPoint;

            if (RenderTarget) tex_flags |= (TextureFlags.RenderTarget | TextureFlags.RenderTargetWriteOnly);

            this.TexFlags = tex_flags;
        }

        private void ImplSetData(Pixmap pixmap)
        {
            var memory = MemoryBlock.MakeRef(pixmap.PixelDataPtr, pixmap.SizeBytes, IntPtr.Zero);

            Texture.Update2D(0, 0, 0, 0, pixmap.Width, pixmap.Height, memory, pixmap.Stride);
        }

        private Pixmap ImplGetData()
        {
            return null;
        }

        private Pixmap ImplGetData(int src_x, int src_y, int src_w, int src_h)
        {
            return null;
        }

        private void ImplDispose()
        {
            this.Texture.Dispose();
        }
    }

    
}
