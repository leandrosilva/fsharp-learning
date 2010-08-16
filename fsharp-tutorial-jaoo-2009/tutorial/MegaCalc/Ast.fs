module Ast

open System

type Factor =
    | Float   of Double
    | Integer of Int32
    | ParenEx of Expr

and Term =
    | Times  of Term * Factor
    | Divide of Term * Factor
    | Factor of Factor

and Expr =
    | Plus  of Expr * Term
    | Minus of Expr * Term
    | Term  of Term
    
and Equation =
    | Equation of Expr