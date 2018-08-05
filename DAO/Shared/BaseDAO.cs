using EF;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;

namespace DAO.Shared
{
    public class BaseDAO : IDisposable
    {
        protected OnlineCustomerServiceContext onlineCustomerServiceContext { set; get; }
        protected ConnectionMultiplexer redisConnection { set; get; }

        public BaseDAO(DbContext dbContext)
        {
            if (dbContext is OnlineCustomerServiceContext)
            {
                onlineCustomerServiceContext = (OnlineCustomerServiceContext)dbContext;
            }
        }

        public BaseDAO(ConnectionMultiplexer redisConnection)
        {
            this.redisConnection = redisConnection;
        }

        public void Dispose()
        {
            try
            {
                if (onlineCustomerServiceContext != null)
                {
                    onlineCustomerServiceContext.Dispose();
                }
            }
            catch (Exception)
            {
                
            }

            try
            {
                if (redisConnection != null)
                {
                    redisConnection.Close();
                    redisConnection.Dispose();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
