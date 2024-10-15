using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFU_Beyond_Reach {
    public static class WordWrap {
        public static string Wrap(this string text, int lineLength) {
            using (var reader = new StringReader(text))
                return reader.ReadToEnd(lineLength);
        }
        public static string ReadToEnd(this TextReader reader, int lineLength) {
            return string.Join(Environment.NewLine, reader.ReadLines(lineLength));
        }
        public static IEnumerable<string> ReadLines(this TextReader reader, int lineLength) {
            var line = new StringBuilder();
            foreach (var word in reader.ReadWords())
                if (line.Length + word.Length < lineLength)
                    line.Append($"{word} ");
                else {
                    yield return line.ToString().Trim();
                    line = new StringBuilder($"{word} ");
                }

            if (line.Length > 0)
                yield return line.ToString().Trim();
        }
        public static IEnumerable<string> ReadWords(this TextReader reader) {
            while (!reader.IsEof()) {
                var word = new StringBuilder();
                while (!reader.IsBreak()) {
                    word.Append(reader.Text());
                    reader.Read();
                }

                reader.Read();
                if (word.Length > 0)
                    yield return word.ToString();
            }
        }
        static bool IsBreak(this TextReader reader) => reader.IsEof() || reader.IsWhiteSpace();
        static bool IsWhiteSpace(this TextReader reader) => string.IsNullOrWhiteSpace(reader.Text());
        static string Text(this TextReader reader) => char.ConvertFromUtf32(reader.Peek());
        static bool IsEof(this TextReader reader) => reader.Peek() == -1;
    }
}
