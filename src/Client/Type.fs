module Type
open Voting.Shared


type Message =
    | SetVote of vote : int * index :int * name : string
    | SetSender of string
    | Submit
    | Reset
    | Success of VoteResult
    | Error of exn


type PageStatus = Input | Ok  | VoteError of string

type PageModel = { PageStatus : PageStatus; Vote : Vote}

