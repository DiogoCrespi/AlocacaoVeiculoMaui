using SQLite;
using AlocacaoVeiuculo.RentalManager.Model.Vehicles;
using AlocacaoVeiuculo.RentalManager.Model.Users;
using AlocacaoVeiuculo.RentalManager.Model.Reservations;

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


                //// Verifica se o banco de dados existe e o exclui se necessário
                //if (File.Exists(dbPath))
                //{
                //  File.Delete(dbPath);
                //}

                // Criação direta das tabelas 
                database = new SQLiteAsyncConnection(dbPath);
                await database.CreateTableAsync<Carro>();
                await database.CreateTableAsync<Moto>();
                await database.CreateTableAsync<ModeloVeiculo>();
                await database.CreateTableAsync<Usuario>();
                await database.CreateTableAsync<Reserva>();
                await database.CreateTableAsync<Disponibilidade>();


                // Verificar e adicionar o administrador
                var admin = await database.Table<Usuario>().FirstOrDefaultAsync(u => u.Nome == "admin");
                if (admin == null)
                {
                    var usuarioAdmin = new Usuario
                    {
                        Nome = "admin",
                        Senha = "admin123",
                        Cpf = "000.000.000-00",
                        DataNascimento = new DateTime(2000, 1, 1),
                        Telefone = "00000000000"
                    };
                    await database.InsertAsync(usuarioAdmin);
                }
            }
            return database;
        }
    }
}
