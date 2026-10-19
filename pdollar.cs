using System.Runtime.InteropServices;
using System;
using System.Diagnostics;
class Pdollar
{
    static int Main(string[] args)
    {
        bool debugMode = false;

        if (args.Length < 1){
            Console.WriteLine("\n             >>>>USES<<<<\n");
            Console.WriteLine("'-t <filepath>'  adds a gesture file to the list of gesture templates");
            Console.WriteLine("'-r'             removes all gesture files");
            return -1;
        }

        if (args[0] == "-d")
            debugMode = true;
        
        if (debugMode)
            Console.WriteLine($"input {args[0]}");
        
        

        return 0;
    }
}
