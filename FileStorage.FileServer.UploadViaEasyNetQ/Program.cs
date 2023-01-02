// See https://aka.ms/new-console-template for more information
using EasyNetQ;
using FileStorage.Models;

Console.WriteLine("Hello, World!");

using (var bus = RabbitHutch.CreateBus("host=localhost"))
{
    bus.PubSub.Subscribe<FileTaskModel>("test", HandleTextMessage);
    Console.WriteLine("Listening for messages. Hit <return> to quit.");
    Console.ReadLine();
}

static void HandleTextMessage(FileTaskModel textMessage)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Got message: {0}", textMessage.Content);
    Console.ResetColor();
}