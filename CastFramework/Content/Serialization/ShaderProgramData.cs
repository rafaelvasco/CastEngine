using System;

namespace CastFramework
{
    [Serializable]
    public class ShaderProgramData : ResourceData
    {
        public byte[] VertexShader;
        public byte[] FragmentShader;
        public string[] Samplers;
        public string[] Params;

        public override ResourceDataType Type => ResourceDataType.Shader;
    }
}
