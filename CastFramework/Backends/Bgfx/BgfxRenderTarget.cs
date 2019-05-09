namespace CastFramework
{
    public partial class RenderTarget
    {
        internal FrameBuffer FrameBuffer { get; private set;}

        internal Texture NativeTexture { get; private set;}

        private void ImplInitialize(int width, int height)
        {
            FrameBuffer = new FrameBuffer(width, height, TextureFormat.BGRA8, TextureFlags.ClampU | TextureFlags.ClampV | TextureFlags.FilterPoint);

            NativeTexture = FrameBuffer.GetTexture();
        }

        private void ImplDispose()
        {
            FrameBuffer.Dispose();
        }

    }
}
