using System;
using System.Collections.Generic;
using System.Text;

namespace Domino.Data
{
    public class WordList
    {
        private static string LastWord = "";

        public static string GetWordRepeatable()
        {
            int number = new Random().Next(0, Words.List.Length);
            return Words.List[number];
        }

        public static string GetWord()
        {
            while (true)
            {
                int number = new Random().Next(0, Words.List.Length);
                if (LastWord != Words.List[number])
                {
                    LastWord = Words.List[number];
                    break;
                }
            }
            return LastWord;
        }
    }
}
