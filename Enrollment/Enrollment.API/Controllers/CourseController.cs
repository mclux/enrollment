using Enrollment.Core.Entities;
using Enrollment.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Enrollment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;

        public CourseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _unitOfWork.CourseRepository.GetAll();

            if(result.Any())
            {
                result = result.Where(p => !p.IsDeleted);
            }

            return Ok(result);
        }


        [HttpGet]
        [Route("{Id:int}")]
        public async Task<IActionResult> Get(long Id)
        {
            var courseRecord = await _unitOfWork.CourseRepository.Get(Id);
            if (courseRecord.IsDeleted)
            {
                return NotFound($"Course with ID {Id} does not exist.");
            }

            return Ok(courseRecord);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Course course)
        {           

            if(string.IsNullOrEmpty(course.CourseName))
            {
                return BadRequest("Course name is required");
            }

            if (string.IsNullOrEmpty(course.CourseCode))
            {
                return BadRequest("Course code is required");
            }

            course.Created = DateTime.Now;
            _unitOfWork.CourseRepository.Add(course);
            bool isCompleted = await _unitOfWork.Complete();
            if (isCompleted)
            {
                return Ok("Course successfully created.");
            }

            return BadRequest("Please try again.");
        }


        [HttpPost]
        [Route("RegisterCourse")]
        public async Task<IActionResult> RegisterCourse([FromBody] StudentCourse record)
        {

            var courseRecord = await _unitOfWork.CourseRepository.Get(record.CourseId);
            if (courseRecord==null || courseRecord.IsDeleted)
            {
                return NotFound($"Course with ID {record.Id} does not exist.");
            }

            var studentRecord = await _unitOfWork.StudentRepository.Get(record.StudentId);
            if (studentRecord==null || studentRecord.IsDeleted)
            {
                return NotFound($"Student with ID {record.StudentId} does not exist.");
            }

            var studentCourses = await _unitOfWork.StudentCourseRepository.GetWhere(p => p.StudentId == record.StudentId);
            if (studentCourses.Any())
            {
                var activeCourses = studentCourses.Where(p => p.IsActive).ToList();
                if (activeCourses.FirstOrDefault(p=>p.CourseId==record.CourseId)!=null)
                {
                    return BadRequest($"Student already registered for course with ID {record.CourseId}");
                }
            }

            if(studentCourses.Any())
            {
                var activeCourses = studentCourses.Where(p => p.IsActive).ToList();
                if (activeCourses.Count>=3)
                {
                    return BadRequest("Student can not have more than three active courses");
                }
            }

            record.Created = DateTime.Now;
            _unitOfWork.StudentCourseRepository.Add(record);
            bool isCompleted = await _unitOfWork.Complete();
            if (isCompleted)
            {
                return Ok("Course successfully registered.");
            }

            return BadRequest("Please try again.");
        }

        [HttpPost]
        [Route("RemoveStudentCourse")]
        public async Task<IActionResult> RemoveStudentCourse([FromBody] StudentCourse record)
        {

            var studentCourses = await _unitOfWork.StudentCourseRepository.GetWhere(p => p.StudentId == record.StudentId && p.CourseId==record.CourseId);
            if (!studentCourses.Any())
            {
                return BadRequest($"Student not registered for course.");
            }

            var courseToRemove = studentCourses.FirstOrDefault();

            courseToRemove.LastModified = DateTime.Now;
            courseToRemove.IsActive = false;
            _unitOfWork.StudentCourseRepository.Update(record);
            bool isCompleted = await _unitOfWork.Complete();
            if (isCompleted)
            {
                return Ok("Course successfully removed.");
            }

            return BadRequest("Please try again.");
        }


        [HttpPut("{Id}")]
        public async Task<IActionResult> Put(long Id, [FromBody] Course value)
        {
            var courseRecord = await _unitOfWork.CourseRepository.Get(value.Id);
            if (courseRecord == null)
            {
                return NotFound($"Course with ID {value.Id} does not exist.");
            }

            courseRecord.CourseName = value.CourseName;
            courseRecord.CourseCode = value.CourseCode;
            courseRecord.LastModified = DateTime.Now;
            _unitOfWork.CourseRepository.Update(courseRecord);
            bool isCompleted = await _unitOfWork.Complete();
            if (isCompleted)
            {
                return Ok("Course record updated successfully.");
            }
            else
            {
                return BadRequest("Record not updated. Please try again.");
            }
        }


        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(long Id)
        {
            var courseRecord = await _unitOfWork.CourseRepository.Get(Id);
            if (courseRecord == null)
            {
                return NotFound($"Course with ID {Id} does not exist.");
            }

            if (courseRecord.IsDeleted)
            {
                return NotFound($"Course with ID {Id} does not exist.");
            }

            courseRecord.IsDeleted = true;
            courseRecord.LastModified = DateTime.Now;
            _unitOfWork.CourseRepository.Update(courseRecord);
            bool isCompleted = await _unitOfWork.Complete();
            if (isCompleted)
            {
                return Ok("Course deleted.");
            }
            else
            {
                return BadRequest("Course not deleted. Please try again.");
            }
        }

    }
}
