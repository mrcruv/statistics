using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rectangle_management_cs
{
    public partial class Form1 : Form
    {
        private Bitmap bitmap1;
        private Graphics graphics1;
        private Rectangle rectangle;
        private readonly Random generator = new Random();
        private Boolean move = false, resize = false;

        private int x_mouse, x_mouse_down, y_mouse, y_mouse_down;
        private readonly double scale_factor = 0.1;

        private void initialize_attributes()
        {
            this.bitmap1 = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.graphics1 = Graphics.FromImage(this.bitmap1);
        }

        private void set_image()
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

        public Form1()
        {
            InitializeComponent();
        }

        private void generate_rectangle()
        {
            var rectangle_width = this.pictureBox1.Width / 5;
            var rectangle_height = this.pictureBox1.Height / 5;
            var x = this.generator.Next(this.pictureBox1.Width - rectangle_width);
            var y = this.generator.Next(this.pictureBox1.Height - rectangle_height);
            this.rectangle = new Rectangle(x, y, rectangle_width, rectangle_height);
        }

        private void rectangle_mouse_up(Object sender, MouseEventArgs e)
        {
            this.move = false;
            this.resize = false;
        }

        private void rectangle_mouse_down(Object sender, MouseEventArgs e)
        {
            if (rectangle.Contains(e.X, e.Y))
            {
                this.x_mouse = e.X;
                this.y_mouse = e.Y;

                this.x_mouse_down = this.rectangle.X;
                this.y_mouse_down = this.rectangle.Y;

                if (e.Button == MouseButtons.Left)
                {
                    this.move = true;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    this.resize = true;
                }
            }
        }

        private bool x_and_y_are_valid(int x, int y, int width, int height)
        {
            if (x <= 0 || x >= this.pictureBox1.Width - width)
            {
                return false;
            }

            if (y <= 0 || y >= this.pictureBox1.Height - height)
            {
                return false;
            }

            return true;
        }

        private bool width_and_height_are_valid(int width, int height)
        {
            if (width <= 0 || width >= this.pictureBox1.Width)
            {
                return false;
            }

            if (height <= 0 || height >= this.pictureBox1.Height)
            {
                return false;
            }

            var new_rectangle = new Rectangle(this.rectangle.X, this.rectangle.Y, width, height);

            return x_and_y_are_valid(new_rectangle.X, new_rectangle.Y, width, height);
        }
        
        private void rectangle_mouse_move(Object sender, MouseEventArgs e)
        {
            var delta_x = e.X - this.x_mouse;
            var delta_y = e.Y - this.y_mouse;

            if (this.move)
            {
                var new_rectangle_x = this.x_mouse_down + delta_x;
                var new_rectangle_y = this.y_mouse_down + delta_y;

                if (!this.x_and_y_are_valid(new_rectangle_x, new_rectangle_y, this.rectangle.Width, this.rectangle.Height)) return;

                this.rectangle.X = new_rectangle_x;
                this.rectangle.Y = new_rectangle_y;

                this.graphics1.Clear(this.pictureBox1.BackColor);
                this.graphics1.DrawRectangle(Pens.Black, this.rectangle);
                this.refresh_graphics();
            }

            if (this.resize)
            {
                var new_rectangle_width = this.rectangle.Width + delta_x;
                var new_rectangle_height = this.rectangle.Height + delta_y;

                if (!width_and_height_are_valid(new_rectangle_width, new_rectangle_height)) return;

                this.rectangle.Width = new_rectangle_width;
                this.rectangle.Height = new_rectangle_height;

                this.graphics1.Clear(this.pictureBox1.BackColor);
                this.graphics1.DrawRectangle(Pens.Black, this.rectangle);
                this.refresh_graphics();
            }
        }

        private void rectangle_zoom(Object sender, MouseEventArgs e)
        {
            if (this.rectangle.Contains(e.X, e.Y))
            {
                this.x_mouse_down = this.rectangle.X;
                this.y_mouse_down = this.rectangle.Y;

                var zoom = (int)(e.Delta * this.scale_factor);

                this.rectangle.Width += zoom;
                this.rectangle.Height += zoom;

                this.rectangle.X = x_mouse_down - (int)((float)zoom / 2);
                this.rectangle.Y = y_mouse_down - (int)((float)zoom / 2);
            }
        }

        private void associate_handlers()
        {
            this.pictureBox1.MouseUp += new MouseEventHandler(this.rectangle_mouse_up);
            this.pictureBox1.MouseDown += new MouseEventHandler(this.rectangle_mouse_down);
            this.pictureBox1.MouseMove += new MouseEventHandler(this.rectangle_mouse_move);
            this.pictureBox1.MouseWheel += new MouseEventHandler(this.rectangle_zoom);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.initialize_attributes();
            this.set_image();
            this.generate_rectangle();
            this.graphics1.DrawRectangle(Pens.Black, this.rectangle);
            this.associate_handlers();
            this.refresh_graphics();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.clear_graphics();
        }
    }
}
