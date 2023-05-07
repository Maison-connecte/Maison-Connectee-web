namespace MaisonConnecteBlazor.Extensions
{
    /// <summary>
    /// Classe servant à faire des extensions pour les chaines de caractères
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Fonction qui sert à trouver la xième occurence d'un caractère dans une chaine
        /// </summary>
        /// <param name="str">string, La string à executer cette fonction dessus</param>
        /// <param name="character">char, Le caractère à trouver</param>
        /// <param name="occurence">int, Le numéro de l'occurence</param>
        /// <returns>int, L'index du caractère</returns>
        public static int IndexOfNth(this string str, char character, int occurence)
        {
            int occurenceCount = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == character)
                {
                    occurenceCount++;

                    if (occurenceCount == occurence)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Fonction qui sert à trouver la xième occurence d'un caractère dans une chaine
        /// </summary>
        /// <param name="str">string, La string à executer cette fonction dessus</param>
        /// <param name="character">string, Le caractère à trouver sous forme de chaine</param>
        /// <param name="occurence">int, Le numéro de l'occurence</param>
        /// <returns>int, L'index du caractère</returns>
        public static int IndexOfNth(this string str, string character, int occurence)
        {
            return str.IndexOfNth(character[0], occurence);
        }
    }
}
