using DataAccess.Interfaces;
using Entities;
using Microsoft.IdentityModel.Protocols.WSIdentity;
using Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class ModeratorService : IModeratorService
    {
        private readonly IRepository<Moderator> _moderatorRepository;
        public ModeratorService(IRepository<Moderator> moderatorRepository)
        {
            this._moderatorRepository = moderatorRepository;
        }
        public Moderator AddModerator(Moderator moderator)
        {
            var newModerator = _moderatorRepository.Add(moderator);

            return newModerator;
        }

        public ICollection<Moderator> ListModerators()
        {
            var moderator = _moderatorRepository.List;

            return moderator.ToList();
        }
    }
}
