using System.Collections.Generic;
using System.Numerics;

namespace CastFramework
{
    public partial class ShaderParameter
    {
        internal int ArrayLength;

        public bool Constant;

        internal bool SubmitedOnce;

        public Vector4 Value => _value;

        private Vector4 _value;

        internal ShaderParameter(string name)
        {
            this.ImplInitialize(name);
        }

        public void SetValue(float v)
        {
            ArrayLength = 1;
            _value.X = v;
        }

        public void SetValue(Vector2 v)
        {
            ArrayLength = 2;

            _value.X = v.X;
            _value.Y = v.Y;
        }

        public void SetValue(Vector3 v)
        {
            ArrayLength = 3;

            _value.X = v.X;
            _value.Y = v.Y;
            _value.Z = v.Z;
        }

        public void SetValue(Vector4 v)
        {
            ArrayLength = 4;
            _value = v;
        }

        public void SetValue(Color color)
        {
            ArrayLength = 4;

            _value.X = color.Rf;
            _value.Y = color.Gf;
            _value.Z = color.Bf;
            _value.W = color.Af;
        }
    }

    public partial class ShaderProgram : Resource
    {
        internal ShaderProgram(byte[] vertex_src, byte[] frag_src, IReadOnlyList<string> samplers, IReadOnlyList<string> @params)
        {
            this.ImplInitialize(vertex_src, frag_src, samplers, @params);
        }

        internal ShaderProgram(string vertex_src, string frag_src, IReadOnlyList<string> samplers, IReadOnlyList<string> @params)
        {
            this.ImplInitialize(vertex_src, frag_src, samplers, @params);
        }

        public ShaderParameter GetParameter(string name)
        {
            if (ParametersMap.TryGetValue(name, out var index))
            {
                return Parameters[index];
            }

            return null;
        }

        internal unsafe void SubmitValues()
        {
            this.ImplSubmitValues();
        }

        internal override void Dispose()
        {
            this.ImplDispose();
        }
    }
}
