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
    public partial class Form4 : Form
    {
        public delegate void AddProxy(string s);
        public event AddProxy ProxyEvent;
        public Form4()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //https://www.us-proxy.org/
            ProxyEvent?.Invoke(textBox1.Text.Trim());
            this.Close();
        }
    }
}
