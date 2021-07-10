using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cade.Chip8.Core;
using Cade.Common.Interfaces;
using Veldrid;

namespace Cade.Chip8
{
    public class Chip8Extension : CadeExtension
    {
        public readonly CoreManager Core;
        private readonly CancellationTokenSource _tokenSource;

        public override void Load(string path)
        {
            var game = File.ReadAllBytes(path);
            Core.Load(game);
        }


        public override void Run()
        {
            var token = _tokenSource.Token;


            var outputManager = OutputManager as Chip8OutputManager;
            var inputManager = InputManager as Chip8InputManager;

            Task.Factory.StartNew(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                while (!token.IsCancellationRequested)
                {
                    if (sw.Elapsed >= TimeSpan.FromSeconds(1.0 / 540))
                    {
                        Core.EmulateCycle();
                        sw.Restart();
                    }

                    Thread.Yield();
                }
            }, token);

            Task.Factory.StartNew(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                while (!token.IsCancellationRequested)
                {
                    if (sw.Elapsed >= TimeSpan.FromSeconds(1.0 / 60))
                    {
                        if (Core.DelayTimer > 0)
                        {
                            Core.DelayTimer--;
                        }

                        if (Core.SoundTimer > 0)
                        {
                            Console.WriteLine("Beep");
                            Core.SoundTimer--;
                        }

                        outputManager!.Graphics = Core.Graphics;
                        if (Core.DrawFlag)
                            outputManager.Draw();
                        Core.DrawFlag = false;

                        Core.Keys = inputManager?.CheckKeys();
                        sw.Restart();
                    }

                    Thread.Yield();
                }
            }, token);
        }

        public override void Close()
        {
            Core.Reset();
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


        public Chip8Extension(GraphicsDevice graphicsDevice, CancellationTokenSource tokenSource) : base(
            new Chip8InputManager(), new Chip8OutputManager(graphicsDevice))
        {
            Core = new CoreManager();
            _tokenSource = tokenSource;

            CoreName = "Chip8";
            CoreDescription = "CHIP-8 Extension for the Cade Arcade System";
            CoreDeveloper = "Cade";
            PlatformName = "CHIP-8";
            PlatformDescription = "Chip 8 Interpreter";
            PlatformDeveloper = "Joseph Weisbecker";
            MaxPlayers = 1;
            ReleaseDate = new DateTime(1970, 01, 01);
            SupportedFileExtensions = new[] { "ch8" };
        }
    }
}