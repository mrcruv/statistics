using System.Reflection;

namespace coin_toss_sequence_cs
{
    public partial class Form1 : Form
    {
        private int n_tosses;
        private double p_success;
        private Random generator = new Random();
        private Bitmap bitmap1, bitmap2, bitmap3, bitmap4;
        private Graphics graphics1, graphics2, graphics3, graphics4;
        private readonly string log_delimiter = "*****";
        public Form1()
        {
            InitializeComponent();
        }

        private float transform_x(float x, float x_min, float x_max, float width)
        {
            return ((float)(x - x_min) / (x_max - x_min)) * width;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

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
            this.bitmap1 = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.graphics1 = Graphics.FromImage(this.bitmap1);
            this.bitmap2 = new Bitmap(this.pictureBox2.Width, this.pictureBox2.Height);
            this.graphics2 = Graphics.FromImage(this.bitmap2);
            this.bitmap3 = new Bitmap(this.pictureBox3.Width, this.pictureBox3.Height);
            this.graphics3 = Graphics.FromImage(this.bitmap3);
            this.bitmap4 = new Bitmap(this.pictureBox4.Width, this.pictureBox4.Height);
            this.graphics4 = Graphics.FromImage(this.bitmap4);
            this.n_tosses = int.Parse(this.textBox2.Text);
            this.p_success = double.Parse(this.textBox1.Text) / 100;
            var n_success = 0;
            this.richTextBox1.Text += $"{this.log_delimiter} begin coin tossing and frequency computation (p = {this.p_success}; n = {this.n_tosses}) {this.log_delimiter}\n";
            for (int i = 0; i < this.n_tosses; i++)
            {
                n_success += (int)this.toss();
                var x1 = this.transform_x(i, 0, this.n_tosses, this.pictureBox1.Width);
                var x2 = x1;
                var y1 = this.transform_y(0, 0, this.n_tosses, this.pictureBox1.Height);
                var y2 = this.transform_y(n_success, 0, this.n_tosses, this.pictureBox1.Height);
                this.graphics1.DrawLine(Pens.Black, x1, y1, x2, y2);

                x1 = this.transform_x(i, 0, this.n_tosses, this.pictureBox2.Width);
                x2 = x1;
                y1 = this.transform_y(0, 0, 1, this.pictureBox2.Height);
                y2 = this.transform_y((float)n_success / (i + 1), 0, 1, this.pictureBox2.Height);
                this.graphics2.DrawLine(Pens.Black, x1, y1, x2, y2);

                x1 = this.transform_x(i, 0, this.n_tosses, this.pictureBox3.Width);
                x2 = x1;
                y1 = this.transform_y(0, 0, (float)Math.Sqrt(this.n_tosses), this.pictureBox3.Height);
                y2 = this.transform_y(n_success / (float)Math.Sqrt(i + 1), 0, (float)Math.Sqrt(this.n_tosses), this.pictureBox3.Height);
                this.graphics3.DrawLine(Pens.Black, x1, y1, x2, (float)(y2 / (Math.Sqrt(i + 1))));
            }
            var n_unsuccess = this.n_tosses - n_success;
            this.richTextBox1.Text += $"{this.log_delimiter} frequency succesfully computed (#success = {n_success}; #unsuccess = {n_unsuccess}) {this.log_delimiter}\n";
            this.richTextBox1.Text += $"{this.log_delimiter} begin histogram computation {this.log_delimiter}\n";
            var bar_width = this.pictureBox4.Width / 10;
            var center = (int)((float)this.pictureBox4.Width / 2);
            var success_height = (int)(((float)n_success / this.n_tosses) * this.pictureBox4.Height);
            var unsuccess_height = (int)(((float)n_unsuccess / this.n_tosses) * this.pictureBox4.Height);
            var success_bar = new Rectangle(center - bar_width, this.pictureBox4.Height - success_height, bar_width, success_height);
            var unsuccess_bar = new Rectangle(center, this.pictureBox4.Height - unsuccess_height, bar_width, unsuccess_height);
            this.graphics4.DrawRectangle(Pens.Black, success_bar);
            this.graphics4.DrawRectangle(Pens.Black, unsuccess_bar);
            this.richTextBox1.Text += $"{this.log_delimiter} histogram succesfully computed {this.log_delimiter}\n";

            this.pictureBox1.Image = this.bitmap1;
            this.pictureBox1.Refresh();
            this.pictureBox2.Image = this.bitmap2;
            this.pictureBox2.Refresh();
            this.pictureBox3.Image = this.bitmap3;
            this.pictureBox3.Refresh();
            this.pictureBox4.Image = this.bitmap4;
            this.pictureBox4.Refresh();
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

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}