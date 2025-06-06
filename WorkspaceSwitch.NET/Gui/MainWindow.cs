using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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
        private Icon[] DisabledWorkspaceIcons;
        private int LastDesktop;
        private bool LastRunning;

        private readonly string tooltipTitle = "Messages";
        private readonly string popupTitle = "Errors";
        private readonly string fatalPrefix = "Fatal error. Exiting.";

        private NotifyIcon trayIcon;

        private MenuItem MenuItemSuspend;
        private MenuItem MenuItemContinue;

        /// <summary>
        /// Constructor for MainWindow, takes a HotkeyListener.
        /// </summary>
        /// <param name="hotkeyListener"></param>
        public MainWindow(HotkeyListener hotkeyListener)
        {
            LastDesktop = -1;
            HotkeyListener = hotkeyListener;

            this.timer = new System.Windows.Forms.Timer();

            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            AppIcon = new Icon(assembly.GetManifestResourceStream("AppIcon"));

            WorkspaceIcons = new Icon[10];
            DisabledWorkspaceIcons = new Icon[10];
            
            InitializeResources(assembly);

            var components = new System.ComponentModel.Container();
            trayIcon = new NotifyIcon(components);
            MenuItemSuspend = new MenuItem();
            MenuItemContinue = new MenuItem();

            InitialiseForm();
            InitializeTray();

            hotkeyListener.RunningChanged += (sender, e) => {
                MenuItemContinue.Enabled = !hotkeyListener.Running;
                MenuItemSuspend.Enabled = hotkeyListener.Running;
            };

            hotkeyListener.OnCompleted += () => {
                Logger.GetInstance().Log("Hotkeys suspended", MessageType.Info, PresentationType.ToolTip);
            };
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
        private void InitializeResources(System.Reflection.Assembly assembly)
        {
            for (var i = 1; i <= 10; i++)
            {
#if DEBUG
                WorkspaceIcons[i - 1] = new Icon(assembly.GetManifestResourceStream(String.Format("Icon{0}_dev", i)));
                DisabledWorkspaceIcons[i - 1] = new Icon(assembly.GetManifestResourceStream(String.Format("Icon{0}_dev_disabled", i)));
#else
                WorkspaceIcons[i - 1] = new Icon(assembly.GetManifestResourceStream(string.Format("Icon{0}", i)));
                DisabledWorkspaceIcons[i - 1] = new Icon(assembly.GetManifestResourceStream(string.Format("Icon{0}_disabled", i)));
#endif
            }
        }

        /// <summary>
        /// Initializes the tray icon.
        /// </summary>
        private void InitializeTray()
        {
            var trayMenu = new ContextMenu();
            var menuItemAbout = new MenuItem();
            var menuItemExit = new MenuItem();
            var menuItemLog = new MenuItem();
            var menuItemRefresh = new MenuItem();
            var menuItemRestart = new MenuItem();

            menuItemAbout.Text = "About...";
            menuItemAbout.Click += new EventHandler(MenuItemAbout_Click);

            menuItemExit.Text = "Exit";
            menuItemExit.Click += new EventHandler(MenuItemExit_Click);

            menuItemRestart.Text = "Restart";
            menuItemRestart.Click += new EventHandler(MenuItemExit_Restart);

            menuItemLog.Text = "Show log...";
            menuItemLog.Click += new System.EventHandler(this.MenuItemLog_Click);

            menuItemRefresh.Text = "Refresh Workspace";
            menuItemRefresh.Click += new EventHandler(MenuItemRefresh_Click);

            MenuItemSuspend.Text = "Suspend Hotkeys";
            MenuItemSuspend.Click += (sender, e) => HotkeyListener.Stop();
            MenuItemSuspend.Enabled = HotkeyListener.Running;

            MenuItemContinue.Text = "Continue Hotkeys";
            MenuItemContinue.Click += (sender, e) => HotkeyListener.Run();
            MenuItemContinue.Enabled = !HotkeyListener.Running;

            trayMenu.MenuItems.AddRange(new MenuItem[] { menuItemRefresh, MenuItemSuspend, MenuItemContinue, menuItemAbout, menuItemLog, menuItemRestart, menuItemExit });

            trayIcon.Text = "WorkspaceSwitch.NET";
            trayIcon.Icon = AppIcon;
            trayIcon.Visible = true;
            trayIcon.ContextMenu = trayMenu;
        }

        /// <summary>
        /// Method to handle Click event for restart menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void MenuItemExit_Restart(object sender, EventArgs e) {
            Application.Restart();
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
            var cd = -1;
            try {
                cd = Desktop.FromDesktop(Desktop.Current);
            } catch (COMException e) {
                // very likely the RPC connection died, no way to recover from this with VirtualDesktop
                Logger.GetInstance().Log("Cannot retrieve virtual desktop at this time, restarting Application.", MessageType.Info, PresentationType.None);
                Application.Restart();
                return;
            }

            if (cd != LastDesktop || LastRunning != HotkeyListener.Running)
            {
                Icon icon;
                if (cd >= 0 && cd < 10)
                {
                    icon = (HotkeyListener.Running) ? WorkspaceIcons[cd] : DisabledWorkspaceIcons[cd];
                }
                else
                {
                    icon = AppIcon;
                }

                trayIcon.Icon = icon;
                LastDesktop = cd;
                LastRunning = HotkeyListener.Running;
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
