using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interfaces
{
    public interface IDocumentService
    {
        Document AddDocument(Document subject);
        ICollection<Document> ListDocuments();
        Document GeyById(int id);
    }
}
