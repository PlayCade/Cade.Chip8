using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cade.Common.Interfaces;
using PlayCade.Chip8.Core;

namespace PlayCade.Chip8
{
    public class Chip8Extension : ICadeExtension
    {
        private CoreManager _core;
        private const int UpdatesPerSecond = 25;
        private const int WaitTicks = 1000 / UpdatesPerSecond;
        private const int MaxFrameskip = 5;
        private const int MaxUpdatesPerSecond = 60;
        private const int MinWaitTicks = 1000 / MaxUpdatesPerSecond;
        private bool _isRunning;
        private readonly Chip8InputManager _inputManager = new Chip8InputManager();
        
        public void Load(string path)
        {
            var game = File.ReadAllBytes(path);
            _core = new CoreManager();
            _core.Load(game);
        }

        public async void Run()
        {
            await Task.Run(() =>
            {
                long nextUpdate = Environment.TickCount;
                long lastUpdate = Environment.TickCount;

                _isRunning = true;

                while (_isRunning)
                {
                    while (Environment.TickCount < lastUpdate + MinWaitTicks)
                    {
                        Thread.Sleep(0);
                    }

                    lastUpdate = Environment.TickCount;
                    var framesSkipped = 0;
                    while (Environment.TickCount > nextUpdate && framesSkipped < MaxFrameskip)
                    {
                        _core.Keys = _inputManager.CheckKeys();
                        _core.EmulateCycle();
                        // UPDATE HERE
                        nextUpdate += WaitTicks;
                        framesSkipped++;
                    }

                    // Calculate interpolation for smooth animation between states:
                    //var interpolation = ((float)Environment.TickCount + _waitTicks - nextUpdate) / _waitTicks;

                    // Render-events:
                    // repaint(interpolation);
                }
            });
        }

        public string CoreName { get; }
        public string CoreDescription { get; }
        public string CoreDeveloper { get; }
        public string PlatformName { get; }
        public string PlatformDescription { get; }
        public string PlatformDeveloper { get; }
        public int MaxPlayers { get; }
        public DateTime ReleaseDate { get; }
        public string[] SupportedFileExtensions { get; }

        public Chip8Extension()
        {
            CoreName = "Chip8";
            CoreDescription = "CHIP-8 Extension for the Cade Arcade System";
            CoreDeveloper = "Cade";
            PlatformName = "CHIP-8";
            PlatformDescription = "Chip 8 Interpreter";
            PlatformDeveloper = "Joseph Weisbecker";
            MaxPlayers = 1;
            ReleaseDate = new DateTime(1970, 01, 01);
            SupportedFileExtensions = new [] {"ch8"};
        }
    }
}