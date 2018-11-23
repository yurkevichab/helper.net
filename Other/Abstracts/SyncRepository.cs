using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Logging;
using SynchronizationSubscriberService.Helpers;

namespace SynchronizationSubscriberService.Abstracts
{
    public class SyncRepository<T>
    {
        protected readonly string _connectionString;
        protected readonly string _storedProcedure;

    
        public SyncRepository(string connectionString, string storedProcedure)
        {
            _connectionString = connectionString;
            _storedProcedure = storedProcedure;
        }

        public void Synchronization(List<T> dataList)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (dataList.Count > 0)
                {
                    var parameters = new ParameterTvp<T>(dataList);
                    var result = connection.Query<int>(_storedProcedure, parameters);
                }
            }
        }
    }
}