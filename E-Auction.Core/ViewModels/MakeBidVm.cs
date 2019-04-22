using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Auction.Core.ViewModels
{
    public class MakeBidVm
    {
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int AuctionId { get; set; }
        public int OrganizationId { get; set; }
    }
}
