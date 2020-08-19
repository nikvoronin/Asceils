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

        static void Main(string[] args)
        {
            var samples = GetSamples();

            foreach(var filename in samples) {
                IReadOnlyList<ColorTape> colorTapes;
                try {
                    using (Stream stream = File.OpenRead(filename))
                        colorTapes = PicToAscii.Convert(stream, PicToAscii.Fix.Horizontal, size: 80, FONT_ASPECT);
                }
                catch {
                    continue;
                }

                PrintTapes(colorTapes);

                Console.ReadLine();
                Console.Clear();
            }
        }

        private static IEnumerable<string> GetSamples()
        {
            return Directory
                .GetFiles(IMAGE_DIR, "*", SearchOption.TopDirectoryOnly)
                .Where(f => f.LastIndexOf(".jpg") > -1
                         || f.LastIndexOf(".jpeg") > -1
                         || f.LastIndexOf(".png") > -1);
        }

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
