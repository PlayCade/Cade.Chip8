using System;
using System.Threading;
using Cade.Chip8.Core;
using Cade.Chip8.Exceptions;
using Xunit;

namespace Cade.Chip8.Tests
{
    public class CoreUnitTests
    {
        [Fact]
        public void Pc_Default_ReturnsTrue()
        {
            var core = new CoreManager();

            var pc = core.Pc;
            
            Assert.Equal(0x200, pc);
        }

        [Fact]
        public void FileLoaded_Default_ReturnFalse()
        {
            var core = new CoreManager();
            var fileLoaded = core.FileLoaded;
            
            Assert.False(fileLoaded);
        }
        
        [Fact]
        public void FileLoaded_NoFile_ThrowsFileNotLoadedException()
        {
            var core = new CoreManager();

            Action actual = () => core.Start(new CancellationTokenSource());
            
            Assert.Throws<FileNotLoadedException>(actual);
        }
        
        [Fact]
        public void FileLoaded_FileSize4096_ThrowsFileTooLargeException()
        {
            var core = new CoreManager();
            
            Action actual = () => core.Load(new byte[4096]);
            
            Assert.Throws<FileToLargeException>(actual);
        }
        
        [Fact]
        public void FileLoaded_FileSize3585_ThrowsFileTooLargeException()
        {
            var core = new CoreManager();
            
            Action actual = () => core.Load(new byte[3585]);
            
            Assert.Throws<FileToLargeException>(actual);
        }
        
        [Fact]
        public void FileLoaded_FileSize3584_ReturnsTrue()
        {
            var core = new CoreManager();
            
            core.Load(new byte[3584]);
            var actual = core.FileLoaded;
            
            Assert.True(actual);
        }

        [Fact]
        public void FileLoaded_FileIsNull_ThrowsNullReferenceException()
        {
            var core = new CoreManager();

            Action actual = () => core.Load(null!);
            Assert.Throws<NullReferenceException>(actual);
        }

        [Fact]
        public void FileLoaded_Reset_ReturnsFalse()
        {
            var core = new CoreManager();
            
            core.Load(new byte[3584]);
            core.SetDefaults();
            var actual = core.FileLoaded;
            
            Assert.False(actual);
        }
    }
}