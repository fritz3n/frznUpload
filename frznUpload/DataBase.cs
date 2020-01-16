using Dapper;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace frznUpload.Server
{
    class DataBase : IDisposable
    {
        static Random rnd = new Random();

        MySqlConnection conn;
        int userId;
        public bool IsAuthenticated { get; private set; }
        byte[] token = new byte[0];

        public DataBase()
        {
            string connStr = "server=localhost;user=frznUpload;database=frznUpload;port=3306;password=BLANKCHRIS";


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
            if (string.IsNullOrWhiteSpace(fileIdentifier))
                throw new ArgumentException("Parameter is invalid", nameof(fileIdentifier));

            ThrowIfNotAuthenticated();

            return conn.Query<Share>("SELECT * FROM shares WHERE file_identifier = @fileIdentifier", new { fileIdentifier }).AsList();
        }

        public bool CheckTokenExists(byte[] token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

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
            string identifier;

            do
            {
                identifier = GenerateFileIdentifier();
            } while (conn.QuerySingle<int>("SELECT COUNT(*) FROM files WHERE identifier = @ident", new { ident = identifier }) != 0);

            return "";
        }

        private string GenerateFileIdentifier()
        {
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
            byte[] rndBytes = new byte[5];
            rnd.NextBytes(rndBytes);

            string s = Convert.ToBase64String(rndBytes).Replace('/', '-');

            return s.Substring(0, 6);
        }

        public void SetUser(byte[] token)
        {
            userId = conn.QuerySingle<int>("SELECT user_id FROM tokens WHERE signature = @tp", new { tp = token });
            conn.Execute("UPDATE tokens SET last_used = now() WHERE signature = @tp", new { tp = token });
            this.token = token;
            IsAuthenticated = true;
        }

        public void Deauthenticate()
        {
            if (!IsAuthenticated)
                return;

            bool exists = conn.QuerySingle<int>("SELECT COUNT(*) FROM tokens WHERE signature = @sig", new { sig = token }) > 0;

            if (exists)
            {
                conn.Execute("DELETE FROM tokens WHERE signature = @sig", new { sig = token });
                IsAuthenticated = false;
            }
        }

        //TODO: improve 
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

            using (SHA512CryptoServiceProvider sha = new SHA512CryptoServiceProvider())
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

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private void ThrowIfNotAuthenticated()
        {
            if (!IsAuthenticated)
                throw new ArgumentException("Not Authenticated");
        }

        public void Dispose() => conn.Dispose();

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

        /// <summary>
        /// Gets a filename for a given identifier
        /// </summary>
        public string GetFileName(string file_identifier) => conn.QuerySingle<string>("SELECT filename FROM files WHERE identifier=@file_identifier", new { file_identifier });
        /// <summary>
        /// Gets the extension for a given identifier
        /// </summary>
        public string GetFileExtension(string file_identifier) => conn.QuerySingle<string>("SELECT file_extension FROM files WHERE identifier=@file_identifier", new { file_identifier });

        /// <summary>
        /// Get a full filename for a given file_identifier (e.g picrure.png)
        /// </summary>
        public string GetFullFileName(string file_identifier) => GetFileName(file_identifier) + GetFileExtension(file_identifier);

        /// <summary>
        /// Gets the userId of a file
        /// </summary>
        /// <param name="file_identifier">The file_identifier to be used</param>
        /// <returns>the userId</returns>
        public int GeTwonerOfFile(string file_identifier) => conn.QuerySingle<int>("SELECT user_id FROM files WHERE identifier=@file_identifier", new { file_identifier });

        /// <summary>
        /// Checks if the user owns this file
        /// </summary>
        /// <param name="file_identifier">the file_identifier to be checked</param>
        public bool UserOwnsFile(string file_identifier) => GeTwonerOfFile(file_identifier) == userId;

        /// <summary>
        /// Deletes a file (Including from the fs) and all shares with it
        /// and sends a Succsess message.
        /// </summary>
        /// <param name="share">The file_identifier to be delted</param>
        /// <exception cref="UnauthorizedAccessException">Thrown when the user is not the owner of this file</exception>
        public void DeleteFile(string file_identifier)
        {
            ThrowIfNotAuthenticated();
            if (UserOwnsFile(file_identifier))
            {
                //deletes all the shares for that file
                conn.Execute("DELETE FROM shares WHERE file_identifier=@id", new { id = file_identifier });
                //deletes the file
                conn.Execute("DELETE FROM files WHERE identifier=@file_identifier", new { file_identifier });
            }
            else
            {
                throw new UnauthorizedAccessException("The user dose not own this share");
            }
        }

        /// <summary>
        /// Gets a Two Fa Secret for a given userID
        /// </summary>
        public string GetTwoFactorSecret(int? id = null)
        {
            id = id ?? userId;
            return conn.QuerySingle<string>("SELECT Two_fa_secret FROM users WHERE id=@id", new { id });
        }

        public bool HasTwoFa(int? id = null) => GetTwoFactorSecret(id) != "null";


        /// <summary>
        /// Gets the corresponding id to a username
        /// </summary>
        /// <param name="username">the username to query for</param>
        /// <returns>the id or NULL if the user isnt found</returns>
        public int? GetUserId(string username)
        {
            if (conn.QuerySingle<int>("SELECT COUNT(*) FROM users WHERE name = @username", new { username }) != 1)
                return null;

            return conn.QuerySingle<int>("SELECT id FROM users WHERE name = @username", new { username });
        }

        /// <summary>
        /// Sets a TwoFa secret in the db
        /// </summary>
        public void SetTwoFactorSecret(string val) => conn.Execute("UPDATE users SET Two_fa_secret=@val WHERE id=@id", new { val, id = userId });

        public void RemoveTwoFactorSecret() => conn.Execute("UPDATE users SET Two_fa_secret=null WHERE id=@id", new { id = userId });

        public string GetUsername() => conn.QuerySingle<string>("SELECT name FROM users WHERE id=@id", new { id = userId });
    }
}