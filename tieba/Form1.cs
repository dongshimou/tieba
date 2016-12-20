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
            bd = new baidu();
            label1.Text = bd.init();
            textBox1.Text = "ksbe74906888@163.com";
            textBox2.Text = "zxj654321";
            button1.Text = "获取验证码";
            button2.Text = "校验验证码";
            button3.Text = "一键签到";
            button4.Text = "IE打开";
            button5.Text = "登录";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bd.isgetcodeString(textBox1.Text))
                pictureBox1.Image = bd.getCNCode();
            else
                label1.Text = "不需要验证码";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bd.setCNCode(textBox3.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text = bd.signall();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            HttpHelper.StartIe("http://tieba.baidu.com/", bd.cookie);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (bd.login(textBox1.Text, textBox2.Text))
                label1.Text = "登录成功";
            else
                label1.Text = "登录失败";
        }
    }
}
