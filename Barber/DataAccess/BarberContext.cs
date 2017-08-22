using Barber.API.DomainModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Barber.API.DataAccess
{
    public class BarberContext : IdentityDbContext<BarberUser>
    {
        public BarberContext(DbContextOptions<BarberContext> options) : base(options)
        {
        }
    }
}
