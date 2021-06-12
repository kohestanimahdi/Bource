using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Common.Utilities
{
    public static class ApplicationHelpers
    {
        public static string TseClientCompress(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            MemoryStream stream = new();
            using (var stream2 = new GZipStream(stream, CompressionMode.Compress, true))
            {
                stream2.Write(bytes, 0, bytes.Length);
            }
            stream.Position = 0L;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            byte[] dst = new byte[buffer.Length + 4];
            Buffer.BlockCopy(buffer, 0, dst, 4, buffer.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(bytes.Length), 0, dst, 0, 4);
            return Convert.ToBase64String(dst);
        }

        public static async Task DoFunctionsWithProgressBar<T>(List<T> symbols, Func<T, CancellationToken, int, Task> func, CancellationToken cancellationToken)
        {
            int n = 0;
            Common.Utilities.ConsoleHelper.WriteProgressBar(n);

            foreach (var symbol in symbols)
            {
                await func(symbol, cancellationToken, 0);
                n++;
                Common.Utilities.ConsoleHelper.WriteProgressBar((int)(n * 100.0 / symbols.Count), true);
            }
            Common.Utilities.ConsoleHelper.WriteProgressBar(100, true);
        }

        public static Task DoFunctionsOFListWithMultiTask<T>(List<T> symbols, Func<T, CancellationToken, int, Task> func, CancellationToken cancellationToken, int numberOfThreads = 5)
        {
            List<Task> tasks = new();
            int n = 0;
            int count = symbols.Count / numberOfThreads;

            while (n < symbols.Count)
            {
                var subSymbols = symbols.Skip(n).Take(count).ToList();

                tasks.Add(Task.Run(() => DoFunctionsOFSubListWithMultiTask(subSymbols, func, cancellationToken)));
                n += count;
            }

            return Task.WhenAll(tasks);
        }

        private static async Task DoFunctionsOFSubListWithMultiTask<T>(List<T> symbols, Func<T, CancellationToken, int, Task> func, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var symbol in symbols)
                await func(symbol, cancellationToken, 0);
        }

        public static string HangfirePasswordGenerator(string password)
        {
            string result = string.Empty;
            using (var cryptoProvider = System.Security.Cryptography.SHA1.Create())
            {
                byte[] passwordHash = cryptoProvider.ComputeHash(Encoding.UTF8.GetBytes(password));
                result = "new byte[] { " +
                   String.Join(",", passwordHash.Select(x => "0x" + x.ToString("x2")).ToArray())
                    + " } ";
            }

            return result;
        }

        public static async Task DoFuncEverySecond(Func<CancellationToken, Task> func, CancellationToken cancellationToken = default(CancellationToken))
        {
            var startTime = DateTime.Now;
            while (DateTime.Now < startTime.AddMinutes(1))
            {
                var time = DateTime.Now;
                await func(cancellationToken);
                var delay = DateTime.Now - time;
                if (delay < TimeSpan.FromSeconds(1))
                    await Task.Delay(TimeSpan.FromSeconds(1) - delay);
            }
        }
    }
}
