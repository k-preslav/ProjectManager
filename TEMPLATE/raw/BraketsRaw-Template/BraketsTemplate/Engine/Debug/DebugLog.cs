using System;

namespace BraketsEngine;

public static class Debug
{
    public static void Log(string msg, object sender=null)
    {
        MESSAGE(msg, sender);
    }

    public static void Warning(string msg, object sender=null)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        MESSAGE("WARNING: " + msg, sender);
    }

    public static void Error(string msg, object sender=null)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        MESSAGE("ERROR: " + msg, sender);
    }

    public static void Fatal(string msg, object sender=null)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.DarkRed;
        MESSAGE("FATAL: " + msg, sender);

        Environment.Exit(1);
    }

    private static void MESSAGE(string msg, object sender)
    {
        string message = "";
        if (sender is not null)
            message += $"[{sender}] ";

        message += msg;

        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.Gray;
    }
}