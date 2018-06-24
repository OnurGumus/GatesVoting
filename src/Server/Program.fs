open Api
open Giraffe
open Giraffe.Serialization
open Microsoft.Extensions.DependencyInjection
open Saturn
open System.IO

let clientPath = "wwwroot"
let port = 8085us

let browserRouter = scope {
    get "/" (htmlFile (Path.Combine(clientPath, "/index.html"))) }

let mainRouter = scope {

    forward "/api" apiRouter
    forward "" browserRouter }

let config (services:IServiceCollection) =
    let fableJsonSettings = Newtonsoft.Json.JsonSerializerSettings()
    fableJsonSettings.Converters.Add(Fable.JsonConverter())
    services.AddSingleton<IJsonSerializer>(NewtonsoftJsonSerializer fableJsonSettings) |> ignore
    services.AddCors |> ignore
    services

let app = application {
    //pipe_through (pipeline{ plug  (enableCors CORS.defaultCORSConfig) })
    router mainRouter
    url ("http://0.0.0.0:" + port.ToString() + "/")
    memory_cache
    use_cors "CORS_policy" (fun builder -> builder.AllowAnyHeader() |> ignore; builder.AllowAnyMethod() |> ignore; builder.AllowAnyOrigin() |> ignore)
    use_static clientPath
    service_config config
    use_gzip }

run app