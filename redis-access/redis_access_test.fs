#light
 
open NUnit.Framework

[<TestFixture>]
type RedisAccessTestCases = class
  new() = {}
 
  [<Test>]
  member x.ShouldReadAndWrite() =
    let k = "foo"
    let v = "bar"
    
    let reddo = new Redis()
    
    reddo.FlushAll()
    Assert.AreEqual(0, reddo.Keys.Length)
    
    reddo.Set(k, v)
    Assert.AreEqual(1, reddo.Keys.Length)
    Assert.AreEqual(Redis.KeyType.String, reddo.TypeOf(k))
    Assert.AreEqual(v, reddo.GetString(k))
    
    reddo.Remove(k) |> ignore
    Assert.AreEqual(0, reddo.Keys.Length)
end