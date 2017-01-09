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
            this.StartPosition = FormStartPosition.CenterParent;
            InitList();
            //KuaiDaili("978379447402888");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //https://www.us-proxy.org/
            ProxyEvent?.Invoke(textBox1.Text.Trim());
            removeproxy();
        }
        void KuaiDaili(string order)
        {
            var url = "http://dev.kuaidaili.com/api/getproxy/?orderid="+order
                +"&num=100&b_pcchrome=1&b_pcie=1&b_pcff=1&protocol=1&method=2&an_an=1&an_ha=1&sp1=1&quality=1&sort=1&sep=1";
            var httpResult = new HttpHelper().GetHtml(
                    new HttpItem()
                    {
                        URL = url,
                        Method = "GET",
                    });
            if (string.IsNullOrEmpty(httpResult.Html)) return;
            var html = httpResult.Html.Replace("\n","");
            var list = html.Split('\r');
            foreach(var one in list)
            {
                var address = one.Split(':');
                GetProxy(address[0], address[1]);
            }
        }

        private void InitList(bool cn = true)
        {
            if (!cn)
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
            else
            {
                string url = "http://www.kuaidaili.com/free/";
                var httpResult = new HttpHelper().GetHtml(
                    new HttpItem()
                    {
                        URL = url,
                        Method = "GET",
                    });
                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(httpResult.Html);
                var container = html.GetElementbyId("container");
                if (container == null)
                {
                    label2.Text = "抓取代理失败";
                    return;
                }
                var node = html.DocumentNode;
                var list=node.SelectSingleNode("/html[1]/body[1]/div[2]/div[1]/div[2]/table[1]/tbody[1]");
                var t1 = node.FirstChild;
                var t2 = t1.FirstChild;
                foreach(var tr in list.SelectNodes("tr"))
                {
                    var one = tr.SelectNodes("td");
                    var ip = one[0].InnerText;
                    var port = one[1].InnerText;
                    GetProxy(ip, port);
                }
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

        private void button3_Click(object sender, EventArgs e)
        {
            bool[] vis = new bool[listBox1.Items.Count];
            int count = 0;
            foreach (var one in listBox1.Items)
            {
                var address = one.ToString().Split(':');
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
                    task.Wait(1000);
                    if (task.IsCompleted)
                        tcpconnet = true;
                    else
                        task.Dispose();
                }
                catch { }
                if (tcpconnet == false)
                    vis[count++] = false;
                else
                    vis[count++] = true;
            }
            for(int i= vis.Length-1; i>=0;i--)
            {
                if (!vis[i])
                    listBox1.Items.RemoveAt(i);
            }
            label2.Text = "全部测试完成";
        }
    }
}
