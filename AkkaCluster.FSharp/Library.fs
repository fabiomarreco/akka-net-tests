namespace AkkaCluster.FSharp
open FParsec
open FParsec.CharParsers
open System
open ColorConsole


type INode = 
    abstract member Start : unit -> unit
    abstract member Stop : unit -> unit
    abstract member Set : int * string -> unit
    abstract member Get : int -> string
    //abstract member Schedule : int * TimeSpan -> unit
    abstract member Ping : int -> unit;
    abstract member Broadcast : unit -> unit;

type Nodes = System.Collections.Generic.IDictionary<int, INode>

module Console = 
    let console =  ConsoleWriter()
    let print color msg = console.WriteLine (msg, color)

module Parser = 
    let (?>>) (parser:Parser<'a, unit>) (fn : 'a -> INode -> unit) = parser |>> (fun a ->  fn a)
    let pEntity = pint32 
    let pStart  = pstringCI "start" ?>> (fun _ node -> node.Start())
    let pStop = pstringCI "stop" ?>> (fun _ node -> node.Stop())
    let pSet = 
         (pstringCI "set" .>> spaces) >>. pEntity .>> spaces .>>. restOfLine false
         ?>> (fun (entity, value) node -> node.Set(entity, value))

    let pGet =
         (pstringCI "get" .>> spaces) >>. pEntity
         ?>> (fun entity node -> 
                let value = node.Get(entity)
                Console.print ConsoleColor.DarkYellow (sprintf "Response: %s" value ))

    let pPing  = (pstringCI "ping" >>. pEntity) ?>> (fun entity node -> node.Ping(entity))
    let pBroadCast = pstringCI "broadcast" ?>> (fun _ node -> node.Broadcast());

    let pSeconds : Parser<TimeSpan, unit> = (pint32 .>> pstring "s")  |>> (fun s-> System.TimeSpan.FromSeconds(float s))
    (*
    let pSchedule : Parser<INode -> unit, unit> = (pstringCI "schedule " >>. pEntity) .>>. (pstringCI "for " >>. pSeconds)
                                                  ?>> (fun (entity, secs) node -> node.Schedule(entity, secs))
    *)
    let pAction = pStart <|> pStop <|> pGet  <|> pSet  <|> pPing <|> pBroadCast
    let pNode = (((pstringCI "from node" <|> pstringCI "@ node" <|> pstringCI "node") .>> spaces)<|> pstring "@") >>. pint32
    let pNodeAction = (pNode .>> spaces) .>>. pAction
                      |>> (fun (node, fn) -> fun (nodes:Nodes) -> nodes.[node]|> fn)
        


type NodeCommandParser() = 
      member __.Run(nodes : Nodes , str : string) = 
        match run Parser.pNodeAction str with
        | Success (fn, _, _) -> (fn nodes)
        | Failure (str, err, _) -> Console.print ConsoleColor.Red (sprintf "Error: %A, %A" str err)
