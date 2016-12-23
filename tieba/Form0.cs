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
    public partial class Form0 : Form
    {
        private List<Form1> user;
        public Form0()
        {
            InitializeComponent();
            user = new List<Form1>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            user.Add(new Form1());
            listBox1.Items.Add("账号_"+user.Count.ToString());
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var c = listBox1.SelectedIndex;
            user[listBox1.SelectedIndex].Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            user[listBox1.SelectedIndex].Hide();
        }
    }
}
