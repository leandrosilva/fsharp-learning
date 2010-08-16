

open System
open Microsoft.FSharp.Text.Lexing

open Ast
open Lexer
open Parser

/// Evaluate a factor
let rec evalFactor factor =
    match factor with
    | Float x   -> x
    | Integer x -> float x
    | ParenEx x -> evalExpr x

/// Evaluate a term
and evalTerm term =
    match term with
    | Times (term, fact)  -> (evalTerm term) * (evalFactor fact)
    | Divide (term, fact) -> (evalTerm term) / (evalFactor fact)
    | Factor fact         -> evalFactor fact

/// Evaluate an expression
and evalExpr expr =
    match expr with
    | Plus (expr, term)  -> (evalExpr expr) + (evalTerm term)
    | Minus (expr, term) -> (evalExpr expr) - (evalTerm term)
    | Term term          -> evalTerm term

/// Evaluate an equation
and evalEquation eq =
    match eq with
    | Equation expr -> evalExpr expr



[<EntryPoint>]
let main (args : string array) = 
    
    printfn "MegaCalc"

    let mutable input = ""
    while input <> "quit" do
       
        try
            
            printf ":"
            input <- Console.ReadLine()
            
            printfn "Lexing [%s]" input
            let lexbuff = LexBuffer<_>.FromString input
            
            printfn "Parsing..."
            let equation = Parser.start Lexer.tokenize lexbuff
            
            printfn "Evaluating Equation..."
            let result = evalEquation equation
            
            printfn "Result: %s" (result.ToString())
            
        with ex ->
            printfn "Unhandled Exception: %s" ex.Message
        
        // Print blank line
        printfn ""
    done

    Console.WriteLine("(press any key)")
    Console.ReadKey(true) |> ignore
    
    // Exit code
    0
