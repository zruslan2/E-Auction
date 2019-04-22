using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Auction.Core.DataModels
{
    public class OrganizationType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Organization> Organizations { get; set; }

        public OrganizationType()
        {
            Organizations = new List<Organization>();
        }
    }

    public class Organization
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Site { get; set; }
        public string IdentificationNumber { get; set; }
        public DateTime RegistrationDate { get; set; }

        public int OrganizationTypeId { get; set; }
        public OrganizationType OrganizationType { get; set; }       
        public virtual ICollection<Auction> Auctions { get; set; }
        public virtual ICollection<Bid> Bids { get;set; }
        public ICollection<OrganizationFile> OrganizationFiles { get; set; }
        public ICollection<OrganizationRating> OrganizationRatings { get; set; }
        public ICollection<Employee> Employees { get; set; }        
        public ICollection<Transaction> Transactions { get; set; }

        public Organization()
        {
            Auctions = new List<Auction>();
            Bids = new List<Bid>();
        }
    }

    public class OrganizationRating
    {
        public int Id { get; set; }
        public double Score { get; set; }

        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }

    public class OrganizationFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }        
        public byte[] ContentAsBase64 { get; set; }
        public DateTime CreatedAt { get; set; }

        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }

    public enum TransactionType { Deposit, Withdraw }
    public class Transaction
    {
        public int Id { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Sum { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }

        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}
