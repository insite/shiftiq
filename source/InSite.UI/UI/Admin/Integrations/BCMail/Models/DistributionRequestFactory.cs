using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Banks.Read;
using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
using InSite.Persistence.Integration.BCMail;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.UI.Desktops.Custom.SkilledTradesBC.Distributions.Models
{
    public class DistributionRequestFactory
    {
        public List<DistributionRequest> CreateDistributionRequests(IEventSearch eventSearch, IRegistrationSearch attemptSearch, IBankSearch assessmentSearch, QEventFilter filter)
        {
            var list = new List<DistributionRequest>();

            var events = eventSearch.GetEvents(filter, null, null, x => x.VenueOffice, x => x.VenueLocation, x => x.Attendees, x => x.Attendees.Select(y => y.Person));

            foreach (var @event in events)
                if (!@event.IntegrationWithholdDistribution)
                    list.Add(CreateDistributionRequest(attemptSearch, assessmentSearch, @event));

            return list;
        }

        public DistributionRequest CreateDistributionRequest(IRegistrationSearch search, IBankSearch assessmentSearch, QEvent ev)
        {
            var request = new DistributionRequest();

            request.When.ActivityIdentifier = ev.EventIdentifier;
            request.When.ActivityNumber = ev.EventNumber;
            request.When.ActivityBillingCode = ev.EventBillingType;
            request.When.ActivityType = ev.ExamType;
            request.When.ActivityDate = ev.EventDate;
            request.When.ActivityTime = ev.EventTime;
            request.When.DistributionExpected = ev.DistributionExpectedText;
            request.When.ExamType = ev.ExamType;

            request.Where.VenueLocationName = ev.VenueLocation?.GroupName;
            request.Where.VenueLocationRoom = ev.VenueRoom;

            var venueContact = ev.Attendees.Where(x => x.AttendeeRole.Contains("Venue Contact")).FirstOrDefault();
            if (venueContact != null)
            {
                request.Where.VenueContactName = venueContact.UserFullName;
                request.Where.VenueContactPhone = venueContact.UserPhone;
            }

            request.Where.InvigilatingOffice = ev.VenueOfficeName;

            GetGroupAddress(ev.VenueOfficeIdentifier, "Shipping", request.Where.VenueLocationShipping);
            GetGroupAddress(ev.VenueLocationIdentifier, "Physical", request.Where.VenueLocationPhysical);

            var registrations = search.GetRegistrations(
                new QRegistrationFilter 
                { 
                    EventIdentifier = ev.EventIdentifier ,
                    HasExamForm = true
                },
                x => x.Candidate, x => x.Accommodations
            );

            var forms = assessmentSearch.GetForms(registrations.Select(x => x.ExamFormIdentifier.Value).Distinct().ToArray());

            foreach (var form in forms)
            {
                var bank = assessmentSearch.GetBankState(form.BankIdentifier);

                /*
                The BC Mail API requires a unique integer value for every exam form, but InSite uses decimal values
                to distinguish between versions of the same exam form. Because BC Mail is unable to store decimal
                values for this purpose (e.g. Asset #1234.5 = v5 of exam form 1234), we need to convert our decimal
                value into an integer value before we send it to BC Mail. To do this, we multiple by 100 and add the 
                version number.

                For example, InSite Asset #1234.5 becomes BC Mail Form #123405

                !IMPORTANT NOTE! This assumes a maximum of 99 versions per form.
                 */

                if (form.FormAssetVersion > 99)
                    throw new ArgumentOutOfRangeException($"The version number for this exam form ({form.FormAsset}.{form.FormAssetVersion}) cannot be submitted to BC Mail because the version number on a form cannot exceed 99.");

                var formNumber = (form.FormAsset * 100) + form.FormAssetVersion;

                var what = new DistributionRequestWhat
                {
                    FormNumber = formNumber,
                    FormNumberType = "Shift iQ Asset",
                    FormType = bank.Level.Type,
                    FormTitle = form.FormTitle,
                    FormLevel = $"{bank.Level.Number}",
                    FormName = $"{form.FormCode}{formNumber}"
                };

                if (form.FormOrigin.HasValue())
                {
                    var origin = form.FormOrigin;
                    if (origin.StartsWith("Access Scheduler Exam ID "))
                    {
                        var number = origin.Replace("Access Scheduler Exam ID ", string.Empty);
                        if (int.TryParse(number, out int n))
                        {
                            what.FormNumber = n;
                            what.FormNumberType = "Access Scheduler Exam ID";
                        }
                    }
                }

                var f = bank.FindForm(form.FormIdentifier);
                if (f != null)
                {
                    what.Materials.DiagramBook = f.HasDiagrams;
                    what.Materials.ForCandidates = f.Content?.MaterialsForParticipation?.Default;
                    what.Materials.ForDistribution = f.Content?.MaterialsForDistribution?.Default;
                }

                foreach (var registration in registrations.Where(x => x.ExamFormIdentifier == form.FormIdentifier))
                {
                    var accomodationTypes = string.Join(", ", registration.Accommodations.Select(x => x.AccommodationType).OrderBy(x => x));
                    var accomodationNames = string.Join(", ", registration.Accommodations.Select(x => x.AccommodationName).OrderBy(x => x));

                    var who = new DistributionRequestWhatWho
                    {
                        CandidateCode = registration.Candidate.PersonCode,
                        CandidateFirstName = registration.Candidate.UserFirstName,
                        CandidateLastName = registration.Candidate.UserLastName,
                        AccommodationType = accomodationTypes,
                        AccommodationComment = accomodationNames
                    };
                    what.Who.Add(who);
                }

                request.What.Add(what);
            }

            return request;
        }

        private void GetGroupAddress(Guid? groupId, string addressType, DistributionRequestLocation location)
        {
            if (groupId == null)
                return;

            AddressType type;

            if (addressType == "Shipping")
                type = AddressType.Shipping;
            else if (addressType == "Physical")
                type = AddressType.Physical;
            else
                throw ApplicationError.Create("Unexpected address type: {0}", addressType);

            var address = ServiceLocator.GroupSearch.GetAddress(groupId.Value, type);
            if (address != null)
            {
                location.Description = address.Description;
                location.Address1 = address.Street1;
                location.Address2 = address.Street2;
                location.City = address.City;
                location.PostalCode = address.PostalCode;
                location.Province = address.Province;
            }
        }
    }
}