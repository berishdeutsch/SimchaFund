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
                                VALUES (@firstName, @lastName, @cell, @date, @alwaysInclude) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@firstName", contributor.FirstName);
            cmd.Parameters.AddWithValue("@lastName", contributor.LastName);
            cmd.Parameters.AddWithValue("@cell", contributor.Cell);
            cmd.Parameters.AddWithValue("@date", contributor.Date);
            cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
            connection.Open();
            int id = (int)(decimal)cmd.ExecuteScalar();
            if (contributor.initialDeposit != 0)
            {
                AddDeposit(new Deposit
                {
                    Amount = contributor.initialDeposit,
                    Date = contributor.Date,
                    ContributorId = id
                });
            }

        }
        public void AddDeposit(Deposit deposit)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Deposits (Amount, Date, ContributorId)
                                VALUES (@amount, @date, @contributorId)";
            cmd.Parameters.AddWithValue("@amount", deposit.Amount);
            cmd.Parameters.AddWithValue("@date", deposit.Date);
            cmd.Parameters.AddWithValue("@contributorId", deposit.ContributorId);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public void UpdateContributor(Contributor contributor)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE Contributors SET FirstName = @firstName, LastName = @lastName, Cell = @cell,
                                Date = @date, AlwaysInclude = @alwaysInclude WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", contributor.Id);
            cmd.Parameters.AddWithValue("@firstName", contributor.FirstName);
            cmd.Parameters.AddWithValue("@lastName", contributor.LastName);
            cmd.Parameters.AddWithValue("@cell", contributor.Cell);
            cmd.Parameters.AddWithValue("@date", contributor.Date);
            cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public List<Simcha> GetSimchas()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT s.Id, SimchaName, COUNT(c.SimchaId) AS ContributorCount, ISNULL(SUM(c.Amount), 0) AS Total, s.Date
                                FROM Simchas s
                                LEFT JOIN Contributions c
                                ON s.Id = c.SimchaId
                                GROUP BY s.Id, s.SimchaName, s.Date";
            connection.Open();
            var result = new List<Simcha>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Simcha
                {
                    Id = (int)reader["Id"],
                    SimchaName = (string)reader["SimchaName"],
                    SimchaContributors = (int)reader["ContributorCount"],
                    Total = (decimal)reader["Total"],
                    Date = (DateTime)reader["Date"]
                });
            }
            return result;
        }
        public void AddSimcha(Simcha simcha)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Simchas (SimchaName, Date)
                                VALUES (@simchaName, @date)";
            cmd.Parameters.AddWithValue("@simchaName", simcha.SimchaName);
            cmd.Parameters.AddWithValue("@date", simcha.Date);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
