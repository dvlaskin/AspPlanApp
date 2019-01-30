using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AspPlanApp.Data;
using AspPlanApp.Models;
using AspPlanApp.Models.DbModels;
using AspPlanApp.Models.ManageUsersViewModels;
using Dapper;
using Microsoft.Extensions.Configuration;


namespace AspPlanApp.Services.DbHelpers
{
    public class ManageUsersServ
    {
        private AppDbContext _dbContext;
        private IConfiguration _config;

        public ManageUsersServ(AppDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }
        
        public async Task<EditOwnerViewModel> GetOwnerInfoAcync(User user)
        {
            if (user == null) return null;

            return await Task.Run(async () =>
            {
                EditOwnerViewModel result = new EditOwnerViewModel()
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email,
                    NewPassword = string.Empty,
                    OldPassword = string.Empty,
                    ConfirmNewPassword = string.Empty
                };
                
                result.Orgs = _dbContext.Org
                    .Where(w => w.owner == user.Id).ToList();
                
                ConnectionDb conn = new ConnectionDb(_config);

                using (IDbConnection sqlCon = conn.GetConnection)
                {
                    string query = @"
                        select 
                            u.UserName,
                            u.Email as emailUser,
                            s.orgStaffId,
                            s.isConfirm,
                            s.orgId,
                            s.staffId
                        from org o
                        join orgStaff s on s.orgId = o.orgId
                        join user u on u.id = s.staffId
                        where o.owner = @userId
                        ";
                    
                    // Ensure that the connection state is Open
                    ConnectionDb.OpenConnect(sqlCon);
                    
                    var resultQuery = await sqlCon.QueryAsync<OwnerOrgStaffViewModel>(query, new { userId = user.Id });

                    result.Staff = resultQuery.ToList();
                }

                return result;
            });
        }
    }
}