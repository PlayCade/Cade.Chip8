using System;

namespace Cade.Chip8.Exceptions
{
    public class FileToLargeException : Exception
    {
        public override string Message => "File is too large.";
    }
}