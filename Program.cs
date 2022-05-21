using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Interop;
using System.Windows.Forms;
using VirtualDesktop;
using System.ComponentModel;

namespace WorkspaceSwitcher
{
    public static class Program
    {

        public static void Main(string[] args)
        {
            var WorkerManager = new BackgroundWorkerManager();
            var g = new Gui(WorkerManager);
            
            Application.Run(g);
        }
    }

}

