using Entities;
using System.Collections;
using System.Collections.Generic;

namespace Services.Interfaces
{
    public interface ISubjectService
    {
        Subject AddSubject(Subject subject);
        ICollection<Subject> ListSubjects();
        void UpdateSubject(Subject subject);
        Subject GeyById(int id);

    }
}
