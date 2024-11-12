using SQLite;

namespace AlocacaoVeiuculo.Modelo
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
    }
}
