using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD = System.Drawing;

namespace FileStorage.Helpers.Images
{
    public class ImageEditor
    {
        public void SquareCrop(byte[] content, Stream outStream)
        {
            var smallFile = Image.Load(content);

            var side = smallFile.Width > smallFile.Height ? smallFile.Height : smallFile.Width;

            var top_bottom = (smallFile.Height - side) / 2;
            var left_right = (smallFile.Width - side) / 2;

            //smallFile.Crop(top_bottom, left_right, top_bottom, left_right)
            //    .Resize(201, 201, true, true)
            //    .Crop(1, 1);

            //return smallFile.GetBytes();

            var cropped = smallFile.Clone(ctx => ctx.Crop(100, 50));

            cropped.Save(outStream, new PngEncoder());
        }

        public void Resize(byte[] content, Stream outStream)
        {
            var smallFile = Image.Load(content);

            var side = smallFile.Width > smallFile.Height ? smallFile.Height : smallFile.Width;

            var top_bottom = (smallFile.Height - side) / 2;
            var left_right = (smallFile.Width - side) / 2;

            //smallFile.Crop(top_bottom, left_right, top_bottom, left_right)
            //    .Resize(201, 201, true, true)
            //    .Crop(1, 1);

            //return smallFile.GetBytes();

            var resized = smallFile.Clone(ctx => ctx.Resize(smallFile.Width / 2, smallFile.Height / 2));

            resized.Save(outStream, new PngEncoder());
        }
    }
}
