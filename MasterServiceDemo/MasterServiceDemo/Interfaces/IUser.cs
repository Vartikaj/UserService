
using CommonService.Utility;
using MasterServiceDemo.Models;

namespace MasterServiceDemo.Interfaces
{
    public interface IUser
    {
        public Task<Response<UserModel>> GetAllData(RabbitMQConnectionHelper rabbitMq);
    }
}
