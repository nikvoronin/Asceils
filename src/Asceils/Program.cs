using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Asceils
{
    class Program
    {
        const string IMAGESAMPLES_PATH = @"/../../DIR";   // path to image samples
        const float FONT_ASPECT = 8f / 12f;                 // symbol width divided by height in pixels
        const int CONSOLE_W = 80;                           // console width in chars

        static void Main(string[] args)
        {
            var options = new PicToAsciiOptions() {
                FixedDimension = PicToAsciiOptions.Fix.Horizontal,
                FixedSize = CONSOLE_W,
                SymbolAspectRatio = FONT_ASPECT
            };

            var pic2ascii = new PicToAscii(options);

            foreach (var filename in ImageSamples) {
                IReadOnlyList<ColorTape> colorTapes;
                try {
                    using Stream stream = File.OpenRead(filename);

                    pic2ascii.Options.AsciiTable = PicToAsciiOptions.ASCIITABLE_SYMBOLIC_LIGHT;
                    

                    //colorTapes = PicToAscii.CreateDefault.Convert(stream);
                    colorTapes = pic2ascii.Convert(stream);
                }
                catch {
                    continue;
                }

                PrintTapes(colorTapes);

                Console.ReadLine();
                Console.Clear();
            }
        }

        private static IEnumerable<string> ImageSamples => Directory 
            .GetFiles(IMAGESAMPLES_PATH, "*", SearchOption.TopDirectoryOnly)
            .Where(f => f.LastIndexOf(".jpg" ) > -1
                        || f.LastIndexOf(".jpeg") > -1
                        || f.LastIndexOf(".png" ) > -1);

        private static void PrintTapes(IReadOnlyList<ColorTape> colorTapes)
        {
            foreach (var tape in colorTapes) {
                Console.ForegroundColor = tape.ForeColor;
                Console.Write(tape.Chunk);
            }

            Console.ResetColor();
        }
    }
}