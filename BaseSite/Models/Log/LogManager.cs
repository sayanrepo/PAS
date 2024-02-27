using BaseSite.Data;
using BaseSite.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BaseSite.Models.Log
{
    public class LogManager
    {
        public static List<Log_Logs> Log_Logs_Search(int? docNumber, int? tableId, int? userId, DateTime? eventTimeFrom, DateTime? eventTimeTo)
        {
            using (var context = new PantaEntities())
            {
                if (docNumber == null && tableId == null && userId == null && eventTimeFrom == null && eventTimeTo == null)
                {
                    List<Log_Logs> result = context.Log_Logs.Include(m => m.Account_Users).Include(m => m.BaseSystem_Tables).Include(m => m.Log_LogActivity)
                                              .OrderByDescending(m => m.EventTime).Take(1000).ToList();

                    return result;
                }
                else
                {
                    var list = from p in context.Log_Logs.Include(m => m.Account_Users).Include(m => m.BaseSystem_Tables).Include(m => m.Log_LogActivity)
                               select p;

                    if (docNumber != null) list = list.Where(p => p.EntityId == docNumber);
                    if (tableId != null) list = list.Where(p => p.EntityTableId == tableId);
                    if (userId != null) list = list.Where(p => p.UserId == userId);
                    if (eventTimeFrom != null) list = list.Where(p => p.EventTime >= eventTimeFrom.Value);
                    if (eventTimeTo != null) list = list.Where(p => p.EventTime <= eventTimeTo.Value);
                    list = list.OrderByDescending(p => p.EventTime);

                    // Execute the query
                    List<Log_Logs> result = list.ToList();

                    return result;
                }
            }
        }

        public static Log_Logs Log_Logs_Add(Log_Logs log)
        {
            using (var context = new PantaEntities())
            {
                log.EventTime = DateTime.Now;
                log.StatusId = 0;

                context.Log_Logs.Add(log);
                context.SaveChanges();
                return log;
            }
        }

        public static Log_Logs Log_Logs_Add(int tableId, int entityId, int userid, string ipaddress, int activityId, string description, double? data = null)
        {
            using (var context = new PantaEntities())
            {
                Log_Logs log = new Log_Logs()
                {
                    EntityTableId = tableId,
                    EntityId = entityId,
                    UserId = userid,
                    IPAddress = ipaddress,
                    ActivityId = activityId,
                    Description = description,
                    EventTime = DateTime.Now,
                    StatusId = 0,
                    LogData1 = data
                };

                context.Log_Logs.Add(log);
                context.SaveChanges();
                return log;
            }
        }

        public static Log_Logs Log_Logs_Get(int logId)
        {
            using (var context = new PantaEntities())
            {
                Log_Logs log = context.Log_Logs.Include(m => m.Account_Users).Include(m => m.BaseSystem_Tables).Include(m => m.Log_LogActivity).Where(m => m.Id == logId).SingleOrDefault();

                return log;
            }
        }

        public static void Log_Logs_Delete(DateTime? eventTimeFrom, DateTime? eventTimeTo)
        {
            using (var context = new PantaEntities())
            {
                List<Log_Logs> result = context.Log_Logs.ToList();

                if (eventTimeFrom != null)
                    result = result.Where(x => x.EventTime >= eventTimeFrom.Value).ToList();
                if (eventTimeTo != null)
                    result = result.Where(x => x.EventTime <= eventTimeTo.Value).ToList();

                for (int i = 0; i < result.Count; i++)
                {
                    context.Log_Logs.Remove(result.ElementAt(i));
                    result.RemoveAt(i);
                    i--;
                }
                context.SaveChanges();
            }
        }
    }
}