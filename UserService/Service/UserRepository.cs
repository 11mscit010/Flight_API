using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Entity;

namespace UserService.Service
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext context;
        public UserRepository(AppDbContext _context)
        {
            context = _context;
        }

        public int RegisterUser(UserEntity input)
        {
            var isExists = context.Users.Any(z => z.UserName == input.UserName);
            if (isExists)
                return 0;
            context.Users.Add(input);
            return 1;
        }

        public UserEntity GetUserById(int Id)
        {
            return context.Users.Find(Id);
        }

        public UserEntity GetUserByUsername(string Username)
        {
            return context.Users.FirstOrDefault(z=>z.UserName == Username);
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
