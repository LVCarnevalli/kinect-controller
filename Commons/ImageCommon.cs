// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Microsoft.Kinect;

namespace Commons
{
    internal static class ImageCommon
    {
        public const int RedIndex = 2;
        public const int GreenIndex = 1;
        public const int BlueIndex = 0;

        const float MaxDepthDistance = 4000;
        const float MinDepthDistance = 800;
        const float MaxDepthDistanceOffset = MaxDepthDistance - MinDepthDistance;

        public static short[] ToDepthArray(this DepthImageFrame image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            var width = image.Width;
            var height = image.Height;

            short[] rawDepthData = new short[image.PixelDataLength];
            image.CopyPixelDataTo(rawDepthData);

            short[] allPoints = new short[image.PixelDataLength];

            for (int i = 0; i < rawDepthData.Length; i++)
            {
                allPoints[i] = (short)GetDepth(rawDepthData[i]);
            }
            return allPoints;
        }

        public static byte CalculateIntensityFromDepth(int distance)
        {
            return (byte)(255 - (255 * Math.Max(distance - MinDepthDistance, 0) / (MaxDepthDistanceOffset)));
        }

        public static void SkeletonOverlay(ref byte redFrame, ref byte greenFrame, ref byte blueFrame, int player)
        {
            switch (player)
            {
                default:
                    break;
                case 1:
                    greenFrame = 0;
                    blueFrame = 0;
                    break;
                case 2:
                    redFrame = 0;
                    greenFrame = 0;
                    break;
                case 3:
                    redFrame = 0;
                    blueFrame = 0;
                    break;
                case 4:
                    greenFrame = 0;
                    break;
                case 5:
                    blueFrame = 0;
                    break;
                case 6:
                    redFrame = 0;
                    break;
                case 7:
                    redFrame /= 2;
                    blueFrame = 0;
                    break;
            }
        }

        public static byte[] ConvertDepthFrameToBitmap(DepthImageFrame depthFrame)
        {
            if (depthFrame == null)
            {
                return null;
            }

            short[] depthData = new short[depthFrame.PixelDataLength];
            depthFrame.CopyPixelDataTo(depthData);

            Byte[] depthColors = new Byte[depthData.Length * 4];

            for (int colorIndex = 0, depthIndex = 0; colorIndex < depthColors.Length; colorIndex += 4, depthIndex++)
            {
                int depth = GetDepth(depthData[depthIndex]);

                if (depth == -1)
                {
                    depthColors[colorIndex + RedIndex] = 66;
                    depthColors[colorIndex + GreenIndex] = 66;
                    depthColors[colorIndex + BlueIndex] = 33;
                }
                else
                {
                    var intensity = ImageCommon.CalculateIntensityFromDepth(depth);

                    depthColors[colorIndex + RedIndex] = intensity;
                    depthColors[colorIndex + GreenIndex] = intensity;
                    depthColors[colorIndex + BlueIndex] = intensity;
                }

                int player = GetPlayerIndex(depthData[depthIndex]);
                SkeletonOverlay(
                    ref depthColors[colorIndex + RedIndex],
                    ref depthColors[colorIndex + GreenIndex],
                    ref depthColors[colorIndex + BlueIndex], player);
            }
            return depthColors;
        }

        public static int GetPlayerIndex(short depthPoint)
        {
            return depthPoint & DepthImageFrame.PlayerIndexBitmask;
        }

        public static int GetDepth(short depthPoint)
        {
            return depthPoint >> DepthImageFrame.PlayerIndexBitmaskWidth;
        }

        /**
         *  Método responsável por realizar a chamada ao método de conversão de Bitmaps.
         */
        public static BitmapSource ToBitmapSource(this byte[] pixels, int width, int height)
        {
            return ToBitmapSource(pixels, width, height, PixelFormats.Bgr32);
        }

        /**
        *  Método responsável por criar o Bitmap.
        */
        private static BitmapSource ToBitmapSource(this byte[] pixels, int width, int height, PixelFormat format)
        {
            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, width * format.BitsPerPixel / 8);
        }

        /**
         *  Método responsável por realizar os cálculos de conversão e definir a cor do visualizar do esqueleto.
         */
        public static BitmapSource ToBitmapSource(this short[] depthData, int width, int height, int minimumDistance, Color highlightColor)
        {
            if (depthData == null)
            {
                return null;
            }
            var depthColors = new byte[depthData.Length * 4];
            for (int colorIndex = 0, depthIndex = 0; colorIndex < depthColors.Length; colorIndex += 4, depthIndex++)
            {
                if (depthData[depthIndex] == -1)
                {
                    // Define a cor do esqueleto
                    depthColors[colorIndex + ImageCommon.RedIndex] = 66;
                    depthColors[colorIndex + ImageCommon.GreenIndex] = 66;
                    depthColors[colorIndex + ImageCommon.BlueIndex] = 33;
                }
                else
                {
                    var intensity = ImageCommon.CalculateIntensityFromDepth(depthData[depthIndex]);
                    depthColors[colorIndex + ImageCommon.RedIndex] = intensity;
                    depthColors[colorIndex + ImageCommon.GreenIndex] = intensity;
                    depthColors[colorIndex + ImageCommon.BlueIndex] = intensity;
                    if (depthData[depthIndex] <= minimumDistance && depthData[depthIndex] > 0)
                    {
                        var color = Color.Multiply(highlightColor, intensity / 255f);
                        depthColors[colorIndex + ImageCommon.RedIndex] = color.R;
                        depthColors[colorIndex + ImageCommon.GreenIndex] = color.G;
                        depthColors[colorIndex + ImageCommon.BlueIndex] = color.B;
                    }
                }
            }
            return depthColors.ToBitmapSource(width, height);
        }

        /**
         *  Método responsável por convertar imagem em bytes.
         */
        public static BitmapSource ToBitmapSource(this DepthImageFrame image)
        {
            if (image == null)
            {
                return null;
            }
            var bytes = ImageCommon.ConvertDepthFrameToBitmap(image);
            return bytes.ToBitmapSource(image.Width, image.Height);
        }

        /**
         *  Método responsável por convertar todos os pixels.
         */
        public static BitmapSource ToBitmapSource(this ColorImageFrame image)
        {
            if (image == null)
            {
                return null;
            }
            byte[] colorData = new byte[image.PixelDataLength];
            image.CopyPixelDataTo(colorData);
            return colorData.ToBitmapSource(image.Width, image.Height);
        }
    }
}