open System
open System.Net.Http
open System.Net.Http.Headers
open System.Threading.Tasks
open Newtonsoft.Json

type Issue = {
    Url: string
    Title: string
}

let getAsync (url:string) =
    async {
        use httpClient = new System.Net.Http.HttpClient()
        httpClient.DefaultRequestHeaders.Accept.Add(
                MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"))
        httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter")

        use! response = httpClient.GetAsync(url) |> Async.AwaitTask
        printfn "%A" <| response.Headers.GetValues "Link"
        response.EnsureSuccessStatusCode () |> ignore
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        return content
    }

[<EntryPoint>]
let main argv =
    let issues =
        "https://api.github.com/repos/julienXX/terminal-notifier/issues?state=closed&per_page=100&page=1"
        |> getAsync
        |> Async.RunSynchronously
    let deserializedIssues = JsonConvert.DeserializeObject<Issue list>(issues)

    deserializedIssues |> List.iter (fun x -> printfn "Url: %s, Title: %s" x.Url x.Title)

    0 // return an integer exit code
