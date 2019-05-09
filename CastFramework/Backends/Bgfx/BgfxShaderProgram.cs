using System;
using System.Collections.Generic;

namespace CastFramework
{
    public partial class ShaderParameter
    {
        internal Uniform Uniform;

        private void ImplInitialize(string name)
        {
            this.Uniform = new Uniform(name, UniformType.Vector4);
        }
    }

    public partial class ShaderProgram : Resource
    {
        internal Program Program;

        private ShaderParameter[] Parameters;

        private Dictionary<string, int> ParametersMap;

        internal Uniform[] Samplers;

        private void ImplInitialize(byte[] vertex_src, byte[] frag_src, IReadOnlyList<string> samplers, IReadOnlyList<string> @params)
        {
            var vertex_shader = new Shader(MemoryBlock.FromArray(vertex_src));
            var frag_shader = new Shader(MemoryBlock.FromArray(frag_src));

            this.Program = new Program(vertex_shader, frag_shader, true);

            BuildSamplersList(samplers);

            BuildParametersList(@params);
        }

        private void ImplInitialize(string vertex_src, string frag_src, IReadOnlyList<string> samplers, IReadOnlyList<string> @params)
        {
            throw new NotImplementedException();
        }

        private unsafe void ImplSubmitValues()
        {
            for (int i = 0; i < Parameters.Length; ++i)
            {
                var p = Parameters[i];

                if (p.ArrayLength == 0)
                {
                    continue;
                }

                if (p.Constant)
                {
                    if (p.SubmitedOnce)
                    {
                        continue;
                    }
                    else
                    {
                        p.SubmitedOnce = true;
                    }

                }

                var val = p.Value;

                Bgfx.SetUniform(p.Uniform, &val);
            }
        }

        private void ImplDispose()
        {
            for (int i = 0; i < Samplers.Length; ++i)
            {
                Samplers[i].Dispose();
            }

            for (int i = 0; i < Parameters.Length; ++i)
            {
                Parameters[i].Uniform.Dispose();
            }

            Program.Dispose();
        }

        private void BuildSamplersList(IReadOnlyList<string> samplers)
        {
            Samplers = new Uniform[samplers.Count];

            for (int i = 0; i < samplers.Count; ++i)
            {
                Samplers[i] = new Uniform(samplers[i], UniformType.Sampler);
            }
        }

        private void BuildParametersList(IReadOnlyList<string> _params)
        {
            Parameters = new ShaderParameter[_params.Count];
            ParametersMap = new Dictionary<string, int>();

            for (int i = 0; i < _params.Count; ++i)
            {
                Parameters[i] = new ShaderParameter(_params[i]);
                ParametersMap.Add(_params[i], i);
            }
        }
    }
}
