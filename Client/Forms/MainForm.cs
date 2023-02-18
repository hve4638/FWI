using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FWIClient
{
    public partial class MainForm : Form
    {
        private bool normalExit;

        public MainForm()
        {
            InitializeComponent();
            WindowState = FormWindowState.Minimized;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            Opacity = 0;
            Size = new Size(0, 0);

            normalExit = false;
        }

        public void SetTitle(string text)
        {
            notifyIcon.Text = text;
        }

        public void ShowTip(string title, string text, ToolTipIcon? icon = null)
        {
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = text;
            notifyIcon.BalloonTipIcon = icon ?? ToolTipIcon.Info;
        }

        public void TryNormalClose()
        {
            normalExit = true;
            TryClose();
        }

        public void TryClose()
        {
            if (!IsDisposed) Close();
        }

        private void showPromptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.RunPromptOnRemoteConsole();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryNormalClose();
        }

        private void openConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var optionForm = new OptionForm();

            optionForm.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Program.RunPromptOnRemoteConsole();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.FormNormalExit = normalExit;
            Program.FormCloseReason = e.CloseReason;
        }
    }
}
