using instantMessagingCore.Models.Dto;
using instantMessagingServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace instantMessagingServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class KeysController : ControllerBase
    {

        private readonly IConfiguration Configuration;
        private readonly Authentication authentication;
        private readonly LogsManager logsManager = LogsManager.GetInstance();

        public KeysController(IConfiguration configuration)
        {
            Configuration = configuration;
            this.authentication = Authentication.GetInstance();
        }

        /// <summary>
        /// Return the selected friend user public key
        /// </summary>
        /// <param name="friendId">friend id user</param>
        /// <returns>the selecte public key</returns>
        // GET api/<KeysController>/5
        [HttpGet("get/{friendId}")]
        public IActionResult GetFriendKey(int friendId)
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
                    var publicKey = db.PublicKeys.FirstOrDefault(pk => pk.UserId == friendId);
                    if (publicKey != null)
                    {
                        response = Ok(publicKey);
                    }
                    else
                    {
                        logsManager.write(Logs.EType.info, $"function: {nameof(GetFriendKey)}, error: key {nameof(NotFound)}, User: {User.Identity.Name} token, FriendId: {friendId}");
                        response = NotFound();
                    }
                }
                else
                {
                    logsManager.write(Logs.EType.warning, $"function: {nameof(GetFriendKey)}, error: {nameof(Unauthorized)}, User: {User.Identity.Name} token, FriendId: {friendId}");
                }
            }
            else
            {
                logsManager.write(Logs.EType.warning, $"function: {nameof(GetFriendKey)}, error: {nameof(Unauthorized)}, User: {User.Identity.Name} token");
            }

            return response;
        }

        /// <summary>
        /// Registre the user public key
        /// </summary>
        /// <param name="pk">The public key to register</param>
        /// <returns>the http status</returns>
        // POST api/<KeysController>
        [HttpPost("submit")]
        public IActionResult SendMyKey([FromBody] PublicKeys pk)
        {
            IActionResult response = Unauthorized();

            if (ModelState.IsValid)
            {
                var ClaimIDToken = User.Claims.FirstOrDefault((c) => c.Type == "IDToken");
                if (ClaimIDToken != null && authentication.isAutheticate(User.Identity.Name, ClaimIDToken))
                {
                    DatabaseContext db = new(Configuration);
                    var currentUser = db.Users.FirstOrDefault(u => u.Username == User.Identity.Name);

                    if (currentUser != null && pk.UserId == currentUser.Id)
                    {

                        var dbpk = db.PublicKeys.FirstOrDefault(p => p.UserId == currentUser.Id);
                        if (dbpk == null)
                        {
                            db.PublicKeys.Add(pk);
                        }
                        else
                        {
                            dbpk.Key = pk.Key;
                            db.PublicKeys.Update(dbpk);
                        }
                        db.SaveChanges();
                        response = Ok();
                    }
                    else
                    {
                        logsManager.write(Logs.EType.warning, $"function: {nameof(SendMyKey)}, error: {nameof(Unauthorized)}, {nameof(PublicKeys)} from {nameof(pk.UserId)}:{pk.UserId} does not belong to {User.Identity.Name} token user");
                        response = Unauthorized();
                    }
                }
                else
                {
                    logsManager.write(Logs.EType.warning, $"function: {nameof(SendMyKey)}, error: {nameof(Unauthorized)}, User: {User.Identity.Name} token");
                }
            }
            else
            {
                logsManager.write(Logs.EType.error, $"function: {nameof(SendMyKey)}, error: {nameof(PublicKeys)} model invalid, User: {User.Identity.Name} token");
            }

            return response;
        }

        /*
        [Route("/{**catchAll}")]
        public IActionResult CatchAll([FromBody] object body, string catchAll)
        {
            logsManager.write(Logs.EType.warning, $"function: {nameof(CatchAll)}, enable");
            return Ok(catchAll);
        }*/
    }
}
