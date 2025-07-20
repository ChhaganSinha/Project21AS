using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Project21AS.DataContext;
using Project21AS.Dto;

namespace Project21AS.Server.Controllers.Api.OData
{
    [Authorize]
    public class StudentController : ODataController
    {
        public ILogger<StudentController> Logger { get; }
        public AppDbContext DbContext { get; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        public StudentController(ILogger<StudentController> logger, AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            Logger = logger;
            DbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        [EnableQuery]

        public IQueryable<Student> Get()
        {
            // Access the username from the HttpContext
            string _userName = _httpContextAccessor.HttpContext.User.Identity.Name;
            IQueryable<Student> data;
            if (!string.Equals(_userName, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                 data = DbContext.Student.Where(x=>x.Admin == _userName).AsQueryable();
            }
            else
            {
                data = DbContext.Student.AsQueryable();
            }
            return data;
        }
    }
}
