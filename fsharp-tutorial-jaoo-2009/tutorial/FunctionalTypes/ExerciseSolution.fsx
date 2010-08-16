//---------------------------------------------------------------------------
// This is a demonstration script showing the solution to an exercise to value
// blackjack hands, initially assuming aces are worth 11.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 

#load @"..\show.fs"

// Simple Union Type
type Suit = 
    | Heart 
    | Diamond 
    | Club 
    | Spade

// Union type holding data
type Rank =
    | Ace
    | King
    | Queen
    | Jack
    | Value of int
    
type Card = Card of Rank * Suit

// Generate all the cards in the deck
let fullDeck =
    [
        for suit in [ Heart; Diamond; Club; Spade] do
            for v in 2 .. 10 do
                yield Card(Value v, suit)
            for face in [Jack; Queen; King; Ace] do
                yield Card(face, suit)
    ]

fullDeck |> show



let cardValue (Card (rank, suit)) = 
    match rank with 
    | Ace                 -> 11
    | King | Queen | Jack -> 10
    | Value v             -> v
    

(*
let typeTest (inp : obj) = 
    match inp with 
    | :? string as s     -> sprintf "I'm a string with length %d" s.Length    
    | :? int             -> "I'm an integer"    
    | :? System.DateTime -> "I'm a DateTime"    
    | _ -> "I don't know what I am!"    
    
typeTest "Hello World"    
typeTest 4
typeTest System.DateTime.Now
*)    
    
cardValue (Card (Ace, Heart)) = 11

fullDeck |> List.map cardValue |> show

// -----------------------------------------------

// The possible results of the game
type Result = 
    | Busted // > 21
    | Hand of int // < 21
    | Blackjack // 21

// Compute the total of a hand
let judgeHand cards =

    let handTotal = 
       cards |> List.map cardValue |> List.sum

    match handTotal with
    | n when n > 21 -> Busted
    | 21 -> Blackjack
    | n -> Hand n


judgeHand [ Card(Ace,Diamond) ] |> show
judgeHand [ Card(Ace,Diamond); Card(Ace,Spade)  ] |> show



//--------------------------------------

module AcesOneOrEleven = 

    // Evaluate hand for all possible 1/11 value assignments for aces
    let possibleCardValues card =
        [ match card with 
          | Card(Ace,_) ->
            yield 1  
            yield 11              
          | card ->
            yield cardValue card ]

    let rec possibleHandTotals cards =
        [ match cards with 
          | card :: rest ->
            for v1 in possibleCardValues card do
              for v2 in possibleHandTotals rest do
                yield v1+v2  
          | [] -> 
            yield 0 ]

    let judgeValue v =
        match v with
        | n when n > 21 -> Busted
        | 21 -> Blackjack
        | n -> Hand n

    let judgeHand2 cards =
        cards 
           |> possibleHandTotals  
           |> List.map judgeValue 
           |> List.sort
           |> List.rev
           |> List.head

    let ace = Card(Ace,Heart)

    judgeHand2 [ ace;ace;ace;ace ]        
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
open System.IO
open System.Collections.Generic

let countLines (filePath : string) =

    let lineCounts = new Dictionary<string, int>()

    use file = new StreamReader(filePath)

    while not file.EndOfStream do

        let line = file.ReadLine()

        if lineCounts.ContainsKey(line) then
            lineCounts.[line] <- lineCounts.[line] + 1
        else
            lineCounts.[line] <- 1
        
    // return the dictionary of line counts
    lineCounts
    
countLines @"c:\Windows\DirectX.log"    

