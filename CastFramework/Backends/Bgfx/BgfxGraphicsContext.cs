using System;

namespace CastFramework
{
    public partial class GraphicsContext
    {
        private void ImplInitialize(IntPtr graphics_surface_ptr, int width, int height) 
        {

            Bgfx.SetPlatformData(new PlatformData
            {
                WindowHandle = graphics_surface_ptr
            });

            var bgfx_callback_handler = new BgfxCallbackHandler();

            var settings = new InitSettings
            {
                Backend = RendererBackend.Default,
                ResetFlags = ResetFlags.Vsync,
                Width = width,
                Height = height,
                CallbackHandler = bgfx_callback_handler
            };

            Bgfx.Init(settings);


            var caps = Bgfx.GetCaps();

            GraphicsBackend gfx_backend = GraphicsBackend.OpenGL;

            switch(caps.Backend)
            {
                case RendererBackend.OpenGL : gfx_backend = GraphicsBackend.OpenGL; break;
                case RendererBackend.Direct3D11:
                case RendererBackend.Direct3D12:
                case RendererBackend.Direct3D9:
                    gfx_backend = GraphicsBackend.OpenGL; break;
            }

            Info = new GraphicsInfo(gfx_backend, caps.MaxTextureSize);
        }

        private void ImplSetClearColor(byte render_pass, Color color)
        {
            Bgfx.SetViewClear(render_pass, ClearTargets.Color, color);
        }

        private void ImplSwapBuffers()
        {
            Bgfx.Frame();
        }

        private void ImplResizeBackbuffer(int width, int height)
        {
            Bgfx.Reset(width, height, ResetFlags.Vsync);
        }

        private void ImplSetRenderTarget(byte render_pass, RenderTarget target)
        {
            Bgfx.SetViewFrameBuffer(render_pass, target?.FrameBuffer ?? FrameBuffer.Invalid);
            Bgfx.Touch(render_pass);
        }

        private void ImplSetViewport(byte render_pass, int x, int y, int w, int h)
        {
            Bgfx.SetViewRect(render_pass, x, y, w, h);
        }

        private void ImplSetScissor(byte render_pass, int x, int y, int w, int h)
        {
            Bgfx.SetViewScissor(render_pass, x, y, w, h);
        }

        private unsafe void ImplSetProjectionMatrix(byte render_pass, float* matrix)
        {
            Bgfx.SetViewTransform(render_pass, null, matrix);
        }

        private void ImplTakeScreenshot(string output_path)
        {
            Bgfx.RequestScreenShot(output_path);
        }

        private void ImplDispose()
        {
            Bgfx.Shutdown();
        }

        private class BgfxCallbackHandler : ICallbackHandler
        {
            public void ReportError(string fileName, int line, ErrorType errorType, string message)
            {
            }

            public void ReportDebug(string fileName, int line, string format, IntPtr args)
            {
            }

            public void ProfilerBegin(string name, int color, string filePath, int line)
            {
            }

            public void ProfilerEnd()
            {
            }

            public int GetCachedSize(long id)
            {
                return 0;
            }

            public bool GetCacheEntry(long id, IntPtr data, int size)
            {
                return false;
            }

            public void SetCacheEntry(long id, IntPtr data, int size)
            {
            }

            public void SaveScreenShot(string path, int width, int height, int pitch, IntPtr data, int size,
                bool flipVertical)
            {
                var pixmap = new Pixmap(data, width, height);

                pixmap.SaveToFile(path);

                pixmap.Dispose();
            }

            public void CaptureStarted(int width, int height, int pitch, TextureFormat format, bool flipVertical)
            {
            }

            public void CaptureFinished()
            {
            }

            public void CaptureFrame(IntPtr data, int size)
            {
            }
        }
    }
    
}
