using instantMessagingCore.Models.Dto;
using instantMessagingServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace instantMessagingServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {

        private readonly IConfiguration Configuration;
        private readonly Authentication authentication;
        private readonly LogsManager logsManager = LogsManager.GetInstance();

        public FriendsController(IConfiguration configuration)
        {
            Configuration = configuration;
            this.authentication = Authentication.GetInstance();
        }

        /// <summary>
        /// Return the current user friends list
        /// </summary>
        /// <returns>the friends list</returns>
        // GET: api/<FriendsController>/
        [HttpGet]
        public IActionResult GetFriendList()
        {
            IActionResult response = Unauthorized();

            var ClaimIDToken = User.Claims.FirstOrDefault((c) => c.Type == "IDToken");
            if (ClaimIDToken != null && authentication.isAutheticate(User.Identity.Name, ClaimIDToken))
            {
                DatabaseContext db = new(Configuration);

                var currentUser = db.Users.FirstOrDefault(u => u.Username == User.Identity.Name);

                var friendList = db.Friends.Where(f => (f.UserId == currentUser.Id || f.FriendId == currentUser.Id) && f.Status == Friends.RequestStatus.accepted);

                response = Ok(friendList);

            }
            else
            {
                logsManager.write(Logs.EType.warning, $"function: {nameof(GetFriendList)}, error: {nameof(Unauthorized)}, User: {User.Identity.Name} token");
            }

            return response;
        }

        /// <summary>
        /// Return the yours pending friend request
        /// </summary>
        /// <returns>yours pending friend request</returns>
        // GET: api/<FriendsController>/pendingrequest
        [HttpGet("pendingrequest")]
        public IActionResult GetPendingrequest()
        {
            IActionResult response = Unauthorized();

            var ClaimIDToken = User.Claims.FirstOrDefault((c) => c.Type == "IDToken");
            if (ClaimIDToken != null && authentication.isAutheticate(User.Identity.Name, ClaimIDToken))
            {
                DatabaseContext db = new(Configuration);

                var currentUser = db.Users.FirstOrDefault(u => u.Username == User.Identity.Name);

                var pendingRequest = db.Friends.Where(f => f.FriendId == currentUser.Id && f.Status == Friends.RequestStatus.waiting);

                response = Ok(pendingRequest);

            }
            else
            {
                logsManager.write(Logs.EType.warning, $"function: {nameof(GetPendingrequest)}, error: {nameof(Unauthorized)}, User: {User.Identity.Name} token");
            }

            return response;
        }

        /// <summary>
        /// Process one of yours pending friend request
        /// </summary>
        /// <param name="senderId">the sender id</param>
        /// <param name="requestAction">your action in Friends.Action enum</param>
        /// <returns>The http status request</returns>
        // PUT api/<FriendsController>/pendingrequest/5/0
        [HttpGet("pendingrequest/{senderId}/{requestAction}")]
        public IActionResult ActionOnPendingrequest(int senderId, Friends.Action requestAction)
        {
            IActionResult response = Unauthorized();

            var ClaimIDToken = User.Claims.FirstOrDefault((c) => c.Type == "IDToken");
            if (ClaimIDToken != null && authentication.isAutheticate(User.Identity.Name, ClaimIDToken))
            {
                DatabaseContext db = new(Configuration);

                var currentUser = db.Users.FirstOrDefault(u => u.Username == User.Identity.Name);

                var friend = db.Friends.FirstOrDefault(f => f.UserId == senderId && f.FriendId == currentUser.Id);

                if (friend != null && requestAction == Friends.Action.accept)
                {
                    friend.Status = Friends.RequestStatus.accepted;
                    db.Friends.Update(friend);
                }
                else if(requestAction == Friends.Action.refuse)
                {
                    friend ??= db.Friends.FirstOrDefault(f => f.UserId == currentUser.Id && f.FriendId == senderId);
                    if(friend != null) db.Friends.Remove(friend);
                }
                else if(friend != null && requestAction == Friends.Action.block)
                {
                    friend.Status = Friends.RequestStatus.blocked;
                    db.Friends.Update(friend);
                }

                if (friend != null)
                {
                    db.SaveChanges();
                    response = Ok();
                }
                else
                {
                    logsManager.write(Logs.EType.warning, $"function: {nameof(ActionOnPendingrequest)}, error: {nameof(NotFound)}, Friendid: {friend.UserId}, {nameof(Friends.Action)}: {Enum.GetName(typeof(Friends.Action), requestAction)}");
                    response = NotFound();
                }
            }
            else
            {
                logsManager.write(Logs.EType.warning, $"function: {nameof(ActionOnPendingrequest)}, error: {nameof(Unauthorized)}, User: {User.Identity.Name} token");
            }

            return response;
        }

        /// <summary>
        /// Send a friend request to a other user
        /// </summary>
        /// <param name="friendName">the fiend name to add</param>
        /// <returns>The http status request</returns>
        // GET api/<FriendsController>/request/Neod
        [HttpGet("request/{friendName}")]
        public IActionResult SendRequest(string friendName)
        {
            IActionResult response = Unauthorized();

            var ClaimIDToken = User.Claims.FirstOrDefault((c) => c.Type == "IDToken");
            if (ClaimIDToken != null && authentication.isAutheticate(User.Identity.Name, ClaimIDToken))
            {
                DatabaseContext db = new(Configuration);

                var currentUser = db.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
                var friendUser = db.Users.FirstOrDefault(u => u.Username == friendName);

                if (friendUser == null)
                {
                    logsManager.write(Logs.EType.error, $"function: {nameof(SendRequest)}, error: {nameof(BadRequest)}, {nameof(friendName)}: {friendName} don't exist");
                    response = BadRequest($"{nameof(ArgumentException)}: {nameof(friendName)} {friendName} don't exist");
                }
                else
                {
                    if (!db.Friends.Any(f => (f.UserId == currentUser.Id) && (f.FriendId == friendUser.Id)))
                    {
                        var friend = new Friends(currentUser.Id, friendUser.Id);
                        db.Friends.Add(friend);

                        db.SaveChanges();
                    }

                    response = Ok();
                }

            }
            else
            {
                logsManager.write(Logs.EType.warning, $"function: {nameof(SendRequest)}, error: {nameof(Unauthorized)}, User: {User.Identity.Name} token");
            }

            return response;
        }

    }
}