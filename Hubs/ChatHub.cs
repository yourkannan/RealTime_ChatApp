using CHAT.Data;
using CHAT.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CHAT.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatContext _db;
        private static readonly Dictionary<string, int> Conns = new();

        public ChatHub(ChatContext db) => _db = db;

        public override async Task OnConnectedAsync()
        {
            var http = Context.GetHttpContext();
            var uid = http?.Session.GetInt32("UserId");
            if (uid == null) { Context.Abort(); return; }

            Conns[Context.ConnectionId] = uid.Value;

            var user = await _db.Users.FindAsync(uid);
            if (user != null)
            {
                user.IsOnline = true;
                await _db.SaveChangesAsync();
            }

            await Clients.Caller.SendAsync("Connected", uid.Value);
            await BroadcastUsers();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            if (Conns.TryGetValue(Context.ConnectionId, out int uid))
            {
                var user = await _db.Users.FindAsync(uid);
                if (user != null)
                {
                    user.IsOnline = false;
                    await _db.SaveChangesAsync();
                }
            }

            Conns.Remove(Context.ConnectionId);
            await BroadcastUsers();
            await base.OnDisconnectedAsync(ex);
        }

        public async Task SendMessage(int toUserId, string message)
        {
            if (!Conns.TryGetValue(Context.ConnectionId, out var fromId)) return;

            var msg = new ChatMessage { FromUserId = fromId, ToUserId = toUserId, Message = message };
            _db.ChatMessages.Add(msg);
            await _db.SaveChangesAsync();

            await Clients.Caller.SendAsync("ReceiveMessage", fromId, message, msg.Timestamp);
            foreach (var conn in Conns.Where(x => x.Value == toUserId).Select(x => x.Key))
                await Clients.Client(conn).SendAsync("ReceiveMessage", fromId, message, msg.Timestamp);
        }

        public async Task LoadMessages(int withUserId)
        {
            if (!Conns.TryGetValue(Context.ConnectionId, out var currentUserId)) return;

            var messages = await _db.ChatMessages
                .Where(m =>
                    (m.FromUserId == currentUserId && m.ToUserId == withUserId) ||
                    (m.FromUserId == withUserId && m.ToUserId == currentUserId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            var mapped = messages.Select(m => new
            {
                fromUserId = m.FromUserId,
                message = m.Message,
                timestamp = m.Timestamp.ToLocalTime().ToString("HH:mm")
            }).ToList();

            await Clients.Caller.SendAsync("LoadMessages", withUserId, mapped);
        }

        private async Task BroadcastUsers()
        {
            var users = await _db.Users.ToListAsync();

            var currentUserId = Context.GetHttpContext()?.Session.GetInt32("UserId") ?? 0;

            var result = users
                .Where(u => u.Id != currentUserId)
                .Select(u => new { u.Id, u.Username, u.AvatarUrl, u.IsOnline })
                .ToList();

            await Clients.Caller.SendAsync("UserList", result);
        }
    }
}