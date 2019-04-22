using E_Auction.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Auction.Core.Exceptions
{
    public class OpenAuctionProcessException : Exception
    {
        public OpenAuctionRequestVm ModelOnException { get; set; }
        public string ExceptionText { get; set; }
        public DateTime ExceptionTime { get; set; }

        public OpenAuctionProcessException(OpenAuctionRequestVm model, string text)
            : base(text)
        {
            ModelOnException = model;
            ExceptionText = text;
            ExceptionTime = DateTime.Now;
        }
    }
}
