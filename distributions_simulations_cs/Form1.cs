using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;

namespace distributions_simulations_cs
{
    public partial class Form1 : Form
    {
        private int n_values;
        private float[] values;
        private Hashtable distribution;
        private string distribution_type;
        private int n_intervals;
        private int n_degrees_1;
        private int n_degrees_2;
        private Random generator = new Random();
        private Bitmap bitmap1;
        private Graphics graphics1;
        private string delimiter = "*****";

        public Form1()
        {
            InitializeComponent();
            this.initialize_attributes();
        }

        private void initialize_graphics()
        {
            this.bitmap1 = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.graphics1 = Graphics.FromImage(this.bitmap1);
        }

        private void set_images()
        {
            this.pictureBox1.Image = this.bitmap1;
        }

        private void refresh_graphics()
        {
            this.pictureBox1.Refresh();
        }

        private void clear_graphics()
        {
            this.pictureBox1.Image = null;
            this.pictureBox1.Refresh();
        }

        private void initialize_attributes()
        {
            this.trackBar1_Scroll(this, new EventArgs());
            this.trackBar2_Scroll(this, new EventArgs());
            this.trackBar3_Scroll(this, new EventArgs());
            this.trackBar4_Scroll(this, new EventArgs());
            this.comboBox1_SelectedIndexChanged(this, new EventArgs());
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.n_values = this.trackBar1.Value;
            this.label1.Text = "# VALUES: " + this.n_values;
        }

        private int generate_binary_bernoulli_sign(float p)
        {
            var tmp = this.generator.NextDouble();
            if (tmp <= p)
            {
                return 1;
            }
            else return -1;
        }

        private (float, float) generate_iid_normal_values()
        {
            float normal_value_1, normal_value_2;
            float s, x, y, c;
            do
            {
                x = (float)this.generator.NextDouble() * this.generate_binary_bernoulli_sign(0.5f);
                y = (float)this.generator.NextDouble() * this.generate_binary_bernoulli_sign(0.5f);
                s = (float)Math.Pow(x, 2) + (float)Math.Pow(y, 2);

            } while (!(s > 0 && s < 1));
            c = (float)Math.Sqrt(-2 * Math.Log(s)) / (float)Math.Sqrt(s);
            normal_value_1 = x * c;
            normal_value_2 = y * c;
            return (normal_value_1, normal_value_2);
        }

        private float generate_normal_value()
        {
            float uniform_value_1 = (float)this.generator.NextDouble();
            float uniform_value_2 = (float)this.generator.NextDouble();
            float normal_value = Math.Abs(uniform_value_1 * (float)Math.Cos(uniform_value_2));
            return normal_value;
        }
        private float generate_chi_squared_value(int n_degrees)
        {
            var chi_squared_value = 0.0f;
            var normal_squared_values = new float[n_degrees];
            int i;
            for (i = 0; i < n_degrees; i ++)
            {
                normal_squared_values[i] = (float)Math.Pow(this.generate_normal_value(), 2);
                chi_squared_value += normal_squared_values[i];
            }
            return chi_squared_value;
        }

        private float generate_students_t_value(int n_degrees)
        {
            var normal_value = this.generate_normal_value();
            var chi_squared_value = this.generate_chi_squared_value(n_degrees);
            var students_t_value = normal_value / (float)Math.Sqrt(chi_squared_value / n_degrees);
            return students_t_value;
        }

        private float generate_fishers_f_value(int n_degrees, int m_degrees)
        {
            var chi_squared_value_1 = this.generate_chi_squared_value(n_degrees) / n_degrees;
            var chi_squared_value_2 = this.generate_chi_squared_value(m_degrees) / m_degrees;
            var fishers_f_value = chi_squared_value_1 / chi_squared_value_2;
            return fishers_f_value;
        }
        private float generate_cauchy_value()
        {
            var normal_iid_values = this.generate_iid_normal_values();
            var normal_value_1 = normal_iid_values.Item1;
            var normal_value_2 = normal_iid_values.Item2;
            var cauchy_value = normal_value_1 / normal_value_2;
            return cauchy_value;
        }

        private void update_distribution(Hashtable distribution, int key)
        {
            if (distribution.ContainsKey(key))
            {
                distribution[key] = (int)distribution[key] + 1;
            }
            else
            {
                distribution.Add(key, 1);
            }
        }

        private int get_interval(float c, float min, float max, int n_intervals)
        {
            var interval_length = (float)Math.Abs(max - min) / n_intervals;
            for (int i = 0; i < n_intervals; i++)
            {
                var current_min = min + interval_length * i;
                var current_max = current_min + interval_length;
                if (c > current_min && c <= current_max) return i;
            }
            return n_intervals - 1;
        }

        private float variance(float[] values, float mean)
        {
            float variance = 0;
            var n = values.Count();
            for (int i = 0; i < n; i++)
            {
                variance = (float)variance * i + (float)Math.Pow(mean - values[i], 2);
                variance /= i + 1;
            }
            return variance;
        }

