﻿using System;
using System.Numerics;

namespace CastFramework
{
    public partial class RenderPipeline : IDisposable
    {
        public RenderPipeline(GraphicsContext gfx, int max_vertex_count, Rect render_area)
        {
            this.render_surfaces = new RenderSurface[5];

            AddSurface(render_area, "MainSurface");

            this.max_vertex_count = max_vertex_count;

            vertices = new Vertex2D[max_vertex_count];

            indices = new ushort[max_vertex_count / 4 * 6];

            ushort indice_i = 0;

            for (var i = 0; i < indices.Length; i += 6, indice_i += 4)
            {
                indices[i + 0] = (ushort)(indice_i + 0);
                indices[i + 1] = (ushort)(indice_i + 1);
                indices[i + 2] = (ushort)(indice_i + 2);
                indices[i + 3] = (ushort)(indice_i + 0);
                indices[i + 4] = (ushort)(indice_i + 2);
                indices[i + 5] = (ushort)(indice_i + 3);
            }

            this.gfx = gfx;

            ImplInitialize();
        }

        public void Reset()
        {
            current_render_pass = 0;
            max_render_pass = 0;
        }

        public void SetBlendMode(BlendMode blend)
        {
            if(current_blend_mode == blend)
            {
                return;
            }

            if (vertex_index > 0)
            {
                Submit();
            }

            current_blend_mode = blend;

            ImplSetBlendMode(blend);
        }

        public void SetShaderProgram(ShaderProgram shader)
        {
            if(current_shader_program == shader)
            {
                return;
            }

            if(vertex_index > 0)
            {
                Submit();
            }

            this.current_shader_program = shader;

            ImplSetShaderProgram(shader);
        }

        public void SetRenderSurface(RenderSurface surface)
        {
            ImplSetRenderSurface(surface);
        }

        public void SetScissor(int x, int y, int w, int h)
        {
            ImplSetScissor(x, y, w, h);
        }

        public RenderSurface AddSurface(Rect area, string name = "Surface")
        {
            var surface = new RenderSurface(area);
            surface.Name = name;

            render_surfaces[overlay_surface_idx++] = surface;

            return surface;
        }

        public unsafe void DrawSurfaces()
        {
            for(var i = 0; i < overlay_surface_idx; ++i)
            {
                ImplDrawSurface(render_surfaces[i], i+1);
            }
        }

        public void ResizeSurfaces(int width, int height)
        {
            for (var i = 0; i < overlay_surface_idx; ++i)
            {
                render_surfaces[i].ResizeSurface(width, height);
            }
        }

        public void SetSurfaceAreas(Rect area)
        {
            for (var i = 0; i < overlay_surface_idx; ++i)
            {
                render_surfaces[i].SetArea(area);
            }
        }

        public void SetScreenProjection(Matrix4x4 mat)
        {
            this.screen_projection = mat;
        }

        public unsafe void PushQuads(Texture2D texture, Quad[] quads, int length)
        {
            if (vertex_index >= max_vertex_count ||
                current_texture != texture || current_blend_mode != quads[0].Blend)
            {
                Submit();

                if (current_blend_mode != quads[0].Blend)
                {
                    SetBlendMode(quads[0].Blend);
                }

                current_texture = texture;
            }

            var vidx = vertex_index;

            for(var i = 0; i < length; ++i)
            {
                var qv = quads[i];

                ref var v0 = ref qv.V0;
                ref var v1 = ref qv.V1;
                ref var v2 = ref qv.V2;
                ref var v3 = ref qv.V3;

                fixed (Vertex2D* vertex_ptr = vertices)
                {
                    *(vertex_ptr + vidx++) = new Vertex2D(v0.X, v0.Y, v0.Tx, v0.Ty, v0.Col);
                    *(vertex_ptr + vidx++) = new Vertex2D(v1.X, v1.Y, v1.Tx, v1.Ty, v1.Col);
                    *(vertex_ptr + vidx++) = new Vertex2D(v2.X, v2.Y, v2.Tx, v2.Ty, v2.Col);
                    *(vertex_ptr + vidx++) = new Vertex2D(v3.X, v3.Y, v3.Tx, v3.Ty, v3.Col);
                }
            }

            unchecked
            {
                vertex_index += length * 4;
            }
        }

        public unsafe void PushQuad(Texture2D texture, ref Quad quad)
        {
            if (vertex_index >= max_vertex_count ||
                current_texture != texture || current_blend_mode != quad.Blend)
            {
                Submit();

                if (current_blend_mode != quad.Blend)
                {
                    SetBlendMode(quad.Blend);
                }

                current_texture = texture;
            }

            var vidx = vertex_index;
            var qv = quad;

            ref var v0 = ref qv.V0;
            ref var v1 = ref qv.V1;
            ref var v2 = ref qv.V2;
            ref var v3 = ref qv.V3;

            fixed (Vertex2D* vertex_ptr = vertices)
            {
                *(vertex_ptr + vidx++) = new Vertex2D(v0.X, v0.Y, v0.Tx, v0.Ty, v0.Col);
                *(vertex_ptr + vidx++) = new Vertex2D(v1.X, v1.Y, v1.Tx, v1.Ty, v1.Col);
                *(vertex_ptr + vidx++) = new Vertex2D(v2.X, v2.Y, v2.Tx, v2.Ty, v2.Col);
                *(vertex_ptr + vidx) = new Vertex2D(v3.X, v3.Y, v3.Tx, v3.Ty, v3.Col);
            }

            unchecked
            {
                vertex_index += 4;
            }
        }

        public void Submit()
        {
            if(vertex_index == 0)
            {
                return;
            }

            ImplSubmit();

            vertex_index = 0;
        }

        public void Dispose()
        {
            ImplDispose();
        }

        //private RenderSurface main_render_surface;

        private RenderSurface[] render_surfaces;

        private int overlay_surface_idx;

        private Vertex2D[] vertices;

        private ushort[] indices;

        private int vertex_index;

        private int max_vertex_count;

        private Texture2D current_texture;

        private BlendMode current_blend_mode;

        private ShaderProgram current_shader_program;

        private GraphicsContext gfx;

        private Matrix4x4 screen_projection;

    }
}
