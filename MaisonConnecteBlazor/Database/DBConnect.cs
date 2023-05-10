using MaisonConnecteBlazor.Configuration;
using MaisonConnecteBlazor.Database.Models;

namespace MaisonConnecteBlazor.Database
{
    /// <summary>
    /// Classe plus sécuritaire et plus rapide pour utiliser le context EntityFramework
    /// </summary>
    public class DBConnect
    {
        /// <summary>
        /// MaisonConnecteContext, Le contexte de la base de données
        /// </summary>
        private MaisonConnecteContext _db;
        /// <summary>
        /// MaisonConnecteContext, Le contexte de la base de données en lecture seul
        /// </summary>
        public MaisonConnecteContext db { get { return _db; } }

        /// <summary>
        /// Constructeur par défaut qui utilise les informations du manager de configuration
        /// </summary>
        public DBConnect()
        {
            _db = new MaisonConnecteContext(new MSSQLConnection(ConfigManager.ConfigurationPresente.Serveur, ConfigManager.ConfigurationPresente.Utilisateur, ConfigManager.ConfigurationPresente.MotDePasse, ConfigManager.ConfigurationPresente.BD));
        }

        /// <summary>
        /// Constructeur qui utilise des informations de connexion quelconque
        /// </summary>
        /// <param name="mssql"></param>
        public DBConnect(MSSQLConnection mssql)
        {
            _db = new MaisonConnecteContext(mssql);
        }
    }
}
