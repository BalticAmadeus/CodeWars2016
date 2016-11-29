namespace Game.ControlPanel
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.Label label7;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.grpDebugService = new System.Windows.Forms.GroupBox();
            this.proxyPswTxt = new System.Windows.Forms.TextBox();
            this.proxyUserTxt = new System.Windows.Forms.TextBox();
            this.useProxyChk = new System.Windows.Forms.CheckBox();
            this.lastMsgTxt = new System.Windows.Forms.TextBox();
            this.persistentAccessChk = new System.Windows.Forms.CheckBox();
            this.saveSettingsBtn = new System.Windows.Forms.Button();
            this.revokeMyAccessBtn = new System.Windows.Forms.Button();
            this.grantAccessBtn = new System.Windows.Forms.Button();
            this.checkMyAccessBtn = new System.Windows.Forms.Button();
            this.serviceBaseUrlTxt = new System.Windows.Forms.TextBox();
            this.accessTicker = new System.Windows.Forms.Timer(this.components);
            this.grpDemoClients = new System.Windows.Forms.GroupBox();
            this.demoClientPswTxt = new System.Windows.Forms.TextBox();
            this.demoClientTeamTxt = new System.Windows.Forms.TextBox();
            this.browseDemoClientBtn = new System.Windows.Forms.Button();
            this.demoClientPathTxt = new System.Windows.Forms.TextBox();
            this.launchBothClientsBtn = new System.Windows.Forms.Button();
            this.launchTest2Btn = new System.Windows.Forms.Button();
            this.launchTest1Btn = new System.Windows.Forms.Button();
            this.browseDemoClientDlg = new System.Windows.Forms.OpenFileDialog();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            this.grpDebugService.SuspendLayout();
            this.grpDemoClients.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 24);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(71, 17);
            label1.TabIndex = 0;
            label1.Text = "Base URI:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(6, 117);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(100, 17);
            label2.TabIndex = 8;
            label2.Text = "Last message:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(6, 24);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(66, 17);
            label3.TabIndex = 3;
            label3.Text = "Program:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(24, 52);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(48, 17);
            label4.TabIndex = 6;
            label4.Text = "Team:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(267, 52);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(73, 17);
            label5.TabIndex = 8;
            label5.Text = "Password:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(213, 52);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(42, 17);
            label6.TabIndex = 10;
            label6.Text = "User:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(404, 52);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(73, 17);
            label7.TabIndex = 12;
            label7.Text = "Password:";
            // 
            // grpDebugService
            // 
            this.grpDebugService.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDebugService.Controls.Add(this.proxyPswTxt);
            this.grpDebugService.Controls.Add(label7);
            this.grpDebugService.Controls.Add(this.proxyUserTxt);
            this.grpDebugService.Controls.Add(label6);
            this.grpDebugService.Controls.Add(this.useProxyChk);
            this.grpDebugService.Controls.Add(label2);
            this.grpDebugService.Controls.Add(this.lastMsgTxt);
            this.grpDebugService.Controls.Add(this.persistentAccessChk);
            this.grpDebugService.Controls.Add(this.saveSettingsBtn);
            this.grpDebugService.Controls.Add(this.revokeMyAccessBtn);
            this.grpDebugService.Controls.Add(this.grantAccessBtn);
            this.grpDebugService.Controls.Add(this.checkMyAccessBtn);
            this.grpDebugService.Controls.Add(label1);
            this.grpDebugService.Controls.Add(this.serviceBaseUrlTxt);
            this.grpDebugService.Location = new System.Drawing.Point(12, 12);
            this.grpDebugService.Name = "grpDebugService";
            this.grpDebugService.Size = new System.Drawing.Size(723, 147);
            this.grpDebugService.TabIndex = 2;
            this.grpDebugService.TabStop = false;
            this.grpDebugService.Text = "DebugService Control";
            // 
            // proxyPswTxt
            // 
            this.proxyPswTxt.Location = new System.Drawing.Point(483, 49);
            this.proxyPswTxt.Name = "proxyPswTxt";
            this.proxyPswTxt.Size = new System.Drawing.Size(145, 22);
            this.proxyPswTxt.TabIndex = 13;
            this.proxyPswTxt.UseSystemPasswordChar = true;
            this.proxyPswTxt.TextChanged += new System.EventHandler(this.proxyPswTxt_TextChanged);
            // 
            // proxyUserTxt
            // 
            this.proxyUserTxt.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Game.ControlPanel.Properties.Settings.Default, "ProxyUser", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.proxyUserTxt.Location = new System.Drawing.Point(261, 49);
            this.proxyUserTxt.Name = "proxyUserTxt";
            this.proxyUserTxt.Size = new System.Drawing.Size(137, 22);
            this.proxyUserTxt.TabIndex = 11;
            this.proxyUserTxt.Text = global::Game.ControlPanel.Properties.Settings.Default.ProxyUser;
            this.proxyUserTxt.TextChanged += new System.EventHandler(this.proxyUserTxt_TextChanged);
            // 
            // useProxyChk
            // 
            this.useProxyChk.AutoSize = true;
            this.useProxyChk.Checked = global::Game.ControlPanel.Properties.Settings.Default.ProxyNeedsPassword;
            this.useProxyChk.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Game.ControlPanel.Properties.Settings.Default, "ProxyNeedsPassword", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.useProxyChk.Location = new System.Drawing.Point(9, 51);
            this.useProxyChk.Name = "useProxyChk";
            this.useProxyChk.Size = new System.Drawing.Size(191, 21);
            this.useProxyChk.TabIndex = 9;
            this.useProxyChk.Text = "Use Proxy authentication:";
            this.useProxyChk.UseVisualStyleBackColor = true;
            this.useProxyChk.CheckedChanged += new System.EventHandler(this.useProxyChk_CheckedChanged);
            // 
            // lastMsgTxt
            // 
            this.lastMsgTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lastMsgTxt.Location = new System.Drawing.Point(112, 114);
            this.lastMsgTxt.Name = "lastMsgTxt";
            this.lastMsgTxt.ReadOnly = true;
            this.lastMsgTxt.Size = new System.Drawing.Size(605, 22);
            this.lastMsgTxt.TabIndex = 7;
            // 
            // persistentAccessChk
            // 
            this.persistentAccessChk.Appearance = System.Windows.Forms.Appearance.Button;
            this.persistentAccessChk.Location = new System.Drawing.Point(444, 77);
            this.persistentAccessChk.Name = "persistentAccessChk";
            this.persistentAccessChk.Size = new System.Drawing.Size(136, 31);
            this.persistentAccessChk.TabIndex = 6;
            this.persistentAccessChk.Text = "Keep on";
            this.persistentAccessChk.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.persistentAccessChk.UseVisualStyleBackColor = true;
            this.persistentAccessChk.CheckedChanged += new System.EventHandler(this.persistentAccessChk_CheckedChanged);
            // 
            // saveSettingsBtn
            // 
            this.saveSettingsBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveSettingsBtn.Location = new System.Drawing.Point(634, 17);
            this.saveSettingsBtn.Name = "saveSettingsBtn";
            this.saveSettingsBtn.Size = new System.Drawing.Size(83, 31);
            this.saveSettingsBtn.TabIndex = 5;
            this.saveSettingsBtn.Text = "Save";
            this.saveSettingsBtn.UseVisualStyleBackColor = true;
            this.saveSettingsBtn.Click += new System.EventHandler(this.saveSettingsBtn_Click);
            // 
            // revokeMyAccessBtn
            // 
            this.revokeMyAccessBtn.Location = new System.Drawing.Point(294, 77);
            this.revokeMyAccessBtn.Name = "revokeMyAccessBtn";
            this.revokeMyAccessBtn.Size = new System.Drawing.Size(144, 31);
            this.revokeMyAccessBtn.TabIndex = 4;
            this.revokeMyAccessBtn.Text = "Revoke my access";
            this.revokeMyAccessBtn.UseVisualStyleBackColor = true;
            this.revokeMyAccessBtn.Click += new System.EventHandler(this.revokeMyAccessBtn_Click);
            // 
            // grantAccessBtn
            // 
            this.grantAccessBtn.Location = new System.Drawing.Point(153, 77);
            this.grantAccessBtn.Name = "grantAccessBtn";
            this.grantAccessBtn.Size = new System.Drawing.Size(135, 31);
            this.grantAccessBtn.TabIndex = 3;
            this.grantAccessBtn.Text = "Grant me access";
            this.grantAccessBtn.UseVisualStyleBackColor = true;
            this.grantAccessBtn.Click += new System.EventHandler(this.grantAccessBtn_Click);
            // 
            // checkMyAccessBtn
            // 
            this.checkMyAccessBtn.Location = new System.Drawing.Point(10, 77);
            this.checkMyAccessBtn.Name = "checkMyAccessBtn";
            this.checkMyAccessBtn.Size = new System.Drawing.Size(137, 31);
            this.checkMyAccessBtn.TabIndex = 2;
            this.checkMyAccessBtn.Text = "Check my access";
            this.checkMyAccessBtn.UseVisualStyleBackColor = true;
            this.checkMyAccessBtn.Click += new System.EventHandler(this.checkMyAccessBtn_Click);
            // 
            // serviceBaseUrlTxt
            // 
            this.serviceBaseUrlTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serviceBaseUrlTxt.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Game.ControlPanel.Properties.Settings.Default, "ServiceBaseUrl", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.serviceBaseUrlTxt.Location = new System.Drawing.Point(83, 21);
            this.serviceBaseUrlTxt.Name = "serviceBaseUrlTxt";
            this.serviceBaseUrlTxt.Size = new System.Drawing.Size(545, 22);
            this.serviceBaseUrlTxt.TabIndex = 0;
            this.serviceBaseUrlTxt.Text = global::Game.ControlPanel.Properties.Settings.Default.ServiceBaseUrl;
            // 
            // accessTicker
            // 
            this.accessTicker.Interval = 30000;
            this.accessTicker.Tick += new System.EventHandler(this.accessTicker_Tick);
            // 
            // grpDemoClients
            // 
            this.grpDemoClients.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDemoClients.Controls.Add(this.demoClientPswTxt);
            this.grpDemoClients.Controls.Add(label5);
            this.grpDemoClients.Controls.Add(this.demoClientTeamTxt);
            this.grpDemoClients.Controls.Add(label4);
            this.grpDemoClients.Controls.Add(this.browseDemoClientBtn);
            this.grpDemoClients.Controls.Add(this.demoClientPathTxt);
            this.grpDemoClients.Controls.Add(label3);
            this.grpDemoClients.Controls.Add(this.launchBothClientsBtn);
            this.grpDemoClients.Controls.Add(this.launchTest2Btn);
            this.grpDemoClients.Controls.Add(this.launchTest1Btn);
            this.grpDemoClients.Location = new System.Drawing.Point(12, 165);
            this.grpDemoClients.Name = "grpDemoClients";
            this.grpDemoClients.Size = new System.Drawing.Size(723, 119);
            this.grpDemoClients.TabIndex = 3;
            this.grpDemoClients.TabStop = false;
            this.grpDemoClients.Text = "Demo Clients";
            // 
            // demoClientPswTxt
            // 
            this.demoClientPswTxt.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Game.ControlPanel.Properties.Settings.Default, "DemoClientPassword", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.demoClientPswTxt.Location = new System.Drawing.Point(346, 49);
            this.demoClientPswTxt.Name = "demoClientPswTxt";
            this.demoClientPswTxt.Size = new System.Drawing.Size(185, 22);
            this.demoClientPswTxt.TabIndex = 9;
            this.demoClientPswTxt.Text = global::Game.ControlPanel.Properties.Settings.Default.DemoClientPassword;
            this.demoClientPswTxt.UseSystemPasswordChar = true;
            // 
            // demoClientTeamTxt
            // 
            this.demoClientTeamTxt.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Game.ControlPanel.Properties.Settings.Default, "DemoClientTeam", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.demoClientTeamTxt.Location = new System.Drawing.Point(78, 49);
            this.demoClientTeamTxt.Name = "demoClientTeamTxt";
            this.demoClientTeamTxt.Size = new System.Drawing.Size(183, 22);
            this.demoClientTeamTxt.TabIndex = 7;
            this.demoClientTeamTxt.Text = global::Game.ControlPanel.Properties.Settings.Default.DemoClientTeam;
            // 
            // browseDemoClientBtn
            // 
            this.browseDemoClientBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseDemoClientBtn.Location = new System.Drawing.Point(634, 17);
            this.browseDemoClientBtn.Name = "browseDemoClientBtn";
            this.browseDemoClientBtn.Size = new System.Drawing.Size(83, 31);
            this.browseDemoClientBtn.TabIndex = 5;
            this.browseDemoClientBtn.Text = "Browse...";
            this.browseDemoClientBtn.UseVisualStyleBackColor = true;
            this.browseDemoClientBtn.Click += new System.EventHandler(this.browseDemoClientBtn_Click);
            // 
            // demoClientPathTxt
            // 
            this.demoClientPathTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.demoClientPathTxt.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Game.ControlPanel.Properties.Settings.Default, "DemoClientPath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.demoClientPathTxt.Location = new System.Drawing.Point(78, 21);
            this.demoClientPathTxt.Name = "demoClientPathTxt";
            this.demoClientPathTxt.Size = new System.Drawing.Size(550, 22);
            this.demoClientPathTxt.TabIndex = 4;
            this.demoClientPathTxt.Text = global::Game.ControlPanel.Properties.Settings.Default.DemoClientPath;
            // 
            // launchBothClientsBtn
            // 
            this.launchBothClientsBtn.Location = new System.Drawing.Point(295, 77);
            this.launchBothClientsBtn.Name = "launchBothClientsBtn";
            this.launchBothClientsBtn.Size = new System.Drawing.Size(142, 31);
            this.launchBothClientsBtn.TabIndex = 2;
            this.launchBothClientsBtn.Text = "Launch 1 && 2";
            this.launchBothClientsBtn.UseVisualStyleBackColor = true;
            this.launchBothClientsBtn.Click += new System.EventHandler(this.launchBothClientsBtn_Click);
            // 
            // launchTest2Btn
            // 
            this.launchTest2Btn.Location = new System.Drawing.Point(152, 77);
            this.launchTest2Btn.Name = "launchTest2Btn";
            this.launchTest2Btn.Size = new System.Drawing.Size(137, 31);
            this.launchTest2Btn.TabIndex = 1;
            this.launchTest2Btn.Text = "Launch Test2";
            this.launchTest2Btn.UseVisualStyleBackColor = true;
            this.launchTest2Btn.Click += new System.EventHandler(this.launchTest2Btn_Click);
            // 
            // launchTest1Btn
            // 
            this.launchTest1Btn.Location = new System.Drawing.Point(9, 77);
            this.launchTest1Btn.Name = "launchTest1Btn";
            this.launchTest1Btn.Size = new System.Drawing.Size(137, 31);
            this.launchTest1Btn.TabIndex = 0;
            this.launchTest1Btn.Text = "Launch Test1";
            this.launchTest1Btn.UseVisualStyleBackColor = true;
            this.launchTest1Btn.Click += new System.EventHandler(this.launchTest1Btn_Click);
            // 
            // browseDemoClientDlg
            // 
            this.browseDemoClientDlg.Filter = "Programs|*exe|All files|*.*";
            this.browseDemoClientDlg.Title = "Select Demo Client executable";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 294);
            this.Controls.Add(this.grpDemoClients);
            this.Controls.Add(this.grpDebugService);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "CodeWars Control Panel";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.grpDebugService.ResumeLayout(false);
            this.grpDebugService.PerformLayout();
            this.grpDemoClients.ResumeLayout(false);
            this.grpDemoClients.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox serviceBaseUrlTxt;
        private System.Windows.Forms.GroupBox grpDebugService;
        private System.Windows.Forms.Button revokeMyAccessBtn;
        private System.Windows.Forms.Button grantAccessBtn;
        private System.Windows.Forms.Button checkMyAccessBtn;
        private System.Windows.Forms.Button saveSettingsBtn;
        private System.Windows.Forms.TextBox lastMsgTxt;
        private System.Windows.Forms.CheckBox persistentAccessChk;
        private System.Windows.Forms.Timer accessTicker;
        private System.Windows.Forms.GroupBox grpDemoClients;
        private System.Windows.Forms.Button launchBothClientsBtn;
        private System.Windows.Forms.Button launchTest2Btn;
        private System.Windows.Forms.Button launchTest1Btn;
        private System.Windows.Forms.TextBox demoClientPswTxt;
        private System.Windows.Forms.TextBox demoClientTeamTxt;
        private System.Windows.Forms.Button browseDemoClientBtn;
        private System.Windows.Forms.TextBox demoClientPathTxt;
        private System.Windows.Forms.OpenFileDialog browseDemoClientDlg;
        private System.Windows.Forms.TextBox proxyPswTxt;
        private System.Windows.Forms.TextBox proxyUserTxt;
        private System.Windows.Forms.CheckBox useProxyChk;
    }
}

