using System;
using System.IO;

namespace CastBuilder
{
    public static class Executor
    {
        private static void PrintUsage()
        {
            ConsoleUtils.ShowInfo("Usage: CastBuilder [action] [params]");
            ConsoleUtils.ShowInfo("Examples:");
            ConsoleUtils.ShowInfo(" ");
            ConsoleUtils.ShowInfo("   [1] Build (Project Root Path)");
            ConsoleUtils.ShowInfo("   [2] Watch (Project Root Path)");
            ConsoleUtils.ShowInfo("   [3] Create (Project Root Path) (Project Name)");
        }

        public static void ExecArgs(string[] args)
        {
            if(args.Length < 2)
            {
                PrintUsage();
                return;
            }

            var action = args[0].ToLower();
            var root_path = args[1];

            switch (action)
            {
                case "build":

                    ExecBuild(root_path);

                    break;

                case "watch": 

                    ExecWatch(root_path);

                    break;

                case "create":

                    if(args.Length < 3)
                    {
                        PrintUsage();
                        return;
                    }

                    var proj_name = args[2];

                    ExecCreate(root_path, proj_name);

                    break;

                default:

                    ConsoleUtils.ShowError($"Invalid Action: {action}");
                    break;
            }
        }

        private static void ExecBuild(string project_root_path)
        {
            if(Directory.Exists(project_root_path))
            {
                try
                {
                    ContentBuilder.Build(project_root_path);
                    ConsoleUtils.ShowSuccess("Project Built Successfully.");
                }
                catch(Exception e)
                {
                    ConsoleUtils.ShowError($"An error ocurred: {e.Message} :: {e.StackTrace}");
                }
            }
            else
            {
                ConsoleUtils.ShowError($"Invalid Path: {project_root_path}");
            }
        }
        
        private static void ExecWatch(string project_root_path)
        {
            if (Directory.Exists(project_root_path))
            {
                try
                {
                    ContentWatcher.Watch(project_root_path);
                }
                catch (Exception e)
                {
                    ConsoleUtils.ShowError($"An error ocurred: {e.Message} :: {e.StackTrace}");
                }
            }
            else
            {
                ConsoleUtils.ShowError($"Invalid Path: {project_root_path}");
            }
        }
        
        private static void ExecCreate(string project_root_path, string proj_name)
        {
            try
            {
                ProjectCreator.Create(project_root_path, proj_name);
                ConsoleUtils.ShowSuccess($"Project {proj_name} created successfuly on {project_root_path}");
            }
            catch (Exception e)
            {
                ConsoleUtils.ShowError($"An error ocurred: {e.Message} :: {e.StackTrace}");
            }
        }
    }
}
