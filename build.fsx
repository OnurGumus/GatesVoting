#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators


let serverPath = "./src/Server" |> Path.getFullName
let clientPath = "./src/Client" |> Path.getFullName
let deployDir = "./deploy" |> Path.getFullName

let platformTool tool winTool =
    let tool = if Environment.isUnix then tool else winTool
    match Process.tryFindFileOnPath tool with Some t -> t | _ -> failwithf "%s not found" tool

let nodeTool = platformTool "node" "node.exe"
let yarnTool = platformTool "yarn" "yarn.cmd"

[<Literal>]
let Clean = "Clean"

[<Literal>]
let InstallClient = "InstallClient"

[<Literal>]
let Build = "Build"

[<Literal>]
let RestoreServer = "RestoreServer"

[<Literal>]
let Run = "Run"

let run cmd args workingDir =
    let result =
        Process.execSimple 
            (fun info ->
                {info with 
                    FileName = cmd
                    WorkingDirectory = workingDir
                    Arguments = args})
                System.TimeSpan.MaxValue
         
           
    if result <> 0 then failwithf "'%s %s' failed" cmd args


Target.create Clean (fun _ ->
    !! (serverPath + "/**/bin")
    ++ (serverPath + "/**/obj")
    ++ (clientPath + "/**/bin")
    ++ (clientPath + "/**/obj")
    ++ deployDir
    |> Shell.cleanDirs 
)

Target.create InstallClient (fun _ ->
    printfn "Node version:"
    run nodeTool "--version" __SOURCE_DIRECTORY__
    printfn "Yarn version:"
    run yarnTool "--version" __SOURCE_DIRECTORY__
    run yarnTool "install" __SOURCE_DIRECTORY__
    DotNet.restore id clientPath
)

Target.create RestoreServer (fun _ ->
    DotNet.restore id serverPath 
)
Target.create "BuildClient" (fun _ ->DotNet.exec (fun e -> { e with WorkingDirectory = clientPath} ) "fable" "webpack -- -p" |> ignore)
Target.create Build (fun _ ->
    !! (serverPath + "/**/*.*proj")
    |> Seq.iter (DotNet.build id)
    DotNet.exec (fun e -> { e with WorkingDirectory = clientPath} ) "fable" "webpack -- -p" |> ignore
)

Target.create "RunServer" (fun _ -> DotNet.exec (fun e -> { e with WorkingDirectory = serverPath} ) "watch" "run " |> ignore)
Target.create "RunClient" (fun _ ->  DotNet.exec (fun e -> { e with WorkingDirectory = clientPath} ) "fable" "webpack-dev-server"  |> ignore )
Target.create Run (fun _ ->
    
    let server = async {
        Target.runSimple "RunServer" [] |> ignore
    }
    let client = async {
        Target.runSimple "RunClient" [] |> ignore
    }
    // let browser = async {
    //     System.Threading.Thread.Sleep 5000
    //     System.Diagnostics.Process.Start "http://localhost:8080" |> ignore
    // }

    [ client ; server ]
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore
)

Clean
    ==> InstallClient
    ==> Build
Build 
   ==> RestoreServer

Target.runOrDefault Run
