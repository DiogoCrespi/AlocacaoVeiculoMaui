using AlocacaoVeiuculo.Modelo;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlocacaoVeiuculo.Services;

namespace AlocacaoVeiuculo.Data
{
    public class ReservaData
    {
        private readonly SQLiteAsyncConnection database;

        public ReservaData()
        {
            database = DatabaseService.GetDatabaseAsync().Result;
        }


        public Task<int> AdicionarReservaAsync(Reserva reserva) => database.InsertAsync(reserva);

        public Task<List<Reserva>> ObterReservasAsync() => database.Table<Reserva>().ToListAsync();
    }
}
