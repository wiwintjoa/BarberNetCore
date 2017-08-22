using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;

namespace Barber.API.DomainModel
{
    public class BarberUser: IdentityUser
    {
        public int Status
        {
            get;
            set;
        }

        public DateTime CreateDateTime
        {
            get;
            set;
        }

        public string CreateUser
        {
            get;
            set;
        }

        public string ModifiedBy
        {
            get;
            set;
        }

        public DateTime? ModifiedDateTime
        {
            get;
            set;
        }       
    }
}
