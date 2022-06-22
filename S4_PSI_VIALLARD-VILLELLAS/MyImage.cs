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
    /// Class contenant toute les infos d'une image, permetant leurs initialisation/création et leurs manipulations
    /// </summary>
    class MyImage
    {
        // Tous les parametres d'une image
        #region ATTRIBUTS
        //son nom
        private string nom;
        //son type (generalement Bitmap "BM")
        private string type;
        //la taille de son fichier
        private int taille;
        //la taille du header : 54
        private int offset;
        //Largeur de l'image (en pixel)
        private int largeur;
        //hauteur de l'image (en pixel)
        private int hauteur;
        // nombre de bits par couleur (24, 8*3 (8 par couleur))
        private int nombreBitsCouleur;
        //tableau contenant les données du fichier de l'image
        private byte[] myfile;
        //tableau contenant les données du header de l'image
        private byte[] header = new byte[54];
        // uniquement pour les QR code, matrice de 21*21 ou 25*25 de int permetant de simplifier l'assignation de valeur (0 ou 1)
        private int[,] valRGB;
        //booléen permetant de savoir si l'image est un QR code ou non
        private bool QRcode = true;
        //Matrice de pixel représentant l'image 
        private Pixel[,] image;
        #endregion
        // L'ensemble des fonctions permetant la création d'une image
        #region CONSTRUCTEURS
        /// <summary>
        /// création d'une image à partir de rien : dessin
        /// </summary>
        public MyImage() 
        {
        }
        /// <summary>
        /// Création d'une image à partir d'un fichier "nom"
        /// </summary>
        /// <param name="nom"></param>Le nom du fichier contenant l'image
        public MyImage(string nom)
        {
            myfile = File.ReadAllBytes(nom);
            this.nom = nom;
            //header
            for (int i = 0; i < 54; i++)
            {
                header[i] = myfile[i];
            }
            //type image

            /* TYPE IMAGE EN INT. 
            byte[] tabType = { myfile[0], myfile[1] };
            type = Convertir_Endian_To_Int(tabType);

            CI DESSOUS CONVERTIT EN STR
            */
            for (int i = 0; i < 2; i++)
            {
                if (myfile[i] == 66) { type += "B"; }
                if (myfile[i] == 77) { type += "M"; }
            }
            //taille fichier
            byte[] tabTaille = { myfile[2], myfile[3], myfile[4], myfile[5] };
            taille = Convertir_Endian_To_Int(tabTaille);
            //taille offset
            byte[] tabOffset = { myfile[10], myfile[11], myfile[12], myfile[13] };
            offset = Convertir_Endian_To_Int(tabOffset);
            //largeur
            byte[] tabLargeur = { myfile[18], myfile[19], myfile[20], myfile[21] };
            largeur = Convertir_Endian_To_Int(tabLargeur);
            //hauteur
            byte[] tabHauteur = { myfile[22], myfile[23], myfile[24], myfile[25] };
            hauteur = Convertir_Endian_To_Int(tabHauteur);
            //nombre bits
            byte[] tabNombreBitsCouleur = { myfile[28], myfile[29] };
            nombreBitsCouleur = Convertir_Endian_To_Int(tabNombreBitsCouleur);
            //image --> matrice de Pixels
            image = new Pixel[hauteur, largeur];
            int index = 54;
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    image[i, j] = new Pixel(myfile[index], myfile[index + 1], myfile[index + 2]);
                    index += 3;
                }
            }
        }
        /// <summary>
        /// Uniquement pour les QR code, créer une image à partir d'une chaine binaire
        /// </summary>
        /// <param name="type"></param>version du QR code (1 ou 2)
        /// <param name="contenu"></param>chaine binaire que l'on va poser sur le QR code
        public MyImage(int type,string contenu) 
        {
            string chaine_binaire = QR.CréerChaineBinComplete(contenu);
            if (type == 1)
            {
                this.nom = "QRcode.bmp";
                this.type = "BM";
                this.taille = 21 * 3 * 21 * 3 + 54;
                this.offset = 54;
                this.largeur = 63;
                this.hauteur = 63;
                this.nombreBitsCouleur = 24;
                this.header = new byte[54] { 66, 77, 185, 46, 0, 0, 0, 0, 0, 0, 54, 0, 0, 0, 40, 0, 0, 0, 63, 0, 0, 0, 63, 0, 0, 0, 1, 0, 24, 0, 0, 0, 0, 0, 131, 46, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                this.valRGB = new int[21, 21] {{1,1,1,1,1,1,1,2,2,0,0,0,0,2,1,1,1,1,1,1,1 },//1 pour noir non modifiable, 2 pour blanc non modifiable,3 pour niveau de coorecteur et masque
                                              { 1,2,2,2,2,2,1,2,2,0,0,0,0,2,1,2,2,2,2,2,1 },
                                              { 1,2,1,1,1,2,1,2,1,0,0,0,0,2,1,2,1,1,1,2,1 },
                                              { 1,2,1,1,1,2,1,2,2,0,0,0,0,2,1,2,1,1,1,2,1 },
                                              { 1,2,1,1,1,2,1,2,2,0,0,0,0,2,1,2,1,1,1,2,1 },
                                              { 1,2,2,2,2,2,1,2,2,0,0,0,0,2,1,2,2,2,2,2,1 },
                                              { 1,1,1,1,1,1,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1 },
                                              { 2,2,2,2,2,2,2,2,1,0,0,0,0,2,2,2,2,2,2,2,2 },
                                              { 1,1,1,2,1,1,1,1,1,0,0,0,0,1,1,2,2,2,1,2,2 },// masque et niveau encodage (encodage L et masque 0) 111011111000100 ou ici 111211111222122
                                              { 0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                                              { 0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                                              { 0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                                              { 0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                                              { 2,2,2,2,2,2,2,2,1,0,0,0,0,0,0,0,0,0,0,0,0 },
                                              { 1,1,1,1,1,1,1,2,1,0,0,0,0,0,0,0,0,0,0,0,0 },
                                              { 1,2,2,2,2,2,1,2,1,0,0,0,0,0,0,0,0,0,0,0,0 },
                                              { 1,2,1,1,1,2,1,2,1,0,0,0,0,0,0,0,0,0,0,0,0 },
                                              { 1,2,1,1,1,2,1,2,2,0,0,0,0,0,0,0,0,0,0,0,0 },
                                              { 1,2,1,1,1,2,1,2,1,0,0,0,0,0,0,0,0,0,0,0,0 },
                                              { 1,2,2,2,2,2,1,2,1,0,0,0,0,0,0,0,0,0,0,0,0 },
                                              { 1,1,1,1,1,1,1,2,1,0,0,0,0,0,0,0,0,0,0,0,0}};
                Pixel[,] image1 = new Pixel[21 * 3, 21 * 3];
                //rectangle en bas a droite
                for (int i = 0; i < 4; i++)// 8 de largeur mais on mutliplie par 2 donc 8/2=4 (car gauche droite)
                {
                    for (int j = 0; j < 12; j++)//rectangle en bas a droite de 12 (longeur) par 8 (largeur)
                    {
                        for (int k = 0; k < 2; k++)//pour faire droite gauche 
                        {
                            if (i % 2 == 0)//colonne 21,20 et 16,17
                            {
                                valRGB[20 - j, 20 - k - 2 * i] = chaine_binaire[k + 2 * j + 24 * i];//21-1-j 
                                if ((20 - j + 20 - k - 2 * i) % 2 == 0)//on applique le masque 0 (quadrillage) ((colonne+ligne) % 2 == 0)
                                {
                                    if (chaine_binaire[2 * j + 24 * i + k] == '0') 
                                    {
                                        valRGB[20 - j, 20 - k - 2 * i] = 1;
                                    }
                                    else
                                    {
                                        valRGB[20 - j, 20 - k - 2 * i] = 0;
                                    }
                                }
                            }
                            else //colonne 19,18 et 14,15
                            {
                                valRGB[9 + j, 20 - k - 2 * i] = chaine_binaire[2 * j + 24 * i + k];
                                if ((9 + j + 20 - k - 2 * i) % 2 == 0) //Masque, même principe
                                {
                                    if (chaine_binaire[2 * j + 24 * i + k] == '0') 
                                    {
                                        valRGB[9 + j, 20 - k - 2 * i] = 1;
                                    }
                                    else
                                    {
                                        valRGB[9 + j, 20 - k - 2 * i] = 0;
                                    }
                                }
                            }
                        }
                    }
                }
                //colonne 12, 13 jusqu'au séparateurs du milieu
                for (int i = 0; i < 14; i++) //de 96 a 124 bit (+28) 
                {
                    for (int j = 0; j < 2; j++)// gauche droite
                    {
                        valRGB[20 - i, 21 - 9 - j] = chaine_binaire[96 + j + 2 * i];
                        if ((20 - i + 21 - 9 - j) % 2 == 0)//Mask
                        {
                            if (chaine_binaire[96 + j + 2 * i] == '0')
                            {
                                valRGB[20 - i, 21 - 9 - j] = 1;
                            }
                            else
                            {
                                valRGB[20 - i, 21 - 9 - j] = 0;
                            }
                        }
                    }
                }
                // rectangle 4 par 6 milieu en haut (au dessus du separateur)  124+6*4=148 bits
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            //colonne 12, 13 au dessus du separateur jusqu'au haut du QR code
                            if (i % 2 == 0)
                            {
                                valRGB[5 - j, 12 - 2 * i - k] = chaine_binaire[2 * j + k + 12 * i + 124];
                                {
                                    if ((5 - j + 12 - 2 * i - k) % 2 == 0)//Rebelote le masque
                                    {
                                        if (chaine_binaire[2 * j + k + 12 * i + 124] == '0')
                                        {
                                            valRGB[5 - j, 12 - 2 * i - k] = 1;
                                        }
                                        else { valRGB[5 - j, 12 - 2 * i - k] = 0; }
                                    }
                                }
                            }
                            //colonne 10, 11 au dessus du separateur jusqu'au haut du QR code
                            else
                            {
                                valRGB[j, 12 - 2 * i - k] = chaine_binaire[2 * j + k + 12 * i + 124];
                                {
                                    if ((j + 12 - 2 * i - k) % 2 == 0)//Rebelote le masque
                                    {
                                        if (chaine_binaire[2 * j + k + 12 * i + 124] == '0')
                                        {
                                            valRGB[j, 12 - 2 * i - k] = 1;
                                        }
                                        else { valRGB[j, 12 - 2 * i - k] = 0; }
                                    }
                                }
                            }
                        }
                    }
                }
                //colonne 10,11 du separateur jusqu'au bas du QR code
                //2*14=28
                for (int i = 0; i < 14; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        valRGB[7 + i, 10 - j] = chaine_binaire[j + 2 * i + 148];
                        if ((7 + i + 10 - j) % 2 == 0) 
                        {
                            if (chaine_binaire[j + 2 * i + 148] == '0')
                            {
                                valRGB[7 + i, 10 - j] = 1;
                            }
                            else { valRGB[7 + i, 10 - j] = 0; }
                        }
                    }
                }
                //colonne 8,9 a droite du separateur entre les deux endroit ou son placé la chaine de 14 bits pour l'encodage et le masque (111011111000100)
                //4*2=8
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        valRGB[12 - i, 8 - j] = chaine_binaire[j + 2 * i + 176];
                        if ((12 - i + 8 - j) % 2 == 0) //MAASQUE
                        {
                            if (chaine_binaire[j + 2 * i + 176] == '0')
                            {
                                valRGB[12 - i, 8 - j] = 1;
                            }
                            else { valRGB[12 - i, 8 - j] = 0; }
                        }
                    }
                }
                //colonne 1 à 6
                //rectangle 4 * 6 =24
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            if (i % 2 == 0)
                            {
                                //colonne 1,2 et 5,6
                                valRGB[9 + j, -2 * i + 5 - k] = chaine_binaire[2 * j + 8 * i + 184 + k];
                                if ((9 + j - 2 * i + 5 - k) % 2 == 0)
                                {
                                    if (chaine_binaire[2 * j + 8 * i + 184 + k] == '0')
                                    {
                                        valRGB[9 + j, -2 * i + 5 - k] = 1;
                                    }
                                    else { valRGB[9 + j, -2 * i + 5 - k] = 0; }
                                }
                            }
                            else
                            {
                                //colonne 3 et 4
                                valRGB[12 - j, -2 * i + 5 - k] = chaine_binaire[2 * j + 8 * i + 184 + k];
                                if ((12 - j - 2 * i + 5 - k) % 2 == 0)
                                {
                                    if (chaine_binaire[2 * j + 8 * i + 184 + k] == '0')
                                    {
                                        valRGB[12 - j, -2 * i + 5 - k] = 1;
                                    }
                                    else { valRGB[12 - j, -2 * i + 5 - k] = 0; }
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < 21; i++) //remplir les pixels de l'image
                {
                    for (int j = 0; j < 21; j++)
                    {
                        if (valRGB[i, j] == 48 || valRGB[i, j] == 49)//Car convert.ToInt32 donne la veleur ASCII de 0 ou 1, on reviens donc à la valeur normal
                        {
                            valRGB[i, j] -= 48;
                        }
                        if (valRGB[i, j] == 1)
                        {
                            image1[(20 - i) * 3, j * 3] = new Pixel(0, 0, 0);
                            image1[(20 - i) * 3, j * 3 + 1] = new Pixel(0, 0, 0);
                            image1[(20 - i) * 3, j * 3 + 2] = new Pixel(0, 0, 0);
                            image1[(20 - i) * 3 + 1, j * 3] = new Pixel(0, 0, 0);
                            image1[(20 - i) * 3 + 1, j * 3 + 1] = new Pixel(0, 0, 0);
                            image1[(20 - i) * 3 + 1, j * 3 + 2] = new Pixel(0, 0, 0);
                            image1[(20 - i) * 3 + 2, j * 3] = new Pixel(0, 0, 0);
                            image1[(20 - i) * 3 + 2, j * 3 + 1] = new Pixel(0, 0, 0);
                            image1[(20 - i) * 3 + 2, j * 3 + 2] = new Pixel(0, 0, 0);
                        }
                        if (valRGB[i, j] == 2 || valRGB[i, j] == 0)
                        {
                            image1[(20 - i) * 3, j * 3] = new Pixel(255, 255, 255);
                            image1[(20 - i) * 3, j * 3 + 1] = new Pixel(255, 255, 255);
                            image1[(20 - i) * 3, j * 3 + 2] = new Pixel(255, 255, 255);
                            image1[(20 - i) * 3 + 1, j * 3] = new Pixel(255, 255, 255);
                            image1[(20 - i) * 3 + 1, j * 3 + 1] = new Pixel(255, 255, 255);
                            image1[(20 - i) * 3 + 1, j * 3 + 2] = new Pixel(255, 255, 255);
                            image1[(20 - i) * 3 + 2, j * 3] = new Pixel(255, 255, 255);
                            image1[(20 - i) * 3 + 2, j * 3 + 1] = new Pixel(255, 255, 255);
                            image1[(20 - i) * 3 + 2, j * 3 + 2] = new Pixel(255, 255, 255);
                        }
                        //On peut modifier les 1 et 0 par 5 et 6 pour voir visuelement sur le QR code ou sont placé les bits
                        if (valRGB[i, j] == 6)//test bleu
                        {
                            image1[(20 - i) * 3, j * 3] = new Pixel(240, 0, 0);
                            image1[(20 - i) * 3, j * 3 + 1] = new Pixel(240, 0, 0);
                            image1[(20 - i) * 3, j * 3 + 2] = new Pixel(240, 0, 0);
                            image1[(20 - i) * 3 + 1, j * 3] = new Pixel(240, 0, 0);
                            image1[(20 - i) * 3 + 1, j * 3 + 1] = new Pixel(240, 0, 0);
                            image1[(20 - i) * 3 + 1, j * 3 + 2] = new Pixel(240, 0, 0);
                            image1[(20 - i) * 3 + 2, j * 3] = new Pixel(240, 0, 0);
                            image1[(20 - i) * 3 + 2, j * 3 + 1] = new Pixel(240, 0, 0);
                            image1[(20 - i) * 3 + 2, j * 3 + 2] = new Pixel(240, 0, 0);
                        }
                        if (valRGB[i, j] == 5)//test vert
                        {
                            image1[(20 - i) * 3, j * 3] = new Pixel(0, 250, 0);
                            image1[(20 - i) * 3, j * 3 + 1] = new Pixel(0, 250, 0);
                            image1[(20 - i) * 3, j * 3 + 2] = new Pixel(0, 250, 0);
                            image1[(20 - i) * 3 + 1, j * 3] = new Pixel(0, 250, 0);
                            image1[(20 - i) * 3 + 1, j * 3 + 1] = new Pixel(0, 250, 0);
                            image1[(20 - i) * 3 + 1, j * 3 + 2] = new Pixel(0, 250, 0);
                            image1[(20 - i) * 3 + 2, j * 3] = new Pixel(0, 250, 0);
                            image1[(20 - i) * 3 + 2, j * 3 + 1] = new Pixel(0, 250, 0);
                            image1[(20 - i) * 3 + 2, j * 3 + 2] = new Pixel(0, 250, 0);
                        }
                    }
                }
                this.image = image1;
                From_Image_To_File(QRcode);
            }
            else if (type == 2)
            {
                this.nom = "QRcode.bmp";
                this.type = "BM";
                this.taille = 25 * 3 * 25 * 3 + 54;
                this.offset = 54;
                this.largeur = 75;
                this.hauteur = 75;
                this.nombreBitsCouleur = 24;//pas sur 
                this.header = new byte[54] { 66, 77, 135, 37, 2, 0, 0, 0, 0, 0, 54, 0, 0, 0, 40, 0, 0, 0, 75, 0, 0, 0, 75, 0, 0, 0, 1, 0, 24, 0, 0, 0, 0, 0, 81, 37, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                this.valRGB = new int[25, 25]  {{1,1,1,1,1,1,1,2,2,0,0,0,0,0,0,0,0,2,1,1,1,1,1,1,1 },//1 pour noir non modifiable, 2 pour blanc non modifiable,3 pour niveau de coorecteur et masque
                                               { 1,2,2,2,2,2,1,2,2,0,0,0,0,0,0,0,0,2,1,2,2,2,2,2,1 },
                                               { 1,2,1,1,1,2,1,2,1,0,0,0,0,0,0,0,0,2,1,2,1,1,1,2,1 },
                                               { 1,2,1,1,1,2,1,2,2,0,0,0,0,0,0,0,0,2,1,2,1,1,1,2,1 },
                                               { 1,2,1,1,1,2,1,2,2,0,0,0,0,0,0,0,0,2,1,2,1,1,1,2,1 },
                                               { 1,2,2,2,2,2,1,2,2,0,0,0,0,0,0,0,0,2,1,2,2,2,2,2,1 },
                                               { 1,1,1,1,1,1,1,2,1,2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1 },
                                               { 2,2,2,2,2,2,2,2,1,0,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2 },
                                               { 1,1,1,2,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,2,2,2,1,2,2 },// masque et niveau encodage 111011111000100 ou ici 111211111222122
                                               { 0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                                               { 0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                                               { 0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                                               { 0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                                               { 0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                                               { 0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                                               { 0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                                               { 0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0 },
                                               { 2,2,2,2,2,2,2,2,1,0,0,0,0,0,0,0,1,2,2,2,1,0,0,0,0 },
                                               { 1,1,1,1,1,1,1,2,1,0,0,0,0,0,0,0,1,2,1,2,1,0,0,0,0 },
                                               { 1,2,2,2,2,2,1,2,1,0,0,0,0,0,0,0,1,2,2,2,1,0,0,0,0 },
                                               { 1,2,1,1,1,2,1,2,1,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0 },
                                               { 1,2,1,1,1,2,1,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                                               { 1,2,1,1,1,2,1,2,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                                               { 1,2,2,2,2,2,1,2,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
                                               { 1,1,1,1,1,1,1,2,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}};
                Pixel[,] image1 = new Pixel[25 * 3, 25 * 3];
                //Rectangle de 4 par 16 en bas a droite 
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        for (int k = 0; k < 2; k++)//pour faire droite gauche 
                        {
                            if (i % 2 == 0)//colonne 25, 24
                            {
                                valRGB[24 - j, 24 - k - 2 * i] = chaine_binaire[k + 2 * j + 24 * i];//25-1-j  
                                if ((24 - j + 24 - k - 2 * i) % 2 == 0)//on applique le masque 0 (quadrillage) ((colonne+ligne) % 2 == 0)
                                {
                                    if (chaine_binaire[2 * j + 32 * i + k] == '0')
                                    {
                                        valRGB[24 - j, 24 - k - 2 * i] = 1;
                                    }
                                    else
                                    {
                                        valRGB[24 - j, 24 - k - 2 * i] = 0;
                                    }
                                }
                            }
                            else //colonne 23, 22
                            {
                                valRGB[25 - 16 + j, 24 - k - 2 * i] = chaine_binaire[2 * j + 32 * i + k];
                                if ((25 - 16 + j + 24 - k - 2 * i) % 2 == 0) //Masque, même principe
                                {
                                    if (chaine_binaire[2 * j + 32 * i + k] == '0') //'0'
                                    {
                                        valRGB[25 - 16 + j, 24 - k - 2 * i] = 1;
                                    }
                                    else
                                    {
                                        valRGB[25 - 16 + j, 24 - k - 2 * i] = 0;
                                    }
                                }
                            }
                        }
                    }
                }
                //colonne 20,21 jusqu'au modèle d'alignement
                //64+8=72
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 2; j++)// gauche droite
                    {
                        valRGB[24 - i, 25 - 5 - j] = chaine_binaire[64 + j + 2 * i];
                        if ((24 - i + 25 - 5 - j) % 2 == 0)//Mask
                        {
                            if (chaine_binaire[64 + j + 2 * i] == '0')
                            {
                                valRGB[24 - i, 25 - 5 - j] = 1;
                            }
                            else
                            {
                                valRGB[24 - i, 25 - 5 - j] = 0;
                            }
                        }
                    }
                }
                //colonne 18 à 21 au dessus du modèle d'alignement
                //72+28=100
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        for (int k = 0; k < 2; k++)//pour faire droite gauche 
                        {
                            if (i % 2 == 0)//colonne 20, 21
                            {
                                valRGB[25 - 10 - j, 25 - 5 - k - 2 * i] = chaine_binaire[72 + k + 2 * j + 14 * i];
                                if ((25 - 10 - j + 25 - 5 - k - 2 * i) % 2 == 0)//on applique le masque 0 (quadrillage) ((colonne+ligne) % 2 == 0)
                                {
                                    if (chaine_binaire[72 + k + 2 * j + 14 * i] == '0')
                                    {
                                        valRGB[25 - 10 - j, 25 - 5 - k - 2 * i] = 1;
                                    }
                                    else
                                    {
                                        valRGB[25 - 10 - j, 25 - 5 - k - 2 * i] = 0;
                                    }
                                }
                            }
                            else //colonne 18,19
                            {
                                valRGB[25 - 16 + j, 25 - 5 - k - 2 * i] = chaine_binaire[72 + 2 * j + 14 * i + k];
                                if ((25 - 16 + j + 25 - 5 - k - 2 * i) % 2 == 0) //Masque, tjrs même principe
                                {
                                    if (chaine_binaire[72 + 2 * j + 14 * i + k] == '0') 
                                    {
                                        valRGB[25 - 16 + j, 25 - 5 - k - 2 * i] = 1;
                                    }
                                    else
                                    {
                                        valRGB[25 - 16 + j, 25 - 5 - k - 2 * i] = 0;
                                    }
                                }
                            }
                        }
                    }
                }
                //colonne 18,19 en dessus du modèle d'alignement jusqu(a bas du QR code
                //100+8=108
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 2; j++)// gauche droite
                    {
                        valRGB[25 - 4 + i, 25 - 7 - j] = chaine_binaire[100 + j + 2 * i];
                        if ((25 - 4 - i + 25 - 7 - j) % 2 == 0)//Mask
                        {
                            if (chaine_binaire[100 + j + 2 * i] == '0')
                            {
                                valRGB[25 - 4 + i, 25 - 7 - j] = 1;
                            }
                            else
                            {
                                valRGB[25 - 4 + i, 25 - 7 - j] = 0;
                            }
                        }
                    }
                }
                //colonne 16, 17 en dessous du modèle d'align
                //108+8=116
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 2; j++)// gauche droite
                    {
                        valRGB[24 - i, 25 - 9 - j] = chaine_binaire[108 + j + 2 * i];
                        if ((24 - i + 25 - 9 - j) % 2 == 0)//Mask
                        {
                            if (chaine_binaire[108 + j + 2 * i] == '0')
                            {
                                valRGB[24 - i, 25 - 9 - j] = 1;
                            }
                            else
                            {
                                valRGB[24 - i, 25 - 9 - j] = 0;
                            }
                        }
                    }
                }
                //colonne 16 à gauche du modèle d'align
                //116+5=21
                for (int i = 0; i < 5; i++)
                {
                    valRGB[25 - 5 - i, 25 - 10] = chaine_binaire[116 + i];
                    if ((25 - 5 - i + 25 - 10) % 2 == 0)//Mask
                    {
                        if (chaine_binaire[116 + i] == '0')
                        {
                            valRGB[25 - 5 - i, 25 - 10] = 1;
                        }
                        else
                        {
                            valRGB[25 - 5 - i, 25 - 10] = 0;
                        }
                    }
                }
                // colonne 16,17 au dessus du motif d'align jusqu'au séparateur
                //121+18=139
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 2; j++)// gauche droite
                    {
                        valRGB[25 - 10 - i, 25 - 9 - j] = chaine_binaire[121 + j + 2 * i];
                        if ((25 - 10 - i + 25 - 9 - j) % 2 == 0)//Mask
                        {
                            if (chaine_binaire[121 + j + 2 * i] == '0')
                            {
                                valRGB[25 - 10 - i, 25 - 9 - j] = 1;
                            }
                            else
                            {
                                valRGB[25 - 10 - i, 25 - 9 - j] = 0;
                            }
                        }
                    }
                }
                //colonne 14,15,16 et 17
                //entre le haut du QR code et le séparateur 
                //139+24=163
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            if (i % 2 == 0)//colonne 16, 17
                            {
                                valRGB[25 - 20 - j, 25 - 9 - k - 2 * i] = chaine_binaire[139 + 2 * j + 12 * i + k];
                                if ((25 - 20 - j + 25 - 9 - k - 2 * i) % 2 == 0)//on applique le masque 0 (quadrillage) ((colonne+ligne) % 2 == 0)
                                {
                                    if (chaine_binaire[139 + 2 * j + 12 * i + k] == '0')
                                    {
                                        valRGB[25 - 20 - j, 25 - 9 - k - 2 * i] = 1;
                                    }
                                    else
                                    {
                                        valRGB[25 - 20 - j, 25 - 9 - k - 2 * i] = 0;
                                    }
                                }
                            }
                            else //colonne 14,15
                            {
                                valRGB[j, 25 - 9 - k - 2 * i] = chaine_binaire[139 + 2 * j + 12 * i + k];
                                if ((j + 25 - 9 - k - 2 * i) % 2 == 0) //Masque, même principe
                                {
                                    if (chaine_binaire[139 + 2 * j + 12 * i + k] == '0') 
                                    {
                                        valRGB[j, 25 - 9 - k - 2 * i] = 1;
                                    }
                                    else
                                    {
                                        valRGB[j, 25 - 9 - k - 2 * i] = 0;
                                    }
                                }
                            }
                        }
                    }
                }
                //colonne 12,13,14 et 15 du séparateur au bas du QR code
                //163+4*18=235
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 18; j++)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            if (i % 2 == 0)//colonne 14,15
                            {
                                valRGB[25 - 18 + j, 25 - 11 - k - 2 * i] = chaine_binaire[163 + 2 * j + 36 * i + k];//7=25-18
                                if ((25 - 18 + j + 25 - 11 - k - 2 * i) % 2 == 0)//on applique le masque 0 (quadrillage) ((colonne+ligne) % 2 == 0)
                                {
                                    if (chaine_binaire[163 + 2 * j + 36 * i + k] == '0')
                                    {
                                        valRGB[25 - 18 + j, 25 - 11 - k - 2 * i] = 1;
                                    }
                                    else
                                    {
                                        valRGB[25 - 18 + j, 25 - 11 - k - 2 * i] = 0;
                                    }
                                }
                            }
                            else //colonne 12,13
                            {
                                valRGB[25 - 1 - j, 25 - 11 - k - 2 * i] = chaine_binaire[163 + 2 * j + 36 * i + k];
                                if ((25 - 1 - j + 25 - 11 - k - 2 * i) % 2 == 0) //Masque, même principe
                                {
                                    if (chaine_binaire[163 + 2 * j + 36 * i + k] == '0') 
                                    {
                                        valRGB[25 - 1 - j, 25 - 11 - k - 2 * i] = 1;
                                    }
                                    else
                                    {
                                        valRGB[25 - 1 - j, 25 - 11 - k - 2 * i] = 0;
                                    }
                                }
                            }
                        }
                    }
                }
                //colonne 10,11,12 et 13 du haut du QR jusqu'au séparateur 
                //235+24=259
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        for (int k = 0; k < 2; k++)//pour faire droite gauche 
                        {
                            if (i % 2 == 0)//colonne 12,13
                            {
                                valRGB[25 - 20 - j, 25 - 13 - k - 2 * i] = chaine_binaire[235 + 2 * j + 12 * i + k];
                                if ((25 - 20 - j + 25 - 13 - k - 2 * i) % 2 == 0)//on applique le masque 0 (quadrillage) ((colonne+ligne) % 2 == 0)
                                {
                                    if (chaine_binaire[235 + 2 * j + 12 * i + k] == '0')
                                    {
                                        valRGB[25 - 20 - j, 25 - 13 - k - 2 * i] = 1;
                                    }
                                    else
                                    {
                                        valRGB[25 - 20 - j, 25 - 13 - k - 2 * i] = 0;
                                    }
                                }
                            }
                            else //colonne 10,11
                            {
                                valRGB[j, 25 - 13 - k - 2 * i] = chaine_binaire[235 + 2 * j + 12 * i + k];
                                if ((j + 25 - 13 - k - 2 * i) % 2 == 0) //Masque, même principe
                                {
                                    if (chaine_binaire[235 + 2 * j + 12 * i + k] == '0') 
                                    {
                                        valRGB[j, 25 - 13 - k - 2 * i] = 1;
                                    }
                                    else
                                    {
                                        valRGB[j, 25 - 13 - k - 2 * i] = 0;
                                    }
                                }
                            }
                        }
                    }
                }
                //colonne 10,11 du séparateur jusqu'au bas du QR 
                //259+18*2=295
                for (int i = 0; i < 18; i++)
                {
                    for (int j = 0; j < 2; j++)// gauche droite
                    {
                        valRGB[25 - 18 + i, 25 - 15 - j] = chaine_binaire[259 + j + 2 * i];
                        if ((25 - 18 + i + 25 - 15 - j) % 2 == 0)//Mask
                        {
                            if (chaine_binaire[259 + j + 2 * i] == '0')
                            {
                                valRGB[25 - 18 + i, 25 - 15 - j] = 1;
                            }
                            else
                            {
                                valRGB[25 - 18 + i, 25 - 15 - j] = 0;
                            }
                        }
                    }
                }
                //colonne 8,9 à droite du séparateur 
                //295+16=311
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 2; j++)// gauche droite
                    {
                        valRGB[9 + i, 25 - 17 - j] = chaine_binaire[295 + j + 2 * i];
                        if ((9 + i + 25 - 17 - j) % 2 == 0)//Mask
                        {
                            if (chaine_binaire[295 + j + 2 * i] == '0')
                            {
                                valRGB[i + 9, 25 - 17 - j] = 1;
                            }
                            else
                            {
                                valRGB[i + 9, 25 - 17 - j] = 0;
                            }
                        }
                    }
                }
                //colonne 1 à 6
                //311+6*8=352+7
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        for (int k = 0; k < 2; k++)//pour faire droite gauche 
                        {
                            if (311 + 2 * j + 16 * i + k < 352)//pas deppaser la taille de chaine binaire
                            {
                                if (i % 2 == 0)//monté
                                {
                                    valRGB[25 - 16 + j, 25 - 20 - k - 2 * i] = chaine_binaire[311 + 2 * j + 16 * i + k];
                                    if ((25 - 16 + j + 25 - 20 - k - 2 * i) % 2 == 0)//on applique le masque 0 (quadrillage) ((colonne+ligne) % 2 == 0)
                                    {
                                        if (chaine_binaire[311 + 2 * j + 16 * i + k] == '0')
                                        {
                                            valRGB[25 - 16 + j, 25 - 20 - k - 2 * i] = 1;
                                        }
                                        else
                                        {
                                            valRGB[25 - 16 + j, 25 - 20 - k - 2 * i] = 0;
                                        }
                                    }
                                }
                                else //déscente
                                {
                                    valRGB[25 - 9 - j, 25 - 20 - k - 2 * i] = chaine_binaire[311 + 2 * j + 16 * i + k];
                                    if ((25 - 9 - j + 25 - 20 - k - 2 * i) % 2 == 0) //Masque, même principe
                                    {
                                        if (chaine_binaire[311 + 2 * j + 16 * i + k] == '0') //'0'
                                        {
                                            valRGB[25 - 9 - j, 25 - 20 - k - 2 * i] = 1;
                                        }
                                        else
                                        {
                                            valRGB[25 - 9 - j, 25 - 20 - k - 2 * i] = 0;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                { valRGB[25 - 16 + j, 25 - 20 - k - 2 * i] = (k+j)%2; }//quadrillage à la fin (7 dernieres valeurs)
                            }
                        }
                    }
                }
                for (int i = 0; i < 25; i++) //remplir les pixels
                {
                    for (int j = 0; j < 25; j++)
                    {
                        if (valRGB[i, j] == 48 || valRGB[i, j] == 49)//Car convert.ToInt32 donne la velur ASCII de 0 ou 1, on reviens donc à la valeur normal
                        {
                            valRGB[i, j] -= 48;
                        }
                        if (valRGB[i, j] == 1)
                        {
                            image1[(24 - i) * 3, j * 3] = new Pixel(0, 0, 0);
                            image1[(24 - i) * 3, j * 3 + 1] = new Pixel(0, 0, 0);
                            image1[(24 - i) * 3, j * 3 + 2] = new Pixel(0, 0, 0);
                            image1[(24 - i) * 3 + 1, j * 3] = new Pixel(0, 0, 0);
                            image1[(24 - i) * 3 + 1, j * 3 + 1] = new Pixel(0, 0, 0);
                            image1[(24 - i) * 3 + 1, j * 3 + 2] = new Pixel(0, 0, 0);
                            image1[(24 - i) * 3 + 2, j * 3] = new Pixel(0, 0, 0);
                            image1[(24 - i) * 3 + 2, j * 3 + 1] = new Pixel(0, 0, 0);
                            image1[(24 - i) * 3 + 2, j * 3 + 2] = new Pixel(0, 0, 0);
                        }
                        if (valRGB[i, j] == 2 || valRGB[i, j] == 0)
                        {
                            image1[(24 - i) * 3, j * 3] = new Pixel(255, 255, 255);
                            image1[(24 - i) * 3, j * 3 + 1] = new Pixel(255, 255, 255);
                            image1[(24 - i) * 3, j * 3 + 2] = new Pixel(255, 255, 255);
                            image1[(24 - i) * 3 + 1, j * 3] = new Pixel(255, 255, 255);
                            image1[(24 - i) * 3 + 1, j * 3 + 1] = new Pixel(255, 255, 255);
                            image1[(24 - i) * 3 + 1, j * 3 + 2] = new Pixel(255, 255, 255);
                            image1[(24 - i) * 3 + 2, j * 3] = new Pixel(255, 255, 255);
                            image1[(24 - i) * 3 + 2, j * 3 + 1] = new Pixel(255, 255, 255);
                            image1[(24 - i) * 3 + 2, j * 3 + 2] = new Pixel(255, 255, 255);
                        }
                        //On peut modifier les 1 et 0 par 5 et 6 pour voir visuelement sur le QR code ou sont placé les bits
                        if (valRGB[i, j] == 6)//test bleu
                        {
                            image1[(24 - i) * 3, j * 3] = new Pixel(240, 0, 0);
                            image1[(24 - i) * 3, j * 3 + 1] = new Pixel(240, 0, 0);
                            image1[(24 - i) * 3, j * 3 + 2] = new Pixel(240, 0, 0);
                            image1[(24 - i) * 3 + 1, j * 3] = new Pixel(240, 0, 0);
                            image1[(24 - i) * 3 + 1, j * 3 + 1] = new Pixel(240, 0, 0);
                            image1[(24 - i) * 3 + 1, j * 3 + 2] = new Pixel(240, 0, 0);
                            image1[(24 - i) * 3 + 2, j * 3] = new Pixel(240, 0, 0);
                            image1[(24 - i) * 3 + 2, j * 3 + 1] = new Pixel(240, 0, 0);
                            image1[(24 - i) * 3 + 2, j * 3 + 2] = new Pixel(240, 0, 0);
                        }
                        if (valRGB[i, j] == 5)//test vert
                        {
                            image1[(24 - i) * 3, j * 3] = new Pixel(0, 250, 0);
                            image1[(24 - i) * 3, j * 3 + 1] = new Pixel(0, 250, 0);
                            image1[(24 - i) * 3, j * 3 + 2] = new Pixel(0, 250, 0);
                            image1[(24 - i) * 3 + 1, j * 3] = new Pixel(0, 250, 0);
                            image1[(24 - i) * 3 + 1, j * 3 + 1] = new Pixel(0, 250, 0);
                            image1[(24 - i) * 3 + 1, j * 3 + 2] = new Pixel(0, 250, 0);
                            image1[(24 - i) * 3 + 2, j * 3] = new Pixel(0, 250, 0);
                            image1[(24 - i) * 3 + 2, j * 3 + 1] = new Pixel(0, 250, 0);
                            image1[(24 - i) * 3 + 2, j * 3 + 2] = new Pixel(0, 250, 0);
                        }
                    }
                }
                this.image = image1;
                From_Image_To_File(QRcode);
            }
            else 
            {
               Console.WriteLine("ERREUR");
            }
        }
        #endregion
        //permet la lecture et parfois l'assignation de valeur à des attributs
        #region ACCESSEURS
        public string Nom
        {
            get { return nom; }
        }
        public string Type
        {
            get { return type; }
        }
        public int Taille
        {
            get { return taille; }
        }
        public int Offset
        {
            get { return offset; }
        }
        public int Largeur
        {
            get { return largeur; }
        }
        public int Hauteur
        {
            get { return hauteur; }
        }
        public int NombrebBitsCouleurs
        {
            get { return nombreBitsCouleur; }
        }
        public int[,] ValRGB
        {
            get { return valRGB; }
            set { valRGB = value; }
        }
        public Pixel[,] Image
        {
            get { return image; }
        }
        #endregion
        //Fonctions permetant d'avoir accès au données de l'image et d'enregistrer une image
        #region INFOS + CREATION IMAGE
        /// <summary>
        /// créer une chaine de caractere décrivant l'image (type, taille, hauteur largeur etc) 
        /// </summary>
        /// <returns></returns> la chaine de caractère décrivant le fichier
        public override string ToString()
        {
            string s = "";
            s += "Type fichier : " + type;
            s += "\nTaille fichier : " + taille;
            s += "\nOffset : " + offset;
            s += "\nLargeur image : " + largeur;
            s += "\nHauteur image : " + hauteur;
            s += "\nNombre de bits par couleur : " + nombreBitsCouleur;
            s += "\nHEADER\n";
            for (int i = 0; i < 14; i++)
                s += myfile[i] + " ";
            s += "\nHEADER INFO\n";
            for (int i = 14; i < offset; i++)
                s += myfile[i] + " ";
            /*#region Affichage image ( !!! A FAIRE SEULEMENT SI TRES PETITE IMAGE !!! )
            s += "\nimage\n";
            for (int i = offset; i < myfile.Length; i += largeur * 3)
            {
                for (int j = i; j < i + largeur * 3; j++)
                {
                    s += myfile[j] + " ";
                }
            }
            #endregion*/
            return s;
        }
        /// <summary>
        /// Fonction qui sers à enregstrer une image en tant que fichier bitmap
        /// </summary>
        /// <param name="QRcode"></param>booléen qui sers à savoir si l'image à enregistrer est un QRcode
        public void From_Image_To_File(bool QRcode = false)
        {
            List<byte> newimage = new();
            //type : BM
            newimage.Add(66);
            newimage.Add(77);
            //taille
            byte[] temp = Convertir_Int_To_Endian(taille, 4);
            for (int i = 0; i <= 3; i++)
            {
                newimage.Add(temp[i]);
            }
            //toujours pareil
            for (int i = 0; i < 4; i++)
            {
                newimage.Add(0);
            }
            //taille offset
            temp = Convertir_Int_To_Endian(offset, 4);
            for (int i = 0; i < 4; i++)
            {
                newimage.Add(temp[i]);
            }
            //taille header
            temp = Convertir_Int_To_Endian(40, 4);
            for (int i = 0; i < 4; i++)
            {
                newimage.Add(temp[i]);
            }
            //largeur
            temp = Convertir_Int_To_Endian(largeur, 4);
            for (int i = 0; i < 4; i++)
            {
                newimage.Add(temp[i]);
            }
            //hauteur
            temp = Convertir_Int_To_Endian(hauteur, 4);
            for (int i = 0; i < 4; i++)
            {
                newimage.Add(temp[i]);
            }
            //toujours pareil
            newimage.Add(1);
            newimage.Add(0);
            //nombre de Bits par couleur
            temp = Convertir_Int_To_Endian(nombreBitsCouleur, 2);
            for (int i = 0; i < 2; i++)
            {
                newimage.Add(temp[i]);
            }
            //toujours pareil
            for (int i = 0; i < 24; i++)
            {
                newimage.Add(0);
            }
            //image
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    newimage.Add((byte)(image[i, j].Red));
                    newimage.Add((byte)(image[i, j].Green));
                    newimage.Add((byte)(image[i, j].Blue));
                }
                for (int l = 0; l < image.GetLength(1) % 4; l++)
                {
                    newimage.Add(0);
                }
            }
            if(QRcode == true)
                File.WriteAllBytes("QRcode.bmp", newimage.ToArray());
            else
                File.WriteAllBytes("newimage.bmp", newimage.ToArray());
        }
        #endregion
        //Les méthode permetant de convertir int et endian dans un sens comme dans l'autre
        #region METHODES CONVERTIR
        /// <summary>
        /// convertit un tableau d'endian en entier (sers principalement pour décoder le header)
        /// </summary>
        /// <param name="tab"></param>tableau de byte que l'on cherche à convertir
        /// <returns></returns>l'entier correspondant
        public static int Convertir_Endian_To_Int(byte[] tab)
        {
            int s = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                s += (int)(tab[i] * Math.Pow(256, i));
            }
            return s;
        }
        /// <summary>
        /// convertit un entier en endian de la taille que l'on souhaite
        /// </summary>
        /// <param name="val"></param>valeur que l'on cherche à convertir
        /// <param name="n"></param>taille de nombre en endian souhaité 
        /// <returns></returns>l'endian correspondant
        public static byte[] Convertir_Int_To_Endian(int val, int n)
        {
            byte[] endian = new byte[n];
            for (int i = 0; i < n; i++)
            {
                endian[n - 1 - i] = (byte)(int)(val / Math.Pow(256, n - 1 - i));
                val = (int)(val - endian[i] * Math.Pow(256, n - 1 - i));
            }
            return endian;
        }
        #endregion
        // Ensemble des fonctions qui vont servir à modifier les images (toutes les fonctions créent de nouvelles images et ne modifient pas les images deja existantes)
        #region METHODES TRAITEMENTS IMAGES
        /// <summary>
        /// Créer une image en nuance de gris à partir d'une image
        /// </summary>
        public void Nuance_Gris()
        {
            MyImage newimage = new(nom);
            for (int i = hauteur - 1; i >= 0; i--)
            {
                for (int j = 0; j < largeur; j++)
                {
                    newimage.image[i, j].Nuance_Gris();
                }
            }
            image = newimage.image;
            From_Image_To_File();
        }
        /// <summary>
        /// Créer une image en noir et blanc à partir d'une image
        /// </summary>
        /// </summary>
        public void Noir_Blanc()
        {
            MyImage newimage = new(nom);
            for (int i = hauteur - 1; i >= 0; i--)
            {
                for (int j = 0; j < largeur; j++)
                {
                    newimage.image[i, j].Noir_Blanc();
                }
            }
            image = newimage.image;
            From_Image_To_File();
        }
        /// <summary>
        /// Inverse les couleurs de l'image
        /// </summary>
        public void Inverser_Couleurs()
        {
            MyImage newimage = new(nom);
            for (int i = hauteur - 1; i >= 0; i--)
            {
                for (int j = 0; j < largeur; j++)
                {
                    newimage.image[i, j].Inverser_Couleurs();
                }
            }
            image = newimage.image;
            From_Image_To_File();
        }
        /// <summary>
        /// Remplace le pixel par la couleur primaire la plus présente
        /// </summary>
        public void Couleur_Majoritaire()
        {
            MyImage newimage = new(nom);
            for (int i = hauteur - 1; i >= 0; i--)
            {
                for (int j = 0; j < largeur; j++)
                {
                    newimage.image[i, j].Couleur_Majoritaire();
                }
            }
            image = newimage.image;
            From_Image_To_File();
        }
        /// <summary>
        /// Créer une image miroir de l'image selectionné
        /// </summary>
        public void Effet_Mirroir()
        {
            Pixel[,] newimage = new Pixel[image.GetLength(0), image.GetLength(1)];
            for (int i = hauteur - 1; i >= 0; i--)
            {
                for (int j = 0; j < largeur; j++)
                {
                    newimage[i, j] = image[i, largeur - j - 1];
                }
            }
            image = newimage;
            From_Image_To_File();
        }
        /// <summary>
        /// Créer une image tourné à 90, 180 ou 270 degrés de l'image selectionné
        /// </summary>
        /// <param name="n"></param>
        public void Rotation90(int n)
        {
            while (n % 90 != 0)
            {
                Console.WriteLine("Ecrire un angle multiple de 90. La rotation se fait dans le sens des aiguilles d'une montre.");
                n = Convert.ToInt32(Console.ReadLine());
            }
            for (int i = 0; i < n; i += 90)
            {
                Rotation1fois90();
            }
            From_Image_To_File();
        }
        /// <summary>
        /// créer une image avec une rotation de 90 deg à partir de l'image selectionné (peux être utiliser plusieurs fois pour une rotation à 180 ou 270 deg)
        /// </summary>
        private void Rotation1fois90()
        {
            int newhauteur = largeur;
            int newlargeur = hauteur;
            Pixel[,] newimage = new Pixel[newhauteur, newlargeur];
            for (int l = 0; l < newlargeur; l++)
            {
                for (int c = 0; c < newhauteur; c++)
                {
                    newimage[image.GetLength(1) - 1 - c, l] = image[l, c];
                }
            }
            hauteur = newhauteur;
            largeur = newlargeur;
            image = newimage;
        }
        /// <summary>
        /// créer une image avec une rotation quelconque 
        /// </summary>
        /// <param name="degree"></param>
        public void Rotation(int degree)
        { 
            double angle_rad = degree * 3.10 / 180.0; //sens horaire
            //matrice depart
            int i = 0;
            int j = 0; ;
            //matrices arrivee
            int x = 0;
            int y = 0;
            double distance;
            double newangle; //angle coord polaires
            int midX, midY;
            int newlargeur = largeur;
            int newhauteur = hauteur;
            midX = newlargeur / 2;
            midY = newhauteur / 2;
            MyImage newimage = new(nom);
            Pixel noir = new(0, 0, 0);
            for (i = 0; i < newhauteur; ++i)
            {
                for (j = 0; j < newlargeur; ++j)
                {
                    newimage.image[i, j] = noir;
                }
            }
            for (i = 0; i < newhauteur; ++i)
            {
                for (j = 0; j < newlargeur; ++j)
                {
                    x = j - midX;
                    y = midY - i;
                    //coord polaires
                    distance = Math.Sqrt(x * x + y * y);
                    if (x == 0)
                    {
                        if (y == 0)
                        {
                            //pas de rotation
                            newimage.image[i, j] = this.image[i, j];
                            continue;
                        }
                        else if (y > 0) newangle = 0.5 * Math.PI;
                        else newangle = 1.5 * Math.PI;
                    }
                    else newangle = Math.Atan2((double)y, (double)x);
                    newangle -= angle_rad;
                    //coord cartesiennes
                    x = (int)(Math.Round(distance * Math.Cos(newangle)));
                    y = (int)(Math.Round(distance * Math.Sin(newangle)));
                    x += midX;
                    y = midY - y;
                    if (x < 0 || x >= newlargeur || y < 0 || y >= newhauteur) continue;
                    newimage.image[i, j] = image[y, x];
                }
            }
            image = newimage.image;
            largeur = newlargeur;
            hauteur = newhauteur;
            taille = newlargeur * newhauteur * 3 + 54;
            From_Image_To_File();
        }
        /// <summary>
        /// Créer une nouvelle image qui est l'agrandissement de l'image selectionnée avec une certain coeff d'agrandissement
        /// </summary>
        /// <param name="coef"></param>Coefficient d'agrandissement de l'image
        public void Agrandir(int coef)
        {
            int newhauteur = hauteur * coef;
            int newlargeur = largeur * coef;
            Pixel[,] newimage = new Pixel[newhauteur, newlargeur];
            for (int ligne = 0; ligne < newimage.GetLength(0); ligne++)
            {
                for (int colonne = 0; colonne < newimage.GetLength(1); colonne++)
                {
                    newimage[ligne, colonne] = image[ligne / coef, colonne / coef];
                }
            }
            hauteur = newhauteur;
            largeur = newlargeur;
            taille = (largeur * hauteur * 3) + 54;
            image = newimage;
            From_Image_To_File();
        }
        /// <summary>
        /// Créer une nouvelle image qui est le retrecissement de l'image selectionnée avec une certain coeff 
        /// </summary>
        /// <param name="coef"></param>Coefficient de retrecissement de l'image
        public void Retrecir(int coef)
        {
            if (coef != 0)
            {
                int newhauteur = hauteur / coef;
                int newlargeur = largeur / coef;
                Pixel[,] newimage = new Pixel[newhauteur, newlargeur];
                for (int ligne = 0; ligne < newimage.GetLength(0); ligne++)
                {
                    for (int colonne = 0; colonne < newimage.GetLength(1); colonne++)
                    {
                        newimage[ligne, colonne] = image[ligne * coef, colonne * coef];
                    }
                }
                hauteur = newhauteur;
                largeur = newlargeur;
                taille = (largeur * hauteur * 3) + 54;
                image = newimage;
                From_Image_To_File();
            }
        }
        /// <summary>
        /// Méthode convolution permettant la multiplication entre la matrice image et une matrice noyau afin d'appliquer un filtre
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="coef"></param>
        public void Convolution(int[,] kernel, double coef = 1)
        {
            Pixel[,] newimage = new Pixel[image.GetLength(0), image.GetLength(1)];
            if (image != null && image.Length != 0 && kernel != null && kernel.Length != 0)
            {
                for (int l = 0; l < image.GetLength(0); l++)
                {
                    for (int c = 0; c < image.GetLength(1); c++)
                    {
                        Pixel newpixel = new()
                        {
                            Red = SommeKernel(kernel, l, c, coef, 1),
                            Green = SommeKernel(kernel, l, c, coef, 2),
                            Blue = SommeKernel(kernel, l, c, coef, 3)
                        };
                        newimage[l, c] = new Pixel()
                        {
                            Red = newpixel.Red > 255 ? 255 : newpixel.Red,
                            Blue = newpixel.Blue > 255 ? 255 : newpixel.Blue,
                            Green = newpixel.Green > 255 ? 255 : newpixel.Green
                        };
                    }
                }
            }
            image = newimage;
            From_Image_To_File();
        }
        /// <summary>
        /// utilisé pour convolution
        /// </summary>
        /// <param name="kernel"></param> noyau (en allemand dans le texte)
        /// <param name="l"></param> ligne
        /// <param name="c"></param> colone
        /// <param name="coef"></param> coefficient
        /// <param name="color"></param> couleur
        /// <returns></returns>
        private int SommeKernel(int[,] kernel, int l, int c, double coef, int color)
        {
            double s = 0;
            for (int i = 0; i < kernel.GetLength(0); i++)
            {
                for (int j = 0; j < kernel.GetLength(1); j++)
                {
                    int x = i + (l - kernel.GetLength(0) / 2);
                    if (x < 0)
                    {
                        x = image.GetLength(0) - x;
                    }
                    if (x >= image.GetLength(0))
                    {
                        x -= image.GetLength(0);
                    }
                    int y = j + (c - kernel.GetLength(1) / 2);
                    if (y < 0)
                    {
                        y = image.GetLength(1) - y;
                    }
                    if (y >= image.GetLength(1))
                    {
                        y -= image.GetLength(1);
                    }
                    switch (color)
                    {
                        case 1:
                            s += coef * image[x, y].Red * kernel[i, j];
                            break;
                        case 2:
                            s += coef * image[x, y].Green * kernel[i, j];
                            break;
                        case 3:
                            s += coef * image[x, y].Blue * kernel[i, j];
                            break;
                    }
                }
            }
            return (int)(s);
        }
        /// <summary>
        /// Construit une fractale de Mandelbrot
        /// </summary>
        public void Fractale()
        {
            double xmin = -2.1;
            double xmax = 0.6;
            double ymin = -1.2;
            double ymax = 1.2;
            int imax = 100;
            int X = 5000;
            int Y = 5000;
            double zR;
            double zI;
            int i;
            double tempzR;
            double cR;
            double cI;
            Pixel[,] newimage = new Pixel[X, Y];
            for (int x = 0; x < X; x++)
            {
                for (int y = 0; y < Y; y++)
                {
                    cR = y * (xmax - xmin) / Y + xmin;
                    cI = x * (ymax - ymin) / X + ymin;
                    zR = 0;
                    zI = 0;
                    i = 0;
                    do
                    {
                        tempzR = zR;
                        zR = zR * zR - zI * zI + cR;
                        zI = 2 * zI * tempzR + cI;
                        i++;
                    } while ((zR * zR + zI * zI) < 4 && i < imax);
                    if (i == imax) newimage[x, y] = new Pixel(0, 0, 0);
                    else newimage[x, y] = new Pixel(255, 255, 255);
                }
            }
            nombreBitsCouleur = 24;
            offset = 54;
            largeur = X;
            hauteur = Y;
            taille = (hauteur * largeur * 3) + offset;
            image = newimage;
            From_Image_To_File();
        }
        /// <summary>
        /// Construit un histogramme
        /// </summary>
        public void HistogrammeImage()
        {

        }
        /// <summary>
        /// Permet d'encoder une image dans une autre
        /// </summary>
        /// <param name="image2"></param> Image à encoder dans l'autre 
        public void Coder (MyImage image2)
        {
            for (int i = 0; i < image2.hauteur; i++)
            {
                for (int j = 0; j < image2.largeur; j++)
                {
                    image[i, j].Red = Fusioner_Pixels(image[i, j].Red, image2.image[i, j].Red);
                    image[i, j].Green = Fusioner_Pixels(image[i, j].Green, image2.image[i, j].Green);
                    image[i, j].Blue = Fusioner_Pixels(image[i, j].Blue, image2.image[i, j].Blue);
                }
            }
            From_Image_To_File();
        }
        /// <summary>
        /// permet de decoder une image qui à été encodé dans une autre
        /// </summary>
        public void Decoder()
        {
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    image[i, j].Red = Separer_Pixel(image[i, j].Red);
                    image[i, j].Green = Separer_Pixel(image[i, j].Green);
                    image[i, j].Blue = Separer_Pixel(image[i, j].Blue);
                }
            }
            From_Image_To_File();
        }
        /// <summary>
        /// Fusionne deux pixels (utilisé dans la fonction Coder)
        /// </summary>
        /// <param name="pixel1"></param>pixel de l'image ou l'on encode
        /// <param name="pixel2"></param>pixel de l'image à encoder
        /// <returns></returns>
        private static int Fusioner_Pixels(int pixel1, int pixel2)
        {
            string p1 = Convert.ToString(pixel1,2);
            string p2 = Convert.ToString(pixel2,2);
            while (p1.Length < 8)
                p1 = p1.Insert(0, "0");
            while (p2.Length < 8)
                p2 = p2.Insert(0, "0");
            string p = p1.Substring(0,4)+p2.Substring(0,4);
            int pixel = Convert.ToInt32(p,2);
            return pixel;
        }
        /// <summary>
        /// Sépare et récupère les pixels de l'image caché (utilisé dans la fonction décoder)
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns></returns>
        private static int Separer_Pixel(int pixel)
        {
            string p = Convert.ToString(pixel, 2);
            while (p.Length != 8)
                p = p.Insert(0, "0");
            string p2 = p.Substring(4, 4) + "0000";
            int pixel2 = Convert.ToInt32(p2, 2);
            return pixel2;
        }
        /// <summary>
        /// permet d'encoder du txt dans image
        /// </summary>
        /// <param name="txt"></param>
        public void Coder_Texte(string txt)
        {
            string txtbin="";
            foreach (char ch in txt)
            {
                txtbin += Convert.ToString((int)ch, 2);
            }
            int t = 0;
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    if (t < txtbin.Length - 1)
                    {
                        image[i, j].Red = Fusioner_Texte(image[i, j].Red, txtbin.Substring(t, 2));
                        t += 2;
                    }
                    if (t < txtbin.Length-1)
                    {
                        image[i, j].Green = Fusioner_Texte(image[i, j].Green, txtbin.Substring(t, 2));
                        t += 2;
                    }
                    if (t < txtbin.Length-1)
                    {
                        image[i, j].Blue = Fusioner_Texte(image[i, j].Blue, txtbin.Substring(t, 2));
                        t += 2;
                    }
                }
            }
            From_Image_To_File();
        }
        /// <summary>
        /// permet de décoder le texte dans l'image
        /// </summary>
        /// <returns></returns>
        public string Decoder_Texte()
        {
            string s="";
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    s+= Separer_Texte(image[i, j].Red);
                    s+= Separer_Texte(image[i, j].Green);
                    s+= Separer_Texte(image[i, j].Blue);
                }
            }
            string txt=BinaryToString(s);
            return txt;
        }
        /// <summary>
        /// utilisé dans coder texte
        /// </summary>
        /// <param name="pixel"></param>
        /// <param name="txtbin"></param>
        /// <returns></returns>
        private static int Fusioner_Texte(int pixel, string txtbin)
        {
            string p = Convert.ToString(pixel, 2);
            while (p.Length != 8)
                p = p.Insert(0, "0");
            string newp = p.Substring(0, 6) + txtbin;
            int newpixel = Convert.ToInt32(newp, 2);
            return newpixel;
        }
        /// <summary>
        /// utilisé dans decoder texte
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns></returns>
        private static string Separer_Texte(int pixel)
        {
            string p = Convert.ToString(pixel, 2);
            while (p.Length != 8)
                p = p.Insert(0, "0");
            string txtb = p.Substring(6, 2);
            return txtb;
        }
        /// <summary>
        /// convertit binaire en string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string BinaryToString(string data)
        {
            List<Byte> byteList = new List<Byte>();

            for (int i = 0; i < data.Length; i += 8)
            {
                byteList.Add(Convert.ToByte(data.Substring(i, 8), 2));
            }
            return Encoding.ASCII.GetString(byteList.ToArray());
        }
        #endregion
    }
}