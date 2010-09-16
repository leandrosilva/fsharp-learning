using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

partial class Program
{
    static byte[] quote;
    static int numWritten = 0;

    static void WriteStockQuote(Stream stream)
    {
        Thread.Sleep(1000);  // Mock an I/O wait for 1s for the next quote
        stream.Write(quote, 0, quote.Length);
        Interlocked.Increment(ref numWritten);
    }

    static void ServiceClient(TcpClient client)
    {
        using (var stream = client.GetStream())
        {
            stream.Write(quote, 0, 1);  // write header
            while (true)
            {
                WriteStockQuote(stream);
            }
        }
    }

    static void SyncMain(string[] args)
    {
        var anyErrors = false;
        var socket = new TcpListener(IPAddress.Loopback, 10003);
        var requestCount = 0;
        socket.Start();
        var t = new Thread(new ThreadStart(() =>
        {
            while (true)
            {
                var client = socket.AcceptTcpClient();
                requestCount = requestCount + 1;
                if (requestCount % 100 == 0)
                {
                    System.Console.WriteLine("{0} accepted...", requestCount);
                }
                var t1 = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        using (var _holder = client)
                        {
                            ServiceClient(client);
                        }
                    }
                    catch (Exception e)
                    {
                        if (!anyErrors)
                        {
                            anyErrors = true;
                            System.Console.WriteLine("server ERROR");
                        }
                        throw;
                    }
                }));
                t1.IsBackground = true;
                t1.Start();
            }
        }));
        t.IsBackground = true;
        t.Start();
        while (true)
        {
            Thread.Sleep(1000);
            var count = Interlocked.Exchange(ref numWritten, 0);
            System.Console.WriteLine("QUOTES PER SECOND: {0}", count);
        }
    }

    static void Main(string[] args)
    {
        quote = new byte[512];
        for (int i = 0; i < quote.Length; i++)
            quote[i] = 1;
        SyncMain(args);
    }

}
