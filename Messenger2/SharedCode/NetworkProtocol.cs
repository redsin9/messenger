using System.Net.Sockets;
using System.Text;

namespace SharedCode
{
    static class NetworkProtocol
    {
        public const int DEFAULT_PORT = 11000;
        public const int BUFFER_SIZE = 1024;
        public const string EOF = "<EOF>";
        public const string PASS_PHRASE = "ACS_A3";
        public static readonly Encoding MESSAGE_ENCODE = Encoding.Unicode;



        public static string ReceiveMessage(NetworkStream stream)
        {
            StringBuilder sb = new StringBuilder();
            byte[] buffer = new byte[BUFFER_SIZE];

            while (true)
            {
                int bytesCount = stream.Read(buffer, 0, buffer.Length);
                sb.Append(MESSAGE_ENCODE.GetString(buffer, 0, bytesCount));
                string message = sb.ToString();
                if (message.LastIndexOf(EOF) == message.Length - EOF.Length)
                {
                    return message;
                }
            }
        }



        public static void SendMessage(NetworkStream stream, string message)
        {
            byte[] bytes = MESSAGE_ENCODE.GetBytes(message + EOF);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
