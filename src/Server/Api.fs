module Api


open Giraffe
open Microsoft.AspNetCore.Http
open Saturn
open Voting.Shared



let vote next (ctx : HttpContext)= task {
    let! req = ctx.BindModelAsync<Vote>()
    let! res = Actor.handleVote req |> Async.StartImmediateAsTask
    return! json res next ctx }

let apiRouter = scope {
    post "/Vote" vote
    }
