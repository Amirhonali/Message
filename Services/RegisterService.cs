using System;
using Message.Data;
using Message.Repositories;
using Microsoft.EntityFrameworkCore;


namespace Message.Services
{
	public static class RegisterService
	{
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MessageDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IMessageRepository, MessageRepository>();
            return services;
        }
    }
}

