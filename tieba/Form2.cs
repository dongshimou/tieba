using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tieba;
namespace tieba
{
    public partial class Form2 : Form
    {
        private baidu bd;
        public Form2(ref baidu ff)
        {
            bd = ff;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            replay();
        }
        private void replay()
        {
            if (bd.replay(bd.barname, ContentBox.Text, TitleBox.Text))
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
                CodeBox.Text = s;
                bd.codereplay(s, bd.barname, ContentBox.Text, TitleBox.Text);
            }
        }
    }
}
