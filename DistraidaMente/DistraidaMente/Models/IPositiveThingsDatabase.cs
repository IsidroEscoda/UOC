using SQLite;

namespace DistraidaMente.Model
{
    public interface IPositiveThingsDatabase
    {
        SQLiteConnection GetConnection();
        SQLiteAsyncConnection GetConnectionAsync();
    }
}