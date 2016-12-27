﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using tieba;

namespace tieba
{
    public partial class Form1 : Form
    {
        public baidu bd;
        private Form2 f2;
        private Form3 f3;
        public Form1() :
            this(string.Empty)
        {
        }
        public Form1(string proxy):
            this(string.Empty, string.Empty,proxy)
        {
        }

        public Form1(string username, string password) :
            this(username, password, string.Empty)
        {
        }
        public Form1(string username,string password,string proxy)
        {
            InitializeComponent();
            button2.Text = "1校验验证码";
            button3.Text = "3一键签到";
            button4.Text = "4IE打开";
            button5.Text = "2登录";
            button7.Text = "5发帖";
            button8.Text = "6回帖";
            //this.ControlBox = false;
            init(username,password,proxy);
        }
        public void init(string username, string password, string proxy)
        {
            bd = new baidu();
            bd.SignEvent += new baidu.SignDelegate(getlabel);
            if (!string.IsNullOrEmpty(proxy))
                bd.setProxy(proxy);
            if (!string.IsNullOrEmpty(username))
                textBox1.Text = username;//"ksbe74906888@163.com"
            if (!string.IsNullOrEmpty(password))
                textBox2.Text = password;//"zxj654321"
            label1.Text = bd.Init();
            pictureBox1.Image = null;
            getcode();
        }
        void getlabel(string s)
        {
            label1.Text = "正在签到 " + s;
        }

        void getcode()
        {
            var username = textBox1.Text.Trim();
            label1.Text = username;
            if (string.IsNullOrEmpty(username))
            {
                label1.Text = "用户名为空";
                return;
            }
            var res = bd.IsgetcodeString(username);
            if (res == string.Empty)
            {
                pictureBox1.Image = bd.GetLoginCode();
            }
            else
            {
                label1.Text = res;
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bd.SetLoginCode(textBox3.Text.Trim()))
                label1.Text = bd.error;
        }
        private void thread_signall()
        {
            label1.Text=bd.Signall();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text = "后台正在签到，请勿其他操作";

            //多线程
            Control.CheckForIllegalCrossThreadCalls = false;
            Thread t = new Thread(new ThreadStart(thread_signall));
            t.IsBackground=true;
            t.Start();

            //单线程
            //label1.Text = bd.Signall();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start("http://tieba.baidu.com/");
            HttpHelper.StartIe("http://tieba.baidu.com/", bd.cookie);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (bd.Login(textBox1.Text.Trim(), textBox2.Text.Trim()))
                label1.Text = "登录成功";
            else
                label1.Text = "登录失败";
            bd.SignReady();
            foreach(var one in bd.like)
            {
                listBox1.Items.Add(one);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            init(string.Empty, string.Empty, string.Empty);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            bd.barname = textBox4.Text.Trim();
            if (bd.GetBarInfo(bd.barname))
            {
                f2 = new Form2(ref bd);
                f2.Show();
            }
            else
            {
                label1.Text = "获取贴吧信息失败，稍后重试";
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            bd.barname = textBox4.Text.Trim();
            if (bd.GetBarInfo(bd.barname))
            {
                f3 = new Form3(ref bd);
                f3.Show();
            }
            else
            {
                label1.Text = "获取贴吧信息失败，稍后重试";
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            //this.ShowInTaskbar = false;
            //this.WindowState = FormWindowState.Minimized;
            e.Cancel = true;
            this.Hide();
        }
        public void setProxy(string s)
        {
            label7.Text = s;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox4.Text = listBox1.Text;
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            var username = textBox1.Text.Trim();
            label1.Text = username;
            if (string.IsNullOrEmpty(username))
            {
                label1.Text = "用户名为空";
                return;
            }
            var res = bd.IsgetcodeString(username);
            if (res == string.Empty)
                pictureBox1.Image = bd.GetLoginCode();
            else
                label1.Text = res;
        }
    }
}
