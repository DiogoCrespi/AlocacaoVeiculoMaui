using AlocacaoVeiuculo.Modelo;
using AlocacaoVeiuculo.Services;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlocacaoVeiuculo.Data
{
    public class DisponibilidadeData
    {
        private readonly SQLiteAsyncConnection database;

        public DisponibilidadeData()
        {
            database = DatabaseService.GetDatabaseAsync().Result;
        }

        public Task<int> AdicionarDisponibilidadeAsync(Disponibilidade disponibilidade) => database.InsertAsync(disponibilidade);

        public Task<List<Disponibilidade>> ObterDisponibilidadesAsync() => database.Table<Disponibilidade>().ToListAsync();
    }
}
