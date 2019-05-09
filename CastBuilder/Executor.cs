using System;
using System.IO;

namespace CastBuilder
{
    public static class Executor
    {
        private static void PrintUsage()
        {
            Console.WriteLine("Usage: CastBuilder [action] [params]");
            Console.WriteLine("Actions:");
            Console.WriteLine(" [1] Build");
            Console.WriteLine(" ------> [params]: Project Root Path");
        }

        public static void ExecArgs(string[] args)
        {
            if(args.Length < 2)
            {
                PrintUsage();
                return;
            }

            var action = args[0];
            var root_path = args[1];

            switch (action)
            {
                case "Build":

                    ExecBuild(root_path);

                    break;

                case "Watch": 

                    ExecWatch(root_path);

                    break;

                default:

                    Console.WriteLine($"Invalid Action: {action}");
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
                    Console.WriteLine("Project Built Successfully.");
                }
                catch(Exception e)
                {
                    Console.WriteLine($"An error ocurred: {e.Message} :: {e.StackTrace}");
                }
            }
            else
            {
                Console.WriteLine($"Invalid Path: {project_root_path}");
            }
        }
        
        private static void ExecWatch(string project_root_path)
        {
            if (Directory.Exists(project_root_path))
            {
                try
                {
                    ContentReloader.Watch(project_root_path);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error ocurred: {e.Message} :: {e.StackTrace}");
                }
            }
            else
            {
                Console.WriteLine($"Invalid Path: {project_root_path}");
            }
        }
    }
}
