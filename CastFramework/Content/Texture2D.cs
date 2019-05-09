namespace CastFramework
{
    public partial class Texture2D : Resource
    {
        public int Width => width;

        public int Height => height;

        public bool RenderTarget => render_target;

        public bool Tiled
        {
            get => tiled;
            set
            {
                if (tiled == value) return;

                tiled = value;

                this.ImplUpdateTexProperties();
            }
        }

        public bool Filtered
        {
            get => filtered;
            set
            {
                if (filtered == value) return;

                filtered = value;

                this.ImplUpdateTexProperties();
            }
        }

        private int width;

        private int height;

        private bool render_target;

        private bool tiled;

        private bool filtered;

        internal Texture2D(Pixmap pixmap, bool filtered, bool tiled)
        {
            this.width = pixmap.Width;
            this.height = pixmap.Height;
            this.render_target = false;
            this.filtered = filtered;
            this.tiled = tiled;

            this.ImplInitialize(pixmap);
        }

        internal Texture2D(int width, int height, bool filtered, bool tiled)
        {
            this.width = width;
            this.height = height;
            this.render_target = true;
            this.filtered = filtered;
            this.tiled = tiled;
        }

        public void SetData(Pixmap pixmap)
        {
            this.ImplSetData(pixmap);
        }

        public Pixmap GetData()
        {
            return this.ImplGetData();
        }

        public Pixmap GetData(int srcX, int srcY, int srcW, int srcH)
        {
            return this.ImplGetData(srcX, srcY, srcW, srcH);
        }

        internal override void Dispose()
        {
            this.ImplDispose();
        }
    }
}
