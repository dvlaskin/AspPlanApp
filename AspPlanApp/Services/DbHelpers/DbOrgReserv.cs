﻿using System;
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
        private readonly IConfiguration _config;
        private readonly AppDbContext _dbContext;

        public DbOrgReserv(IConfiguration config, AppDbContext dbContext)
        {
            _config = config;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get all reserved events on week by the date
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="dateCal"></param>
        /// <param name="currUser"></param>
        /// <returns></returns>
        public async Task<List<ReservItemsViewModel>> GetOrgReservByWeek(int orgId, DateTime dateCal, string currUser)
        {
            List<ReservItemsViewModel> result = new List<ReservItemsViewModel>();

            if (orgId == 0 || dateCal == DateTime.MinValue)
                return result;

            DateTime dateFrom = new DateTime(dateCal.Year, dateCal.Month, 1);
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

        /// <summary>
        /// Add new reserve event to calendar of Company
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orgId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="staffId"></param>
        /// <param name="comm"></param>
        /// <returns></returns>
        public async Task AddNewEvent(string userId,int orgId,DateTime dateFrom,DateTime dateTo,int staffId,string comm)
        {
            OrgReserve resNew = new OrgReserve()
            {
                orgId = orgId,
                userId = userId,
                orgStaffId = staffId,
                dateFrom = dateFrom,
                dateTo = dateTo,
                isConfirm = false,
                Comment = comm
            };

            await _dbContext.OrgReserve.AddAsync(resNew);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Get User calendar reserved items
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="dateCal">calendar date</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<UserCalendarViewModel>> GetUserCalendar(string userId, DateTime dateCal)
        {
            List<UserCalendarViewModel> result = new List<UserCalendarViewModel>();

            if (string.IsNullOrEmpty(userId) || dateCal == DateTime.MinValue)
                return result;
            
            DateTime dateFrom = new DateTime(dateCal.Year, dateCal.Month, 1);
            DateTime dateTo = dateFrom.AddMonths(1).AddDays(-1);

            await Task.Run(async () =>
            {
                ConnectionDb conn = new ConnectionDb(_config);

                using (IDbConnection sqlCon = conn.GetConnection)
                {
                    string query = @"
                        select
                            cl.resId,
                            case
                                when ifnull(ow.id,'') = @userId then 1
                                else 0
                            end as isOwner,
                            cl.orgId,
                            ifnull(org.orgName,'') as orgName,
                            ifnull(fn.userName,ow.userName) as staffName,
                            cl.dateFrom,
                            cl.dateTo,
                            cl.isConfirm,
                            cl.comment
                        from orgReserve cl
                        left join org on org.orgId = cl.orgId
                        left join orgStaff st on st.orgStaffId = cl.orgStaffId
                        left join user fn on fn.id = st.staffId 
						left join user ow on ow.id = org.owner	
                        where dateFrom between @dateFrom and @dateTo
                            and (
                                    ifnull(cl.userId,'') = @userId 
                                    or ifnull(fn.id,'') = @userId 
                                    or ifnull(ow.id,'') = @userId
                                )
                    ";

                    // Ensure that the connection state is Open
                    ConnectionDb.OpenConnect(sqlCon);

                    var resultQuery = await sqlCon.QueryAsync<UserCalendarViewModel>(
                        query,
                        new
                        {
                            dateFrom = dateFrom,
                            dateTo = dateTo,
                            userId = userId
                        });

                    result = resultQuery.ToList();
                }
            });
            
            return result;
        }

        /// <summary>
        /// Confirm Reserved Event
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="resId"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public async Task ConfirmResEvent(string userId, int resId, bool isAdmin)
        {
            if (string.IsNullOrEmpty(userId) || resId == 0) return;

            int resultQuery = 0;
            
            await Task.Run(async () =>
            {
                ConnectionDb conn = new ConnectionDb(_config);

                using (IDbConnection sqlCon = conn.GetConnection)
                {
                    string query = @"
                        select
                            count(*) as kol
                        from orgreserve r
                        left join org on org.orgid = r.orgid
                        left join orgstaff s on s.orgid = r.orgid and s.orgstaffid = r.orgstaffid
                        where resid = 5
                            and (ifnull(org.owner,'') = @userId
                                or ifnull(s.staffid,'') = @userId)
                    ";

                    // Ensure that the connection state is Open
                    ConnectionDb.OpenConnect(sqlCon);

                    resultQuery = await sqlCon.ExecuteScalarAsync<int>(
                        query, new { resid = resId, userId = userId }
                    );
                }
            });

            if (resultQuery != 0 || isAdmin)
            {
                var reserveItem = await _dbContext.OrgReserve.Where(w => w.resId == resId).FirstOrDefaultAsync();
                if (reserveItem != null)
                {
                    reserveItem.isConfirm = true;
                    _dbContext.OrgReserve.Update(reserveItem);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}