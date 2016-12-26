using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tieba;
namespace tieba
{

    public partial class Form0 : Form
    {
        private List<Form1> user;
        public Form0()
        {
            InitializeComponent();
            user = new List<Form1>();
            readuser();
            readproxy();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Form5 f5 = new Form5();
            f5.Show();
            f5.UserEvent += new Form5.AddUser(adduser);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            var index = listBox1.SelectedIndex;
            if (index < 0 || index >= listBox1.Items.Count) return;
            listBox1.Items.RemoveAt(index);
            user[index].Close();
            if (index > 0)
                listBox1.SelectedIndex = index - 1;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.Show();
            f4.ProxyEvent += new Form4.AddProxy(addproxy);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            var index = listBox2.SelectedIndex;
            if (index < 0 || index >= listBox2.Items.Count) return;
            listBox2.Items.RemoveAt(index);
            if (index > 0)
                listBox2.SelectedIndex = index - 1;
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var c = listBox1.SelectedIndex;
            //user[listBox1.SelectedIndex].Show();
        }
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            user[listBox1.SelectedIndex].bd.setProxy(listBox2.Text);
            user[listBox1.SelectedIndex].Show();
        }
        private void readuser()
        {
            if (File.Exists("user.txt"))
            {
            }
            else
            {
                var f = File.Create("user.txt");
                f.Close();
            }
            var sr = new StreamReader("user.txt", Encoding.Default);
            var line = sr.ReadLine();
            while (line != null)
            {
                var two = line.Split(',');
                adduser(two.First(), two.Last());
                line = sr.ReadLine();
            }
            sr.Close();
        }
        private void readproxy()
        {
            if (File.Exists("proxy.txt"))
            {
            }
            else
            {
                var f = File.Create("proxy.txt");
                f.Close();
            }
            StreamReader sr = new StreamReader("proxy.txt", Encoding.Default);
            string line = sr.ReadLine();
            while (line != null)
            {
                addproxy(line);
                line = sr.ReadLine();
            }
            sr.Close();
        }
        private void saveuser()
        {
            var sw = new StreamWriter("user.txt");
            foreach (var line in listBox1.Items)
            {
                sw.WriteLine(line.ToString());
            }
            sw.Close();
        }
        private void saveproxy()
        {
            var sw = new StreamWriter("proxy.txt");
            foreach (var line in listBox2.Items)
            {
                sw.WriteLine(line.ToString());
            }
            sw.Close();
        }
        private void addproxy(string s)
        {
            listBox2.Items.Add(s);
            listBox2.SelectedIndex = listBox2.Items.Count - 1;
        }
        private void adduser(string username, string password)
        {
            user.Add(new Form1(username, password));
            listBox1.Items.Add(username + "," + password);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }
        private void close(object sender, FormClosedEventArgs e)
        {
            saveuser();
            saveproxy();
        }
    }
}
