using System;
using System.Globalization;
using System.Text;

namespace Cccc.Application
{
    public static class PreviewText
    {
        // Text elements keep emoji and combining marks intact at the 36-character cut.
        public static string Shorten(string text)
        {
            if (string.IsNullOrEmpty(text)) return "空白文字";
            StringBuilder collapsed = new();
            bool space = false;
            foreach (char c in text)
            {
                if (char.IsWhiteSpace(c) || char.IsControl(c)) { space = collapsed.Length > 0; continue; }
                // Remove bidi and other formatting controls from untrusted clipboard text.
                if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.Format && c != '\u200d') continue;
                if (space) collapsed.Append(' ');
                collapsed.Append(c);
                space = false;
            }
            if (collapsed.Length == 0) return "空白文字";
            string normalized = collapsed.ToString();
            TextElementEnumerator elements = StringInfo.GetTextElementEnumerator(normalized);
            int count = 0;
            while (elements.MoveNext())
                if (++count > 36) return normalized[..elements.ElementIndex] + "…";
            return normalized;
        }
    }
}
