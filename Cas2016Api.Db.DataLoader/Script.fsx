#r "../packages/FSharp.Data.2.3.2/lib/net40/FSharp.Data.dll"
#r "System.Data.dll"
#r "FSharp.Data.TypeProviders.dll"
#r "System.Data.Linq.dll"

open FSharp.Data
open System
open System.Data
open System.Data.Linq
open Microsoft.FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq

let submissionsFile = __SOURCE_DIRECTORY__ + "\Submissions.json"

type Simple = JsonProvider<"./Submissions.json">
let azureConnectionString = "Server=tcp:cas2016apidb.database.windows.net,1433;Initial Catalog=Cas2016ApiDb;Persist Security Info=False;User ID={username};Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
type dbSchema = SqlDataConnection<"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Cas2016Api.db;Integrated Security=SSPI;">

let mutable authors : dbSchema.ServiceTypes.Speakers list = []
let mutable sessions : dbSchema.ServiceTypes.Sessions list = []

let getValue o prefix =
    match o with
    | Some x -> prefix + x
    | None -> ""

for item in Simple.GetSamples() do 
    printfn "%s" item.Email

    let author = 
        new dbSchema.ServiceTypes.Speakers(
            FirstName = item.Name,
            LastName = "",
            TwitterProfile = getValue item.Twitter "http://twitter.com/",
            LinkedinProfile = "",
            Website = getValue item.Url "",
            Biography = getValue item.Bio "",
            Image = item.Avatar,
            City = item.Location,
            Country = "" )

    let session =
        new dbSchema.ServiceTypes.Sessions(
            Title = item.Title,
            Description = item.Description,
            Duration = 45,
            StartTime = new Nullable<DateTime>(new DateTime(2016, 12, 1, 9, 0, 0)),
            EndTime = new Nullable<DateTime>(new DateTime(2016, 12, 1, 9, 0, 0)),
            Tags = ( item.TagList |> String.concat ";" ),
            Room = 1)
    authors <- author :: authors
    sessions <- session :: sessions

let mutable ss : dbSchema.ServiceTypes.SessionsSpeakers list = []
let sessionsspeakers =
    [for i in [4..58] ->
        new dbSchema.ServiceTypes.SessionsSpeakers(
                SessionId = i,
                SpeakerId = i
        )
    ]

let db = dbSchema.GetDataContext(azureConnectionString)
db.Speakers.InsertAllOnSubmit(authors)
db.Sessions.InsertAllOnSubmit(sessions)
db.SessionsSpeakers.InsertAllOnSubmit(sessionsspeakers)


try
  db.DataContext.SubmitChanges()
  printfn "Successfully inserted new rows."
with
| exn -> printfn "Exception:\n%s" exn.Message