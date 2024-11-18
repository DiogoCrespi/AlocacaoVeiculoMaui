using SQLite;

namespace AlocacaoVeiuculo.RentalManager.Model.Vehicles
{
    public class Disponibilidade
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int VeiculoId { get; set; }
        public string TipoVeiculo { get; set; }
        public DateTime DataInicio { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public DateTime DataFim { get; set; }
        public TimeSpan HoraFim { get; set; }
        public string ImagemPath { get; set; } // Caminho da imagem
        public string Modelo { get; set; }
        public string TipoCombustivel { get; set; }

    }
}
