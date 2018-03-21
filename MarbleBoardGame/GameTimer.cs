using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarbleBoardGame
{
    public delegate void GameTick(GameTime gameTime);

    public class GameTimer : IDisposable
    {
        private bool running;
        private long targetFps;

        private GameTime gameTime;
        private GameTick gameTick;
        private Thread thread;
        private ManualResetEvent threadHandle;

        public GameTime GetGameTime()
        {
            return gameTime;
        }

        private void Loop()
        {
            long targetMs = 1000 / targetFps;
            

            Stopwatch sw = Stopwatch.StartNew();
            long lastTimestamp = sw.ElapsedMilliseconds;

            while (running)
            {
                long totalMs = sw.ElapsedMilliseconds;
                long elapsed = totalMs - lastTimestamp;
                lastTimestamp = sw.ElapsedMilliseconds;
                long sleeptime = Math.Max(0, targetMs - elapsed);         

                gameTime = new GameTime(sw.Elapsed, new TimeSpan(0, 0, 0, 0, (int)elapsed));
                gameTime.IsRunningSlowly = elapsed > targetMs;
                gameTick.Invoke(gameTime);

                Thread.Sleep((int)sleeptime);      
            }

            threadHandle.Set();
        }

        public void Start(long targetFps)
        {
            this.targetFps = targetFps;
            if (running)
            {
                return;
            }

            running = true;
            thread.Start();
        }

        public void Stop()
        {
            if (running)
            {
                running = false;
                threadHandle.WaitOne();
            }
        }

        public void Dispose()
        {
            Stop();
            threadHandle.Dispose();
        }

        public GameTimer(GameTick gameTick)
        {
            this.gameTime = new GameTime();
            this.gameTick = gameTick;
            this.thread = new Thread(Loop);
            this.thread.Name = "GameThreading";

            threadHandle = new ManualResetEvent(false);
        }
    }
}
