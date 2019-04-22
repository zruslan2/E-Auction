using AutoMapper;
using E_Auction.Core.DataModels;
using E_Auction.Core.ViewModels;
using E_Auction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Auction.BLL.Services
{
    public class OrganizationManagementService
    {
        private readonly AplicationDbContext _aplicationDbContext;

        public void OpenOrganization(OpenOrganizationRequestVm model)
        {
            if (model == null)
                throw new ArgumentNullException($"{typeof(OpenOrganizationRequestVm).Name} is null");

            var checkOrganization = /*(from o in _aplicationDbContext.Organizations
                                    where o.IdentificationNumber == model.IdentificationNumber ||
                                    o.FullName == model.FullName
                                    select o).ToList(); */
                                    _aplicationDbContext.Organizations
                                    .SingleOrDefault(p => p.IdentificationNumber == model.IdentificationNumber ||
                                        p.FullName == model.FullName);

            var checkOrganizationType = _aplicationDbContext.OrganizationTypes
                .SingleOrDefault(p => p.Name == model.OrganizationType);

            if (checkOrganization != null || checkOrganizationType == null)
                throw new Exception("Model validation error!");

            var organization = Mapper.Map<Organization>(model);
            organization.OrganizationType = checkOrganizationType;
            organization.RegistrationDate = DateTime.Now;

            _aplicationDbContext.Organizations.Add(organization);
            _aplicationDbContext.SaveChanges();         
        }


        public OrganizationManagementService()
        {
            _aplicationDbContext = new AplicationDbContext();
        }
    }
}
