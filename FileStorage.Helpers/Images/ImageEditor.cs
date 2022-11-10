using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
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

            var rect = new Rectangle(left_right, top_bottom, side, side);

            var cropped = smallFile.Clone(ctx => ctx.Crop(rect));

            // TODO учитывать формат картинки
            //cropped.Save(outStream, new PngEncoder());
            cropped.Save(outStream, new JpegEncoder());
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

            // TODO учитывать формат картинки
            resized.Save(outStream, new JpegEncoder());
        }

        public (int, int) GetSize(byte[] content)
        {
            var img = Image.Load(content);
            return (img.Width, img.Height);
        }
    }
}
