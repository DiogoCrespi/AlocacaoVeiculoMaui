﻿
using SQLite;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using AlocacaoVeiuculo.Modelo;

namespace AlocacaoVeiuculo.Services
{
    public static class DatabaseService
    {
        private static SQLiteAsyncConnection database;

        public static async Task<SQLiteAsyncConnection> GetDatabaseAsync()
        {
            if (database == null)
            {
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "AlocacaoVeiculo.db3");
                database = new SQLiteAsyncConnection(dbPath);
                await database.CreateTableAsync<Carro>();
                await database.CreateTableAsync<Moto>();
            }
            return database;
        }
    }
}
