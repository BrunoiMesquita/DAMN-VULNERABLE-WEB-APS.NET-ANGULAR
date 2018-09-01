using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using damnstack.Data;
using damnstack.Model;
using Microsoft.AspNetCore.Authorization;
using damnstack.Helpers;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace damnstack.Controllers
{
	public class UserController : Controller
	{
		private MongoContext _db { get; set; }
		private UserHelper _userHelper { get; set; }
		private ValidationHelper _validationHelper { get; set; }
		private HasherHelper _hasherHelper { get; set; }
		private TokenHelper _tokenHelper { get; set; }
		private ApplicationSettings _applicationSettings { get; set; }
		public UserController(MongoContext mongoContext, UserHelper userHelper, ValidationHelper validationHelper, HasherHelper hasherHelper, TokenHelper tokenHelper, ApplicationSettings applicationSettings)
		{
			_db = mongoContext;
			_userHelper = userHelper;
			_validationHelper = validationHelper;
			_hasherHelper = hasherHelper;
			_tokenHelper = tokenHelper;
			_applicationSettings = applicationSettings;
		}

		[HttpPost]
		[Route("api/v1/authenticate")]
		async public Task<IActionResult> Authenticate([FromBody]AuthenticateDataModel model)
		{
			//Get values from request, sent by client.
			string username = model.Email?.Trim();
			string password = model.Password?.Trim();

			//Client validation passed.  Validate credentials.
			//Does the user have a valid account and did they provide a valid username/password.
			User user = default(User);
			//Does user have valid credentials
			var validated = _userHelper.ValidateUserIdentity(username, password, ref user);
			if (validated == UserValidationResponse.Invalid)
			{
				return BadRequest("Invalid Username or Password");
			}
			else if (validated == UserValidationResponse.LockedOut)
			{
				return BadRequest("Account is Locked. Wait 30 minutes.");
			}
			else if (validated == UserValidationResponse.Invalidated)
			{
				return BadRequest("Email has not been validated");
			}

			SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_applicationSettings.SigningKey));
            TokenProviderOptions options = new TokenProviderOptions()
            {
                Issuer = this.Request.Host.Value,
				SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
			};
			//Client, Tokens, and User validation have all passed.  Build the tokens and response object
			string encodedJwt = await _tokenHelper.BuildJwtAuthorizationToken(user, options);

			UserSlim response = _userHelper.UserToUserSlim(user);
			var lastSignIn = Builders<User>.Update.Set(u => u.LastSignin, DateTime.Now);
			_db.Users.UpdateOne(u => u.Id == user.Id, lastSignIn);

			_tokenHelper.BuildResponseCookie(Request.HttpContext, encodedJwt);

			return Ok(response);
		}

		[Route("api/v1/logout")]
		[HttpGet]
		public IActionResult Logout()
		{
			Response.Cookies.Delete("access_token");
			return Ok();
		}

		[HttpPost]
		[Route("api/v1/users")]
		public IActionResult CreateUser([FromBody]RegisterDataModel model)
		{
			if (ModelState.IsValid)
			{
				if (model.Email == null || model.Email == "")
					return BadRequest("Email is required");

				if (_validationHelper.ValidateEmail(model.Email) == false)
					return BadRequest("Valid email address is required");

				User existing = _userHelper.GetUserByEmail(model.Email);
				if (existing != null)
					return BadRequest("Email address already used.");

				if (model.Password == null || model.Password == "")
					return BadRequest("Password is required");

				if (model.Password != model.ConfirmPassword)
					return BadRequest("Passwords do not match");

				if (!_userHelper.IsValidPassword(model.Password))
					return BadRequest("Password is not complex enough.");

				//Create the user
				User user = new User();
				user.Email = model.Email;
				user.Salt = _userHelper.CreatUserSalt();
				user.Password = _hasherHelper.GetHash(model.Password + user.Salt);

				//As part of this demo, manually activate the account here. There are activation services available - just finish tying in the email logic.
				user.EmailValidated = true;

				bool result = _userHelper.CreateUser(user);
				if (result)
					return Ok(new IdResponse() { Id = user.Id });
				else
					return BadRequest("Could not create user profile.");
			}
			else
				return BadRequest("Invalid data");

		}

		[HttpGet]
		[Route("api/v1/users/{id}")]
		[Authorize]
		public IActionResult GetUser(string id)
		{
			if (ModelState.IsValid)
			{
				User user = _userHelper.GetUserById(User.Identity.Name);
				if (user == null)
					return NotFound();

				//Remove this if you want to allow user info to be requested by others users.
				if (id != user?.Id && id != "me")
					return BadRequest("Invalid Permissions");

				if (user != null)
				{
					UserSlim response = _userHelper.UserToUserSlim(user);
					return Ok(response);
				}
				else
					return NotFound();
			}
			else
				return BadRequest();
		}



		[Route("api/v1/users/{id}/password")]
		[HttpPut]
		[Authorize]
		public IActionResult ChangePassword(string id, [FromBody]ChangePasswordDataModel model)
		{
			if (ModelState.IsValid)
			{
				User user = _userHelper.GetUserById(User.Identity.Name);

				//Ensure that the requesting user is changing their own password. Change this to allow administrators to modify passwords.
				if (user?.Id != id)
					return BadRequest("Invalid Permissions");

				if (model.NewPassword == null || model.NewPassword == "")
					return BadRequest("Password is required");

				//Password confirm must match
				if (model.NewPassword != model.ConfirmPassword)
					return BadRequest("Password and Confirmation must match");

				//Validate that the password meets the complexity requirements.
				if (!_userHelper.IsValidPassword(model.NewPassword))
					return BadRequest("Password is not strong enough");

				//Ensure they passed the old password
				if (model.OldPassword == null)
					return BadRequest("Invalid Password");

				//Check that they know the existing password
				string password = _hasherHelper.GetHash(model.OldPassword + user.Salt);
				if (user.Password == password) { }
				else
					return BadRequest("Invalid Password");

				//Change password is difficult, just remove the password and then add a new password based on the user input.
				string newSalt = _userHelper.CreatUserSalt();
				string newPasswordHash = _hasherHelper.GetHash(model.NewPassword + newSalt);


				//If these parameters grow, us the AddToSet method to build 1 update operation, instead of 2 as seen here.
				var updatePasswordAndSalt = Builders<User>.Update
					.Set(u => u.Salt, newSalt)
					.Set(u => u.Password, newPasswordHash);

				_db.Users.UpdateOne(u => u.Id == user.Id, updatePasswordAndSalt);

				return Ok();
			}
			else
				return BadRequest(ModelState);
		}



		[Authorize]
		[Route("api/v1/users/{id}")]
		[HttpPut]
		public IActionResult UpdateAccount(string id, [FromBody]AccountUpdateDataModel model)
		{
			if (ModelState.IsValid)
			{
				User user = _userHelper.GetUserById(User.Identity.Name);

				//Modify this if you want to allow user data to be requested that doesn't belog to the reques
				if (id != user?.Id)
					return BadRequest("Invalid Permissions");

				if (model.Email == null || model.Email == "")
					return BadRequest("Email address is required");

				if (_validationHelper.ValidateEmail(model.Email) == false)
					return BadRequest("Invalid Email address");
				//Check if the email is already used by an existing user.  If so, return conflict.
				User existing = _userHelper.GetUserByEmail(model.Email);
				if (existing != null && user.Email != model.Email)
					return BadRequest("Email address is used by another account");

				var update = Builders<User>.Update
					.Set(u => u.Email, model.Email)
					.Set(u => u.Name, model.Name);

				_db.Users.UpdateOne(u => u.Id == id, update);

				return Ok();
			}
			else
				return BadRequest(ModelState);
		}


		[HttpPost]
		[Route("api/v1/users/recovery")]
		public IActionResult PasswordRecovery([FromBody]ForgotPasswordDataModel model)
		{
			///
			/// User submits, system checks for username, if the username exists, it emails the user the reset key/email.
			/// If the username does not exist, the user will still see the same message, this is to ensure that somebody
			/// doesn't just attempt to guess the username/email.
			/// Password Recovery emails are sent via the UPQ.
			///
			if (ModelState.IsValid)
			{
				if (model.Email == null || model.Email == "")
					return BadRequest("Email is required");

				User user = _userHelper.GetUserByEmail(model.Email);
				if (user != null)
				{
					PasswordRecoveryToken items = new PasswordRecoveryToken()
					{
						Expiration = DateTime.Now + new TimeSpan(2, 0, 0, 0),
						UserId = user.Id
					};

					var jwt = _tokenHelper.EncodeStandardJwtToken(items);

					try
					{
						//Send recovery email containing token
					}
					catch
					{
					}

					return Ok();//Do not return the recoveryToken in the service.  Send a recovery email to validate the users ownership of the account.
				}
				else
					return NotFound();
			}
			else
				return BadRequest();
		}

		[HttpPost]
		[Route("api/v1/users/recovery/{token}")]
		public IActionResult PasswordReset(string token, [FromBody]ResetPasswordDataModel model)
		{
			//http://stackoverflow.com/questions/25372035/not-able-to-validate-json-web-token-with-net-key-to-short

			if (ModelState.IsValid)
			{
				if (model.NewPassword == null || model.NewPassword == "")
					return BadRequest("Password is required");

				if (model.NewPassword != model.ConfirmPassword)
					return BadRequest("Passwords do not match");

				if (!_userHelper.IsValidPassword(model.NewPassword))
					return BadRequest("Password is not complex enough.");

				PasswordRecoveryToken recoveryToken = _tokenHelper.DecodeStandardJwtToken<PasswordRecoveryToken>(token);

				User user = _userHelper.GetUserById(recoveryToken.UserId);

				string newSalt = _userHelper.CreatUserSalt();
				string newPasswordHash = _hasherHelper.GetHash(model.NewPassword + newSalt);

				var updatePasswordAndSalt = Builders<User>.Update
					.Set(u => u.Salt, newSalt)
					.Set(u => u.Password, newPasswordHash);

				_db.Users.UpdateOne(u => u.Id == user.Id, updatePasswordAndSalt);

				return Ok();
			}
			else
				return BadRequest(ModelState);
		}


		[Authorize]
		[Route("api/v1/users/{id}")]
		[HttpDelete]
		public IActionResult DeleteUser(string id)
		{
			if (ModelState.IsValid)
			{
				User user = _userHelper.GetUserById(User.Identity.Name);

				//Validate that you own the account that is being deleted.
				if (id != user?.Id)
					return BadRequest("Invalid Permissions");

				//Delete any Todo items owned by the user.
				_db.Todos.DeleteMany(t => t.Owner == user.Id);

				//Delete the user profile.
				_db.Users.DeleteOne(u => u.Id == user.Id);

				Response.Cookies.Delete("access_token");
				return Ok();
			}
			else
				return BadRequest(ModelState);
		}

    }
}
