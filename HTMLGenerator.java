import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

public class HTMLGenerator
{
    private String start;
    public HTMLGenerator(String start) {this.start = start;}
    public String getStart() {return this.start;}
    public void walk(String path)
    {
        File root = new File(path);
        File[] list = root.listFiles();
        List<String> directories = new ArrayList<>();
        List<String> pictures = new ArrayList<>();
        for (File f : list) //Listaba rakja a kepeket es a mappakat kulon
        {
            if (f.isDirectory())
            {
                String dir = f.toString().substring(f.toString().lastIndexOf("\\") + 1);
                if (!directories.contains(dir))
                    directories.add(dir);
                walk(f.getAbsolutePath());
            } 
            else if (f.toString().toLowerCase().endsWith(".png") || f.toString().toLowerCase().endsWith(".jpg") || f.toString().toLowerCase().endsWith(".jpeg") || f.toString().toLowerCase().endsWith(".webp") || f.toString().toLowerCase().endsWith(".gif"))
            //Csak a kepeket fogja keresni (Legenerált HTML fajlok utan irtam at erre)
            {
                String pic = f.toString().substring(f.toString().lastIndexOf("\\") + 1);
                if (!pictures.contains(pic))
                    pictures.add(pic);
            }
        }
        Generator(path, getStart(), pictures, directories);
        if (!path.equals(getStart()))
            System.out.println('"' + RelativeGen(getStart(), path) + '"' + " sikeresen bejarva");
        else
            System.out.println("\nHTML Fajlok sikeresen legeneralva.");
    }

    public void Generator(String path, String absolute, List<String> pictures, List<String> directories)
    {
        String relative = RelativeGen(absolute, path);
        for (int i = 0; i < pictures.size(); i++)
        {
            String s = pictures.get(i); //pl.: 001.jpg
            String html = HTMLMaker(s); //pl.: 001.html
            try
            {
                FileWriter fw = new FileWriter(path + "\\" + html); //kep.html letrehozasa (pl.: abszolut eleres + 001.html)
                StringBuilder sb = new StringBuilder();
                if (pictures.size() == 1)  //Ha csak egyetlen kep lenne a mappaban
                    sb.append(HTMLimg(absolute,html, s, html));
                else if(i == 0) //elso elem lekezelese
                    sb.append(HTMLimg(absolute,html, s, HTMLMaker(pictures.get(i+1))));
                else if (i == pictures.size() - 1) //utolso elem lekezelese
                    sb.append(HTMLimg(absolute,HTMLMaker(pictures.get(i-1)), s, html));
                else //barmilyen elem, ami nem elso vagy utolso
                    sb.append(HTMLimg(absolute,HTMLMaker(pictures.get(i-1)), s, HTMLMaker(pictures.get(i+1))));
                fw.write(sb.toString()); //fajlba iras
                fw.close();
            }
            catch (IOException e)
            {
               System.out.println("HIBA KEP.HTML LETREHOZASAKOR!");
               e.printStackTrace();
            }
        }
        StringBuilder sb = new StringBuilder(); //Index.html generalas
        String dirHTML = path + "\\index.html";
        try
        {
            FileWriter fw = new FileWriter(dirHTML);
            sb.append(HTMLindexFirst(absolute, relative));  //Az indexet tobb reszre kellett szednem, mert ellenoriznem kellett hogy van-e tobb mappa vagy kep
			if(!path.equals(absolute))
				sb.append("\t\t<a href=\"../index.html\">^^</a>\r\n");
            sb.append(HTMLDir(directories)); //Directories kiiratas
            sb.append(HTMLPic(pictures));  //Pictures kiiratas (ha van)
            fw.write(sb.toString()); //fajlba iras
            fw.close();
        }
        catch (IOException e)
        {
            System.out.println("HIBA INDEX.HTML LETREHOZASAKOR!");
            e.printStackTrace();
        }
    }
    public static String RelativeGen(String abs, String path)
    {
        if (abs.equals(path))
            return "";
        return path.replace(abs + "\\", "");
    }
    public String HTMLimg(String index, String prev, String name, String next) //A kepek elozo, jelenlegi es kovetkezo elemet keri be, es az abszolut eleresi utat a fo indexbe valo visszalepeshez
    {
        String s = String.format(
            "<html>\r\n" + //
            "<head>\r\n" + //
            "\t<title>%s</title>\r\n" + //
            "\t<style>\r\n" + //
            "\t\th1 {text-align: center;}\r\n" + //
            "\t\tdiv {text-align: center;}\r\n" + //
            "\t\timg {display: block; margin-left: auto; margin-right: auto; max-width: 95%%; max-height: 95%%;}\r\n" + //  //dupla szazalekjel hianya rendkivul sok fajdalmat okozott :D
            "\t</style>\r\n" + //
            "</head>\r\n" + //
            "<body>\r\n" + //
            "\t\t<a href=\"%s\"><h1>Index</h1></a>\r\n" + //
            "\t<hr>\r\n" + //
            "\t<div>\r\n" + //
            "\t\t<a href=\"index.html\">^^</a>\r\n" + //
            "\t\t<p><a href=\"%s\">Back</a>\r\n" + //
            "\t\t%s\r\n" + //
            "\t\t<a href=\"%s\">Next</a></p>\r\n" + //
            "\t</div>\r\n" + //
            "\t<hr>\r\n" + //
            "\t\t<a href=\"%s\"><img src=\"%s\" class=\"img\"></a>\r\n" + //
            "</body>\r\n" + //
            "</html>", name,index + "\\index.html",prev,name,next,next,name);
        return s;
    }
    public String HTMLindexFirst(String index, String name) //Mappa nevet keri be cimnek, es az abszolut elerest a fo indexbe valo visszalepeshez
    {
        String tmp = name.replace(name.substring(0, name.lastIndexOf("\\")+1), "");
        String s = String.format(
            "<html>\r\n" + //
            "<head>\r\n" + //
            "\t<title>%s</title>\r\n" + //
            "</head>\r\n" + //
            "<body>\r\n" + //
            "\t<a href=\"%s\"><h1>Index</h1></a>\r\n" + //
            "\t<hr>\r\n" + //
            "\t<h2><b>Directories</b></h2>\r\n", tmp, index + "\\index.html");
        return s;
    }
    public String HTMLDir(List<String> dir) //Az indexFirst-ben mar legeneraltam a Directories feliratot, itt mar nem kell
    {
        StringBuilder sb = new StringBuilder();
        String[] array = dir.toArray(new String[0]);
        for (String string : array)
            sb.append(String.format("\t<li><a href=\"%s\">%s</a></li>\n", string + "/index.html", string));
        return sb.toString();
    }
    public String HTMLPic(List<String> pic) //Mivel a Directories utan jon, így kulon le kell generaltatni
    {
        if (pic.isEmpty())
            return "</body>\n</html>";  //Ha ures akkor ne generalja le
        StringBuilder sb = new StringBuilder();
        sb.append("\t<hr>\n\t<h2><b>Pictures</b></h2>\n");
        String[] array = pic.toArray(new String[0]);
        for (String string : array)
            sb.append(String.format("\t\t<li><a href=\"%s\">%s</a></li>\n", string.substring(0, string.indexOf(".")) + ".html", string));
        sb.append("</body>\n</html>");
        return sb.toString();
    }
    private static String HTMLMaker(String s)
    {
        return s.substring(0,s.lastIndexOf(".")) + ".html"; //lusta voltam mindig beírni, szóval csak csináltam már egy metódust rá
    }
}