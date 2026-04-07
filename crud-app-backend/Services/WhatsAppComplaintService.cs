using crud_app_backend.DTOs;
using crud_app_backend.Models;
using crud_app_backend.Repositories;

namespace crud_app_backend.Services
{
    public class WhatsAppComplaintService : IWhatsAppComplaintService
    {
        private readonly IWhatsAppComplaintRepository _complaintRepo;
        private readonly IWhatsAppMessageRepository _messageRepo;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<WhatsAppComplaintService> _logger;

        public WhatsAppComplaintService(
            IWhatsAppComplaintRepository complaintRepo,
            IWhatsAppMessageRepository messageRepo,
            IWebHostEnvironment env,
            ILogger<WhatsAppComplaintService> logger)
        {
            _complaintRepo = complaintRepo;
            _messageRepo = messageRepo;
            _env = env;
            _logger = logger;
        }

        public async Task<SubmitComplaintResponseDto> SubmitAsync(
            SubmitComplaintRequestDto req, CancellationToken ct)
        {
            _logger.LogInformation(
                "[Complaint] Submit — staff={S} phone={P} voices={V} images={I}",
                req.StaffId, req.WhatsappPhone,
                req.VoiceMessageIds.Count, req.ImageMessageIds.Count);

            // ── 1. Save complaint row (ComplaintNumber starts as null) ─────────
            var complaint = new WhatsAppComplaint
            {
                WhatsappPhone = req.WhatsappPhone,
                StaffId = req.StaffId,
                StaffName = req.Name,
                OfficialPhone = req.OfficialPhone,
                Designation = req.Designation,
                Dept = req.Dept,
                GroupName = req.GroupName,
                Company = req.Company,
                LocationName = req.LocationName,
                Email = req.Email,
                Description = req.Description,
                Status = "open",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _complaintRepo.InsertAsync(complaint, ct);   // was CancellationToken.None — fixed

            // ── 2. Assign complaint number  PR00001, PR00002 … ────────────────
            var complaintNumber = $"PR{complaint.Id:D5}";
            await _complaintRepo.UpdateComplaintNumberAsync(complaint.Id, complaintNumber, ct);

            // ── 3. Save voice media rows ──────────────────────────────────────
            foreach (var msgId in req.VoiceMessageIds)
            {
                if (string.IsNullOrWhiteSpace(msgId)) continue;

                var waMsg = await _messageRepo.GetByMessageIdAsync(msgId, ct);

                await _complaintRepo.InsertMediaAsync(new WhatsAppComplaintMedia
                {
                    ComplaintId = complaint.Id,
                    MessageId = msgId,
                    MediaType = "voice",
                    FileUrl = waMsg?.FileUrl,
                    MimeType = waMsg?.MimeType ?? "audio/ogg",
                }, ct);
            }

            // ── 4. Save image media rows ──────────────────────────────────────
            foreach (var msgId in req.ImageMessageIds)
            {
                if (string.IsNullOrWhiteSpace(msgId)) continue;

                var waMsg = await _messageRepo.GetByMessageIdAsync(msgId, ct);

                await _complaintRepo.InsertMediaAsync(new WhatsAppComplaintMedia
                {
                    ComplaintId = complaint.Id,
                    MessageId = msgId,
                    MediaType = "image",
                    FileUrl = waMsg?.FileUrl,
                    MimeType = waMsg?.MimeType ?? "image/jpeg",
                    Caption = waMsg?.Caption,
                }, ct);
            }

            _logger.LogInformation(
                "[Complaint] Saved — complaintNumber={N} voices={V} images={I}",
                complaintNumber, req.VoiceMessageIds.Count, req.ImageMessageIds.Count);

            return new SubmitComplaintResponseDto
            {
                Success = true,
                ComplaintId = complaintNumber,
                Message = "Complaint saved successfully",
            };
        }
    }
}
