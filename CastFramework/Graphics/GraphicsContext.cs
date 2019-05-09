using System;
using System.Collections.Generic;

namespace CastFramework
{
    public enum GraphicsBackend
    {
        OpenGL,
        Direct3D
    }

    public struct GraphicsInfo
    {
        public readonly GraphicsBackend Backend;
        public int MaxTextureSize;

        public GraphicsInfo(GraphicsBackend backend, int max_tex_size)
        {
            Backend = backend;
            MaxTextureSize = max_tex_size;
        }
    }

    public unsafe partial class GraphicsContext : IDisposable
    {
        public GraphicsInfo Info { get; private set; }

        private List<RenderPipeline> pipelines;

        internal GraphicsContext(IntPtr graphics_surface_ptr, int width, int height)
        {
            pipelines = new List<RenderPipeline>();

            ImplInitialize(graphics_surface_ptr, width, height);
        }

        public void SetClearColor(byte render_pass, Color color)
        {
            ImplSetClearColor(render_pass, color);
        }

        public RenderPipeline CreatePipeline(int max_vertex_count, Rect render_area)
        {
            var pipeline = new RenderPipeline(this, max_vertex_count, render_area);

            pipelines.Add(pipeline);

            return pipeline;
        }

        public void SwapBuffers()
        {
            ImplSwapBuffers();
        }

        public void ResizeBackBuffer(int width, int height)
        {
            ImplResizeBackbuffer(width, height);
        }

        public void SetRenderTarget(byte render_pass, RenderTarget render_target)
        {
            ImplSetRenderTarget(render_pass, render_target);
        }

        public void SetViewport(byte render_pass, int x, int y, int w, int h)
        {
            ImplSetViewport(render_pass, x, y, w, h);
        }

        public void SetScissor(byte render_pass, int x, int y, int w, int h)
        {
            ImplSetScissor(render_pass, x, y, w, h);
        }

        public void SetProjection(byte render_pass, float* matrix)
        {
            ImplSetProjectionMatrix(render_pass, matrix);
        }

        public void TakeScreenShot(string output_path)
        {
            ImplTakeScreenshot(output_path);
        }

        public void Dispose()
        {
            foreach(var pipeline in pipelines)
            {
                pipeline.Dispose();
            }

            ImplDispose();
        }
    }
}
