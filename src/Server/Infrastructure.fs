namespace Voting.Server.Infrastructure
open Akka.Actor
open Akka.Serialization
open Newtonsoft.Json
open System
open System.IO
open System.Text

type PlainNewtonsoftJsonSerializer ( system : ExtendedActorSystem) =
    inherit Serializer(system)
    let settings =
        new JsonSerializerSettings(TypeNameHandling = TypeNameHandling.All,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead)
    let ser = new JsonSerializer()

    override __.IncludeManifest = true


    override __.Identifier = 1711234423;

    override __.ToBinary(o:obj) =
        ser.TypeNameHandling <- TypeNameHandling.All
        ser.MetadataPropertyHandling <-MetadataPropertyHandling.ReadAhead
        let memoryStream = new MemoryStream();
        use streamWriter = new StreamWriter(memoryStream, Encoding.UTF8)
        ser.Serialize(streamWriter, o, o.GetType())
        streamWriter.Flush()
        memoryStream.ToArray()

    override __.FromBinary(bytes : byte array, ttype : Type) =
        ser.TypeNameHandling <- TypeNameHandling.All
        ser.MetadataPropertyHandling <-MetadataPropertyHandling.ReadAhead
        use streamReader = new StreamReader(new MemoryStream(bytes), Encoding.UTF8)
        ser.Deserialize(streamReader, ttype)
