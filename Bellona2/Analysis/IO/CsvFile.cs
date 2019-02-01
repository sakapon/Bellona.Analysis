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

        static IEnumerable<string[]> ReadRecordsByArray(this IEnumerable<string> lines, bool hasHeader)
        {
            return lines
                .Skip(hasHeader ? 1 : 0)
                .Select(SplitLine);
        }

        public static IEnumerable<string[]> ReadRecordsByArray(Stream stream, bool hasHeader, Encoding encoding = null) =>
            TextFile.ReadLines(stream, encoding).ReadRecordsByArray(hasHeader);

        public static IEnumerable<string[]> ReadRecordsByArray(string path, bool hasHeader, Encoding encoding = null) =>
            TextFile.ReadLines(path, encoding).ReadRecordsByArray(hasHeader);

        static IEnumerable<string> WriteRecordsByArray(this IEnumerable<string[]> records)
        {
            if (records == null) throw new ArgumentNullException(nameof(records));

            return records.Select(ToLine);
        }

        public static void WriteRecordsByArray(Stream stream, IEnumerable<string[]> records, Encoding encoding = null) =>
            TextFile.WriteLines(stream, records.WriteRecordsByArray(), encoding);

        public static void WriteRecordsByArray(string path, IEnumerable<string[]> records, Encoding encoding = null) =>
            TextFile.WriteLines(path, records.WriteRecordsByArray(), encoding);

        static IEnumerable<string> WriteRecordsByArray(this IEnumerable<string[]> records, string[] columnNames)
        {
            if (records == null) throw new ArgumentNullException(nameof(records));
            if (columnNames == null) throw new ArgumentNullException(nameof(columnNames));

            return Enumerable.Repeat(columnNames, 1)
                .Concat(records)
                .Select(ToLine);
        }

        public static void WriteRecordsByArray(Stream stream, IEnumerable<string[]> records, string[] columnNames, Encoding encoding = null) =>
            TextFile.WriteLines(stream, records.WriteRecordsByArray(columnNames), encoding);

        public static void WriteRecordsByArray(string path, IEnumerable<string[]> records, string[] columnNames, Encoding encoding = null) =>
            TextFile.WriteLines(path, records.WriteRecordsByArray(columnNames), encoding);

        // Supposes that a CSV file has the header line.
        static IEnumerable<Dictionary<string, string>> ReadRecordsByDictionary(this IEnumerable<string> lines)
        {
            var lines2 = lines.Select(SplitLine);
            string[] columnNames = null;

            foreach (var fields in lines2)
            {
                if (columnNames == null)
                    columnNames = fields;
                else
                    yield return Enumerable.Range(0, columnNames.Length).ToDictionary(i => columnNames[i], i => fields[i]);
            }
        }

        public static IEnumerable<Dictionary<string, string>> ReadRecordsByDictionary(Stream stream, Encoding encoding = null) =>
            TextFile.ReadLines(stream, encoding).ReadRecordsByDictionary();

        public static IEnumerable<Dictionary<string, string>> ReadRecordsByDictionary(string path, Encoding encoding = null) =>
            TextFile.ReadLines(path, encoding).ReadRecordsByDictionary();

        static IEnumerable<string> WriteRecordsByDictionary(this IEnumerable<Dictionary<string, string>> records)
        {
            if (records == null) throw new ArgumentNullException(nameof(records));

            return records
                .Select(d => d.Values)
                .Select(ToLine);
        }

        public static void WriteRecordsByDictionary(Stream stream, IEnumerable<Dictionary<string, string>> records, Encoding encoding = null) =>
            TextFile.WriteLines(stream, records.WriteRecordsByDictionary(), encoding);

        public static void WriteRecordsByDictionary(string path, IEnumerable<Dictionary<string, string>> records, Encoding encoding = null) =>
            TextFile.WriteLines(path, records.WriteRecordsByDictionary(), encoding);

        static IEnumerable<string> WriteRecordsByDictionary(this IEnumerable<Dictionary<string, string>> records, string[] columnNames)
        {
            if (records == null) throw new ArgumentNullException(nameof(records));
            if (columnNames == null) throw new ArgumentNullException(nameof(columnNames));

            return Enumerable.Repeat(columnNames, 1)
                .Concat(records.Select(d => columnNames.Select(c => d[c])))
                .Select(ToLine);
        }

        public static void WriteRecordsByDictionary(Stream stream, IEnumerable<Dictionary<string, string>> records, string[] columnNames, Encoding encoding = null) =>
            TextFile.WriteLines(stream, records.WriteRecordsByDictionary(columnNames), encoding);

        public static void WriteRecordsByDictionary(string path, IEnumerable<Dictionary<string, string>> records, string[] columnNames, Encoding encoding = null) =>
            TextFile.WriteLines(path, records.WriteRecordsByDictionary(columnNames), encoding);
    }
}
