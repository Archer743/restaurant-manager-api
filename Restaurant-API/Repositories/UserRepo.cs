using Microsoft.EntityFrameworkCore;
using Restaurant_API.Data;
using Restaurant_API.Data.Entities;
using Restaurant_API.Repositories.Interfaces;

namespace Restaurant_API.Repositories
{
	public class UserRepo : IUserRepo
	{
		private readonly AppDbContext context;

		public UserRepo(AppDbContext context)
		{
			this.context = context;
		}

		public async Task AddAsync(User user)
		{
			context.Users.Add(user);
			await context.SaveChangesAsync();
		}

		public async Task<User?> GetByUsernameAsync(string username)
		{
			return await context.Users
								.Include(user => user.Role)
								.FirstOrDefaultAsync(user => user.Username == username);
		}

		public async Task<User?> GetByIdAsync(Guid id)
		{
			return await context.Users
								.Include(user => user.Role)
								.FirstOrDefaultAsync(user => user.Id == id);
		}
	}
}
