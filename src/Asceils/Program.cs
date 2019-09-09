using System;
using System.Collections.Generic;
using System.IO;

namespace Asceils
{
    class Program
    {
        static string[] examples = new string[] { "github.png", "freedom.jpg", "vasiliy.jpg", "johnny.jpg", "mondrian.jpg", "r2d2.jpg", "lenna.png", "homer.png", "evangeline.jpg", "gandalf.png", "marylin.jpg", "mona.png", "monica.jpg" };

        static void Main(string[] args)
        {
            for (int i = 0; i < examples.Length; i++) {
                Stream stream;
                try {
                    stream = File.OpenRead($"..\\..\\img\\{examples[i]}");
                }
                catch {
                    continue;
                }

                List<ColorTape> ascii = PicToAscii.Convert(stream, 80, 40, 16f/12f);

                foreach (var cs in ascii) {
                    Console.ForegroundColor = cs.ForeColor;
                    Console.Write(cs.Chunk);
                }

                Console.ResetColor();
                Console.ReadLine();
                Console.Clear();
            }
        }
    }
}
