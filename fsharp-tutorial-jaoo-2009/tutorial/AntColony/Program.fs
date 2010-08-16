#if INTERACTIVE
#r "System.Windows.Forms.dll"
#r "System.Drawing.dll"
#endif

open System
open System.Drawing
open System.Windows.Forms
open System.Collections.Generic

//Program constants
let antHome = 8;; //X by X in Size
let maxFoodPerSquare = 1000
let chanceFoodPerSquare = 80

let antMaxFoodCarried = 50
let antPheromoneDrop = 200
let antPheromoneDropNoFood = 0
let tickLength = 50

let antAdventurousness = 3 //Lower is more adventurous
let antWanderlust = 4 //Lower is lustier
 
let territoryHeight = 80
let territoryWidth = 80

let pixelWidth = 6
let pixelHeight = 6
let colonyTiles = 5

let _RandomGen = new System.Random()

/// represents a single node in the ants territory
type TerritoryNode =
    { food: int;
      pheromone: int;
      isHome: bool
      hasAnt: bool }
    /// creates a new node with the default values
    static member NewNode home =
        { food = 0;
          pheromone = 0;
          isHome = home;
          hasAnt = home }

    /// creates a new node with food
    static member NewNodeFood f home =
        { food = f;
          pheromone = 0;
          isHome = home;
          hasAnt = home; }

    /// test if the node has food
    member n.HasFood = n.food > 0
    
    /// update where the pheromone value
    member n.UpdatePheromone food =
        if food then
          { n with pheromone = n.pheromone + antPheromoneDrop }
        else
          { n with pheromone = n.pheromone + antPheromoneDropNoFood }

    /// an ant arrives in the node
    member n.AntArrive () =
        { n with hasAnt = true }
        
    /// an ant leaves the node
    member n.AntLeave () =
        { n with hasAnt = false }

/// represents an ant that moves within the territory                     

type Ant =
    { FoodCarried: int
      LocationX: int
      LocationY: int
      History: list<int*int>
      Homeward: bool }

    /// creates an new instance of an ant with the default values
    static member newAnt x y =
      {LocationX = x; LocationY = y; FoodCarried = 0; History = []; Homeward = false; }

     

/// The environment - represents both the ants and the nodes they move within
type Env =
    { Ants: seq<Ant>;
      territory: Map<int*int, TerritoryNode> }
    /// initalize the environment with the default values
    static member initalize() =     
        //create a list of points to represent the grid
        let points =
            seq { for i in [0 .. territoryWidth - 1] do
                    for j in [0 .. territoryHeight - 1] do
                      yield i,j }    

        // randomly creat a node
        let createNode i j =
           let randomFoodDropValue = _RandomGen.Next( 0, chanceFoodPerSquare ) in
           let home = i <= antHome && j <= antHome
           if randomFoodDropValue = 5 then
                TerritoryNode.NewNodeFood ( _RandomGen.Next( 0, maxFoodPerSquare ) ) home
           else
                TerritoryNode.NewNode home
       
        // create a map of points to node values
        let teritory = Seq.fold (fun acc (i,j) -> Map.add (i,j) (createNode i j) acc) Map.empty points

        // create the list of ants
        let antList =
            seq { for i in [0 .. antHome] do
                      for j in [0 .. antHome] do
                        yield Ant.newAnt i j  }                 
        // return the environment
        { Ants = antList;
          territory = teritory }
   
    /// replaces a node in the teritory with a new one
    member x.replaceNode loc NewNode =
        if x.territory.ContainsKey(loc) then
            let newTerr = x.territory.Remove(loc)
            { x with  territory = newTerr.Add(loc, NewNode) }
        else
            { x with  territory = x.territory.Add(loc, NewNode) }

/// the direction that the ants travel in   
type Direction = North | East | South | West

