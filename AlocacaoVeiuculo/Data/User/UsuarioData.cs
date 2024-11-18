using AlocacaoVeiuculo.RentalManager.Model.Users;
using AlocacaoVeiuculo.Services;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlocacaoVeiuculo.Data.User
{
    public class UsuarioData
    {
        private readonly SQLiteAsyncConnection database;

        public UsuarioData()
        {
            database = DatabaseService.GetDatabaseAsync().Result;
        }

        public Task<List<Usuario>> ObterUsuariosAsync() => database.Table<Usuario>().ToListAsync();

        public Task<Usuario> ObterUsuarioPorNomeAsync(string nome) =>
            database.Table<Usuario>().FirstOrDefaultAsync(u => u.Nome == nome);

        public Task<int> AdicionarUsuarioAsync(Usuario usuario) => database.InsertAsync(usuario);

        public Task<int> AtualizarUsuarioAsync(Usuario usuario) => database.UpdateAsync(usuario);

        public async Task<int> RemoverUsuarioAsync(Usuario usuario)
        {
            if (usuario.Nome == "admin")
            {
                throw new InvalidOperationException("O usuário administrador não pode ser removido.");
            }
            return await database.DeleteAsync(usuario);
        }

        public async Task<bool> UsuarioExistenteAsync(string nome)
        {
            var usuario = await ObterUsuarioPorNomeAsync(nome);
            return usuario != null;
        }
    }
}
