using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace random_and_timer_cs
{
    public partial class Form1 : Form
    {
        private readonly Random generator = new Random();
        private readonly Timer timer = new Timer();
        private readonly int interval = 1500;
        private readonly int precision = 1;
        private static readonly int defaultN = 4;
        private static int n_data = defaultN;
        private int[] current_data;
        private float current_mean = 0.0f;

        public Form1()
        {
            InitializeComponent();
        }
        public static int getDefaultN()
        {
            return defaultN;
        }
        private void InitDataAndMean()
        {
            this.current_data = new int[n_data];
            this.current_mean = 0.0f;
        }
        private void GenerateDataAndComputeMean()
        {
            this.InitDataAndMean();
            for (int i = 0; i < n_data; i++)
            {
                var current_data = this.generator.Next(0, 100);
                this.current_data[i] = current_data;
                this.current_mean = (this.current_mean * i + current_data) / (i + 1);
            }
        }

        private void ShowDataAndMean()
        {
            for (int i = 0; i < n_data; i++)
            {
                this.richTextBox1.Text += "(" + (i + 1) + ")" + " " + this.current_data[i].ToString("n" + this.precision) + "\n";
            }
            this.richTextBox1.Text += "mean: " + this.current_mean.ToString("n" + this.precision) + "\n\n";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.timer1.Interval = interval;
            this.timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.GenerateDataAndComputeMean();
            this.ShowDataAndMean();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.timer1.Stop();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (this.numericUpDown1.Value <= 0)
            {
                this.numericUpDown1.Value = defaultN;
            }
            else
            {
                n_data = (int)this.numericUpDown1.Value;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.timer1.Stop();
            this.GenerateDataAndComputeMean();
            this.ShowDataAndMean();
        }

    }
}
