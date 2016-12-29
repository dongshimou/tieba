using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tieba
{
    public partial class Form6 : Form
    {
        public delegate void SendCode(string code);
        public event SendCode SendEvent;
        public Form6(Image m)
        {
            InitializeComponent();
            pictureBox1.Image = m;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendEvent?.Invoke(textBox1.Text.Trim());
            pictureBox1.Image = null;
            textBox1.Text = string.Empty;
            this.Close();
        }
    }
}
