using System;
using System.IO;
using BraketsEngine;

class Program
{
    static void Main(string[] args)
    {
        bool startBridgeClient = false;
        string hostname = "";
        int port = 0;
        string path = Directory.GetCurrentDirectory();
        float diagnosticRefreshRate = 0;

        if (args.Length > 0)
        {
            startBridgeClient = args[0] == "bridge";
            hostname = args[1];
            port = int.Parse(args[2]);
            path = args[3];
            diagnosticRefreshRate = float.Parse(args[4]);
        }
        if (startBridgeClient)
        {
            Globals.BRIDGE_Run = true;
            Globals.BRIDGE_Hostname = hostname;
            Globals.BRIDGE_Port = port;
            Globals.BRIDGE_RefreshRate = diagnosticRefreshRate;
        }
        Globals.Args_Path = path;

        Console.WriteLine("Hello, Curious Player!");

        using var game = new BraketsEngine.Main();
        game.Run();
    }
}