/// make an ant drop food some food
let TNDropFood node ant =
    if (node.food = maxFoodPerSquare) then
        false, node, ant
    else
        let space = maxFoodPerSquare - node.food in
        if (space <= ant.FoodCarried) then
            true, { node with food = node.food + space },  { ant with FoodCarried = ant.FoodCarried - space}
        else
            true, { node with food = node.food + ant.FoodCarried }, { ant with FoodCarried = 0 }

/// remove the appropreate amount of food from a node
let TNGetFood (node:TerritoryNode) f =
    if node.HasFood then
        let food = node.food in
        if food >= f then
            { node with food = food - f },
            f                            
        else
            let retFood = node.food in
            { node with food = 0 },
            retFood
    else node, 0

/// test that a node is within the grid
let GTIsNodeReal i j =
    0 < i && i < territoryWidth &&
    0 < j && j < territoryHeight

/// attempt to move ant to a new node and return false if we can't                     
let ANTGoToNode env ant i j =
    if GTIsNodeReal i j  then
        let node = env.territory.[(i,j)]
        if not node.hasAnt then
            let oldNode = env.territory.[(ant.LocationX,ant.LocationY)]
            let env = env.replaceNode ((ant.LocationX,ant.LocationY))  (oldNode.AntLeave())
            let env = env.replaceNode ((i,j)) (node.AntArrive().UpdatePheromone(ant.FoodCarried > 0))
            true, env, { ant with LocationX = i; LocationY = j; History = (ant.LocationX,ant.LocationY) :: ant.History }
        else
            false, env, ant
    else
        false, env, ant

/// send an ant off in a given direction, trying the opersite direction if we can't
let ANTGoInDirection (env:Env) ant d =
    let x1,y1, x2, y2 =
        match d with
        | North -> ant.LocationX, ant.LocationY - 1, ant.LocationX, ant.LocationY + 1
        | South -> ant.LocationX, ant.LocationY + 1, ant.LocationX, ant.LocationY - 1
        | East -> ant.LocationX - 1, ant.LocationY, ant.LocationX + 1, ant.LocationY
        | West -> ant.LocationX + 1, ant.LocationY, ant.LocationX - 1, ant.LocationY

    let moved,env,ant = ANTGoToNode env ant x1 y1
    if not moved then
        ANTGoToNode env ant x2 y2
    else true, env, ant

/// move an ant in a random direction
let ANTMoveRandomly (env:Env) ant =
    let dir =
       match _RandomGen.Next( 0, 4 ) with
       | 0 -> North | 1 -> East | 2 -> South | 3 -> West | _ -> failwith "invalid"
    ANTGoInDirection env ant dir

/// make the ant hunt according to the pheromone surrounding it   
let ANTHuntByPheromone (env:Env) ant =
    let rnd = _RandomGen.Next( 0, antAdventurousness ) in
    let env, ant =
        if ( rnd = 0 ) then
            let _,env,ant = ANTMoveRandomly env ant
            env, ant
        else env, ant                                    

    let southPheromone =
        if GTIsNodeReal ant.LocationX (ant.LocationY + 1) then
          env.territory.[(ant.LocationX,ant.LocationY + 1)].pheromone
        else 0

    let westPheromone =
      if GTIsNodeReal (ant.LocationX + 1) ant.LocationY then
         env.territory.[(ant.LocationX + 1,ant.LocationY)].pheromone
      else 0

    if (southPheromone = westPheromone ) then
        ANTMoveRandomly env ant
    else
        if (southPheromone > westPheromone ) then
          ANTGoInDirection env ant South
        else
          ANTGoInDirection env ant West

