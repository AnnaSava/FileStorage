using FileStorage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Athn.Tools.FilesToDb
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
