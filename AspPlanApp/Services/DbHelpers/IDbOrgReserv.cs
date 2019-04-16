using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspPlanApp.Models.ReservViewModels;

namespace AspPlanApp.Services.DbHelpers
{
    /// <summary>
    /// Interface to work with Org Reserve data 
    /// </summary>
    public interface IDbOrgReserv
    {
        Task<List<ReservItemsViewModel>> GetOrgReservByWeek(int orgId, DateTime dateCal, string currUser);

        Task AddNewEvent(string userId, int orgId, DateTime dateFrom, DateTime dateTo, int staffId, string comm);
    }
}