/// main encoding of ants behavior
let ANTBehave (env:Env) (ant: Ant) =
    let x, y = ant.LocationX, ant.LocationY
    let curNode = env.territory.[(x,y)] in
    if ant.FoodCarried > 0 then
        if curNode.isHome then
            let isDropped, curNode, ant = TNDropFood curNode ant in
            let env = env.replaceNode ((x, y)) curNode
            let _, env, ant =
                if not isDropped then
                  ANTMoveRandomly env ant
                else
                  false, env, ant
            env, ant
        else
            let rnd = _RandomGen.Next( 0, antWanderlust ) in
            let _,env,ant =
                if rnd = 0 then
                    ANTGoInDirection env ant North
                else if rnd = 1 then
                    ANTGoInDirection env ant East
                else
                    if ant.LocationX > ant.LocationY then
                         ANTGoInDirection  env ant East
                    else
                         ANTGoInDirection env ant North
            env, ant
    else
        let _, env, ant =
            if curNode.HasFood then
                if curNode.isHome then
                    ANTMoveRandomly env ant
                else           
                    let tillMaxFood = antMaxFoodCarried - ant.FoodCarried in
                    let curNode,foodGot = (TNGetFood curNode tillMaxFood) in
                    true,
                    env.replaceNode ((x,y)) curNode,
                    { ant with FoodCarried = ant.FoodCarried + foodGot }
            else
                ANTHuntByPheromone env ant
        env, ant

/// loop though all the ants trying to move them
let ANTLoopBehave env =
    let doAnts (env, acc) ant =
        let envNew, antNew= ANTBehave env ant
        envNew, antNew:: acc
        
    let env, newAnts = Seq.fold (fun env ant -> doAnts env ant) (env, []) env.Ants
    { env with Ants = Seq.of_list newAnts }

// start of the worker mailbox statemachine ala game of life sample
type msg =
    | Run
    | Exit
    | Pause
    | Step

