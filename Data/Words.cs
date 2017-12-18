using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Domino.Data
{
    public static class Words
    {
        public static string[] List { get; set; }

        public static void LoadAllWords()
        {
            List = File.ReadAllLines("bridge/resources/Domino/Resources/words.txt");
        }
    }
}
