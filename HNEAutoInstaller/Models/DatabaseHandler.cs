using HNEAutoInstaller.Services;
using System;

namespace HNEAutoInstaller.Models
{
    public static class DatabaseHandler
    {
        static DatabaseService _dbObject = new();
        static String _queryFullfileName = @"SELECT * FROM Files WHERE FullFileName = @fullFileName;";
    }
}