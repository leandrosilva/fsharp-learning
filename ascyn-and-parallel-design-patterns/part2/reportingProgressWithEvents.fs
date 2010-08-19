#light

open System
open System.Threading

(* Helper *)
type SynchronizationContext with
    // A standard helper extension method to raise an event on the GUI thread
    member syncContext.RaiseEvent (event: Event<_>) args =
        syncContext.Post((fun _ -> event.Trigger args), state = null)

    // A standard helper extension method to capture the current synchronization context.
    // If none is present, use a context that executes work in the thread pool.
    static member CaptureCurrent () =
        match SynchronizationContext.Current with
        | null -> new SynchronizationContext()
        | ctxt -> ctxt

(* Worker *)
type AsyncWorker<'T>(jobs: seq<Async<'T>>) =
 
    // This declares an F# event that we can raise
    let jobCompleted = new Event<int * 'T>()
 
    // Start an instance of the work
    member x.Start() =
        // Capture the synchronization context to allow us to raise events back on the GUI thread
        let syncContext = SynchronizationContext.CaptureCurrent()
 
        // Mark up the jobs with numbers
        let jobs = jobs |> Seq.mapi (fun i job -> (job, i + 1))
 
        let work = 
            Async.Parallel
               [ for (job, jobNumber) in jobs ->
                   async { let! result = job
                           syncContext.RaiseEvent jobCompleted (jobNumber, result)
                           return result } ]
 
        Async.StartImmediate(work |> Async.Ignore)
 
    // Raised when a particular job completes
    member x.JobCompleted = jobCompleted.Publish

(* Fibonacci job *)
[<EntryPoint>] 
let main (args : string[]) =
    printfn "Reporting progress with events"

    let rec fib i =
        if i < 2 then 1 else fib (i-1) + fib (i-2)
  
    let worker =
        new AsyncWorker<_>([ for i in 1 .. 100 -> async { return fib (i % 40) } ])

    worker.JobCompleted.Add(
        fun (jobNumber, result) ->
            printfn "job %d completed with result %A" jobNumber result)

    worker.Start()

    printfn "\n(type any key to terminate)\n"
    let keyPressed = Console.ReadKey(true)

    (0)