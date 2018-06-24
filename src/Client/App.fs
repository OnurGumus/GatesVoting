module App.View

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
open System
open Shared


type Message =
    | SetPersonName of name:string
    | Submit
    | Success of string
    | Error of exn
let authUser (model:Model) =
    promise {
        //if String.IsNullOrEmpty login.UserName then return! failwithf "You need to fill in a username." else
        //if String.IsNullOrEmpty login.Password then return! failwithf "You need to fill in a password." else

        let body = toJson model
        console.log body
        let props =
            [ RequestProperties.Method HttpMethod.POST
              Fetch.requestHeaders [
                  HttpRequestHeaders.ContentType "application/json" ]
              RequestProperties.Body !^body]

        try
            return! Fetch.fetchAs<string> "/api/Person" props
            
        with _ ->
            return! failwithf "Could not authenticate user."
    }
let view model dispatch =
    div [] [
        input [
            Id "username"
            HTMLAttr.Type "text"
            Placeholder "Name"
            OnChange (fun ev -> dispatch (SetPersonName !!ev.target?value))
            AutoFocus true
        ]
        button [ OnClick (fun _ -> dispatch Submit) ]
            [ str "Log In" ]
    ]


let update msg model =
    match msg with
    | SetPersonName name -> { model with Person = name}  , Cmd.none
    | Submit ->
        let cmd = Cmd.ofPromise (authUser) model Success Error
        model, cmd
    | Success e -> model , Cmd.none
    | Error e -> model , Cmd.none

let init () = { Person = ""} , Cmd.none

open Elmish.React
open Elmish.Debug
open Elmish.HMR

// App
Program.mkProgram init update view 

//|> Program.toNavigable (parseHash pageParser) urlUpdate

|> Program.withDebugger
|> Program.withHMR

|> Program.withReact "elmish-app"
|> Program.run