using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;
using System;
using System.Linq;

namespace ContosoUniversity.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly SchoolContext _context;

        public IndexModel(SchoolContext context)
        {
            _context = context;
        }

        public PaginatedList<Student> Student { get;set; }
        public string NameSort { get; set; }
        public string DateSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            CurrentFilter = searchString;
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<Student> studentIQ = from s in _context.Student
                                            select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                studentIQ = studentIQ.Where(s => s.LastName.Contains(searchString) || s.FirstMidName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    studentIQ = studentIQ.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    studentIQ = studentIQ.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    studentIQ = studentIQ.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    studentIQ = studentIQ.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 3;
            Student = await PaginatedList<Student>.CreateAsync(
                studentIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
