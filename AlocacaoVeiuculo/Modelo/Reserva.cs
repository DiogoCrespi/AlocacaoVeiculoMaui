using SQLite;
using System;

namespace AlocacaoVeiuculo.Modelo
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
      //  public string Residencia { get; set; }
    }
}
