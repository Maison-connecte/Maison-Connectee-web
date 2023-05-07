// Cette classe a été faite dans le cadre d'un projet personnel, bien avant le projet de programmation. Elle utilise l'outil Reflection de C#.
// Le code est original

using System.Reflection;

namespace MaisonConnecteBlazor.Misc
{
    /// <summary>
    /// Classe qui agis comme un enum supportant les strings
    /// </summary>
    public class StringEnum
    {
        // Valeur de l'enum (ex: ENUM.Orange)
        protected string _value = string.Empty;

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public StringEnum() { }

        /// <summary>
        /// Constructeur par défaut, initialise la valeur de l'enum
        /// </summary>
        /// <param name="str"></param>
        public StringEnum(string str)
        {
            _value = str;
        }

        /// <summary>
        /// Fonction qui permet d'obtenir les valeurs d'un enum
        /// </summary>
        /// <typeparam name="T">T, Type de l'enum</typeparam>
        /// <returns>List, Liste des valeurs de l'enum</returns>
        public static List<T> GetValues<T>() where T : class
        {
            List<T> newList = new List<T>();
            Type type = typeof(T);

            foreach(FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                T instance = Activator.CreateInstance<T>();
                FieldInfo? fieldInfo = instance.GetType().GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);

                if(fieldInfo != null)
                {
                    TypedReference reference = __makeref(instance);
                    fieldInfo.SetValueDirect(reference, field.GetValue(null));
                }
                newList.Add(instance);   
            }

            return newList;
        }

        /// <summary>
        /// Fonction qui permet de convertir un string vers une valeur d'enum
        /// </summary>
        /// <typeparam name="T">T, Le type d'enum</typeparam>
        /// <param name="toParse">string, La valeur à convertir</param>
        /// <returns>T, la valeur d'un enum</returns>
        public static T Parse<T>(string toParse) where T : class
        {
            Dictionary<string, string> values = GetKeyValuePairs<T>();

            foreach(KeyValuePair<string, string> pair in values)
            {
                if(pair.Value.ToLower() == toParse.ToLower())
                {
                    T instance = Activator.CreateInstance<T>();
                    TypedReference reference = __makeref(instance);
                    instance.GetType().GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance).SetValueDirect(reference, pair.Value);

                    return instance;
                }
            }

            return null;
        }

        /// <summary>
        /// Fonction qui obtient l'enum sous forme de dictionaire
        /// </summary>
        /// <typeparam name="T">T, Le type de l'enum</typeparam>
        /// <returns>Dictionary, Le dictionaire contenant les informations de l'enum</returns>
        public static Dictionary<string, string> GetKeyValuePairs<T>() where T : class
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            Type type = typeof(T);

            foreach(FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                keyValuePairs.Add(field.Name, field.GetValue(null).ToString());
            }

            return keyValuePairs;
        }

        /// <summary>
        /// Fonction qui sert à vérifier si un StringEnum contient une valeur
        /// </summary>
        /// <typeparam name="T">T, Le type de l'enum</typeparam>
        /// <param name="toCheck">string, La valeur à chercher</param>
        /// <returns>bool, Si l'enum contient cette valeur</returns>
        public static bool Contains<T>(string toCheck) where T : class
        {
            bool contains = false;

            Dictionary<string, string> keyValuePairs = GetKeyValuePairs<T>();

            foreach(KeyValuePair<string, string> value in keyValuePairs)
            {
                if(toCheck.Contains(value.Value))
                {
                    contains = true;
                }
            }

            return contains;
        }

        /// <summary>
        /// Opérateur implicite pour convertir une string en enum
        /// </summary>
        /// <param name="str">string, String à convertir en enum</param>
        public static implicit operator StringEnum(string str)
        {
            return new StringEnum(str);
        }

        /// <summary>
        /// Opérateur implicite pour convertir un enum en string
        /// </summary>
        /// <param name="str">Enum, Enum à convertir en string</param>
        public static implicit operator string(StringEnum str)
        {
            return str._value;
        }

        /// <summary>
        /// Opérateur == pour vérifier si un enum et une string sont pareil
        /// </summary>
        /// <param name="strEnum">StringEnum, StringEnum à comparer</param>
        /// <param name="str">string, string à comparer</param>
        /// <returns>bool, Si les objets sont pareils</returns>
        public static bool operator ==(StringEnum strEnum, string str) {
            return strEnum.ToString() == str;
        }

        /// <summary>
        /// Opérateur == pour vérifier si une string et un enum sont pareil
        /// </summary>
        /// <param name="str">string, string à comparer</param>
        /// <param name="strEnum">StringEnum, StringEnum à comparer</param>
        /// <returns>bool, Si les objets sont pareils</returns>
        public static bool operator ==(string str, StringEnum strEnum)
        {
            return strEnum.ToString() == str;
        }
        /// <summary>
        /// Opérateur != pour vérifier si un enum et une string ne sont pas pareil
        /// </summary>
        /// <param name="strEnum">StringEnum, StringEnum à comparer</param>
        /// <param name="str">string, String à comparer</param>
        /// <returns>bool, Si les objets sont différents</returns>
        public static bool operator !=(StringEnum strEnum, string str)
        {
            return strEnum.ToString() != str;
        }

        /// <summary>
        /// Opérateur != pour vérifier si une string et un enum ne sont pas pareil
        /// </summary>
        /// <param name="str">string, String à comparer</param>
        /// <param name="strEnum">StringEnum, StringEnum à comparer</param>
        /// <returns>bool, Si les objets sont différents</returns>
        public static bool operator !=(string str, StringEnum strEnum)
        {
            return strEnum.ToString() != str;
        }


        /// <summary>
        /// Méthode qui permet de convertir un Enum en string
        /// </summary>
        /// <returns>string, La valeur de l'enum</returns>
        public override string ToString()
        {
            return _value;
        }

        /// <summary>
        /// Méthode qui sert à comparer à un autre objet
        /// </summary>
        /// <param name="obj">object, autre objet à comparer</param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Méthode qui permet d'obtenir le hashcode
        /// </summary>
        /// <returns>int, Retourne toujours 0</returns>
        public override int GetHashCode()
        {
            return 0;
        }
    }
}
