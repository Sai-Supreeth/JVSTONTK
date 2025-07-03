using System;
class HelloWorld {
    static void Main(string[] args) {
        // Creates a reader instance which takes;
        // input from standard input - keyboard;
        Scanner reader = new Scanner(System.in);
        Console.Write("Enter a number: ");
        // nextInt() reads the next integer from the keyboard;
        int number = reader.nextInt();
        // println() prints the following line to the output screen;
        Console.WriteLine("You entered: " + number);
    }
}
