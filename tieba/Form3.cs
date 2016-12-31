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
            SetAddress(listBox1.Text.Split(':')[0]);
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
                        SetAddress(one.Key);
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
            if (string.IsNullOrEmpty(ContentBox.Text)||
                string.IsNullOrEmpty (ReplayBox.Text)) return;
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
                    Form7 f7 = new Form7(bd.GetPostCode());
                    f7.StartPosition = this.StartPosition;
                    f7.SendEvent += new Form7.SendCode(GetCode);
                    f7.ShowDialog(this);
                }
            }
        }
        private void GetCode(string s)
        {
            if (bd.SetPostCode(s, bd.getCodeType()))
            {
                bd.codereplay(s, bd.barname, ContentBox.Text);
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

        private void button6_Click(object sender, EventArgs e)
        {
            Form8 f8 = new Form8(ref bd);
            f8.SendEvent += new Form8.SendContent(addContent);
            f8.ShowDialog(this);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            listBox2.Items.RemoveAt(listBox2.SelectedIndex);
        }
        private void addContent(string content)
        {
            listBox2.Items.Add(content);
        }
        private void SetAddress(string s)
        {
            ReplayBox.Text = "tieba.baidu.com/p/" + s.Trim();
        }
        private void close(object sender, FormClosedEventArgs e)
        {
            savereplay();
        }
    }
}
