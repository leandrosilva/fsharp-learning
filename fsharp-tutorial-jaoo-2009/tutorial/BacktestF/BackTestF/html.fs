module StockTicker.Html

open System
open System.IO
open System.Text.RegularExpressions

//type Cell = string
//type Header = seq<Cell>
//type Row = seq<Cell>
//type Table = seq<Row>

// It assumes no table inside table ...
let tableExpr = "<table[^>]*>(.*?)</table>"
let headerExpr = "<th[^>]*>(.*?)</th>"
let rowExpr = "<tr[^>]*>(.*?)</tr>"
let colExpr = "<td[^>]*>(.*?)</td>"
let regexOptions = RegexOptions.Multiline ||| RegexOptions.Singleline ||| RegexOptions.IgnoreCase

let scrapHtmlTables html =
    let parseRow (rowMatch: Match) =
        let matches = Regex.Matches(rowMatch.Value, colExpr, regexOptions)
        seq { for x in matches -> x.Groups.Item(1).ToString() }
    let parseTable (tableMatch: Match) =
        let header = if tableMatch.Value.Contains("<th")
                     then
                        let matches = Regex.Matches(tableMatch.Value, headerExpr, regexOptions)
                        Some(seq { for x in matches -> x.Groups.Item(1).Value.ToString()})
                     else None
        let rows =
            let matches = Regex.Matches(tableMatch.Value, rowExpr, regexOptions)
            seq { for x in matches -> parseRow x}
        header, rows
    let tablesMatches = Regex.Matches(html,tableExpr, regexOptions)
    seq { for x in tablesMatches -> parseTable x }         

let scrapHtmlCells html =
    seq { for x in Regex.Matches(html, colExpr, regexOptions) -> x.Groups.Item(1).ToString()}            

let scrapHtmlRows html =
    seq { for x in Regex.Matches(html, rowExpr, regexOptions) -> scrapHtmlCells x.Value }
            //seq { for y in Regex.Matches(x.Value, colExpr, regexOptions) -> y.Groups.Item(1).ToString()} }
