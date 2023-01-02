using EasyNetQ;
using FileStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FileStorage.Services
{
    public class FileEasyNetQueueService
    {
        public void Publish(FileTaskModel fileTask)
        {
            //try
            //{
            //    var b = RabbitHutch.CreateBus("host=localhost;port=8080");
            //}
            //catch (Exception ex)
            //{
            //    var t = ex.Message;
            //}

            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                bus.PubSub.Publish(fileTask);
            }
        }
    }
}
