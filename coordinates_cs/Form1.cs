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
using System.Windows.Forms.VisualStyles;

namespace coordinates_cs
{
    public partial class Form1 : Form
    {
        private int n_coordinates;
        private (float, float)[] polar_coordinates;
        private (float, float)[] cartesian_coordinates;
        private Hashtable quadrant_distribution;
        private Hashtable x_distribution, y_distribution;
        private int n_intervals;
        private Random generator = new Random();
        private Bitmap bitmap1, bitmap2, bitmap3, bitmap4;
        private Graphics graphics1, graphics2, graphics3, graphics4;
        public Form1()
        {
            InitializeComponent();
            this.initialize_attributes();
        }

        private void initialize_graphics()
        {
            this.bitmap1 = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.bitmap2 = new Bitmap(this.pictureBox2.Width, this.pictureBox2.Height);
            this.bitmap3 = new Bitmap(this.pictureBox3.Width, this.pictureBox3.Height);
            this.bitmap4 = new Bitmap(this.pictureBox4.Width, this.pictureBox4.Height);
            this.graphics1 = Graphics.FromImage(this.bitmap1);
            this.graphics2 = Graphics.FromImage(this.bitmap2);
            this.graphics3 = Graphics.FromImage(this.bitmap3);
            this.graphics4 = Graphics.FromImage(this.bitmap4);
        }

        private void set_images()
        {
            this.pictureBox1.Image = this.bitmap1;
            this.pictureBox2.Image = this.bitmap2;
            this.pictureBox3.Image = this.bitmap3;
            this.pictureBox4.Image = this.bitmap4;
        }

        private void refresh_graphics()
        {
            this.pictureBox1.Refresh();
            this.pictureBox2.Refresh();
            this.pictureBox3.Refresh();
            this.pictureBox4.Refresh();
        }

        private void clear_graphics()
        {
            this.pictureBox1.Image = null;
            this.pictureBox1.Refresh();
            this.pictureBox2.Image = null;
            this.pictureBox2.Refresh();
            this.pictureBox3.Image = null;
            this.pictureBox3.Refresh();
            this.pictureBox4.Image = null;
            this.pictureBox4.Refresh();
        }

        private void initialize_attributes()
        {
            this.trackBar1_Scroll(this, new EventArgs());
            this.trackBar2_Scroll(this, new EventArgs());
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.n_coordinates = this.trackBar1.Value;
            this.label1.Text = "# COORDINATES: " + this.n_coordinates;
        }

        private int get_quadrant((float, float) cartesian_coordinates)
        {
            float x = cartesian_coordinates.Item1;
            float y = cartesian_coordinates.Item2;
            if (x == 0)
            {
                if (y == 0) return 0;
                else if (y > 0) return 11;
                else return 33;
            }
            else if (x > 0)
            {
                if (y == 0) return 44;
                else if (y > 0) return 1;
                else return 4;
            }
            else
            {
                if (y == 0) return 22;
                else if (y > 0) return 2;
                else return 3;
            }
        }

        private (float, float) generate_polar_coordinates()
        {
            float radius;
            float angle;
            radius = (float)this.generator.NextDouble();
            angle = (float)this.generator.NextDouble() * 2 * (float)Math.PI;
            return (radius, angle);
        }

        private (float, float) get_cartesian_coordinates((float, float) polar_coordinates)
        {
            float x;
            float y;
            x = polar_coordinates.Item1 * (float)Math.Cos(polar_coordinates.Item2);
            y = polar_coordinates.Item1 * (float)Math.Sin(polar_coordinates.Item2);
            return (x, y);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            this.n_intervals = this.trackBar2.Value;
            this.label6.Text = "# INTERVALS: " + this.n_intervals;
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.initialize_graphics();
            this.set_images();
            this.polar_coordinates = new (float, float)[this.n_coordinates];
            this.cartesian_coordinates = new (float, float)[this.n_coordinates];
            this.quadrant_distribution = new Hashtable();
            this.x_distribution = new Hashtable();
            this.y_distribution = new Hashtable();

            for (int i = 0; i < this.n_coordinates; i++)
            {
                this.polar_coordinates[i] = this.generate_polar_coordinates();
                this.cartesian_coordinates[i] = this.get_cartesian_coordinates(this.polar_coordinates[i]);
                var x = this.cartesian_coordinates[i].Item1;
                var y = this.cartesian_coordinates[i].Item2;
                var transformed_x = this.transform_x(x, -1, 1, this.pictureBox1.Width);
                var transformed_y = this.transform_y(y, -1, 1, this.pictureBox1.Height);
                var quadrant = this.get_quadrant(this.cartesian_coordinates[i]);
                var x_interval = this.get_interval(x, -1, 1, this.n_intervals);
                var y_interval = this.get_interval(y, -1, 1, this.n_intervals);
                graphics1.FillRectangle(Brushes.Black, transformed_x, transformed_y, 1, 1);
                this.update_distribution(this.quadrant_distribution, quadrant);
                this.update_distribution(this.x_distribution, x_interval);
                this.update_distribution(this.y_distribution, y_interval);
                // this.richTextBox1.Text += $"q:{quadrant} | (x, y)=({this.cartesian_coordinates[i]}, {cartesian_coordinates[i]}\n";
            }

            compute_histogram(x_distribution, this.pictureBox3, this.graphics3, this.progressBar1, this.n_coordinates);
            compute_histogram(y_distribution, this.pictureBox4, this.graphics4, this.progressBar1, this.n_coordinates);
            compute_histogram(quadrant_distribution, this.pictureBox2, this.graphics2, this.progressBar1, this.n_coordinates);
            this.refresh_graphics();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.clear_graphics();
            this.richTextBox1.Clear();
        }
        private float transform_x(float x, float x_min, float x_max, float width)
        {
            return ((float)(x - x_min) / (x_max - x_min)) * width;
        }

        private float transform_y(float y, float y_min, float y_max, float height)
        {
            return height - ((float)(y - y_min) / (y_max - y_min)) * height;
        }

        private void compute_histogram(Hashtable hashtable, PictureBox picture_box, Graphics graphics, ProgressBar progress_bar, int n)
        {
            progress_bar.Value = progress_bar.Minimum;
            var i = 0;
            var n_hashtable = hashtable.Count;
            var bar_width = (int)Math.Round((float)picture_box.Width / n_hashtable);
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
    }
}
