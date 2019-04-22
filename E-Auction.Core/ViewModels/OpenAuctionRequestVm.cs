using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Auction.Core.ViewModels
{
    public class OpenAuctionRequestVm
    {
        public string Description { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingConditions { get; set; }
        public decimal PriceAtStart { get; set; }
        public decimal PriceChangeStep { get; set; }
        public decimal PriceAtMinimum { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDateExpected { get; set; }
        public string AuctionCategory { get; set; }
        public List<string> UploadFiles { get; set; }
    }
}
