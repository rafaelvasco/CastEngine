using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CastFramework
{
    public struct ShaderBuildResult
    {
        public readonly byte[] VsBytes;
        public readonly byte[] FsBytes;
        public readonly string[] Samplers;
        public readonly string[] Params;

        public ShaderBuildResult(byte[] vs_bytes, byte[] fs_bytes, string[] samplers, string[] _params)
        {
            this.VsBytes = vs_bytes;
            this.FsBytes = fs_bytes;
            this.Samplers = samplers;
            this.Params = _params;
        }
    }

    

    public static class ShaderBuilder
    {
        private const string COMPILER_FILE_NAME = "shaderc.exe";
        private const string INCLUDE_FILE_1 = "bgfx_shader.sh";
        private const string INCLUDE_FILE_2 = "common.sh";
        private const string INCLUDE_FILE_3 = "shaderlib.sh";
        private const string SAMPLER_REGEX_VAR = "sampler";
        private const string SAMPLER_REGEX = @"SAMPLER2D\s*\(\s*(?<sampler>\w+)\s*\,\s*(?<index>\d+)\s*\)\s*\;";
        private const string PARAM_REGEX_VAR = "param";
        private const string VEC_PARAM_REGEX = @"uniform\s+vec4\s+(?<param>\w+)\s*\;";

        private static string ExtractShaderCompiler()
        {
            string temp_path = new Uri(Path.Combine(Path.GetTempPath(), COMPILER_FILE_NAME)).LocalPath;

            ExtractResource("CastFramework.Content.ShaderBuilder." + COMPILER_FILE_NAME, temp_path);

            return temp_path;
        }

        private static string ExtractShaderCompilerIncludeDir()
        {
            var temp_include_path = new Uri(Path.Combine(Path.GetTempPath(), "include")).LocalPath;

            Directory.CreateDirectory(temp_include_path);

            var temp_path1 = new Uri(Path.Combine(temp_include_path,  INCLUDE_FILE_1)).LocalPath;
            var temp_path2 = new Uri(Path.Combine(temp_include_path, INCLUDE_FILE_2)).LocalPath;
            var temp_path3 = new Uri(Path.Combine(temp_include_path, INCLUDE_FILE_3)).LocalPath;

            ExtractResource("CastFramework.Content.ShaderBuilder.shader_includes." + INCLUDE_FILE_1, temp_path1);
            ExtractResource("CastFramework.Content.ShaderBuilder.shader_includes." + INCLUDE_FILE_2, temp_path2);
            ExtractResource("CastFramework.Content.ShaderBuilder.shader_includes." + INCLUDE_FILE_3, temp_path3);

            return temp_include_path;
        }

        private static void ExtractResource(string resource, string path)
        {

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);

            byte[] bytes = new byte[stream.Length];

            stream.Read(bytes, 0, bytes.Length);

            File.WriteAllBytes(path, bytes);

        }

        public static ShaderBuildResult Build(string vs_src_path, string fs_src_path)
        {
            

            var directory = Path.GetDirectoryName(vs_src_path);

            string temp_vs_bin_output = string.Empty;
            string temp_fs_bin_output = string.Empty;

            string vs_build_result = string.Empty;
            string fs_build_result = string.Empty;

            // Extract Shader Exe and Build Resources

            string temp_compiler_path = string.Empty;
            string temp_include_dir = string.Empty;

            void DeleteTempIncludeDirectory(string path)
            {
                var di = new DirectoryInfo(path);

                foreach(var file in di.GetFiles())
                {
                    file.Delete();
                }

                Directory.Delete(path);
            }

            void DeleteTempCompilerFiles()
            {
                if (!string.IsNullOrEmpty(temp_compiler_path))
                {
                    File.Delete(temp_compiler_path);
                }

                if (!string.IsNullOrEmpty(temp_include_dir))
                {
                    DeleteTempIncludeDirectory(temp_include_dir);
                }
            }

            try
            {
                temp_compiler_path = ExtractShaderCompiler();
                temp_include_dir = ExtractShaderCompilerIncludeDir();
            }
            catch(Exception)
            {
                DeleteTempCompilerFiles();
            }


            var process_info = new ProcessStartInfo
            {
                UseShellExecute = false,
                FileName = temp_compiler_path
            };

            Process proc_vs;
            Process proc_fs;

            try
            {
                var vs_args = "--platform windows -p vs_4_0 -O 3 --type vertex -f $path -o $output -i $include";

                vs_args = vs_args.Replace("$path", vs_src_path);

                temp_vs_bin_output = new Uri(Path.Combine(directory, "dx_" + Path.GetFileName(vs_src_path))).LocalPath;

                vs_args = vs_args.Replace("$output", temp_vs_bin_output);

                vs_args = vs_args.Replace("$include", temp_include_dir);

                process_info.Arguments = vs_args;

                proc_vs = Process.Start(process_info);

                proc_vs?.WaitForExit();

                var output = proc_vs?.ExitCode ?? -1;

                if (output != 0 && output != -1)
                {
                    using (var reader = proc_vs?.StandardError)
                    {
                        vs_build_result = reader?.ReadToEnd();

                    }
                }

            }
            catch (Exception) { }

            try
            {
                var fs_args = "--platform windows -p ps_4_0 -O 3 --type fragment -f $path -o $output -i $include";

                fs_args = fs_args.Replace("$path", fs_src_path);

                temp_fs_bin_output = new Uri(Path.Combine(directory, "dx_" + Path.GetFileName(fs_src_path))).LocalPath;

                fs_args = fs_args.Replace("$output", temp_fs_bin_output);

                fs_args = fs_args.Replace("$include", temp_include_dir);

                process_info.Arguments = fs_args;

                proc_fs = Process.Start(process_info);

                proc_fs?.WaitForExit();

                var output = proc_fs?.ExitCode ?? -1;

                if (output != 0 && output != -1)
                {
                    using (var reader = proc_fs?.StandardError)
                    {
                        fs_build_result = reader?.ReadToEnd();

                    }
                }
            }
            catch (Exception) { }

            bool vs_ok = File.Exists(temp_vs_bin_output);
            bool fs_ok = File.Exists(temp_fs_bin_output);

            if (vs_ok && fs_ok)
            {
                var vs_bytes = File.ReadAllBytes(temp_vs_bin_output);
                var fs_bytes = File.ReadAllBytes(temp_fs_bin_output);

                var fs_stream = File.OpenRead(fs_src_path);

                ParseUniforms(fs_stream, out var samplers, out var _params);

                var result = new ShaderBuildResult(vs_bytes, fs_bytes, samplers, _params);

                File.Delete(temp_vs_bin_output);
                File.Delete(temp_fs_bin_output);

                DeleteTempCompilerFiles();

                return result;
            }
            else
            {
                if (vs_ok)
                {
                    File.Delete(temp_vs_bin_output);
                }

                if (fs_ok)
                {
                    File.Delete(temp_fs_bin_output);
                }

                DeleteTempCompilerFiles();

                if (!vs_ok)
                {
                    throw new Exception("Error building vertex shader on " + vs_src_path + " : " + vs_build_result);
                }

                throw new Exception("Error building fragment shader on " + fs_src_path + " : " + fs_build_result);
            }
        }

        public static void ParseUniforms(Stream fs_stream, out string[] samplers, out string[] _params)
        {
            string line;

            Regex sampler_regex = new Regex(SAMPLER_REGEX);
            Regex param_regex = new Regex(VEC_PARAM_REGEX);

            var samplers_list = new List<string>();
            var params_list = new List<string>();

            using (var reader = new StreamReader(fs_stream))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    Match sampler_match = sampler_regex.Match(line);


                    if (sampler_match.Success)
                    {
                        string sampler_name = sampler_match.Groups[SAMPLER_REGEX_VAR].Value;
                        samplers_list.Add(sampler_name);
                    }
                    else
                    {
                        Match param_match = param_regex.Match(line);

                        if (param_match.Success)
                        {
                            string param_name = param_match.Groups[PARAM_REGEX_VAR].Value;

                            params_list.Add(param_name);
                        }
                    }
                }
            }

            samplers = samplers_list.Count > 0 ? samplers_list.ToArray() : new string[] { };

            _params = params_list.Count > 0 ? params_list.ToArray() : new string[] { };

        }
    }
}
