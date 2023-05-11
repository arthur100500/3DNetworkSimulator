using System;
using System.Collections.Generic;
using System.Text;
using Interfaces.TextTransformer;

namespace Text
{
    public class EscapeSequencesPrettifier : ITextTransformer
    {
        private readonly List<string> _openTags;
        private readonly Dictionary<StyleSequenceType, string> _styles;

        public EscapeSequencesPrettifier()
        {
            _openTags = new List<string>();
            _styles = new Dictionary<StyleSequenceType, string>
            {
                [StyleSequenceType.BoldBright] = "<b>",
                [StyleSequenceType.NoBoldBright] = "</b>",
                [StyleSequenceType.Underline] = "<u>",
                [StyleSequenceType.NoUnderline] = "</u>",
                [StyleSequenceType.ForegroundBlack] = "<color=black>",
                [StyleSequenceType.ForegroundRed] = "<color=red>",
                [StyleSequenceType.ForegroundGreen] = "<color=green>",
                [StyleSequenceType.ForegroundYellow] = "<color=yellow>",
                [StyleSequenceType.ForegroundBlue] = "<color=blue>",
                [StyleSequenceType.ForegroundMagenta] = "<color=magenta>",
                [StyleSequenceType.ForegroundCyan] = "<color=cyan>",
                [StyleSequenceType.ForegroundWhite] = "<color=white>",
                [StyleSequenceType.ForegroundExtended] = "UNSUPPORTED",
                [StyleSequenceType.ForegroundDefault] = "</color>",
                [StyleSequenceType.BackgroundBlack] = "UNSUPPORTED",
                [StyleSequenceType.BackgroundRed] = "UNSUPPORTED",
                [StyleSequenceType.BackgroundGreen] = "UNSUPPORTED",
                [StyleSequenceType.BackgroundYellow] = "UNSUPPORTED",
                [StyleSequenceType.BackgroundBlue] = "UNSUPPORTED",
                [StyleSequenceType.BackgroundMagenta] = "UNSUPPORTED",
                [StyleSequenceType.BackgroundCyan] = "UNSUPPORTED",
                [StyleSequenceType.BackgroundWhite] = "UNSUPPORTED",
                [StyleSequenceType.BackgroundExtended] = "UNSUPPORTED",
                [StyleSequenceType.BackgroundDefault] = "UNSUPPORTED",
                [StyleSequenceType.BrightForegroundBlack] = "<b><color=black>",
                [StyleSequenceType.BrightForegroundRed] = "<b><color=red>",
                [StyleSequenceType.BrightForegroundGreen] = "<b><color=green>",
                [StyleSequenceType.BrightForegroundYellow] = "<b><color=yellow>",
                [StyleSequenceType.BrightForegroundBlue] = "<b><color=blue>",
                [StyleSequenceType.BrightForegroundMagenta] = "<b><color=magenta>",
                [StyleSequenceType.BrightForegroundCyan] = "<b><color=cyan>",
                [StyleSequenceType.BrightForegroundWhite] = "<b><color=white>",
                [StyleSequenceType.BrightBackgroundBlack] = "UNSUPPORTED",
                [StyleSequenceType.BrightBackgroundRed] = "UNSUPPORTED",
                [StyleSequenceType.BrightBackgroundGreen] = "UNSUPPORTED",
                [StyleSequenceType.BrightBackgroundYellow] = "UNSUPPORTED",
                [StyleSequenceType.BrightBackgroundBlue] = "UNSUPPORTED",
                [StyleSequenceType.BrightBackgroundMagenta] = "UNSUPPORTED",
                [StyleSequenceType.BrightBackgroundCyan] = "UNSUPPORTED",
                [StyleSequenceType.BrightBackgroundWhite] = "UNSUPPORTED"
            };
        }

        public string Process(string input)
        {
            var result = new StringBuilder((int)(input.Length * 1.5f));

            for (var index = 0; index < input.Length; index++)
            {
                var style = CheckForStyleSequence(input, index);
                if (style == StyleSequenceType.None && input[index] != '')
                {
                    result.Append(input[index]);
                    continue;
                }

                if (_styles.ContainsKey(style) && _styles[style] != "UNSUPPORTED")
                {
                    result.Append(_styles[style]);
                    _openTags.Add(_styles[style]);
                }

                if (style == StyleSequenceType.Default)
                    result.Append(CloseAllTags());

                var skipAmount = Convert.ToString((int)style).Length + 2;
                index += skipAmount;
            }

            return result.ToString();
        }

        private StyleSequenceType CheckForStyleSequence(string input, int start)
        {
            if (!input.Substring(start).StartsWith("["))
                return StyleSequenceType.None;

            start += 2;
            var digitCount = 0;
            while (char.IsDigit(input[start + digitCount])) digitCount++;
            var result = int.Parse(input.Substring(start, digitCount));
            return (StyleSequenceType)result;
        }

        private string CloseAllTags()
        {
            var sb = new StringBuilder();
            foreach (var tag in _openTags)
            {
                switch (tag)
                {
                    case "<b>":
                        sb.Append("</b>");
                        break;
                    case "<u>":
                        sb.Append("</u>");
                        break;
                }

                if (tag.StartsWith("<color"))
                    sb.Append("</color>");
            }

            _openTags.Clear();
            return sb.ToString();
        }
    }
}