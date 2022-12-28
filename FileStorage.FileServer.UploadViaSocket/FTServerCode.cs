using FileStorage.Models;
using FileStorage.Services;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

class FTServerCode
{
    IPEndPoint ipEndPoint;
    Socket socket;
    FileServerSettings _settings;

    FileProcessingService _fileProcessingService;

    public FTServerCode(FileProcessingService fileProcessingService, FileServerSettings settings)
    {
        ipEndPoint = new IPEndPoint(IPAddress.Any, settings.Port);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(ipEndPoint);

        _fileProcessingService = fileProcessingService;
        _settings = settings;
    }

    public async Task StartServer()
    {
        try
        {
            while (true)
            {
                socket.Listen(_settings.ConnectionsCount);
                var client = socket.Accept();

                byte[] clientData = new byte[1024 * _settings.MaxFileSize];
                int receivedBytes = client.Receive(clientData);

               var resultModel = await _fileProcessingService.UploadFilePreventDuplicate(clientData.Take(receivedBytes).ToArray());

                var msg = JsonSerializer.Serialize(resultModel); 

                var data = Encoding.Unicode.GetBytes(msg);
                client.Send(data);

                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
        }
        catch (Exception ex)
        {

        }
    }
}