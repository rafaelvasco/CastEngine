using System.Numerics;

namespace CastFramework
{
    public class RenderSurface
    {
        public RenderTarget RenderTarget => render_target;
        public Vertex2D[] Vertices { get; }

        public BlendMode Blending { get; set; }
        public ShaderProgram Shader { get; set; }

        public int Width => RenderTarget.Width;

        public int Height => RenderTarget.Height;

        public int RenderWidth => render_area.Width;

        public int RenderHeight => render_area.Height;

        private RenderTarget render_target;

        internal Matrix4x4 Projection;

        private Rect render_area;

        internal RenderSurface(Rect area)
        {
            render_target = Game.Instance.ContentManager.CreateRenderTarget(area.Width, area.Height);

            Projection = Matrix4x4.CreateOrthographicOffCenter(
                0,
                render_target.Width,
                render_target.Height,
                0,
                0.0f,
                1000.0f
            );

            Vertices = new Vertex2D[4];

            SetArea(area);
        }

        public void ResizeSurface(int width, int height)
        {
            if (width != render_target.Width || height != render_target.Height)
            {
                Game.Instance.ContentManager.DisposeRuntimeLoaded(RenderTarget);

                render_target = Game.Instance.ContentManager.CreateRenderTarget(width, height);

                Projection = Matrix4x4.CreateOrthographicOffCenter(
                    0,
                    render_target.Width,
                    render_target.Height,
                    0,
                    0.0f,
                    1000.0f
                );
            }
        }

        public void SetArea(Rect area)
        {
            render_area = area;

            Vertices[0] = new Vertex2D(area.X1, area.Y1, 0, 0, 0xFFFFFFFF);
            Vertices[1] = new Vertex2D(area.X2, area.Y1, 1, 0, 0xFFFFFFFF);
            Vertices[2] = new Vertex2D(area.X2, area.Y2, 1, 1, 0xFFFFFFFF);
            Vertices[3] = new Vertex2D(area.X1, area.Y2, 0, 1, 0xFFFFFFFF);
        }

    }
}
