// <copyright file="DatabaseService.cs" company="Kristof Solbrig - HNE">
// Copyright (c) Kristof Solbrig - HNE. All rights reserved.
// </copyright>

using System;
using System.Data.SQLite;

namespace HNEAutoInstaller.Services
{
    /// <summary>
    /// Class DatabaseService models database main services.
    /// </summary>
    public class DatabaseService
    {
        private SQLiteConnection _dbConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseService"/> class.
        /// </summary>
        public DatabaseService()
        {
            this._dbConnection = new SQLiteConnection("Data Source=Database\\database.db;Version=3;UseUTF16Encoding=True");
        }

        /// <summary>
        /// Gets and sets for DbConnection.
        /// </summary>
        public SQLiteConnection DbConnection
        {
            get
            {
                return this._dbConnection;
            }

            private set
            {
                this._dbConnection = value;
            }
        }

        /// <summary>
        /// Try to open connection to database.
        /// </summary>
        public void OpenConnection()
        {
            try
            {
                if (this._dbConnection.State != System.Data.ConnectionState.Open)
                {
                    this._dbConnection.Open();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Try to close connection to database.
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                if (this._dbConnection.State != System.Data.ConnectionState.Closed)
                {
                    this._dbConnection.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}