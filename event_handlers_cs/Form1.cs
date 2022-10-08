namespace event_handlers_cs
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.label1.Text = "LAST MSG SENT: " + this.textBox1.Text;
        }

        private void textBox1_MouseEnter(object sender, EventArgs e)
        {
            this.textBox1.BackColor = System.Drawing.Color.Green;
        }

        private void textBox1_MouseLeave(object sender, EventArgs e)
        {
            this.textBox1.BackColor = System.Drawing.Color.Red;
        }

        private void label1_MouseHover(object sender, EventArgs e)
        {
            this.label1.Text = "LAST MSG SENT: ";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}