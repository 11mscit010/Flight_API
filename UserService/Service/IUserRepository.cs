using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Entity;

namespace UserService.Service
{
    public interface IUserRepository
    {
        int RegisterUser(UserEntity input);
        
        UserEntity GetUserById(int Id);

        UserEntity GetUserByUsername(string Username);
        
        void SaveChanges();

    }
}
