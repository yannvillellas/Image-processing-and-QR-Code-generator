using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace S4_PSI_VIALLARD_VILLELLAS
{
    /// <summary>
    /// Class contenant les infos du QR Code 
    /// </summary>
    class QR
    {
        //le message que l'on souhaite transmettre avec un QR code
        private string contenu;
        // La version du QR code (1 ou 2)
        private int version;
        //l'image du QR code 
        private MyImage image;

        //accesseurs
        public string Contenu
        {
            get { return contenu; }
        }
        public int Version
        {
            get { return version; }
        }
        public MyImage Image
        {
            get { return image; }
        }
        /// <summary>
        /// Définie la version du QR code quand le contenu est en entré
        /// </summary>
        /// <param name="contenu"></param>message que l'on souhaite transmettre avec un QR code
        public QR(string contenu)
        {
            if (contenu.Length <= 25) { version = 1; }
            else if (contenu.Length <= 47 && contenu.Length >= 26) { version = 2; }
            else version = -1;
        }
        /// <summary>
        /// Renvoie un tableau de byte qui sert à la correction d'erreur du QR code et qui va venir completer la chaine binaire 
        /// </summary>
        /// <param name="ChaineBin"></param>Chaine binaire contenant le message, la taille, le mode et des fois des zéros pour completer(multiple de 8 ou les 4 zéros)
        /// <returns></returns>un tableau de valeurs renvoyé par l'algorithme de correction d'erreur 
        public static byte[] My_Encodage(string ChaineBin)
        {
            Encoding u8 = Encoding.UTF8;
            byte[] bytesa;
            string[] memTempo;
            byte[] result;
            if (ChaineBin.Length == 152)
            {
                bytesa = new byte[19];
                memTempo = new string[19];
                for (int i = 0; i < bytesa.Length; i++)
                {
                    memTempo[i] = Convert.ToString(ChaineBin[8 * i]) + Convert.ToString(ChaineBin[8 * i + 1]) + Convert.ToString(ChaineBin[8 * i + 2]) + Convert.ToString(ChaineBin[8 * i + 3]) + Convert.ToString(ChaineBin[8 * i + 4]) + Convert.ToString(ChaineBin[8 * i + 5]) + Convert.ToString(ChaineBin[8 * i + 6]) + Convert.ToString(ChaineBin[8 * i + 7]);
                    bytesa[i] = Convert.ToByte(Convert.ToInt32(Convert.ToString(memTempo[i]), 2));
                }
                result = ReedSolomonAlgorithm.Encode(bytesa, 7, ErrorCorrectionCodeType.QRCode);

            }
            else
            {
                bytesa = new byte[34];
                memTempo = new string[34];
                for (int i = 0; i < bytesa.Length; i++)
                {
                    memTempo[i] = Convert.ToString(ChaineBin[8 * i]) + Convert.ToString(ChaineBin[8 * i + 1]) + Convert.ToString(ChaineBin[8 * i + 2]) + Convert.ToString(ChaineBin[8 * i + 3]) + Convert.ToString(ChaineBin[8 * i + 4]) + Convert.ToString(ChaineBin[8 * i + 5]) + Convert.ToString(ChaineBin[8 * i + 6]) + Convert.ToString(ChaineBin[8 * i + 7]);
                    bytesa[i] = Convert.ToByte(Convert.ToInt32(Convert.ToString(memTempo[i]), 2));
                }
                result = ReedSolomonAlgorithm.Encode(bytesa, 10, ErrorCorrectionCodeType.QRCode);

            }
            return result;
        }
        /// <summary>
        /// Retourne la valeur alphanumerique du caractere en entrée
        /// </summary>
        /// <param name="text"></param>caractère dont on souhaite savoir la valeur alphanumérique 
        /// <returns></returns>valeur alphanum
        public static int GetAlphanumPosition(char text)
        {
            var alphanum = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:";
            int index = alphanum.IndexOf(char.ToUpper(text));
            return index;
        }
        /// <summary>
        /// Convertie un entier en chaine binaire de taille 11 bits ou 6 bits
        /// </summary>
        /// <param name="val"></param>valeur a transformer en binaire
        /// <param name="fin"></param>booléen, true s'il s'agit de la fin de la chaine binaire (si true alors chaine binaire sur 6 bits)
        /// <param name="imp"></param>true si le nombre de caractere de la chaine de caractere que l'on transforme en QR code est impaire
        /// <returns></returns>chaine binaire 
        public static string ConvertIntToBin(int val, bool fin, bool imp)
        {
            string bitval = Convert.ToString(val, 2);
            if (imp == true)
            {
                if (fin != true) { while (bitval.Length < 11) { bitval = "0" + bitval; } }
                else { while (bitval.Length < 6) { bitval = "0" + bitval; } }
            }
            else
            {
                while (bitval.Length < 11)
                { bitval = "0" + bitval; } 
            }
            return bitval;
        }
        /// <summary>
        /// Convertie le contenue (chaine de caractere que l'on transforme en QR code) en une liste d'entier, en decortiquant la chaine de caractere 2 par 2, 
        /// utilise la fonction GetAlphanumPosition()
        /// </summary>
        /// <param name="contenu"></param>chaine de caractere que l'on transforme en QR code
        /// <param name="imp"></param>true si le nombre de caractere de contenu est impaire
        /// <returns></returns>Liste d'entier composé de la somme de 45*valeurAlphanum du premier caractere + valeurAlphanum du second (s'il existe, sinon égale à valeurAlphanum)
        public static List<int> ConvertStringToInt(string contenu, bool imp)
        {
            List<int> listInt = new List<int>();
            List<string> ListStr = new List<string>(contenu.Length / 2 + 1);
            if (imp == true)
            {
                for (int i = 0; i < contenu.Length; i += 2)
                {
                    if (i + 1 < contenu.Length) { ListStr.Add(Convert.ToString(contenu[i]) + Convert.ToString(contenu[i + 1])); }
                    else { ListStr.Add(Convert.ToString(contenu[i])); }
                }
                for (int i = 0; i < ListStr.Count; i++)
                {
                    if (i + 1 < ListStr.Count) { listInt.Add(45 * GetAlphanumPosition(ListStr[i][0]) + GetAlphanumPosition(ListStr[i][1])); }
                    else { listInt.Add(GetAlphanumPosition(ListStr[i][0])); }
                }
            }
            else
            {
                for (int i = 0; i < contenu.Length; i += 2)
                {
                    ListStr.Add(Convert.ToString(contenu[i]) + Convert.ToString(contenu[i + 1]));
                }
                for (int i = 0; i < ListStr.Count; i++)
                {
                    listInt.Add(45 * GetAlphanumPosition(ListStr[i][0]) + GetAlphanumPosition(ListStr[i][1]));

                }
            }
            return listInt;
        }
        /// <summary>
        /// Fonction permettant de generer une chaine binaire complete à partir d'une chaine de caractere 
        /// utilise ConvertStringToInt(), ConvertIntToBin(), GetAlphanumPosition et My_Encodage()
        /// </summary>
        /// <param name="contenu"></param>chaine de caractere que l'on transforme en QR code
        /// <returns></returns>la chaine binaire complete sous forme de string composé de 0 et de 1
        public static string CréerChaineBinComplete(string contenu)
        {
            bool imp = false;//pair
            if (contenu.Length % 2 == 1) { imp = true; }//impair
            List<int> ListInt = ConvertStringToInt(contenu, imp);
            string chaine = "";
            for (int i = 0; i < ListInt.Count; i++)
            {
                chaine += ConvertIntToBin(ListInt[i], i == (ListInt.Count - 1), imp);
            }
            int c = 0;
            if (contenu.Length <= 25)
            {
                if (chaine.Length <= 19 * 8 - 13)
                {
                    while (chaine.Length < 19 * 8 - 13 && c < 4)//remplie a droite avec 4 zéros
                    {
                        chaine += "0";
                        c++;
                    }
                }
            }
            else
            {
                if (chaine.Length <= 34 * 8 - 13)
                {
                    while (chaine.Length < 34 * 8 - 13 && c < 4)//remplie a droite avec 4 zéros
                    {
                        chaine += "0";
                        c++;
                    }
                }
            }
            /*if (chaine.Length < contenu.Length * 8 - 4)
            {
                while (chaine.Length < contenu.Length * 8 - 4 && c < 4)//remplie a droite avec 4 zéros
                {
                    chaine += "0";
                    c++;
                }
            }
            else
            {
                for (int i = 0; i < chaine.Length - contenu.Length; i++)
                {
                    chaine += "0";
                }
            }*/
            string taille = Convert.ToString(contenu.Length, 2); ;
            while (taille.Length < 9) { taille = "0" + taille; }
            chaine = "0010" + taille + chaine;
            while (chaine.Length % 8 != 0) { chaine += "0"; }
            if (contenu.Length <= 25)
            {
                while (chaine.Length < 152)
                {
                    chaine += "11101100";
                    if (chaine.Length < 152) { chaine += "00010001"; }
                }
            }
            else
            {
                while (chaine.Length < 272)
                {
                    chaine += "11101100";
                    if (chaine.Length < 272) { chaine += "00010001"; }
                }
            }
            byte[] Encodage = My_Encodage(chaine);
            int[] EncodageInt = new int[Encodage.Length];
            string[] bitval = new string[Encodage.Length];
            for (int i = 0; i < EncodageInt.Length; i++)
            {
                EncodageInt[i] = Convert.ToInt32(Encodage[i]);
                bitval[i] = Convert.ToString(EncodageInt[i], 2);
                while (bitval[i].Length < 8) { bitval[i] = "0" + bitval[i]; }
            }
            for (int i = 0; i < Encodage.Length; i++)
            {
                chaine += bitval[i];
            }
            return chaine;
        }
        /// <summary>
        /// Permet de créer un Qr code, demande à l'utilisateur de rentrer une chaine de caractere conforme, utilise ensuite CréerChaineBinComplete()
        /// pour créer la chaine et initialise une image (ligne 265) avec cette chaine
        /// </summary>
        public static void CréerQRCode()
        {
            Console.WriteLine("\nVeuillez rentrer un mot ou un chaine de mots inférieure à 47 caractères :");
            string chaine_de_caractere = "";
            bool Caractère_interdit = false;
            do
            {
                Caractère_interdit = false;
                Console.Write("> ");
                try { chaine_de_caractere = Convert.ToString(Console.ReadLine()); }
                catch { Console.WriteLine("Bizarre"); }
                for (int i = 0; i < chaine_de_caractere.Length && !Caractère_interdit; i++)
                { if (GetAlphanumPosition(chaine_de_caractere[i]) == -1) Caractère_interdit = true; }
                if (Caractère_interdit == true || chaine_de_caractere.Length > 47) { Console.WriteLine("Entrez une chaine de caractères valide ou plus courte (47 caractères max), les caractères doivent tous apartenir à (0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $% *+-./:)"); }
            } while (chaine_de_caractere == "" || chaine_de_caractere.Length > 47 || Caractère_interdit == true);
            int version = 0;
            if (chaine_de_caractere.Length <= 25) { version = 1; }
            else if (chaine_de_caractere.Length <= 47 && chaine_de_caractere.Length >= 26) { version = 2; }
            string chaine_binaire = CréerChaineBinComplete(chaine_de_caractere);
            MyImage QRCode = new MyImage(version, chaine_de_caractere);
        }
    }
}
