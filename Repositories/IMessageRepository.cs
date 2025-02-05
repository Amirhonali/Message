using System;
using Message.Entity;
namespace Message.Repositories
{
	public interface IMessageRepository
	{
        Task<int> AddMessageAsync(Sobsh message);

        Task<List<Sobsh>> GetMessagesAsync(TimeSpan timeSpan);
    }
}

