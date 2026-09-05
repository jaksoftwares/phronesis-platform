using Microsoft.AspNetCore.Mvc;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/learners")]
public class LearnersController : ControllerBase
{
    [HttpGet]
    public IActionResult ListLearners() => StatusCode(501);

    [HttpPost]
    public IActionResult CreateLearner() => StatusCode(501);

    [HttpGet("{learnerId}")]
    public IActionResult GetLearner(string learnerId) => StatusCode(501);

    [HttpPatch("{learnerId}")]
    public IActionResult UpdateProfile(string learnerId) => StatusCode(501);

    [HttpPost("{learnerId}/activate")]
    public IActionResult ActivateLearner(string learnerId) => StatusCode(501);

    [HttpPost("{learnerId}/suspend")]
    public IActionResult SuspendLearner(string learnerId) => StatusCode(501);

    [HttpGet("{learnerId}/guardians")]
    public IActionResult ListGuardians(string learnerId) => StatusCode(501);

    [HttpPost("{learnerId}/guardians")]
    public IActionResult AddGuardian(string learnerId) => StatusCode(501);

    [HttpPatch("{learnerId}/guardians/{relationshipId}")]
    public IActionResult UpdateGuardian(string learnerId, string relationshipId) => StatusCode(501);

    [HttpDelete("{learnerId}/guardians/{relationshipId}")]
    public IActionResult RemoveGuardian(string learnerId, string relationshipId) => StatusCode(501);

    [HttpGet("/api/v1/me/learner-profile")]
    public IActionResult GetOwnProfile() => StatusCode(501);

    [HttpPatch("/api/v1/me/learner-profile")]
    public IActionResult UpdateOwnProfile() => StatusCode(501);
}
