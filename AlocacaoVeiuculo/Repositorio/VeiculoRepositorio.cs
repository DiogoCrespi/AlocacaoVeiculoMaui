using AlocacaoVeiuculo.Modelo;
using AlocacaoVeiuculo.Services;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlocacaoVeiuculo
{
    public class VeiculoRepositorio
    {
        private readonly SQLiteAsyncConnection database;

        public VeiculoRepositorio()
        {
            database = DatabaseService.GetDatabaseAsync().Result;
        }

        public Task<int> AdicionarCarro(Carro carro)
        {
            return database.InsertAsync(carro);
        }

        public Task<int> AdicionarMoto(Moto moto)
        {
            return database.InsertAsync(moto);
        }

        public Task<List<Carro>> ObterCarros()
        {
            return database.Table<Carro>().ToListAsync();
        }

        public Task<List<Moto>> ObterMotos()
        {
            return database.Table<Moto>().ToListAsync();
        }

        public async Task<List<object>> PesquisarVeiculos(string localRetirada, DateTime dataRetirada, DateTime dataDevolucao, string residencia)
        {
            var carros = await database.Table<Carro>().Where(c => c.Modelo.Contains(localRetirada)).ToListAsync();
            var motos = await database.Table<Moto>().Where(m => m.Modelo.Contains(localRetirada)).ToListAsync();

            var resultado = new List<object>();
            resultado.AddRange(carros);
            resultado.AddRange(motos);

            return resultado;
        }
    }
}
