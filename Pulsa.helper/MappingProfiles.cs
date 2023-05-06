using AutoMapper;
using Pulsa.ViewModel;
using Pulsa.Domain.Entities;
using Pulsa.ViewModel.tagihan;

namespace Pulsa.helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Tagihan_master, InputTagihan>();
            CreateMap<InputTagihan, Tagihan_master>();
            CreateMap<InputTopUpDTO, TopUp>();
            CreateMap<TopUp, InputTopUpDTO>();
            CreateMap<TopUp, VmRequestTopup>();

            // sumber, tujuan
            CreateMap<prabayarProduk, Supplier_produk>();
        }
    }
}