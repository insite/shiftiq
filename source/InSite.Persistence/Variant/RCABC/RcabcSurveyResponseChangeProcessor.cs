using System;

using Shift.Common.Timeline.Changes;

using InSite.Application;
using InSite.Application.Invoices.Write;
using InSite.Application.Surveys.Read;
using InSite.Domain.Invoices;
using InSite.Domain.Surveys.Sessions;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence.Plugin.RCABC
{
    /// <remarks>
    /// A process manager (sometimes called a saga in CQRS) is an independent component that reacts to domain events in 
    /// a cross-aggregate, eventually consistent manner. Time can be a trigger. Process managers are sometimes purely 
    /// reactive, and sometimes represent workflows.
    /// 
    /// From an implementation perspective, a process manager is a state machine that is driven forward by incoming 
    /// events (which may come from many aggregates). Some states will have side effects, such as sending commands, 
    /// talking to external web services, or sending emails.
    /// </remarks>
    public class SurveyResponseChangeProcessor
    {
        private readonly ICommander _commander;

        private readonly ISurveySearch _surveys;

        public SurveyResponseChangeProcessor(
            ICommander commander,
            IChangeQueue publisher,
            ISurveySearch surveys)
        {
            _commander = commander;
            _surveys = surveys;

            publisher.Extend<ResponseSessionCompleted>(Handle, Shift.Constant.OrganizationIdentifiers.RCABC);
        }

        private void Handle(ResponseSessionCompleted c)
        {
            // When a respondent answers Yes to question 1 on page 2 of the Master Roofer survey, this
            // trigger creates a new invoice, payable by the respondent to RCABC.

            // Master Roofer
            var survey = Guid.Parse("62a981d9-bc1c-471d-a59a-af0e0154e492");

            // Are you ready to submit your application and pay your application fee?
            var option = Guid.Parse("257a07a7-927c-4944-abe2-af0e0154e4a5"); // ==> Yes

            // Get the response.
            var session = _surveys.GetResponseSession(c.AggregateIdentifier);
            if (session.SurveyFormIdentifier == survey)
            {
                // If the respondent answered "Yes" to the question about invoicing then create an invoice.
                var o = _surveys.GetResponseOption(session.ResponseSessionIdentifier, option);
                if (o != null && o.ResponseOptionIsSelected)
                {
                    var organization = Shift.Constant.OrganizationIdentifiers.RCABC;
                    var invoice = UniqueIdentifier.Create();
                    var number = Sequence.Increment(organization, SequenceType.Invoice);
                    var customer = session.RespondentUserIdentifier;
                    var product = session.ResponseSessionIdentifier;
                    var amount = 150.0m;
                    var description = "Master Roofer Application";

                    _commander.Send(new DraftInvoice(invoice, organization, number, customer, new[]
                    {
                        new InvoiceItem
                        {
                            Identifier = UuidFactory.Create(),
                            Product = product,
                            Quantity = 1,
                            Price = amount ,
                            Description = description
                        }
                    }));
                    _commander.Send(new SubmitInvoice(invoice));
                }
            }
        }
    }
}
