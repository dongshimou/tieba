﻿using System;
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
    public partial class Form8 : Form
    {
        public delegate void SendContent(string content);
        public event SendContent SendEvent;
        string refre = string.Empty;
        static string[] key = {"呵呵", "蛤蛤", " 吐舌","啊","酷", "怒","开心","汗", "泪","黑线","鄙视",
            "不高兴","真棒","钱","疑问","阴险","吐","噫","委屈","花心","呼","笑眼","冷漠","大开心",
            "滑稽","勉强","狂汗","乖","睡觉","惊哭","怒气","惊讶","喷" };
        Dictionary<string, string> emoji = new Dictionary<string, string>();
        baidu bd = null;
        public Form8(ref baidu ff)
        {
            InitializeComponent();
            bd = ff;
            init();
        }
        void init()
        {
            for (int i = 0; i < key.Length; i++)
            {
                /*Image im = null;
                try
                {
                    im = Image.FromFile("emoji\\"+i + ".png");
                }
                catch
                {
                    im = bd.getEmoji(i);
                    im.Save("emoji\\"+i + ".png");
                }*/
                comboBox1.Items.Add(key[i]);
                emoji.Add(key[i], "[emotion pic_type=1 width=30 height=30]http://tb2.bdstatic.com/tb/editor/images/face/i_f" + i.ToString("D2") + ".png[/emotion]");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            label1.Text = bd.UpLoadImage(openFileDialog1.FileName, refre);
            if (label1.Text.IndexOf("错误") < 0)
            {
                ContentBox.Text += label1.Text;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendEvent?.Invoke(ContentBox.Text);
            ContentBox.Text = string.Empty;
            label1.Text = string.Empty;
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ContentBox.Text += emoji[comboBox1.Text];
        }
    }
}
