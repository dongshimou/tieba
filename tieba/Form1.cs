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
    public partial class Form1 : Form
    {
        public delegate void MessageHandler(string msg);
        public baidu bd;
        private Form2 f2;
        private Form3 f3;
        public Form1()
        {
            InitializeComponent();
            bd = new baidu();
            
            
            label1.Text = bd.Init();
            textBox1.Text = "ksbe74906888@163.com";
            textBox2.Text = "zxj654321";
            button1.Text = "1获取验证码";
            button2.Text = "2校验验证码";
            button3.Text = "5一键签到";
            button4.Text = "4IE打开";
            button5.Text = "3登录";
            button6.Text = "6重置";
            button7.Text = "7发帖";
            button8.Text = "8回帖";
            bd.SignMessage += new MessageHandler(Getsignmessage);
        }

        private void Getsignmessage(string msg)
        {
            label1.Text = msg;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = textBox1.Text;
            if (bd.IsgetcodeString(textBox1.Text))
                pictureBox1.Image = bd.GetCnCode();
            else
                label1.Text = "不需要验证码";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bd.SetCnCode(textBox3.Text))
                label1.Text = bd.error;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text = "正在签到，请等待";
            //2s签到一个吧
            label1.Text = bd.Signall(2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start("http://tieba.baidu.com/");
            HttpHelper.StartIe("http://tieba.baidu.com/", bd.cookie);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (bd.Login(textBox1.Text, textBox2.Text))
                label1.Text = "登录成功";
            else
                label1.Text = "登录失败";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            bd = new baidu();
            label1.Text = bd.Init();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            bd.barname = textBox4.Text.Trim();
            bd.GetBarInfo(bd.barname);
            f2 = new Form2(ref bd);
            f2.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            bd.barname=textBox4.Text.Trim();
            bd.GetBarInfo(bd.barname);
            f3 = new Form3(ref bd);
            f3.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            bd.GetBarInfo("python");
        }
    }
}
