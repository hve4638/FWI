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
        public MainForm()
        {
            InitializeComponent();
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
        }

        public void ShowTip(string title, string text, ToolTipIcon? icon = null)
        {
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = text;
            notifyIcon.BalloonTipIcon = icon ?? ToolTipIcon.Info;
        }

        private void showPromptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.RunPromptOnRemoteConsole();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.AutoReload = false;
            Close();
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
    }
}
