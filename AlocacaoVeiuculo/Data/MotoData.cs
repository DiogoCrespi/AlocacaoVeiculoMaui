using AlocacaoVeiuculo.Modelo;
using AlocacaoVeiuculo.Services;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlocacaoVeiuculo.Data
{
    public class MotoData
    {
        private readonly SQLiteAsyncConnection database;

        public MotoData()
        {
            database = DatabaseService.GetDatabaseAsync().Result;
        }

        public Task<List<Moto>> ObterMotosAsync() => database.Table<Moto>().ToListAsync();

        public Task<int> AdicionarMotoAsync(Moto moto) => database.InsertAsync(moto);

        public Task<int> AtualizarMotoAsync(Moto moto) => database.UpdateAsync(moto);

        public Task<int> RemoverMotoAsync(Moto moto) => database.DeleteAsync(moto);
    }
}
