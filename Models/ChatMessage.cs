using System.ComponentModel.DataAnnotations;

namespace CHAT.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        public int FromUserId { get; set; }
        public User? FromUser { get; set; }

        public int ToUserId { get; set; }
        public User? ToUser { get; set; }

        [Required]
        public string Message { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
