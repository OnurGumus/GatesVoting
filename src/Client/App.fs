module App.View

open Elmish
open Elmish.Browser.Navigation
open State
open View
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