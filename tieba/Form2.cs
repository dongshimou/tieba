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
            this.StartPosition = FormStartPosition.CenterParent;
            openFileDialog1.Multiselect = false;
            openFileDialog1.FileOk += new CancelEventHandler(UpLoadImage);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            replay();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog(this);
            
        }
        private void UpLoadImage(object sender, CancelEventArgs e)
        {
            var result = bd.UpLoadImage(openFileDialog1.FileName, "");
            if (result.IndexOf("错误") < 0)
            {
                ContentBox.Text += result;
            }
        }
        private void replay()
        {
            if (bd.replay(bd.BarName, ContentBox.Text, TitleBox.Text).IndexOf ("验证码")>=0)
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
            else
            {

            }
        }
        private void GetCode(string code)
        {
            if (bd.SetPostCode(code, bd.getCodeType()))
            {
                bd.codereplay(code, bd.BarName, ContentBox.Text, TitleBox.Text);
            }
        }
    }
}
