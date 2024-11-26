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
        public int UsuarioId { get; set; }
        public int VeiculoId { get; set; }
        public string VeiculoTipo { get; set; } 
        public string MotivoModificacao { get; set; }
        public string ModeloVeiculo { get; set; }
        public string MotivoExclusao { get; set; }

        public bool IsDisponivel { get; set; } = true;

        [Ignore] // não será armazenado no banco de dados
        public string NomeCliente { get; set; }
    }
}
