using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Common.LoggerBuilder
{
    public class LoggerBuilderSingleton
    {
        private static LoggerBuilderSingleton _instance;
        private static readonly object instanceLock = new object();
        private  static Logger _loggerConfiguration;
        private LoggerBuilderSingleton()
        {
            _loggerConfiguration = new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .WriteTo.Console(
                                                restrictedToMinimumLevel: LogEventLevel.Debug,
                                                outputTemplate:"[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                                             )
                            .WriteTo.File(
                                            "log.txt",
                                            rollingInterval: RollingInterval.Day,
                                            rollOnFileSizeLimit: true
                                           )
                            .CreateLogger();

        }
        private static LoggerBuilderSingleton Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (_instance == null) { _instance = new LoggerBuilderSingleton(); }
                }
                return _instance;

            }
        }

        public static Logger InventoryLog
        {
            get
            {
                if (_loggerConfiguration == null)
                {
                    var x = Instance;
                }
                return _loggerConfiguration;
            }
        }
    }
}
