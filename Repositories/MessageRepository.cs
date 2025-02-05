using System;
using Message.Data;
using Message.Entity;
using Microsoft.EntityFrameworkCore;

namespace Message.Repositories
{
	public class MessageRepository : IMessageRepository
	{
        private readonly MessageDbContext _context;

        public MessageRepository(MessageDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddMessageAsync(Sobsh message)
        {
            _context.Sobshes.Add(message);
            await _context.SaveChangesAsync();
            return message.Id;
        }

        public async Task<List<Sobsh>> GetMessagesAsync(TimeSpan timeSpan)
        {
            var res = DateTime.UtcNow - timeSpan;
            return await _context.Sobshes
                .Where(m => m.Time >= res)
                .ToListAsync();
        }
    }
}

