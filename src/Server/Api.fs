module Api


open Giraffe
open Microsoft.AspNetCore.Http
open Saturn
open Shared



let getWeather next (ctx : HttpContext)= task {  
    let! req = ctx.BindModelAsync<Model>()
   
    (* Task 4.1 WEATHER: Implement a function that retrieves the weather for
       the given postcode. Use the GeoLocation.getLocation, Weather.getWeatherForPosition and
       asWeatherResponse functions to create and return a WeatherResponse instead of the stub. *)
    return! json req next ctx }

let apiRouter = scope {
    
    pipe_through (pipeline { set_header "x-pipeline-type" "Api"})
    post "/Person" getWeather
    
    (* Task 1.0 CRIME: Add a new /crime/{postcode} endpoint to return crime data
       using the getCrimeReport web part function. Use the above distance
       route as an example of how to add a new route. *)    
        
    (* Task 4.2 WEATHER: Hook up the weather endpoint to the getWeather function. *)
    
    }
