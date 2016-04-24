using LiveChat.DataAccess.Entities.Models;

namespace LiveChat.Domain.Models.Mappers
{
    public interface IUserReturnModelMapper
    {
        UserReturnModel Map(AspNetUser usr);
    }

    public class UserReturnModelMapper : IUserReturnModelMapper
    {
        public UserReturnModel Map(AspNetUser usr)
        {
            return new UserReturnModel
            {
                Id = usr.Id,
                Email = usr.Email,
                UserName = usr.UserName
            };
        }
    }
}
