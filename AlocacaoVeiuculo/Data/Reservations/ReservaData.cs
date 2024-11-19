using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlocacaoVeiuculo.Services;
using AlocacaoVeiuculo.RentalManager.Model.Reservations;
using AlocacaoVeiuculo.RentalManager.Model.Users;


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


        public async Task<List<Reserva>> ObterReservasComClientesAsync()
        {
            var reservas = await database.Table<Reserva>().ToListAsync();

            foreach (var reserva in reservas)
            {
                var cliente = await database.Table<Usuario>().FirstOrDefaultAsync(u => u.Id == reserva.UsuarioId);
                if (cliente != null)
                {
                    reserva.NomeCliente = cliente.Nome;
                }
            }

            return reservas;
        }

        public Task<int> AtualizarReservaAsync(Reserva reserva)
        {
            return database.UpdateAsync(reserva);
        }


        public Task<Reserva> ObterReservaPorIdAsync(int reservaId)
        {
            return database.Table<Reserva>()
                           .FirstOrDefaultAsync(r => r.Id == reservaId);
        }

        public async Task AtualizarBancoDeDados()
        {
            var connection = await DatabaseService.GetDatabaseAsync();
            await connection.ExecuteAsync("ALTER TABLE Reserva ADD COLUMN IsDisponivel INTEGER DEFAULT 1");
        }

    }
}

