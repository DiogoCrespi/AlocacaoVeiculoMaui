using AlocacaoVeiuculo.Modelo;
using AlocacaoVeiuculo.Services;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlocacaoVeiuculo.Data
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

        public Task<int> RemoverUsuarioAsync(Usuario usuario) => database.DeleteAsync(usuario);

        public async Task<bool> UsuarioExistenteAsync(string nome)
        {
            var usuario = await ObterUsuarioPorNomeAsync(nome);
            return usuario != null;
        }
    }
}
