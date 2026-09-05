using Microsoft.AspNetCore.Mvc;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1")]
public class ReportsController : ControllerBase
{
    [HttpGet("reports/learners")]
    public IActionResult LearnerReport() => StatusCode(501);

    [HttpGet("reports/teachers")]
    public IActionResult TeacherReport() => StatusCode(501);

    [HttpGet("reports/content")]
    public IActionResult ContentReport() => StatusCode(501);

    [HttpGet("reports/assessments")]
    public IActionResult AssessmentReport() => StatusCode(501);

    [HttpGet("reports/subscriptions")]
    public IActionResult SubscriptionReport() => StatusCode(501);

    [HttpGet("reports/payments")]
    public IActionResult RevenuePaymentReport() => StatusCode(501);

    [HttpGet("reports/classes")]
    public IActionResult ClassReport() => StatusCode(501);

    [HttpGet("reports/attendance")]
    public IActionResult AttendanceReport() => StatusCode(501);

    [HttpGet("reports/learning-progress")]
    public IActionResult ProgressReport() => StatusCode(501);

    [HttpPost("reports/export")]
    public IActionResult StartExportJob() => StatusCode(501);

    [HttpGet("reports/exports/{jobId}")]
    public IActionResult ExportStatus(string jobId) => StatusCode(501);

    [HttpGet("audit-logs")]
    public IActionResult SearchAuditLogs() => StatusCode(501);

    [HttpGet("audit-logs/{auditId}")]
    public IActionResult GetAuditEvent(string auditId) => StatusCode(501);

    [HttpGet("resources/{resourceId}/audit")]
    public IActionResult ContentAudit(string resourceId) => StatusCode(501);

    [HttpGet("teachers/{teacherId}/audit")]
    public IActionResult TeacherAudit(string teacherId) => StatusCode(501);

    [HttpGet("payments/{paymentId}/audit")]
    public IActionResult PaymentAudit(string paymentId) => StatusCode(501);

    [HttpPost("me/data-export")]
    public IActionResult RequestDataExport() => StatusCode(501);

    [HttpGet("me/data-export/{jobId}")]
    public IActionResult ExportStatusJob(string jobId) => StatusCode(501);

    [HttpPost("me/deletion-request")]
    public IActionResult RequestDeletion() => StatusCode(501);

    [HttpGet("me/deletion-request")]
    public IActionResult DeletionStatus() => StatusCode(501);
}
