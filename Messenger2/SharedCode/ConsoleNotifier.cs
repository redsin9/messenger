using System;

namespace SharedCode
{
    class ConsoleNotifier : INotify
    {
        public void Notify(string notification)
        {
            Console.WriteLine(notification);
        }
    }
}
