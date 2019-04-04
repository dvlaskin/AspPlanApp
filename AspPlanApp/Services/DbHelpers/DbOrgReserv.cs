using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AspPlanApp.Data;
using AspPlanApp.Models;
using AspPlanApp.Models.DbModels;
using AspPlanApp.Models.ManageUsersViewModels;
using AspPlanApp.Models.ReservViewModels;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AspPlanApp.Services.DbHelpers
{
    public class DbOrgReserv : IDbOrgReserv
    {
        private static AppDbContext _dbContext;
        private static IConfiguration _config;

        public DbOrgReserv(AppDbContext dbContext,
            IConfiguration config,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManger)
        {
            _dbContext = dbContext;
            _config = config;
        }

        public async Task<List<ReservItemsViewModel>> GetOrgReservByWeek(int orgId, DateTime dateCal, string currUser)
        {
            List<ReservItemsViewModel> result = new List<ReservItemsViewModel>();

            if (orgId == 0 || dateCal == DateTime.MinValue)
                return result;
            
            DateTime dateFrom = new DateTime(dateCal.Year, dateCal.Month, 1);;
            DateTime dateTo = dateFrom.AddMonths(1).AddDays(-1);

            await Task.Run(async () =>
            {
                ConnectionDb conn = new ConnectionDb(_config);

                using (IDbConnection sqlCon = conn.GetConnection)
                {
                    string query = @"
                        select
                            cl.orgId,
                            ifnull(org.orgName,'') as orgName,
                            ifnull(fn.userName,ow.userName) as staffName,
							case
								when cl.userId = @currUser then 1
								else 0
							end as isMy,
                            cl.dateFrom,
                            cl.dateTo,
                            cl.isConfirm,
                            cl.comment
                        from orgReserve cl
                        left join org on org.orgId = cl.orgId
                        left join orgStaff st on st.orgStaffId = cl.orgStaffId
                        left join user fn on fn.id = st.staffId 
						left join user ow on ow.id = org.owner	
                        where cl.orgId = @orgId 
                            and dateFrom between @dateFrom and @dateTo
                    ";

                    // Ensure that the connection state is Open
                    ConnectionDb.OpenConnect(sqlCon);

                    var resultQuery = await sqlCon.QueryAsync<ReservItemsViewModel>(
                        query, 
                        new
                        {
                            orgId = orgId, 
                            dateFrom = dateFrom, 
                            dateTo = dateTo,
                            currUser = currUser
                        });

                    result = resultQuery.ToList();
                }
            });

            return result;

        }
        
    }
}