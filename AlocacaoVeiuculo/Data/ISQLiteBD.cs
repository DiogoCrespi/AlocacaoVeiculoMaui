using SQLite;

namespace AlocacaoVeiuculo.Data
{
    public interface ISQLiteBD
    {
        SQLiteAsyncConnection GetConnection();
    }
}
