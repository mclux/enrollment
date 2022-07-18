using Enrollment.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enrollment.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Student> StudentRepository { get; }
        IRepository<Course> CourseRepository { get; }
        IRepository<StudentCourse> StudentCourseRepository { get; }
        Task<bool> Complete();
    }
}
