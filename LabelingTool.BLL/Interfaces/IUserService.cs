using LabelingTool.DAL.Interfaces;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelingTool.BLL.Interfaces
{
    public interface IUserService
    {
        int Register(AppUser user, string password);
        bool VerifyPassword(AppUser user, string password);
        AppUser? GetByEmail(string email);
    }
}
