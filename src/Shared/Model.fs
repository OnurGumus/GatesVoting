module Voting.Shared

type VoteItem = {Rating : int; People : string list}

type Vote = { Sender : string ; Votes : VoteItem list}
type VoteResult = Result<Unit,string>


let initialModel =
    {   Sender = "";
        Votes =
            [   { Rating = 1; People = [""]}
                { Rating = 2; People = ["";""]}
                { Rating = 3; People = ["";"";"";]}
                { Rating = 4; People = ["";"";]}
                { Rating = 5; People = [""]}
            ]
    }




let people = ["Onur"; "Anand";"Sivakumar"; "Khalid"; "Chaitanya"; "Shafeeque"; "Ali"; "Segun"; "Shahid" ] |> Set.ofList




