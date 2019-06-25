using System;
using System.IO;

namespace CastBuilder
{
    public enum TemplateType
    {
        ProjectCsProjFile,
        HelloSceneFile,
        ProgramFile,
        ProjectPropsFile
    }

    public static class ProjectCreator
    {
        private const string CsProjTemplateFileName = "ProjectCsProjFileTemplate.txt";
        private const string HelloSceneTemplateFileName = "ProjectHelloSceneFileTemplate.txt";
        private const string ProgramTemplateFileName = "ProjectProgramFileTemplate.txt";
        private const string ProjPropsTemplateFileName = "ProjectPropertiesFileTemplate.txt";

        public static void Create(string target_dir, string project_name)
        {
            string project_filename = ConvertProjectNameToFileName(project_name);


            if (Directory.Exists(target_dir))
            {
                throw new Exception($"A Project on directory {target_dir} already exists.");
            }

            /* CREATE MAIN FOLDER AND CONTENT FOLDER */

            var dir_info = Directory.CreateDirectory(target_dir);

            dir_info.CreateSubdirectory("Content");

            /* CREATE PROJECT FILES FROM TEMPLATES */

            var program_file_template = LoadTemplate(TemplateType.ProgramFile);
            CreateProgramFile(program_file_template, project_filename, target_dir);

            var hello_scene_file_template = LoadTemplate(TemplateType.HelloSceneFile);
            CreateHelloSceneFile(hello_scene_file_template, project_filename, target_dir);

            var project_props_file_template = LoadTemplate(TemplateType.ProjectPropsFile);
            CreateConfigFile(project_props_file_template, project_name, target_dir);

            var csproj_file_template = LoadTemplate(TemplateType.ProjectCsProjFile);
            CreateCsProjFile(csproj_file_template, project_filename, target_dir);

            /* COPY BASE CONTENT PAK */

            File.Copy(Path.Combine("BaseContent", "Base.pak"), Path.Combine(target_dir, "Content", "Base.pak"));

            /* COPY LIBS */

            File.Copy("CastFramework.dll", Path.Combine(target_dir, "CastFramework.dll"));

            //TODO: Detect Platform to choose what Libries to Copy
            File.Copy(Path.Combine("RuntimeDlls", "SDL2.dll"), Path.Combine(target_dir, "SDL2.dll"));
            File.Copy(Path.Combine("RuntimeDlls", "bgfx.dll"), Path.Combine(target_dir, "bgfx.dll"));

        }

        private static void CreateProgramFile(string template, string namespace_name, string target_dir)
        {
            File.WriteAllText(Path.Combine(target_dir, "Program.cs"), template.Replace("#1", namespace_name));
        }

        private static void CreateHelloSceneFile(string template, string namespace_name, string target_dir)
        {
            File.WriteAllText(Path.Combine(target_dir, "HelloScene.cs"), template.Replace("#1", namespace_name));
        }

        private static void CreateConfigFile(string template, string project_title, string target_dir)
        {
            File.WriteAllText(Path.Combine(target_dir, "config.json"), template.Replace("#1", project_title));
        }

        private static void CreateCsProjFile(string template, string project_file_name, string target_dir)
        {
            File.WriteAllText(Path.Combine(target_dir, project_file_name + ".csproj"), template);
        }

        private static string LoadTemplate(TemplateType type)
        {
            switch(type)
            {
                case TemplateType.ProjectCsProjFile:

                    return File.ReadAllText(Path.Combine("Templates", CsProjTemplateFileName));

                case TemplateType.ProjectPropsFile:

                    return File.ReadAllText(Path.Combine("Templates", ProjPropsTemplateFileName));

                case TemplateType.ProgramFile:

                    return File.ReadAllText(Path.Combine("Templates", ProgramTemplateFileName));

                case TemplateType.HelloSceneFile:

                    return File.ReadAllText(Path.Combine("Templates", HelloSceneTemplateFileName));

                default: return string.Empty;

            }
        }

        private static string ConvertProjectNameToFileName(string project_name)
        {
            char[] array = project_name.ToCharArray();

            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {

                    array[0] = char.ToUpper(array[0]);
                }
            }

            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }

            return new string(array).Replace(" ", string.Empty);
        }
    }
}