        private void normalize_values(float[] values, float mean, float variance)
        {
            var n = values.Count();
            for (int i = 0; i < n_degrees_1; i++)
            {
                values[i] = (values[i] - mean) / (float)Math.Sqrt(variance);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.initialize_graphics();
            this.set_images();

            this.values = new float[this.n_values];
            this.distribution = new Hashtable();

            this.richTextBox1.Text += $"{this.delimiter}begin {distribution_type.ToLower()} distribution computation{this.delimiter}\n";
            if (this.distribution_type.Equals("NORMAL"))
            {
                // NORMAL DISTRIBUTION
                for (int i = 0; i < this.n_values; i++) this.values[i] = this.generate_normal_value();
            }
            else if (this.distribution_type.Equals("CHI SQUARED"))
            {
                // CHI SQUARED DISTRIBUTION
                for (int i = 0; i < this.n_values; i++) this.values[i] = this.generate_chi_squared_value(this.n_degrees_1);
            }
            else if (this.distribution_type.Equals("STUDENT'S T"))
            {
                // STUDENT'S T DISTRIBUTION
                for (int i = 0; i < this.n_values; i++) this.values[i] = this.generate_students_t_value(this.n_degrees_1);
            }
            else if (this.distribution_type.Equals("FISHER'S F"))
            {
                // FISHER'S F DISTRIBUTION
                for (int i = 0; i < this.n_values; i++) this.values[i] = this.generate_fishers_f_value(this.n_degrees_1, this.n_degrees_2);
            }
            else if (this.distribution_type.Equals("CAUCHY"))
            {
                // CAUCHY DISTRIBUTION
                for (int i = 0; i < this.n_values; i++) this.values[i] = this.generate_cauchy_value();
            }
            else
            {
                return;
            }

            var mean = this.values.Average();
            var variance = this.variance(values, mean);
            this.normalize_values(this.values, mean, variance);
            var max = this.values.Max();
            var min = this.values.Min();
            for (int i = 0; i < this.n_values; i++)
            {
                var interval = this.get_interval(this.values[i], min, max, this.n_intervals);
                this.update_distribution(this.distribution, interval);
            }
            this.richTextBox1.Text += $"{this.delimiter}{distribution_type.ToLower()} distribution successfully computed{this.delimiter}\n";

            compute_histogram(distribution, this.pictureBox1, this.graphics1, this.progressBar1, this.max_distribution_value(this.distribution));
            this.refresh_graphics();
        }

        private int max_distribution_value(Hashtable distribution)
        {
            var max = Int32.MinValue;
            foreach (int key in distribution.Keys)
            {
                if ((int)distribution[key] > max) max = (int)distribution[key];
            }
            return max;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.clear_graphics();
            this.progressBar1.Value = this.progressBar1.Minimum;
            this.richTextBox1.Clear();
        }

        private void compute_histogram(Hashtable hashtable, PictureBox picture_box, Graphics graphics, ProgressBar progress_bar, int n)
        {
            progress_bar.Value = progress_bar.Minimum;
            var i = 0;
            var n_hashtable = hashtable.Count;
            var bar_width = (int)Math.Ceiling((float)picture_box.Width / n_hashtable);
            var ordered_hashtable_keys = hashtable.Keys.Cast<int>().OrderBy(c => c);
            var ordered_hashtable = from k in ordered_hashtable_keys select new { key = k, value = hashtable[k] };
            foreach (int key in ordered_hashtable_keys)
            {
                int value = (int)hashtable[key];
                var bar_height = (int)Math.Round(((float)value / n) * picture_box.Height);
                var bar = new Rectangle(bar_width * i++, picture_box.Height - bar_height, bar_width, bar_height);
                graphics.DrawRectangle(Pens.Black, bar);
                progress_bar.Value = (int)Math.Round(((double)i / (n_hashtable)) * progress_bar.Maximum);
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            this.n_degrees_1 = this.trackBar2.Value;
            this.label2.Text = "# DEGREES: " + this.n_degrees_1;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            this.n_degrees_2 = this.trackBar3.Value;
            this.label3.Text = "# DEGREES: " + this.n_degrees_2;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.distribution_type = this.comboBox1.Text;
            if (this.distribution_type.Equals("NORMAL") || this.distribution_type.Equals("CAUCHY"))
            {
                this.trackBar2.Enabled = false;
                this.trackBar3.Enabled = false;
            }
            else if (this.distribution_type.Equals("STUDENT'S T") || this.distribution_type.Equals("CHI SQUARED"))
            {
                this.trackBar2.Enabled = true;
                this.trackBar3.Enabled = false;
            }
            else if (this.distribution_type.Equals("FISHER'S F"))
            {
                this.trackBar2.Enabled = true;
                this.trackBar3.Enabled = true;
            }
            else
            {
                this.richTextBox1.Text += "ERROR";
            }
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            this.n_intervals = this.trackBar4.Value;
            this.label4.Text = "# INTERVALS: " + this.n_intervals;
        }
    }
}
