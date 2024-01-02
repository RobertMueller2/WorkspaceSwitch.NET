using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VirtualDesktop;
using Win32HotkeyListener;

namespace WorkspaceSwitcher.Gui
{
    /// <summary>
    /// Main window of the application.
    /// </summary>
    public class MainWindow : ApplicationContext
    {
        private readonly Timer timer;
        private HotkeyListener HotkeyListener;
        private Icon AppIcon;
        private Icon[] WorkspaceIcons;
        private int LastDesktop;

        private readonly string tooltipTitle = "Messages";
        private readonly string popupTitle = "Errors";
        private readonly string fatalPrefix = "Fatal error. Exiting.";

        private NotifyIcon trayIcon;

        /// <summary>
        /// Constructor for MainWindow, takes a HotkeyListener.
        /// </summary>
        /// <param name="hotkeyListener"></param>
        public MainWindow(HotkeyListener hotkeyListener)
        {
            LastDesktop = -1;
            HotkeyListener = hotkeyListener;

            this.timer = new System.Windows.Forms.Timer();

            InitializeResources();
            InitialiseForm();
            InitializeTray();
        }

        /// <summary>
        /// Initialises the hidden MainForm.
        /// </summary>
        private void InitialiseForm() {
            var hiddenForm = new Form();
            hiddenForm.Load += HiddenForm_Load;
            hiddenForm.FormClosed += HiddenForm_FormClosed;
            hiddenForm.WindowState = FormWindowState.Minimized;
            hiddenForm.ShowInTaskbar = false;
            hiddenForm.Show();

            // Set this form as the main form of the application
            MainForm = hiddenForm;
        }

        /// <summary>
        /// Initializes the resources used for the application.
        /// </summary>
        private void InitializeResources()
        {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            AppIcon = new Icon(assembly.GetManifestResourceStream("AppIcon"));

            WorkspaceIcons = new Icon[10];

            for (var i = 1; i <= 10; i++)
            {
#if DEBUG
                WorkspaceIcons[i - 1] = new Icon(assembly.GetManifestResourceStream(String.Format("Icon{0}_dev", i)));
#else
                WorkspaceIcons[i - 1] = new Icon(assembly.GetManifestResourceStream(string.Format("Icon{0}", i)));
#endif
            }
        }

        /// <summary>
        /// Initializes the tray icon.
        /// </summary>
        private void InitializeTray()
        {
            var components = new System.ComponentModel.Container();
            trayIcon = new NotifyIcon(components);
            var trayMenu = new ContextMenu();
            var menuItemAbout = new MenuItem();
            var menuItemExit = new MenuItem();
            var menuItemLog = new MenuItem();
            var menuItemRefresh = new MenuItem();

            menuItemAbout.Index = 1;
            menuItemAbout.Text = "About...";
            menuItemAbout.Click += new EventHandler(MenuItemAbout_Click);

            menuItemExit.Index = 2;
            menuItemExit.Text = "Exit";
            menuItemExit.Click += new EventHandler(MenuItemExit_Click);

            menuItemRefresh.Index = 0;
            menuItemRefresh.Text = "Refresh";
            menuItemRefresh.Click += new EventHandler(MenuItemRefresh_Click);

            menuItemLog.Text = "Show log...";
            menuItemLog.Click += new System.EventHandler(this.MenuItemLog_Click);

            trayMenu.MenuItems.AddRange(new MenuItem[] { menuItemRefresh, menuItemAbout, menuItemLog, menuItemExit });

            trayIcon.Text = "WorkspaceSwitch.NET";
            trayIcon.Icon = AppIcon;
            trayIcon.Visible = true;
            trayIcon.ContextMenu = trayMenu;
        }

        /// <summary>
        /// Method for handling the FormClosed event of the hidden MainForm.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HiddenForm_FormClosed(object sender, FormClosedEventArgs e) {
            ExitThread();
        }

        /// <summary>
        /// Method for handling the Load event of the hidden MainForm.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HiddenForm_Load(object sender, EventArgs e) {
            //TODO: Make this configurable. Delay between MainWindow log updates
            timer.Interval = 500;
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        /// <summary>
        /// Method for handling the Click event of the MenuItemAbout.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        public void MenuItemAbout_Click(object Sender, EventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        /// <summary>
        /// Method for handling the Click event of the MenuItemExit.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        public void MenuItemExit_Click(object Sender, EventArgs e)
        {
            Exit();
        }

        /// <summary>
        /// Method for handling the Click event of the MenuItemLog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemLog_Click(object sender, EventArgs e) {
            var text = string.Join(Environment.NewLine, Logger.GetInstance().GetLogs());

            var form = new LogWindow(text);
            form.ShowDialog();
        }

        /// <summary>
        /// Method for handling the Click event of the MenuItemRefresh.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        public void MenuItemRefresh_Click(object Sender, EventArgs e)
        {
            trayIcon.Icon = AppIcon;
            LastDesktop = -1;
        }

        /// <summary>
        /// Exit the application.
        /// </summary>
        public void Exit()
        {
            HotkeyListener.Stop();
            trayIcon.Visible = false;
            trayIcon.Dispose();
            Application.Exit();
        }

        /// <summary>
        /// Log playback.
        /// </summary>
        private void Playback() {
            (List<LogMessage> fatalMessages, List<LogMessage> errorMessages, List<LogMessage> infoMessages) = Logger.GetInstance().Playback();

            //TODO: Use method
            if (fatalMessages.Count > 0) {
                MessageBox.Show(string.Format("{0}\n{1}", fatalPrefix, string.Join("\n", fatalMessages.Select(x => x.Message))), "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            //TODO: Use method
            if (errorMessages.Count > 0) {
                MessageBox.Show(string.Format("{0}", string.Join("\n", errorMessages.Select(x => x.Message))), popupTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (infoMessages.Count > 0) {
                var msg = string.Format("{0}", string.Join("\n", infoMessages.Select(x => x.Message)));
                ShowToolTip(msg, tooltipTitle);
            }
        }

        /// <summary>
        /// Convenience signature for ShowToolTip(LogMessage).
        /// </summary>
        /// <param name="msg"></param>
        internal void ShowToolTip(LogMessage msg) => ShowToolTip(msg.Message, msg.Type.ToString());

        /// <summary>
        /// Show a tooltip from a message and a title.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        internal void ShowToolTip(string message, string title) {
            //TODO: Icon
            trayIcon.BalloonTipTitle = title;
            trayIcon.BalloonTipText = message;
            trayIcon.ShowBalloonTip(5000);
        }

        /// <summary>
        /// Timer tick event handler.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        private void Timer_Tick(object o, EventArgs args)
        {
            timer.Stop();
            var cd = Desktop.FromDesktop(Desktop.Current);

            if (cd != LastDesktop)
            {
                Icon icon;
                if (cd >= 0 && cd < 10)
                {
                    icon = WorkspaceIcons[cd];
                }
                else
                {
                    icon = AppIcon;
                }

                trayIcon.Icon = icon;
                LastDesktop = cd;
            }

            if (trayIcon != null) {
                Playback();
            }

            if (HotkeyListener != null) {
                HotkeyListener.CheckBGWorker();
            }
            timer.Enabled = true;
        }
    }
}