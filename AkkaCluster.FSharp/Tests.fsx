#load @"C:\temp\nuget\.paket\load\netcoreapp3.1\main.group.fsx"
#r @"C:\temp\nuget\packages\ColorConsole\lib\net40\ColorConsole.dll"
#load "Library.fs"
open FParsec
open AkkaCluster.FSharp

open Parser

run nodeAction "set 1 1"
run nodesAction "from node 1 start"

run (setParser <|> getParser) "from node 1 get 1"
open CharParsers

let getParser : Parser<Nodes ->unit, unit> = 
    (pstringCI "from node " >>. nodeParser) .>>. (pstringCI " get " >>. entityIdParser)
    |>> (fun (node, entity) -> exec node (fun n -> 
                                            let value = n.Get(entity)
                                            Console.print ConsoleColor.DarkYellow (sprintf "Response: %s" value )))
