/*
 *  Copyright (C) 2022 Jon Dennis
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Voxam.MPEG1ToolKit.Threading
{
    public class ThreadWorkerPool : IDisposable
    {
        private readonly int _threadCount;
        private readonly Thread[] _threads;
        private readonly List<EnqueuedJob> _enqueuedJobs = new List<EnqueuedJob>();
        private Mutex _mutex = new Mutex();

        public ThreadWorkerPool(int threadCount)
        {
            _threadCount = threadCount;
            _threads = new Thread[_threadCount];
            for (int i = 0; i < _threadCount; ++i)
            {
                _threads[i] = new Thread(new ThreadStart(workerRun));
                _threads[i].Start();
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                for (int i = 0; i < _threads.Length; i++)
                {
                    _enqueuedJobs.Add(new EnqueuedJob(null));
                }
                Monitor.PulseAll(this);
            }

            foreach (var t in _threads)
                t.Join();
        }

        public IEnqueuedJob EnqueueJob(ThreadStart ts)
        {
            if (ts == null) return null;

            lock (this)
            {
                var rv = new EnqueuedJob(ts);
                _enqueuedJobs.Add(rv);
                Monitor.Pulse(this);
                return rv;
            }
        }




        private void workerRun()
        {
            while (true)
            {
                var job = TakeJob();
                if (job.ShutdownWorker) break;
                job.RunJob();
            }
        }

        private EnqueuedJob TakeJob()
        {
            lock (this)
            {
                while (_enqueuedJobs.Count < 1)
                {
                    Monitor.Wait(this);
                }
                var rv = _enqueuedJobs.First();
                _enqueuedJobs.RemoveAt(0);
                return rv;
            }
        }

        private class EnqueuedJob : IEnqueuedJob
        {
            private readonly ThreadStart _ts;
            private bool _hasStarted = false;
            private bool _hasCompleted = false;

            public EnqueuedJob(ThreadStart ts)
            {
                _ts = ts;
            }

            public bool ShutdownWorker => _ts == null;

            public bool HasCompleted
            {
                get
                {
                    lock (this) return _hasCompleted;
                }
            }

            public bool HasStarted
            {
                get
                {
                    lock (this) return _hasStarted;
                }
            }

            public void WaitForCompletion()
            {
#warning understand busy wait with bitmap lock issue!!!
                /*
                lock(this)
                {
                    while (!_hasCompleted)
                    {
                        Monitor.Wait(this);
                    }
                }
                */
                while (true)
                {
                    lock (this) { if (_hasCompleted) break; }
                }
            }

            public void RunJob()
            {
                lock (this)
                {
                    _hasStarted = true;
                }

                try
                {
                    _ts?.Invoke();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }

                lock (this)
                {
                    _hasCompleted = true;
                    Monitor.PulseAll(this);
                }
            }
        }
    }
}
