using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tieba;
namespace tieba
{
    public partial class Form3 : Form
    {
        private baidu bd;
        public Form3(ref baidu ff)
        {
            bd = ff;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bd.Gettid(textBox1.Text);
            if (bd.replay(bd.barname, textBox2.Text))
            {
                pictureBox1.Image = bd.replaycode();
            }
            else
            {
                this.Hide();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bd.setreplaycode(textBox3.Text.Trim()))
            {
                if (bd.codereplay(textBox3.Text.Trim(),bd.barname,textBox2.Text))
                {
                    
                }
            }
        }
    }
}
