using instantMessagingServer.Models;
using instantMessagingCore.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace instantMessagingServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PeersController : ControllerBase
    {
        private const int heartbeatLimit = 5;
        private readonly IConfiguration Configuration;
        private readonly Authentication authentication;

        private readonly LogsManager logsManager = LogsManager.GetInstance();

        public PeersController(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
            this.authentication = Authentication.GetInstance();
        }

        /// <summary>
        /// Return the friend peer information
        /// </summary>
        /// <param name="friendId">The friend id</param>
        /// <returns>The friend peer</returns>
        // GET api/<PeersController>/5
        [HttpGet("{friendId}")]
        public IActionResult GetPeer(int friendId)
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
                    var peer = db.Peers.FirstOrDefault(p => p.UserId == friendId);
                    if (peer != null && (DateTime.Now - peer.LastHeartBeat).TotalMinutes <= heartbeatLimit)
                    {
                        response = Ok(peer);
                    }
                    else
                    {
                        logsManager.write(Logs.EType.warning, $"function: {nameof(GetPeer)}, error: {nameof(NotFound)}, User: {User.Identity.Name} token, Friendid: {friendId}");
                        response = NotFound();
                    }
                }
                else
                {
                    logsManager.write(Logs.EType.warning, $"function: {nameof(GetPeer)}, error: {nameof(Unauthorized)}, User: {User.Identity.Name} token, Friendid: {friendId}");
                }
            }
            else
            {
                logsManager.write(Logs.EType.warning, $"function: {nameof(GetPeer)}, error: {nameof(Unauthorized)}, User: {User.Identity.Name} token");
            }

            return response;
        }

        /// <summary>
        /// Set the current user peer information
        /// </summary>
        /// <param name="peer">The peer data</param>
        /// <returns>Http request result</returns>
        // POST api/<PeersController>
        [HttpPost("Submit")]
        public IActionResult PostMyPeer([FromBody] Peers peer)
        {
            IActionResult response = Unauthorized();

            if (ModelState.IsValid)
            {
                var ClaimIDToken = User.Claims.FirstOrDefault((c) => c.Type == "IDToken");
                if (ClaimIDToken != null && authentication.isAutheticate(User.Identity.Name, ClaimIDToken))
                {
                    DatabaseContext db = new(Configuration);
                    var currentUser = db.Users.FirstOrDefault(u => u.Username == User.Identity.Name);

                    if (peer.UserId == currentUser.Id)
                    {
                        var dbPeer = db.Peers.FirstOrDefault(p => p.UserId == currentUser.Id);

                        if (dbPeer != null)
                        {
                            dbPeer.Ipv4 = peer.Ipv4;
                            dbPeer.Ipv6 = peer.Ipv6;
                            dbPeer.Port = peer.Port;
                            dbPeer.LastHeartBeat = peer.LastHeartBeat;
                            db.Peers.Update(dbPeer);
                        }
                        else
                        {
                            db.Peers.Add(peer);
                        }

                        db.SaveChanges();

                        response = Ok();
                    }
                    else
                    {
                        logsManager.write(Logs.EType.error, $"function: {nameof(PostMyPeer)}, error: {nameof(Unauthorized)}, {nameof(Peers)} from {nameof(peer.UserId)}:{peer.UserId} does not belong to {User.Identity.Name} token user");
                    }
                }
                else
                {
                    logsManager.write(Logs.EType.warning, $"function: {nameof(PostMyPeer)}, error: {nameof(Unauthorized)}, User: {User.Identity.Name} token");
                }
            }
            else
            {
                logsManager.write(Logs.EType.error, $"function: {nameof(PostMyPeer)}, error: {nameof(Peers)} ModelState invalid, User: {User.Identity.Name} token");
            }

            return response;
        }

        /// <summary>
        /// Update the heartbeat data from the current user
        /// </summary>
        /// <returns>Http request result</returns>
        // POST api/<PeersController>
        [HttpPut("heartbeat")]
        public IActionResult UpdateHeartBeat()
        {
            IActionResult response = Unauthorized();

            var ClaimIDToken = User.Claims.FirstOrDefault((c) => c.Type == "IDToken");
            if (ClaimIDToken != null && authentication.isAutheticate(User.Identity.Name, ClaimIDToken))
            {
                DatabaseContext db = new(Configuration);
                var currentUser = db.Users.FirstOrDefault(u => u.Username == User.Identity.Name);

                var peer = db.Peers.FirstOrDefault(p => p.UserId == currentUser.Id);
                if (peer != null)
                {
                    peer.LastHeartBeat = DateTime.Now;
                    db.Peers.Update(peer);
                    db.SaveChanges();

                    response = Ok();
                }
            }

            return response;
        }
    }
}
