module State

open Fable.Core.JsInterop
open Fable.Import.Browser
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types
open Voting.Shared
open Type
open Elmish

let init () = {PageStatus = Input; Vote = initialModel}, Cmd.none

let post vote =
    promise {

        let body = toJson vote
        console.log body
        let props =
            [ RequestProperties.Method HttpMethod.POST
              Fetch.requestHeaders [
                  HttpRequestHeaders.ContentType "application/json" ]
              RequestProperties.Body !^body]

        try
            return! Fetch.fetchAs<VoteResult> "/api/Vote" props

        with _ ->
            return! failwithf "Could not process voting."
    }


let private alterVote vote index name voteItem =
    if voteItem.Rating = vote then
        let alterPeople name i originalName=
            if i = index then
                name
            else
                originalName
        {voteItem with People = voteItem.People |> List.mapi (alterPeople name)}
    else
        voteItem


let update msg model =
    match msg with
    | SetSender sender ->{ model with Vote = { model.Vote with Sender = sender}}, Cmd.none
    | SetVote (vote, index, name) ->

        let vote =
            {model.Vote with Votes = model.Vote.Votes |> List.map (alterVote vote index name)}

        { model with Vote = vote }, Cmd.none

    | Submit -> model, Cmd.ofPromise (post) model.Vote Success Error
    | Reset -> init()
    | Success (VoteResult.Ok _) -> { model with PageStatus = Ok}, Cmd.none
    | Success (VoteResult.Error e) -> { model with PageStatus = VoteError e}, Cmd.none
    | Error _ -> model , Cmd.none

