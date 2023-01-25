using AutoMapper;
using Pulsa.Domain.Entities;
using Pulsa.ViewModel;

namespace Pulsa.helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Tagihan_master, InputTagihan>();
            CreateMap<InputTagihan, Tagihan_master>();
        }
    }
}