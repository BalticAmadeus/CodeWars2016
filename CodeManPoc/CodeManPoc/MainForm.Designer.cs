namespace CodeManPoc
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.boardPic = new System.Windows.Forms.PictureBox();
            this.gameTimer = new System.Windows.Forms.Timer(this.components);
            this.upBtn = new System.Windows.Forms.Button();
            this.downBtn = new System.Windows.Forms.Button();
            this.leftBtn = new System.Windows.Forms.Button();
            this.rightBtn = new System.Windows.Forms.Button();
            this.pacmanAutoChk = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ghostShortRadio = new System.Windows.Forms.RadioButton();
            this.ghostProxyRadio = new System.Windows.Forms.RadioButton();
            this.ghostRandomRadio = new System.Windows.Forms.RadioButton();
            this.playBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.boardPic)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // boardPic
            // 
            this.boardPic.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.boardPic.Location = new System.Drawing.Point(12, 12);
            this.boardPic.Name = "boardPic";
            this.boardPic.Size = new System.Drawing.Size(600, 400);
            this.boardPic.TabIndex = 0;
            this.boardPic.TabStop = false;
            // 
            // gameTimer
            // 
            this.gameTimer.Interval = 300;
            this.gameTimer.Tick += new System.EventHandler(this.gameTimer_Tick);
            // 
            // upBtn
            // 
            this.upBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.upBtn.Location = new System.Drawing.Point(665, 419);
            this.upBtn.Name = "upBtn";
            this.upBtn.Size = new System.Drawing.Size(41, 34);
            this.upBtn.TabIndex = 1;
            this.upBtn.Text = "I";
            this.upBtn.UseVisualStyleBackColor = true;
            this.upBtn.Click += new System.EventHandler(this.upBtn_Click);
            // 
            // downBtn
            // 
            this.downBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.downBtn.Location = new System.Drawing.Point(665, 459);
            this.downBtn.Name = "downBtn";
            this.downBtn.Size = new System.Drawing.Size(41, 34);
            this.downBtn.TabIndex = 2;
            this.downBtn.Text = "K";
            this.downBtn.UseVisualStyleBackColor = true;
            this.downBtn.Click += new System.EventHandler(this.downBtn_Click);
            // 
            // leftBtn
            // 
            this.leftBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.leftBtn.Location = new System.Drawing.Point(618, 459);
            this.leftBtn.Name = "leftBtn";
            this.leftBtn.Size = new System.Drawing.Size(41, 34);
            this.leftBtn.TabIndex = 3;
            this.leftBtn.Text = "J";
            this.leftBtn.UseVisualStyleBackColor = true;
            this.leftBtn.Click += new System.EventHandler(this.leftBtn_Click);
            // 
            // rightBtn
            // 
            this.rightBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.rightBtn.Location = new System.Drawing.Point(712, 459);
            this.rightBtn.Name = "rightBtn";
            this.rightBtn.Size = new System.Drawing.Size(41, 34);
            this.rightBtn.TabIndex = 4;
            this.rightBtn.Text = "L";
            this.rightBtn.UseVisualStyleBackColor = true;
            this.rightBtn.Click += new System.EventHandler(this.rightBtn_Click);
            // 
            // pacmanAutoChk
            // 
            this.pacmanAutoChk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pacmanAutoChk.AutoSize = true;
            this.pacmanAutoChk.Location = new System.Drawing.Point(636, 238);
            this.pacmanAutoChk.Name = "pacmanAutoChk";
            this.pacmanAutoChk.Size = new System.Drawing.Size(90, 17);
            this.pacmanAutoChk.TabIndex = 6;
            this.pacmanAutoChk.Text = "Auto Pacman";
            this.pacmanAutoChk.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.ghostShortRadio);
            this.groupBox1.Controls.Add(this.ghostProxyRadio);
            this.groupBox1.Controls.Add(this.ghostRandomRadio);
            this.groupBox1.Location = new System.Drawing.Point(618, 279);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(135, 101);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ghosts";
            // 
            // ghostShortRadio
            // 
            this.ghostShortRadio.AutoSize = true;
            this.ghostShortRadio.Location = new System.Drawing.Point(18, 65);
            this.ghostShortRadio.Name = "ghostShortRadio";
            this.ghostShortRadio.Size = new System.Drawing.Size(88, 17);
            this.ghostShortRadio.TabIndex = 2;
            this.ghostShortRadio.TabStop = true;
            this.ghostShortRadio.Text = "Shortest path";
            this.ghostShortRadio.UseVisualStyleBackColor = true;
            // 
            // ghostProxyRadio
            // 
            this.ghostProxyRadio.AutoSize = true;
            this.ghostProxyRadio.Location = new System.Drawing.Point(18, 42);
            this.ghostProxyRadio.Name = "ghostProxyRadio";
            this.ghostProxyRadio.Size = new System.Drawing.Size(66, 17);
            this.ghostProxyRadio.TabIndex = 1;
            this.ghostProxyRadio.Text = "Proximity";
            this.ghostProxyRadio.UseVisualStyleBackColor = true;
            // 
            // ghostRandomRadio
            // 
            this.ghostRandomRadio.AutoSize = true;
            this.ghostRandomRadio.Checked = true;
            this.ghostRandomRadio.Location = new System.Drawing.Point(18, 19);
            this.ghostRandomRadio.Name = "ghostRandomRadio";
            this.ghostRandomRadio.Size = new System.Drawing.Size(65, 17);
            this.ghostRandomRadio.TabIndex = 0;
            this.ghostRandomRadio.TabStop = true;
            this.ghostRandomRadio.Text = "Random";
            this.ghostRandomRadio.UseVisualStyleBackColor = true;
            // 
            // playBtn
            // 
            this.playBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.playBtn.Location = new System.Drawing.Point(264, 432);
            this.playBtn.Name = "playBtn";
            this.playBtn.Size = new System.Drawing.Size(112, 46);
            this.playBtn.TabIndex = 8;
            this.playBtn.Text = "START";
            this.playBtn.UseVisualStyleBackColor = true;
            this.playBtn.Click += new System.EventHandler(this.playBtn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 505);
            this.Controls.Add(this.playBtn);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pacmanAutoChk);
            this.Controls.Add(this.rightBtn);
            this.Controls.Add(this.leftBtn);
            this.Controls.Add(this.downBtn);
            this.Controls.Add(this.upBtn);
            this.Controls.Add(this.boardPic);
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "CodeMap";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.boardPic)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox boardPic;
        private System.Windows.Forms.Timer gameTimer;
        private System.Windows.Forms.Button upBtn;
        private System.Windows.Forms.Button downBtn;
        private System.Windows.Forms.Button leftBtn;
        private System.Windows.Forms.Button rightBtn;
        private System.Windows.Forms.CheckBox pacmanAutoChk;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton ghostProxyRadio;
        private System.Windows.Forms.RadioButton ghostRandomRadio;
        private System.Windows.Forms.RadioButton ghostShortRadio;
        private System.Windows.Forms.Button playBtn;
    }
}

