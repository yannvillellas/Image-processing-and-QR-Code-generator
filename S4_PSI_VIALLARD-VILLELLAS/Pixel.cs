using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S4_PSI_VIALLARD_VILLELLAS
{
    /// <summary>
    /// Class de pixel composé de 3 valeurs RGB. Une matrice de pixel represente une image 
    /// </summary>
    class Pixel
    {

        int red;
        int green;
        int blue;
        /// <summary>
        /// Initialise les valeur RGB du pixel couleur par couleur
        /// </summary>
        /// <param name="R"></param> valeur décimal (entre 0 et 255) du Rouge dans le pixel
        /// <param name="G"></param> valeur décimal (entre 0 et 255) du vert dans le pixel
        /// <param name="B"></param> valeur décimal (entre 0 et 255) du bleu dans le pixel
        public Pixel(int R, int G, int B)
        {
            if (R >= 0 && G >= 0 && B >= 0 && R < 256 && G < 256 && B < 256 )
            {
                red = R;
                green = G;
                blue = B;
            }
        }
        /// <summary>
        /// Initialise les valeur RGB du pixel avec un pixel complet
        /// </summary>
        /// <param name="pixel"></param> pixel complet (avec les valeurs des trois couleurs RGB)
        public Pixel(Pixel pixel)
        {
            red = pixel.Red;
            green = pixel.Green;
            blue = pixel.Blue;
        }
        /// <summary>
        /// Créer un nouveau pixel initialisé à (0,0,0) (noir) sans besoin de parametre d'entré
        /// </summary>
        public Pixel()
        {
            red = 0;
            green = 0;
            blue = 0;
        }
        /// <summary>
        /// Accesseur : permet d'assigner et de retourner la valeur Rouge du pixel
        /// </summary>
        public int Red
        {
            get { return red; }
            set { red = value; }
        }
        /// <summary>
        /// Accesseur : permet d'assigner et de retourner la valeur verte du pixel
        /// </summary>
        public int Green
        {
            get { return green; }
            set { green = value; }
        }
        /// <summary>
        /// Accesseur : permet d'assigner et de retourner la valeur bleu du pixel
        /// </summary>
        public int Blue
        {
            get { return blue; }
            set { blue = value; }
        }
        /// <summary>
        /// Fait la moyenne des valeurs RGB et l'assigne au pixel pour avoir du gris  
        /// </summary>
        public void Nuance_Gris()
        {
            int grey = (red + blue + green) / 3;
            red = grey;
            blue = grey;
            green = grey;
        }
        /// <summary>
        /// Fait la moyenne RGB, si >128 -> pixel blanc (255,255,255)sinon pixel noir (0,0,0)
        /// </summary>
        public void Noir_Blanc()
        {
            int grey = (red + blue + green) / 3;
            if (grey >= 128)
            {
                red = 255;
                blue = 255;
                green = 255;
            }
            else
            {
                red = 0;
                blue = 0;
                green = 0;
            }
        }
        /// <summary>
        /// inverse les couleurs
        /// </summary>
        public void Inverser_Couleurs()
        {
            red = 255 - red;
            green = 255 - green;
            blue = 255 - blue;
        }
        /// <summary>
        /// Remplace le pixel par la couleur primaire la plus présente
        /// </summary>
        public void Couleur_Majoritaire()
        {
            if(red>green && red>blue)
            {
                red = 255;
                green = 0;
                blue = 0;
            }
            if (green > red && green > blue)
            {
                red = 0;
                green = 255;
                blue = 0;
            }
            if (blue > red && blue > green)
            {
                red = 0;
                green = 0;
                blue = 255;
            }
        }
    }
}