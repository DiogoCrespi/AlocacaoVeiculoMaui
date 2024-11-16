using AlocacaoVeiuculo.Modelo;
using AlocacaoVeiuculo.Services;
using SQLite;
using System.Collections.Generic;
using System.IO;
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

        public Task<int> RemoverMotoAsync(Moto moto)
        {
            if (!string.IsNullOrEmpty(moto.ImagemPath) && File.Exists(moto.ImagemPath))
            {
                File.Delete(moto.ImagemPath);
            }
            return database.DeleteAsync(moto);
        }

        public Task<Moto> ObterMotoPorIdAsync(int id) => database.Table<Moto>().FirstOrDefaultAsync(m => m.Id == id);

        public async Task<int> SalvarImagemMotoAsync(int motoId, string caminhoImagem)
        {
            var moto = await ObterMotoPorIdAsync(motoId);
            if (moto != null)
            {
                moto.ImagemPath = caminhoImagem;
                return await AtualizarMotoAsync(moto);
            }
            return 0;
        }
    }
}
