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
    public partial class Form2 : Form
    {
        private baidu bd;
        public Form2(ref baidu ff)
        {
            bd = ff;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bd.post(bd.barname, textBox4.Text, textBox2.Text))
            {
                pictureBox1.Image=bd.replaycode();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bd.setreplaycode(textBox3.Text.Trim()))
            {
                if (bd.codepost(textBox3.Text.Trim(), bd.barname, textBox4.Text, textBox2.Text))
                {
                    
                }
            }
        }
    }
}
