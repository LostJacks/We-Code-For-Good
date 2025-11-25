using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabelingTool.Model;
using Model;

namespace LabelingTool.DAL.Interfaces
{
	public interface IUserRepository
	{
		AppUser? GetById(int id);
		AppUser? GetByEmail(string email);
		bool EmailExists(string email);
		int Create(AppUser user);
		void SaveRefreshToken(int userId, string refreshToken, DateTime expiresAt);
		AppUser? GetByRefreshToken(string refreshToken);

	}
}
