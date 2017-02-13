namespace Coremero.Plugin.FSharpExample

open Coremero
open Coremero.Commands

type FsharpPlugin() =
    interface IPlugin
    [<Command("fsharp")>]
    member this.Go () =
        "This functionally sucks."