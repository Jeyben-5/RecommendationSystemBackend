using DataAccess.Interfaces;
using Entities;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IRepository<Document> _documentRepository;

        public DocumentService(IRepository<Document> _documentRepository)
        {
            this._documentRepository = _documentRepository;
        }

        public Document AddDocument(Document document)
        {
            var newDocument = _documentRepository.Add(document);

            return newDocument;
        }

        public Document GeyById(int id)
        {
            return _documentRepository.FindById(id);
        }

        public ICollection<Document> ListDocuments()
        {
            var subject = _documentRepository.List;

            return subject.ToList();
        }
    }
}
