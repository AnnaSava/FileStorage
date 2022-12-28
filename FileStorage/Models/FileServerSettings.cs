using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Models
{
    public class FileServerSettings
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public int ConnectionsCount { get; set; }

        public int MaxFileSize { get; set; }
    }
}
