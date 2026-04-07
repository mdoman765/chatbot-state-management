using crud_app_backend.DTOs;

namespace crud_app_backend.Services
{
    public interface IWhatsAppComplaintService
    {
        /// <summary>
        /// Fetches stored voice/image files by their message IDs,
        /// assembles a multipart POST, and forwards to the CRM API.
        /// Returns the CRM's complaint ID.
        /// </summary>
        Task<SubmitComplaintResponseDto> SubmitAsync(
            SubmitComplaintRequestDto request,
            CancellationToken ct);
    }
}
