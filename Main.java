import java.io.File;

public class Main
{
    public static void main(String[] args)
    {
        if ((args.length < 1 || (args.length > 1) && (!args[1].equals("-c"))) || (args.length == 1 && (args[0].equals("-h") || args[0].equals("-help")))) //Atirtam a hibauzenet helyett egy Help-re, igy kicsit baratibb
        {
            System.out.println("HASZNALATI UTMUTATO:\n\nHa letre szeretnenk hozni a HTML fajlokat:\n\tjava Main.java <mappa eleresi utvonala>\n\nHa torolni szeretnenk a HTML fajlokat:\n\tjava Main.java <mappa eleresi utvonala> <-c>");
            return;
        }
        if (args.length == 1) //A HTML Generator meghivasa
        {
            File f = new File(args[0]);
            if (f.isDirectory() || f.exists()) //Ellenorzi, hogy letezik-e a mappa amit megadtunk
            {
                HTMLGenerator valami = new HTMLGenerator(args[0]);
                valami.walk(args[0]);
            }
            else
            {
                System.out.println("HIBA! Adj meg egy letezo eleresi utat!");
                return;
            }
        }
        if (args.length == 2 && args[1].equals("-c")) //Cleaner meghivasa
        {
			File f = new File(args[0]);
			if (f.isDirectory() || f.exists()) //Ellenorzi, hogy letezik-e a mappa amit megadtunk
            {
                cleaner(args[0]);
				System.out.println("HTML fajlok torolve!");
				return;
            }
            else
            {
                System.out.println("HIBA! Adj meg egy letezo eleresi utat!");
                return;
            }
            
        }    
    }
    public static void cleaner(String path) //Cleaner metodus
    {
        File root = new File(path);
        File[] list = root.listFiles();
        if (list == null) return;
        for (File f : list) 
        {
            if (f.isDirectory())
                cleaner(f.getAbsolutePath());
            else if (f.toString().endsWith(".html"))
                f.delete();
        }
    }
}
