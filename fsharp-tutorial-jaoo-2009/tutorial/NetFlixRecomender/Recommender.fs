

open System
open System.Collections.Generic
open System.Data
open System.Data.Common
open System.Data.SqlClient
open System.Text

// SQL Compact Edition connection string
//let ConnectionString = @"Max Database Size=4091;Data Source=C:\Users\chrsmith\Desktop\DEFUN08\Scratch\NetFlixPrizeData\NetFlixData.sdf"

let ConnectionString = @"Integrated Security=True;User Instance=True;Data Source=.\SQLEXPRESS;AttachDbFilename=C:\Users\chrsmith\Desktop\DEFUN08\Part IV - Using .NET Libs for Data and Vis\.NET Libraries\GenerateSSEDatabase\bin\Debug\!NetFlixDataSSE.mdf"

type Rating = 
    {
        MovieId    : int 
        CustomerId : int
        Rating     : int16
    } 

let CreateReader (sqlString: string) =
    let connection = new SqlConnection(ConnectionString)
    do connection.Open()
    let command = new SqlCommand(sqlString, connection)
    command.CommandTimeout <- 75
    command.ExecuteReader(CommandBehavior.CloseConnection)

/// [1 .. 10] => (1,2,3,4,5,6,7,8,9,10)
let ListToSqlString (movies: int seq) =
    let str = new StringBuilder("(")
    
    movies |> Seq.iter (fun i -> str.Append(i.ToString() + ",") |> ignore)
    
    // Chop off last ','
    (str.ToString().Substring(0, str.Length - 1) + ")")

let resizearray (s:seq<_>) = new ResizeArray<_>(s)   

/// Get all Customers who have rated each movie in the list
let CustomersWhoRated (movies : int seq) =
    
    let movieIdString = ListToSqlString movies
    
    let sqlString = String.Format("
    SELECT CustomerID
    FROM MovieRatings 
    WHERE MovieRatings.MovieID IN {0}
    GROUP BY CustomerID
    ORDER BY COUNT(*)", movieIdString)

    seq { use reader = CreateReader sqlString 
          while reader.Read() do
              yield reader.GetInt32(0) }
      |> resizearray 
 
/// Gets all ratings for a given customer
let CustomerRatings (custID : int) =
    
    let sqlString = String.Format("
    SELECT MovieRatings.MovieID, MovieRatings.CustomerID, MovieRatings.Rating
    FROM MovieRatings 
    WHERE MovieRatings.CustomerID = {0}", custID)
          
    seq { use reader = CreateReader sqlString 
          while reader.Read() do
              yield     {   MovieId    = reader.GetInt32(0)
                            CustomerId = reader.GetInt32(1)
                            Rating     = reader.GetInt16(2) 
                        } 
        } 
      |> resizearray 


/// Total of mutual ratings stars
let CustomerStarsForMutualRatings (movies : int seq) =
    
    let movieIdString = ListToSqlString movies
    
    let sqlString = String.Format("
    SELECT CustomerID, TotalStars = SUM(Rating)
    FROM MovieRatings 
    WHERE MovieRatings.MovieID IN {0}
    GROUP BY CustomerID
    ORDER BY TotalStars DESC", movieIdString)
          
    seq { use reader = CreateReader sqlString 
          while reader.Read() do
              yield (reader.GetInt32(0), reader.GetInt32(1)) 
        }
      |> resizearray 
 
// -------------------------------------------------------------

// 788   -> Clerks
// 331   -> Chasing Amy
// 12072 -> Mall Rats
let myMovies = new Set<_>( [788; 331; 12072] )
let knownRatings = CustomersWhoRated myMovies

// Get customers which share my tastes, sort by compatability
let customerMutualRatings = CustomerStarsForMutualRatings myMovies
customerMutualRatings.Sort({ new IComparer<int * int> with
                                 member this.Compare ((_,a), (_,b)) = 
                                    if   a > b then -1
                                    elif a = b then  0 
                                    else             1 })

// For the top 10 customers, get all their ratings and group by movies
let topCustMatches = customerMutualRatings |> Seq.map (fun (rating, _) -> rating) |> Seq.take 10

let blackMagicQuery = "
    SELECT MovieID, TotalStars = SUM(MovieID)
    FROM MovieRatings 
    WHERE MovieRatings.CustomerID IN [topCustMatches]
    GROUP BY MovieID
    ORDER BY TotalStars"

let reccomendedMovies =
    topCustMatches
    |> Seq.map CustomerRatings
    |> Seq.concat
    |> Seq.groupBy (fun rating -> rating.MovieId)
    |> Seq.map (fun (movieId, ratings) -> movieId, 
                                          ratings |> Seq.fold (fun total rating -> total + rating.Rating) 0s)
    |> resizearray
    
reccomendedMovies.Sort(
    { new IComparer<int * int16> with
         member this.Compare ((_,a), (_,b)) = 
            if   a > b then  1
            elif a = b then  0 
            else            -1 })

reccomendedMovies
|> Seq.iter (fun (movieId, _) -> printfn "You should check out %d" movieId)