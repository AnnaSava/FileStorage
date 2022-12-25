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

            clientSock.Close();

        }
    }
}
