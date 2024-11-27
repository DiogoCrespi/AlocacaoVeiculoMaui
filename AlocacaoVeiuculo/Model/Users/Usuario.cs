using SQLite;
using System;

namespace AlocacaoVeiuculo.RentalManager.Model.Users
{
    public class Usuario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Senha { get; set; }
        public string Cpf { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Telefone { get; set; }
        public string MotivoExclusao { get; set; }
        public string MotivoModificacao { get; set; }
        public bool IsDisponivel { get; set; } = true;
    }
}
