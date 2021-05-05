using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Console
{
    static class Program
    {
        private static object lockObject = new();
        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);

            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var httpClient = new HttpClient(handler);
            var tse = new Services.Crawlers.Tsetmc.TsetmcCrawlerService(httpClient);


            tse.GetTopSupplyAndDemandAsync().GetAwaiter().GetResult();
            //var t3 = Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        var startTime = DateTime.Now;

            //        tse.GetCashMarketAtGlanceAsync().GetAwaiter().GetResult();

            //        var finishTime = DateTime.Now;
            //        if ((finishTime - startTime).TotalSeconds < 1)
            //            Task.Delay(finishTime - startTime).GetAwaiter().GetResult();
            //    }
            //});

            var t1 = Task.Run(() => tse.SaveSymbolData());
            var t2 = Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var startTime = DateTime.Now;

                        tse.GetLatestSymbolDataAsync().GetAwaiter().GetResult();

                        var finishTime = DateTime.Now;

                        System.Console.WriteLine($"{ (finishTime - startTime).TotalSeconds}");
                        System.Console.WriteLine($"*********************************************");

                        if ((finishTime - startTime).TotalSeconds < 1)
                            Task.Delay(finishTime - startTime).GetAwaiter().GetResult();


                    }
                    catch (Exception ex)
                    {

                        LogException(ex);
                    }

                }
            });


            while (!isclosing) ;


            tse.SaveAllSymbolData().GetAwaiter().GetResult();
        }

        public static void LogException(Exception exception)
        {
            lock (lockObject)
            {
                var content = $"{DateTime.Now.ToLongTimeString()}{Environment.NewLine}";
                content += $"{exception.Message}{Environment.NewLine}";
                content += $"-------------------------------------------------------{Environment.NewLine}";
                content += $"{exception.StackTrace}{Environment.NewLine}";
                content += $"***********************************************************************************************{Environment.NewLine}";
                File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "log.txt"), content);
            }

        }

        private static bool isclosing = false;
        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            // Put your own handler here
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                    isclosing = true;
                    System.Console.WriteLine("CTRL+C received!");
                    break;

                case CtrlTypes.CTRL_BREAK_EVENT:
                    isclosing = true;
                    System.Console.WriteLine("CTRL+BREAK received!");
                    break;

                case CtrlTypes.CTRL_CLOSE_EVENT:
                    isclosing = true;
                    System.Console.WriteLine("Program being closed!");
                    break;

                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    isclosing = true;
                    System.Console.WriteLine("User is logging off!");
                    break;
            }
            return true;
        }

        #region unmanaged

        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.
        [System.Runtime.InteropServices.DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        #endregion

    }
}
