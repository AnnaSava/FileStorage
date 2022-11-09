using System;
using System.Collections.Generic;
using System.Text;

namespace Athn.Helpers.MimeType
{
    public class MimeTypeModel
    {
        public string Name { get; }

        public byte?[] Bytes { get; }

        public MimeTypeModel(string name, byte?[] bytes)
        {
            Name = name;
            Bytes = bytes;
        }
    }
}
