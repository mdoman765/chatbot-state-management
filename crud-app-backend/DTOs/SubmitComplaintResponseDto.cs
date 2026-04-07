using System.Text.Json.Serialization;

namespace crud_app_backend.DTOs
{
    public class SubmitComplaintResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        /// <summary>Complaint ID returned by CRM (e.g. "PR12345").</summary>
        [JsonPropertyName("complaint_id")]
        public string? ComplaintId { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>Raw JSON body the CRM returned — useful for debugging.</summary>
        [JsonPropertyName("crm_response")]
        public object? CrmResponse { get; set; }
    }

    /// <summary>Represents a stored WhatsApp media message loaded for CRM forwarding.</summary>
    public class WhatsAppMediaFile
    {
        public string MessageId { get; set; } = string.Empty;
        public string FileName   { get; set; } = string.Empty;
        public string MimeType   { get; set; } = string.Empty;
        public byte[] Data       { get; set; } = Array.Empty<byte>();
        public string? Caption   { get; set; }
    }
}
