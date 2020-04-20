using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypePracticeWpf
{
    public class Punctuation
    {
        public Punctuation(string value, bool capitalizeNextWord)
        {
            Value = value;
            CapitalizeNextWord = capitalizeNextWord;
        }

        public string Value { get; set; }
        public bool CapitalizeNextWord { get; set; }
    }
}
