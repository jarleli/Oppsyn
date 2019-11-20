using System;
using System.Collections.Generic;
using System.Text;

namespace Oppsyn.ExtensionsClasses
{
    public static class StringExtensions
    {
        public static string SafeSubstring(this string target, int startIndex, int length)
        {
            length = Math.Min(length, target.Length);
            return target.Substring(startIndex, length);
        }
    }
}
