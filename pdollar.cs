using System.Runtime.InteropServices;
using System;
using System.Diagnostics;
using System.Data;
class Pdollar
{
    static int Main(string[] args)
    {
        bool debugMode = false;
        string help = "\n             >>>>HELP<<<<\n\n'-t <filepath>'  adds a gesture file to the list of gesture templates\n'-r'             removes all gesture files\n";

        if (args.Length < 1){
            Console.WriteLine(help);
            return -1;
        }

        if (args[0] == "-d")
            debugMode = true;
        
        if (debugMode){
            Console.WriteLine("input");
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine(args[i]);
            }
        }
        
        for (int i = 0; i < args.Length; i++)
        {
            if(args[i] == "-t")
            {
                return 0;
            }
            if(args[i] == "-r")
            {
                return 0;
            }
        }

        Console.WriteLine("Invalid Command");
        Console.WriteLine(help);
        return -1;
    }
}
