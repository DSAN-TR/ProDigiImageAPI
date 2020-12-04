using ProDigiAPI.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ProDigiAPI.Infrastructure
{
    public static class ImageProcess
    {
        //wishImageWidth and wishImageHeight can be decide automatically.
        public static Color ColorDetector(string ImagePath, int wishImageWidth = 100, int wishImageHeight = 100)
        {
            Bitmap sourceImage = null;
            try
            {
                sourceImage = new Bitmap(ImagePath);
                return ColorDetector(sourceImage, wishImageWidth, wishImageHeight);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (sourceImage != null)
                {
                    sourceImage.Dispose();
                }
            }
        }

        public static Color ColorDetector(Stream ImageStream, int wishImageWidth = 100, int wishImageHeight = 100)
        {
            Bitmap sourceImage = null;
            try
            {
                sourceImage = new Bitmap(ImageStream);
                return ColorDetector(sourceImage, wishImageWidth, wishImageHeight);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (sourceImage != null)
                {
                    sourceImage.Dispose();
                }
            }
        }

        private static Color ColorDetector(Bitmap sourceImage, int wishImageWidth, int wishImageHeight)
        {

            int destImageWidth = 0;
            int destImageHeight = 0;

            // Calculate factor of image
            double faktor = (double)sourceImage.Width / (double)sourceImage.Height;

            if (faktor >= 1.0) // Landscape
            {
                destImageWidth = wishImageWidth;
                destImageHeight = (int)(destImageWidth / faktor);
            }
            else // Port
            {
                destImageHeight = wishImageHeight;
                destImageWidth = (int)(destImageHeight * faktor);
            }

            double[][] imageRGBData = new double[destImageWidth * destImageHeight][];

            Bitmap thumbBmp = sourceImage;

            //for testing
            //new Bitmap(sourceImage.GetThumbnailImage(
            //    destImageWidth, destImageHeight, () =>
            //    {
            //        return false;
            //    }, IntPtr.Zero));

            //for testing
            //thumbBmp.Save(@"C:\Users\denis.shanov\source\repos\ProDigiAPI\ProDigiAPI\Downloads\ProcessedImage.png");

            //We are using for checking colors  of specific area in image.
            BitmapData data = thumbBmp.LockBits(new Rectangle(0, 0, destImageWidth, destImageHeight), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                byte[] pixelData = new Byte[data.Stride];
                for (int scanline = 0; scanline < data.Height; scanline++)
                {
                    Marshal.Copy(data.Scan0 + (scanline * data.Stride), pixelData, 0, data.Stride);
                    for (int pixeloffset = 0; pixeloffset < data.Width; pixeloffset++)
                    {
                        imageRGBData[0 + (scanline * destImageWidth) + pixeloffset] = new[] { (double)pixelData[pixeloffset * 3 + 2], (double)pixelData[pixeloffset * 3 + 1], (double)pixelData[pixeloffset * 3] };
                    }
                }

                //Here I'm using K-Mean algorithm(K - nearest neighbors algorithm). 
                //Because There is too many color pixel and I want to detect most intensive color into image.
                //Still open to improvement
                int[] ColorsCluster = KMeanImageColorsClustering.Cluster(imageRGBData, 3);
                var MainThreeColorGroup = ColorsCluster.GroupBy(x => x).Select(x => new { x.Key, c = x.Count() }).OrderByDescending(x => x.c);
                int IntenseColorGroup = MainThreeColorGroup.First().Key;

                if (MainThreeColorGroup.Count() > 0)
                {
                    int Index = Array.IndexOf(ColorsCluster, IntenseColorGroup);
                    var IntenseColor = imageRGBData[Index];

                    return Color.FromArgb((int)IntenseColor[0], (int)IntenseColor[1], (int)IntenseColor[2]);
                }

                return Color.Transparent;
            }
            catch (Exception ex)
            {
                return Color.Transparent;
            }
            finally
            {
                thumbBmp.UnlockBits(data);
                sourceImage.Dispose();
            }
        }

        public static ColorModel GetColor(Color color)
        {
            //Extra code can add to get approximate color or etc. 
            ColorModel ProDigiColor = ColorsInMemoryData.ColorsData.Where(x => x.R == color.R && x.G == color.G && x.B == color.B).FirstOrDefault();

            if (ProDigiColor == null)
            {
                ProDigiColor = new ColorModel()
                {
                    KnownColor = false,
                    Name = "Unknown Color",
                    R = color.R,
                    G = color.G,
                    B = color.B,
                };
            }
            return ProDigiColor;

        }
    }
}
