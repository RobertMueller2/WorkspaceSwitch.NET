using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WorkspaceSwitcher.Gui {

    /// <summary>
    /// Window for showing info about the application.
    /// </summary>
    internal class AboutWindow : Form {
        private Label label;
        private Label label1;
        private LinkLabel linkLabel1;
        private TextBox textBox1;
        private Label label2;
        private LinkLabel linkLabel;

        /// <summary>
        /// 
        /// </summary>
        internal AboutWindow() {
            this.label = new System.Windows.Forms.Label();
            this.linkLabel = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();

            InitializeComponent();
            textBox1.Text = File.ReadAllText("NOTICE.md");
            label.Text = "WorkspaceSwitch.NET v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void InitializeComponent() {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            this.Icon = new Icon(assembly.GetManifestResourceStream("AppIcon"));


            this.SuspendLayout();
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(12, 9);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(200, 16);
            this.label.TabIndex = 0;
            this.label.Text = "WorkspaceSwitch.NET";
            // 
            // linkLabel
            // 
            this.linkLabel.AutoSize = true;
            this.linkLabel.Location = new System.Drawing.Point(32, 41);
            this.linkLabel.Name = "linkLabel";
            this.linkLabel.Size = new System.Drawing.Size(350, 16);
            this.linkLabel.TabIndex = 1;
            this.linkLabel.TabStop = true;
            this.linkLabel.Text = "https://github.com/RobertMueller2/WorkspaceSwitch.NET";
            this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(170, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "© 2024 René D. Obermüller ";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(32, 57);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(99, 16);
            this.linkLabel1.TabIndex = 3;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Show License...";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // textBox1
            // 
            this.textBox1.AcceptsReturn = true;
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 116);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(638, 624);
            this.textBox1.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 77);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.label2.Size = new System.Drawing.Size(66, 36);
            this.label2.TabIndex = 6;
            this.label2.Text = "NOTICES";
            // 
            // AboutWindow
            // 
            this.ClientSize = new System.Drawing.Size(662, 752);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label);
            this.Controls.Add(this.linkLabel);
            this.Name = "AboutWindow";
            this.Text = "About WorkspaceSwitch.NET";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(((LinkLabel)sender).Text);
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start("LICENSE");
        }
    }
}
