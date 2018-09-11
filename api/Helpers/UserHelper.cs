using api.Data;
using api.Model;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace api.Helpers
{
    public class UserHelper
    {
        private MongoContext _db { get; set; }
        private HasherHelper _hasher { get; set; }
        public UserHelper(MongoContext context, HasherHelper hasher)
        {
            _db = context;
            _hasher = hasher;
        }

        /// <summary>
        /// Retrieve user by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User GetUserById(string id)
        {
            User user = _db.Users.Find(u => u.Id == id).FirstOrDefault();
            if (user != null)
                return user;
            else
                return default(User);
        }

        /// <summary>
        /// Retrieve user by email address
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public User GetUserByEmail(string email)
        {
            User user = _db.Users.Find(u => u.Email == email).FirstOrDefault();
            if (user != null)
                return user;
            else
                return default(User);
        }

        /// <summary>
        /// Validate user and save to DB
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool CreateUser(User user)
        {
            if (GetUserByEmail(user.Email) != null) //User email address must be unique
                return false;
            else if (!IsValidPassword(user.Password)) //User password must meet complexity requirements
                return false;

            _db.Users.InsertOne(user);

            return true;
        }

        public UserSlim UserToUserSlim(User user)
        {
            return new UserSlim()
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                LastSignin = user.LastSignin,
                Created = user.Created,
                Updated = user.Updated,
                Roles = user.Roles
            };
        }

        /// <summary>
        /// Validate usernamd and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserValidationResponse ValidateUserIdentity(string username, string password, ref User userResult)
        {
            if (username != null)
                username = username.ToLower();
            User user = _db.Users.Find(u => u.Email.ToLower() == username).FirstOrDefault();

            if (user == null)
            {
                return UserValidationResponse.Invalid;
            }
            else if (user.EmailValidated == false)
            {
                return UserValidationResponse.Invalidated;
            }
            else if (PasswordMatch(user, password) == false)
            {
                user.LockoutCount++;
                var updateLockoutCount = Builders<User>.Update.Set(u => u.LockoutCount, user.LockoutCount);
                _db.Users.UpdateOne(u => u.Id == user.Id, updateLockoutCount);

                if (user.LockoutCount >= 10)
                {
                    user.LockoutDateTime = DateTime.Now.ToUniversalTime();
                    var updateLockoutDate = Builders<User>.Update.Set(u => u.LockoutDateTime, user.LockoutDateTime);
                    _db.Users.UpdateOne(u => u.Id == user.Id, updateLockoutDate);

                    return UserValidationResponse.LockedOut;
                }

                return UserValidationResponse.Invalid;
            }
            else
            {
                if (user.LockoutCount >= 10 && user.LockoutDateTime <= DateTime.Now.Subtract(new TimeSpan(0, 30, 0)).ToUniversalTime())
                {
                    user.LockoutCount = 0;
                    var updateLockoutCount = Builders<User>.Update.Set(u => u.LockoutCount, user.LockoutCount).Set(u => u.LockoutDateTime, null);
                    _db.Users.UpdateOne(u => u.Id == user.Id, updateLockoutCount);
                    userResult = user;
                    return UserValidationResponse.Validated;
                }

                else if (user.LockoutCount >= 10 && user.LockoutDateTime > DateTime.Now.Subtract(new TimeSpan(0, 30, 0)))
                {
                    return UserValidationResponse.LockedOut;
                }
                else
                {
                    if (user.LockoutCount >= 0)
                    {
                        user.LockoutCount = 0;
                        var updateLockoutCount = Builders<User>.Update.Set(u => u.LockoutCount, user.LockoutCount).Set(u => u.LockoutDateTime, null);
                        _db.Users.UpdateOne(u => u.Id == user.Id, updateLockoutCount);
                    }
                    userResult = user;
                    return UserValidationResponse.Validated;
                }
            }
        }

        

        /// <summary>
        /// Test password for complexity 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IsValidPassword(string password)
        {
            return true;
        }

        /// <summary>
        /// Compare password hashes
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool PasswordMatch(User user, string password)
        {
            string hash = _hasher.GetHash(password + user.Salt);

            if (user.Password == hash)
                return true;
            else
                return false;
        }

        public string CreatUserSalt()
        {
            var random = RandomNumberGenerator.Create();// new RNGCryptoServiceProvider();

            // Maximum length of salt
            int max_length = 128;

            // Empty salt array
            byte[] salt = new byte[max_length];

            // Build the random bytes
            random.GetBytes(salt);//.GetNonZeroBytes(salt);

            // Return the string encoded salt
            return Convert.ToBase64String(salt); ;
        }


    }
}
