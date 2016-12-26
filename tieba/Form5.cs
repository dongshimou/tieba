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
    public partial class Form5 : Form
    {
        public delegate void AddUser(string un,string pw);
        public event AddUser UserEvent;
        public Form5()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            UserEvent?.Invoke(textBox1.Text.Trim(), textBox2.Text.Trim());
            this.Close();
        }
    }
}
