using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNEAutoInstaller.Services
{
    public class DatabaseService
    {
        public SQLiteConnection DbConnection;

        public DatabaseService()
        {
            this.DbConnection = new SQLiteConnection("Data Source=Database\\database.db;Version=3;UseUTF16Encoding=True");
        }

        public void OpenConnection()
        {
            if (this.DbConnection.State != System.Data.ConnectionState.Open)
            {
                this.DbConnection.Open();
            }
        }

        public void CloseConnection()
        {
            if (this.DbConnection.State != System.Data.ConnectionState.Closed)
            {
                this.DbConnection.Close();
            }
        }
    }
}
