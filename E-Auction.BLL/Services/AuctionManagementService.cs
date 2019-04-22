using AutoMapper;
using E_Auction.Core.DataModels;
using E_Auction.Core.Exceptions;
using E_Auction.Core.ViewModels;
using E_Auction.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace E_Auction.BLL.Services
{
    public class AuctionManagementService
    {
        public enum FindCategory
        {
            AuctionCategory,
            Description,
            ShippingAddress,
            StartDate,
            Customer
        }
        private readonly AplicationDbContext _aplicationDbContext;
        public void OpenAuction(OpenAuctionRequestVm model, int organizationId)
        {
            if (model == null)
                throw new ArgumentNullException($"{typeof(OpenAuctionRequestVm).Name} is null");

            int maximumAllowedActiveAuctions = 3;

            var auctionsCheck = _aplicationDbContext
                .Organizations
                .Find(organizationId)
                .Auctions
                .Where(p => p.AuctionStatus == AuctionStatus.Active)
                .Count() < maximumAllowedActiveAuctions;

            var categoryCheck = _aplicationDbContext.AuctionCategories
                .SingleOrDefault(p => p.Name == model.AuctionCategory);

            if (categoryCheck == null)
                throw new Exception("Ошибка валидации модели!");

            if (!auctionsCheck)
                throw new OpenAuctionProcessException(model, "Превышено максимальное количество активных аукционов!");

            var auctionModel = Mapper.Map<Auction>(model);
            auctionModel.AuctionStatus = AuctionStatus.Active;
            auctionModel.Category = categoryCheck;
            auctionModel.OrganizationId = organizationId;
            _aplicationDbContext.Auctions.Add(auctionModel);
            _aplicationDbContext.SaveChanges();
        }
        public void addFilesToAuction(List<string> files, int auctionId)
        {
            if (files.Count != 0)
            {
                byte[] Fdata;
                foreach (var item in files)
                {
                    Fdata = null;
                    using (System.IO.FileStream fs = new System.IO.FileStream(item, FileMode.Open))
                    {
                        Fdata = new byte[fs.Length];
                        fs.Read(Fdata, 0, Fdata.Length);
                    }
                    AuctionFileMeta file = new AuctionFileMeta()
                    {
                        FileName = item.Substring(item.LastIndexOf('\\') + 1),
                        AuctionId = auctionId,
                        ContentAsBase64 = Fdata,
                        CreatedAt = DateTime.Now
                    };
                    _aplicationDbContext.AuctionFiles.Add(file);
                }
                _aplicationDbContext.SaveChanges();
            }
        }
        public void MakeBidToAuction(MakeBidVm model, decimal paymentForBids)
        {
            var bidExists = _aplicationDbContext.Bids
                .Any(p => p.Price == model.Price &&
                p.AuctionId == model.AuctionId &&
                p.Description == model.Description &&
                p.OrganizationId == model.OrganizationId);

            if (bidExists)
                throw new Exception("Invalid bid");

            var inValidPriceRange = _aplicationDbContext
                .Auctions.Where(p => p.Id == model.AuctionId &&
                p.PriceAtMinimum < model.Price &&
                p.PriceAtStart > model.Price);
            
            var inStepRange = inValidPriceRange
                .Any(p => (p.PriceAtStart - model.Price) % p.PriceChangeStep == 0);

            if (!inStepRange)
                throw new Exception("Invalid bid according price step");

            var organizationTransactions = _aplicationDbContext.Transactions
                .Where(p => p.OrganizationId == model.OrganizationId).ToList();
            
            if (organizationTransactions.Count == 0)
                throw new Exception("Organization has zero balance!");

            var organizationBalance = organizationTransactions
                .Where(p => p.TransactionType == TransactionType.Deposit)
                .Sum(p => p.Sum) -
                organizationTransactions
                .Where(p => p.TransactionType == TransactionType.Withdraw)
                .Sum(p => p.Sum);
            if (organizationBalance < paymentForBids)
                throw new Exception("Organization does not have enough money");

            using (var transaction1 = _aplicationDbContext.Database.BeginTransaction())
            {
                try
                {
                    Bid bid = new Bid()
                    {
                        Price = model.Price,
                        Description = model.Description,
                        AuctionId = model.AuctionId,
                        OrganizationId = model.OrganizationId,
                        CreatedDate = DateTime.Now,
                        BidStatus = BidStatus.Active
                    };
                    _aplicationDbContext.Bids.Add(bid);
                    _aplicationDbContext.SaveChanges();

                    Transaction transaction = new Transaction()
                    {
                        Sum = paymentForBids,
                        TransactionType = TransactionType.Withdraw,
                        TransactionDate = DateTime.Now,
                        OrganizationId = model.OrganizationId,
                        Description = $"Withdraw participation cost for bids {model.AuctionId}"
                    };
                    _aplicationDbContext.Transactions.Add(transaction);
                    _aplicationDbContext.SaveChanges();
                    transaction1.Commit();
                }
                catch (Exception ex)
                {
                    transaction1.Rollback();
                }
            }
        }        
        public void RevokeBidFromAuction(int BidId)
        {
            var bidExists = _aplicationDbContext.Bids
                .SingleOrDefault(p => p.Id == BidId);

            if (bidExists == null)
                throw new Exception("Такой ставки не существует!");

            var auctionFinishDate = _aplicationDbContext.Auctions.SingleOrDefault(p => p.Id == bidExists.AuctionId);
            if ((auctionFinishDate.FinishDateExpected - DateTime.Now).Days < 1)
                throw new Exception("Ставку нельзя удалить! До завершение аукциона осталось меньше 24 часов.");

            bidExists.BidStatus = BidStatus.Revoked;
            _aplicationDbContext.SaveChanges();
        }
        public AuctionInfoVM GetAuctionInfo(int AuctionId)
        {
            var AuctionExists = _aplicationDbContext.Auctions
                .SingleOrDefault(p => p.Id == AuctionId);

            if (AuctionExists == null)
                throw new Exception("Такого аукциона не существует!");

            var FileList = _aplicationDbContext.AuctionFiles
                .Where(p => p.AuctionId == AuctionExists.Id).ToList();

            List<string> files = new List<string>();

            foreach (var item in FileList)
            {
                files.Add(item.FileName);
            }

            AuctionInfoVM AuctionInfo = new AuctionInfoVM()
            {
                Id = AuctionExists.Id,
                AuctionCategory = _aplicationDbContext.AuctionCategories
                .SingleOrDefault(p => p.Id == AuctionExists.CategoryId).Name,
                Description = AuctionExists.Description,
                ShippingAddress = AuctionExists.ShippingAddress,
                PriceAtStart = AuctionExists.PriceAtStart,
                PriceChangeStep = AuctionExists.PriceChangeStep,
                PriceAtMinimum = AuctionExists.PriceAtMinimum,
                StartDate = AuctionExists.StartDate,
                FinishDateExpected = AuctionExists.FinishDateExpected,
                OrganizationName = _aplicationDbContext.Organizations
                .SingleOrDefault(p => p.Id == AuctionExists.OrganizationId).FullName,
                AuctionFiles = files
            };
            return AuctionInfo;
        }
        public List<AuctionInfoVM> FindAuction(FindCategory cFind, string value)
        {
            var ListAuctions = new List<AuctionInfoVM>();

            if (cFind == FindCategory.AuctionCategory)
            {
                var categoryId = _aplicationDbContext.AuctionCategories
                .SingleOrDefault(p => p.Name == value);

                if (categoryId != null)
                {
                    var Auctions = _aplicationDbContext.Auctions
                     .Where(p => p.CategoryId == categoryId.Id).ToList();

                    foreach (var item in Auctions)
                    {
                        ListAuctions.Add(GetAuctionInfo(item.Id));
                    }
                }
            }
            else if (cFind == FindCategory.Description)
            {
                var Auctions = _aplicationDbContext.Auctions
                    .Where(p => p.Description == value).ToList();

                foreach (var item in Auctions)
                {
                    ListAuctions.Add(GetAuctionInfo(item.Id));
                }
            }
            else if (cFind == FindCategory.ShippingAddress)
            {
                var Auctions = _aplicationDbContext.Auctions
                    .Where(p => p.ShippingAddress == value).ToList();

                foreach (var item in Auctions)
                {
                    ListAuctions.Add(GetAuctionInfo(item.Id));
                }
            }
            else if (cFind == FindCategory.Customer)
            {
                var customerId = _aplicationDbContext.Organizations
                    .SingleOrDefault(p => p.FullName == value);

                if (customerId != null)
                {
                    var Auctions = _aplicationDbContext.Auctions
                      .Where(p => p.OrganizationId == customerId.Id).ToList();

                    foreach (var item in Auctions)
                    {
                        ListAuctions.Add(GetAuctionInfo(item.Id));
                    }
                }

            }
            else if (cFind == FindCategory.StartDate)
            {
                DateTime sdate = DateTime.Parse(value);
                var Auctions = _aplicationDbContext.Auctions
                    .ToList()
                    .Where(p => p.StartDate.Date == sdate.Date).ToList();

                foreach (var item in Auctions)
                {
                    ListAuctions.Add(GetAuctionInfo(item.Id));
                }
            }
            return ListAuctions;
        }
        public void ElectWinnerInAuction(int BidId)
        {
            var bidExists = _aplicationDbContext.Bids
                .SingleOrDefault(p => p.Id == BidId);

            var auctionExist = _aplicationDbContext.Auctions
                .SingleOrDefault(p => p.Id == bidExists.AuctionId);

            var AuctionsBids = _aplicationDbContext.Bids.Where(p => p.AuctionId == auctionExist.Id);

            if (AuctionsBids != null && auctionExist != null)
            {
                using (var transaction = _aplicationDbContext.Database.BeginTransaction())
                {
                    try
                    {
                        auctionExist.WinnerBidId = BidId;
                        auctionExist.AuctionStatus = AuctionStatus.Finished;
                        auctionExist.FinishDateActual = DateTime.Now;
                        _aplicationDbContext.SaveChanges();
                        foreach (var item in AuctionsBids)
                        {
                            item.BidStatus = BidStatus.Finished;
                        }
                        _aplicationDbContext.SaveChanges();

                        emailModel model = new emailModel()
                        {
                            nameFrom = auctionExist.Organization.FullName,
                            from = auctionExist.Organization.Email,
                            to = bidExists.Organization.Email,
                            subject = $"You winner in auction: id-{auctionExist.Id}",
                            body = $"Contact customer:\nphone: {auctionExist.Organization.Phone}"
                        };
                        SendMail(model);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }
        private void SendMail(emailModel email)
        {            
            MailAddress from = new MailAddress(email.from, email.nameFrom);            
            MailAddress to = new MailAddress(email.to);            
            MailMessage mes = new MailMessage(from, to);            
            mes.Subject = email.subject;            
            mes.Body = email.body;            
            mes.IsBodyHtml = true;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            // логин и пароль
            smtp.Credentials = new NetworkCredential(email.from, "mypassword");
            smtp.EnableSsl = true;
            smtp.Send(mes);
        }
        public void RestartAuction(int AuctionId)
        {
            using (var transaction = _aplicationDbContext.Database.BeginTransaction())
            {
                try
                {
                    var AuctionExists = _aplicationDbContext.Auctions
                   .SingleOrDefault(p => p.Id == AuctionId);

                    AuctionExists.AuctionStatus = AuctionStatus.Closed;
                    _aplicationDbContext.SaveChanges();

                    Auction auction = new Auction()
                    {
                        Description = AuctionExists.Description,
                        ShippingAddress = AuctionExists.ShippingAddress,
                        ShippingConditions = AuctionExists.ShippingConditions,
                        PriceAtStart = AuctionExists.PriceAtStart,
                        PriceChangeStep = AuctionExists.PriceChangeStep,
                        PriceAtMinimum = AuctionExists.PriceAtMinimum,
                        StartDate = DateTime.Now,
                        FinishDateExpected = DateTime.Now.AddDays(5),
                        AuctionStatus = AuctionStatus.Active,
                        CategoryId = AuctionExists.CategoryId,
                        OrganizationId = AuctionExists.OrganizationId
                    };
                    _aplicationDbContext.Auctions.Add(auction);
                    _aplicationDbContext.SaveChanges();                       
                    int aid = auction.Id;
                    var files = _aplicationDbContext.AuctionFiles
                        .Where(p => p.AuctionId == AuctionExists.Id);
                    foreach (var item in files)
                    {
                        item.AuctionId = aid;
                    }
                    _aplicationDbContext.SaveChanges();
                    var Bids=_aplicationDbContext.Bids
                        .Where(p => p.AuctionId == AuctionExists.Id);
                    foreach (var item in Bids)
                    {
                        item.AuctionId = aid;
                    }
                    _aplicationDbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }
        public void GetRating(int PutsOrganizationId, int OrganizationId, double score)
        {
            var AuctionExist = _aplicationDbContext.Auctions
                .Where(p => p.OrganizationId == PutsOrganizationId)
                .Where(p => p.AuctionStatus == AuctionStatus.Finished).ToList();

            if(AuctionExist.Count!=0)
            {
                foreach (var item in AuctionExist)
                {
                    var BidExist = _aplicationDbContext.Bids
                        .SingleOrDefault(p => p.Id == item.WinnerBidId);
                    if(BidExist.OrganizationId== OrganizationId)
                    {
                        OrganizationRating raiting = new OrganizationRating()
                        {
                            Score = score,
                            OrganizationId = OrganizationId
                        };
                        _aplicationDbContext.OrganizationRatings.Add(raiting);
                        _aplicationDbContext.SaveChanges();
                    }
                }
            }
        }
        public AuctionManagementService()
        {
            _aplicationDbContext = new AplicationDbContext();
        }
    }
}
