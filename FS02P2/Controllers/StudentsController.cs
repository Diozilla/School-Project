using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FS02P2.Data;
using FS02P2.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using FS02P2.Models.SchoolViewModels;
using System.Configuration;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace FS02P2.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;

        public StudentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Students
        [AllowAnonymous]
        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber, string currentFilter, DateTime? startDate, DateTime? endDate)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["NameSortParm2"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["StartDate"] = startDate;
            ViewData["EndDate"] = endDate;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            var students = from s in _context.Students
                           select s;


            if (startDate != null && endDate != null)
            {
                students = students.Where(s => s.EnrollmentDate >= startDate && s.EnrollmentDate <= endDate);
            }


            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString) || s.FirstMidName.Contains(searchString));

            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "name_desc2":
                    students = students.OrderByDescending(s => s.FirstMidName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName).ThenBy(s => s.FirstMidName);
                    break;
            }
            int pageSize = 3;
            return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        [Authorize(Roles ="RegularUser")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.StudentID == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create

        [Authorize(Roles ="SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentDate,FirstMidName,LastName")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", ex.Message);

            }

            return View(student);
        }

        // GET: Students/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Students == null)
        //    {
        //        return NotFound();
        //    }

        //    var student = await _context.Students
        //        .Include(e => e.Enrollments)
        //        .ThenInclude(c => c.Course)
        //        .FirstOrDefaultAsync(s => s.StudentID == id);
        //    if (student == null)
        //    {
        //        return NotFound();
        //    }
        //    var courseg = student.Enrollments.Select(e => new StudentCourseg
        //    {
        //        CourseID = e.CourseID,
        //        Grade = e.Grade ?? Grade.NA
        //    }).ToList();
        //    ViewBag.Courseg = courseg;

        //    popEnrollmentsAndCourses(student);
        //    return View(student);
        //}
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(e => e.Enrollments)
                .ThenInclude(c => c.Course)
                .FirstOrDefaultAsync(s => s.StudentID == id);
            if (student == null)
            {
                return NotFound();
            }
            var courseg = student.Enrollments.Select(e => new StudentCourseg
            {
                CourseID = e.CourseID,
                Grade = e.Grade ?? Grade.NA
            }).ToList();
            ViewBag.Courseg = courseg;

            popEnrollmentsAndCourses(student);
            return View(student);
        }

       
        public void popEnrollmentsAndCourses(Student student)
        {
            var courses = _context.Courses;
            var viewModel = new List<AssignedCourseData>();
            var Scourses = new HashSet<int>(student.Enrollments.Select(c => c.CourseID));
            foreach (var course in courses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = Scourses.Contains(course.CourseID)
                });
            }

            ViewData["Courses"] = viewModel;
        }
        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Dictionary<int, Grade?> selectedg, string[] selectedc)
        {
            var sUpdate = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(s => s.StudentID == id);

            if (sUpdate == null)
            {
                return NotFound();
            }

            UsG(selectedg, sUpdate);
            UsC(selectedc, sUpdate);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sUpdate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(sUpdate.StudentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw new Exception("Concurrency error occurred while updating the student.");
                    }
                }
            }

            popEnrollmentsAndCourses(sUpdate);
            return View(sUpdate);
        }


        //public async Task<IActionResult> Edit(int id, Dictionary<int, Grade?> selectedg, string[] selectedc)
        //{
        //    var sUpdate = await _context.Students
        //        .Include(s => s.Enrollments)
        //        .ThenInclude(s => s.Course)
        //        .FirstOrDefaultAsync(s => s.StudentID == id);
        //    if (id != sUpdate.StudentID)
        //    {
        //        return NotFound();
        //    }
        //    UsG(selectedg, sUpdate);
        //    UsC(selectedc, sUpdate);
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(sUpdate);
        //            await _context.SaveChangesAsync();

        //        }
        //        catch (DbUpdateConcurrencyException /* ex */)
        //        {
        //            if (!StudentExists(sUpdate.StudentID))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));

        //    }
        //    UsG(selectedg, sUpdate);
        //    UsC(selectedc, sUpdate);
        //    popEnrollmentsAndCourses(sUpdate);
        //    return View(sUpdate);
        //}


        
        public void UsG(Dictionary<int, Grade?> selectedg, Student sUpdate)
        {
            if (selectedg?.Count == 0)
            {
                return;
            }

            foreach (var e in sUpdate.Enrollments)
            {
                if (selectedg?.TryGetValue(e.CourseID, out var sGrade) == true)
                {
                    e.Grade = sGrade;
                }
            }
        }

        //public void UsG(Dictionary<int, Grade?> selectedg, Student sUpdate)
        //{
        //    if (selectedg == null)
        //    {
        //        return;
        //    }
        //    foreach (var e in sUpdate.Enrollments)
        //    {
        //        if (selectedg.TryGetValue(e.CourseID, out var sGrade))
        //        {
        //            e.Grade = sGrade;
        //        }
        //    }
        //}
        
        public void UsC(string[] selectedc, Student sUpdate)
        {
            if (selectedc == null)
            {
                sUpdate.Enrollments = new List<Enrollment>();
                return;
            }

            var hSelected = new HashSet<string>(selectedc);
            var sCourses = new HashSet<int>(sUpdate.Enrollments.Select(s => s.CourseID));

            foreach (var c in _context.Courses)
            {
                if (hSelected.Contains(c.CourseID.ToString()))
                {
                    if (!sCourses.Contains(c.CourseID))
                    {
                        sUpdate.Enrollments.Add(new Enrollment { StudentID = sUpdate.StudentID, CourseID = c.CourseID });
                    }
                }
                else
                {
                    if (sCourses.Contains(c.CourseID))
                    {
                        Enrollment enrollmentToDelete = sUpdate.Enrollments.FirstOrDefault(i => i.CourseID == c.CourseID);
                        _context.Remove(enrollmentToDelete);
                    }
                }
            }
        }

        //public void UsC(string[] selectedc, Student sUpdate)
        //{
        //    if (selectedc == null)
        //    {
        //        sUpdate.Enrollments = new List<Enrollment>();
        //        return;
        //    }

        //    var hSelected = new HashSet<string>(selectedc);
        //    var sCourses = new HashSet<int>(sUpdate.Enrollments.Select(s => s.CourseID));
        //    foreach (var c in _context.Courses)
        //    {
        //        if (selectedc.Contains(c.CourseID.ToString()))
        //        {
        //            if (!sCourses.Contains(c.CourseID))
        //            {
        //                sUpdate.Enrollments.Add(new Enrollment { StudentID = sUpdate.StudentID, CourseID = c.CourseID });
        //            }
        //        }
        //        else
        //        {
        //            if (sCourses.Contains(c.CourseID))
        //            {
        //                Enrollment cDel = sUpdate.Enrollments.FirstOrDefault(i => i.CourseID == c.CourseID);
        //                _context.Remove(cDel);
        //            }

        //        }
        //    }
        //}

        // GET: Students/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.StudentID == id);
            if (student ==null)
            {
              
                return NotFound();
            }

            return View(student);
        }
        [Authorize(Roles = "SuperAdmin")]
        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'SchoolContext.Students'  is null.");
            }
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return (_context.Students?.Any(e => e.StudentID == id)).GetValueOrDefault();
        }
    }
 }

