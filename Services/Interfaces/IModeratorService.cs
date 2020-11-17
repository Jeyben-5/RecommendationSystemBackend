using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interfaces
{
    public interface IModeratorService
    {
        Moderator AddModerator(Moderator client);
        ICollection<Moderator> ListModerators();   
    }
}
