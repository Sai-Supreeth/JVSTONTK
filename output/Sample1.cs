// Java Program to demonstrate ArrayList
using System.Collections.Generic;
class Main {
    static void Main(string[] args) {
          // Creating an ArrayList;
          List<int> a = new List<int>();
          // Adding Element in ArrayList;
          a.Add(1);
          a.Add(2);
          a.Add(3);
          // Printing ArrayList;
          Console.WriteLine(string.Join(", ", a));
    }
}
