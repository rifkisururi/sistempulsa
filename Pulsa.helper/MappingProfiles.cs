using AutoMapper;
using Pulsa.Domain.Entities;
using Pulsa.ViewModel;
using System.Net;
using System.Security.AccessControl;

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