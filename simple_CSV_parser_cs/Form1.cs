using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace simple_CSV_parser_cs
{
    public partial class Form1 : Form
    {
        private readonly char separator = ',';
        private string[] lines;
        private string file_name = "data.csv";
        private int n_data = 0;
        private int n_variables = 0;
        private string[] header;
        private string[,] data;
        private static readonly bool sanitizeDefault = false;
        private bool sanitize = sanitizeDefault;
        private static readonly bool lowercaseDefault = true;
        private bool lowercase = lowercaseDefault;
        public Form1()
        {
            InitializeComponent();
        }

        private static string SanitizeString(string str)
        {
            var result = str.Trim();
            var length = result.Length;
            if (length >= 2)
            {
                result = str.Remove(length - 1, 1);
                result = result.Remove(0, 1);
            }
            return result;
        }

        private static string EscapeDoubleQuotes(string str)
        {
            var result = str.Replace("\"\"", "\"");
            return result;
        }

        private static string ProcessString(string str, bool sanitize, bool lowercase)
        {
            var result = str;
            if (sanitize)
            {
                result = SanitizeString(result);
            }
            if (lowercase)
            {
                result = result.ToLower();
            }
            result = EscapeDoubleQuotes(result);
            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.lines = File.ReadAllLines(this.file_name);
            this.n_data = this.lines.Count() - 1;
            this.header = this.lines[0].Split(this.separator);
            this.n_variables = this.header.Count();
            this.data = new string[this.n_data, this.n_variables];
            this.richTextBox1.Text += "# VARIABLES: " + this.n_variables + "\n";
            this.richTextBox1.Text += "# DATA: " + this.n_data + "\n";
            this.richTextBox1.Text += "HEADER: ";
            for (int i = 0; i < this.n_variables - 1; i++)
            {
                this.header[i] = ProcessString(this.header[i], this.sanitize, this.lowercase);
                this.richTextBox1.Text += this.header[i] + ", ";
            }
            this.header[this.n_variables - 1] = ProcessString(this.header[this.n_variables - 1], this.sanitize, this.lowercase);
            this.richTextBox1.Text += this.header[this.n_variables - 1] + "\n";
            for (int i = 0; i < this.n_data; i++)
            {
                var current_line = this.lines[i + 1].Split(this.separator);
                for (int j = 0; j < this.n_variables - 1; j++)
                {
                    this.data[i, j] = ProcessString(current_line[j], this.sanitize, this.lowercase);
                    this.richTextBox1.Text += this.data[i, j] + ", ";
                }
                this.data[i, this.n_variables - 1] = ProcessString(current_line[this.n_variables - 1], this.sanitize, this.lowercase);
                this.richTextBox1.Text += this.data[i, this.n_variables - 1] + "\n";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Clear();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.sanitize = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            this.lowercase = checkBox2.Checked;
        }
    }
}
