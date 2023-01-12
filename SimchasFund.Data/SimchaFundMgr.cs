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
            cmd.CommandText = @"SELECT  c.Id, c.FirstName, c.LastName, c.Cell, c.Date, c.AlwaysInclude,
                                SUM(d.Amount) - ISNULL((SELECT SUM(Amount) FROM Contributions 
						                                WHERE ContributorID = c.ID), 0) AS 'Balance'
                                FROM contributors c                             
                                LEFT JOIN Deposits d
                                ON c.Id = d.ContributorId
                                GROUP BY c.Id, c.FirstName, c.LastName, c.cell, c.Date, c.AlwaysInclude";
            connection.Open();
            var result = new List<Contributor>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var contributor = new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Cell = (string)reader["Cell"],
                    Date = (DateTime)reader["Date"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"]
                };
                result.Add(contributor);
            }
            foreach (var contributor in result)
            {
                contributor.Balance = GetBalance(contributor.Id);
            }
            return result;
        }
        public decimal GetBalance(int contributorId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"Select ISNULL(Sum(Amount), 0) from Deposits where ContributorId = @contributorId";
            cmd.Parameters.AddWithValue("@contributorId", contributorId);
            connection.Open();
            return (decimal)cmd.ExecuteScalar();
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
            cmd.CommandText = @"SELECT s.Id, s.SimchaName, COUNT(c.SimchaId) AS ContributorCount, ISNULL(SUM(c.Amount), 0) AS Total, s.Date
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

        public Contributor GetContributorById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Contributors WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();
            reader.Read();
            return new Contributor
            {
                Id = (int)reader["Id"],
                FirstName = (string)reader["FirstName"],
                LastName = (string)reader["LastName"],
                Cell = (string)reader["Cell"],
                Date = (DateTime)reader["Date"],
                AlwaysInclude = (bool)reader["AlwaysInclude"],
                Balance = GetBalance(id)
            };
        }
        public List<Simcha> GetSimchasById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT s.SimchaName, s.Date, c.Amount FROM Contributions c 
                                JOIN Simchas s
                                ON c.SimchaId = s.Id
                                WHERE ContributorId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var result = new List<Simcha>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Simcha
                {
                    SimchaName = (string)reader["SimchaName"],
                    Date = (DateTime)reader["Date"],
                    Total = (decimal)reader["Amount"]
                });
            }
            return result;
        }
        public Simcha GetSimchaById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Simchas WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();
            reader.Read();
            return new Simcha
            {
                Id = (int)reader["Id"],
                SimchaName = (string)reader["SimchaName"]
            };
        }
    }
}
