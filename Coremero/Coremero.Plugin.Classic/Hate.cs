﻿using System;
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
        FileIndex _spongeFileIndex;
        FileIndex _ppLFileIndex;

        private Random _rnd = new Random();

        private string _spongeFile = Path.Combine(PathExtensions.ResourceDir, "sponge_list.txt");
        private string _ppFile = Path.Combine(PathExtensions.ResourceDir, "lilpp_list.txt");
        
        public Hate()
        {
            _spongeFileIndex = new FileIndex(_spongeFile);
            _ppLFileIndex = new FileIndex(_ppFile);
        }

        [Command("lilpp", Help = "Impersonates LilPP.")]
        public string LilPPHates()
        {
            return $"LilPP: i hate {_ppLFileIndex.GetRandomLine()}";
        }

        [Command("hate", Help = "Hate a random object.")]
        public string PlainHate()
        {
            return $"i hate {_ppLFileIndex.GetRandomLine()}";
        }


        [Command("love", Help = "Love a random object.")]
        public string PlainLove()
        {
            return $"i love {_ppLFileIndex.GetRandomLine()}";
        }


        [Command("sponge", Help = "Impersonates sponge.")]
        public string SpongeFeels()
        {
            var i = _rnd.Next(10);
            var verb = i % 3 == 0 ? "love" : i % 3 == 1 ? "am ambivalent towards" : "hate";
            return $"sponge: i {verb} {_spongeFileIndex.GetRandomLine()}";
        }
    }
}