using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class HTMLGenerator
{
    private string start;

    public HTMLGenerator(string start) 
    { 
        this.start = start; 
    }

    public string GetStart() 
    { 
        return this.start; 
    }

    public void Walk(string path)
    {
        DirectoryInfo root = new DirectoryInfo(path);
        FileSystemInfo[] list = root.GetFileSystemInfos();
        List<string> directories = new List<string>();
        List<string> pictures = new List<string>();

        foreach (FileSystemInfo f in list) //Listaba rakja a kepeket es a mappakat kulon
        {
            if (f is DirectoryInfo)
            {
                string dir = f.Name;
                if (!directories.Contains(dir))
                    directories.Add(dir);
                Walk(f.FullName);
            }
            else if (f.FullName.ToLower().EndsWith(".png") || f.FullName.ToLower().EndsWith(".jpg") || 
                     f.FullName.ToLower().EndsWith(".jpeg") || f.FullName.ToLower().EndsWith(".webp") || 
                     f.FullName.ToLower().EndsWith(".gif"))
            //Csak a kepeket fogja keresni (Legenerált HTML fajlok utan irtam at erre)
            {
                string pic = f.Name;
                if (!pictures.Contains(pic))
                    pictures.Add(pic);
            }
        }

        Generator(path, GetStart(), pictures, directories);
        if (!path.Equals(GetStart()))
            Console.WriteLine("\"" + RelativeGen(GetStart(), path) + "\"" + " sikeresen bejarva");
        else
            Console.WriteLine("\nHTML Fajlok sikeresen legeneralva.");
    }

    public void Generator(string path, string absolute, List<string> pictures, List<string> directories)
    {
        string relative = RelativeGen(absolute, path);
        for (int i = 0; i < pictures.Count; i++)
        {
            string s = pictures[i]; //pl.: 001.jpg
            string html = HTMLMaker(s); //pl.: 001.html
            try
            {
                using (StreamWriter fw = new StreamWriter(Path.Combine(path, html))) //kep.html letrehozasa (pl.: abszolut eleres + 001.html)
                {
                    StringBuilder sb = new StringBuilder();
                    if (pictures.Count == 1)  //Ha csak egyetlen kep lenne a mappaban
                        sb.Append(HTMLimg(absolute, html, s, html));
                    else if (i == 0) //elso elem lekezelese
                        sb.Append(HTMLimg(absolute, html, s, HTMLMaker(pictures[i + 1])));
                    else if (i == pictures.Count - 1) //utolso elem lekezelese
                        sb.Append(HTMLimg(absolute, HTMLMaker(pictures[i - 1]), s, html));
                    else //barmilyen elem, ami nem elso vagy utolso
                        sb.Append(HTMLimg(absolute, HTMLMaker(pictures[i - 1]), s, HTMLMaker(pictures[i + 1])));
                    fw.Write(sb.ToString()); //fajlba iras
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("HIBA KEP.HTML LETREHOZASAKOR!");
                Console.WriteLine(e.ToString());
            }
        }

        StringBuilder sbIndex = new StringBuilder(); //Index.html generalas
        string dirHTML = Path.Combine(path, "index.html");
        try
        {
            using (StreamWriter fw = new StreamWriter(dirHTML))
            {
                sbIndex.Append(HTMLindexFirst(absolute, relative));  //Az indexet tobb reszre kellett szednem, mert ellenoriznem kellett hogy van-e tobb mappa vagy kep
                if (!path.Equals(absolute))
                    sbIndex.Append("\t\t<a href=\"../index.html\">^^</a>\r\n");
                sbIndex.Append(HTMLDir(directories)); //Directories kiiratas
                sbIndex.Append(HTMLPic(pictures));  //Pictures kiiratas (ha van)
                fw.Write(sbIndex.ToString()); //fajlba iras
            }
        }
        catch (IOException e)
        {
            Console.WriteLine("HIBA INDEX.HTML LETREHOZASAKOR!");
            Console.WriteLine(e.ToString());
        }
    }

    public static string RelativeGen(string abs, string path)
    {
        if (abs.Equals(path))
            return "";
        return path.Replace(abs + Path.DirectorySeparatorChar, "");
    }

    public string HTMLimg(string index, string prev, string name, string next) //A kepek elozo, jelenlegi es kovetkezo elemet keri be, es az abszolut eleresi utat a fo indexbe valo visszalepeshez
    {
        string s = string.Format(
            "<html>\r\n" +
            "<head>\r\n" +
            "\t<title>{0}</title>\r\n" +
            "\t<style>\r\n" +
            "\t\th1 {{text-align: center;}}\r\n" +
            "\t\tdiv {{text-align: center;}}\r\n" +
            "\t\timg {{display: block; margin-left: auto; margin-right: auto; max-width: 95%; max-height: 95%;}}\r\n" +
            "\t</style>\r\n" +
            "</head>\r\n" +
            "<body>\r\n" +
            "\t\t<a href=\"{1}\"><h1>Index</h1></a>\r\n" +
            "\t<hr>\r\n" +
            "\t<div>\r\n" +
            "\t\t<a href=\"index.html\">^^</a>\r\n" +
            "\t\t<p><a href=\"{2}\">Back</a>\r\n" +
            "\t\t{3}\r\n" +
            "\t\t<a href=\"{4}\">Next</a></p>\r\n" +
            "\t</div>\r\n" +
            "\t<hr>\r\n" +
            "\t\t<a href=\"{5}\"><img src=\"{6}\" class=\"img\"></a>\r\n" +
            "</body>\r\n" +
            "</html>", 
            name, 
            Path.Combine(index, "index.html"), 
            prev, 
            name, 
            next, 
            next, 
            name);
        return s;
    }

    public string HTMLindexFirst(string index, string name) //Mappa nevet keri be cimnek, es az abszolut elerest a fo indexbe valo visszalepeshez
    {
        string tmp = string.IsNullOrEmpty(name) ? Path.GetFileName(index) : 
                     name.Contains(Path.DirectorySeparatorChar.ToString()) ? 
                     name.Substring(name.LastIndexOf(Path.DirectorySeparatorChar) + 1) : name;
        string s = string.Format(
            "<html>\r\n" +
            "<head>\r\n" +
            "\t<title>{0}</title>\r\n" +
            "</head>\r\n" +
            "<body>\r\n" +
            "\t<a href=\"{1}\"><h1>Index</h1></a>\r\n" +
            "\t<hr>\r\n" +
            "\t<h2><b>Directories</b></h2>\r\n", 
            tmp, 
            Path.Combine(index, "index.html"));
        return s;
    }

    public string HTMLDir(List<string> dir) //Az indexFirst-ben mar legeneraltam a Directories feliratot, itt mar nem kell
    {
        StringBuilder sb = new StringBuilder();
        foreach (string directory in dir)
            sb.Append(string.Format("\t<li><a href=\"{0}\">{1}</a></li>\n", directory + "/index.html", directory));
        return sb.ToString();
    }

    public string HTMLPic(List<string> pic) //Mivel a Directories utan jon, így kulon le kell generaltatni
    {
        if (pic.Count == 0)
            return "</body>\n</html>";  //Ha ures akkor ne generalja le
        StringBuilder sb = new StringBuilder();
        sb.Append("\t<hr>\n\t<h2><b>Pictures</b></h2>\n");
        foreach (string picture in pic)
            sb.Append(string.Format("\t\t<li><a href=\"{0}\">{1}</a></li>\n", 
                picture.Substring(0, picture.LastIndexOf(".")) + ".html", picture));
        sb.Append("</body>\n</html>");
        return sb.ToString();
    }

    private static string HTMLMaker(string s)
    {
        return s.Substring(0, s.LastIndexOf(".")) + ".html"; //lusta voltam mindig beírni, szóval csak csináltam már egy metódust rá
    }
}
