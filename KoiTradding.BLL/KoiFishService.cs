using System.Collections.Generic;
using KoiTradding.DAL;
using KoiTradding.DAL.Models;

namespace KoiTradding.BLL
{
    public class KoiFishService
    {
        private readonly KoiFishRepository _repository = new();
        
        public List<KoiFish> GetAllKoiFish()
        {
            return _repository.GetAll();
        }

        public KoiFish GetKoiFishById(int koiId)
        {
            return _repository.GetById(koiId);
        }
    }
}