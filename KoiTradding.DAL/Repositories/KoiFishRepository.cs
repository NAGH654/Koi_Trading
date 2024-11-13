
using KoiTradding.DAL.Models;

namespace KoiTradding.DAL.Repositories
{
    public class KoiFishRepository
    {
        private readonly KoiFishTradingContext _context;

        public KoiFishRepository()
        {
            _context = new KoiFishTradingContext(); 
        }

        public List<KoiFish> GetAll()
        {
            return _context.KoiFishes.ToList();
        }

        public KoiFish GetById(int koiId)
        {
            return _context.KoiFishes.FirstOrDefault(k => k.KoiId == koiId);
        }
    }
}