using System;
using System.Collections.Generic;
using System.Text;

namespace Vlingo
{
    public interface ILogger
    {
        //public static Logger noOpLogger()
        //{
        //    return new NoOpLogger();
        //}

        //public static Logger testLogger()
        //{
        //    return JDKLogger.testInstance();
        //}

        void Close();
        bool IsEnabled();
        void Log(String message);
        string Name { get; }
    }

    public interface ILoggerProvider
    {
        //public static LoggerProvider noOpLoggerProvider()
        //{
        //    return new NoOpLoggerProvider();
        //}

        //public static LoggerProvider standardLoggerProvider(final World world, final String name)
        //{
        //    return JDKLoggerPlugin.registerStandardLogger(name, world);
        //}

        void Close();
        ILogger CreateLogger();
    }

}
