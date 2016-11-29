using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game.PerfMon
{
    public partial class MainForm : Form
    {
        private ObserverWorker _worker;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            logList.Items.Clear();
            _worker = new ObserverWorker();
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            _worker.Start();
            pingTimer.Enabled = true;
        }

        private void pingTimer_Tick(object sender, EventArgs e)
        {
            List<LogEntry> log = _worker.PullLog();
            if (log == null)
                return;

            logList.BeginUpdate();
            ListViewItem lastItem = null;
            try
            {
                foreach (LogEntry entry in log)
                {
                    var li = new ListViewItem();
                    li.Text = entry.Timestamp.ToString("o").Substring(0, 23);
                    li.SubItems.Add(entry.Difference.TotalMilliseconds.ToString());
                    li.SubItems.Add(entry.Message);
                    logList.Items.Add(li);
                    lastItem = li;
                }
            }
            finally
            {
                logList.EndUpdate();
                if (lastItem != null)
                    lastItem.EnsureVisible();
            }
        }
    }
}
