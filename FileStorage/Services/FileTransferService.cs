using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Services
{
    public class FileTransferService
    {
        public void Send(byte[] fileContent)
        {
            IPAddress[] ipAddress = Dns.GetHostAddresses("localhost");
            IPEndPoint ipEnd = new IPEndPoint(ipAddress[0], 5656);
            /* Make IP end point same as Server. */
            Socket clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            clientSock.Connect(ipEnd);

            clientSock.Send(fileContent);

            // получаем ответ
            var data = new byte[256]; // буфер для ответа
            StringBuilder builder = new StringBuilder();
            int bytes = 0; // количество полученных байт

            do
            {
                bytes = clientSock.Receive(data, data.Length, 0);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (clientSock.Available > 0);
            Console.WriteLine("ответ сервера: " + builder.ToString());

            // закрываем сокет
            clientSock.Shutdown(SocketShutdown.Both);
            clientSock.Close();

        }
    }
}
