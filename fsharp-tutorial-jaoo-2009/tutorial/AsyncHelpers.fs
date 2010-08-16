
namespace FSharp.Tutorial

open System.Text
open System.IO
open Microsoft.FSharp.Control.WebExtensions

[<AutoOpen>]
module AsyncHelpers =

    type System.IO.StreamReader with 

        /// An extension member to read all the lines from a stream reader asynchronously.
        /// 
        /// In this implementation, the operation is pseduo-asynchronous.
        member reader.AsyncReadAllLines () = 
            async { return [ while not reader.EndOfStream do 
                                yield reader.ReadLine()] }
    
    type System.Net.WebRequest with


        /// An extension member to write content into an WebRequest. 
        /// 
        /// In this implementation, the operation is pseduo-asynchronous.
        member req.AsyncWriteContent (content:string) = 
            async { let bytes = Encoding.UTF8.GetBytes content
                    req.ContentLength <- int64 bytes.Length
                    use stream = req.GetRequestStream()
                    stream.Write(bytes,0,bytes.Length)  }

        /// An extension member to read all the content of a response to a WebRequest. 
        /// 
        /// In this implementation, the operation is pseduo-asynchronous.
        member req.AsyncReadResponse () = 
            async { use! response = req.AsyncGetResponse()
                    use responseStream = response.GetResponseStream()
                    use reader = new StreamReader(responseStream)
                    return reader.ReadToEnd() }

        /// An extension member to read all the content of a response to a WebRequest as a set of lines.
        /// 
        /// In this implementation, the operation is pseduo-asynchronous.
        member req.AsyncReadResponseLines () = 
            async { use! response = req.AsyncGetResponse()
                  
                    return [use stream = response.GetResponseStream()
                            use reader = new StreamReader(stream)
                            while not reader.EndOfStream do 
                                yield reader.ReadLine()] }


