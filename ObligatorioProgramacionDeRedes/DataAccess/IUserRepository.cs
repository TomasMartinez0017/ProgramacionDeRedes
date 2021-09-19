using Domain;
namespace DataAccess
{
    public interface IUserRepository
    {
        void AddUser(User user);

        User GetUser(string username);

        bool UserExists(string username);
    }
}