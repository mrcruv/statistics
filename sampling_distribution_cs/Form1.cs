using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sampling_distribution_cs
{
    public partial class Form1 : Form
    {
        private readonly char separator = ',';
        private readonly string file_name = "data.csv";
        private int n_data = 0;
        private int n_variables = 0;
        private string[] header;
        private string[,] data;
        private bool double_quotes_as_delimiter;
        private bool lowercase;
        private int variable_number = 0;
        private readonly int precision = 1;
        private Random generator = new Random();
        private CSVParser parser;
        private int n_samples = 0;
        private int sample_size = 0;
        private float[,] samples;
        private float[] samples_means, samples_variances;
        private readonly string log_delimiter = "*****";

        private Bitmap bitmap1, bitmap2;
        private Graphics graphics1, graphics2;
        private Hashtable mean_distribution, variance_distribution;

        public Form1()
        {
            InitializeComponent();
            this.set_defaults();
        }

        private void set_defaults()
        {
            this.double_quotes_as_delimiter = this.checkBox1.Checked;
            this.lowercase = this.checkBox2.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.parser = new CSVParser(this.separator, this.double_quotes_as_delimiter, this.lowercase);
            // this.richTextBox1.Text += $"{this.log_delimiter} begin CSV parsing {this.log_delimiter}\n";
            parser.parse(this.file_name);
            this.n_data = this.parser.getNData();
            this.header = this.parser.getHeader();
            this.n_variables = this.parser.getNVariables();
            this.data = this.parser.getData();
            // this.richTextBox1.Text += $"{this.log_delimiter} CSV succesfully parsed {this.log_delimiter}\n";
            this.variable_number = 0;
            this.comboBox1.Items.AddRange(this.header);
            this.comboBox1.Enabled = true;
            this.comboBox1.SelectedIndex = this.variable_number;
            this.button3.Enabled = true;
            this.trackBar1.Maximum = this.n_data;
            this.trackBar1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.variable_number = 0;
            this.richTextBox1.Clear();
            this.comboBox1.Items.Clear();
            this.comboBox1.Enabled = false;
            this.comboBox1.ResetText();
            this.button3.Enabled = false;
            this.clear_graphics();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.n_samples = this.trackBar1.Value;
            this.trackBar2.Maximum = (int)Math.Round((float)this.n_data / this.n_samples);
            this.trackBar2.Enabled = true;
            this.trackBar2.Value = this.trackBar2.Minimum;
            this.sample_size = this.trackBar2.Value;
            this.label1.Text = $"# SAMPLES: {this.n_samples}";
            this.label2.Text = $"SAMPLE SIZE: {this.sample_size}";
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            this.sample_size = this.trackBar2.Value;
            this.label2.Text = $"SAMPLE SIZE: {this.sample_size}";
        }

        private void initialize_attributes()
        {
            this.bitmap1 = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.graphics1 = Graphics.FromImage(this.bitmap1);
            this.bitmap2 = new Bitmap(this.pictureBox2.Width, this.pictureBox2.Height);
            this.graphics2 = Graphics.FromImage(this.bitmap2);
        }

        private void set_images()
        {
            this.pictureBox1.Image = this.bitmap1;
            this.pictureBox2.Image = this.bitmap2;
        }

        private void refresh_graphics()
        {
            this.pictureBox1.Refresh();
            this.pictureBox2.Refresh();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var variable = this.comboBox1.Text;
            this.variable_number = Array.IndexOf(this.header, variable);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.double_quotes_as_delimiter = this.checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            this.lowercase = this.checkBox2.Checked;
        }

        private void clear_graphics()
        {
            this.pictureBox1.Image = null;
            this.pictureBox1.Refresh();
            this.pictureBox2.Image = null;
            this.pictureBox2.Refresh();
        }

        private bool draw_samples()
        {
            this.samples = new float[this.n_samples, this.sample_size];
            for (int i = 0; i < this.n_samples; i++)
            {
                var current_sample = new float[this.sample_size];
                for (int j = 0; j < this.sample_size; j++)
                {
                    try
                    {
                        current_sample[j] = float.Parse(data[generator.Next(0, this.n_data), this.variable_number]);
                    }
                    catch(Exception e)
                    {
                        return false;
                    }
                }
                for (int j = 0; j < this.sample_size; j++)
                {
                    this.samples[i, j] = current_sample[j];
                }
            }
            return true;
        }

        private float mean(float[] values)
        {
            float mean = 0;
            var length = values.Length;
            for (int i = 0; i < length; i++)
            {
                mean = ((mean * i) + values[i]) / (i + 1);
            }
            return mean;
        }

        private float variance(float[] values, float mean)
        {
            float variance = 0;
            var length = values.Length;
            for (int i = 0; i < length; i++)
            {
                variance = ((variance * i) + (float)Math.Pow(values[i] - mean, 2)) / (i + 1);
            }
            return variance;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.initialize_attributes();
            this.set_images();
            //this.richTextBox1.Text += $"{this.log_delimiter} begin mean distribution computation {this.log_delimiter}\n";

            this.mean_distribution = new Hashtable();
            this.samples_means = new float[this.n_samples];
            var means_sum = 0;
            var max_mean = 0;

            if (!this.draw_samples())
            {
                this.richTextBox1.Text += $"{this.log_delimiter} ERROR TRYING STRING-FLOAT PARSING {this.log_delimiter}\n";
                return;
            }

            for (int i = 0; i < this.n_samples; i++)
            {
                float current_sum = 0;
                for (int j = 0; j < this.sample_size; j++)
                {
                    current_sum += this.samples[i, j];
                }
                var current_mean = (int)Math.Round((float)current_sum / this.sample_size);
                this.samples_means[i] = current_mean;
                if (mean_distribution.ContainsKey(current_mean))
                {
                    mean_distribution[current_mean] = (int)mean_distribution[current_mean] + 1;
                }
                else
                {
                    mean_distribution.Add(current_mean, 1);
                }
                means_sum += current_mean;
                max_mean = current_mean > max_mean ? current_mean : max_mean;
            }
            var means = this.mean_distribution.Values;
            var n_means = mean_distribution.Count;
            var distinct_samples_means = this.samples_means.Distinct().OrderBy(x => x).ToArray();

            // this.richTextBox1.Text += $"{this.log_delimiter} mean distribution successfully computed {this.log_delimiter}\n";
            // this.richTextBox1.Text += $"{this.log_delimiter} begin mean histogram computation {this.log_delimiter}\n";

            var vertical_bar_width = this.pictureBox1.Width / n_means;
            var pen = new Pen(Color.Black, 1);
            int k = 0;
            foreach (int mean in distinct_samples_means)
            {
                var current_value = (int)mean_distribution[mean];
                var vertical_bar_height = (int)Math.Round(((float)current_value / max_mean) * this.pictureBox1.Height);
                var vertical_bar = new Rectangle(vertical_bar_width * k, this.pictureBox1.Height - vertical_bar_height, vertical_bar_width, vertical_bar_height);
                this.graphics1.DrawRectangle(pen, vertical_bar);
                k++;
            }

            // this.richTextBox1.Text += $"{this.log_delimiter} mean histogram successfully computed {this.log_delimiter}\n";
            // this.richTextBox1.Text += $"{this.log_delimiter} begin variance distribution computation {this.log_delimiter}\n";

            this.variance_distribution = new Hashtable();
            this.samples_variances = new float[this.n_samples];
            var variances_sum = 0;
            var max_variance = 0;

            for (int i = 0; i < this.n_samples; i++)
            {
                float current_sum = 0;
                for (int j = 0; j < this.sample_size; j++)
                {
                    current_sum += (float)Math.Pow(this.samples[i, j] - this.samples_means[i], 2);
                }
                var current_variance = (int)Math.Round((float)current_sum / this.sample_size);
                this.samples_variances[i] = current_variance;
                if (variance_distribution.ContainsKey(current_variance))
                {
                    variance_distribution[current_variance] = (int)variance_distribution[current_variance] + 1;
                }
                else
                {
                    variance_distribution.Add(current_variance, 1);
                }
                variances_sum += current_variance;
                max_variance = current_variance > max_variance ? current_variance : max_variance;
            }

            var variances = this.variance_distribution.Values;
            var n_variances = variance_distribution.Count;
            var distinct_samples_variances = this.samples_variances.Distinct().OrderBy(x => x).ToArray();
            // this.richTextBox1.Text += $"{this.log_delimiter} variance distribution successfully computed {this.log_delimiter}\n";

            // this.richTextBox1.Text += $"{this.log_delimiter} begin variance histogram computation {this.log_delimiter}\n";
            vertical_bar_width = this.pictureBox2.Width / n_variances;
            k = 0;
            foreach (int variance in distinct_samples_variances)
            {
                var current_value = (int)variance_distribution[variance];
                var vertical_bar_height = (int)Math.Round(((float)current_value / max_variance) * this.pictureBox2.Height);
                var vertical_bar = new Rectangle(vertical_bar_width * k, this.pictureBox2.Height - vertical_bar_height, vertical_bar_width, vertical_bar_height);
                this.graphics2.DrawRectangle(pen, vertical_bar);
                k++;
            }
            // this.richTextBox1.Text += $"{this.log_delimiter} variance histogram successfully computed {this.log_delimiter}\n";

            var means_mean = this.mean(samples_means);
            var variances_mean = this.mean(samples_variances);
            var means_variance = this.variance(samples_means, means_mean);
            var variances_variance = this.variance(samples_variances, variances_mean);

            float population_mean = 0;
            for (int i = 0; i < this.n_data; i++)
            {
                population_mean += float.Parse(this.data[i, this.variable_number]);
            }
            population_mean /= this.n_data;

            float population_variance = 0;
            for (int i = 0; i < this.n_data; i++)
            {
                population_variance += (float)Math.Pow(float.Parse(this.data[i, this.variable_number]) - population_mean, 2);
            }
            population_variance /= this.n_data;

            this.richTextBox1.Text += $"{this.log_delimiter} population mean = {population_mean} | sampling mean = {means_mean} {this.log_delimiter}\n";
            this.richTextBox1.Text += $"{this.log_delimiter} population variance = {population_variance} | sampling variance = {variances_mean} {this.log_delimiter}\n";

            this.refresh_graphics();
        }
    }
}
