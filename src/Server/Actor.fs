module Actor

open Voting.Shared
open Akkling
open Akkling.Persistence
open Akka.Persistence
open Akka.Persistence.Sqlite
open System
open System.IO
open Validation

let akkaConfig = """
akka  {
    actor  {
        serializers  {
            plainnewtonsoft = "Voting.Server.Infrastructure.PlainNewtonsoftJsonSerializer, Server"
        }
    }
}


akka.persistence{
  journal {
    plugin = "akka.persistence.journal.sqlite"
    sqlite
    {
        connection-string = "Data Source=mydb.db;"
        auto-initialize = on
        serializer = plainnewtonsoft
    }
  }
  snapshot-store{
    plugin = "akka.persistence.snapshot-store.sqlite"
    sqlite {
        auto-initialize = on
        connection-string = "Data Source=mydb.db"
        serializer = plainnewtonsoft
    }
  }
}"""


type Command = Vote of Vote
type VoteData = { Vote : Vote; Date : DateTime}
type Event = Voted of VoteData


type Message =
    | Command of Command
    | Event of Event


let system = System.create "persisting-sys" <| Configuration.parse akkaConfig

SqlitePersistence.Get(system) |> ignore




let createVoterActor name =

    let actorProp (mailbox : Eventsourced<_>)=
            let oneMonth = TimeSpan.FromDays 30.


            let rec setState lastVoteDate =
                actor {
                    let! msg = mailbox.Receive()

                    match msg with

                    | Event(Voted voteData) ->
                        if  mailbox.IsRecovering() |> not then
                            mailbox.Sender() <! (Ok() :VoteResult)
                        return! setState voteData.Date

                    | Command(Vote vote) ->
                        let now = DateTime.Now
                        if now - lastVoteDate < oneMonth then
                            mailbox.Sender()
                                <! (Error (sprintf "You cannot vote before %A" <| lastVoteDate.Add oneMonth) : VoteResult)
                            return! setState lastVoteDate
                        else
                            match validateVote vote with
                            | Ok _ ->
                                return
                                    Voted {Vote = vote; Date = now}
                                    |> Event
                                    |> Persist
                            | Error e ->
                                mailbox.Sender() <! ((Error e) :VoteResult)
                                return! setState lastVoteDate
                }
            DateTime.MinValue |> setState

    spawn system name <| propsPersist actorProp

let map =
    File.ReadAllLines "Guids.txt"
        |> Array.map ( fun e -> e, createVoterActor e)
        |> Map.ofArray



let handleVote (vote:Vote) : Async<VoteResult> =
    async {
        match map.TryFind vote.Sender with
        | Some actor ->
            return! actor <? Command (Vote vote)
        | _ ->
            return Error "Sender found"
   }
