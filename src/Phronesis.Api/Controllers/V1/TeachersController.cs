using Microsoft.AspNetCore.Mvc;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/teachers")]
public class TeachersController : ControllerBase
{
    [HttpGet]
    public IActionResult ListTeachers() => StatusCode(501);

    [HttpPost]
    public IActionResult CreateTeacher() => StatusCode(501);

    [HttpGet("{teacherId}")]
    public IActionResult GetTeacher(string teacherId) => StatusCode(501);

    [HttpPatch("{teacherId}")]
    public IActionResult UpdateProfile(string teacherId) => StatusCode(501);

    [HttpGet("{teacherId}/qualifications")]
    public IActionResult GetQualifications(string teacherId) => StatusCode(501);

    [HttpPost("{teacherId}/qualifications")]
    public IActionResult AddQualification(string teacherId) => StatusCode(501);

    [HttpPatch("{teacherId}/qualifications/{qualificationId}")]
    public IActionResult UpdateQualification(string teacherId, string qualificationId) => StatusCode(501);

    [HttpDelete("{teacherId}/qualifications/{qualificationId}")]
    public IActionResult RemoveQualification(string teacherId, string qualificationId) => StatusCode(501);

    [HttpGet("{teacherId}/subjects")]
    public IActionResult GetSubjects(string teacherId) => StatusCode(501);

    [HttpPut("{teacherId}/subjects")]
    public IActionResult SetSubjects(string teacherId) => StatusCode(501);

    [HttpGet("{teacherId}/availability")]
    public IActionResult GetAvailability(string teacherId) => StatusCode(501);

    [HttpPut("{teacherId}/availability")]
    public IActionResult SetAvailability(string teacherId) => StatusCode(501);

    [HttpGet("/api/v1/me/teacher-profile")]
    public IActionResult GetOwnProfile() => StatusCode(501);

    [HttpPost("/api/v1/teacher-applications")]
    public IActionResult CreateApplication() => StatusCode(501);

    [HttpGet("/api/v1/teacher-applications/{applicationId}")]
    public IActionResult GetApplicationStatus(string applicationId) => StatusCode(501);

    [HttpPatch("/api/v1/teacher-applications/{applicationId}")]
    public IActionResult EditApplication(string applicationId) => StatusCode(501);

    [HttpPost("/api/v1/teacher-applications/{applicationId}/submit")]
    public IActionResult SubmitApplication(string applicationId) => StatusCode(501);

    [HttpPost("/api/v1/teacher-applications/{applicationId}/documents")]
    public IActionResult CreateDocumentUpload(string applicationId) => StatusCode(501);

    [HttpGet("/api/v1/teacher-applications/{applicationId}/documents")]
    public IActionResult ListDocuments(string applicationId) => StatusCode(501);

    [HttpDelete("/api/v1/teacher-applications/{applicationId}/documents/{documentId}")]
    public IActionResult RemoveDocument(string applicationId, string documentId) => StatusCode(501);
}
