using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using VirtualDesktop;

namespace WorkspaceSwitcher {
    public class Gui : ApplicationContext {
            private System.Windows.Forms.Timer timer;
            private BackgroundWorkerManager WorkerManager;
            private Icon AppIcon;
            private Icon[] WorkspaceIcons;
            private int LastDesktop;
            

            private NotifyIcon trayIcon;
            public Gui(BackgroundWorkerManager manager) {
                LastDesktop = -1;
                WorkerManager = manager;
                //FIXME: Constant
                this.timer = new System.Windows.Forms.Timer();
                timer.Interval = 500;
                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();
                this.InitializeResources();
                this.InitializeTray();
            }

            private void InitializeResources() {
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
                //AppIcon = ((Icon)Resources.GetObject("AppIcon"));
                AppIcon = new Icon(assembly.GetManifestResourceStream("AppIcon"));
                WorkspaceIcons = new Icon[10];

                for (var i = 1; i <= 10; i++) {
                    //WorkspaceIcons[i] = ((Icon)Resources.GetObject(String.Format("Icon{0}", i)));
                    WorkspaceIcons[i-1] = new Icon(assembly.GetManifestResourceStream(String.Format("Icon{0}",i)));
                }
            }

            private void InitializeTray() {
                var components = new System.ComponentModel.Container();
                trayIcon = new NotifyIcon(components);
                var trayMenu = new ContextMenu();
                var menuItemAbout = new MenuItem();
                var menuItemExit = new MenuItem();
                var menuItemRefresh = new MenuItem();

                menuItemAbout.Index = 1;
                menuItemAbout.Text = "About...";
                menuItemAbout.Click += new System.EventHandler(this.menuItemAbout_Click);

                menuItemExit.Index = 2;
                menuItemExit.Text = "Exit";
                menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);

                menuItemRefresh.Index = 0;
                menuItemRefresh.Text = "Refresh";
                menuItemRefresh.Click += new System.EventHandler(this.menuItemRefresh_Click);

                trayMenu.MenuItems.AddRange(new MenuItem[] {menuItemRefresh, menuItemAbout, menuItemExit} );

                trayIcon.Text = "WorkspaceSwitch.NET";
                trayIcon.Icon = AppIcon;
                trayIcon.Visible = true;
                trayIcon.ContextMenu = trayMenu;
            }

            public void menuItemAbout_Click(object Sender, EventArgs e) {
                MessageBox.Show("WorkspaceSwitch.NET 0.1. © René D.Obermueller 2022");
            }

            public void menuItemExit_Click(object Sender, EventArgs e) {
                this.exit();
            }

            public void menuItemRefresh_Click(object Sender, EventArgs e) {
                trayIcon.Icon = AppIcon;
                LastDesktop = -1;
            }

            public void exit() {
                WorkerManager.CancelWorkers();
                trayIcon.Visible = false;
                trayIcon.Dispose();
                Application.Exit();
            }

            public void timer_Tick(Object o, EventArgs args) {
                timer.Stop();
                var cd = Desktop.FromDesktop(Desktop.Current);
                
                if (cd != LastDesktop) {
                    Icon icon;
                    if (cd >= 0 && cd < 10) {
                        icon = WorkspaceIcons[cd];
                    } else {
                        icon = AppIcon;
                    }
                    
                    trayIcon.Icon =  icon;
                    LastDesktop = cd;
                }
                timer.Enabled = true;
            }

    }
}