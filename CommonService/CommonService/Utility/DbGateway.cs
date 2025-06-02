using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonService.Utility
{
    public class DbGateway
    {

        private readonly IConfiguration? _configuration;

        private string _connectionString;

        public DbGateway(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection Connection()
        {
            return new MySqlConnection(_connectionString);
        }

        //Execute a single-row query asynchronously using Task.
        public async Task<T> ExeScalarQuery<T>(string QueryText, DynamicParameters paras)
        {
            try
            {
                T result;
                using (IDbConnection conn = Connection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    result = await conn.QueryFirstOrDefaultAsync<T>(QueryText, (object)paras, (IDbTransaction)null, (int?)null, (CommandType?)null);
                }
                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> ExeQuery(string QueryText, DynamicParameters paras)
        {
            try
            {
                int result;
                using (IDbConnection conn = Connection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    result = await conn.ExecuteAsync(QueryText, (object)paras, (IDbTransaction)null, (int?)null, (CommandType?)null);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // perform command to map the result in the form of list
        public async Task<List<T>> ExeQueryList<T>(string QueryText, DynamicParameters paras)
        {
            try
            {
                using (IDbConnection conn = Connection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    return (await conn.QueryAsync<T>(QueryText, (object)paras, (IDbTransaction)null, (int?)null, (CommandType?)null)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // using this function we get the data without passing any parameters
        // Basically used to showcase data in the grid format
        public async Task<T> ExeScalarQuery<T>(string QueryText)
        {
            try
            {
                T result;
                using (IDbConnection conn = Connection())
                {
                    if (conn.State == ConnectionState.Closed) { conn.Open(); }
                    result = await conn.QueryFirstOrDefaultAsync<T>(QueryText, (object)null, (IDbTransaction)null, (int?)null, (CommandType?)null);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //for taking list of values
        public async Task<List<T>> ExeQueryList<T> (string QueryText)
        {
            try
            {
                List<T> result;
                using(IDbConnection conn = Connection())
                {
                    if(conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    result = (await conn.QueryAsync<T>(QueryText)).ToList();
                }
                return result;
            } catch (Exception ex)
            {
                throw ex;
            }
        }

        // query first async : Provide us the first result if multiple record present
        // using stored procedure in this
        public async Task<T> ExeSPScalar<T>(string QueryText)
        {
            try
            {
                T result;
                using (IDbConnection conn = Connection())
                {
                    if (conn.State == ConnectionState.Closed) { conn.Open(); }

                    CommandType? commandType = CommandType.StoredProcedure;
                    result = await conn.QueryFirstAsync<T>(QueryText, (object)null, (IDbTransaction)null, (int?)null, commandType);
                }
                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //get the list of data using storedProcedure
        public async Task<List<T>> ExeSPList<T>(string QueryText, DynamicParameters param)
        {
            try
            {
                using (IDbConnection conn = Connection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    CommandType? commandType = CommandType.StoredProcedure;
                    return (await conn.QueryAsync<T>(QueryText, (object)param, (IDbTransaction)null, (int?)null, commandType)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //this will return the numbers of rows effected
        public async Task<int> ExeSP(string QueryText, DynamicParameters param)
        {
            try
            {
                int result;
                using (IDbConnection conn = Connection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    CommandType? commandType = CommandType.StoredProcedure;
                    result = await conn.ExecuteAsync(QueryText, (object)param, (IDbTransaction)null, (int?)null, commandType);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
