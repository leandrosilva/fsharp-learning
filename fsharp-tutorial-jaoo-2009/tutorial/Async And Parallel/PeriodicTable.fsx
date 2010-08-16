//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of asynchronous 
// web-service programming in F#.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


#load @"..\show.fs"
#r "PeriodicTable.dll"


open System.Xml
open System.Threading

let webService = new PeriodicTableWS.periodictable()

// A synchronous web service call
webService.GetAtoms() |> show


// A synchronous web service call
webService.GetAtomicWeight("Hydrogen") |> show




/// Make the web service operations available as extension methods
type PeriodicTableWS.periodictable with 

    member ws.GetAtomsAsAsync() =
        Async.FromBeginEnd(ws.BeginGetAtoms,
                           ws.EndGetAtoms)

    member ws.GetAtomicNumberAsAsync(element) =
        Async.FromBeginEnd(element,
                           ws.BeginGetAtomicNumber,
                           ws.EndGetAtomicNumber)

    member ws.GetAtomicWeightAsAsync(element) =
        Async.FromBeginEnd(element,
                           ws.BeginGetAtomicWeight,
                           ws.EndGetAtomicWeight)


async 
    { let! xml = webService.GetAtomicWeightAsAsync "Cadmium"
      return xml }
    |> Async.RunSynchronously
    |> show



let decodeWeight(xml:string) =
      let doc = new XmlDocument()
      doc.LoadXml(xml)
      let node = doc.SelectSingleNode("/NewDataSet/Table/AtomicWeight")
      match node with 
      | null -> failwith "weight not found"
      | s -> float(node.InnerText)


async 
    { let! xml = webService.GetAtomicWeightAsAsync "Cadmium"
      let weight = decodeWeight xml
      return weight }
    |> Async.RunSynchronously
    |> show


async 
    { show "Getting Cadmium..."
      let! xml1 = webService.GetAtomicWeightAsAsync "Cadmium"
      let weight1 = decodeWeight(xml1)
      show "Getting Oxygen..."
      let! xml2 = webService.GetAtomicWeightAsAsync "Oxygen"
      let weight2 = decodeWeight(xml2)
      show "Getting Hydrogen..."
      let! xml3 = webService.GetAtomicWeightAsAsync "Hydrogen"
      let weight3 = decodeWeight(xml3)
      return (weight1,weight2,weight3) }
    |> Async.RunSynchronously
    |> show

async 
    { show "getting three in parallel..."
      let! xmls = 
        Async.Parallel
          [ for element in ["Cadmium"; "Oxygen"; "Hydrogen"] ->
              webService.GetAtomicWeightAsAsync element ]
      let weights = Array.map decodeWeight xmls
      return weights }
    |> Async.RunSynchronously
    |> show


async 
    { let! xml1 = webService.GetAtomicWeightAsAsync "Cadmium"
      let weight1 = decodeWeight xml1
      try 
        let! xml2 = webService.GetAtomicWeightAsAsync "Pentium"
        let weight2 = decodeWeight(xml2)
        return box (weight1, weight2)
      with _ -> 
        return box (weight1, "couldn't get weight for Pentium!") }
    |> Async.RunSynchronously
    |> show


async 
    { let! xml1 = webService.GetAtomicWeightAsAsync "Cadmium"
      let! xml2 = webService.GetAtomicWeightAsAsync "Pentium"
      let weight2 = decodeWeight(xml2)
      show (xml1,xml2) }
    |> Async.RunSynchronously
    |> show



/// A workflow to get the names of all the elements via XML
let workFlow1() = 
    async { 
        show "Getting Element Data List"

        let! elementsXML = webService.GetAtomsAsAsync()

        let elements = 
            let doc = new XmlDocument()
            do doc.LoadXml(elementsXML)
            let nodes = doc.SelectNodes("/NewDataSet/Table/ElementName")
            [ for node in nodes -> node.InnerText ]
    
        sprintf "Got %d Elements" elements.Length |> show
        return elements 
    }


workFlow1() |> Async.RunSynchronously |> show



/// A workflow to get the atomic number and weights in sequence
let workFlow2(element) = 
    async { 
        show (sprintf "Getting %s" element)

        let! weight = webService.GetAtomicWeightAsAsync(element)

        show (sprintf "Got raw data, now processing %s response" element)
        return (element, decodeWeight weight)
    }

workFlow2 "Cadmium" |> Async.RunSynchronously |> show



/// A workflow to get all element names then get all weights in parallel
let workFlow3  = 
    async { 
        let! elements = workFlow1() 

        let! weights = 
            Async.Parallel [ for element in elements -> workFlow2 element ]

        return weights
    }

workFlow3 |> Async.RunSynchronously |> show

