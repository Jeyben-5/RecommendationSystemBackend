using Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace RecomendationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            this._subjectService = subjectService;
        }

        [HttpPost]
        [Route("")]
        public ActionResult<Subject> Post(Subject subject)
        {
            return _subjectService.AddSubject(subject);
        }

        [HttpGet]
        [Route("")]
        public ActionResult<ICollection<Subject>> Get()
        {
            return Ok(_subjectService.ListSubjects());
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<Subject> GetById(int id)
        {
            var subject = _subjectService.GeyById(id);

            return Ok(subject);
        }

        [HttpPatch]
        [Route("{id}")]
        public ActionResult<Subject> Update(Subject subject)
        {
            _subjectService.UpdateSubject(subject);

            return Ok(subject);
        }
    }
}
