using System.Collections.Generic;
using System.ComponentModel;

namespace WorkspaceSwitcher {
    public class BackgroundWorkerManager {

        private List<BackgroundWorker> Workers;
        public BackgroundWorkerManager() {
            Workers = new List<BackgroundWorker>();
            var w = KeyListener.run();
            Workers.Add(w);
        }

        public void CancelWorkers() {
            foreach (var w in Workers) {
                if (w.WorkerSupportsCancellation) {
                    w.CancelAsync();
                }
            }
        }
    }
}