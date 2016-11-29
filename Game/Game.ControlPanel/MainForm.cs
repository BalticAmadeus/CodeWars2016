using Game.ControlPanel.DebugServiceRef;
using Game.ControlPanel.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game.ControlPanel
{
    public partial class MainForm : Form
    {
        private DateTime _lastAccessExtension;
        private SimpleCredentials _proxyCreds;

        public MainForm()
        {
            _proxyCreds = new SimpleCredentials();
            InitializeComponent();
        }

        private void ShowError(Exception ex)
        {
            string message = $"{ex.GetType().FullName}: {ex.Message}";
            AppendInnerException(ref message, ex);
            MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void AppendInnerException(ref string message, Exception ex)
        {
            if (ex.InnerException == null)
                return;

            var inner = ex.InnerException;
            message = $"{message}\n\nInner exception:\n{inner.GetType().FullName}: {inner.Message}";
            AppendInnerException(ref message, inner);
        }

        private DebugServiceClient GetServiceClient()
        {
            return new DebugServiceClient("BasicHttpBinding_IDebugService", serviceBaseUrlTxt.Text + "/DebugService.svc");
        }

        private void checkMyAccessBtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (var client = GetServiceClient())
                {
                    string message = client.CheckMyAccess();
                    lastMsgTxt.Text = message;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private string GenerateChallengeResponse(string challenge)
        {
            string secret = "SekPgijIL0zzWBQpiZJVRyXw/Gz4T3Ef";
            string text = secret + challenge + secret;
            using (var dig = HashAlgorithm.Create("SHA1"))
            {
                byte[] sig = Encoding.UTF8.GetBytes(text);
                return BitConverter.ToString(dig.ComputeHash(sig)).Replace("-", "").ToLower();
            }
        }

        private void grantAccessBtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (var client = GetServiceClient())
                {
                    string challenge = client.GrantMeAccess("");
                    string response = GenerateChallengeResponse(challenge);
                    string message = client.GrantMeAccess(response);
                    lastMsgTxt.Text = message;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void revokeMyAccessBtn_Click(object sender, EventArgs e)
        {
            persistentAccessChk.Checked = false;
            try
            {
                using (var client = GetServiceClient())
                {
                    string challenge = client.RevokeMyAccess("");
                    string response = GenerateChallengeResponse(challenge);
                    string message = client.RevokeMyAccess(response);
                    lastMsgTxt.Text = message;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void saveSettingsBtn_Click(object sender, EventArgs e)
        {
            Settings.Default.Save();
        }

        private void persistentAccessChk_CheckedChanged(object sender, EventArgs e)
        {
            if (persistentAccessChk.Checked)
            {
                persistentAccessChk.Text = "KEEP ON";
                accessTicker.Enabled = true;
                accessTicker_Tick(null, null);
            }
            else
            {
                persistentAccessChk.Text = "Keep on";
                accessTicker.Enabled = false;
            }
        }

        private void accessTicker_Tick(object sender, EventArgs e)
        {
            try
            {
                using (var client = GetServiceClient())
                {
                    string message = client.CheckMyAccess();
                    lastMsgTxt.Text = message;
                    DateTime now = DateTime.Now;
                    if (!message.Contains("GRANTED") || _lastAccessExtension.AddMinutes(15) < now)
                    {
                        string challenge = client.GrantMeAccess("");
                        string response = GenerateChallengeResponse(challenge);
                        message = client.GrantMeAccess(response);
                        lastMsgTxt.Text = message;
                        _lastAccessExtension = now;
                    }
                }
            }
            catch (Exception ex)
            {
                lastMsgTxt.Text = $"{ex.GetType().FullName}: {ex.Message}";
            }
        }

        private void launchTest1Btn_Click(object sender, EventArgs e)
        {
            LaunchDemoClient("Test1");
        }

        private void launchTest2Btn_Click(object sender, EventArgs e)
        {
            LaunchDemoClient("Test2");
        }

        private void launchBothClientsBtn_Click(object sender, EventArgs e)
        {
            LaunchDemoClient("Test1");
            LaunchDemoClient("Test2");
        }

        private void LaunchDemoClient(string name)
        {
            string prm = string.Format("{0} {1} {2} {3}",
                serviceBaseUrlTxt.Text + "/ClientService.svc",
                demoClientTeamTxt.Text,
                name,
                demoClientPswTxt.Text);
            System.Diagnostics.Process.Start(demoClientPathTxt.Text, prm);
        }

        private void browseDemoClientBtn_Click(object sender, EventArgs e)
        {
            if (browseDemoClientDlg.ShowDialog(this) != DialogResult.OK)
                return;
            demoClientPathTxt.Text = browseDemoClientDlg.FileName;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            AssignProxyCreds();
            SetupProxyAuthentication();
        }

        private void useProxyChk_CheckedChanged(object sender, EventArgs e)
        {
            SetupProxyAuthentication();
        }

        private void AssignProxyCreds()
        {
            _proxyCreds.UserName = proxyUserTxt.Text;
            _proxyCreds.Password = proxyPswTxt.Text;
        }

        private void SetupProxyAuthentication()
        {
            if (useProxyChk.Checked)
                WebRequest.DefaultWebProxy.Credentials = _proxyCreds;
            else
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
        }

        private void proxyUserTxt_TextChanged(object sender, EventArgs e)
        {
            AssignProxyCreds();
        }

        private void proxyPswTxt_TextChanged(object sender, EventArgs e)
        {
            AssignProxyCreds();
        }
    }
}
