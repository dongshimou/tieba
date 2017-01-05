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
        private RuoKuaiCode rk;
        int minf = 0;
        int maxf = 29;
        Thread t;
        bool stop = true;
        bool useRuoKuai = false;
        public Form3(ref baidu ff, ref RuoKuaiCode cc)
        {
            rk = cc;
            bd = ff;
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            initList();
            readreplay();
        }
        private void initList()
        {
            listBox1.Items.Clear();
            foreach (var one in bd.barReplay)
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
            var address = listBox1.Text.Split(':')[0].Trim();
            SetAddress(address);
            SetTitle(bd.barTitle[address]);
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ContentBox.Text = listBox2.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bd.GetBarInfo(bd.BarName);
            initList();
        }

        private void listBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button1.PerformClick();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (stop)
            {
                stop = false;
                button4.Text = "停止";
                Control.CheckForIllegalCrossThreadCalls = false;
                t = new Thread(new ThreadStart(loop));
                t.IsBackground = true;
                t.Start();
            }
            else
            {
                stop = true;
                button4.Text = "抢楼";
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (rk == null)
            {
                label3.Text = "若快未登录";
                return;
            }
                if (!useRuoKuai)
            {
                useRuoKuai = true;
                button5.Text = "停止若快";
            }
            else
            {
                useRuoKuai = false;
                button5.Text = "使用若快";
            }
        }
        private void loop()
        {
            Random r = new Random();
            var total = listBox2.Items.Count;
            var time = Convert.ToInt32(numericUpDown1.Value) * 1000;
            while (true)
            {
                foreach (var one in bd.barReplay)
                {
                    if (stop) return;
                    var value = Convert.ToInt32(one.Value);
                    if (value >= minf && value <= maxf)
                    {
                        SetAddress(one.Key);
                        SetTitle(bd.barTitle[one.Key]);
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
            label3.Text = string.Empty;
            if (string.IsNullOrEmpty(ContentBox.Text) ||
                string.IsNullOrEmpty(ReplayBox.Text)) return;
            bd.Gettid(ReplayBox.Text);
            var result = bd.replay(bd.BarName, ContentBox.Text);
            if (result.IndexOf ("验证码")>=0)
            {
                var m = bd.GetPostCode();
                if (m == null)
                {
                    label3.Text = "获取图片失败";
                    return;
                }
                if (bd.getCodeType() == 1)
                {
                    if (useRuoKuai)
                        GetCode(rk.UpLoadImage(m));
                    else
                    {
                        Form6 f6 = new Form6(m);
                        f6.SendEvent += new Form6.SendCode(GetCode);
                        f6.ShowDialog(this);
                    }
                }
                else if (bd.getCodeType() == 4)
                {
                    Form7 f7 = new Form7(m);
                    f7.SendEvent += new Form7.SendCode(GetCode);
                    f7.ShowDialog(this);
                }
            }
            else
            {
                label3.Text = result;
            }
        }
        private void GetCode(string s)
        {
            if (s.IndexOf("错误") >= 0)
            {
                label3.Text = s;
                return;
            }
            if (bd.SetPostCode(s, bd.getCodeType()))
            {
                if (bd.codereplay(s, bd.BarName, ContentBox.Text))
                    label3.Text = "回复成功";
                else
                    label3.Text = "回复失败";
            }
            else
                label3.Text = "图片验证失败";
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
            ReplayBox.Text = "tieba.baidu.com/p/" + s;
        }
        private void SetTitle(string s)
        {
            label4.Text = s;
        }
        private void close(object sender, FormClosedEventArgs e)
        {
            savereplay();
        }
    }
}
