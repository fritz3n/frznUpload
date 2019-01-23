using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using Dapper;
using System.Security.Cryptography;

namespace frznUpload.Server
{
    class DataBase : IDisposable
    {
        MySqlConnection conn;
        int userId;
        public bool IsAuthenticated { get; private set; }

        public DataBase()
        {
            string connStr = "server=192.168.2.187;user=frznUpload;database=frznUpload;port=3306;password=BLANKCHRIS";


            conn = new MySqlConnection(connStr);
            conn.Open();
        }

        public List<Sql_File> GetFiles()
        {
            ThrowIfNotAuthenticated();

            return conn.Query<Sql_File>("SELECT * FROM files WHERE user_id = @userId", new { userId }).AsList();
        }

        public List<Share> GetShares(string fileIdentifier)
        {
            ThrowIfNotAuthenticated();

            return conn.Query<Share>("SELECT * FROM shares WHERE file_identifier = @fileIdentifier", new { fileIdentifier }).AsList();
        }

        public bool CheckTokenExists(byte[] token)
        {
            return conn.QuerySingle<int>("SELECT COUNT(*) FROM tokens WHERE signature = @tp", new { tp = token }) >= 1;
        }

        public string CreateFile(string identifier, string filename, string extension, int size)
        {
            ThrowIfNotAuthenticated();

            conn.Execute("INSERT INTO files (user_id, identifier, filename, file_extension, size) VALUES (@userId, @identifier, @filename, @extension, @size)", new { userId, identifier, filename, extension, size });

            return identifier;
        }

        public string GetAvailableFileIdentifier()
        {
            string identifier = "";

            do
            {
                identifier = GenerateFileIdentifier();
            } while (conn.QuerySingle<int>("SELECT COUNT(*) FROM files WHERE identifier = @ident", new { ident = identifier }) != 0);

            return identifier;
        }

        private string GenerateFileIdentifier()
        {
            Random rnd = new Random();
            byte[] rndBytes = new byte[96];
            rnd.NextBytes(rndBytes);

            return Convert.ToBase64String(rndBytes).Replace('/', '-');
        }

        public string GetAvailableShareIdentifier()
        {
            string identifier = "";

            do
            {
                identifier = GenerateShareIdentifier();
            } while (conn.QuerySingle<int>("SELECT COUNT(*) FROM shares WHERE share_id = @ident", new { ident = identifier }) != 0);

            return identifier;
        }

        private string GenerateShareIdentifier()
        {
            Random rnd = new Random();
            byte[] rndBytes = new byte[5];
            rnd.NextBytes(rndBytes);

            string s = Convert.ToBase64String(rndBytes).Replace('/', '-');

            return s.Substring(0, 6);
        }

        public void SetUser(byte[] token)
        {
            userId = conn.QuerySingle<int>("SELECT user_id FROM tokens WHERE signature = @tp", new { tp = token });
            conn.Execute("UPDATE tokens SET last_used = now() WHERE signature = @tp", new { tp = token });
            IsAuthenticated = true;
        }

        public string HashPassword(string password, int id)
        {
            User User = conn.QueryFirst<User>("SELECT * FROM users WHERE id=@id", new { id });

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

            using (var sha = new SHA512CryptoServiceProvider())
            {
                return Convert.ToBase64String(sha.ComputeHash(Encoding.Default.GetBytes(HashString)));
            }

        }

        public bool SetToken(string username, string password, byte[] token)
        {
            bool exists = conn.QuerySingle<int>("SELECT COUNT(*) FROM tokens WHERE signature = @sig", new { sig = token }) > 0;

            if (exists)
            {
                Token dbToken = conn.QuerySingle<Token>("SELECT * FROM tokens WHERE signature = @sig", new { sig = token });

                if (dbToken != null)
                {
                    if (conn.QuerySingle<string>("SELECT name FROM users WHERE id = @id", new { id = dbToken.UserId }) == username)
                    {
                        return true;
                    }
                    return false;
                }
            }

            if (conn.QuerySingle<int>("SELECT COUNT(*) FROM users WHERE name = @name", new { name = username }) != 1)
                return false;

            User User = conn.QuerySingle<User>("SELECT * FROM users WHERE name = @name", new { name = username });

            if (User.Name != username)
                return false;

            if (User.Hash != HashPassword(password, User.Id))
                return false;

            conn.Execute("INSERT INTO tokens (user_id, signature) VALUES(@id, @sig)", new { id = User.Id, sig = token });
            return true;
        }

        public string SetFileShare(string fileIdentifier, bool firstView = false, bool isPublic = true, bool publicRegistered = true, bool whitelisted = false, string whitelist = null)
        {
            ThrowIfNotAuthenticated();
            if (conn.QuerySingle<int>("SELECT COUNT(*) FROM files WHERE identifier = @ident;", new { ident = fileIdentifier }) != 1)
                return null;

            if (conn.QuerySingle<int>("SELECT user_id FROM files WHERE identifier = @ident", new { ident = fileIdentifier }) != userId)
                return null;

            string shareId = GetAvailableShareIdentifier();

            conn.Execute(
                "INSERT INTO shares (file_identifier, share_id, first_view, public, public_registered, whitelisted, whitelist) " +
                "VALUES(@fileIdentifier, @shareId, @firstView, @isPublic, @publicRegistered, @whitelisted, @whitelist)",
                new { fileIdentifier, shareId, firstView, isPublic, publicRegistered, whitelisted, whitelist });

            return shareId;
        }

        private void ThrowIfNotAuthenticated()
        {
            if (!IsAuthenticated)
                throw new ArgumentException("Not Authenticated");
        }

        public void Dispose()
        {
            conn.Dispose();
        }

        ~DataBase() => Dispose();

        public string Name
        {
            get
            {
                if (!IsAuthenticated)
                    throw new Exception("not authenticated");

                return conn.QuerySingle<string>("SELECT name FROM users WHERE id = @id", new { id = userId });
            }
        }


    }       
}