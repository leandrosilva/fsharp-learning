#light
 
open System
open NUnit.Framework
 
let rec score_bowls bowls =
    let rec score_bowls' frame l  =
        match l with
        | _ when frame = 10 -> (List.foldBack (fun x y -> x + y) l 0)
        | [] -> 0
        | [f] -> f
        | [f;s] -> f + s
        | f :: s :: n :: tail when f = 10       -> 10 + s + n + score_bowls' (frame+1) ( s :: n :: tail )
        | f :: s :: n :: tail when (f + s) = 10 -> 10 + n + score_bowls' (frame+1) (n :: tail)
        | f :: s :: n :: tail                   -> f + s + score_bowls' (frame+1) (n :: tail)
    score_bowls' 1 bowls
 
[<TestFixture>]
type BowlingTestCases = class
   new() = {}  
   [<Test>]
   member x.SimpleScoring() = Assert.AreEqual(6, score_bowls [1;2;3] )
   [<Test>]
   member x.ScoreSpare() = Assert.AreEqual(12, score_bowls [2;8;1] )
   [<Test>]
   member x.ScoreStrike() = Assert.AreEqual(16, score_bowls [10;1;2] )
   [<Test>]
   member x.ScorePerfectGame() = Assert.AreEqual( 300, score_bowls [for i in 1..12 -> 10] )
   [<Test>]
   member x.SpareLastFrame() = Assert.AreEqual( 11, score_bowls ([for i in 1..18 -> 0] @ [2;8;1]) )
   [<Test>]
   member x.StrikeLastFrame() = Assert.AreEqual( 21, score_bowls ([for i in 1..18 -> 0] @ [10;10;1]) )
end