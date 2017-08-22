using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Barber.API.DomainModel
{
    public class BarberRole : IdentityRole
    {
        public string Description
        {
            get;
            set;
        }
    }
}
