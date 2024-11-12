using SQLite;
using System;

namespace AlocacaoVeiuculo.Modelo
{
    public class Disponibilidade
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int VeiculoId { get; set; } // ID do veículo (Carro ou Moto)
        public string TipoVeiculo { get; set; } // "Carro" ou "Moto"
        public DateTime DataInicio { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public DateTime DataFim { get; set; }
        public TimeSpan HoraFim { get; set; }
    }
}
