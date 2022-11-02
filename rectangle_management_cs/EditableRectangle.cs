using System;

namespace rectangle_management_cs
{
	public class EditableRectangle
	{
		public Rectangle rectangle;
		PictureBox pictureBox;
		Form form;
        private Boolean move = false, resize = false;
        private int x_mouse, x_down, y_mouse, y_down;
        private double scale_fact = 0.1;

        public EditableRectangle(int x, int y, int width, int heigth, PictureBox picture_box, Form form)
		{
			this.rectangle = new Rectangle(X, Y, Width, Height);
			this.pictureBox = picture_box;
			this.form = form;

            associate_handlers();
		}

        private void refresh_graphics()
        {
            this.pictureBox.Refresh();
        }

        private void clear_graphics()
        {
            this.pictureBox.Image = null;
            this.pictureBox.Refresh();
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

                this.x_down = this.rectangle.X;
                this.y_down = this.rectangle.Y;

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
            if (x <= 0 || x >= this.pictureBox.Width - width)
            {
                return false;
            }

            if (y <= 0 || y >= this.pictureBox.Height - height)
            {
                return false;
            }

            return true;
        }

        private bool width_and_height_are_valid(int width, int height)
        {
            if (width <= 0 || width >= this.pictureBox.Width)
            {
                return false;
            }

            if (height <= 0 || height >= this.pictureBox.Height)
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
                var new_rectangle_x = this.x_down + delta_x;
                var new_rectangle_y = this.y_down + delta_y;

                if (!this.x_and_y_are_valid(new_rectangle_x, new_rectangle_y, this.rectangle.Width, this.rectangle.Height)) return;

                this.rectangle.X = new_rectangle_x;
                this.rectangle.Y = new_rectangle_y;

                this.graphics1.Clear(this.pictureBox.BackColor);
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

                this.graphics.Clear(this.pictureBox.BackColor);
                this.graphics.DrawRectangle(Pens.Black, this.rectangle);
                this.refresh_graphics();
            }
        }

        private void rectangle_zoom(Object sender, MouseEventArgs e)
        {
            if (this.rectangle.Contains(e.X, e.Y))
            {
                this.x_down = this.rectangle.X;
                this.y_down = this.rectangle.Y;

                this.rectangle.Width = this.rectangle.Width + (int)(e.Delta * this.scale_fact);
                this.rectangle.Height = this.rectangle.Height + (int)(e.Delta * this.scale_fact);

                this.rectangle.X = x_down - (int)((e.Delta * this.scale_fact) / 2);
                this.rectangle.Y = y_down - (int)((e.Delta * this.scale_fact) / 2);
            }
        }

        private void associate_handlers()
        {
            this.pictureBox.MouseUp += new MouseEventHandler(this.rectangle_mouse_up);
            this.pictureBox.MouseDown += new MouseEventHandler(this.rectangle_mouse_down);
            this.pictureBox.MouseMove += new MouseEventHandler(this.rectangle_mouse_move);
            this.pictureBox.MouseWheel += new MouseEventHandler(this.rectangle_zoom);
        }
    }
}
}
