namespace tieba
{
    partial class 点击验证码
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            picture = new System.Windows.Forms.PictureBox[4];
            for (int i = 0; i < 4; i++)
                picture[i] = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            for (int i = 0; i < 4; i++)
                ((System.ComponentModel.ISupportInitialize)(this.picture[i])).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(145, 180);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            for (int i = 0; i < 4; i++)
            {
                this.picture[i].Location = new System.Drawing.Point(12, 12);
                this.picture[i].Name = "picture" + i;
                this.picture[i].Size = new System.Drawing.Size(60, 80);
                this.picture[i].TabIndex = i + 1;
                this.picture[i].TabStop = false;
                this.picture[i].Click += new System.EventHandler(this.deleteClick);
            }
            // 
            // 点击验证码
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(251, 358);
            this.Controls.Add(this.pictureBox1);
            for (int i = 0; i < 4; i++)
                this.Controls.Add(this.picture[i]);
            this.Name = "Form7";
            this.Text = "点击验证码";
            for (int i = 0; i < 4; i++)
                ((System.ComponentModel.ISupportInitialize)(this.picture[i])).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox[] picture;
    }
}