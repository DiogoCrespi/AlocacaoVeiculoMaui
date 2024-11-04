using AlocacaoVeiuculo.Modelo;
using AlocacaoVeiuculo.Services;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlocacaoVeiuculo.Data
{
    public class CarroData
    {
        private readonly SQLiteAsyncConnection database;

        public CarroData()
        {
            database = DatabaseService.GetDatabaseAsync().Result;
        }

        public Task<List<Carro>> ObterCarrosAsync() => database.Table<Carro>().ToListAsync();

        public Task<int> AdicionarCarroAsync(Carro carro) => database.InsertAsync(carro);

        public Task<int> AtualizarCarroAsync(Carro carro) => database.UpdateAsync(carro);

        public Task<int> RemoverCarroAsync(Carro carro) => database.DeleteAsync(carro);
    }
}
