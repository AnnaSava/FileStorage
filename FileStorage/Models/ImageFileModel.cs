using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Models
{
    public class ImageFileModel
    {
        public string FileId { get; set; }

        public string Type { get; set; } // Web, FullSize, Thumb etc.

        public int Width { get; set; }

        public int Height { get; set; }
    }
}
