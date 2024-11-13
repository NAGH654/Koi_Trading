
using KoiTradding.DAL.Models;

namespace AirConditionerShop.DAL.Repositories
{
    public class StaffRepository
    {
        private KoiFishTradingContext _context;

        public Account? Login(string Email, string pass)
        {
            _context = new();
            return _context.Accounts.FirstOrDefault(x => x.Email == Email && x.Password == pass
            );

        }
    }
}