using Microsoft.AspNetCore.Mvc;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/academic")]
public class AcademicController : ControllerBase
{
    [HttpGet("/api/v1/curricula")]
    public IActionResult ListCurricula() => StatusCode(501);

    [HttpPost("/api/v1/curricula")]
    public IActionResult CreateCurriculum() => StatusCode(501);

    [HttpGet("/api/v1/curricula/{curriculumId}")]
    public IActionResult GetCurriculum(string curriculumId) => StatusCode(501);

    [HttpPatch("/api/v1/curricula/{curriculumId}")]
    public IActionResult UpdateCurriculum(string curriculumId) => StatusCode(501);

    [HttpPost("/api/v1/curricula/{curriculumId}/publish")]
    public IActionResult PublishCurriculum(string curriculumId) => StatusCode(501);

    [HttpGet("/api/v1/curricula/{curriculumId}/grades")]
    public IActionResult ListGrades(string curriculumId) => StatusCode(501);

    [HttpPost("/api/v1/curricula/{curriculumId}/grades")]
    public IActionResult CreateGrade(string curriculumId) => StatusCode(501);

    [HttpPatch("/api/v1/grades/{gradeId}")]
    public IActionResult UpdateGrade(string gradeId) => StatusCode(501);

    [HttpGet("/api/v1/grades/{gradeId}/subjects")]
    public IActionResult GradeSubjects(string gradeId) => StatusCode(501);

    [HttpPost("/api/v1/grades/{gradeId}/subjects")]
    public IActionResult AttachSubject(string gradeId) => StatusCode(501);

    [HttpDelete("/api/v1/grades/{gradeId}/subjects/{subjectId}")]
    public IActionResult DetachSubject(string gradeId, string subjectId) => StatusCode(501);

    [HttpGet("/api/v1/subjects")]
    public IActionResult ListSubjects() => StatusCode(501);

    [HttpPost("/api/v1/subjects")]
    public IActionResult CreateSubject() => StatusCode(501);

    [HttpGet("/api/v1/subjects/{subjectId}")]
    public IActionResult GetSubject(string subjectId) => StatusCode(501);

    [HttpPatch("/api/v1/subjects/{subjectId}")]
    public IActionResult UpdateSubject(string subjectId) => StatusCode(501);

    [HttpGet("/api/v1/subjects/{subjectId}/topics")]
    public IActionResult ListTopics(string subjectId) => StatusCode(501);

    [HttpPost("/api/v1/subjects/{subjectId}/topics")]
    public IActionResult CreateTopic(string subjectId) => StatusCode(501);

    [HttpGet("/api/v1/topics/{topicId}")]
    public IActionResult GetTopic(string topicId) => StatusCode(501);

    [HttpPatch("/api/v1/topics/{topicId}")]
    public IActionResult UpdateTopic(string topicId) => StatusCode(501);

    [HttpPost("/api/v1/topics/{topicId}/children")]
    public IActionResult CreateSubtopic(string topicId) => StatusCode(501);

    [HttpGet("/api/v1/topics/{topicId}/resources")]
    public IActionResult MappedResources(string topicId) => StatusCode(501);

    [HttpGet("/api/v1/academic/catalog")]
    public IActionResult FullCatalog() => StatusCode(501);
}
