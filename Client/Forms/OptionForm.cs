using FWI.Results;
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
    public partial class OptionForm : Form
    {
        Config config;
        public OptionForm()
        {
            InitializeComponent();
        }

        private void OptionForm_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            config = Program.GetLastConfig();
            
            textIP.Text = config.serverIP;
            textPort.Text = config.serverPort;
            textAFKTime.Text = config.afkTime;

            checkAutoReload.Checked = config.autoReload;
            checkObserverMode.Checked = !config.observerMode;
            checkDebug.Checked = config.debug;
        }

        private void textAFKTime_TextChanged(object sender, EventArgs e)
        {
            config.afkTime = textAFKTime.Text;
        }

        private void textIP_TextChanged(object sender, EventArgs e)
        {
            config.serverIP = textIP.Text;
        }

        private void textPort_TextChanged(object sender, EventArgs e)
        {
            config.serverPort = textPort.Text;
        }

        private void checkAutoReload_CheckedChanged(object sender, EventArgs e)
        {
            config.autoReload = checkAutoReload.Checked;
        }

        private void checkDebug_CheckedChanged(object sender, EventArgs e)
        {
            config.debug = checkDebug.Checked;
        }

        private void checkObserverMode_CheckedChanged(object sender, EventArgs e)
        {
            config.observerMode = checkObserverMode.Checked;
        }

        private void checkBoxOpenConsole_CheckedChanged(object sender, EventArgs e)
        {
            config.openConsoleWhenStartup = checkBoxOpenConsole.Checked;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            var result = Program.ChangeConfig(config);
            if (result.State == ResultState.Normal)
            {
                Close();
            }
            else if (result.State == ResultState.HasProblem)
            {
                var message = "저장을 실패했습니다\n\n";
                foreach(var reason in result)
                {
                    message += $"- {reason}\n";
                }

                MessageBox.Show(message);
            }

            
        }

    }
}
