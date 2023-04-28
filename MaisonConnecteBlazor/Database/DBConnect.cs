using MaisonConnecteBlazor.Configuration;
using MaisonConnecteBlazor.Database.Models;

namespace MaisonConnecteBlazor.Database
{
    public class DBConnect
    {
        private MaisonConnecteContext _db;
        public MaisonConnecteContext db { get { return _db; } }

        public DBConnect()
        {
            _db = new MaisonConnecteContext(new MSSQLConnection(ConfigManager.CurrentConfig.Server, ConfigManager.CurrentConfig.User, ConfigManager.CurrentConfig.Password, ConfigManager.CurrentConfig.Database));
        }

        public DBConnect(MSSQLConnection mssql)
        {
            _db = new MaisonConnecteContext(mssql);
        }

    }
}
