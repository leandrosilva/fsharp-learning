#light

open System.IO 

let sizeOfFolderComposed (*No Parameters!*) = 
    let getFiles folder = 
        Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories) 
    // The result of this expression is a function that takes 
    // one parameter, which will be passed to getFiles and piped 
    // through the following functions. 
    getFiles 
    >> Array.map (fun file -> new FileInfo(file)) 
    >> Array.map (fun info -> info.Length) 
    >> Array.sum;; 

let funcSizeOfFolderComposed = sizeOfFolderComposed

printfn ": %A" funcSizeOfFolderComposed
printfn ": %A" (funcSizeOfFolderComposed "/Users/leandro/Projects/c")