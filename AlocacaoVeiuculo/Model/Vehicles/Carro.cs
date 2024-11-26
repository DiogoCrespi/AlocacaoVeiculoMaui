using SQLite;

namespace AlocacaoVeiuculo.RentalManager.Model.Vehicles
{
    public class Carro
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public int Ano { get; set; }
        public double Quilometragem { get; set; }
        public int NumeroPortas { get; set; }
        public string TipoCombustivel { get; set; }
        public string ImagemPath { get; set; }
        public string MotivoExclusao { get; set; }
        public string MotivoModificacao { get; set; }
        public bool IsAlugado { get; set; } = true;
        public bool IsDisponivel { get; set; } = true;
        
    }
}
