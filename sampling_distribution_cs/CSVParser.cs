using System.IO;
using System.Linq;

namespace sampling_distribution_cs
{
    internal class CSVParser
    {
        private readonly char separator = ',';
        private string[] lines;
        private string file_name = "data.csv";
        private int n_data = 0;
        private int n_variables = 0;
        private string[] header;
        private string[,] data;
        private bool double_quotes_as_delimiter;
        private bool lowercase;

        public CSVParser(char separator, bool double_quotes_as_delimiter, bool lowercase)
        {
            this.separator = separator;
            this.double_quotes_as_delimiter = double_quotes_as_delimiter;
            this.lowercase = lowercase;
        }

        public int getNData() { return this.n_data; }
        public int getNVariables() { return this.n_variables; }
        public string[] getHeader() { return this.header; }
        public string[,] getData() { return this.data; }

        private static string SanitizeString(string str, bool double_quotes_as_delimiter)
        {
            var result = str.Trim();
            var length = result.Length;
            if (double_quotes_as_delimiter && length > 0)
            {
                if (result[0] == '\"' && result[length - 1] == '\"')
                {
                    result = str.Remove(length - 1, 1);
                    result = result.Remove(0, 1);
                }
            }
            return result;
        }

        private static string EscapeDoubleQuotes(string str)
        {
            var result = str.Replace("\"\"", "\"");
            return result;
        }

        private static string ProcessString(string str, bool double_quotes_as_delimiter, bool lowercase)
        {
            var result = str;
            if (double_quotes_as_delimiter)
            {
                result = SanitizeString(result, double_quotes_as_delimiter);
            }
            if (lowercase)
            {
                result = result.ToLower();
            }
            result = EscapeDoubleQuotes(result);
            return result;
        }

        public void parse(string file_name)
        {
            this.clear();
            this.file_name = file_name;
            this.lines = File.ReadAllLines(file_name);
            this.n_data = this.lines.Count() - 1;
            this.header = this.lines[0].Split(this.separator);
            this.n_variables = this.header.Count();
            this.data = new string[this.n_data, this.n_variables];

            for (int i = 0; i < this.n_variables - 1; i++)
            {
                this.header[i] = ProcessString(this.header[i], this.double_quotes_as_delimiter, this.lowercase);
            }
            this.header[this.n_variables - 1] = ProcessString(this.header[this.n_variables - 1], this.double_quotes_as_delimiter, this.lowercase);
            for (int i = 0; i < this.n_data; i++)
            {
                var current_line = this.lines[i + 1].Split(this.separator);
                for (int j = 0; j < this.n_variables - 1; j++)
                {
                    this.data[i, j] = ProcessString(current_line[j], this.double_quotes_as_delimiter, this.lowercase);
                }
                this.data[i, this.n_variables - 1] = ProcessString(current_line[this.n_variables - 1], this.double_quotes_as_delimiter, this.lowercase);
            }
        }

        private void clear()
        {
            this.lines = null;
            this.file_name = "";
            this.n_data = 0;
            this.n_variables = 0;
            this.header = null;
            this.data = null;
        }
    }
}