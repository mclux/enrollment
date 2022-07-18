using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enrollment.Core.Entities
{
    public class Course :BaseEntity
    {
        public string CourseName { get; set; }
        public string CourseCode { get; set; }

        public ICollection<Student> Students { get; set; }
    }
}
