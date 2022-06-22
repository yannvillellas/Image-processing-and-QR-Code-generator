using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S4_PSI_VIALLARD_VILLELLAS
{
    /// <summary>
    /// Class rempli de matrcie de convolution servant pour appliquer des filtres
    /// </summary>
    class Kernel
    {
        /// <summary>
        /// Matrice identité (ne fait rien)
        /// </summary>
        /// <returns></returns> retourne la matrice correspondante
        public static int[,] Identite()
        {
            return new int[,] { { 0, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } };
        }
        /// <summary>
        /// Améliore la netteté (filtre passe-haut)
        /// </summary>
        /// <returns></returns> retourne la matrice
        public static int[,] Renforcement()
        {
            return new int[,] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
        }
        /// <summary>
        /// Floute l'image
        /// </summary>
        /// <returns></returns> retourne la matrice correspondante
        public static int[,] Flou()
        {
            return new int[,] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
        }
        /// <summary>
        /// Contour
        /// </summary>
        /// <returns></returns> retourne la matrice correspondante
        public static int[,] Contour1()
        {
            return new int[,] { { 1, 0, -1 }, { 0, 0, 0 }, { -1, 0, 1 } };
        }
        /// <summary>
        /// Opérateur Laplacien (sans diagonal)
        /// </summary>
        /// <returns></returns> retourne la matrice correspondante
        public static int[,] Contour2()
        {
            return new int[,] { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
        }
        /// <summary>
        /// Opérateur Laplacien (avec diagonal)
        /// </summary>
        /// <returns></returns> retourne la matrice correspondante
        public static int[,] Contour3()
        {
            return new int[,] { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };
        }
        /// <summary>
        /// Filtre repoussage
        /// </summary>
        /// <returns></returns> retourne la matrice correspondante
        public static int[,] Repoussage()
        {
            return new int[,] { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };
        }

    }
}
