using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using Dapper;
using System.Security.Cryptography;

namespace frznUpload.Server
{
    class DataBase
    {
        MySqlConnection conn;
        int userId;
        public bool IsAuthenticated { get; private set; }

        public DataBase()
        {
            string connStr = "server=192.168.2.187;user=frznUpload;database=frznupload;port=3306;password=BLANKCHRIS";
            

            conn = new MySqlConnection(connStr);
            conn.Open();
        }

        public bool CheckTokenExists(byte[] token)
        {
            return (int)conn.QueryFirst("SELECT COUNT(*) FROM tokens WHERE signature = @tp", new { tp = token }) >= 1;
        }



        public void SetUser(byte[] token)
        {
            userId = (int)conn.QueryFirst("SELECT user_id FROM tokens WHERE signature = @tp", new { tp = token });
            conn.Execute("UPDATE tokens SET last_used = now() WHERE signature = @tp", new { tp = token });
            IsAuthenticated = true;
        }

        public string HashPassword(string password, int id)
        {
            User User = conn.QueryFirst<User>("SELECT * WHERE id=@id", new { id });

            if (string.IsNullOrEmpty(User.Hash))
            {
                using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
                {
                    byte[] tokenData = new byte[24];
                    rng.GetBytes(tokenData);

                    User.Salt = Convert.ToBase64String(tokenData);
                }

                conn.Execute("UPDATE users SET salt=@salt WHERE id=@id", new { salt = User.Salt, id });
            }

            string HashString = User.Name + password + User.Salt;

            using(SHA512CryptoServiceProvider sha = new SHA512CryptoServiceProvider())
            {
                return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(HashString)));
            }

        }

        public string Name { get
            {
                if (!IsAuthenticated)
                    throw new Exception("not authenticated");

                return (string)QuerySingle("SELECT name FROM users WHERE id = @id", "id", userId);
                    } }

        private IEnumerable<MySqlDataReader> Query(string query)
        {
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            
            while (rdr.Read())
            {
                yield return rdr;
            }
            rdr.Close();
            
        }

        private IEnumerable<MySqlDataReader> Query(string query, IDictionary<string, object> parameters)
        {
            MySqlCommand cmd = new MySqlCommand(query, conn);

            foreach(var parameter in parameters)
            {
                cmd.Parameters.AddWithValue("@" + parameter.Key, parameter.Value);
            }

            MySqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                yield return rdr;
            }
            rdr.Close();
        }

        private object Query(string query, string parameterName, object parameter)
        {
            return Query(query, new Dictionary<string, object> { { parameterName, parameter } });
        }

        private MySqlDataReader QueryFirst(string query)
        {
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();

            rdr.Read();
            return rdr;
        }

        private MySqlDataReader QueryFirst(string query, IDictionary<string, object> parameters)
        {
            MySqlCommand cmd = new MySqlCommand(query, conn);

            foreach (var parameter in parameters)
            {
                cmd.Parameters.AddWithValue("@" + parameter.Key, parameter.Value);
            }

            MySqlDataReader rdr = cmd.ExecuteReader();

            rdr.Read();
            return rdr;
        }

        private MySqlDataReader QueryFirst(string query, string parameterName, object parameter)
        {
            return QueryFirst(query, new Dictionary<string, object> { { parameterName, parameter } });
        }

        private object QuerySingle(string query)
        {
            MySqlCommand cmd = new MySqlCommand(query, conn);

            return cmd.ExecuteScalar();
        }

        private object QuerySingle(string query, IDictionary<string, object> parameters)
        {
            MySqlCommand cmd = new MySqlCommand(query, conn);

            foreach (var parameter in parameters)
            {
                cmd.Parameters.AddWithValue("@" + parameter.Key, parameter.Value);
            }

            return cmd.ExecuteScalar();
        }

        private object QuerySingle(string query, string parameterName, object parameter)
        {
            return QuerySingle(query, new Dictionary<string, object> { { parameterName, parameter } });
        }

        private void QueryInert(string query)
        {
            MySqlCommand cmd = new MySqlCommand(query, conn);

            cmd.ExecuteNonQuery();
        }

        private void QueryInert(string query, IDictionary<string, object> parameters)
        {
            MySqlCommand cmd = new MySqlCommand(query, conn);

            foreach (var parameter in parameters)
            {
                cmd.Parameters.AddWithValue("@" + parameter.Key, parameter.Value);
            }

            cmd.ExecuteNonQuery();
        }

        private void QueryInert(string query, string parameterName, object parameter)
        {
            QueryInert(query, new Dictionary<string, object> { { parameterName, parameter } });
        }
    }
}