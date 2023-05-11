using System.Text.RegularExpressions;
using Interfaces.TextTransformer;

namespace Text
{
    public class EscapeSequenceRemover : ITextTransformer
    {
        public string Process(string input)
        {
            return Regex.Replace(input, "\\[\\dm", "");
        }
    }
}