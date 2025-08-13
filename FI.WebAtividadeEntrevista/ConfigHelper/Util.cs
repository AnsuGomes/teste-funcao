using System.Text.RegularExpressions;

namespace FI.WebAtividadeEntrevista.ConfigHelper
{
    public static class Util
    {
        public static string RemoveNaoNumericos(string input)
        {
            return Regex.Replace(input ?? string.Empty, @"[^0-9]", string.Empty);
        }
    }
}