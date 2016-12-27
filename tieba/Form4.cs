using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tieba;

namespace tieba
{
    using HtmlAgilityPack;

    public partial class Form4 : Form
    {
        public delegate void AddProxy(string s);

        public event AddProxy ProxyEvent;

        public Form4()
        {
            InitializeComponent();
            InitList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //https://www.us-proxy.org/
            ProxyEvent?.Invoke(textBox1.Text.Trim());
            removeproxy();
        }

        private void InitList()
        {
            string url = "https://www.us-proxy.org/";
            var httpResult = new HttpHelper().GetHtml(
                new HttpItem()
                {
                    URL = url,
                    Method = "GET",
                });
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(httpResult.Html);
            if (html.GetElementbyId("proxylisttable") == null)
            {
                label2.Text = "抓取代理失败";
                return;
            }
            var node = html.GetElementbyId("proxylisttable").Element("tbody");
            foreach (var td in node.SelectNodes("tr"))
            {
                var one = td.SelectNodes("td");
                var ip = one[0].InnerText;
                var port = one[1].InnerText;
                GetProxy(ip, port);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = listBox1.Text;
            label2.Text = string.Empty;
        }

        private void GetProxy(string ip, string port)
        {
            listBox1.Items.Add(ip + ":" + port);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0) return;
            var address = listBox1.Text.Split(':');
            if (address?.Length != 2) return;
            var ip = address[0];
            var port = address[1];
            bool tcpconnet = false;
            try
            {
                IPAddress myip = IPAddress.Parse(ip);
                IPEndPoint myendport = new IPEndPoint(myip, Convert.ToInt32(port));

                TcpClient tcp = new TcpClient();
                var task = tcp.ConnectAsync(myip, Convert.ToInt32(port));//异步
                task.Wait(2000);
                if (task.IsCompleted)
                    tcpconnet = true;
                else
                    task.Dispose();
                //tcp.Connect(myendport);//同步
                //tcpconnet = true;
                //tcp.Close();
            }
            catch { }
            if (tcpconnet == false)
            {
                removeproxy();
                label2.Text = "已删除不可用代理";
                return;
            }
            label2.Text = "代理可用";
        }

        void removeproxy()
        {
            var index = listBox1.SelectedIndex;
            if(listBox1.SelectedIndex>=0)
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            if (listBox1.Items.Count > index) listBox1.SelectedIndex = index;
            else listBox1.SelectedIndex = index - 1;
        }
    }
}
