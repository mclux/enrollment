using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enrollment.Core.Entities
{
    public class Student :BaseEntity
    {
        public string StudentName { get; set; }
        public string StudentNo { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}
