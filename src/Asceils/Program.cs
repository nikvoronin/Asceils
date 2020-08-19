using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Asceils
{
    class Program
    {
        const string IMAGE_DIR = @"..\..\..\img";
        const float FONT_ASPECT = 8f / 12f; // width/height (in pixels), it's a good idea to use fixed size console fonts.
        const int CONSOLE_W = 80;

        static void Main(string[] args)
        {
            var options = new PicToAsciiOptions() {
                FixedDimension = PicToAsciiOptions.Fix.Horizontal,
                FixedSize = CONSOLE_W,
                SymbolAspectRatio = FONT_ASPECT
            };

            var converter = new PicToAscii(options);

            foreach (var filename in ImageSamples) {
                IReadOnlyList<ColorTape> colorTapes;
                try {
                    using Stream stream = File.OpenRead(filename);
                    //colorTapes = PicToAscii.CreateDefault.Convert(stream);

                    converter.Options.AsciiChars = Environment.TickCount % 2 == 0 
                        ? PicToAsciiOptions.ASCII_SOLID 
                        : PicToAsciiOptions.ASCII_SYMBOLIC;
                    colorTapes = converter.Convert(stream);
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
            .GetFiles(IMAGE_DIR, "*", SearchOption.TopDirectoryOnly)
            .Where(f => f.LastIndexOf(".jpg" ) > -1
                     || f.LastIndexOf(".jpeg") > -1
                     || f.LastIndexOf(".png" ) > -1);

        private static void PrintTapes(IReadOnlyList<ColorTape> colorTapes)
        {
            foreach (var cs in colorTapes) {
                Console.ForegroundColor = cs.ForeColor;
                Console.Write(cs.Chunk);
            }

            Console.ResetColor();
        }
    }
}
