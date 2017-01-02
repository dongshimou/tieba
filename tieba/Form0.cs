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
        private Dictionary<string,Form1> user;
        private Dictionary<string, string> proxy;
        private Dictionary<string,bool> address;
        public Form0()
        {
            InitializeComponent();
            user = new Dictionary<string, Form1>();
            address=new Dictionary<string, bool>();
            proxy=new Dictionary<string, string>();
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
            if (user.ContainsKey(listBox1.Text))
            {
                user[listBox1.Text].Close();
                user[listBox1.Text].Dispose();
                user.Remove(listBox1.Text);
                proxy.Remove(listBox1.Text);
            }
            listBox1.Items.RemoveAt(index);
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
            removeproxy(listBox2.Text);
            listBox2.Items.RemoveAt(index);
            if (index > 0)
                listBox2.SelectedIndex = index - 1;
        }
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(listBox1.Text))
                return;
            var obj = listBox1.Text.Split(',');
            if(!user.ContainsKey(listBox1.Text))
            user.Add(listBox1.Text,new Form1(obj[0],obj[1]));
            if (!proxy.ContainsKey(listBox1.Text))
            {
                proxy.Add(listBox1.Text,listBox2.Text);
                user[listBox1.Text].setProxy(listBox2.Text);
            }
            else if (proxy[listBox1.Text] != listBox2.Text)
            {
                proxy[listBox1.Text] = listBox2.Text;
                user[listBox1.Text].init(obj[0], obj[1], listBox2.Text);
            }
            user[listBox1.Text].Show();
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
            var sr = new StreamReader("user.txt", Encoding.UTF8);
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
            StreamReader sr = new StreamReader("proxy.txt", Encoding.UTF8);
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
            var sw = new StreamWriter("user.txt",false,Encoding.UTF8);
            foreach (var line in listBox1.Items)
            {
                sw.WriteLine(line.ToString());
            }
            sw.Close();
        }
        private void saveproxy()
        {
            var sw = new StreamWriter("proxy.txt", false, Encoding.UTF8);
            foreach (var line in listBox2.Items)
            {
                sw.WriteLine(line.ToString());
            }
            sw.Close();
        }
        private void addproxy(string s)
        {
            if(address.ContainsKey(s))return;
            address.Add(s,true);
            listBox2.Items.Add(s);
            listBox2.SelectedIndex = listBox2.Items.Count - 1;
        }

        private void removeproxy(string s)
        {
            address.Remove(s);
        }
        private void adduser(string username, string password)
        {
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
