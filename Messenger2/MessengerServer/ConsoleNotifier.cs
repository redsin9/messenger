using System;

namespace MessengerServer
{
    class ConsoleNotifier : INotify
    {
        public void Notify(string notification)
        {
            Console.WriteLine(notification);
        }
    }
}
