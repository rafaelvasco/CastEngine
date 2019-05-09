namespace CastFramework
{
    public partial class RenderTarget : Resource
    {
        public int Width => width;

        public int Height => height;

        internal RenderTarget(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.ImplInitialize(width, height);
        }

        internal override void Dispose()
        {
            ImplDispose();
        }

        private int width;

        private int height;
    }
}
