using System.ComponentModel.DataAnnotations;

namespace CHAT.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string AvatarUrl { get; set; } = "/images/default-avatar.png"; // Default avatar

        public bool IsOnline { get; set; } = false; // For online/offline indicator

        public ICollection<ChatMessage> SentMessages { get; set; } = new List<ChatMessage>();
        public ICollection<ChatMessage> ReceivedMessages { get; set; } = new List<ChatMessage>();
    }
}