/// A worker automaton is a reactive automaton running on a dedicated thread of its
/// own.
type Worker() =
    // Capture the synchronization context of the thread that creates this object. This
    // allows us to send messages back to the GUI thread painlessly.
    let callerCtxt =
        match System.Threading.SynchronizationContext.Current with
        | null -> null // System.ComponentModel.AsyncOperationManager.SynchronizationContext
        | x -> x

    //do if callerCtxt = null then failwith "Couldn't detect the synchronization context of the calling thread"
    let runInGuiCtxt f =
        match callerCtxt with
        | null ->
            // callerCtxt is null on Mono. This is a bug. System.Threading.SynchronizationContext.Current doesn't return a useful
            // result. This is a little unfortunate. System.ComponentModel.AsyncOperationManager.SynchronizationContext returns
            // an inconsistent result.
            //
            // So here we works around, where we finds the open form and sends to it.
            if System.Windows.Forms.Application.OpenForms.Count > 0 then
                System.Windows.Forms.Application.OpenForms.Item(0).BeginInvoke(new System.Windows.Forms.MethodInvoker(fun _ -> f())) |> ignore
        | _ -> callerCtxt.Post((fun _ -> f()),null)

    // This events are fired in the synchronization context of the GUI (i.e. the thread
    // that created this object)
    let fireUpdates,onUpdates = Event.create()

    // Updates are generated very, very quickly. So we keep a queue, and every time we push
    // into an empty queue we trigger an event in the GUI context that empties the queue and invokes
    // the update event in the GUI context.
    let updateQueue = new Queue<_>()
    let enqueueUpdates(update) =
        let first =
            lock (updateQueue) (fun () ->
                let first = (updateQueue.Count = 0)
                updateQueue.Enqueue(update);
                first)
        if first then
            runInGuiCtxt(fun _ ->
                let updates =
                    lock (updateQueue) (fun () -> 
                        [ while updateQueue.Count > 0 do
                             yield updateQueue.Dequeue() ])
                fireUpdates(updates))

    /// Compute one step of the game and call the
    /// NotifyUpdates callback.  That is, this function provides
    /// glue between the core computation and the computation of that algorithm
    let oneStep(env) =
        let env = ANTLoopBehave env
        let current = Seq.map (fun ant -> ant.LocationX,ant.LocationY) env.Ants
        let old = env.Ants |> Seq.map (fun ant -> ant.History) |> Seq.concat
        let foodAnt =
            env.territory 
            |> Map.to_list
            |> Seq.of_list
            |> Seq.map (fun (loc, node) -> loc, node.food)
            |> Seq.filter (fun (_, food) -> food > 0)

        runInGuiCtxt(fun _ -> fireUpdates([ old, current, foodAnt]))

        // clean out History
        let ants = env.Ants |> Seq.map (fun ant ->  {ant with History = [] } ) 
        { env with Ants = ants }  

    // The control logic is written using the 'async' non-blocking style. We could also write this
    // using a set of synchronous recursive functions, or using a synchronous workflow,
    // but it's more fun to use the asynchronous version, partly because the MailboxProcessor type gives us a
    // free message queue.
    //
    // Wherever you see 'return!' in the code below you should interpret
    /// that as 'go to the specified state'.
    let mailboxProcessor =
        new MailboxProcessor<msg>(fun inbox ->
            /// This is the States of the worker's automata using a set of
            /// tail-calling recursive functions.
            let rec Running(s) =
                async { let! msgOption = inbox.TryReceive(timeout=0)
                        match msgOption with
                        | None -> return! StepThen (SleepThen Running) s
                        | Some(msg) ->
                            match msg with
                            | Pause          -> return! Paused s
                            | Step           -> return! Running s
                            | Run            -> return! Running s
                            | Exit           -> return! Finish s }

            and StepThen f s = 
                async { let s = oneStep(s)
                        return! f s }
            and SleepThen f s =
                async { // yield to give the GUI time to update
                        do! System.Threading.Thread.AsyncSleep(20);
                        // Requeue in thread pool - strictly speaking we dont have to
                        // do this, but it ensures we reclaim stack on Mono and other
                        // platforms that do not take tailcalls on all architectures.
                        do! Async.SwitchToThreadPool()
                        return! f(s)  }
            and Paused(s) =
                async { let! msg = inbox.Receive()
                        match msg with
                        | Pause          -> return! Paused s
                        | Step           -> return! StepThen Paused s
                        | Run            -> return! Running s
                        | Exit           -> return! Finish s  }

            and Finish(s) =
                async { return () }

            // Enter the initial state
            Running (Env.initalize()))

    /// Here is the public API to the worker
    member w.RunAsync () = mailboxProcessor.Post(Run)
    member w.StopAsync() = mailboxProcessor.Post(Pause)
    member w.ExitAsync() = mailboxProcessor.Post(Exit)
    member w.StepAsync() = mailboxProcessor.Post(Step)
    member w.Updates       = onUpdates
    member w.Start()  = mailboxProcessor.Start()

/// make and show the form
let main() =
    let form = new Form(Visible=true,Text="Ant Colony",Width=800, Height=600)
    form.Visible <- true
    for x in [ 0 .. 1 ] do
        for y in [ 0 .. 1 ] do
            let bitmap = new Bitmap(territoryWidth, territoryHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            let pb = new PictureBox(SizeMode=PictureBoxSizeMode.Zoom)
            pb.Size <- new Size(300, 300)
            pb.Location <- new Point( x * 310, y * 310)
            pb.Image <- bitmap
            form.Controls.Add( pb )
            let worker = new Worker()
            worker.Updates.Add(fun updates ->
                for  (old, current, foodAnt) in updates do
                    old |> Seq.iter (fun (x,y) -> bitmap.SetPixel(x,y, Color.Black) )
                    current |> Seq.iter (fun (x,y) -> bitmap.SetPixel(x,y, Color.Red))
                    foodAnt |> Seq.iter (fun ((x,y), food) ->
                        let c = Color.FromArgb( food / maxFoodPerSquare * 255, Color.Green)
                        bitmap.SetPixel(x,y, c))
                    pb.Invalidate() )
            worker.Start()
    form.Activate()
    Application.Run(form)

[<STAThread>]
do main()

