using System.Runtime.InteropServices;
using System;
using System.Diagnostics;
using System.Data;
using System.IO;
class Pdollar
{
    static int Main(string[] args)
    {
        bool debugMode = false;
        string help =   "\n             >>>>HELP<<<<\n\n"+
                        "'-t <filepath>'  adds a gesture file to the list of gesture templates\n"+
                        "'-r'             removes all gesture files\n"+
                        "'<eventstream>'  prints gestures as they are recognized from an eventstream";
        string path = @".\$P\PDollarDemo\bin\Release\GestureSet\NewGestures";
        DirectoryInfo dir = new DirectoryInfo(path);

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
                string file = args[i+1];
                File.Copy(file, path, true);
                Console.WriteLine($"Added {args[i+1]} to gesture maps");
                return 0;
            }
            if(args[i] == "-r")
            {
                foreach (FileInfo file in dir.GetFiles())
                {
                    file.Delete();
                }
                Console.WriteLine("Successfully deleted all gesture maps");
                return 0;
            }
            if(args[i] == "eventstream")
            {
                return 0;
            }
        }

        Console.WriteLine("Invalid Command");
        Console.WriteLine(help);
        return -1;
    }
}
