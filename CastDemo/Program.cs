using CastFramework;
using System;

namespace CastDemo
{
    class Program
    {
        static void Main()
        {
            using (var game = new Game())
            {
                game.Start(new HelloWorld());
            }
        }
    }
}
