using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypePracticeWpf
{
    public static class Extension
    {
        public static void Capitalize(this char[] chars)
        {
            chars[0] = char.ToUpper(chars[0]);
        }
    }
}
