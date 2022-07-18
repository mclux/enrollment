using Enrollment.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enrollment.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EnrollContext _enrollContext;

        public UnitOfWork(EnrollContext enrollContext)
        {
            _enrollContext = enrollContext;
        }


        private IRepository<Student> studentRepository;
        public IRepository<Student> StudentRepository => studentRepository ?? new Repository<Student>(_enrollContext);


        private IRepository<Course> courseRepository;
        public IRepository<Course> CourseRepository => courseRepository ?? new Repository<Course>(_enrollContext);


        private IRepository<StudentCourse> studentCourseRepository;
        public IRepository<StudentCourse> StudentCourseRepository => studentCourseRepository ?? new Repository<StudentCourse>(_enrollContext);

        public async Task<bool> Complete() => await _enrollContext.SaveChangesAsync() > 0;

        public void Dispose()
        {
            _enrollContext.Dispose();
        }

    }
}
