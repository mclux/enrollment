using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enrollment.Core.Entities
{
    public class StudentCourse :BaseEntity
    {
        public long StudentId { get; set; }
        public long CourseId { get; set; }
        public bool IsActive { get; set; }
    }
}
