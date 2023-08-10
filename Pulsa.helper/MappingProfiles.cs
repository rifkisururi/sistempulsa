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
            CreateMap<CariProdukDTO, TopUp>();
            CreateMap<TopUp, CariProdukDTO>();
            CreateMap<TopUp, VmRequestTopup>();
            CreateMap<Tagihan_master, TagihanMasterDTO > ();

            // tujuan, sumber 
            CreateMap<prabayarProduk, Supplier_produk>();
            CreateMap<TagihanMasterDTO, Tagihan_master>();
            CreateMap<TopUp, InputTopUpDTO>();

        }
    }
}