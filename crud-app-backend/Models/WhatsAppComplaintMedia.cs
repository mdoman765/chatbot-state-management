using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace crud_app_backend.Models
{
    /// <summary>
    /// One row per voice note or image attached to a complaint.
    /// MessageId links back to WhatsAppMessages for the actual file on disk.
    /// </summary>
    public class WhatsAppComplaintMedia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // ── FK to parent complaint ────────────────────────────────────────────
        public int ComplaintId { get; set; }

        [ForeignKey(nameof(ComplaintId))]
        public WhatsAppComplaint Complaint { get; set; } = default!;

        // ── Link to original WhatsApp message ─────────────────────────────────
        /// <summary>
        /// 360dialog messageId (wamid.HBgN…).
        /// Use this to look up WhatsAppMessages.FileUrl for the actual file.
        /// </summary>
        [Required][MaxLength(255)]
        public string MessageId { get; set; } = default!;

        /// <summary>voice | image</summary>
        [Required][MaxLength(10)]
        public string MediaType { get; set; } = default!;

        /// <summary>Copied from WhatsAppMessages.FileUrl e.g. /wa-media/audio/wamid.xxx.ogg</summary>
        [MaxLength(2048)]
        public string? FileUrl { get; set; }

        [MaxLength(100)]
        public string? MimeType { get; set; }

        /// <summary>Image caption if any.</summary>
        [MaxLength(1000)]
        public string? Caption { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
