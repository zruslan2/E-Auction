using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Auction.Core.DataModels
{
    public class User
    {
        [Key]
        [ForeignKey("Employee")]
        public int Id { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public int FailedSignInCount { get; set; }        
        public DateTime CreatedDate { get; set; }

        public Employee Employee { get; set; }
        //public int EmployeeId { get; set; }
        public ICollection<UserPasswordHistory> UserPasswordHistories { get; set; }
        public ICollection<UserAutorizationHistory> UserAutorizationHistory { get; set; }
    }

    public class UserPasswordHistory
    {
        public int Id { get; set; }
        public DateTime SetupDate { get; set; }
        public DateTime? InvalidatedDate { get; set; }
        public string Password { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }

    public class UserAutorizationHistory
    {
        public int Id { get; set; }
        public DateTime AutorizationTime { get; set; }
        public string MachineIp { get; set; }
        public string IpToGeoCountry { get; set; }
        public string IpToGeoCity { get; set; }
        public double IpToGeoLatitude { get; set; }
        public double IpToGeoLongitude { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
