using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace histogram_from_distribution_cs
{
    public partial class Form1 : Form
    {
        private readonly Random generator = new Random();
        private Bitmap bitmap1, bitmap2;
        private Graphics graphics1, graphics2;
        private int n_keys;
        private int sum;
        private Hashtable distribution;
        private readonly string log_delimiter = "*****";

        public Form1()
        {
            InitializeComponent();
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

        private void clear_graphics()
        {
            this.pictureBox1.Image = null;
            this.pictureBox1.Refresh();
            this.pictureBox2.Image = null;
            this.pictureBox2.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.initialize_attributes();
            this.set_images();

            this.richTextBox1.Text += $"{this.log_delimiter} begin random distribution computation {this.log_delimiter}\n";

            this.n_keys = this.generator.Next(15)+6;
            distribution = new Hashtable();
            this.progressBar1.Value = this.progressBar1.Minimum;
            this.sum = 0;

            for (int i = 0; i < this.n_keys; i++)
            {
                var current_value = this.generator.Next(1000);
                this.sum += current_value;
                distribution.Add(i, current_value);
                this.progressBar1.Value = (int)Math.Round(((double)(i + 1) / (this.n_keys)) * this.progressBar1.Maximum);
            }

            this.richTextBox1.Text += $"{this.log_delimiter} random distribution successfully computed ({this.n_keys} keys) {this.log_delimiter}\n";

            this.richTextBox1.Text += $"{this.log_delimiter} begin histogram computation {this.log_delimiter}\n";

            var vertical_bar_width = this.pictureBox1.Width / this.n_keys;
            var horizontal_bar_height = this.pictureBox2.Height / this.n_keys;
            this.progressBar1.Value = this.progressBar1.Minimum;
            var pen = new Pen(Color.Black, 2);
            for (int i = 0; i < n_keys; i++)
            {
                var current_value = (int)distribution[i];
                var vertical_bar_height = (int)Math.Round(((float)current_value / this.sum) * this.pictureBox1.Height);
                var vertical_bar = new Rectangle(vertical_bar_width * i, this.pictureBox1.Height - vertical_bar_height, vertical_bar_width, vertical_bar_height);
                var horizontal_bar_width = (int)Math.Round(((float)current_value / this.sum) * this.pictureBox2.Width);
                var horizontal_bar = new Rectangle(0, horizontal_bar_height * i, horizontal_bar_width, horizontal_bar_height);
                this.graphics1.DrawRectangle(pen, vertical_bar);
                this.graphics2.DrawRectangle(pen, horizontal_bar);
                this.progressBar1.Value = (int)Math.Round(((double)(i + 1) / (this.n_keys)) * this.progressBar1.Maximum);
            }
            this.richTextBox1.Text += $"{this.log_delimiter} histogram successfully computed {this.log_delimiter}\n";
            this.refresh_graphics();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.clear_graphics();
            this.richTextBox1.Clear();
            this.progressBar1.Value = this.progressBar1.Minimum;
        }
    }
}
