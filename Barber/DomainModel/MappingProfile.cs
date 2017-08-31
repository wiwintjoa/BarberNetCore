using AutoMapper;
using Barber.API.ViewModel;
using Barber.API.Variable;

namespace Barber.API.DomainModel
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<BarberUser, UserViewModel>()
            .ForMember(d => d.Status,
                op => op.ResolveUsing(o => MapStatus(o.Status)));
        }

        private static string MapStatus(int status)
        {            
            var statusBarber = (BarberEnum.Status)status;

            return statusBarber.ToString();
        }
    }
}
