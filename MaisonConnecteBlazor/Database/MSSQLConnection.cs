using MaisonConnecteBlazor.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace MaisonConnecteBlazor.Database
{
    public class MSSQLConnection
    {
        private string Server;
        private string User;
        private string Password;
        private string Database;

        public MSSQLConnection(string _server, string _user, string _password, string _database)
        {
            Server = _server;
            User = _user;
            Password = _password;
            Database = _database;
        }

        public MSSQLConnection((string server, string user, string password, string database) config)
        {
            Server = config.server;
            User = config.user;
            Password = config.password;
            Database = config.database;
        }

        public DbContextOptions<MaisonConnecteContext> GetContextOptions()
        {
            DbContextOptionsBuilder<MaisonConnecteContext> optionsBuilder = new DbContextOptionsBuilder<MaisonConnecteContext>();
            optionsBuilder.UseSqlServer(ToString());
            return optionsBuilder.Options;
        }

        public static implicit operator DbContextOptions<MaisonConnecteContext>(MSSQLConnection mssql)
        {
            return mssql.GetContextOptions();
        }

        public static implicit operator string(MSSQLConnection mssql)
        {
            return mssql.ToString();
        }

        public override string ToString()
        {
            return "Server=" + Server + ";User ID=" + User + ";Password=" + Password + ";Database=" + Database + ";Trusted_Connection=False;Encrypt=False";
        }
    }
}