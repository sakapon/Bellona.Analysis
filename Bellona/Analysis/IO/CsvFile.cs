using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bellona.IO
{
    /// <summary>
    /// Provides a set of methods to access CSV files.
    /// </summary>
    /// <remarks>
    /// RFC 4180
    /// https://www.ietf.org/rfc/rfc4180.txt
    /// </remarks>
    public static class CsvFile
    {
        // Excepts the definition for CRLF in a field.
        // Uses ?: to minimize capturing groups.
        static readonly Regex CsvFieldPattern = new Regex("(?<=^|,)" + "(?:\"(.*?)\"|[^,]*?)" + "(?=$|,)");

        static IEnumerable<string> SplitLine0(string line) =>
            CsvFieldPattern.Matches(line)
                .Cast<Match>()
                .Select(m => m.Groups[1].Success ? m.Groups[1].Value : m.Value)
                .Select(s => s.Replace("\"\"", "\""));

        public static string[] SplitLine(string line) => SplitLine0(line).ToArray();

        static readonly Regex QualifyingFieldPattern = new Regex("^.*[,\"].*$");

        public static string ToLine(IEnumerable<string> fields) => string.Join(",",
            fields
                .Select(f => f.Replace("\"", "\"\""))
                .Select(f => QualifyingFieldPattern.Replace(f, "\"$&\""))
        );

        static TResult ReadFile<TResult>(string path, Func<Stream, TResult> func)
        {
            using (var stream = File.OpenRead(path))
            {
                return func(stream);
            }
        }

        static void WriteFile(string path, Action<Stream> action)
        {
            using (var stream = File.Create(path))
            {
                action(stream);
            }
        }

        public static IEnumerable<string[]> ReadRecordsByArray(Stream stream, bool hasHeader, Encoding encoding = null)
        {
            return stream.ReadLines(encoding)
                .Skip(hasHeader ? 1 : 0)
                .Select(SplitLine);
        }

        public static IEnumerable<string[]> ReadRecordsByArray(string path, bool hasHeader, Encoding encoding = null) =>
            ReadFile(path, stream => ReadRecordsByArray(stream, hasHeader, encoding));

        public static void WriteRecordsByArray(Stream stream, IEnumerable<string[]> records, Encoding encoding = null)
        {
            if (records == null) throw new ArgumentNullException(nameof(records));

            var lines = records.Select(ToLine);

            stream.WriteLines(lines, encoding);
        }

        public static void WriteRecordsByArray(string path, IEnumerable<string[]> records, Encoding encoding = null) =>
            WriteFile(path, stream => WriteRecordsByArray(stream, records, encoding));

        public static void WriteRecordsByArray(Stream stream, IEnumerable<string[]> records, string[] columnNames, Encoding encoding = null)
        {
            if (records == null) throw new ArgumentNullException(nameof(records));
            if (columnNames == null) throw new ArgumentNullException(nameof(columnNames));

            WriteRecordsByArray(stream, Enumerable.Repeat(columnNames, 1).Concat(records), encoding);
        }

        public static void WriteRecordsByArray(string path, IEnumerable<string[]> records, string[] columnNames, Encoding encoding = null) =>
            WriteFile(path, stream => WriteRecordsByArray(stream, records, columnNames, encoding));

        // Supposes that a CSV file has the header line.
        public static IEnumerable<Dictionary<string, string>> ReadRecordsByDictionary(Stream stream, Encoding encoding = null)
        {
            var lines = stream.ReadLines(encoding).Select(SplitLine);
            string[] columnNames = null;

            foreach (var fields in lines)
            {
                if (columnNames == null)
                    columnNames = fields;
                else
                    yield return Enumerable.Range(0, columnNames.Length).ToDictionary(i => columnNames[i], i => fields[i]);
            }
        }

        public static IEnumerable<Dictionary<string, string>> ReadRecordsByDictionary(string path, Encoding encoding = null) =>
            ReadFile(path, stream => ReadRecordsByDictionary(stream, encoding));

        public static void WriteRecordsByDictionary(Stream stream, IEnumerable<Dictionary<string, string>> records, Encoding encoding = null)
        {
            if (records == null) throw new ArgumentNullException(nameof(records));

            var lines = records
                .Select(d => d.Values)
                .Select(ToLine);

            stream.WriteLines(lines, encoding);
        }

        public static void WriteRecordsByDictionary(string path, IEnumerable<Dictionary<string, string>> records, Encoding encoding = null) =>
            WriteFile(path, stream => WriteRecordsByDictionary(stream, records, encoding));

        public static void WriteRecordsByDictionary(Stream stream, IEnumerable<Dictionary<string, string>> records, string[] columnNames, Encoding encoding = null)
        {
            if (records == null) throw new ArgumentNullException(nameof(records));
            if (columnNames == null) throw new ArgumentNullException(nameof(columnNames));

            var lines = Enumerable.Repeat(columnNames, 1)
                .Concat(records.Select(d => columnNames.Select(c => d[c])))
                .Select(ToLine);

            stream.WriteLines(lines, encoding);
        }

        public static void WriteRecordsByDictionary(string path, IEnumerable<Dictionary<string, string>> records, string[] columnNames, Encoding encoding = null) =>
            WriteFile(path, stream => WriteRecordsByDictionary(stream, records, columnNames, encoding));
    }
}
