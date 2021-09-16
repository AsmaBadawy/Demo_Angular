using System;
using System.Collections.Generic;

namespace APIDEMO.Models
{
    public partial class UserInfo
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
      //  public decimal BookPrice { get; set; }
        public DateTime Date { get; set; }
    }
}
