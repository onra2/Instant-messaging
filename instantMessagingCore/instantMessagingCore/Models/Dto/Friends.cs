using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace instantMessagingCore.Models.Dto
{
    public class Friends
    {
        /// <summary>
        /// The owner id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The owner friend
        /// </summary>
        public int FriendId { get; set; }

        /// <summary>
        /// The friend request status
        /// </summary>
        public RequestStatus Status { get; set; }

        public Friends(int userId, int friendId)
        {
            UserId = userId;
            FriendId = friendId;
            Status = RequestStatus.waiting;
        }

        /// <summary>
        /// A enum of friend request status
        /// </summary>
        public enum RequestStatus
        {
            waiting,
            accepted,
            blocked
        }

        /// <summary>
        /// A enum of possible action on friend request
        /// </summary>
        public enum Action
        {
            accept,
            refuse,
            block
        }
    }
}
