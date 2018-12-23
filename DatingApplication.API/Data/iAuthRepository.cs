using System.Threading.Tasks;
using DatingApplication.API.Models;

namespace DatingApplication.API.Data
{
    public interface iAuthRepository
    {
         Task<User> Register (User user,string password);
         Task<User> Login(string username,string password);

         Task<bool> UserExists(string username);


    }
}