using Project21AS.DataContext;
using Project21AS.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Query;

namespace Project21AS.Server.Controllers.Api.OData
{
    [Authorize]
    public class DetailsController : ODataController
    {
        public ILogger<DetailsController> Logger { get; }
        public AppDbContext DbContext { get; }
        public DetailsController(ILogger<DetailsController> logger, AppDbContext dbContext)
        {
            Logger = logger;  
            DbContext = dbContext;
        }

        [EnableQuery]

        public IQueryable<Details> Get()
        {
            var data = DbContext.Details.AsQueryable();
            return data;
        }
    }
}
