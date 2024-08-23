using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BraketsEngine
{
    public class DebugData
    {
        static float lastMem;
        static int lastThreadsCount;

        static float getDataCooldown = 1.25f;
        static float getDataTimer = 0.1f;
        static bool canGetData = false;

        public static void GetDiagnostics()
        {
            getDataTimer -= Globals.DEBUG_DT;
            if (getDataTimer <= 0)
            {
                canGetData = true;
                getDataTimer = getDataCooldown;
                return;
            }

            Task.Run(() => {
                GetMemoryUsage();
                GetGCCalls();
                GetThreadsCount();
            });
        }

        static float GetMemoryUsage()
        {
            if (!canGetData) return lastMem;

            float mem = Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024);
            Globals.DEBUG_MEMORY = mem;
            lastMem = mem;
            return mem;
        }

        static int GetGCCalls()
        {
            int gc = GC.CollectionCount(0) + GC.CollectionCount(1) + GC.CollectionCount(2);
            Globals.DEBUG_GC_CALLS = gc;
            return gc;
        }

        static int GetThreadsCount()
        {
            if (!canGetData) return lastThreadsCount;

            int threads = Process.GetCurrentProcess().Threads.Count;
            Globals.DEBUG_THREADS_COUNT = threads;
            lastThreadsCount = threads;
            return threads;
        }
    }
}
