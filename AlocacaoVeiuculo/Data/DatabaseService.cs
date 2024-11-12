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

                // Verifica se o banco de dados existe e o exclui se necessário
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                }

                database = new SQLiteAsyncConnection(dbPath);
                await database.CreateTableAsync<Carro>();
                await database.CreateTableAsync<Moto>();
                await database.CreateTableAsync<ModeloVeiculo>();
                await database.CreateTableAsync<Usuario>();
                await database.CreateTableAsync<Reserva>();
                await database.CreateTableAsync<Disponibilidade>();
            }
            return database;
        }
    }
}
