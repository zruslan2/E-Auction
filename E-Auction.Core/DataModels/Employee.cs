using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Auction.Core.DataModels
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }        
        public DateTime DoB { get; set; }
        public string Email { get; set; }

        public int EmployeePositionId { get; set; }
        public EmployeePosition EmployeePosition { get; set; }

        //public int UserId { get; set; }
        public User User { get; set; }

        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }

    public class EmployeePosition
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
