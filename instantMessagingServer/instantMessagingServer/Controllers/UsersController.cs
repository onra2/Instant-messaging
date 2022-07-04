using instantMessagingCore.Crypto;
using instantMessagingCore.Models.Dto;
using instantMessagingServer.Models;
using instantMessagingServer.Models.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace instantMessagingServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly Authentication authentication;

        private readonly LogsManager logsManager = LogsManager.GetInstance();

        public UsersController(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
            this.authentication = Authentication.GetInstance();
        }

        // PUT api/<UsersController>/Connexion
        /// <summary>
        /// Users connexion
        /// </summary>
        /// <param name="user">The user to connect</param>
        /// <returns>The connexion token</returns>
        [HttpPost("Connexion")]
        public IActionResult Connexion([FromBody] UsersBasic user)
        {
            IActionResult response = Unauthorized();

            if (ModelState.IsValid)
            {
                user.Username = user.Username.ToLower();

                DatabaseContext db = new(Configuration);
                var users = db.Users.Where(u => u.Username == user.Username).ToList();
                var selectedUser = users.FirstOrDefault(u => u.Password == PasswordUtils.hashAndSalt(user.Password, u.Salt));

                if (selectedUser != null)
                {
                    var IDToken = Authentication.GetInstance().GetIDToken();
                    var token = JWTTokens.Generate(selectedUser.Username, IDToken, Configuration["Jwt:Key"], Configuration["Jwt:Issuer"], Int32.Parse(Configuration["Jwt:Duration"]));

                    var dbToken = db.Tokens.FirstOrDefault(t => t.UserId == selectedUser.Id);
                    var ExpirationDate = DateTime.Now.AddMinutes(Int32.Parse(Configuration["Jwt:Duration"]));
                    if (dbToken == null)
                    {
                        dbToken = new Tokens(selectedUser.Id, IDToken, ExpirationDate);
                        db.Tokens.Add(dbToken);
                    }
                    else
                    {
                        dbToken.Token = IDToken;
                        dbToken.ExpirationDate = ExpirationDate;
                        db.Tokens.Update(dbToken);
                    }

                    db.SaveChanges();

                    response = Ok(new Tokens(selectedUser.Id, token, ExpirationDate));
                }
                else
                {
                    logsManager.write(Logs.EType.warning, $"function: {nameof(Connexion)}, error: {nameof(Unauthorized)}, {user.Username} {nameof(user.Username)}");
                }
            }
            else
            {
                logsManager.write(Logs.EType.error, $"function: {nameof(Connexion)}, error: {nameof(UsersBasic)} ModelState invalid, User: {user.Username}");
            }

            return response;
        }

        // PUT api/<UsersController>/Inscription
        /// <summary>
        /// Users inscription
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("Inscription")]
        public IActionResult Inscription([FromBody] UsersBasic user)
        {
            IActionResult response = Unauthorized();

            if (ModelState.IsValid)
            {
                user.Username = user.Username.ToLower();

                DatabaseContext db = new(Configuration);

                if (db.Users.Any(u => u.Username == user.Username))
                {
                    logsManager.write(Logs.EType.warning, $"function: {nameof(Inscription)}, error: {nameof(BadRequest)}, {user.Username} {nameof(user.Username)} is already used");
                    response = BadRequest($"{nameof(ArgumentException)}: {nameof(user.Username)} {user.Username} is already used");
                }
                else
                {
                    if (PasswordUtils.CheckPolicy(user.Username, user.Password))
                    {
                        var salt = PasswordUtils.getSalt();
                        var newUser = new Users(user.Username, PasswordUtils.hashAndSalt(user.Password, salt), salt);
                        db.Users.Add(newUser);

                        var IDToken = Authentication.GetInstance().GetIDToken();
                        var token = JWTTokens.Generate(user.Username, IDToken, Configuration["Jwt:Key"], Configuration["Jwt:Issuer"], Int32.Parse(Configuration["Jwt:Duration"]));
                        var ExpirationDate = DateTime.Now.AddMinutes(Int32.Parse(Configuration["Jwt:Duration"]));
                        var dbToken = new Tokens(newUser.Id, IDToken, ExpirationDate);
                        db.Tokens.Add(dbToken);

                        db.SaveChanges();

                        response = Ok(new Tokens(newUser.Id, token, ExpirationDate));
                    }
                    else
                    {
                        response = Unauthorized("PASSWORD_POLICY: the password must be between 8 and 255 characters long" +
                            ", have a capital letter, a lowercase letter, a special character and a number");
                    }

                }
            }
            else
            {
                logsManager.write(Logs.EType.error, $"function: {nameof(Inscription)}, error: {nameof(UsersBasic)} ModelState invalid, User: {user.Username}");
            }

            return response;
        }

        /// <summary>
        /// Return the username of friend by his id
        /// </summary>
        /// <param name="friendId">the friend id to retrieve the username</param>
        /// <returns>the username, NotFound or the http request status</returns>
        [HttpGet("UserById/{friendId}")]
        public IActionResult getUserById(int friendId)
        {
            IActionResult response = Unauthorized();

            var ClaimIDToken = User.Claims.FirstOrDefault((c) => c.Type == "IDToken");
            if (ClaimIDToken != null && authentication.isAutheticate(User.Identity.Name, ClaimIDToken))
            {
                DatabaseContext db = new(Configuration);
                var currentUser = db.Users.FirstOrDefault(u => u.Username == User.Identity.Name);

                if (db.Friends.Any(f =>
                     (f.UserId == currentUser.Id && f.FriendId == friendId) ||
                     (f.UserId == friendId && f.FriendId == currentUser.Id)
                ))
                {
                    var friend = db.Users.FirstOrDefault(u => u.Id == friendId);
                    if (friend != null)
                    {
                        response = Ok(friend.Username);
                    }
                    else
                    {
                        logsManager.write(Logs.EType.info, $"function: {nameof(getUserById)}, error: key {nameof(NotFound)}, User: {User.Identity.Name} token, FriendId: {friendId}");
                        response = NotFound();
                    }
                }
                else
                {
                    logsManager.write(Logs.EType.warning, $"function: {nameof(getUserById)}, error: {nameof(Unauthorized)}, User: {User.Identity.Name} token, FriendId: {friendId}");
                }
            }
            else
            {
                logsManager.write(Logs.EType.warning, $"function: {nameof(getUserById)}, error: {nameof(Unauthorized)}, User: {User.Identity.Name} token");
            }

            return response;
        }
    }
}
