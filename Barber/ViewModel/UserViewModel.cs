using System;

namespace Barber.API.ViewModel
{
    public class UserViewModel
    {
        public string Id { get; set; }
                        
        public string FirstName { get; set; }
                        
        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        
        public DateTime DOB { get; set; }

        public string Status { get; set; }

        public string Address { get; set; }      

        public string UserName { get; set; }        
    }
}
