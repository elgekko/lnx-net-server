using System;
using System.Text;

namespace EchoServer
{
    public static class StringExtensions
    {
        public static string ToControlCodeString(this string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if (char.IsControl(c))
                {
                    sb.Append($"[{(int)c}]");
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
