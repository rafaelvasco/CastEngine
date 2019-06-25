﻿using CastFramework;

namespace CastDemo
{
    class Program
    {
        static void Main()
        {
            using (var game = new Game())
            {
                game.Start(new GuiDemo());
            }
        }
    }
}
