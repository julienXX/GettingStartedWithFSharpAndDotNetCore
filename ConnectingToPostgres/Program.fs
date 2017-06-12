open System
open System.Dynamic
open System.Collections.Generic
open Dapper
open Npgsql

type Post = {
    id : int
    title : string
    body : string
    }

type Comment = {
    id : int
    body : string
    date : System.DateTime
    }

// TODO: replace by your Postgres connection
let connectionString = "Host=HOST;Username=USERNAME;Password=PASSWORD;Database=DATABASE"

let connection =
    let conn = new NpgsqlConnection(connectionString)
    conn.Open()
    conn

let getComments postId =
    connection.Query<Comment> (sprintf "SELECT comments.* FROM comments WHERE comments.post_id = %d ORDER BY comments.date ASC" postId)

[<EntryPoint>]
let main argv =
    let (|EmptySeq|_|) a = if Seq.isEmpty a then Some () else None

    match argv with
        | [||] ->
            printfn "Usage: ConnectingToPostgres <postId>"

            1 // return an integer exit code
        | _ ->
            let postId = int argv.[0]
            let comments = getComments postId

            match comments with
                | EmptySeq -> printfn "No comments for this post."
                | _ ->
                    printfn "Found %d comments" <| Seq.length comments
                    printfn "Last comment: %A" <| Seq.head comments

            0 // return an integer exit code
