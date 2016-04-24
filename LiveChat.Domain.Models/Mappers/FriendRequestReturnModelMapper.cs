using LiveChat.DataAccess.Entities.Models;

namespace LiveChat.Domain.Models.Mappers
{
    public interface IFriendRequestReturnModelMapper
    {
        FriendRequestReturnModel Map(FriendRequest req);
    }

    public class FriendRequestReturnModelMapper : IFriendRequestReturnModelMapper
    {
        private IUserReturnModelMapper userReturnModelMapper;

        public FriendRequestReturnModelMapper(IUserReturnModelMapper userMapper)
        {
            userReturnModelMapper = userMapper;
        }

        public FriendRequestReturnModel Map(FriendRequest req)
        {
            return new FriendRequestReturnModel
            {
                Date = req.Date,
                Receiver = userReturnModelMapper.Map(req.Receiver),
                Requestor = userReturnModelMapper.Map(req.Requestor)
            };
        }
    }
}
