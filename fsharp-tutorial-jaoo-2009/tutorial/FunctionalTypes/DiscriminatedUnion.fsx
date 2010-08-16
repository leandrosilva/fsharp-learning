//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of union types in F#.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


// Simple DU
type Suit = 
    | Heart 
    | Diamond 
    | Club 
    | Spade

// DU holding data
type Rank =
    | Ace
    | King
    | Queen
    | Jack
    | Value of int
    
type Card = Card of Rank * Suit

let FullDeck() =
    [
        for suit in [ Heart; Diamond; Club; Spade] do
            for value in 2 .. 10 do
                yield Card(Value(value), suit)
            for face in [Jack; Queen; King; Ace] do
                yield Card(face, suit)
    ]

FullDeck()
    
    
    
    
    
    
    
    
    
    
    
// Recursive DU
type BinaryTreeNode<'a> =
    | Node     of 'a * BinaryTreeNode<'a> * BinaryTreeNode<'a>
    | LeafNode of 'a
    
(*
      4
  2       5
1   3   6   7
*)
let tree = 
    Node(
        4,
        Node(
            2,
            LeafNode(1),
            LeafNode(3)
        ),
        Node(
            5,
            LeafNode(6),
            LeafNode(7)
        )
    )
                
// Pattern matching
// - Call out type inference on c based on match rules
// - Call out warning if c does not cover all possibilities
let cardValue (Card(r,s)) =
    match r with
    | Ace                 -> 11
    | King | Queen | Jack -> 10
    | Value(x)            -> x

let cardValueDict =
    dict [ for card in FullDeck() do
                yield card, cardValue(card) ]

cardValueDict.[Card(Queen, Heart)]

let handTotal = [Card(Ace, Spade); Card(Value(2), Club)]
                |> Seq.map (fun c -> cardValueDict.[c])
                |> Seq.sum

// - Call out matching against constant    
// - Call out matching structure of data
// - Call out capturing value
let DescribeHand cards =
    match cards with
    | [ ] -> "No cards in that hand."
    
    | [ Card(r,_) ] -> sprintf "High card = %A" r
    
    | [ Card(Ace,_);  Card(Ace,_)  ] -> "Bullets"
    | [ Card(King,_); Card(King,_) ] -> "Cowboys"
    
    | [ Card(Value(x),_); Card(Value(y), _); Card(Value(z),_) ]
        when x = y && y = z
        -> "Three of a kind"
        
    | _ -> "No idea"
