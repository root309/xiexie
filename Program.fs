open System
open System.IO
open System.Net
open System.Threading.Tasks
open Microsoft.FSharp.Control

let listener = new HttpListener()
listener.Prefixes.Add("http://*:8080/")

let processRequest (context: HttpListenerContext) = async {
    let response = context.Response
    let responseString = $"xiexie The current time is: {DateTime.Now}"
    let buffer = System.Text.Encoding.UTF8.GetBytes(responseString)
    response.ContentLength64 <- int64 buffer.Length
    use output = response.OutputStream
    do! output.WriteAsync(buffer, 0, buffer.Length) |> Async.AwaitTask
    response.Close()
}

let startServer () =
    listener.Start()
    printfn "Listening on http://localhost:8080/"
    let rec loop () = async {
        let! context = listener.GetContextAsync() |> Async.AwaitTask
        do! processRequest context
        return! loop ()
    }
    Async.StartImmediate(loop())

startServer ()
Console.ReadLine() |> ignore
