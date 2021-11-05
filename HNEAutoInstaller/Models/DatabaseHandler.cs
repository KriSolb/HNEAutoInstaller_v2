// <copyright file="DatabaseHandler.cs" company="Kristof Solbrig - HNE">
// Copyright (c) Kristof Solbrig - HNE. All rights reserved.
// </copyright>

using Narumikazuchi;
using System;
using System.Data;
using System.Data.SQLite;

namespace HNEAutoInstaller.Models
{
    /// <summary>
    /// Class DatabaseHandler models handling database access.
    /// </summary>
    public sealed partial class DatabaseHandler : Singleton
    {
        private SQLiteConnection _dbConnection;

        private DatabaseHandler()
        {
            this._dbConnection = new SQLiteConnection(Properties.Resources.SQLConnectionString);
            try
            {
                this._dbConnection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Executes the specified query and returns the query result.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="parameters">The parameters for the command.</param>
        /// <returns>Returns a table filled with the results of the query.</returns>
        public DataTable ExecuteQuery(String query, params Tuple<String, Object>[] parameters)
        {
            SQLiteCommand command = new(query, this._dbConnection);
            foreach (Tuple<String, Object> parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.Item1, parameter.Item2);
            }
            SQLiteDataReader reader = command.ExecuteReader();
            DataTable table = new();
            table.Load(reader);
            return table;
        }
    }

    partial class DatabaseHandler : IDisposable
    {
        /// <inheritdoc/>
        public void Dispose()
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