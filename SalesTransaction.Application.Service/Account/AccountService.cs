
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SalesTransaction.Application.DataAccess;
using SalesTransaction.Application.Model.Account;
using SalesTransaction.Application.Service.Account;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SalesTransaction.Application.Service
{
    public class AccountService : IAccountService
    {
        private DataAccessHelper _da;
        private readonly int _commandTimeout;
        private readonly string _connectionString;
        private IConfiguration _configuration;

        public AccountService(IConfiguration configuration)
        {
            _configuration = configuration;

            dynamic connectionString = _configuration.GetSection("ConnectionString");
            _connectionString = connectionString["DefaultConnection"];

            if (_connectionString != null)
            {
                _da = new DataAccessHelper(_connectionString);
            }

            _commandTimeout = Convert.ToInt32(connectionString["CommandTimeout"]);
        }

        public dynamic GetLogin(MvLogin login)
        {
            using (var conn = _da.GetConnection())
            {
                using (var cmd = new SqlCommand("SpUserSel", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Username", login.UserName));
                    cmd.Parameters.Add(new SqlParameter("@Password", login.Password));

                    using (var reader = cmd.ExecuteReader())
                    {
                        try
                        {
                            if (reader.HasRows)
                            {
                                return _da.GetJson(reader);
                            }
                            return null;
                        }
                        catch (Exception ex)
                        {

                            throw ex;
                        }
                    }
                }
            }

        }
        public dynamic GetUserDetail(string json)
        {
            var jsonNew = JsonConvert.DeserializeObject(json);
            using (var conn = _da.GetConnection())
            {
                using (var cmd = new SqlCommand("SpUserDetailSel", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Json", jsonNew.ToString()));


                    using (var reader = cmd.ExecuteReader())
                    {
                        try
                        {
                            if (reader.HasRows)
                            {
                                return _da.GetJson(reader);
                            }
                            return null;
                        }
                        catch (Exception ex)
                        {

                            throw ex;
                        }
                    }
                }
            }
        }
    }
}