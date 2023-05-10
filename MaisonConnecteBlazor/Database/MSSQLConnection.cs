using MaisonConnecteBlazor.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace MaisonConnecteBlazor.Database
{
    /// <summary>
    /// Classe faciliant la génération d'une "Connexion string" MSSQL
    /// </summary>
    public class MSSQLConnection
    {
        // Variable de connexion
        private string Serveur;
        private string Utilisateur;
        private string MotDePasse;
        private string BD;

        /// <summary>
        /// Constructeur, initialise les données membres de l'objet
        /// </summary>
        /// <param name="_server">string, l'IP ou le nom de connexion du serveur</param>
        /// <param name="_user">string, Le nom d'utilisateur à utiliser pour la connexion</param>
        /// <param name="_password">string, le mot de passe de connexion</param>
        /// <param name="_database">string, La base de données à utiliser</param>
        public MSSQLConnection(string _server, string _user, string _password, string _database)
        {
            Serveur = _server;
            Utilisateur = _user;
            MotDePasse = _password;
            BD = _database;
        }

        /// <summary>
        /// Méthode générant le ContextOptions pour que EntityFrameworkCore utilise les bonnes informations de connexion
        /// </summary>
        /// <returns></returns>
        public DbContextOptions<MaisonConnecteContext> GetContextOptions()
        {
            DbContextOptionsBuilder<MaisonConnecteContext> optionsBuilder = new DbContextOptionsBuilder<MaisonConnecteContext>();
            optionsBuilder.UseSqlServer(ToString());
            return optionsBuilder.Options;
        }

        /// <summary>
        /// Opérateur implicite pour obtenir le ContextOptions
        /// </summary>
        /// <param name="mssql">MSSQLConnection, cet objet</param>
        public static implicit operator DbContextOptions<MaisonConnecteContext>(MSSQLConnection mssql)
        {
            return mssql.GetContextOptions();
        }

        /// <summary>
        /// Opérateur implicit qui retourne une connexion string sous forme de texte
        /// </summary>
        /// <param name="mssql">MSSQLConnection, cet objet</param>
        public static implicit operator string(MSSQLConnection mssql)
        {
            return mssql.ToString();
        }

        /// <summary>
        /// Méthode qui retourne une connexion string sous forme de texte
        /// </summary>
        /// <returns>string, La connexion string</returns>
        public override string ToString()
        {
            return "Server=" + Serveur + ";User ID=" + Utilisateur + ";Password=" + MotDePasse + ";Database=" + BD + ";Trusted_Connection=False;Encrypt=False";
        }
    }
}