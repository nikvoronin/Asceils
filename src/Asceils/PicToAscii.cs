using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Text;

namespace Asceils
{
    class PicToAscii
    {
        //public static string asciils = " `'.,:;i+o*%&$#@";
        public static string asciils = " ░▒▓█";

        /// <summary>
        /// Returns a list of the colored string chunks
        /// </summary>
        /// <param name="stream">Bitmap stream</param>
        /// <param name="h">Console lines</param>
        /// <param name="symbolAspect">Symbol height / symbol width in pixels</param>
        /// <returns></returns>
        public static List<ColorTape> Convert(Stream stream, int w, int h, float symbolAspect = 2f)
        {
            Bitmap source = new Bitmap(stream);
            float k = h * symbolAspect / source.Height;
            int ww = (int)(source.Width * k);

            Bitmap reduced = new Bitmap(ww, h);
            Graphics g = Graphics.FromImage(reduced);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            g.DrawImage(source, 0, 0, ww, h);

            var chunks = new List<ColorTape>();

            StringBuilder chunkBuilder = new StringBuilder();
            ConsoleColor lastColor = ConsoleColor.Black;
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < ww; x++) {
                    Color c = reduced.GetPixel(x, y);
                    char symbol = BrightnessToChar(c.GetBrightness(), asciils);
                    ConsoleColor cc = ToConsoleColor(c, .8f);

                    if (lastColor != cc) {
                        if (chunkBuilder.Length > 0) {
                            var tape = new ColorTape(chunkBuilder.ToString(), lastColor);
                            chunks.Add(tape);
                            chunkBuilder.Clear();
                        }

                        lastColor = cc;
                    }

                    chunkBuilder.Append(symbol);
                }

                chunkBuilder.Append('\n');
            }

            if (chunkBuilder.Length > 0)
                chunks.Add(new ColorTape(chunkBuilder.ToString(), lastColor));

            return chunks;
        }

        public static char BrightnessToChar(float bright, string symbols)
        {
            int charIndex = (int)(bright * (symbols.Length - 1));
            return symbols[charIndex];
        }

        public static ConsoleColor ToConsoleColor(Color c, float factor)
        {
            int index = (c.R > 200 | c.G > 170 | c.B > 220) ? 8 : 0; // bright

            float max = Math.Max(Math.Max(c.R, c.G), c.B);
            index |= (c.R / max > factor) ? 4 : 0;
            index |= (c.G / max > factor) ? 2 : 0;
            index |= (c.B / max > factor) ? 1 : 0;

            return (ConsoleColor)index;
        }
    } // class PicToAscii

    public struct ColorTape
    {
        public ColorTape(string chunk, ConsoleColor color)
        {
            ForeColor = color;
            Chunk = chunk;
        }

        public ConsoleColor ForeColor;
        public string Chunk;
    } // struct ColorTape
}
