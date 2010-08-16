using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VerboseCSharp
{
    class Program
    {
        public static IEnumerable<U> Map<T, U>(Func<T, U> f, IEnumerable<T> e)
        {
            foreach (var x in e)
            {
                yield return f(x);
            }
        }

        static void Main(string[] args)
        {
            var range = Enumerable.Range(1, 10);
            var mapped = Map(x => x * x, range);
            foreach (var map in mapped) Console.WriteLine(map);
        }
    }
}
