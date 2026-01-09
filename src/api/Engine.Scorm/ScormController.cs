using Engine.Common;

using Microsoft.AspNetCore.Mvc;

using Shift.Common;
using Shift.Common.Scorm;

namespace Engine.Scorm
{
    [ApiController]
    public class ScormController(IMonitor monitor)
        : ControllerBase
    {
        private readonly IMonitor _monitor = monitor;

        private ScormApi BuildClient()
        {
            var userName = (string)HttpContext.Items["AuthenticatedUserName"]!;

            var password = (string)HttpContext.Items["AuthenticatedPassword"]!;

            return new ScormApi(userName, password);
        }

        [HttpPost("registrations")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<Problem>(StatusCodes.Status402PaymentRequired)]
        [EndpointName("createRegistration")]
        public IActionResult CreateRegistration(RegistrationRequest request)
        {
            try
            {
                var scorm = BuildClient();

                scorm.CreateRegistration(
                    request.RegistrationId,
                    request.CourseSlug,
                    request.LearnerId,
                    request.LearnerEmail,
                    request.LearnerFirstName,
                    request.LearnerLastName);

                return NoContent();
            }
            catch (Exception ex)
            {
                var message = ex.Message;

                if (message.Contains("already exists under appid"))
                    return NoContent();

                var detail = $"Unable to create SCORM Cloud registration {request.RegistrationId}"
                    + $" for learner {request.LearnerId} ({request.LearnerEmail}). {message}";

                if (message.Contains("maximum number of registrations for this account has been reached"))
                {
                    var instance = _monitor.Information(detail);

                    return ProblemFactory.PaymentRequired(detail, instance).ToActionResult(this);
                }
                else
                {
                    var instance = _monitor.Error(detail);

                    return ProblemFactory.InternalServerError(detail, instance).ToActionResult(this);
                }
            }
        }

        [HttpGet("registrations")]
        [ProducesResponseType<Registration[]>(StatusCodes.Status200OK, "application/json")]
        [EndpointName("getRegistrations")]
        public ActionResult<Registration[]> GetRegistrations([FromQuery] string? course = null, [FromQuery] string? more = null)
        {
            var scorm = BuildClient();
            var result = scorm.GetRegistrations(course, more);
            Response.Headers.Append("X-Engine-Scorm-More", result.More);
            return Ok(result.Registrations);
        }

        [HttpGet("registrations/{registrationId}/last-instance")]
        [ProducesResponseType<int?>(StatusCodes.Status200OK)]
        [EndpointName("getRegistrationInstance")]
        public ActionResult<int?> GetRegistrationInstance(Guid registrationId)
        {
            var scorm = BuildClient();
            var lastInstance = scorm.GetRegistrationInstance(registrationId);
            return Ok(lastInstance);
        }

        [HttpGet("registrations/{registrationId}/launch-url")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        [EndpointName("getRegistrationLaunchUrl")]
        public ActionResult<string> GetRegistrationLaunchUrl(Guid registrationId, string courseSlug, bool preview, string callbackUrl, string exitUrl)
        {
            var scorm = BuildClient();
            var link = scorm.GetRegistrationLaunchUrl(registrationId, courseSlug, preview, callbackUrl, exitUrl);
            return Ok(link);
        }

        [HttpGet("registrations/{registrationId}/progress")]
        [ProducesResponseType<RegistrationProgress>(StatusCodes.Status200OK, "application/json")]
        [EndpointName("getRegistrationInstanceProgress")]
        public ActionResult<RegistrationProgress> GetRegistrationInstanceProgress(Guid registrationId)
        {
            var scorm = BuildClient();
            var instance = scorm.GetRegistrationInstance(registrationId);
            var progress = scorm.GetRegistrationInstanceProgress(registrationId, instance);
            return Ok(progress);
        }

        [HttpGet("courses/{courseSlug}")]
        [ProducesResponseType<Course>(StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EndpointName("getCourse")]
        public ActionResult<Course> GetCourse(string courseSlug)
        {
            var scorm = BuildClient();
            var course = scorm.GetCourse(courseSlug);
            return course != null
                ? Ok(course)
                : NotFound();
        }

        [HttpGet("courses")]
        [ProducesResponseType<Course[]>(StatusCodes.Status200OK, "application/json")]
        [EndpointName("getCourses")]
        public ActionResult<Course[]> GetCourses()
        {
            var scorm = BuildClient();
            var courses = scorm.GetCourses();
            return Ok(courses);
        }

        [HttpGet("courses/{courseSlug}/learners/{learnerId}/registration")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        [EndpointName("getRegistrationId")]
        public ActionResult<string> GetRegistrationId(string courseSlug, Guid learnerId)
        {
            var scorm = BuildClient();
            var registrationId = scorm.GetRegistrationId(courseSlug, learnerId);
            return Ok(registrationId);
        }

        [HttpPost("imports")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        [EndpointName("createImport")]
        public async Task<ActionResult<string>> CreateImportAsync(string courseSlug, bool mayCreateNewVersion, string callbackUrl, string uploadedContentType, string contentMetadata)
        {
            using var stream = new MemoryStream();
            await Request.Body.CopyToAsync(stream);
            var scorm = BuildClient();
            var importId = scorm.CreateImport(courseSlug, mayCreateNewVersion, callbackUrl, uploadedContentType, contentMetadata, stream);
            return Ok(importId);
        }

        [HttpGet("imports/{importSlug}")]
        [ProducesResponseType<CourseImport>(StatusCodes.Status200OK, "application/json")]
        [EndpointName("getImportStatus")]
        public ActionResult<CourseImport> GetImportStatus(string importSlug)
        {
            var scorm = BuildClient();
            var status = scorm.GetImportStatus(importSlug);
            return Ok(status);
        }
    }
}
