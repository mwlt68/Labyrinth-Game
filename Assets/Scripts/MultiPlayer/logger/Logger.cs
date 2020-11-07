using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Logger
{
    public static void i(string message)
    {
        Log(LogLevel.INFO, message);
    }
    public static void w(string message)
    {
        Log(LogLevel.WARN, message);
    }
    public static void e(string message)
    {
        Log(LogLevel.ERR, message);
    }

    public static void Log(LogLevel logLevel, string message)
    {
        switch (logLevel)
        {
            case LogLevel.INFO:
                Debug.Log(message);
                break;
            case LogLevel.WARN:
                Debug.LogWarning(message);
                break;
            case LogLevel.ERR:
                Debug.LogError(message);
                break;
            default:
                break;
        }
    }

    private static string ToString(LogLevel logLevel)
    {
        switch (logLevel)
        {
            case LogLevel.INFO:
                return "info";
            case LogLevel.WARN:
                return "warn";
            case LogLevel.ERR:
                return "err";
            default:
                return "impossible";  // :)
        }
    }
}

/// <summary>
/// Log level
/// </summary>
public enum LogLevel
{
    INFO,
    WARN,
    ERR,
}

