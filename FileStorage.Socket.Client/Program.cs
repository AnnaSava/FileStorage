// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Hello, client! Press any key to send file");

Console.ReadKey();

FTClientCode.SendFile("test.txt");

//// Establish the local endpoint for the socket.
//IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
//IPAddress ipAddr = ipHost.AddressList[0];
////IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 8005);

//IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8005);

//// Create a TCP socket.
//Socket client = new Socket(AddressFamily.InterNetwork,
//        SocketType.Stream, ProtocolType.Tcp);

//// Connect the socket to the remote endpoint.
//client.Connect(ipEndPoint);

//// There is a text file test.txt located in the app root directory.
//string fileName = "test.txt";

//// Send file fileName to remote device
//Console.WriteLine("Sending {0} to the host.", fileName);
//client.SendFile(fileName);

////byte[] data = Encoding.Unicode.GetBytes("1234");
////client.Send(data);


////// получаем ответ
////data = new byte[256]; // буфер для ответа
////StringBuilder builder = new StringBuilder();
////int bytes = 0; // количество полученных байт

////do
////{
////    bytes = client.Receive(data, data.Length, 0);
////    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
////}
////while (client.Available > 0);
////Console.WriteLine("ответ сервера: " + builder.ToString());

//// Release the socket.
//client.Shutdown(SocketShutdown.Both);
//client.Close();