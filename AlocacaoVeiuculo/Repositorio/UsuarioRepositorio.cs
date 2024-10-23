namespace AlocacaoVeiuculo
{
    public class UsuarioRepositorio
    {
        private Dictionary<string, (string senha, string cpf, DateTime dataNascimento, string telefone)> usuarios = new();

        public void AdicionarUsuario(string nome, string senha, string cpf, DateTime dataNascimento, string telefone)
        {
            usuarios[nome] = (senha, cpf, dataNascimento, telefone);
        }

        public (string senha, string cpf, DateTime dataNascimento, string telefone) ObterUsuario(string nome)
        {
            return usuarios[nome];
        }

        public bool UsuarioExistente(string nome)
        {
            return usuarios.ContainsKey(nome);
        }
    }
}
