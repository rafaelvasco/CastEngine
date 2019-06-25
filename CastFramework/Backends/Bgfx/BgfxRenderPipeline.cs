using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace CastFramework
{
    public partial class RenderPipeline : IDisposable
    {
        private static readonly VertexLayout Vertex2DLayout = new VertexLayout()
          .Begin()
          .Add(VertexAttributeUsage.Position, 2, VertexAttributeType.Float)
          .Add(VertexAttributeUsage.TexCoord0, 2, VertexAttributeType.Float)
          .Add(VertexAttributeUsage.Color0, 4, VertexAttributeType.UInt8, true)
          .End();

        private RenderState base_render_state;

        private RenderState blending_state;

        private RenderState render_state;

        private IndexBuffer index_buffer;

        private byte current_render_pass;

        private byte max_render_pass;

        private void ImplInitialize()
        {
            index_buffer = new IndexBuffer(MemoryBlock.FromArray(indices));

            base_render_state = RenderState.WriteRGB | RenderState.WriteA;

            blending_state = RenderState.BlendFunction(RenderState.BlendSourceAlpha,
                                       RenderState.BlendInverseSourceAlpha);

            render_state = base_render_state | blending_state;
        }

        private void ImplDispose()
        {
            index_buffer.Dispose();
        }

        private void ImplSetBlendMode(BlendMode blend)
        {
            switch (blend)
            {
                case BlendMode.AlphaBlend:

                    blending_state = 
                                   RenderState.BlendFunction(RenderState.BlendSourceAlpha,
                                       RenderState.BlendInverseSourceAlpha);
                    break;

                case BlendMode.AlphaAdd:

                    blending_state = RenderState.BlendFunction(RenderState.BlendSourceAlpha, RenderState.BlendOne);
                    break;

                case BlendMode.ColorMul:

                    blending_state = RenderState.BlendMultiply;
                    break;


                case BlendMode.None:

                    blending_state = RenderState.BlendNormal; //TODO:
                    break;
            }

            render_state = base_render_state | blending_state;
        }

        private void ImplSetShaderProgram(ShaderProgram shader)
        {
            
        }

        private unsafe void ImplSetRenderSurface(RenderSurface surface)
        {
            if(vertex_index > 0)
            {
                Submit();
            }

            if (surface != null)
            {
                current_render_pass++;

                if (current_render_pass > max_render_pass)
                {
                    max_render_pass = current_render_pass;
                }
            }
            else
            {
                surface = render_surfaces[0];
                current_render_pass = 0;
            }

            Matrix4x4 projection = surface.Projection;

            gfx.SetRenderTarget(current_render_pass, surface.RenderTarget);
            gfx.SetClearColor(current_render_pass, surface != render_surfaces[0] ? Color.Transparent : Color.Black);
            gfx.SetViewport(current_render_pass, 0, 0, surface.Width, surface.Height);
            gfx.SetProjection(current_render_pass, &projection.M11);
        }

        private unsafe void ImplDrawSurface(RenderSurface surface, int index)
        {
            var vertex_buffer = new TransientVertexBuffer(4, Vertex2DLayout);

            fixed (void* v = surface.Vertices)
            {
                Unsafe.CopyBlock((void*)vertex_buffer.Data, v, (uint)(4 * Vertex2D.Stride));
            }

            var pass = (byte)(max_render_pass + index + 1);

            gfx.SetViewport(pass, 0, 0, Game.Instance.ScreenSize.W, Game.Instance.ScreenSize.H);

            var proj = screen_projection;

            gfx.SetProjection(pass, &proj.M11);

            //gfx.SetClearColor(pass, Color.Blue);

            Bgfx.SetTexture(0, current_shader_program.Samplers[0], surface.RenderTarget.NativeTexture, TextureFlags.FilterPoint | TextureFlags.ClampUVW);

            Bgfx.SetRenderState(render_state);

            Bgfx.SetIndexBuffer(index_buffer, 0, 6);

            Bgfx.SetVertexBuffer(0, vertex_buffer, 0, vertex_index);

            Bgfx.Submit(pass, surface.Shader?.Program ?? current_shader_program.Program);
        }

        private void ImplSetScissor(int x, int y, int w, int h)
        {
            gfx.SetScissor(current_render_pass, x, y, w, h);
        }

        private unsafe void ImplSubmit()
        {
            var vertex_buffer = new TransientVertexBuffer(vertex_index, Vertex2DLayout);

            fixed (void* v = vertices)
            {
                Unsafe.CopyBlock((void*)vertex_buffer.Data, v, (uint)(vertex_index * Vertex2D.Stride));
            }

            Bgfx.SetTexture(0, current_shader_program.Samplers[0], current_texture.Texture, current_texture.TexFlags);

            Bgfx.SetRenderState(render_state);

            Bgfx.SetIndexBuffer(index_buffer, 0, vertex_index / 4 * 6);

            Bgfx.SetVertexBuffer(0, vertex_buffer, 0, vertex_index);

            current_shader_program.SubmitValues();

            Bgfx.SetViewMode(current_render_pass, ViewMode.Sequential); 

            Bgfx.Submit(current_render_pass, current_shader_program.Program);

        }
    }
}
