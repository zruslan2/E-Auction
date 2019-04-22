using AutoMapper;
using E_Auction.Core.DataModels;
using E_Auction.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Auction.BLL.Mappers
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<OpenOrganizationRequestVm, Organization>()
                .ForMember(p => p.FullName,
                    opt => opt.MapFrom(p => p.FullName))
                .ForMember(p => p.IdentificationNumber,
                    opt => opt.MapFrom(p => p.IdentificationNumber));
        }
    }
}
