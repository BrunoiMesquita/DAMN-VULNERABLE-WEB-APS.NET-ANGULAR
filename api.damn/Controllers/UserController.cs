using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using logic.damn;
using model.damn;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.damn.Controllers
{
	public class UserController : Controller
	{
		private UserLogic _userHelper { get; set; }
		private ValidationLogic _validationHelper { get; set; }
		private HasherLogic _hasherHelper { get; set; }
		private TokenLogic _tokenHelper { get; set; }
		private ApplicationSettings _applicationSettings { get; set; }
		public UserController(UserLogic userHelper, ValidationLogic validationHelper, HasherLogic hasherHelper, TokenLogic tokenHelper, ApplicationSettings applicationSettings)
		{
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
            string username = model.Email?.ToLower()?.Trim();
            string password = model.Password?.Trim();

            //Client validation passed.  Validate credentials.
            //Does the user have a valid account and did they provide a valid username/password.
            //Does user have valid credentials
            UserValidationResponse validation = await _userHelper.ValidateUserIdentityAsync(username, password);
            if (validation.Code == UserValidationResponseCode.Invalid)
            {
                return BadRequest("Invalid Username or Password");
            }
            else if (validation.Code == UserValidationResponseCode.LockedOut)
            {
                return BadRequest("Account is Locked. Wait 30 minutes.");
            }
            else if (validation.Code == UserValidationResponseCode.Invalidated)
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
            string encodedJwt = await _tokenHelper.BuildJwtAuthorizationToken(validation.User, options);

            UserSlim response = _userHelper.UserToUserSlim(validation.User);
            await _userHelper.UserSetLastSignInAsync(validation.User);

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
        [Route("api/v1/user")]
        async public Task<IActionResult> CreateUser([FromBody]RegisterDataModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Email == null || model.Email == "")
                    return BadRequest("Email is required");

                if (_validationHelper.Email(model.Email) == false)
                    return BadRequest("Valid email address is required");

                User existing = await _userHelper.GetUserByEmailAsync(model.Email);
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
                user.Email = model.Email.ToLower();
                user.Salt = _userHelper.CreatUserSalt();
                user.Password = _hasherHelper.GetHash(model.Password + user.Salt);

                bool result = await _userHelper.CreateUserAsync(user);
                if (result)
                {
                    //Use this if you want to send an activation email.
                    //await _emailHelper.SendActivationEmailAsync(user);
                    return Ok(new IdResponse() { Id = user.Id });
                }
                else
                    return BadRequest("Could not create user profile.");
            }
            else
                return BadRequest("Invalid data");

        }

        [HttpGet]
        [Route("api/v1/user/{id}")]
        [Authorize]
        async public Task<IActionResult> GetUser(string id)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserByIdAsync(User.Identity.Name);
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



        [Route("api/v1/user/{id}/password")]
        [HttpPut]
        [Authorize]
        async public Task<IActionResult> ChangePassword(string id, [FromBody]ChangePasswordDataModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserByIdAsync(User.Identity.Name);

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

                await _userHelper.ChangePasswordAsync(user, newSalt, newPasswordHash);

                return Ok();
            }
            else
                return BadRequest(ModelState);
        }




        [Authorize]
        [Route("api/v1/user/{id}")]
        [HttpPut]
        async public Task<IActionResult> UpdateAccount(string id, [FromBody]AccountUpdateDataModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserByIdAsync(User.Identity.Name);

                //Modify this if you want to allow user data to be requested that doesn't belog to the reques
                if (id != user?.Id)
                    return BadRequest("Invalid Permissions");

                if (model.Email == null || model.Email == "")
                    return BadRequest("Email address is required");

                if (_validationHelper.Email(model.Email) == false)
                    return BadRequest("Invalid Email address");
                //Check if the email is already used by an existing user.  If so, return conflict.
                User existing = await _userHelper.GetUserByEmailAsync(model.Email);
                if (existing != null && user.Email != model.Email)
                    return BadRequest("Email address is used by another account");

                await _userHelper.UpdateUserAsync(id, model.Email, model.Name);

                return Ok();
            }
            else
                return BadRequest(ModelState);
        }

        [Authorize]
        [Route("api/v1/user/{id}")]
        [HttpDelete]
        async public Task<IActionResult> DeleteUser(string id)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserByIdAsync(User.Identity.Name);

                //Validate that you own the account that is being deleted.
                if (id != user?.Id)
                    return BadRequest("Invalid Permissions");

                //Delete the user profile.
                await _userHelper.DeleteUserAsync(user);

                Response.Cookies.Delete("access_token");
                return Ok();
            }
            else
                return BadRequest(ModelState);
        }

        [HttpGet]
        [Route("api/v1/user/activate/{token}")]
        async public Task<IActionResult> ActivateUser(string token)
        {
            User user = await _userHelper.ValidateActivationToken(token);
            if (user == null)
                return BadRequest();

            await _userHelper.ActivateUser(user);

            return Ok();
        }

        [HttpPost]
        [Route("api/v1/user/activation/resend")]
        async public Task<IActionResult> ResendActivationEmail([FromBody]ResendActivationDataModel model)
        {
            User user = await _userHelper.GetUserByEmailAsync(model.Email);
            if (user == null)
                return BadRequest();
            
            //Use this if you want to send another activation email email
            //await _emailHelper.SendReActivationEmailAsync(user);

            return Ok();
        }

        [HttpPost]
        [Route("api/v1/user/recovery")]
        async public Task<IActionResult> PasswordRecovery([FromBody]ForgotPasswordDataModel model)
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

                User user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user != null)
                {
                    //Use this if youw wanto send a recovery email
                    //await _emailHelper.SendRecoveryEmailAsync(user);
                    return Ok();//Do not return the recoveryToken in the service.  Send a recovery email to validate the users ownership of the account.
                }
                else
                    return BadRequest("No account with the specified email address");
            }
            else
                return BadRequest();
        }

        [HttpPost]
        [Route("api/v1/user/recovery/{token}")]
        async public Task<IActionResult> PasswordReset(string token, [FromBody]ResetPasswordDataModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.NewPassword == null || model.NewPassword == "")
                    return BadRequest("Password is required");

                if (model.NewPassword != model.ConfirmPassword)
                    return BadRequest("Passwords do not match");

                if (!_userHelper.IsValidPassword(model.NewPassword))
                    return BadRequest("Password is not complex enough.");

                try
                {
                    User user = await _userHelper.ValidateRecoveryToken(token);
                    if (user == null)
                        return BadRequest("Invalid Token");

                    await _userHelper.UpdateUserPasswordByUserIdAsync(user.Id, model.NewPassword);
                }
                catch
                {
                    return BadRequest();
                }

                return Ok("Password changed.  Sign in to continue using the application.");
            }
            else
                return BadRequest(ModelState);
        }

    }
}
