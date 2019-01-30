using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace AspPlanApp.Data
{
    public class ConnectionDb
    {
        private readonly IConfiguration _config;

        private SqliteConnection connObj = null;

        public ConnectionDb(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection GetConnection
        {
            get
            {
                if (connObj == null)
                {
                    connObj = new SqliteConnection(_config.GetConnectionString("SqliteConnection"));
                }
                
                return connObj;
            }
        }

        public static void OpenConnect(IDbConnection currCon)
        {
            if (currCon == null) return;
            
            if (currCon.State != ConnectionState.Open)
            {
                currCon.Open();
            }
        }
    }
}