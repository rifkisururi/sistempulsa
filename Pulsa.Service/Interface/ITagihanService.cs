using Pulsa.Domain.Entities;
using Pulsa.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsa.Service.Interface
{
    public interface ITagihanService
    {
        public bool actionTagihanMaster(InputTagihan tm);
        //public List<Tagihan_detail> getAllTagihanActive();
    }
}
