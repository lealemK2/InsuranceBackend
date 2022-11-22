using System.Collections.Generic;
using System.Threading.Tasks;
using Insurance.Dto;
using Insurance.Model;

namespace Insurance.Data
{
    public interface IUserRepository
    {
        Task<List<User>>? Get();
        Task<User> Login(UserDto userDto);
        Task<User> Register(UserDto userDto);
    }
}
