using System;
using System.IO;

public class Program
{
    public static void Main(string[] args)
    {
        if ((args.Length < 1 || (args.Length > 1) && (!args[1].Equals("-c"))) || (args.Length == 1 && (args[0].Equals("-h") || args[0].Equals("-help")))) //Atirtam a hibauzenet helyett egy Help-re, igy kicsit baratibb
        {
            Console.WriteLine("HASZNALATI UTMUTATO:\n\nHa letre szeretnenk hozni a HTML fajlokat:\n\tdotnet run <mappa eleresi utvonala>\n\nHa torolni szeretnenk a HTML fajlokat:\n\tdotnet run <mappa eleresi utvonala> -c");
            return;
        }
        if (args.Length == 1) //A HTML Generator meghivasa
        {
            DirectoryInfo f = new DirectoryInfo(args[0]);
            if (f.Exists) //Ellenorzi, hogy letezik-e a mappa amit megadtunk
            {
                HTMLGenerator valami = new HTMLGenerator(args[0]);
                valami.Walk(args[0]);
            }
            else
            {
                Console.WriteLine("HIBA! Adj meg egy letezo eleresi utat!");
                return;
            }
        }
        if (args.Length == 2 && args[1].Equals("-c")) //Cleaner meghivasa
        {
            DirectoryInfo f = new DirectoryInfo(args[0]);
            if (f.Exists) //Ellenorzi, hogy letezik-e a mappa amit megadtunk
            {
                Cleaner(args[0]);
                Console.WriteLine("HTML fajlok torolve!");
                return;
            }
            else
            {
                Console.WriteLine("HIBA! Adj meg egy letezo eleresi utat!");
                return;
            }
        }
    }

    public static void Cleaner(string path) //Cleaner metodus
    {
        DirectoryInfo root = new DirectoryInfo(path);
        FileInfo[] files = root.GetFiles();
        DirectoryInfo[] dirs = root.GetDirectories();

        foreach (FileInfo file in files)
        {
            if (file.Extension.ToLower() == ".html")
                file.Delete();
        }

        foreach (DirectoryInfo dir in dirs)
        {
            Cleaner(dir.FullName);
        }
    }
}
