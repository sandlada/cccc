using System.Globalization;
using System.Text;

namespace Cccc.Application
{
    public static class PreviewText
    {
        // Text elements keep emoji and combining marks intact at the 36-character cut.
        // Blank/empty input returns an empty string so callers can localize the blank-text label.
        public static string Shorten(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
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
            if (collapsed.Length == 0) return "";
            string normalized = collapsed.ToString();
            TextElementEnumerator elements = StringInfo.GetTextElementEnumerator(normalized);
            int count = 0;
            while (elements.MoveNext())
                if (++count > 36) return normalized[..elements.ElementIndex] + "…";
            return normalized;
        }
    }
}
