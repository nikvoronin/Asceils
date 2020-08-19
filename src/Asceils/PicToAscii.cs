using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;

namespace Asceils
{
    class PicToAscii
    {
        //public static string asciils = " `'.,:;i+o*%&$#@";
        public static string asciils = " ░▒▓█";
        public enum Fix { Horizontal = 0, Vertical};
        public static ColorSpaceConverter _spaceConverter = new ColorSpaceConverter();

        public static IReadOnlyList<ColorTape> Convert(Stream stream, Fix fixedDimension, int size, float symbolAspect = .5f) 
        {
            using Image<Rgb24> source = Image.Load<Rgb24>(stream);
            float sourceAspect = (float)source.Width / source.Height;

            int width, height;

            switch (fixedDimension) {
                case Fix.Vertical:
                    height = size;
                    width = (int)Math.Round(size * sourceAspect / symbolAspect);
                    break;

                default:
                case Fix.Horizontal:
                    width = size;
                    height = (int)Math.Round(size / sourceAspect * symbolAspect);
                    break;
            }

            return ConvertInternal(source, width, height);
        }

        /// <summary>
        /// Returns a list of the colored string chunks
        /// </summary>
        /// <param name="source">Bitmap source image</param>
        /// <param name="width">Result width in symbols</param>
        /// <param name="height">Result height in symbols</param>
        /// <returns>Colored tapes ready to print to the console</returns>
        private static IReadOnlyList<ColorTape> ConvertInternal(Image<Rgb24> source, int width, int height)
        {
            using var reduced = source.Clone(x => x.Resize(width, height));

            var chunks = new List<ColorTape>();

            StringBuilder chunkBuilder = new StringBuilder();
            ConsoleColor lastColor = ConsoleColor.Black;
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    Rgb24 c = reduced[x, y];
                    (ConsoleColor cc, char symbol) = ToColoredChar(c);

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

        private static (ConsoleColor, char) ToColoredChar(Rgb24 c)
        {
            Hsl hsl = _spaceConverter.ToHsl(c);            
            char symbol = BrightnessToChar(hsl.L, asciils);
            ConsoleColor cc = ToConsoleColor(c, .8f);

            return (cc, symbol);
        }

        public static char BrightnessToChar(float bright, string symbols)
        {
            int charIndex = (int)(bright * (symbols.Length - 1));
            return symbols[charIndex];
        }

        public static ConsoleColor ToConsoleColor(Rgb24 c, float factor)
        {
            int index = (c.R > 200 | c.G > 170 | c.B > 220) ? 8 : 0; // bright bit

            float max = Math.Max(Math.Max(c.R, c.G), c.B);
            index |= (c.R / max > factor) ? 4 : 0;
            index |= (c.G / max > factor) ? 2 : 0;
            index |= (c.B / max > factor) ? 1 : 0;

            return (ConsoleColor)index;
        }
    }

    public struct ColorTape
    {
        public ColorTape(string chunk, ConsoleColor color)
        {
            ForeColor = color;
            Chunk = chunk;
        }

        public ConsoleColor ForeColor;
        public string Chunk;
    }
}
