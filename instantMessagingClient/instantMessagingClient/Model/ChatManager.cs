using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace instantMessagingClient.Model
{
    class ChatManager
    {
        // Singleton manager
        private static ChatManager instance { get; set; }

        public static ChatManager getInstance()
        {
            return instance ??= new ChatManager();
        }

        private ChatManager()
        {
            ChatInstances = new Dictionary<int, ChatInstance>();
        }

        private Dictionary<int, ChatInstance> ChatInstances { get; set; }

        // Instance
        public void AskUpdate(int friendId)
        {

            GetOrAddChatInstance(friendId).onTextChangeTrigger(EventArgs.Empty);
        }

        public void AddEvent(int friendId, EventHandler eventHandler)
        {
            GetOrAddChatInstance(friendId).OnTextChange += eventHandler;
        }

        public void ClearEvent(int friendId)
        {
            GetOrAddChatInstance(friendId).ClearEvent();
        }

        public ChatInstance GetOrAddChatInstance(int friendId)
        {
            if (!ChatInstances.ContainsKey(friendId))
            {
                ChatInstances.Add(friendId, new ChatInstance());
            }

            return ChatInstances[friendId] ??= new ChatInstance();
        }

        public class ChatInstance
        {
            public event EventHandler OnTextChange;

            public virtual void onTextChangeTrigger(EventArgs e)
            {
                OnTextChange?.Invoke(this, e);
            }

            public void ClearEvent()
            {
                OnTextChange = null;
            }
        }
    }
}
