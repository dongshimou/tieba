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
    public partial class Form7 : Form
    {
        private int ClickCount = 0;
        private string vcode = string.Empty;
        public delegate void SendCode(string code);
        public event SendCode SendEvent;
        int width = 0;
        int height = 0;
        int yoffset = 0;
        int xoffset = 0;
        Bitmap[] pick = new Bitmap[4];
        PictureBox CanClick;
        public Form7(Image m)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.Padding = new Padding(0) ;
            this.AutoScrollMargin = new Size(0, 0);
            pictureBox1.Location = this.Location;
            pictureBox1.Image = m;
            width = m.Size.Width;
            height = m.Size.Height;
            pictureBox1.Size = m.Size;
            this.Width = width+8*2;
            this.Height = height+20+8*2;
            yoffset = height / 4;
            width = width / 3;
            height = yoffset;
            xoffset = width - m.Size.Width / 4;
            this.Height = this.Height + height;
            for (int i=0;i<4;i++)
            {
                picture[i].Location = new Point(m.Size.Width / 4 * i, m.Height);
                picture[i].Image = null;
                picture[i].Size = new Size(m.Size.Width / 4, height);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            var mouse = (MouseEventArgs)e;
            //label1.Text=mouse.Location.ToString();
            var x = mouse.Location.X / width;
            var y = (mouse.Location.Y -yoffset) / height;
            const string bs = "000";
            var str=bs + x + bs + y;
            vcode += str;

            pick[ClickCount] = new System.Drawing.Bitmap(width-xoffset, height);
            var pickedG = System.Drawing.Graphics.FromImage(pick[ClickCount]);
            Rectangle from = new Rectangle(x *width+xoffset, y*height+yoffset, width-xoffset, height);
            Rectangle to = new Rectangle(0, 0, width-xoffset, height);
            pickedG.DrawImage(pictureBox1.Image, to,from, GraphicsUnit.Pixel);
            picture[ClickCount].Image = pick[ClickCount];
            CanClick = picture[ClickCount];
            ClickCount++;
            if (ClickCount == 4)
            {
                /*for(int i=0;i<4;i++)
                {
                    string s = i + ".png";
                    pick[i].Save(s);
                }*/
                SendEvent?.Invoke(vcode);
                vcode = string.Empty;
                pictureBox1.Image = null;
                this.Close();
            }
        }
        private void deleteClick(object sender,EventArgs e)
        {
            var pictureBox = (sender as PictureBox);
            if (pictureBox != CanClick) return;
            ClickCount--;
            pictureBox.Image = null;
        }
    }
}
