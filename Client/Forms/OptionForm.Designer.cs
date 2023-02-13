namespace FWIClient
{
    partial class OptionForm
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
            this.textIP = new System.Windows.Forms.TextBox();
            this.textPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkDebug = new System.Windows.Forms.CheckBox();
            this.checkAutoReload = new System.Windows.Forms.CheckBox();
            this.checkObserverMode = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textAFKTime = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonAccept = new System.Windows.Forms.Button();
            this.groupServer = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxOpenConsole = new System.Windows.Forms.CheckBox();
            this.groupServer.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textIP
            // 
            this.textIP.Location = new System.Drawing.Point(51, 25);
            this.textIP.Name = "textIP";
            this.textIP.Size = new System.Drawing.Size(121, 23);
            this.textIP.TabIndex = 0;
            this.textIP.TextChanged += new System.EventHandler(this.textIP_TextChanged);
            // 
            // textPort
            // 
            this.textPort.Location = new System.Drawing.Point(51, 54);
            this.textPort.Name = "textPort";
            this.textPort.Size = new System.Drawing.Size(121, 23);
            this.textPort.TabIndex = 1;
            this.textPort.TextChanged += new System.EventHandler(this.textPort_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "port";
            // 
            // checkDebug
            // 
            this.checkDebug.AutoSize = true;
            this.checkDebug.Location = new System.Drawing.Point(18, 139);
            this.checkDebug.Name = "checkDebug";
            this.checkDebug.Size = new System.Drawing.Size(62, 19);
            this.checkDebug.TabIndex = 4;
            this.checkDebug.Text = "Debug";
            this.checkDebug.UseVisualStyleBackColor = true;
            this.checkDebug.CheckedChanged += new System.EventHandler(this.checkDebug_CheckedChanged);
            // 
            // checkAutoReload
            // 
            this.checkAutoReload.AutoSize = true;
            this.checkAutoReload.Location = new System.Drawing.Point(18, 89);
            this.checkAutoReload.Name = "checkAutoReload";
            this.checkAutoReload.Size = new System.Drawing.Size(89, 19);
            this.checkAutoReload.TabIndex = 5;
            this.checkAutoReload.Text = "Auto reload";
            this.checkAutoReload.UseVisualStyleBackColor = true;
            this.checkAutoReload.CheckedChanged += new System.EventHandler(this.checkAutoReload_CheckedChanged);
            // 
            // checkObserverMode
            // 
            this.checkObserverMode.AutoSize = true;
            this.checkObserverMode.Location = new System.Drawing.Point(18, 64);
            this.checkObserverMode.Name = "checkObserverMode";
            this.checkObserverMode.Size = new System.Drawing.Size(108, 19);
            this.checkObserverMode.TabIndex = 6;
            this.checkObserverMode.Text = "Observer mode";
            this.checkObserverMode.UseVisualStyleBackColor = true;
            this.checkObserverMode.CheckedChanged += new System.EventHandler(this.checkObserverMode_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "AFK Time";
            // 
            // textAFKTime
            // 
            this.textAFKTime.Location = new System.Drawing.Point(80, 32);
            this.textAFKTime.Name = "textAFKTime";
            this.textAFKTime.Size = new System.Drawing.Size(71, 23);
            this.textAFKTime.TabIndex = 7;
            this.textAFKTime.TextChanged += new System.EventHandler(this.textAFKTime_TextChanged);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(59, 329);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "취소";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonAccept
            // 
            this.buttonAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAccept.Location = new System.Drawing.Point(138, 329);
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.Size = new System.Drawing.Size(75, 23);
            this.buttonAccept.TabIndex = 10;
            this.buttonAccept.Text = "확인";
            this.buttonAccept.UseVisualStyleBackColor = true;
            this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
            // 
            // groupServer
            // 
            this.groupServer.Controls.Add(this.textIP);
            this.groupServer.Controls.Add(this.textPort);
            this.groupServer.Controls.Add(this.label1);
            this.groupServer.Controls.Add(this.label2);
            this.groupServer.Location = new System.Drawing.Point(17, 12);
            this.groupServer.Name = "groupServer";
            this.groupServer.Size = new System.Drawing.Size(196, 92);
            this.groupServer.TabIndex = 12;
            this.groupServer.TabStop = false;
            this.groupServer.Text = "서버 설정";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxOpenConsole);
            this.groupBox1.Controls.Add(this.textAFKTime);
            this.groupBox1.Controls.Add(this.checkDebug);
            this.groupBox1.Controls.Add(this.checkAutoReload);
            this.groupBox1.Controls.Add(this.checkObserverMode);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(17, 132);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(196, 176);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "클라이언트 설정";
            // 
            // checkBoxOpenConsole
            // 
            this.checkBoxOpenConsole.AutoSize = true;
            this.checkBoxOpenConsole.Location = new System.Drawing.Point(18, 114);
            this.checkBoxOpenConsole.Name = "checkBoxOpenConsole";
            this.checkBoxOpenConsole.Size = new System.Drawing.Size(174, 19);
            this.checkBoxOpenConsole.TabIndex = 9;
            this.checkBoxOpenConsole.Text = "Open console when startup";
            this.checkBoxOpenConsole.UseVisualStyleBackColor = true;
            this.checkBoxOpenConsole.CheckedChanged += new System.EventHandler(this.checkBoxOpenConsole_CheckedChanged);
            // 
            // OptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(230, 370);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupServer);
            this.Controls.Add(this.buttonAccept);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "OptionForm";
            this.Text = "Config";
            this.Load += new System.EventHandler(this.OptionForm_Load);
            this.groupServer.ResumeLayout(false);
            this.groupServer.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TextBox textIP;
        private TextBox textPort;
        private Label label1;
        private Label label2;
        private CheckBox checkDebug;
        private CheckBox checkAutoReload;
        private CheckBox checkObserverMode;
        private Label label3;
        private TextBox textAFKTime;
        private Button buttonCancel;
        private Button buttonAccept;
        private GroupBox groupServer;
        private GroupBox groupBox1;
        private CheckBox checkBoxOpenConsole;
    }
}