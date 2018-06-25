module View

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types
open Voting.Shared
open Fable.Helpers
open Type


let sender text dispatch =
    div [Class "sender"] [
        label [Class "sender-label"] [str "Sender"]
        input [
            Id <| "sender"
            Class "sender-textbox"
            HTMLAttr.Type "password"
            Value text
            Required true
            OnChange (fun ev -> dispatch (SetSender !!ev.target?value))
        ]

    ]
let ratedTextBox rate text (index:int) dispatch =
    input [
        Id <| sprintf "rate_%d" rate
        HTMLAttr.Type "text"
        HTMLAttr.Custom ("data-rate",rate)
        Value text
        Class "rated-textbox"
        Required true
        Key (sprintf "%d_%d" rate index)
        OnChange (fun ev -> dispatch (SetVote (rate, index, !!ev.target?value)))
    ]

let renderVoteItem  (voteItem : VoteItem ) dispatch =
    let rating = voteItem.Rating.ToString()
    div [ Key rating; Class "rate-item" ] [
        label [Class "rating-label"] [str <| sprintf "Rate %s" rating]
        div [Class "rate-textboxes"]
            (voteItem.People
            |> List.mapi (fun i item -> ratedTextBox (voteItem.Rating) item i dispatch))
    ]

let sideList =
    div[Class "side-list"] [
        h3 [] [str "Team"]
        ul [] [
            yield! (people  |> Seq.map (fun p-> li [Class "target-person"] [str p]))
        ]
    ]
let buttonPanel dispatch =
    div [Class "button-panel"][

        button [ OnClick (fun _ -> dispatch Submit) ] [ str "Submit" ]

        button [ OnClick (fun _ -> dispatch Reset) ][ str "Reset" ]
    ]

let voteItems votes dispatch =
    div [Class "rate-items"][
        yield h3 [][str "Enter your rating below"]
        yield! votes
            |> List.map(fun item -> renderVoteItem item dispatch)
    ]


let root model error dispatch =
    let vote = model.Vote

    div [Class "root"] [
        div [Class "edit-panel"][
            label [Class "error"] [str error ]
            voteItems vote.Votes dispatch
            sender vote.Sender dispatch
            buttonPanel dispatch
        ]
        sideList
    ]


let view (model : PageModel) dispatch =
    match model.PageStatus with
    | Input -> root model "" dispatch
    | Ok -> str "Thank you for your rating."
    | VoteError error -> root model error dispatch

