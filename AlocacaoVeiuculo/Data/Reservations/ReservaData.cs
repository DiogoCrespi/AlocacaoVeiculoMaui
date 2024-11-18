using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlocacaoVeiuculo.Services;
using AlocacaoVeiuculo.RentalManager.Model.Reservations;

namespace AlocacaoVeiuculo.Data.Reservations
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




        // Método para obter reservas por usuário
        public Task<List<Reserva>> ObterReservasPorUsuarioAsync(int usuarioId)
        {
            return database.Table<Reserva>()
                           .Where(r => r.UsuarioId == usuarioId) 
                           .ToListAsync();
        }
        // Método para remover reservas 
        public async Task<int> RemoverReservaAsync(int reservaId)
        {
            var reserva = await database.Table<Reserva>().FirstOrDefaultAsync(r => r.Id == reservaId);
            if (reserva != null)
            {
                return await database.DeleteAsync(reserva);
            }
            return 0; // Retorna 0 se a reserva não foi encontrada
        }




    }
}

