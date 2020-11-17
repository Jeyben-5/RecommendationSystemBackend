using Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Collections.Generic;

namespace RecomendationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController (IDocumentService _documentService)
        {
            this._documentService = _documentService;
        }

        [HttpPost]
        [Route("")]
        public ActionResult<Document> Post(Document subject)
        {
            return _documentService.AddDocument(subject);
        }

        [HttpGet]
        [Route("")]
        public ActionResult<ICollection<Document>> Get()
        {
            return Ok(_documentService.ListDocuments());
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<Document> GetById(int id)
        {
            var subject = _documentService.GeyById(id);

            return Ok(subject);
        }
    }
}
