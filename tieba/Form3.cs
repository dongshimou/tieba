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
        }
        private void initList()
        {
            listBox1.Items.Clear();
            foreach (var one in bd.barinfo)
            {
                var str = one.Key + "    :" + one.Value;
                listBox1.Items.Add(str);
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
        private void button1_Click(object sender, EventArgs e)
        {
            bd.Gettid(textBox1.Text);
            if (bd.replay(bd.barname, textBox2.Text))
            {
                pictureBox1.Image = bd.GetPostCode();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bd.SetPostCode(textBox3.Text.Trim()))
            {
                if (bd.codereplay(textBox3.Text.Trim(), bd.barname, textBox2.Text))
                {

                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = listBox1.Text.Split(':')[0];
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox2.Text = listBox2.Text;
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

        private void loop()
        {
            Random r = new Random();
            var total = listBox2.Items.Count;
            var time = Convert.ToInt32(numericUpDown1.Value)*1000;
            while (true)
            {
                foreach (var one in bd.barinfo)
                {
                    if (stop) return;
                    var value = Convert.ToInt32(one.Value);
                    if (value >= minf && value <= maxf)
                    {
                        bd.Gettid(one.Key);
                        textBox1.Text = one.Key;
                        var index = r.Next(total);
                        textBox2.Text = listBox2.Items[index].ToString();
                        if (bd.replay(bd.barname, textBox2.Text))
                        {
                            Form6 f6 = new Form6(bd.GetPostCode());
                            f6.StartPosition = this.StartPosition;
                            f6.SendEvent += new Form6.SendCode(GetCode);
                            f6.ShowDialog(this);
                        }
                    }
                    Thread.Sleep(time);
                }
                button3.PerformClick();
            }
        }

        private bool GetCode(string s)
        {
            if (bd.SetPostCode(s))
            {
                return bd.codereplay(textBox3.Text, bd.barname, s);
            }
            return false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            stop = true;
        }
    }
}
