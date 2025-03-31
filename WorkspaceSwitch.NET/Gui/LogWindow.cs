namespace WorkspaceSwitcher.Gui {
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Win32HotkeyListener;

    /// <summary>
    /// Window for showing application log.
    /// </summary>
    internal class LogWindow : Form, IDisposable {

        private TextBox textbox;

        /// <summary>
        /// Constructor for LogWindow, takes a string of messages to display.
        /// </summary>
        /// <param name="messages"></param>
        internal LogWindow(string messages) {
            InitializeComponent(messages);
            Logger.GetInstance().LogUpdated += UpdateLog;
            textbox = new TextBox();
        }

        /// <summary>
        /// Initialize the components of the window.
        /// </summary>
        /// <param name="messages"></param>
        private void InitializeComponent(string messages) {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            this.Icon =  new Icon(assembly.GetManifestResourceStream("AppIcon"));
            this.Width = 800;
            this.Height = 600;
            
            this.Controls.Add(textbox);

            textbox.ReadOnly = true;
            textbox.Multiline = true;
            textbox.ScrollBars = ScrollBars.Vertical;
            textbox.Dock = DockStyle.Fill;
            textbox.Text = messages + Environment.NewLine;
            textbox.Select(0, 0);
            FormClosing += LogWindow_FormClosing;
        }

        /// <summary>
        /// Update the log window with a new message.
        /// </summary>
        /// <param name="message"></param>
        private void UpdateLog(string message) {
            if (textbox == null || textbox.IsDisposed) {
                return;
            }

            textbox.BeginInvoke(new Action(() => textbox.AppendText(message + Environment.NewLine)));
        }

        private void LogWindow_FormClosing(object sender, FormClosingEventArgs e) {
            Logger.GetInstance().LogUpdated -= UpdateLog;
        }

    }
}
