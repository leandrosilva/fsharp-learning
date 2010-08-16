//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of using F# 
// meta-programming using quotations.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns

type Error = Err of float

let rec errorEstimateAux t (env : Map<Var,_>) =

    match t with
    | SpecificCall <@ (+) @> (None,tyargs,[xt;yt]) ->
        let x,Err(xerr) = errorEstimateAux xt env
        let y,Err(yerr) = errorEstimateAux yt env
        (x+y,Err(xerr+yerr))

    | SpecificCall <@ (-) @> (None,tyargs,[xt;yt]) ->
        let x,Err(xerr) = errorEstimateAux xt env
        let y,Err(yerr) = errorEstimateAux yt env
        (x-y,Err(xerr+yerr))

    | SpecificCall <@ ( * ) @> (None,tyargs,[xt;yt]) ->
        let x,Err(xerr) = errorEstimateAux xt env
        let y,Err(yerr) = errorEstimateAux yt env
        (x*y,Err(xerr*abs(x)+yerr*abs(y)+xerr*yerr))

    | SpecificCall <@ ( / ) @> (None,tyargs,[xt;yt]) ->
        let x,Err(xerr) = errorEstimateAux xt env
        let y,Err(yerr) = errorEstimateAux yt env
        (x/y,Err(xerr*abs(x)+abs(1.0/y)/yerr+xerr/yerr))

    | SpecificCall <@ abs @> (None,tyargs,[xt]) ->
        let x,Err(xerr) = errorEstimateAux xt env
        (abs(x),Err(xerr))

    | Let(var,vet, bodyt) ->
        let varv,verr = errorEstimateAux vet env
        errorEstimateAux bodyt (env.Add(var,(varv,verr)))

    | Call(None,MethodWithReflectedDefinition(Lambda(v,body)),[arg]) ->
        errorEstimateAux  (Expr.Let(v,arg,body)) env

    | Var(x) -> env.[x]

    | Double(n) -> (n,Err(0.0))

    | _ -> failwithf "unrecognized term: %A" t

let rec errorEstimateRaw (t : Expr) =
    match t with
    | Lambda(x,t) ->
        (fun xv -> errorEstimateAux t (Map.ofSeq [(x,xv)]))
    | PropertyGet(None,PropertyGetterWithReflectedDefinition(body),[]) ->
        errorEstimateRaw body
    | _ -> failwithf "unrecognized term: %A - expected a lambda" t

let errorEstimate (t : Expr<float -> float>) = errorEstimateRaw (t :> Expr)

let test1 = errorEstimate <@ (fun x -> x) @> (3.0,Err 0.5)

let test2 = errorEstimate <@ (fun x -> x  + x) @> (3.0,Err 0.5)

