// Password for Twitter samples
//

[<AutoOpen>]
module TwitterPassword

let userName : string = 
    System.Windows.Forms.MessageBox.Show "please set your Twitter user name in TwitterPassword.fs" |> ignore
    failwith "set your Twitter user name in TwitterPassword.fs"

let password : string = 
    System.Windows.Forms.MessageBox.Show "please set your Twitter password in TwitterPassword.fs" |> ignore
    failwith "set your Twitter password in TwitterPassword.fs"
