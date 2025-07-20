using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Project21AS.DataContext;
using Project21AS.Dto;

namespace Project21AS.Server.Controllers.Api.OData
{
    [Authorize]
    public class BatchController : ODataController
    {
        private readonly ILogger<BatchController> _logger;
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BatchController(ILogger<BatchController> logger, AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        [EnableQuery]
        public IQueryable<Batch> Get()
        {
            string _userName = _httpContextAccessor.HttpContext.User.Identity.Name;
            IQueryable<Batch> data;

            if (!string.Equals(_userName, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                data = _dbContext.Batch
                    .Where(x => x.Admin == _userName)
                    .Select(batch => new Batch
                    {
                        Id = batch.Id,
                        Name = batch.Name,
                        Admin = batch.Admin,
                        Username = batch.Username,
                        Status = batch.Status,
                        AllowedBatches = batch.AllowedBatches,
                        StudentCount = _dbContext.Student.Count(s => s.Batch == batch.Name),
                    })
                    .AsQueryable();
            }
            else
            {
                data = _dbContext.Batch
                    .Select(batch => new Batch
                    {
                        Id = batch.Id,
                        Name = batch.Name,
                        Admin = batch.Admin,
                        Username = batch.Username,
                        Status = batch.Status,
                        AllowedBatches = batch.AllowedBatches,
                        StudentCount = _dbContext.Student.Count(s => s.Batch == batch.Name),
                    })
                    .AsQueryable();
            }

            return data;
        }

    }
}
