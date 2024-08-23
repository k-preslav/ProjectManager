using System;
using System.Threading.Tasks;

namespace BraketsEngine;

public class DebugBridge
{
        public static void SendData()
        {
            if (Globals.BRIDGE_Client != null)
            {
                string data = $"{Globals.DEBUG_FPS}|{Globals.DEBUG_MEMORY}|" +
                              $"{Globals.ENGINE_Main.Sprites.Count}|{Globals.DEBUG_GC_CALLS}|" +
                              $"{Globals.DEBUG_THREADS_COUNT}|{Globals.DEBUG_DT}";

                Globals.BRIDGE_Client.SendMessageAsync(data);
            }
        }
}