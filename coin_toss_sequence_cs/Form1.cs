using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Linq;

namespace coin_toss_sequence_cs
{
    public partial class Form1 : Form
    {
        private string distribution_type = "BINOMIAL";
        private int n_tosses, n_sequence;
        private double p_success;
        private double lambda = 1;
        private readonly Random generator = new Random();
        private Bitmap bitmap1, bitmap2, bitmap3, bitmap4;
        private Graphics graphics1, graphics2, graphics3, graphics4;
        private GraphicsPath graphics_path1, graphics_path2, graphics_path3;
        private readonly byte type = 1;
        private readonly string log_delimiter = "*****";
        public Form1()
        {
            InitializeComponent();
            this.initialize_attributes();
        }

        private void initialize_attributes()
        {
            this.n_tosses = this.trackBar1.Value;
            this.p_success = (double)(this.trackBar2.Value) / 100;
            this.n_sequence = this.trackBar3.Value;
        }

        private void initialize_graphics()
        {
            this.bitmap1 = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.graphics1 = Graphics.FromImage(this.bitmap1);
            this.bitmap2 = new Bitmap(this.pictureBox2.Width, this.pictureBox2.Height);
            this.graphics2 = Graphics.FromImage(this.bitmap2);
            this.bitmap3 = new Bitmap(this.pictureBox3.Width, this.pictureBox3.Height);
            this.graphics3 = Graphics.FromImage(this.bitmap3);
            this.bitmap4 = new Bitmap(this.pictureBox4.Width, this.pictureBox4.Height);
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.distribution_type = comboBox1.Text;
            if (this.distribution_type.Equals("BINOMIAL"))
            {
                this.trackBar2.Minimum = 0;
                this.trackBar2.Maximum = 100;
                this.trackBar2.Value = 50;
                this.trackBar3.Value = 50;
                this.trackBar3.Enabled = true;
                this.trackBar3_Scroll(this.comboBox1, new EventArgs());
                this.label6.Text = "SUCCESS HISTOGRAM";
                this.label6.Location = new System.Drawing.Point(93, 389);
            }
            else //if (this.distribution_type.Equals("POISSON")
            {
                this.trackBar2.Minimum = 1;
                this.trackBar2.Maximum = (int)Math.Round((float)this.n_tosses / 2);
                this.trackBar2.Value = 1;
                this.trackBar3.Value = 1;
                this.trackBar3.Enabled = false;
                this.trackBar3_Scroll(this.comboBox1, new EventArgs());
                this.label6.Text = "INTERARRIVAL TIMES HISTOGRAM";
                this.label6.Location = new System.Drawing.Point(75, 389);
            }
            trackBar2_Scroll(this.comboBox1, new EventArgs());
        }

