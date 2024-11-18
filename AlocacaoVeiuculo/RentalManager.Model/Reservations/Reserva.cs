using SQLite;
using System;

namespace AlocacaoVeiuculo.RentalManager.Model.Reservations
{
    public class Reserva
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string LocalRetirada { get; set; }
        public DateTime DataRetirada { get; set; }
        public TimeSpan HoraRetirada { get; set; }
        public DateTime DataDevolucao { get; set; }
        public TimeSpan HoraDevolucao { get; set; }
        public int UsuarioId { get; set; } // ID do usuário para associar a reserva
        public string VeiculoTipo { get; set; } // "Carro" ou "Moto"
        public int VeiculoId { get; set; } // ID do veículo alugado
    }
}
