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
        public async Task<StoredFileModel> Send(FileTaskModel fileTask)
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                var result = await bus.Rpc.RequestAsync<FileTaskModel, StoredFileModel>(fileTask);
                return result;
            }
        }
    }
}