        private void draw_paths()
        {
            this.graphics1.DrawPath(Pens.Black, this.graphics_path1);
            this.graphics2.DrawPath(Pens.Black, this.graphics_path2);
            this.graphics3.DrawPath(Pens.Black, this.graphics_path3);
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
            this.initialize_graphics();
            this.set_images();
            this.progressBar1.Value = this.progressBar1.Minimum;
            Hashtable frequencies = new Hashtable();
            Hashtable interarrival_times = new Hashtable();

            for (int j = 0; j < this.n_sequence; j++)
            {
                var n_success = 0;
                var n_unsuccess = 0;
                var points1 = new List<PointF>();
                var points2 = new List<PointF>();
                var points3 = new List<PointF>();
                var types = new List<byte>();
                var counter = 0;
                int i = 0;

                this.richTextBox1.Text += $"{this.log_delimiter} ({j + 1}) begin coin tossing and frequency computation (p = {this.p_success}; n = {this.n_tosses}) {this.log_delimiter}\n";
                for (i = 0; i < this.n_tosses; i++)
                {
                    var current_toss = (int)this.toss();
                    n_success += current_toss;

                    if (this.distribution_type.Equals("POISSON"))
                    {
                        if (current_toss == 1)
                        {
                            if (interarrival_times.ContainsKey(counter))
                            {
                                interarrival_times[counter] = (int)interarrival_times[counter] + 1;
                            }
                            else
                            {
                                interarrival_times.Add(counter, 1);
                            }
                            counter = 0;
                        }
                        else
                        {
                            counter++;
                        }
                    }

                    var x1 = this.transform_x(i, 0, this.n_tosses, this.pictureBox1.Width);
                    var y1 = this.transform_y(n_success, 0, this.n_tosses, this.pictureBox1.Height);
                    points1.Add(new PointF(x1, y1));

                    var x2 = this.transform_x(i, 0, this.n_tosses, this.pictureBox2.Width);
                    var y2 = this.transform_y((float)n_success / (i + 1), 0, 1, this.pictureBox2.Height);
                    points2.Add(new PointF(x2, y2));

                    var x3 = this.transform_x(i, 0, this.n_tosses, this.pictureBox3.Width);
                    var y3 = this.transform_y(n_success / (float)Math.Sqrt(i + 1), 0, (float)Math.Sqrt(this.n_tosses), this.pictureBox3.Height);
                    points3.Add(new PointF(x3, y3));

                    types.Add(this.type);
                }
                n_unsuccess = this.n_tosses - n_success;

                if (this.distribution_type.Equals("BINOMIAL"))
                {
                    if (frequencies.ContainsKey(n_success))
                    {
                            frequencies[n_success] = (int)frequencies[n_success] + 1;
                    }
                    else
                    {
                            frequencies.Add(n_success, 1);
                    }
                }
                this.richTextBox1.Text += $"{this.log_delimiter} ({j + 1}) frequency successfully computed (#success = {n_success}; #unsuccess = {n_unsuccess}) {this.log_delimiter}\n";

                this.graphics_path1 = new GraphicsPath(points1.ToArray(), types.ToArray());
                this.graphics_path2 = new GraphicsPath(points2.ToArray(), types.ToArray());
                this.graphics_path3 = new GraphicsPath(points3.ToArray(), types.ToArray());

                this.draw_paths();

                if (this.distribution_type.Equals("POISSON"))
                {
                    this.richTextBox1.Text += $"{this.log_delimiter} begin interarrival times histogram computation {this.log_delimiter}\n";
                    compute_histogram(interarrival_times, this.pictureBox4, this.graphics4, this.progressBar1, compute_max_value(interarrival_times));
                    this.richTextBox1.Text += $"{this.log_delimiter} interarrival times histogram successfully computed {this.log_delimiter}\n";
                }
                this.progressBar1.Value = (int)Math.Round(((double)(j + 1) / (this.n_sequence)) * this.progressBar1.Maximum);
            }
            
            if (this.distribution_type.Equals("BINOMIAL"))
            {
                this.richTextBox1.Text += $"{this.log_delimiter} begin success histogram computation {this.log_delimiter}\n";
                compute_histogram(frequencies, this.pictureBox4, this.graphics4, this.progressBar1, frequencies.Count);
                this.richTextBox1.Text += $"{this.log_delimiter} success histogram successfully computed {this.log_delimiter}\n";
            }

            this.refresh_graphics();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.label1.Text = "# COIN TOSSES: " + this.trackBar1.Value;
            this.n_tosses = this.trackBar1.Value;
            if (this.distribution_type.Equals("POISSON"))
            {
                this.trackBar2.Maximum = (int)Math.Round((float)this.n_tosses / 2);
                this.trackBar2.Value = 1;
                trackBar2_Scroll(this.trackBar1, new EventArgs());
            }
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

        private int compute_max_value(Hashtable hashtable)
        {
            int res = 0;
            foreach (int key in hashtable.Keys)
            {
                res = Math.Max(res, (int)hashtable[key]);
            }
            return res;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (this.distribution_type.Equals("BINOMIAL"))
            {
                this.p_success = (double)(this.trackBar2.Value) / 100;
                this.label2.Text = "SUCCESS PROBABILITY: " + Math.Round(this.p_success * 100) + "%";
            }
            else //if (this.distribution_type.Equals("POISSON"))
            {
                this.lambda = (double)(this.trackBar2.Value);
                this.p_success = (double)(this.lambda) / this.n_tosses;
                this.label2.Text = "ARRIVAL RATE: " + Math.Round(this.lambda);
            }
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            this.label7.Text = "# SEQUENCES: " + this.trackBar3.Value;
            this.n_sequence = this.trackBar3.Value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.clear_graphics();
            this.richTextBox1.Clear();
            this.progressBar1.Value = this.progressBar1.Minimum;
        }
    }
}