using instantMessagingCore.Models.Dto;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace instantMessagingServer.Models
{
    public class LogsManager
    {
        // Singleton
        private static LogsManager instance { get; set; }

        private const int memoryLogsSize = 1_000;
        private readonly ConcurrentQueue<Logs> LogsQueue;

        private readonly bool isEnable;

        private DatabaseContext db { get; set; }

        public static LogsManager GetInstance()
        {
            return instance ??= new LogsManager();
        }

        private LogsManager()
        {
            isEnable = (Config.Configuration["InternalLog:enable"] == "1") ? true : false;

            LogsQueue = new ConcurrentQueue<Logs>();
            db = new DatabaseContext(Config.Configuration);

            if (isEnable)
            {
                Task.Factory.StartNew(() =>
                {
                    int count;
                    Logs log;
                    while (true)
                    {
                        count = 0;
                        try
                        {
                            while (LogsQueue.TryDequeue(out log) && count < memoryLogsSize)
                            {
                                ++count;
                                db.Logs.Add(log);
                            }
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(ex.Message);
                            Console.ResetColor();
                        }
                        Thread.Sleep(1000);
                    }
                });
            }

        }

        // Instance

        public void write(Logs log)
        {
            if (isEnable)
                try
                {
                    LogsQueue.Enqueue(log);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
                
        }
        public void write(Logs.EType type, string message)
        {
            write(new Logs(type, message));
        }
        public void write(string type, string message)
        {
            write(new Logs(type, message));
        }
    }
}
