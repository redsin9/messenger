using System;

namespace MessengerServer
{
    class ConsoleNotifier : Notifier
    {
        public void Notify(string notification)
        {
            Console.WriteLine(notification);
        }
    }
}
