using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Xml.Linq;

namespace coin_toss_sequence_cs
{
    public partial class Form1 : Form
    {
        private int n_tosses, n_sequence;
        private double p_success;
        private readonly Random generator = new Random();
        private Bitmap bitmap1, bitmap2, bitmap3, bitmap4;
        private Graphics graphics1, graphics2, graphics3, graphics4;
        private readonly byte type = 1;
        private readonly string log_delimiter = "*****";
        public Form1()
        {
            InitializeComponent();
        }

        private void initialize_attributes()
        {
            this.n_tosses = this.trackBar1.Value;
            this.p_success = (double)(this.trackBar2.Value) / 100;
            this.n_sequence = this.trackBar3.Value;
            this.bitmap1 = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.graphics1 = Graphics.FromImage(this.bitmap1);
            this.bitmap2 = new Bitmap(this.pictureBox2.Width, this.pictureBox2.Height);
            this.graphics2 = Graphics.FromImage(this.bitmap2);
            this.bitmap3 = new Bitmap(this.pictureBox3.Width, this.pictureBox3.Height);
            this.graphics3 = Graphics.FromImage(this.bitmap3);
            this.bitmap4 = new Bitmap(this.pictureBox4.Width, this.pictureBox4.Height);
            this.graphics4 = Graphics.FromImage(this.bitmap4);
        }

        private void clear()
        {
            this.pictureBox1.Image = this.bitmap1;
            this.pictureBox1.Refresh();
            this.pictureBox2.Image = this.bitmap2;
            this.pictureBox2.Refresh();
            this.pictureBox3.Image = this.bitmap3;
            this.pictureBox3.Refresh();
            this.pictureBox4.Image = this.bitmap4;
            this.pictureBox4.Refresh();
        }

        private float transform_x(float x, float x_min, float x_max, float width)
        {
            return ((float)(x - x_min) / (x_max - x_min)) * width;
        }

        private float transform_y(float y, float y_min, float y_max, float height)
        {
            return height - ((float)(y - y_min) / (y_max - y_min)) * height;
        }

        private byte toss()
        {
            var toss = this.generator.NextDouble();
            if (toss <= this.p_success)
            {
                toss = 1;
            }
            else
            {
                toss = 0;
            }
            return (byte)toss;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.initialize_attributes();
            int[] n_success = new int[this.n_sequence], n_unsuccess = new int[this.n_sequence];
            this.progressBar1.Value = this.progressBar1.Minimum;

            for (int j = 0; j < this.n_sequence; j++)
            {
                this.richTextBox1.Text += $"{this.log_delimiter} ({j + 1}) begin coin tossing and frequency computation (p = {this.p_success}; n = {this.n_tosses}) {this.log_delimiter}\n";
                n_success[j] = 0;
                var points1 = new List<PointF>();
                var points2 = new List<PointF>();
                var points3 = new List<PointF>();
                var types = new List<byte>();
                for (int i = 0; i < this.n_tosses; i++)
                {
                    n_success[j] += (int)this.toss();

                    var x1 = this.transform_x(i, 0, this.n_tosses, this.pictureBox1.Width);
                    var y1 = this.transform_y(n_success[j], 0, this.n_tosses, this.pictureBox1.Height);
                    points1.Add(new PointF(x1, y1));

                    var x2 = this.transform_x(i, 0, this.n_tosses, this.pictureBox2.Width);
                    var y2 = this.transform_y((float)n_success[j] / (i + 1), 0, 1, this.pictureBox2.Height);
                    points2.Add(new PointF(x2, y2));

                    var x3 = this.transform_x(i, 0, this.n_tosses, this.pictureBox3.Width);
                    var y3 = this.transform_y(n_success[j] / (float)Math.Sqrt(i + 1), 0, (float)Math.Sqrt(this.n_tosses), this.pictureBox3.Height);
                    points3.Add(new PointF(x3, y3));

                    types.Add(this.type);
                }
                n_unsuccess[j] = this.n_tosses - n_success[j];

                var graphics_path1 = new GraphicsPath(points1.ToArray(), types.ToArray());
                this.graphics1.DrawPath(Pens.Black, graphics_path1);

                var graphics_path2 = new GraphicsPath(points2.ToArray(), types.ToArray());
                this.graphics2.DrawPath(Pens.Black, graphics_path2);

                var graphics_path3 = new GraphicsPath(points3.ToArray(), types.ToArray());
                this.graphics3.DrawPath(Pens.Black, graphics_path3);

                this.richTextBox1.Text += $"{this.log_delimiter} ({j + 1}) frequency succesfully computed (#success = {n_success[j]}; #unsuccess = {n_unsuccess[j]}) {this.log_delimiter}\n";
                this.progressBar1.Value = (int)Math.Round(((double)(j + 1) / (this.n_sequence)) * this.progressBar1.Maximum);
            }
            this.richTextBox1.Text += $"{this.log_delimiter} begin histogram computation {this.log_delimiter}\n";

            Hashtable frequencies = new Hashtable();
            this.progressBar1.Value = this.progressBar1.Minimum;

            for (int i = 0; i < this.n_sequence; i++)
            {
                var current_value = n_success[i];
                if (frequencies.ContainsKey(current_value))
                {
                    frequencies[current_value] = (int)frequencies[current_value] + 1;
                }
                else
                {
                    frequencies.Add(current_value, 1);
                }
                this.progressBar1.Value = (int)Math.Round(((double)(i + 1) / (this.n_sequence)) * this.progressBar1.Maximum);
            }

            var distinct_success = n_success.Distinct().OrderBy(e => e).ToArray();
            var n_distinct_success = distinct_success.Count();
            var bar_width = this.pictureBox4.Width / n_distinct_success;
            this.progressBar1.Value = this.progressBar1.Minimum;
            for (int i = 0; i < n_distinct_success; i++)
            {
                var index = distinct_success[i];
                var count_sequence = (int)frequencies[index];
                var success_height = (int)Math.Round(((float)count_sequence / this.n_sequence) * this.pictureBox4.Height);
                var success_bar = new Rectangle(bar_width * i, this.pictureBox4.Height - success_height, bar_width, success_height);
                this.graphics4.DrawRectangle(Pens.Black, success_bar);
                this.progressBar1.Value = (int)Math.Round(((double)(i + 1) / (n_distinct_success)) * this.progressBar1.Maximum);
            }
            this.richTextBox1.Text += $"{this.log_delimiter} histogram succesfully computed {this.log_delimiter}\n";
            this.clear();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.label1.Text = "# COIN TOSSES: " + this.trackBar1.Value;
            this.n_tosses = this.trackBar1.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            this.label2.Text = "SUCCESS PROBABILITY: " + this.trackBar2.Value + "%";
            this.p_success = (double)(this.trackBar2.Value) / 100;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            this.label7.Text = "# SEQUENCES: " + this.trackBar3.Value;
            this.n_sequence = this.trackBar3.Value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.pictureBox1.Image = null;
            this.pictureBox1.Refresh();
            this.pictureBox2.Image = null;
            this.pictureBox2.Refresh();
            this.pictureBox3.Image = null;
            this.pictureBox3.Refresh();
            this.pictureBox4.Image = null;
            this.pictureBox4.Refresh();
            this.richTextBox1.Clear();
            this.progressBar1.Value = this.progressBar1.Minimum;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}