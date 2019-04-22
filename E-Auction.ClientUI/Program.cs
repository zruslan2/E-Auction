using AutoMapper;
using E_Auction.BLL.Mappers;
using E_Auction.BLL.Services;
using E_Auction.Core.DataModels;
using E_Auction.Core.Exceptions;
using E_Auction.Core.ViewModels;
using E_Auction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Auction.ClientUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Mapper.Initialize(p=>
            {
                p.AddProfile<OrganizationProfile>();
                p.CreateMap<OpenAuctionRequestVm, Auction>();
                p.ValidateInlineMaps = false;
            });

            AuctionManagementService service = new AuctionManagementService();

            //service.ElectWinnerInAuction(1);

            //AuctionInfoVM ainfo = service.GetAuctionInfo(1);

            //List<string> files = new List<string>();
            //files.Add(@"F:\Work\Замена масла.cdr");
            //files.Add(@"F:\Work\Замена масла1.cdr");

            //service.addFilesToAuction(files, 1);            

            //service.RevokeBidFromAuction(1);

            service.OpenAuction(new OpenAuctionRequestVm()
            {
                Description = "Yjdsq",
                ShippingAddress = "Almaty",
                ShippingConditions = "",
                PriceAtStart = 700000,
                PriceChangeStep = 20000,
                PriceAtMinimum = 50000,
                StartDate = DateTime.Now,
                FinishDateExpected = DateTime.Now.AddDays(5),
                AuctionCategory = "12"
            }, 1);


            //service.MakeBidToAuction(new MakeBidVm()
            //{
            //    AuctionId = 1,
            //    Description = "Поставим настоящуую шерсть баранов из Мадагаскара!",
            //    OrganizationId = 2,
            //    Price = 600000
            //});

            //service.MakeBidToAuction(new MakeBidVm()
            //{
            //    AuctionId = 1,
            //    Description = "Поставим настоящуую шерсть баранов из Келиманжару!",
            //    OrganizationId = 2,
            //    Price = 600000
            //});


            Console.ReadLine();
        }
    }
}
