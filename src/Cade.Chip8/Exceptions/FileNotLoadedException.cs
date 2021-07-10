using System;

namespace Cade.Chip8.Exceptions
{
    public class FileNotLoadedException : Exception
    {
        public override string Message => "File Not Loaded";
    }
}