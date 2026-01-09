using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.UI;

using InSite.Api.Settings;
using InSite.Application.Banks.Read;
using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [DisplayName("Booking")]
    public partial class RegistrationsController : ApiBaseController
    {
        /// <summary>
        /// Count registrations
        /// </summary>
        /// <remarks>
        /// Count the number of event registration attendance records that match your search criteria. 
        /// 
        /// Please note the date and time parameter assumes the <see href="https://en.wikipedia.org/wiki/ISO_8601">ISO 8601</see> 
        /// standard format. For example, "2002-09-01T23:10:50+00:00" means "Sep 1, 2002 at 11:10:50 PM UTC"
        /// </remarks>
        [Route("api/registrations/count")]
        [HttpPost]
        public HttpResponseMessage Count(AttendanceCriteria criteria)
        {
            try
            {
                var filter = GetFilter(criteria);
                var count = ServiceLocator.RegistrationSearch.CountAttendances(filter);
                return JsonSuccess(count);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Search registrations
        /// </summary>
        /// <remarks>
        /// Find registration attendance information that matches your search criteria.
        /// </remarks>
        [Route("api/registrations/search")]
        [HttpPost]
        public HttpResponseMessage Search(AttendanceCriteria criteria)
        {
            try
            {
                var filter = GetFilter(criteria);

                var result = ServiceLocator.RegistrationSearch
                    .GetAttendances(filter)
                    .Select(x => new
                    {
                        x.EventIdentifier,
                        x.EventNumber,
                        x.EventFormat,
                        x.EventScheduledStart,

                        x.AssessmentFormCode,
                        x.AssessmentFormIdentifier,
                        x.AssessmentFormName,
                        x.AssessmentFormTitle,

                        x.LearnerIdentifier,
                        x.LearnerCode,
                        x.LearnerName,
                        x.LearnerEmail,

                        x.AttendanceStatus,

                        x.LastChangeTime,
                        x.LastChangeType
                    })
                    .ToList();

                return JsonSuccess(result);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Delete a registration
        /// </summary>
        /// <remarks>
        /// The registration identifier must be provided as part of the request URL.
        /// </remarks>
        [Route("api/registrations/{registration}")]
        [HttpDelete]
        public HttpResponseMessage DeleteRegistration([FromUri] Guid registration)
        {
            try
            {
                var organization = GetOrganization().Identifier;

                var reg = FindRegistration(registration);
                if (reg != null)
                {
                    if (reg.Event.OrganizationIdentifier != organization)
                        return JsonError($"Registration Not Found: {registration}", HttpStatusCode.NotFound);
                    else
                        SendCommand(new DeleteRegistration(registration, false));
                }

                return JsonSuccess(registration, HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Retrieve a registration
        /// </summary>
        [Route("api/registrations/{registration}")]
        [HttpGet]
        public HttpResponseMessage GetRegistration([FromUri] Guid registration)
        {
            try
            {
                var organization = GetOrganization().Identifier;

                var registrationEntity = FindRegistration(registration);
                if (registrationEntity == null || registrationEntity.Event.OrganizationIdentifier != organization)
                    return JsonError($"Registration Not Found: {registration}", HttpStatusCode.NotFound);

                var model = new ApiRegistrationModel
                {
                    RegistrationIdentifier = registration,
                    RegistrationAttendanceStatus = registrationEntity.AttendanceStatus,

                    LastChangeTime = registrationEntity.LastChangeTime.Value,
                    LastChangeType = registrationEntity.LastChangeType,

                    EventIdentifier = registrationEntity.EventIdentifier,
                    EventNumber = registrationEntity.Event.EventNumber,
                    EventFormat = registrationEntity.Event.EventFormat,
                    EventStart = registrationEntity.Event.EventScheduledStart,
                    EventExamType = registrationEntity.Event.ExamType,
                    EventVenue = registrationEntity.Event.VenueLocationIdentifier
                };

                if (registrationEntity.Candidate != null)
                {
                    model.LearnerIdentifier = registrationEntity.Candidate.UserIdentifier;
                    model.LearnerCode = registrationEntity.Candidate.PersonCode;
                    model.LearnerName = registrationEntity.Candidate.UserFullName;
                    model.LearnerEmail = registrationEntity.Candidate.UserEmail;
                }

                if (registrationEntity.Form != null)
                {
                    model.AssessmentFormIdentifier = registrationEntity.Form.FormIdentifier;
                    model.AssessmentFormCode = registrationEntity.Form.FormCode;
                    model.AssessmentFormName = registrationEntity.Form.FormName;
                    model.AssessmentFormTitle = registrationEntity.Form.FormTitle;
                }

                return JsonSuccess(model, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Create a registration
        /// </summary>
        /// <remarks>
        /// Add a new registration based on a lookup of the event using its venue and start date/time rather than its identifier, and based on a 
        /// lookup of the assessment form based on its code rather than its identifier. 
        /// 
        /// This method for adding a new registration is lax (as opposed to strict) in that it does not specify a unique identifier for the event or 
        /// the assessment. Instead, it allows the caller to assume a matching event may or may not exist.If no matching event is found then a new 
        /// event is created automatically. Similarly, it allows the caller to assume at least one published, non-bilingual assessment form exists 
        /// with a matching assessment form code. If no matching assessment is found, then it returns 400 Bad Request with an error message to 
        /// indicate the failed match. 
        /// 
        /// The registration ID is provided as part of the request URL. 
        /// 
        /// The following parameters must be provided in the request body:
        /// - **EventVenue**: The venue ID.
        /// - **EventStart**: The date when the event starts.
        /// - **EventExamType**: The exam type, it is required if the exam does not exist and a new one should be created.
        /// - **EventExamFormat**: The exam format, it is required if the exam does not exist and a new one should be created
        /// - **EventBillingCode**: the exam billing code, it is required if the exam does not exist and a new one should be created.
        /// - **Learner**: ID of the learner that will be registered.
        /// - **Assessment**: Assessment form code identifying the assessment to be assigned to the registration
        /// - **Accommodations**: (Optional) An array of accommodations for the new registration
        /// 
        /// The date and time parameter uses ISO 8601 standard to specify format. 
        /// For example "2002-09-01T23:10:50+00:00" means "Sep 1, 2002 at 11:10:50 PM UTC"
        /// 
        /// **Potential Error Messages:**
        /// 
        /// Here are some of the errors that can be returned by this method when there is a problem with your request:
        /// 
        /// - Accommodation type cannot be empty.
        /// - Learner not found.
        /// - Assessment form not found.
        /// - Registration already exists. You cannot add a new registration with the same identifier.**
        /// - EventExamType not specified.
        /// - Invalid EventExamType value.
        /// - EventBillingCode not specified.
        /// - Invalid EventBillingCode value.
        /// - EventExamFormat not specified.
        /// - Invalid EventExamFormat value.
        /// - Invalid EventStartTime value.
        /// - Venue not found.
        /// </remarks>
        [Route("api/registrations/{registration}/commands/register-lax")]
        [HttpPost]
        public HttpResponseMessage PostRegistration([FromUri] Guid registration, AddRegistrationLax add)
        {
            try
            {
                var organization = GetOrganization().Identifier;

                var isAccommodated = add.Accommodations != null && add.Accommodations.Any();
                if (isAccommodated)
                {
                    foreach (var accommodation in add.Accommodations)
                    {
                        if (string.IsNullOrEmpty(accommodation.Type))
                            return JsonError($"Accommodation type cannot be empty.", HttpStatusCode.BadRequest);
                    }
                }

                if (add.EventExamType.IsNotEmpty() && StringHelper.StartsWith(add.EventExamType, "Individual"))
                    add.EventExamType += isAccommodated ? " (A)" : " (N)";

                var learner = FindLearner(organization, add.Learner);
                if (learner == null)
                    return JsonError($"Learner Not Found: {add.Learner}", HttpStatusCode.BadRequest);

                var assessment = FindAssessment(organization, add.Assessment);
                if (assessment == null)
                    return JsonError($"Assessment Form Not Found: {add.Assessment}", HttpStatusCode.BadRequest);

                var registrationEntity = FindRegistration(registration);
                if (registrationEntity != null)
                    return JsonError($"Registration {registration} already exists. {Describe(registrationEntity)} You cannot add a new registration with the same identifier.", HttpStatusCode.BadRequest);

                var (exam, error, created) = FindOrCreateExam(organization, add);
                if (exam == null)
                    return JsonError(error, HttpStatusCode.BadRequest);

                AddAssessmentFormToEvent(exam.Value, assessment.FormIdentifier);

                SendCommand(new RequestRegistration(registration, organization, exam.Value, learner.UserIdentifier, null, null, null, null, null));
                SendCommand(new AssignExamForm(registration, assessment.FormIdentifier, null));
                SendCommand(new ChangeEligibility(registration, "Eligible", "API", null));
                SendCommand(new ChangeApproval(registration, "Eligible", "Eligible", null, null));

                if (!string.IsNullOrEmpty(add.LearnerRegistrantType))
                    SendCommand(new ChangeCandidateType(registration, add.LearnerRegistrantType));

                if (isAccommodated)
                    foreach (var accommodation in add.Accommodations)
                        SendCommand(new GrantAccommodation(registration, accommodation.Type, accommodation.Name, accommodation.TimeExtension));

                SendCommand(new LimitExamTime(registration));

                LogResponse(registration, exam, created);

                return JsonSuccess(new { Exam = exam.Value, registration }, created ? HttpStatusCode.Created : HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Transfer a registration
        /// </summary>
        /// <remarks>
        /// Transfer an existing registration from one event to another event based on a lookup of the event using its venue and start date/time 
        /// rather than its identifier. This method for transferring a registration is lax(as opposed to strict) in that it does not specify a unique 
        /// identifier for the event. Instead, it allows the caller to assume a matching event may or may not exist.If no match is found then a new 
        /// event is created automatically. Note the assessment form can NOT be modified in a transfer. The registration ID is provided as part of 
        /// the request URL.The following parameters must be provided in the request body:
        /// <para>EventVenue. The venue ID.</para>
        /// <para>EventStart. The date when the event starts.</para>
        /// <para>EventExamType.The exam type, it is required if the exam does not exist and a new one should be created.</para>
        /// <para>Learner.ID of the learner that will be registered.</para>
        /// <para>Reason.The reason for the transfer.</para>
        /// The date and time parameter uses ISO 8601 standard to specify format. 
        /// For example "2002-09-01T23:10:50+00:00" means "Sep 1, 2002 at 11:10:50 PM UTC"
        /// 
        /// **Potential Error Messages:**
        /// 
        /// Here are some of the errors that can be returned by this method when there is a problem with your request:
        /// 
        /// - Registration not found.
        /// - Registration has no assessment form.
        /// - Learner does not belong to registration.
        /// - Registration already assigned to event.
        /// - Learner not found.
        /// </remarks>
        [Route("api/registrations/{registration}/commands/transfer-lax")]
        [HttpPost]
        public HttpResponseMessage PostRegistration([FromUri] Guid registration, TransferRegistrationLax transfer)
        {
            try
            {
                var organization = GetOrganization().Identifier;

                var isAccommodated = transfer.Accommodations != null && transfer.Accommodations.Any();

                var registrationEntity = FindRegistration(registration);
                if (registrationEntity == null || registrationEntity.Event.OrganizationIdentifier != organization)
                    return JsonError($"Registration Not Found: {registration}", HttpStatusCode.BadRequest);

                if (registrationEntity.ExamFormIdentifier == null)
                    return JsonError($"Registration Has No Assessment Form: {registration}", HttpStatusCode.BadRequest);

                if (registrationEntity.CandidateIdentifier != transfer.Learner)
                    return JsonError($"Learner Does Not Belong to Registration: {transfer.Learner}", HttpStatusCode.BadRequest);

                var add = new AddRegistrationLax
                {
                    EventStart = transfer.EventStart,
                    EventVenue = transfer.EventVenue,
                    EventExamType = registrationEntity.Event.ExamType,
                    EventExamFormat = registrationEntity.Event.EventFormat,
                    EventBillingCode = registrationEntity.Event.EventBillingType,
                    Learner = transfer.Learner,
                    Assessment = registrationEntity.Form.FormCode
                };

                // If an exam type is provided in the transfer request, then use it to create a new event (instead of
                // using the exam type from the existing event).

                if (!string.IsNullOrEmpty(transfer.EventExamType))
                    add.EventExamType = transfer.EventExamType;

                var (exam, error, created) = FindOrCreateExam(organization, add);
                if (exam == null)
                    return JsonError(error, HttpStatusCode.BadRequest);

                if (exam == registrationEntity.EventIdentifier)
                    return JsonError($"Registration Already Assigned to Event {exam}", HttpStatusCode.BadRequest);

                var learner = FindLearner(organization, transfer.Learner);
                if (learner == null)
                    return JsonError($"Learner Not Found: {transfer.Learner}", HttpStatusCode.BadRequest);

                if (registrationEntity.ExamFormIdentifier.HasValue)
                    AddAssessmentFormToEvent(exam.Value, registrationEntity.ExamFormIdentifier.Value);

                var cancelEmptyEvent = registrationEntity.Event?.EventType == "Exam";

                SendCommand(new ChangeEvent(registration, exam.Value, transfer.Reason, cancelEmptyEvent));

                if (isAccommodated)
                    foreach (var accommodation in transfer.Accommodations)
                        SendCommand(new GrantAccommodation(registration, accommodation.Type, accommodation.Name, accommodation.TimeExtension));

                LogResponse(registration, exam, created);

                return JsonSuccess(new { Exam = exam, registration }, created ? HttpStatusCode.Created : HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Cancel a registration
        /// </summary>
        /// <remarks>
        /// The registration ID is provided as part of the request URL. The following parameters must be provided in the request body:
        /// <para>Reason - Short description of the reason for the cancellation.</para>
        /// 
        /// **Potential Error Messages:**
        /// 
        /// Here are some of the errors that can be returned by this method when there is a problem with your request:
        /// 
        /// - Registration not found.
        /// </remarks>
        [Route("api/registrations/{registration}/commands/cancel")]
        [HttpPost]
        public HttpResponseMessage PostRegistration([FromUri] Guid registration, Shift.Sdk.UI.CancelRegistration cancel)
        {
            try
            {
                var organization = GetOrganization().Identifier;

                var registrationEntity = FindRegistration(registration);
                if (registrationEntity == null || registrationEntity.Event.OrganizationIdentifier != organization)
                    return JsonError($"Registration Not Found: {registration}", HttpStatusCode.BadRequest);

                var cancelEmptyEvent = registrationEntity.Event?.EventType == "Exam"
                    && cancel.Reason == "Cancel request";

                if (organization == OrganizationIdentifiers.SkilledTradesBC)
                    SendCommand(new Application.Registrations.Write.DeleteRegistration(
                        registration, cancelEmptyEvent));
                else
                    SendCommand(new Application.Registrations.Write.CancelRegistration(
                        registration, cancel.Reason, cancelEmptyEvent));

                return JsonSuccess("OK");
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Assign an assessment to a registration
        /// </summary>
        /// <remarks>
        /// Change the assessment form assigned to an existing event registration. If the registration does not exist, then return a 404 Not Found 
        /// error. Allow the caller to assume at least one published, non-bilingual match for the assessment form code in the request.If no matching 
        /// assessment is found, then return 400 Bad Request with an error message to indicate the failed match. The registration ID is provided as 
        /// part of the request URL. The following parameters must be provided in the request body:
        /// <para>Assessment - Form code identifying the assessment to be assigned to the registration.</para>
        /// 
        /// **Potential Error Messages:**
        /// 
        /// Here are some of the errors that can be returned by this method when there is a problem with your request:
        /// 
        /// - Registration not found.
        /// - Assessment form not found.
        /// - Assessment already assigned to registration.
        /// </remarks>
        [Route("api/registrations/{registration}/commands/change-assessment")]
        [HttpPost]
        public HttpResponseMessage PostRegistration([FromUri] Guid registration, ChangeAssessment change)
        {
            try
            {
                var organization = GetOrganization().Identifier;

                var registrationEntity = FindRegistration(registration);
                if (registrationEntity == null || registrationEntity.Event.OrganizationIdentifier != organization)
                    return JsonError($"Registration Not Found: {registration}", HttpStatusCode.BadRequest);

                var assessment = FindAssessment(organization, change.Assessment);
                if (assessment == null)
                    return JsonError($"Assessment Form Not Found: {change.Assessment}", HttpStatusCode.BadRequest);

                if (assessment.FormIdentifier == registrationEntity.ExamFormIdentifier)
                    return JsonError($"Assessment {change.Assessment} Already Assigned to Registration {registration}", HttpStatusCode.BadRequest);

                SendCommand(new AssignExamForm(registration, assessment.FormIdentifier, null));
                SendCommand(new LimitExamTime(registration));
                SendCommand(new ChangeEligibility(registration, "Eligible", "API", null));
                SendCommand(new ChangeApproval(registration, "Eligible", "Eligible", null, null));

                return JsonSuccess(new { form = assessment.FormIdentifier, registration }, HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Add an accommodation to a registration
        /// </summary>
        /// <remarks>
        /// Grant an accommodation to an existing event registration. If the registration does not exist, then return a 404 Not Found error. The 
        /// registration ID is provided as part of the request URL. The following parameters must be provided in the request body:
        /// <para>Type - Uniquly identifies the type of accommodation</para>
        /// <para>Name - (optional) Short clarifying description</para>
        /// <para>TimeExtension - (optional) Time extension in minutes</para>
        /// 
        /// **Potential Error Messages:**
        /// 
        /// Here are some of the errors that can be returned by this method when there is a problem with your request:
        /// 
        /// - Registration not found.
        /// </remarks>
        [Route("api/registrations/{registration}/commands/grant-accommodation")]
        [HttpPost]
        public HttpResponseMessage PostRegistration([FromUri] Guid registration, GrantRegistrationAccommodation grant)
        {
            try
            {
                var organization = GetOrganization().Identifier;

                var registrationEntity = FindRegistration(registration);
                if (registrationEntity == null || registrationEntity.Event.OrganizationIdentifier != organization)
                    return JsonError($"Registration Not Found: {registration}", HttpStatusCode.BadRequest);

                SendCommand(new GrantAccommodation(registration, grant.Type, grant.Name, grant.TimeExtension));

                return JsonSuccess(registration, HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Remove an accommodation from a registration
        /// </summary>
        /// <remarks>
        /// Revoke an existing accommodation from an existing event registration. If the registration does not exist, then return a 404 Not Found 
        /// error. The registration ID is provided as part of the request URL. The request body MUST include the Type parameter, which identifies the
        /// type of accommodation.
        /// 
        /// **Potential Error Messages:**
        /// 
        /// Here are some of the errors that can be returned by this method when there is a problem with your request:
        /// 
        /// - Registration not found.
        /// </remarks>
        [Route("api/registrations/{registration}/commands/revoke-accommodation")]
        [HttpPost]
        public HttpResponseMessage PostRegistration([FromUri] Guid registration, RevokeRegistrationAccommodation revoke)
        {
            try
            {
                var organization = GetOrganization().Identifier;

                var registrationEntity = FindRegistration(registration);
                if (registrationEntity == null || registrationEntity.Event.OrganizationIdentifier != organization)
                    return JsonError($"Registration Not Found: {registration}", HttpStatusCode.BadRequest);

                SendCommand(new RevokeAccommodation(registration, revoke.Type));

                return JsonSuccess(registration, HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        #region Helpers

        private VAttendanceFilter GetFilter(AttendanceCriteria criteria)
        {
            return new VAttendanceFilter
            {
                OrganizationIdentifier = GetOrganization().Identifier,
                LearnerName = criteria?.Name,
                LearnerEmail = criteria?.Email,
                LearnerCode = criteria?.Code,
                LastChangeTimeSince = criteria?.LastChangeTimeSince,
                LastChangeTimeBefore = criteria?.LastChangeTimeBefore
            };
        }

        private string Describe(QRegistration reg)
        {
            var description = new StringBuilder();

            if (reg.Event != null)
                description.Append($"Event {reg.Event.EventIdentifier} is scheduled to start {reg.Event.EventScheduledStart} in venue {reg.Event.VenueLocationIdentifier}. ");

            if (reg.Candidate != null)
                description.Append($"Learner {reg.CandidateIdentifier} is {reg.Candidate.UserFullName} ({reg.Candidate.PersonCode}). ");

            return description.ToString();
        }

        private void AddAssessmentFormToEvent(Guid @event, Guid form)
        {
            var e = ServiceLocator.EventSearch.GetEvent(@event, x => x.ExamForms, x => x.VenueOffice, x => x.VenueLocation);
            if (e.ExamForms != null && !e.ExamForms.Any(f => f.FormIdentifier == form))
            {
                SendCommand(new AddEventAssessment(@event, form));

                var title = EventHelper.GetTitle(@event, e.VenueLocationName, ServiceLocator.EventSearch);
                SendCommand(new RetitleEvent(@event, title));
            }
        }

        private (Guid?, string, bool) FindOrCreateExam(Guid organization, AddRegistrationLax add)
        {
            if (StringHelper.StartsWith(add.EventExamType, "Individual"))
                return CreateExam(organization, UniqueIdentifier.Create(), add.EventStart, add.EventExamType, add.EventBillingCode, add.EventExamFormat, add.EventVenue);

            var events = FindEvents(organization, add.EventStart, add.EventVenue, true).OrderBy(x => x.EventNumber).ToList();
            if (events.Count > 0)
            {
                var @event = events.Where(x => x.EventSource == "Shift API").LastOrDefault() ?? events.Last();
                return (@event.EventIdentifier, null, false);
            }

            return CreateExam(organization, UniqueIdentifier.Create(), add.EventStart, add.EventExamType, add.EventBillingCode, add.EventExamFormat, add.EventVenue);
        }

        internal static (Guid? exam, string error, bool created) CreateExam(Guid organization, Guid eventId, DateTimeOffset eventStart, string examType, string billingCode, string examFormat, Guid eventVenue)
        {
            if (string.IsNullOrWhiteSpace(examType))
                return (null, "EventExamType Not Specified", false);

            if (!EventExamType.ContainsValue(examType))
                return (null, $"Invalid EventExamType Value: {examType}", false);

            if (string.IsNullOrWhiteSpace(billingCode))
                return (null, "EventBillingCode Not Specified", false);

            var billingCodeProvider = new ItemNameComboBoxSettings(string.Empty, new StateBag());
            billingCodeProvider.CollectionName = CollectionName.Activities_Exams_Billing_Type;
            billingCodeProvider.OrganizationIdentifier = organization;

            var billingCodesItems = billingCodeProvider.CreateDataSource();
            if (!billingCodesItems.ContainsValue(billingCode))
                return (null, $"Invalid EventBillingCode Value: {billingCode}", false);

            if (string.IsNullOrWhiteSpace(examFormat))
                return (null, "EventExamFormat Not Specified", false);

            if (!EventExamFormat.ContainsValue(examFormat))
                return (null, $"Invalid EventExamFormat Value: {examFormat}", false);

            if (eventStart < DateTimeOffset.Now)
                return (null, "Invalid EventStartTime Value: the value must be a future date and time", false);

            var venue = ServiceLocator.GroupSearch.GetGroup(eventVenue);
            if (venue == null)
                return (null, $"Venue Not Found: {eventVenue}", false);

            var venueAddress = ServiceLocator.GroupSearch.GetAddress(venue.GroupIdentifier, AddressType.Physical);
            if (venueAddress != null)
            {
                TimeZoneInfo venueTimeZone = null;

                if (venueAddress.PostalCode.IsNotEmpty())
                {
                    venueTimeZone = venueAddress.Country.IsNotEmpty()
                        ? TimeZones.FindByPostalCode(venueAddress.PostalCode, venueAddress.Country)
                        : TimeZones.FindByPostalCode(venueAddress.PostalCode);
                }

                if (venueTimeZone == null && venueAddress.Province.IsNotEmpty())
                {
                    venueTimeZone = venueAddress.Country.IsNotEmpty()
                        ? TimeZones.FindByProvince(venueAddress.Province, venueAddress.Country)
                        : TimeZones.FindByProvince(venueAddress.Province);
                }

                if (venueTimeZone != null)
                    eventStart = TimeZoneInfo.ConvertTime(eventStart, venueTimeZone);
            }

            var venueIdentifier = venue.GroupIdentifier;
            var eventTitle = EventHelper.GetTitle(Guid.Empty, venue.GroupName, ServiceLocator.EventSearch);

            var number = Sequence.Increment(organization, SequenceType.Event);

            var create = new ScheduleExam(eventId, organization)
            {
                ExamType = examType,
                ExamDuration = EventHelper.GetDuration(billingCode),

                EventStartTime = eventStart,
                EventFormat = examFormat,
                EventNumber = number,
                EventBillingCode = billingCode,

                CapacityMaximum = 1,

                VenueIdentifier = venueIdentifier,
                EventTitle = eventTitle,
                EventSource = "Shift API"
            };

            SendCommand(create);
            SendCommand(new ChangeEventStatus(eventId, null, "Approved"));
            SendCommand(new StartEventPublication(eventId));

            return (eventId, null, true);
        }

        private QRegistration FindRegistration(Guid registrationId)
        {
            return ServiceLocator.RegistrationSearch.GetRegistration(registrationId, x => x.Event, x => x.Candidate, x => x.Form);
        }

        private List<QEvent> FindEvents(Guid organization, DateTimeOffset start, Guid venue, bool excludeCancelled)
        {
            var filter = new QEventFilter
            {
                OrganizationIdentifier = organization,
                EventScheduledSince = start,
                EventScheduledBefore = start.AddMinutes(1),
                VenueLocationIdentifier = new[] { venue }
            };

            if (excludeCancelled)
                filter.ExcludeEventSchedulingStatus = "Cancelled";

            return ServiceLocator.EventSearch.GetEvents(filter);
        }

        private QUser FindLearner(Guid organizationId, Guid learnerId)
        {
            return ServiceLocator.PersonSearch.GetPerson(learnerId, organizationId, x => x.User)?.User;
        }

        private QBankForm FindAssessment(Guid organization, string formCode)
        {
            var filter = new QBankFormFilter
            {
                OrganizationIdentifier = organization,
                FormCode = formCode,
                IncludeFormStatus = "Published"
            };

            var form = ServiceLocator.BankSearch
                .GetForms(filter)
                .Where(x => !StringHelper.EndsWithAny(x.FormTitle, new[] { "Bilingual", "(Bilingual)" }))
                .OrderByDescending(x => x.FormAsset)
                .FirstOrDefault();

            return form;
        }

        private void LogResponse(Guid registration, Guid? exam, bool created)
        {
            if (!Request.Headers.TryGetValues("X-InSiteApiResponseLogId", out IEnumerable<string> values)
                || values == null
                || values.Count() == 0)
            {
                return;
            }
            var responseLogId = Guid.Parse(values.First());
            LogResponse(responseLogId, new { Exam = exam.Value, registration }, created ? HttpStatusCode.Created : HttpStatusCode.OK);
        }

        private void LogResponse(Guid key, object value, HttpStatusCode httpStatusCode)
        {
            try
            {
                var apiRequest = ApiRequestSearch.SelectByRequestLogKey(key);

                if (apiRequest == null)
                    return;

                if (value == null)
                    return;

                var json = ServiceLocator.Serializer.Serialize(value);

                apiRequest.ResponseCompleted = DateTimeOffset.Now;
                apiRequest.ResponseTime = (int)(apiRequest.ResponseCompleted.Value - apiRequest.RequestStarted).TotalMilliseconds;
                apiRequest.ResponseContentType = "application/json; charset=utf-8";
                apiRequest.ResponseContentData = json;
                apiRequest.ResponseStatusName = httpStatusCode.GetName();
                apiRequest.ResponseStatusNumber = (int)httpStatusCode;

                ApiRequestStore.Update(apiRequest);
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);
            }
        }

        private HttpResponseMessage HandleError(Exception ex)
        {
            // Sending to Sentry is disabled by Aleksey on 2024-10-23
            // https://insite.atlassian.net/browse/DEV-9599
            //
            // AppSentry.SentryError(ex);

            return JsonError(ex.GetAllMessages());
        }

        #endregion
    }
}