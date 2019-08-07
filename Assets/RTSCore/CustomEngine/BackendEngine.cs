using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Assets.RTSCore.CustomEngine
{
    public static class BackendEngine
    {
        private static bool _running;
        private static Thread _workerThread;

        public static void Initialize()
        {
            _running = true;
            _workerThread = new Thread(WorkerThread);
            _workerThread.Start();
        }

        public static void Shutdown()
        {
            _running = false;
        }

        private static readonly Queue<IEngineTask> TaskQueue = new Queue<IEngineTask>();

        public static void AddTask(IEngineTask newTask)
        {
            lock (TaskQueue)
            {
                TaskQueue.Enqueue(newTask);
            }
        }

        public static void WorkerThread()
        {
            IEngineTask currentTask;
            while (_running)
            {
                Thread.Sleep(500);

                lock (TaskQueue)
                {
                    currentTask = TaskQueue.Count > 0 ? TaskQueue.Dequeue() : null;
                }

                while (currentTask != null)
                {
                    currentTask.Run();

                    lock (TaskQueue)
                    {
                        currentTask = TaskQueue.Count > 0 ? TaskQueue.Dequeue() : null;
                    }
                }
            }
        }
    }
}
