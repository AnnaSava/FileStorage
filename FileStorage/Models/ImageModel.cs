using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Models
{
    public class ImageModel
    {
        public string Id { get; set; }

        public string PreviewId { get; set; } // Инфа дублируется с айтемом из Files

        public List<ImageFileModel> Files { get; set; }
    }
}
