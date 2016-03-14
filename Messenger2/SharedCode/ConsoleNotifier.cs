using System;

namespace SharedCode
{
    class ConsoleNotifier : ILogger
    {
        public void Log(string notification)
        {
            Console.WriteLine(notification);
        }
    }
}
