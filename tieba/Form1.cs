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
    public partial class Form1 : Form
    {
        private baidu bd;
        public Form1()
        {
            InitializeComponent();
            bd=new baidu();
            textBox1.Text = "ksbe74906888@163.com";
            textBox2.Text = "zxj654321";
            button1.Text = "1获取验证码";
            button2.Text = "2登录";
            button3.Text = "一键签到";
            button4.Text = "IE打开";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image= bd.GetValCode();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bd.CheckValCode(textBox3.Text);
            bd.login(textBox1.Text, textBox2.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text=bd.signall();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            HttpHelper.StartIe("http://tieba.baidu.com/", bd.cookie);
        }
    }
}
