module Validation
open Voting.Shared

let checkSets voteList =
    let concat s item = s @ item.People
    let set = voteList |> List.fold concat [] |> Set.ofList

    if set.IsSubsetOf people &&
        people.IsSubsetOf set then
        Ok voteList
    else
        Error "Invalid vote"


let check voteItem targetLength =
    voteItem.People.Length = targetLength

let checkItems voteList =
    match voteList with
    |[first; second; third; fourth ; fifth]
        when
            (check first 1)
            && (check second 2)
            && (check third 3)
            && (check fourth 2)
            && (check fifth 1)
            -> Ok voteList

    | _ -> Error "Vote count doesn't match"


let validateVote vote =
    vote.Votes |> (checkItems >> Result.bind checkSets)

