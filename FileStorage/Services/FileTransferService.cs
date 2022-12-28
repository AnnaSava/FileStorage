using FileStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FileStorage.Services
{
    public class FileTransferService
    {
        private readonly FileServerSettings _fileServerSettings;

        public FileTransferService(FileServerSettings settings)
        {
            _fileServerSettings = settings;
        }

        public StoredFileModel Send(byte[] fileContent)
        {
            var ipAddress = Dns.GetHostAddresses(_fileServerSettings.Host);
            var ipEndPoint = new IPEndPoint(ipAddress[0], _fileServerSettings.Port);

            var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            client.Connect(ipEndPoint);

            client.Send(fileContent);

            var responseData = new byte[256];
            var sb = new StringBuilder();
            int bytes;

            do
            {
                bytes = client.Receive(responseData, responseData.Length, 0);
                sb.Append(Encoding.Unicode.GetString(responseData, 0, bytes));
            }
            while (client.Available > 0);

            var model = JsonSerializer.Deserialize<StoredFileModel>(sb.ToString());

            client.Shutdown(SocketShutdown.Both);
            client.Close();

            return model;
        }
    }
}
