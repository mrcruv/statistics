using System;
using System.Collections;
using System.Collections.Generic;
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
        private static readonly bool double_quotes_as_delimiter_default = true;
        private bool double_quotes_as_delimiter = double_quotes_as_delimiter_default;
        private static readonly bool lowercase_default = true;
        private bool lowercase = lowercase_default;
        private static readonly bool show_dataset_default = false;
        private bool show_dataset = show_dataset_default;
        private int variable_number = 0;
        private readonly int precision = 1;
        public Form1()
        {
            InitializeComponent();
        }

        private Hashtable RelativeFrequency(int variable_number)
        {
            Hashtable frequencies = new Hashtable();
            for (int i = 0; i < n_data; i++)
            {
                var current_value = data[i, variable_number];
                if (frequencies.ContainsKey(current_value))
                {
                    frequencies[current_value] = (int)frequencies[current_value] + 1;
                }
                else
                {
                    frequencies.Add(current_value, 1);
                }
            }
            return frequencies;
        }

        private static string SanitizeString(string str, bool double_quotes_as_delimiter)
        {
            var result = str.Trim();
            if (double_quotes_as_delimiter)
            {
                var length = result.Length;
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Text += "***** begin CSV parsing *****\n";
            this.lines = File.ReadAllLines(this.file_name);
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

            if (this.show_dataset)
            {
                this.richTextBox1.Text += "# VARIABLES: " + this.n_variables + "\n";
                this.richTextBox1.Text += "# DATA: " + this.n_data + "\n";
                this.richTextBox1.Text += "HEADER: ";
                for (int i = 0; i < this.n_variables - 1; i++)
                {
                    this.richTextBox1.Text += this.header[i] + ", ";
                }
                this.richTextBox1.Text += this.header[this.n_variables - 1] + "\n\n";
                for (int i = 0; i < this.n_data; i++)
                {
                    for (int j = 0; j < this.n_variables - 1; j++)
                    {
                        this.richTextBox1.Text += this.data[i, j] + ", ";
                    }
                    this.richTextBox1.Text += this.data[i, this.n_variables - 1] + "\n";
                }
            }

            this.richTextBox1.Text += "***** CSV correctly parsed *****\n";
            this.variable_number = 0;
            this.comboBox1.Items.AddRange(this.header);
            this.comboBox1.Enabled = true;
            this.comboBox1.SelectedIndex = this.variable_number;
            this.button3.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.variable_number = 0;
            this.richTextBox1.Clear();
            this.comboBox1.Items.Clear();
            this.comboBox1.Enabled = false;
            this.comboBox1.ResetText();
            this.button3.Enabled = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.double_quotes_as_delimiter = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            this.lowercase = checkBox2.Checked;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var variable = this.comboBox1.Text;
            this.variable_number = Array.IndexOf(this.header, variable);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var frequencies = this.RelativeFrequency(this.variable_number);
            this.richTextBox1.Text += "\nRELATIVE FREQUENCY WITH RESPECT TO THE VARIABLE " + this.header[this.variable_number] + "\n";
            foreach (string key in frequencies.Keys)
            {
                this.richTextBox1.Text += key + ": " + frequencies[key] + "/" + this.n_data;
                this.richTextBox1.Text += "≈" + ((int)frequencies[key] * 100 / (double)this.n_data).ToString("n" + this.precision) + "%" + "\n";
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            this.show_dataset = checkBox3.Checked;
        }
    }
}
