using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using StudentPortal.Data;
using StudentPortal.Models;
using StudentPortal.Models.Entities;

namespace StudentPortal.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public StudentsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddStudentViewController viewModel)
        {
            var student = new Student
            {
                Name = viewModel.Name,
                Email = viewModel.Email,
                Phone = viewModel.Phone,
                Subscribed = viewModel.Subscribed,
            };

            await dbContext.AddAsync(student);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("StudentList", "Students");
        }

        //[HttpGet]
        public async Task<IActionResult> StudentList(string searchString)
        {
            //var _studentList = await dbContext.Students.ToListAsync();
            //return View(_studentList);

            if (dbContext.Students == null)
            {
                return Problem("Entity is Empty!");
            }

            var student = from s in dbContext.Students
                          select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                student = student.Where(s => s.Name!.Contains(searchString));
            }
            return View(await student.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var student = await dbContext.Students.FindAsync(id);
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Student studentModel)
        {
            var student = await dbContext.Students.FindAsync(studentModel.Id);

            if (student != null)
            {
                student.Name = studentModel.Name;
                student.Email = studentModel.Email;
                student.Phone = studentModel.Phone;
                student.Subscribed = studentModel.Subscribed;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("StudentList", "Students");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Student deleteModel)
        {
            var student = await dbContext.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == deleteModel.Id);

            if (student != null)
            {
                dbContext.Students.Remove(deleteModel);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("StudentList", "Students");
        }
    }
}
