using System;

namespace IlvAlbumOptimizer.Utils
{
    public static class Logger
    {
        private static Action<string> WriteImpl { get; set; }
        private static Action<string> WriteLineImpl { get; set; }
        private static Action<string> WriteOnPrevLineImpl { get; set; }
        public static bool Verbose { get; set; }

        public static void Setup(Action<string> write, Action<string> writeLine, Action<string> writeOnPrevLineImpl)
        {
            WriteImpl = write;
            WriteLineImpl = writeLine;
            WriteOnPrevLineImpl = writeOnPrevLineImpl;
        }

        public static void Write(string message, bool isVerbose = false)
        {
            if (!isVerbose || !Verbose)
                WriteImpl?.Invoke(message);
        }
        public static void WriteLine(string message = "", bool isVerbose = false)
        {
            if (!isVerbose || !Verbose)
                WriteLineImpl?.Invoke(message);
        }
        public static void WriteOnPrevLine(string message = "", bool isVerbose = false)
        {
            if (!isVerbose || !Verbose)
                WriteOnPrevLineImpl?.Invoke(message);
        }
    }
}
