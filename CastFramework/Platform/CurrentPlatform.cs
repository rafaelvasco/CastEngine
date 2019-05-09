using System.Runtime.InteropServices;

namespace CastFramework
{
    public enum OS : byte
    {
        Win, Linux, OSX, Unknown
    }

    public class CurrentPlatform
    {
        public static OS RunningOS { get; }

        static CurrentPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                RunningOS = OS.Win;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                RunningOS = OS.Linux;
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                RunningOS = OS.OSX;
            }
            else
            {
                RunningOS = OS.Unknown;
            }
        }
    }
}
