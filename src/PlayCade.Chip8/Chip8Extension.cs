using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cade.Common.Interfaces;
using PlayCade.Chip8.Core;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace PlayCade.Chip8
{
    public class Chip8Extension : CadeExtension
    {
        public readonly CoreManager _core;
        private const int UpdatesPerSecond = 25;
        private const int WaitTicks = 1000 / UpdatesPerSecond;
        private const int MaxFrameSkip = 5;
        private const int MaxUpdatesPerSecond = 60;
        private const int MinWaitTicks = 1000 / MaxUpdatesPerSecond;
        private CancellationTokenSource _tokenSource;

        public override void Load(string path)
        {
            var game = File.ReadAllBytes(path);
            _core.Load(game);
        }
        

        public override void Run()
        {

            var token = _tokenSource.Token;

            Task.Run(() =>
            {
                long nextUpdate = Environment.TickCount;
                long lastUpdate = Environment.TickCount;


                var outputManager = OutputManager as Chip8OutputManager;
                
                while (!token.IsCancellationRequested)
                {
                    
                    lastUpdate = Environment.TickCount;
                    var framesSkipped = 0;
                    while (Environment.TickCount > nextUpdate && framesSkipped < MaxFrameSkip)
                    {
                        var inputManager = InputManager as Chip8InputManager;
                        _core.Keys = inputManager?.CheckKeys();
                        _core.EmulateCycle();
                        // UPDATE HERE
                        nextUpdate += WaitTicks;
                        framesSkipped++;
                    }
                    
                    outputManager!.Graphics = _core.Graphics;
                    if(_core.DrawFlag)
                        outputManager.Draw();
                    _core.DrawFlag = false;
                }
                
            }, token);
        }

        public override string CoreName { get; }
        public override string CoreDescription { get; }
        public override string CoreDeveloper { get; }
        public override string PlatformName { get; }
        public override string PlatformDescription { get; }
        public override string PlatformDeveloper { get; }
        public override int MaxPlayers { get; }
        public override DateTime ReleaseDate { get; }
        public override string[] SupportedFileExtensions { get; }


        public Chip8Extension(GraphicsDevice graphicsDevice, CancellationTokenSource tokenSource) : base(new Chip8InputManager(), new Chip8OutputManager(graphicsDevice))
        {
            _core = new CoreManager();
            _tokenSource = tokenSource;

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