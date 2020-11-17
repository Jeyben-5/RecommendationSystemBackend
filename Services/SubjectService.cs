using DataAccess.Interfaces;
using Entities;
using Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class SubjectService : ISubjectService
    {
        private readonly IRepository<Subject> _subjectRepository;

        public SubjectService(IRepository<Subject> subjectRepository) 
        {
            this._subjectRepository = subjectRepository;
        }
        public Subject AddSubject(Subject subject)
        {
            var newSubject = _subjectRepository.Add(subject);

            return newSubject;
        }

        public Subject GeyById(int id)
        {
            return _subjectRepository.FindById(id);
        }

        public ICollection<Subject> ListSubjects()
        {
            var subject = _subjectRepository.List;

            return subject.ToList();
        }

        public void UpdateSubject(Subject subject)
        {
            _subjectRepository.Update(subject);
        }
    }
}
