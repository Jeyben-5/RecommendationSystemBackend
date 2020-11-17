using Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Collections.Generic;

namespace SchoolSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModeratorsController : ControllerBase
    {
        private readonly IModeratorService _moderatorService;
        public ModeratorsController(IModeratorService moderatorService) {
            this._moderatorService = moderatorService;
        }

        [HttpPost]
        [Route("")]
        public ActionResult<Moderator> Post(Moderator moderator)
        {

            var newClient = _moderatorService.AddModerator(moderator);

            return Created("api/moderators", newClient);
        }

        [HttpGet]
        [Route("")]
        public ActionResult<ICollection<Moderator>> Get()
        {
            return Ok(_moderatorService.ListModerators());
        }
    }
}