using Project21AS.Dto;
using Project21AS.Dto.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project21AS.Repositories
{
    public interface IAppRepository
    {
        Task<Details> GetDetailsById(int id);
        Task<IEnumerable<Details>> GetAllDetails();
        Task<Dashboard> GetDashboardStatistics(string userName);
    }
}
