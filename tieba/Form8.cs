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
    public partial class Form8 : Form
    {
        public delegate void SendContent(string content);
        public event SendContent SendEvent;
        string refre=string.Empty;
        baidu bd=null;
        public Form8(ref baidu ff)
        {
            InitializeComponent();
            bd = ff;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            label1.Text = bd.UpLoadImage(openFileDialog1.FileName, refre);
            if(label1.Text.IndexOf("错误")<0)
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
    }
}
