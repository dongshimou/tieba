using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using tieba;
namespace tieba
{
    public partial class Form3 : Form
    {
        private baidu bd;
        int minf = 0;
        int maxf = 29;
        Thread t;
        bool stop = false;
        public Form3(ref baidu ff)
        {
            
            bd = ff;
            InitializeComponent();
            initList();
            readreplay();
            openFileDialog1.Multiselect = false;
            textBox1.ReadOnly = true;
        }
        private void initList()
        {
            listBox1.Items.Clear();
            foreach (var one in bd.barinfo)
            {
                var str = one.Key + "    :    " + one.Value;
                listBox1.Items.Add(str);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            replay();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReplayBox.Text = listBox1.Text.Split(':')[0];
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ContentBox.Text = listBox2.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bd.GetBarInfo(bd.barname);
            initList();
        }

        private void listBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button1.PerformClick();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            t = new Thread(new ThreadStart(loop));
            t.IsBackground = true;
            t.Start();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            stop = true;
        }
        private void loop()
        {
            Random r = new Random();
            var total = listBox2.Items.Count;
            var time = Convert.ToInt32(numericUpDown1.Value) * 1000;
            while (true)
            {
                foreach (var one in bd.barinfo)
                {
                    if (stop) return;
                    var value = Convert.ToInt32(one.Value);
                    if (value >= minf && value <= maxf)
                    {
                        ReplayBox.Text = one.Key;
                        var index = r.Next(total);
                        ContentBox.Text = listBox2.Items[index].ToString();
                        replay();
                    }
                    Thread.Sleep(time);
                }
                button3.PerformClick();
            }
        }
        
        private void replay()
        {
            bd.Gettid(ReplayBox.Text);
            if (bd.replay(bd.barname, ContentBox.Text))
            {
                if (bd.getCodeType() == 1)
                {
                    Form6 f6 = new Form6(bd.GetPostCode());
                    f6.StartPosition = this.StartPosition;
                    f6.SendEvent += new Form6.SendCode(GetCode);
                    f6.ShowDialog(this);
                }
                else if (bd.getCodeType() == 4)
                {
                    点击验证码 f7 = new 点击验证码(bd.GetPostCode());
                    f7.StartPosition = this.StartPosition;
                    f7.SendEvent += new 点击验证码.SendCode(GetCode);
                    f7.ShowDialog(this);
                }
            }
        }
        private void GetCode(string s)
        {
            if (bd.SetPostCode(s,bd.getCodeType ()))
            {
                CodeBox.Text = s;
                bd.codereplay(s,bd.barname, ContentBox.Text);
            }
        }
        private void readreplay()
        {
            if (File.Exists("replay.txt"))
            {
            }
            else
            {
                var f = File.Create("replay.txt");
                f.Close();
            }
            var sr = new StreamReader("replay.txt", Encoding.UTF8);
            var line = sr.ReadLine();
            while (line != null)
            {
                listBox2.Items.Add(line);
                line = sr.ReadLine();
            }
            sr.Close();
        }
        private void savereplay()
        {
            var sw = new StreamWriter("replay.txt", false, Encoding.UTF8);
            foreach (var line in listBox2.Items)
            {
                sw.WriteLine(line.ToString());
            }
            sw.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            textBox1.Text = openFileDialog1.FileName;
            textBox2.Text=bd.UpLoadImage(textBox1.Text,ReplayBox.Text.Trim());
            if(textBox2.Text!="error")
            {
                ContentBox.Text += textBox2.Text;
            }
        }
    }
}
