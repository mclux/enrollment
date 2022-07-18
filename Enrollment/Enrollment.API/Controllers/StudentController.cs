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
    public class StudentController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;

        public StudentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _unitOfWork.StudentRepository.GetAll();

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
            var studentRecord = await _unitOfWork.StudentRepository.Get(Id);
            if (studentRecord.IsDeleted)
            {
                return NotFound($"Student with ID {Id} does not exist.");
            }

            return Ok(studentRecord);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Student student)
        {           

            if(string.IsNullOrEmpty(student.StudentName))
            {
                return BadRequest("Student name is required");
            }

            if (string.IsNullOrEmpty(student.StudentNo))
            {
                return BadRequest("Student number is required");
            }

            student.Created = DateTime.Now;
            _unitOfWork.StudentRepository.Add(student);
            bool isCompleted = await _unitOfWork.Complete();
            if (isCompleted)
            {
                return Ok("Student successfully created.");
            }

            return BadRequest("Please try again.");
        }


        [HttpPut("{Id}")]
        public async Task<IActionResult> Put(long Id, [FromBody] Student value)
        {
            var studentRecord = await _unitOfWork.StudentRepository.Get(value.Id);
            if (studentRecord == null)
            {
                return NotFound($"Student with ID {value.Id} does not exist.");
            }

            studentRecord.StudentName = value.StudentName;
            studentRecord.StudentNo = value.StudentNo;
            studentRecord.LastModified = DateTime.Now;
            _unitOfWork.StudentRepository.Update(studentRecord);
            bool isCompleted = await _unitOfWork.Complete();
            if (isCompleted)
            {
                return Ok("Student record updated successfully.");
            }
            else
            {
                return BadRequest("Record not updated. Please try again.");
            }
        }


        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(long Id)
        {
            var studentRecord = await _unitOfWork.StudentRepository.Get(Id);
            if (studentRecord == null)
            {
                return NotFound($"Student with ID {Id} does not exist.");
            }

            if (studentRecord.IsDeleted)
            {
                return NotFound($"Student with ID {Id} does not exist.");
            }

            studentRecord.IsDeleted = true;
            studentRecord.LastModified = DateTime.Now;
            _unitOfWork.StudentRepository.Update(studentRecord);
            bool isCompleted = await _unitOfWork.Complete();
            if (isCompleted)
            {
                return Ok("Student deleted.");
            }
            else
            {
                return BadRequest("Student not deleted. Please try again.");
            }
        }

    }
}
