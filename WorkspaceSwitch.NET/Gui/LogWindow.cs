namespace WorkspaceSwitcher.Gui {
    using System;
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
        }

        /// <summary>
        /// Initialize the components of the window.
        /// </summary>
        /// <param name="messages"></param>
        private void InitializeComponent(string messages) {

            this.Width = 800;
            this.Height = 600;
            textbox = new TextBox();
            this.Controls.Add(textbox);

            textbox.ReadOnly = true;
            textbox.Multiline = true;
            textbox.ScrollBars = ScrollBars.Vertical;
            textbox.Dock = DockStyle.Fill;
            textbox.Text = messages + Environment.NewLine;
            textbox.Select(0, 0);

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

        /// <summary>
        /// Dispose of the window.
        /// </summary>
        public new void Dispose() {
            Logger.GetInstance().LogUpdated -= UpdateLog;
            textbox.Dispose();
            base.Dispose();
        }
    }
}
