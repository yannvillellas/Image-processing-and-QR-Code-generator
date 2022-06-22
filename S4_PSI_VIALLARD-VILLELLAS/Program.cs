using System;
using System.IO;
using System.Diagnostics;

namespace S4_PSI_VIALLARD_VILLELLAS
{
    /// <summary>
    /// Interface permettant à l'utilisateur de choisir ses images, ses fonctions, permet d'afficher l'image et son header
    /// </summary>
    class Program
    {
        
        static void Main(string[] args)
        {
            MyImage image = new(Select());
            Settings(image);
            Functions(image);
        }
        /// <summary>
        /// Cette fonction affiche le menu qui permet de choisir l'image sur laquelle on va travailler  
        /// </summary>
        /// <returns></returns> retourne le nom du fichier
        static string Select()
        {
            string filename="";
            Console.WriteLine("Ecrire un numéro pour faire le choix de votre image.\n" +
                "0. Entrer un nom de fichier après avoir déposé votre image dans .../bin/Debug/net5.0\n" +
                "1. coco\n" +
                "2. lac\n" +
                "3. lena\n" +
                "4. test\n" +
                "5. newimage\n" +
                "6. Dessiner la fractale de Mandelbrot\n"+
                "7. Créer un QR code");
            int n = -1;
            do
            {
                Console.Write("> ");
                try { n = Convert.ToInt32(Console.ReadLine()); }
                catch { Console.WriteLine("Entrer un entier."); }
                if (n < 0 || n > 7) Console.WriteLine("Entrer un nombre valide.");
            } while (n < 0 || n > 7);
            switch (n)
            {
                case 0:
                    byte[] exist=null;
                    do
                    {
                        Console.WriteLine("\nSaisir le nom de l'image (sans écrire l'extension '.bmp').");
                        Console.Write("> ");
                        string name = Console.ReadLine();
                        filename = name + ".bmp";
                        try { exist = File.ReadAllBytes(filename); }
                        catch { Console.WriteLine("Saisir un nom d'image existant."); }
                    } while (exist == null) ;
                    break;
                case 1:
                    filename = "coco.bmp";
                    break;
                case 2:
                    filename = "lac.bmp";
                    break;
                case 3:
                    filename = "lena.bmp";
                    break;
                case 4:
                    filename = "test.bmp";
                    break;
                case 5:
                    filename = "newimage.bmp";
                    break;
                case 6:
                    MyImage image = new();
                    image.Fractale();
                    Open("newimage.bmp");
                    Environment.Exit(0);
                    break;
                case 7:
                    filename = "QRcode.bmp";
                    QR.CréerQRCode();
                    Open("QRcode.bmp");
                    Environment.Exit(0);
                    break;
            }
            Console.WriteLine("\nOuvrir cette image ? Ecrire o (oui) ou n (non).");
            Console.Write("> ");
            string s = Console.ReadLine();
            while (s != "o" && s != "n")
            {
                Console.WriteLine("Voulez vous ouvrir l'image ? Ecrire la lettre 'o' pour oui et la lettre 'n' pour non.");
                Console.Write("> ");
                s = Console.ReadLine();
            }
            bool open = s == "o";
            if (open) Open(filename);
            return filename;
        }
        /// <summary>
        /// Demande à l'utilisateur s'il veut afficher l'image ou ses informations ainsi que son header
        /// </summary>
        /// <param name="image"></param> Prend en entrée une image 
        static void Settings(MyImage image)
        {
            Console.WriteLine("\nAfficher les caractéristiques ? Ecrire o (oui) ou n (non).");
            Console.Write("> ");
            string s = Console.ReadLine();
            while (s != "o" && s != "n")
            {
                Console.WriteLine("Voulez vous afficher les caractéristiques ? Ecrire la lettre 'o' pour oui et la lettre 'n' pour non.");
                s = Console.ReadLine();
            }
            bool open = s == "o";
            if (open) Console.WriteLine(image.ToString());
        }
        /// <summary>
        /// Demande à l'utilisateur la fonction à utiliser sur l'image selectionnée
        /// </summary>
        /// <param name="image"></param> Prend une image en entrée
        static void Functions(MyImage image)
        {
            Console.WriteLine("\nEcrire le numéro de la fonction souhaitée.\n" +
                "0. Sortir\n" +
                "1. Nuance de gris\n" +
                "2. Noir et blanc\n" +
                "3. Effet miroir\n" +
                "4. Rotation\n" +
                "5. Agrandir\n" +
                "6. Rétrécir\n" +
                "7. Appliquer un filtre\n" + 
                "8. Coder une image dans une autre\n" +
                "9. Décoder une image\n" +
                "BONUS :\n" +
                "10. Inverser les couleurs\n" +
                "11. Couleurs primaires majoritaires\n" +
                "12. Coder un texte dans une image\n" + 
                "13. Décoder un texte dans une image");
            int n = -1;
            do
            {
                Console.Write("> ");
                try { n = Convert.ToInt32(Console.ReadLine()); }
                catch { Console.WriteLine("Ecrire un entier."); }
                if (n < 0 || n > 13) Console.WriteLine("Entrer un nombre valide.");
            } while (n < 0 || n > 13);
            switch (n)
            {
                case 0:
                    Environment.Exit(0);
                    break;
                case 1:
                    image.Nuance_Gris();
                    Open("newimage.bmp");
                    break;
                case 2:
                    image.Noir_Blanc();
                    Open("newimage.bmp");
                    break;
                case 3:
                    image.Effet_Mirroir();
                    Open("newimage.bmp");
                    break;
                case 4:
                    //Console.WriteLine("\nEcrire un angle multiple de 90. La rotation se fait dans le sens des aiguilles d'une montre.");
                    Console.WriteLine("\nEcrire un angle pour effectuer une rotation à l'image. La rotation se fait dans le sens des aiguilles d'une montre.");
                    Console.Write("> ");
                    int angle = Convert.ToInt32(Console.ReadLine());
                    if(angle%90 == 0) image.Rotation90(angle);
                    else image.Rotation(angle);
                    Open("newimage.bmp");
                    break;
                case 5:
                    Console.WriteLine("\nEcrire un coefficient entier pour agrandir l'image.");
                    Console.Write("> ");
                    int coefA = Convert.ToInt32(Console.ReadLine());
                    image.Agrandir(coefA);
                    Open("newimage.bmp");
                    break;
                case 6:
                    Console.WriteLine("\nEcrire un coefficient entier pour rétrécir l'image.");
                    Console.Write("> ");
                    int coefR = Convert.ToInt32(Console.ReadLine());
                    image.Retrecir(coefR);
                    Open("newimage.bmp");
                    break;
                case 7:
                    Console.WriteLine("\nEcrire le numéro du filtre souhaité.\n" +
                        "0. Test matrice identité\n" +
                        "1. Renforcement\n" +
                        "2. Flou\n" +
                        "3. Contour 1\n" +
                        "4. Contour 2\n" +
                        "5. Contour 3\n" +
                        "6. Repoussage");
                    int m = -1;
                    do
                    {
                        Console.Write("> ");
                        try { m = Convert.ToInt32(Console.ReadLine()); }
                        catch { Console.WriteLine("Ecrire un entier."); }
                        if (m < 0 || m > 6) Console.WriteLine("Entrer un nombre valide.");
                    } while (m < 0 || m > 6);
                    switch (m)
                    {
                        case 0:
                            image.Convolution(Kernel.Identite());
                            Open("newimage.bmp");
                            break;
                        case 1:
                            image.Convolution(Kernel.Renforcement());
                            Open("newimage.bmp");
                            break;
                        case 2:
                            image.Convolution(Kernel.Flou(), 0.11111111);
                            Open("newimage.bmp");
                            break;
                        case 3:
                            image.Convolution(Kernel.Contour1());
                            Open("newimage.bmp");
                            break;
                        case 4:
                            image.Convolution(Kernel.Contour2());
                            Open("newimage.bmp");
                            break;
                        case 5:
                            image.Convolution(Kernel.Contour3());
                            Open("newimage.bmp");
                            break;
                        case 6:
                            image.Convolution(Kernel.Repoussage());
                            Open("newimage.bmp");
                            break;
                    }
                    break;
                case 8:
                    string filename = "";
                    Console.WriteLine("\nEcrire un numéro pour faire le choix de votre image à cacher dans l'image principale. L'image à cacher doit être plus petite ou de même taille que l'image principale.\n" +
                        "0. Entrer un nom de fichier après avoir déposé votre image dans .../bin/Debug/net5.0\n" +
                        "1. coco\n" +
                        "2. lac\n" +
                        "3. lena\n" +
                        "4. test");
                    int p = -1;
                    do
                    {
                        Console.Write("> ");
                        try { p = Convert.ToInt32(Console.ReadLine()); }
                        catch { Console.WriteLine("Entrer un entier."); }
                        if (p < 0 || p > 5) Console.WriteLine("Entrer un nombre valide.");
                    } while (p < 0 || p > 4);
                    switch (p)
                    {
                        case 0:
                            byte[] exist = null;
                            do
                            {
                                Console.WriteLine("\nSaisir le nom de l'image (sans écrire l'extension '.bmp').");
                                Console.Write("> ");
                                string name = Console.ReadLine();
                                filename = name + ".bmp";
                                try { exist = File.ReadAllBytes(filename); }
                                catch { Console.WriteLine("Saisir un nom d'image existant."); }
                            } while (exist == null);
                            break;
                        case 1:
                            filename = "coco.bmp";
                            break;
                        case 2:
                            filename = "lac.bmp";
                            break;
                        case 3:
                            filename = "lena.bmp";
                            break;
                        case 4:
                            filename = "test.bmp";
                            break;
                    }
                    MyImage imageCache = new MyImage(filename);
                    if (imageCache.Hauteur > image.Hauteur || imageCache.Largeur > image.Largeur)
                    {
                        Console.WriteLine("L'image à cacher est plus grande que l'image principale. Il est impossible de cacher cette image dans la principale.");
                    }
                    else
                    {
                        image.Coder(imageCache);
                        Open("newimage.bmp");
                    }
                    break;
                case 9:
                    image.Decoder();
                    Open("newimage.bmp");
                    break;
                case 10:
                    image.Inverser_Couleurs();
                    Open("newimage.bmp");
                    break;
                case 11:
                    image.Couleur_Majoritaire();
                    Open("newimage.bmp");
                    break;
                case 12:
                    Console.WriteLine("\nEcrire le texte à cacher dans l'image.");
                    Console.Write("> ");
                    string txt = Console.ReadLine();
                    image.Coder_Texte(txt);
                    Open("newimage.bmp");
                    break;
                case 13:
                    Console.WriteLine("Le message caché dans cette image ce situe au début de cette chaine de caractère :\n");
                    Console.WriteLine(image.Decoder_Texte());
                    break;
            }
        }
        /// <summary>
        /// Ouvre une image à l'aide de son nom
        /// </summary>
        /// <param name="filename"></param> nom de l'image (avec.bmp à la fin)
        static void Open(string filename)
        {
            Process p = new();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = filename;
            p.Start();
        }
    }
}