using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace instantMessagingClient.Model
{
    //send a heartbeat to the server so it knows if a user is online or not
    public class Heartbeat
    {

        private static Heartbeat instance { get; set; }

        public static Heartbeat getInstance()
        {
            return instance ??= new Heartbeat();
        }

        private Heartbeat()
        {
            restClient = new Rest();
        }

        private readonly Rest restClient;
        private bool isRunning;
        private Task hearbeatTask;

        public void start()
        {
            if(hearbeatTask == null ||
                hearbeatTask.IsCanceled ||
                hearbeatTask.IsFaulted ||
                hearbeatTask.IsCompleted)
            {
                isRunning = true;
                hearbeatTask = new Task(() =>
                {
                    while (isRunning)
                    {
                        restClient.sendHeartbeat();
                        Thread.Sleep(60_000);
                    }
                });
                hearbeatTask.Start();
            }
        }

        public void stop()
        {
            isRunning = false;
            hearbeatTask.Wait(1000);
            hearbeatTask.Dispose();
        }

    }
}
