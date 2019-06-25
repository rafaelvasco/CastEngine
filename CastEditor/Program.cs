using CastFramework;

namespace CastEditor
{
    class Program
    {
        static void Main()
        {
            using (var app = new Game())
            {
                app.Start(new Editor());
            }
        }
    }
}
