//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of agent programming 
// in F#.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 

#time "off"

type Agent<'T> = MailboxProcessor<'T>


let printingAgent = 
       Agent.Start(fun inbox ->
         async { while true do 
                   let! msg = inbox.Receive()
                   if msg = "100000" then 
                      printfn "got message %s" msg } )


printingAgent.Post "three"
printingAgent.Post "four"

for i in 0 .. 100000 do 
    printingAgent.Post (string i)

let agents = 
   [ for i in 0 .. 100000 ->
       Agent.Start(fun inbox ->
         async { while true do 
                   let! msg = inbox.Receive()
                   printfn "%d got message %s" i msg })]

for agent in agents do
    agent.Post "Hello"

/// This is a mailbox processing agent that accepts integer messages
let countingAgent = 
    Agent.Start (fun inbox ->
        let rec loop(state) =         
            async { printfn "Agent, current state = %d" state
                    let! msg = inbox.Receive()
                    
                    printfn "Agent, current state = %d" state
                    return! loop(state+msg) } 

        loop(0)
    )

countingAgent.Post 3
countingAgent.Post 4

for i = 0 to 100 do
    countingAgent.Post i



