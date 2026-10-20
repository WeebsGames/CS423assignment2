using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using PDollarGestureRecognizer;
using PDollarDemo;
class Pdollar
{
    static int Main(string[] args)
    {
        bool debugMode = false;
        string help =   "\n             >>>>HELP<<<<\n\n"+
                        "'-t <filepath>'  adds a gesture file to the list of gesture templates\n"+
                        "'-r'             removes all gesture files\n"+
                        "'<eventstream>'  prints gestures as they are recognized from an eventstream";
        string gestureSetPath = @".\$P\PDollarDemo\bin\Release\GestureSet";
        string path = Path.Combine(gestureSetPath, "NewGestures");
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
                string filePath = args[i+1];
                string fileName = Path.GetFileName(filePath);
                Directory.CreateDirectory(path);
                File.Copy(filePath, Path.Combine(path, fileName), true);
                Console.WriteLine($"Added {fileName} to gesture maps");
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
            if(File.Exists(args[i]))
            {
                try
                {
                    Gesture[] trainingSet = LoadTrainingSet(gestureSetPath);
                    if (trainingSet.Length == 0)
                    {
                        Console.Error.WriteLine("No gesture templates found in " + gestureSetPath);
                        return -1;
                    }
                    foreach (Gesture candidate in ReadTextGestures(args[i]))
                        Console.WriteLine(PointCloudRecognizer.Classify(candidate, trainingSet));
                }
                catch (FormatException ex)
                {
                    Console.Error.WriteLine(ex.Message);
                    return -1;
                }

                return 0;
            }
        }

        Console.WriteLine("Invalid Command");
        Console.WriteLine(help);
        return -1;
    }

    private static Gesture[] LoadTrainingSet(string gestureSetPath)
    {
        List<Gesture> gestures = new List<Gesture>();
        if (!Directory.Exists(gestureSetPath))
            return gestures.ToArray();

        foreach (string folder in Directory.GetDirectories(gestureSetPath))
        {
            foreach (string file in Directory.GetFiles(folder, "*.xml"))
                gestures.Add(GestureIO.ReadGesture(file));
            foreach (string file in Directory.GetFiles(folder, "*.txt"))
                gestures.AddRange(ReadTextGestures(file));
        }
        return gestures.ToArray();
    }

    private static Gesture[] ReadTextGestures(string fileName)
    {
        List<Gesture> gestures = new List<Gesture>();
        List<Point> points = new List<Point>();
        string gestureName = Path.GetFileNameWithoutExtension(fileName);
        string? expectedEnd = null;
        int strokeId = -1;
        int strokeStart = 0;
        int lineNumber = 0;
        bool hasContent = false;

        FormatException Invalid(string message) =>
            new FormatException($"{fileName}, line {lineNumber}: {message}");

        void AddGesture()
        {
            bool hasMovement = false;
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].StrokeID == points[i - 1].StrokeID &&
                    (points[i].X != points[i - 1].X || points[i].Y != points[i - 1].Y))
                    hasMovement = true;
            }
            if (!hasMovement)
                throw Invalid("A gesture must contain movement within at least one stroke.");

            gestures.Add(new Gesture(points.ToArray(), gestureName));
            points.Clear();
            strokeId = -1;
        }

        foreach (string rawLine in File.ReadLines(fileName))
        {
            lineNumber++;
            string line = rawLine.Trim();
            if (line.Length == 0)
                continue;

            if (line == "MOUSEDOWN" || line == "BEGIN")
            {
                if (expectedEnd != null)
                    throw Invalid("The previous stroke has not ended.");
                strokeId++;
                strokeStart = points.Count;
                expectedEnd = line == "BEGIN" ? "END" : "MOUSEUP";
            }
            else if (line == "MOUSEUP" || line == "END")
            {
                if (line != expectedEnd)
                    throw Invalid("Unexpected stroke end.");
                if (points.Count == strokeStart)
                    throw Invalid("A stroke must contain coordinates.");
                expectedEnd = null;
            }
            else if (line == "RECOGNIZE")
            {
                if (expectedEnd != null)
                    throw Invalid("End the stroke before RECOGNIZE.");
                AddGesture();
            }
            else if (!hasContent && !line.Contains(','))
            {
                gestureName = line;
            }
            else
            {
                string[] xy = line.Split(',');
                if (expectedEnd == null)
                    throw Invalid("Coordinates must be inside a stroke.");
                if (xy.Length != 2 ||
                    !float.TryParse(xy[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) ||
                    !float.TryParse(xy[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) ||
                    !float.IsFinite(x) || !float.IsFinite(y))
                    throw Invalid("Expected finite x,y coordinates.");
                points.Add(new Point(x, y, strokeId));
            }
            hasContent = true;
        }

        if (expectedEnd != null)
            throw Invalid("The final stroke has not ended.");
        if (points.Count > 0)
            AddGesture();
        if (gestures.Count == 0)
            throw Invalid("No gestures found.");
        return gestures.ToArray();
    }
}
