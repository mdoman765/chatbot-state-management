using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace crud_app_backend.DTOs
{
    /// <summary>
    /// Sent by n8n when a staff member confirms Y on the complaint screen.
    /// Contains staff info (from HRIS session), text description, and
    /// the message IDs of already-uploaded voice and image files.
    /// </summary>
    public class SubmitComplaintRequestDto
    {
        // ── Who is submitting ─────────────────────────────────────────────────

        [Required]
        [JsonPropertyName("whatsapp_phone")]
        public string WhatsappPhone { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("staff_id")]
        public string StaffId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("official_phone")]
        public string? OfficialPhone { get; set; }

        [JsonPropertyName("designation")]
        public string? Designation { get; set; }

        [JsonPropertyName("dept")]
        public string? Dept { get; set; }

        [JsonPropertyName("groupname")]
        public string? GroupName { get; set; }

        [JsonPropertyName("company")]
        public string? Company { get; set; }

        [JsonPropertyName("locationname")]
        public string? LocationName { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        // ── Complaint content ─────────────────────────────────────────────────

        /// <summary>All text messages the user sent, joined with newline.</summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// IDs returned by POST /api/whatsapp/messages/voice when each
        /// voice note was uploaded. Your backend fetches the actual file
        /// using these IDs and forwards the binary to the CRM.
        /// </summary>
        [JsonPropertyName("voice_message_ids")]
        public List<string> VoiceMessageIds { get; set; } = new();

        /// <summary>
        /// IDs returned by POST /api/whatsapp/messages/image when each
        /// image was uploaded.
        /// </summary>
        [JsonPropertyName("image_message_ids")]
        public List<string> ImageMessageIds { get; set; } = new();
    }
}
