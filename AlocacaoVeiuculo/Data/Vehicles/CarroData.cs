using AlocacaoVeiuculo.RentalManager.Model.Vehicles;
using AlocacaoVeiuculo.Services;
using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AlocacaoVeiuculo.Data.Vehicles
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

        public Task<int> RemoverCarroAsync(Carro carro)
        {
            if (!string.IsNullOrEmpty(carro.ImagemPath) && File.Exists(carro.ImagemPath))
            {
                File.Delete(carro.ImagemPath);
            }
            return database.DeleteAsync(carro);
        }

        public Task<Carro> ObterCarroPorIdAsync(int id) => database.Table<Carro>().FirstOrDefaultAsync(c => c.Id == id);

        public async Task<int> SalvarImagemCarroAsync(int carroId, string caminhoImagem)
        {
            var carro = await ObterCarroPorIdAsync(carroId);
            if (carro != null)
            {
                carro.ImagemPath = caminhoImagem;
                return await AtualizarCarroAsync(carro);
            }
            return 0;
        }
    }
}
