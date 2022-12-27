﻿//FILE TRANSFER USING C#.NET SOCKET - SERVER
using FileStorage.Models;
using FileStorage.Services;
using System.Net;
using System.Net.Sockets;
using System.Text;

//https://www.codeproject.com/Articles/24017/File-Transfer-using-Socket-Application-in-C-NET-2
class FTServerCode
{
    IPEndPoint ipEnd;
    Socket sock;

    FileProcessingService _fileProcessingService;

    public FTServerCode(FileProcessingService fileProcessingService)
    {
        ipEnd = new IPEndPoint(IPAddress.Any, 5656);
        //Make IP end point to accept any IP address with port no 5656.
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Here creating new socket object with protocol type and transfer data type
        sock.Bind(ipEnd);
        //Bind end point with newly created socket.

        _fileProcessingService = fileProcessingService;
    }
    public static string receivedPath = "";
    public static string curMsg = "Stopped";
    public async Task StartServer()
    {
        try
        {
            while (true)
            {
                curMsg = "Starting...";
                sock.Listen(100);
                /* That socket object can handle maximum 100 client connection at a time & 
                waiting for new client connection */
                curMsg = "Running and waiting to receive file.";
                Socket clientSock = sock.Accept();
                /* When request comes from client that accept it and return 
                new socket object for handle that client. */
                byte[] clientData = new byte[1024 * 50000];
                int receivedBytesLen = clientSock.Receive(clientData);
                curMsg = "Receiving data...";
                /* I've sent byte array data from client in that format like 
                [file name length in byte][file name] [file data], so need to know 
                first how long the file name is. */
                string fileName = "test" + DateTime.Now.Ticks;
                /* Read file name */

                var model = new StoredFileModel
                {
                    Content = clientData.Take(receivedBytesLen).ToArray(),
                };

                await _fileProcessingService.UploadFilePreventDuplicate(model);

                BinaryWriter bWrite = new BinaryWriter(File.Open(fileName, FileMode.CreateNew));
                /* Make a Binary stream writer to saving the receiving data from client. */
                bWrite.Write(clientData, 0, receivedBytesLen);
                /* Read remain data (which is file content) and 
                save it by using binary writer. */
                curMsg = "Saving file...";
                bWrite.Close();

                // отправляем ответ
                string message = "файл " + fileName + " сохранен";
                var data = Encoding.Unicode.GetBytes(message);
                clientSock.Send(data);

                clientSock.Shutdown(SocketShutdown.Both);
                clientSock.Close();
                /* Close binary writer and client socket */
                curMsg = "Received & Saved file; Server Stopped.";
            }
        }
        catch (Exception ex)
        {
            curMsg = "File Receiving error.";
        }
    }
}