using crud_app_backend.DTOs;
using crud_app_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace crud_app_backend.Controllers
{
    /// <summary>
    /// Called by n8n when the staff member confirms Y on the complaint screen.
    /// This controller fetches the stored voice/image files, assembles the
    /// full payload, and forwards everything to the CRM API.
    /// Lives at /api/whatsapp/complaints
    /// </summary>
    [ApiController]
    [Route("api/whatsapp/complaints")]
    [Produces("application/json")]
    public class WhatsAppComplaintController : ControllerBase
    {
        private readonly IWhatsAppComplaintService _service;
        private readonly ILogger<WhatsAppComplaintController> _logger;

        public WhatsAppComplaintController(
            IWhatsAppComplaintService service,
            ILogger<WhatsAppComplaintController> logger)
        {
            _service = service;
            _logger  = logger;
        }

        // ─────────────────────────────────────────────────────────────────────
        // POST /api/whatsapp/complaints/submit
        //
        // n8n sends:
        // {
        //   "whatsapp_phone":   "8801704134097",
        //   "staff_id":         "359778",
        //   "name":             "Sheikh Shariar Newaz",
        //   "official_phone":   "01704137508",
        //   "designation":      "Manager",
        //   "dept":             "Quality Control",
        //   "groupname":        "PRAN GROUP",
        //   "company":          "CS PRAN",
        //   "locationname":     "Habiganj Industrial Park",
        //   "email":            "ehs3@hip.prangroup.com",
        //   "description":      "Product was damaged on delivery",
        //   "voice_message_ids":["wamid.abc123", "wamid.xyz456"],
        //   "image_message_ids":["wamid.img001"]
        // }
        //
        // Returns: { "complaint_id": "PR12345", "success": true }
        // ─────────────────────────────────────────────────────────────────────

        [HttpPost("submit")]
        [ProducesResponseType(typeof(SubmitComplaintResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Submit(
            [FromBody] SubmitComplaintRequestDto dto,
            CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.SubmitAsync(dto, ct);

                if (!result.Success)
                {
                    _logger.LogWarning(
                        "[Complaint] Submit failed for staff={StaffId}: {Msg}",
                        dto.StaffId, result.Message);

                    // Still return 200 so n8n can show the user a meaningful error
                    // rather than a generic HTTP error node failure
                    return Ok(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[Complaint] Submit crashed — staff={StaffId} phone={Phone}",
                    dto.StaffId, dto.WhatsappPhone);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponseDto<object>.Fail("Failed to submit complaint: " + ex.Message));
            }
        }
    }
}
