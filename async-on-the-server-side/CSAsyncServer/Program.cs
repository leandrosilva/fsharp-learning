using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

internal abstract class AsyncResult : IAsyncResult
{
    private AsyncCallback callback;
    private bool completedSynchronously;
    private bool endCalled;
    private Exception exception;
    private bool isCompleted;
    private ManualResetEvent manualResetEvent;
    private object state;
    private object thisLock;
    private string stackTraceOfCallToEnd;

    public AsyncResult(AsyncCallback callback, object state)
    {
        this.callback = callback;
        this.state = state;
        this.thisLock = new object();
    }

    public void Complete(bool completedSynchronously)
    {
        if (this.isCompleted)
        {
            Debug.Assert(false);
        }
        this.completedSynchronously = completedSynchronously;
        if (completedSynchronously)
        {
            this.isCompleted = true;
        }
        else
        {
            lock (this.thisLock)
            {
                this.isCompleted = true;
                if (this.manualResetEvent != null)
                {
                    this.manualResetEvent.Set();
                }
            }
        }
        if (this.callback != null)
        {
            this.callback(this);
        }
    }

    public void Complete(bool completedSynchronously, Exception exception)
    {
        this.exception = exception;
        this.Complete(completedSynchronously);
    }

    public static TAsyncResult End<TAsyncResult>(IAsyncResult result) where TAsyncResult : AsyncResult
    {
        if (result == null)
        {
            throw new ArgumentNullException("result");
        }
        TAsyncResult local = result as TAsyncResult;
        if (local == null)
        {
            Debug.Assert(false);
            throw new ArgumentException("bad arg type", "result");
        }
        if (local.endCalled)
        {
            Debug.Assert(false);
            throw new InvalidOperationException("Async object already ended");
        }
        local.endCalled = true;
        local.stackTraceOfCallToEnd = new StackTrace().ToString();
        if (!local.isCompleted)
        {
            local.AsyncWaitHandle.WaitOne();
        }
        if (local.manualResetEvent != null)
        {
            local.manualResetEvent.Close();
        }
        if (local.exception != null)
        {
            throw local.exception;
        }
        return local;
    }

    public object AsyncState
    {
        get
        {
            return this.state;
        }
    }

    public WaitHandle AsyncWaitHandle
    {
        get
        {
            if (this.manualResetEvent == null)
            {
                lock (this.thisLock)
                {
                    if (this.manualResetEvent == null)
                    {
                        this.manualResetEvent = new ManualResetEvent(this.isCompleted);
                    }
                }
            }
            return this.manualResetEvent;
        }
    }

    public bool CompletedSynchronously
    {
        get
        {
            return this.completedSynchronously;
        }
    }

    public bool HasCallback
    {
        get
        {
            return (this.callback != null);
        }
    }

    public bool IsCompleted
    {
        get
        {
            return this.isCompleted;
        }
    }
}

class WriteStockQuoteAsyncResult : AsyncResult
{
    static int n = 0;
    public int N;
    public WriteStockQuoteAsyncResult(AsyncCallback callback, object state)
        : base(callback, state)
    {
        N = Interlocked.Increment(ref n);
    }
}

class ServiceClientAsyncResult : AsyncResult, IDisposable
{
    Stream toDispose;
    public ServiceClientAsyncResult(Stream toDispose, AsyncCallback callback, object state)
        : base(callback, state)
    {
        this.toDispose = toDispose;
    }
    public void Dispose()
    {
        toDispose.Dispose();
    }
}

partial class Program
{
    static byte[] quote;
    static int numWritten = 0;

    static void SleepThen(int milliSeconds, Action callback)
    {
        Timer timer = null;
        timer = new Timer(o =>
        {
            var tmp = timer;
            if (tmp != null)
                tmp.Dispose();
            callback();
        }, null, milliSeconds, -1);
    }
    static IAsyncResult BeginWriteStockQuote(Stream stream, AsyncCallback cb, object state)
    {
        var ar = new WriteStockQuoteAsyncResult(cb, state);
        stream.BeginWrite(quote, 0, quote.Length, (iar) =>
        {
            SleepThen(1000, () =>  // Mock an I/O wait for 1s for the next quote
            {
                Interlocked.Increment(ref numWritten);
                ar.Complete(false); // we always sleep
            });
        }, null);
        return ar;
    }

    static void EndWriteStockQuote(IAsyncResult iar)
    {
        AsyncResult.End<WriteStockQuoteAsyncResult>(iar);
    }

    static IAsyncResult BeginServiceClient(TcpClient client, AsyncCallback cb, object state)
    {
        var stream = client.GetStream();
        var ar = new ServiceClientAsyncResult(stream, cb, state);

        try
        {
            stream.Write(quote, 0, 1);  // write header
        }
        catch
        {
            stream.Dispose();
            throw;
        }
        var everWentAsync = false;
        Action<Action> wrap = (a) =>
        {
            try { a(); }
            catch (Exception e)
            {
                ar.Complete(!everWentAsync, e);
            }
        };
        Action loop = null;
        Action<IAsyncResult> end = (iar) =>
        {
            wrap(() => EndWriteStockQuote(iar));
        };
        AsyncCallback callback = (iar) =>
        {
            if (iar.CompletedSynchronously)
                return;
            end(iar);
            loop();
        };
        loop = () =>
        {
            wrap(() =>
            {
                while (true)
                {
                    var iar = BeginWriteStockQuote(stream, callback, state);
                    if (!iar.CompletedSynchronously)
                    {
                        everWentAsync = true;
                        break;
                    }
                    end(iar);
                }
            });
        };
        loop();
        return ar;
    }

    static void EndServiceClient(IAsyncResult iar)
    {
        try
        {
            AsyncResult.End<ServiceClientAsyncResult>(iar);
        }
        finally
        {
            (iar as ServiceClientAsyncResult).Dispose();
        }
    }

    static void AsyncMain(string[] args)
    {
        var anyErrors = false;
        var socket = new TcpListener(IPAddress.Loopback, 10003);
        var requestCount = 0;
        socket.Start();
        var t = new Thread(new ThreadStart(() =>
        {
            Exception e = null;
            while (true)
            {
                var client = socket.AcceptTcpClient();
                Action handler = () =>
                {
                    client.Close();
                    if (!anyErrors)
                    {
                        anyErrors = true;
                        Console.WriteLine("server ERROR");
                    }
                };
                requestCount = requestCount + 1;
                if (requestCount % 100 == 0)
                {
                    Console.WriteLine("{0} accepted...", requestCount);
                }
                try
                {
                    BeginServiceClient(client, (iar) =>
                    {
                        try
                        {
                            EndServiceClient(iar);
                        }
                        catch (Exception ex)
                        {
                            handler();
                            e = ex;
                        }
                    }, null);
                }
                catch (Exception)
                {
                    handler();
                    throw;
                }
                if (e != null)
                    throw e;
            }
        }));
        t.IsBackground = true;
        t.Start();
        while (true)
        {
            Thread.Sleep(1000);
            var count = Interlocked.Exchange(ref numWritten, 0);
            Console.WriteLine("QUOTES PER SECOND: {0}", count);
        }
    }

    static void Main(string[] args)
    {
        quote = new byte[512];
        for (int i = 0; i < quote.Length; i++)
            quote[i] = 1;
        AsyncMain(args);
    }

}
