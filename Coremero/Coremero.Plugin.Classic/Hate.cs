using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Coremero.Utilities;
using Coremero.Commands;

namespace Coremero.Plugin.Classic
{
    public class Hate : IPlugin
    {
        // Get all \n so we can just skip to that directly when reading the file and reduce IO a bit.
        private List<long> _spongeLineIndexes = new List<long>();
        private List<long> _ppLineIndexes = new List<long>();

        private string _spongeFile = Path.Combine(PathExtensions.PluginDir, "sponge_list.txt");
        private string _ppFile = Path.Combine(PathExtensions.PluginDir, "lilpp_list.txt");

        private Random _rnd = new Random();


        public Hate()
        {
            IndexFile(_spongeFile, ref _spongeLineIndexes);
            IndexFile(_ppFile, ref _ppLineIndexes);
        }

        private void IndexFile(string filePath, ref List<long> indexList)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                indexList.Add(stream.Position);

                int character;
                while ((character = stream.ReadByte()) != -1) // "-1" denotes the end of the file
                {
                    if (character == '\n')
                    {
                        indexList.Add(stream.Position);
                    }
                }
            }
        }

        private string GetRandomLine(string filePath, ref List<long> indexList)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                stream.Position = indexList[new Random().Next(indexList.Count)];
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadLine();
                }
            }
        }

        [Command("lilpp", Help = "Impersonates LilPP.")]
        public string LilPPHates()
        {
            return $"LilPP: i hate {GetRandomLine(_ppFile, ref _ppLineIndexes)}";
        }
        [Command("hate", Help = "Hate a random object.")]
        public string PlainHate()
        {
            return $"i hate {GetRandomLine(_ppFile, ref _ppLineIndexes)}";
        }


        [Command("love", Help = "Love a random object.")]
        public string PlainLove()
        {
            return $"i love {GetRandomLine(_ppFile, ref _ppLineIndexes)}";
        }



        [Command("sponge", Help = "Impersonates sponge.")]
        public string SpongeFeels()
        {
            var i = _rnd.Next(10);
            var verb = i % 3 == 0 ? "love" : i % 3 == 1 ? "am ambivalent towards" : "hate";
            return $"sponge: i {verb} {GetRandomLine(_spongeFile, ref _spongeLineIndexes)}";
        }

    }
}
