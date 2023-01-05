using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SimchasFund.Data
{
    public class SimchaFundMgr
    {
        private string _connectionString;
        public SimchaFundMgr(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<Contributor> GetContributors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Contributors";
            connection.Open();
            var result = new List<Contributor>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Cell = (string)reader["Cell"],
                    Date = (DateTime)reader["Date"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"]       
                });
            }
            return result;
        }
        public void AddContributor(Contributor contributor)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Contributors (FirstName, LastName, Cell, Date, AlwaysInclude)
                                VALUES (@firstName, @lastName, @cell, @date, @alwaysInclude)";
            cmd.Parameters.AddWithValue("@firstName", contributor.FirstName);
            cmd.Parameters.AddWithValue("@lastName", contributor.LastName);
            cmd.Parameters.AddWithValue("@cell", contributor.Cell);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
