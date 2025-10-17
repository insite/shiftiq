namespace Shift.Contract
{
    ///<summary>
    /// Defines authorization policies for every entity
    /// </summary>
    /// <remarks>
    /// ## Queries
    /// Assert = **HEAD**: Check for the existence of one specific item
    /// Retrieve = **GET**:  Retrieve one specific item
    /// Collect = **GET or POST**: Collect the list of items that match specific criteria
    /// Count = **GET or POST**: Count the list of items that match specific criteria
    /// Download = **GET or POST**: Download the list of items that match specific criteria
    /// Export = **POST**: Export the list of items that match specific criteria
    /// Search = **GET or POST**: Search for the list of items that match specific criteria
    /// ## Commands
    /// Create = **POST**: Insert one specific item
    /// Delete = **DELETE**: Delete one specific item
    /// Import = **POST**: Insert a list of items
    /// Modify = **PUT**: Update one specific item
    /// Purge = **POST**: Delete the list of items that match specific criteria
    /// </remarks>
    public static partial class Policies
    {
        public static partial class Assessment // Component
        {
            public static partial class Answers // Subcomponent
            {
                public static partial class Attempt // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Answers.Attempt.Assert";
                    public const string Retrieve = "Assessment.Answers.Attempt.Retrieve";

                    public const string Collect = "Assessment.Answers.Attempt.Collect";
                    public const string Count = "Assessment.Answers.Attempt.Count";
                    public const string Search = "Assessment.Answers.Attempt.Search";
                    public const string Download = "Assessment.Answers.Attempt.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class AttemptMatch // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Answers.AttemptMatch.Assert";
                    public const string Retrieve = "Assessment.Answers.AttemptMatch.Retrieve";

                    public const string Collect = "Assessment.Answers.AttemptMatch.Collect";
                    public const string Count = "Assessment.Answers.AttemptMatch.Count";
                    public const string Search = "Assessment.Answers.AttemptMatch.Search";
                    public const string Download = "Assessment.Answers.AttemptMatch.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class AttemptOption // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Answers.AttemptOption.Assert";
                    public const string Retrieve = "Assessment.Answers.AttemptOption.Retrieve";

                    public const string Collect = "Assessment.Answers.AttemptOption.Collect";
                    public const string Count = "Assessment.Answers.AttemptOption.Count";
                    public const string Search = "Assessment.Answers.AttemptOption.Search";
                    public const string Download = "Assessment.Answers.AttemptOption.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class AttemptPin // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Answers.AttemptPin.Assert";
                    public const string Retrieve = "Assessment.Answers.AttemptPin.Retrieve";

                    public const string Collect = "Assessment.Answers.AttemptPin.Collect";
                    public const string Count = "Assessment.Answers.AttemptPin.Count";
                    public const string Search = "Assessment.Answers.AttemptPin.Search";
                    public const string Download = "Assessment.Answers.AttemptPin.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class AttemptQuestion // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Answers.AttemptQuestion.Assert";
                    public const string Retrieve = "Assessment.Answers.AttemptQuestion.Retrieve";

                    public const string Collect = "Assessment.Answers.AttemptQuestion.Collect";
                    public const string Count = "Assessment.Answers.AttemptQuestion.Count";
                    public const string Search = "Assessment.Answers.AttemptQuestion.Search";
                    public const string Download = "Assessment.Answers.AttemptQuestion.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class AttemptSection // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Answers.AttemptSection.Assert";
                    public const string Retrieve = "Assessment.Answers.AttemptSection.Retrieve";

                    public const string Collect = "Assessment.Answers.AttemptSection.Collect";
                    public const string Count = "Assessment.Answers.AttemptSection.Count";
                    public const string Search = "Assessment.Answers.AttemptSection.Search";
                    public const string Download = "Assessment.Answers.AttemptSection.Download";

                    // Commands

                    public const string Create = "Assessment.Answers.AttemptSection.Create";
                    public const string Delete = "Assessment.Answers.AttemptSection.Delete";
                    public const string Modify = "Assessment.Answers.AttemptSection.Modify";
                }

                public static partial class AttemptSolution // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Answers.AttemptSolution.Assert";
                    public const string Retrieve = "Assessment.Answers.AttemptSolution.Retrieve";

                    public const string Collect = "Assessment.Answers.AttemptSolution.Collect";
                    public const string Count = "Assessment.Answers.AttemptSolution.Count";
                    public const string Search = "Assessment.Answers.AttemptSolution.Search";
                    public const string Download = "Assessment.Answers.AttemptSolution.Download";

                    // Commands

                    public const string Create = "Assessment.Answers.AttemptSolution.Create";
                    public const string Delete = "Assessment.Answers.AttemptSolution.Delete";
                    public const string Modify = "Assessment.Answers.AttemptSolution.Modify";
                }

                public static partial class LearnerFormAttempt // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Answers.LearnerFormAttempt.Assert";
                    public const string Retrieve = "Assessment.Answers.LearnerFormAttempt.Retrieve";

                    public const string Collect = "Assessment.Answers.LearnerFormAttempt.Collect";
                    public const string Count = "Assessment.Answers.LearnerFormAttempt.Count";
                    public const string Search = "Assessment.Answers.LearnerFormAttempt.Search";
                    public const string Download = "Assessment.Answers.LearnerFormAttempt.Download";

                    // Commands

                    public const string Create = "Assessment.Answers.LearnerFormAttempt.Create";
                    public const string Delete = "Assessment.Answers.LearnerFormAttempt.Delete";
                    public const string Modify = "Assessment.Answers.LearnerFormAttempt.Modify";
                }

                public static partial class QuizAttempt // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Answers.QuizAttempt.Assert";
                    public const string Retrieve = "Assessment.Answers.QuizAttempt.Retrieve";

                    public const string Collect = "Assessment.Answers.QuizAttempt.Collect";
                    public const string Count = "Assessment.Answers.QuizAttempt.Count";
                    public const string Search = "Assessment.Answers.QuizAttempt.Search";
                    public const string Download = "Assessment.Answers.QuizAttempt.Download";

                    // Commands

                    public const string Create = "Assessment.Answers.QuizAttempt.Create";
                    public const string Delete = "Assessment.Answers.QuizAttempt.Delete";
                    public const string Modify = "Assessment.Answers.QuizAttempt.Modify";
                }
            }

            public static partial class Questions // Subcomponent
            {
                public static partial class Bank // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Questions.Bank.Assert";
                    public const string Retrieve = "Assessment.Questions.Bank.Retrieve";

                    public const string Collect = "Assessment.Questions.Bank.Collect";
                    public const string Count = "Assessment.Questions.Bank.Count";
                    public const string Search = "Assessment.Questions.Bank.Search";
                    public const string Download = "Assessment.Questions.Bank.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class BankForm // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Questions.BankForm.Assert";
                    public const string Retrieve = "Assessment.Questions.BankForm.Retrieve";

                    public const string Collect = "Assessment.Questions.BankForm.Collect";
                    public const string Count = "Assessment.Questions.BankForm.Count";
                    public const string Search = "Assessment.Questions.BankForm.Search";
                    public const string Download = "Assessment.Questions.BankForm.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class BankFormQuestionGradeitem // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Questions.BankFormQuestionGradeitem.Assert";
                    public const string Retrieve = "Assessment.Questions.BankFormQuestionGradeitem.Retrieve";

                    public const string Collect = "Assessment.Questions.BankFormQuestionGradeitem.Collect";
                    public const string Count = "Assessment.Questions.BankFormQuestionGradeitem.Count";
                    public const string Search = "Assessment.Questions.BankFormQuestionGradeitem.Search";
                    public const string Download = "Assessment.Questions.BankFormQuestionGradeitem.Download";

                    // Commands

                    public const string Create = "Assessment.Questions.BankFormQuestionGradeitem.Create";
                    public const string Delete = "Assessment.Questions.BankFormQuestionGradeitem.Delete";
                    public const string Modify = "Assessment.Questions.BankFormQuestionGradeitem.Modify";
                }

                public static partial class BankOption // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Questions.BankOption.Assert";
                    public const string Retrieve = "Assessment.Questions.BankOption.Retrieve";

                    public const string Collect = "Assessment.Questions.BankOption.Collect";
                    public const string Count = "Assessment.Questions.BankOption.Count";
                    public const string Search = "Assessment.Questions.BankOption.Search";
                    public const string Download = "Assessment.Questions.BankOption.Download";

                    // Commands

                    public const string Create = "Assessment.Questions.BankOption.Create";
                    public const string Delete = "Assessment.Questions.BankOption.Delete";
                    public const string Modify = "Assessment.Questions.BankOption.Modify";
                }

                public static partial class BankQuestion // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Questions.BankQuestion.Assert";
                    public const string Retrieve = "Assessment.Questions.BankQuestion.Retrieve";

                    public const string Collect = "Assessment.Questions.BankQuestion.Collect";
                    public const string Count = "Assessment.Questions.BankQuestion.Count";
                    public const string Search = "Assessment.Questions.BankQuestion.Search";
                    public const string Download = "Assessment.Questions.BankQuestion.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class BankQuestionAttachment // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Questions.BankQuestionAttachment.Assert";
                    public const string Retrieve = "Assessment.Questions.BankQuestionAttachment.Retrieve";

                    public const string Collect = "Assessment.Questions.BankQuestionAttachment.Collect";
                    public const string Count = "Assessment.Questions.BankQuestionAttachment.Count";
                    public const string Search = "Assessment.Questions.BankQuestionAttachment.Search";
                    public const string Download = "Assessment.Questions.BankQuestionAttachment.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class BankQuestionCompetency // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Questions.BankQuestionCompetency.Assert";
                    public const string Retrieve = "Assessment.Questions.BankQuestionCompetency.Retrieve";

                    public const string Collect = "Assessment.Questions.BankQuestionCompetency.Collect";
                    public const string Count = "Assessment.Questions.BankQuestionCompetency.Count";
                    public const string Search = "Assessment.Questions.BankQuestionCompetency.Search";
                    public const string Download = "Assessment.Questions.BankQuestionCompetency.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class BankSpecification // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Questions.BankSpecification.Assert";
                    public const string Retrieve = "Assessment.Questions.BankSpecification.Retrieve";

                    public const string Collect = "Assessment.Questions.BankSpecification.Collect";
                    public const string Count = "Assessment.Questions.BankSpecification.Count";
                    public const string Search = "Assessment.Questions.BankSpecification.Search";
                    public const string Download = "Assessment.Questions.BankSpecification.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class Quiz // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Questions.Quiz.Assert";
                    public const string Retrieve = "Assessment.Questions.Quiz.Retrieve";

                    public const string Collect = "Assessment.Questions.Quiz.Collect";
                    public const string Count = "Assessment.Questions.Quiz.Count";
                    public const string Search = "Assessment.Questions.Quiz.Search";
                    public const string Download = "Assessment.Questions.Quiz.Download";

                    // Commands

                    public const string Create = "Assessment.Questions.Quiz.Create";
                    public const string Delete = "Assessment.Questions.Quiz.Delete";
                    public const string Modify = "Assessment.Questions.Quiz.Modify";
                }
            }

            public static partial class Rubrics // Subcomponent
            {
                public static partial class Rubric // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Rubrics.Rubric.Assert";
                    public const string Retrieve = "Assessment.Rubrics.Rubric.Retrieve";

                    public const string Collect = "Assessment.Rubrics.Rubric.Collect";
                    public const string Count = "Assessment.Rubrics.Rubric.Count";
                    public const string Search = "Assessment.Rubrics.Rubric.Search";
                    public const string Download = "Assessment.Rubrics.Rubric.Download";

                    // Commands

                    public const string Create = "Assessment.Rubrics.Rubric.Create";
                    public const string Delete = "Assessment.Rubrics.Rubric.Delete";
                    public const string Modify = "Assessment.Rubrics.Rubric.Modify";
                }

                public static partial class RubricCriterion // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Rubrics.RubricCriterion.Assert";
                    public const string Retrieve = "Assessment.Rubrics.RubricCriterion.Retrieve";

                    public const string Collect = "Assessment.Rubrics.RubricCriterion.Collect";
                    public const string Count = "Assessment.Rubrics.RubricCriterion.Count";
                    public const string Search = "Assessment.Rubrics.RubricCriterion.Search";
                    public const string Download = "Assessment.Rubrics.RubricCriterion.Download";

                    // Commands

                    public const string Create = "Assessment.Rubrics.RubricCriterion.Create";
                    public const string Delete = "Assessment.Rubrics.RubricCriterion.Delete";
                    public const string Modify = "Assessment.Rubrics.RubricCriterion.Modify";
                }

                public static partial class RubricRating // Entity
                {
                    // Queries

                    public const string Assert = "Assessment.Rubrics.RubricRating.Assert";
                    public const string Retrieve = "Assessment.Rubrics.RubricRating.Retrieve";

                    public const string Collect = "Assessment.Rubrics.RubricRating.Collect";
                    public const string Count = "Assessment.Rubrics.RubricRating.Count";
                    public const string Search = "Assessment.Rubrics.RubricRating.Search";
                    public const string Download = "Assessment.Rubrics.RubricRating.Download";

                    // Commands

                    public const string Create = "Assessment.Rubrics.RubricRating.Create";
                    public const string Delete = "Assessment.Rubrics.RubricRating.Delete";
                    public const string Modify = "Assessment.Rubrics.RubricRating.Modify";
                }
            }
        }

        public static partial class Billing // Component
        {
            public static partial class Discounts // Subcomponent
            {
                public static partial class Discount // Entity
                {
                    // Queries

                    public const string Assert = "Billing.Discounts.Discount.Assert";
                    public const string Retrieve = "Billing.Discounts.Discount.Retrieve";

                    public const string Collect = "Billing.Discounts.Discount.Collect";
                    public const string Count = "Billing.Discounts.Discount.Count";
                    public const string Search = "Billing.Discounts.Discount.Search";
                    public const string Download = "Billing.Discounts.Discount.Download";

                    // Commands

                    public const string Create = "Billing.Discounts.Discount.Create";
                    public const string Delete = "Billing.Discounts.Discount.Delete";
                    public const string Modify = "Billing.Discounts.Discount.Modify";
                }
            }

            public static partial class Invoices // Subcomponent
            {
                public static partial class Invoice // Entity
                {
                    // Queries

                    public const string Assert = "Billing.Invoices.Invoice.Assert";
                    public const string Retrieve = "Billing.Invoices.Invoice.Retrieve";

                    public const string Collect = "Billing.Invoices.Invoice.Collect";
                    public const string Count = "Billing.Invoices.Invoice.Count";
                    public const string Search = "Billing.Invoices.Invoice.Search";
                    public const string Download = "Billing.Invoices.Invoice.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class InvoiceItem // Entity
                {
                    // Queries

                    public const string Assert = "Billing.Invoices.InvoiceItem.Assert";
                    public const string Retrieve = "Billing.Invoices.InvoiceItem.Retrieve";

                    public const string Collect = "Billing.Invoices.InvoiceItem.Collect";
                    public const string Count = "Billing.Invoices.InvoiceItem.Count";
                    public const string Search = "Billing.Invoices.InvoiceItem.Search";
                    public const string Download = "Billing.Invoices.InvoiceItem.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }

            public static partial class Orders // Subcomponent
            {
                public static partial class Order // Entity
                {
                    // Queries

                    public const string Assert = "Billing.Orders.Order.Assert";
                    public const string Retrieve = "Billing.Orders.Order.Retrieve";

                    public const string Collect = "Billing.Orders.Order.Collect";
                    public const string Count = "Billing.Orders.Order.Count";
                    public const string Search = "Billing.Orders.Order.Search";
                    public const string Download = "Billing.Orders.Order.Download";

                    // Commands

                    public const string Create = "Billing.Orders.Order.Create";
                    public const string Delete = "Billing.Orders.Order.Delete";
                    public const string Modify = "Billing.Orders.Order.Modify";
                }
            }

            public static partial class Payments // Subcomponent
            {
                public static partial class Payment // Entity
                {
                    // Queries

                    public const string Assert = "Billing.Payments.Payment.Assert";
                    public const string Retrieve = "Billing.Payments.Payment.Retrieve";

                    public const string Collect = "Billing.Payments.Payment.Collect";
                    public const string Count = "Billing.Payments.Payment.Count";
                    public const string Search = "Billing.Payments.Payment.Search";
                    public const string Download = "Billing.Payments.Payment.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }

            public static partial class Products // Subcomponent
            {
                public static partial class Product // Entity
                {
                    // Queries

                    public const string Assert = "Billing.Products.Product.Assert";
                    public const string Retrieve = "Billing.Products.Product.Retrieve";

                    public const string Collect = "Billing.Products.Product.Collect";
                    public const string Count = "Billing.Products.Product.Count";
                    public const string Search = "Billing.Products.Product.Search";
                    public const string Download = "Billing.Products.Product.Download";

                    // Commands

                    public const string Create = "Billing.Products.Product.Create";
                    public const string Delete = "Billing.Products.Product.Delete";
                    public const string Modify = "Billing.Products.Product.Modify";
                }
            }
        }

        public static partial class Booking // Component
        {
            public static partial class Events // Subcomponent
            {
                public static partial class Event // Entity
                {
                    // Queries

                    public const string Assert = "Booking.Events.Event.Assert";
                    public const string Retrieve = "Booking.Events.Event.Retrieve";

                    public const string Collect = "Booking.Events.Event.Collect";
                    public const string Count = "Booking.Events.Event.Count";
                    public const string Search = "Booking.Events.Event.Search";
                    public const string Download = "Booking.Events.Event.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class EventForm // Entity
                {
                    // Queries

                    public const string Assert = "Booking.Events.EventForm.Assert";
                    public const string Retrieve = "Booking.Events.EventForm.Retrieve";

                    public const string Collect = "Booking.Events.EventForm.Collect";
                    public const string Count = "Booking.Events.EventForm.Count";
                    public const string Search = "Booking.Events.EventForm.Search";
                    public const string Download = "Booking.Events.EventForm.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class EventSeat // Entity
                {
                    // Queries

                    public const string Assert = "Booking.Events.EventSeat.Assert";
                    public const string Retrieve = "Booking.Events.EventSeat.Retrieve";

                    public const string Collect = "Booking.Events.EventSeat.Collect";
                    public const string Count = "Booking.Events.EventSeat.Count";
                    public const string Search = "Booking.Events.EventSeat.Search";
                    public const string Download = "Booking.Events.EventSeat.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class EventTimer // Entity
                {
                    // Queries

                    public const string Assert = "Booking.Events.EventTimer.Assert";
                    public const string Retrieve = "Booking.Events.EventTimer.Retrieve";

                    public const string Collect = "Booking.Events.EventTimer.Collect";
                    public const string Count = "Booking.Events.EventTimer.Count";
                    public const string Search = "Booking.Events.EventTimer.Search";
                    public const string Download = "Booking.Events.EventTimer.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class EventUser // Entity
                {
                    // Queries

                    public const string Assert = "Booking.Events.EventUser.Assert";
                    public const string Retrieve = "Booking.Events.EventUser.Retrieve";

                    public const string Collect = "Booking.Events.EventUser.Collect";
                    public const string Count = "Booking.Events.EventUser.Count";
                    public const string Search = "Booking.Events.EventUser.Search";
                    public const string Download = "Booking.Events.EventUser.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }

            public static partial class Registrations // Subcomponent
            {
                public static partial class Registration // Entity
                {
                    // Queries

                    public const string Assert = "Booking.Registrations.Registration.Assert";
                    public const string Retrieve = "Booking.Registrations.Registration.Retrieve";

                    public const string Collect = "Booking.Registrations.Registration.Collect";
                    public const string Count = "Booking.Registrations.Registration.Count";
                    public const string Search = "Booking.Registrations.Registration.Search";
                    public const string Download = "Booking.Registrations.Registration.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class RegistrationAccommodation // Entity
                {
                    // Queries

                    public const string Assert = "Booking.Registrations.RegistrationAccommodation.Assert";
                    public const string Retrieve = "Booking.Registrations.RegistrationAccommodation.Retrieve";

                    public const string Collect = "Booking.Registrations.RegistrationAccommodation.Collect";
                    public const string Count = "Booking.Registrations.RegistrationAccommodation.Count";
                    public const string Search = "Booking.Registrations.RegistrationAccommodation.Search";
                    public const string Download = "Booking.Registrations.RegistrationAccommodation.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class RegistrationInstructor // Entity
                {
                    // Queries

                    public const string Assert = "Booking.Registrations.RegistrationInstructor.Assert";
                    public const string Retrieve = "Booking.Registrations.RegistrationInstructor.Retrieve";

                    public const string Collect = "Booking.Registrations.RegistrationInstructor.Collect";
                    public const string Count = "Booking.Registrations.RegistrationInstructor.Count";
                    public const string Search = "Booking.Registrations.RegistrationInstructor.Search";
                    public const string Download = "Booking.Registrations.RegistrationInstructor.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class RegistrationTimer // Entity
                {
                    // Queries

                    public const string Assert = "Booking.Registrations.RegistrationTimer.Assert";
                    public const string Retrieve = "Booking.Registrations.RegistrationTimer.Retrieve";

                    public const string Collect = "Booking.Registrations.RegistrationTimer.Collect";
                    public const string Count = "Booking.Registrations.RegistrationTimer.Count";
                    public const string Search = "Booking.Registrations.RegistrationTimer.Search";
                    public const string Download = "Booking.Registrations.RegistrationTimer.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }
        }

        public static partial class Competency // Component
        {
            public static partial class Documents // Subcomponent
            {
                public static partial class Document // Entity
                {
                    // Queries

                    public const string Assert = "Competency.Documents.Document.Assert";
                    public const string Retrieve = "Competency.Documents.Document.Retrieve";

                    public const string Collect = "Competency.Documents.Document.Collect";
                    public const string Count = "Competency.Documents.Document.Count";
                    public const string Search = "Competency.Documents.Document.Search";
                    public const string Download = "Competency.Documents.Document.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class DocumentCompetency // Entity
                {
                    // Queries

                    public const string Assert = "Competency.Documents.DocumentCompetency.Assert";
                    public const string Retrieve = "Competency.Documents.DocumentCompetency.Retrieve";

                    public const string Collect = "Competency.Documents.DocumentCompetency.Collect";
                    public const string Count = "Competency.Documents.DocumentCompetency.Count";
                    public const string Search = "Competency.Documents.DocumentCompetency.Search";
                    public const string Download = "Competency.Documents.DocumentCompetency.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class DocumentConnection // Entity
                {
                    // Queries

                    public const string Assert = "Competency.Documents.DocumentConnection.Assert";
                    public const string Retrieve = "Competency.Documents.DocumentConnection.Retrieve";

                    public const string Collect = "Competency.Documents.DocumentConnection.Collect";
                    public const string Count = "Competency.Documents.DocumentConnection.Count";
                    public const string Search = "Competency.Documents.DocumentConnection.Search";
                    public const string Download = "Competency.Documents.DocumentConnection.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }

            public static partial class Standards // Subcomponent
            {
                public static partial class ProfileGroupCompetency // Entity
                {
                    // Queries

                    public const string Assert = "Competency.Standards.ProfileGroupCompetency.Assert";
                    public const string Retrieve = "Competency.Standards.ProfileGroupCompetency.Retrieve";

                    public const string Collect = "Competency.Standards.ProfileGroupCompetency.Collect";
                    public const string Count = "Competency.Standards.ProfileGroupCompetency.Count";
                    public const string Search = "Competency.Standards.ProfileGroupCompetency.Search";
                    public const string Download = "Competency.Standards.ProfileGroupCompetency.Download";

                    // Commands

                    public const string Create = "Competency.Standards.ProfileGroupCompetency.Create";
                    public const string Delete = "Competency.Standards.ProfileGroupCompetency.Delete";
                    public const string Modify = "Competency.Standards.ProfileGroupCompetency.Modify";
                }

                public static partial class ProfileGroupLearner // Entity
                {
                    // Queries

                    public const string Assert = "Competency.Standards.ProfileGroupLearner.Assert";
                    public const string Retrieve = "Competency.Standards.ProfileGroupLearner.Retrieve";

                    public const string Collect = "Competency.Standards.ProfileGroupLearner.Collect";
                    public const string Count = "Competency.Standards.ProfileGroupLearner.Count";
                    public const string Search = "Competency.Standards.ProfileGroupLearner.Search";
                    public const string Download = "Competency.Standards.ProfileGroupLearner.Download";

                    // Commands

                    public const string Create = "Competency.Standards.ProfileGroupLearner.Create";
                    public const string Delete = "Competency.Standards.ProfileGroupLearner.Delete";
                    public const string Modify = "Competency.Standards.ProfileGroupLearner.Modify";
                }

                public static partial class Standard // Entity
                {
                    // Queries

                    public const string Assert = "Competency.Standards.Standard.Assert";
                    public const string Retrieve = "Competency.Standards.Standard.Retrieve";

                    public const string Collect = "Competency.Standards.Standard.Collect";
                    public const string Count = "Competency.Standards.Standard.Count";
                    public const string Search = "Competency.Standards.Standard.Search";
                    public const string Download = "Competency.Standards.Standard.Download";

                    // Commands

                    public const string Create = "Competency.Standards.Standard.Create";
                    public const string Delete = "Competency.Standards.Standard.Delete";
                    public const string Modify = "Competency.Standards.Standard.Modify";
                }

                public static partial class StandardAchievement // Entity
                {
                    // Queries

                    public const string Assert = "Competency.Standards.StandardAchievement.Assert";
                    public const string Retrieve = "Competency.Standards.StandardAchievement.Retrieve";

                    public const string Collect = "Competency.Standards.StandardAchievement.Collect";
                    public const string Count = "Competency.Standards.StandardAchievement.Count";
                    public const string Search = "Competency.Standards.StandardAchievement.Search";
                    public const string Download = "Competency.Standards.StandardAchievement.Download";

                    // Commands

                    public const string Create = "Competency.Standards.StandardAchievement.Create";
                    public const string Delete = "Competency.Standards.StandardAchievement.Delete";
                    public const string Modify = "Competency.Standards.StandardAchievement.Modify";
                }

                public static partial class StandardCategory // Entity
                {
                    // Queries

                    public const string Assert = "Competency.Standards.StandardCategory.Assert";
                    public const string Retrieve = "Competency.Standards.StandardCategory.Retrieve";

                    public const string Collect = "Competency.Standards.StandardCategory.Collect";
                    public const string Count = "Competency.Standards.StandardCategory.Count";
                    public const string Search = "Competency.Standards.StandardCategory.Search";
                    public const string Download = "Competency.Standards.StandardCategory.Download";

                    // Commands

                    public const string Create = "Competency.Standards.StandardCategory.Create";
                    public const string Delete = "Competency.Standards.StandardCategory.Delete";
                    public const string Modify = "Competency.Standards.StandardCategory.Modify";
                }

                public static partial class StandardConnection // Entity
                {
                    // Queries

                    public const string Assert = "Competency.Standards.StandardConnection.Assert";
                    public const string Retrieve = "Competency.Standards.StandardConnection.Retrieve";

                    public const string Collect = "Competency.Standards.StandardConnection.Collect";
                    public const string Count = "Competency.Standards.StandardConnection.Count";
                    public const string Search = "Competency.Standards.StandardConnection.Search";
                    public const string Download = "Competency.Standards.StandardConnection.Download";

                    // Commands

                    public const string Create = "Competency.Standards.StandardConnection.Create";
                    public const string Delete = "Competency.Standards.StandardConnection.Delete";
                    public const string Modify = "Competency.Standards.StandardConnection.Modify";
                }

                public static partial class StandardContainment // Entity
                {
                    // Queries

                    public const string Assert = "Competency.Standards.StandardContainment.Assert";
                    public const string Retrieve = "Competency.Standards.StandardContainment.Retrieve";

                    public const string Collect = "Competency.Standards.StandardContainment.Collect";
                    public const string Count = "Competency.Standards.StandardContainment.Count";
                    public const string Search = "Competency.Standards.StandardContainment.Search";
                    public const string Download = "Competency.Standards.StandardContainment.Download";

                    // Commands

                    public const string Create = "Competency.Standards.StandardContainment.Create";
                    public const string Delete = "Competency.Standards.StandardContainment.Delete";
                    public const string Modify = "Competency.Standards.StandardContainment.Modify";
                }

                public static partial class StandardGroup // Entity
                {
                    // Queries

                    public const string Assert = "Competency.Standards.StandardGroup.Assert";
                    public const string Retrieve = "Competency.Standards.StandardGroup.Retrieve";

                    public const string Collect = "Competency.Standards.StandardGroup.Collect";
                    public const string Count = "Competency.Standards.StandardGroup.Count";
                    public const string Search = "Competency.Standards.StandardGroup.Search";
                    public const string Download = "Competency.Standards.StandardGroup.Download";

                    // Commands

                    public const string Create = "Competency.Standards.StandardGroup.Create";
                    public const string Delete = "Competency.Standards.StandardGroup.Delete";
                    public const string Modify = "Competency.Standards.StandardGroup.Modify";
                }

                public static partial class StandardOrganization // Entity
                {
                    // Queries

                    public const string Assert = "Competency.Standards.StandardOrganization.Assert";
                    public const string Retrieve = "Competency.Standards.StandardOrganization.Retrieve";

                    public const string Collect = "Competency.Standards.StandardOrganization.Collect";
                    public const string Count = "Competency.Standards.StandardOrganization.Count";
                    public const string Search = "Competency.Standards.StandardOrganization.Search";
                    public const string Download = "Competency.Standards.StandardOrganization.Download";

                    // Commands

                    public const string Create = "Competency.Standards.StandardOrganization.Create";
                    public const string Delete = "Competency.Standards.StandardOrganization.Delete";
                    public const string Modify = "Competency.Standards.StandardOrganization.Modify";
                }
            }

            public static partial class Tiers // Subcomponent
            {
                public static partial class StandardTier // Entity
                {
                    // Queries

                    public const string Assert = "Competency.Tiers.StandardTier.Assert";
                    public const string Retrieve = "Competency.Tiers.StandardTier.Retrieve";

                    public const string Collect = "Competency.Tiers.StandardTier.Collect";
                    public const string Count = "Competency.Tiers.StandardTier.Count";
                    public const string Search = "Competency.Tiers.StandardTier.Search";
                    public const string Download = "Competency.Tiers.StandardTier.Download";

                    // Commands

                    public const string Create = "Competency.Tiers.StandardTier.Create";
                    public const string Delete = "Competency.Tiers.StandardTier.Delete";
                    public const string Modify = "Competency.Tiers.StandardTier.Modify";
                }
            }

            public static partial class Validations // Subcomponent
            {
                public static partial class Validation // Entity
                {
                    // Queries

                    public const string Assert = "Competency.Validations.Validation.Assert";
                    public const string Retrieve = "Competency.Validations.Validation.Retrieve";

                    public const string Collect = "Competency.Validations.Validation.Collect";
                    public const string Count = "Competency.Validations.Validation.Count";
                    public const string Search = "Competency.Validations.Validation.Search";
                    public const string Download = "Competency.Validations.Validation.Download";

                    // Commands

                    public const string Create = "Competency.Validations.Validation.Create";
                    public const string Delete = "Competency.Validations.Validation.Delete";
                    public const string Modify = "Competency.Validations.Validation.Modify";
                }

                public static partial class ValidationChange // Entity
                {
                    // Queries

                    public const string Assert = "Competency.Validations.ValidationChange.Assert";
                    public const string Retrieve = "Competency.Validations.ValidationChange.Retrieve";

                    public const string Collect = "Competency.Validations.ValidationChange.Collect";
                    public const string Count = "Competency.Validations.ValidationChange.Count";
                    public const string Search = "Competency.Validations.ValidationChange.Search";
                    public const string Download = "Competency.Validations.ValidationChange.Download";

                    // Commands

                    public const string Create = "Competency.Validations.ValidationChange.Create";
                    public const string Delete = "Competency.Validations.ValidationChange.Delete";
                    public const string Modify = "Competency.Validations.ValidationChange.Modify";
                }
            }
        }

        public static partial class Content // Component
        {
            public static partial class Comments // Subcomponent
            {
                public static partial class Comment // Entity
                {
                    // Queries

                    public const string Assert = "Content.Comments.Comment.Assert";
                    public const string Retrieve = "Content.Comments.Comment.Retrieve";

                    public const string Collect = "Content.Comments.Comment.Collect";
                    public const string Count = "Content.Comments.Comment.Count";
                    public const string Search = "Content.Comments.Comment.Search";
                    public const string Download = "Content.Comments.Comment.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }

            public static partial class Files // Subcomponent
            {
                public static partial class File // Entity
                {
                    // Queries

                    public const string Assert = "Content.Files.File.Assert";
                    public const string Retrieve = "Content.Files.File.Retrieve";

                    public const string Collect = "Content.Files.File.Collect";
                    public const string Count = "Content.Files.File.Count";
                    public const string Search = "Content.Files.File.Search";
                    public const string Download = "Content.Files.File.Download";

                    // Commands

                    public const string Create = "Content.Files.File.Create";
                    public const string Delete = "Content.Files.File.Delete";
                    public const string Modify = "Content.Files.File.Modify";
                }

                public static partial class FileActivity // Entity
                {
                    // Queries

                    public const string Assert = "Content.Files.FileActivity.Assert";
                    public const string Retrieve = "Content.Files.FileActivity.Retrieve";

                    public const string Collect = "Content.Files.FileActivity.Collect";
                    public const string Count = "Content.Files.FileActivity.Count";
                    public const string Search = "Content.Files.FileActivity.Search";
                    public const string Download = "Content.Files.FileActivity.Download";

                    // Commands

                    public const string Create = "Content.Files.FileActivity.Create";
                    public const string Delete = "Content.Files.FileActivity.Delete";
                    public const string Modify = "Content.Files.FileActivity.Modify";
                }

                public static partial class FileClaim // Entity
                {
                    // Queries

                    public const string Assert = "Content.Files.FileClaim.Assert";
                    public const string Retrieve = "Content.Files.FileClaim.Retrieve";

                    public const string Collect = "Content.Files.FileClaim.Collect";
                    public const string Count = "Content.Files.FileClaim.Count";
                    public const string Search = "Content.Files.FileClaim.Search";
                    public const string Download = "Content.Files.FileClaim.Download";

                    // Commands

                    public const string Create = "Content.Files.FileClaim.Create";
                    public const string Delete = "Content.Files.FileClaim.Delete";
                    public const string Modify = "Content.Files.FileClaim.Modify";
                }

                public static partial class Upload // Entity
                {
                    // Queries

                    public const string Assert = "Content.Files.Upload.Assert";
                    public const string Retrieve = "Content.Files.Upload.Retrieve";

                    public const string Collect = "Content.Files.Upload.Collect";
                    public const string Count = "Content.Files.Upload.Count";
                    public const string Search = "Content.Files.Upload.Search";
                    public const string Download = "Content.Files.Upload.Download";

                    // Commands

                    public const string Create = "Content.Files.Upload.Create";
                    public const string Delete = "Content.Files.Upload.Delete";
                    public const string Modify = "Content.Files.Upload.Modify";
                }

                public static partial class UploadObject // Entity
                {
                    // Queries

                    public const string Assert = "Content.Files.UploadObject.Assert";
                    public const string Retrieve = "Content.Files.UploadObject.Retrieve";

                    public const string Collect = "Content.Files.UploadObject.Collect";
                    public const string Count = "Content.Files.UploadObject.Count";
                    public const string Search = "Content.Files.UploadObject.Search";
                    public const string Download = "Content.Files.UploadObject.Download";

                    // Commands

                    public const string Create = "Content.Files.UploadObject.Create";
                    public const string Delete = "Content.Files.UploadObject.Delete";
                    public const string Modify = "Content.Files.UploadObject.Modify";
                }
            }

            public static partial class Glossaries // Subcomponent
            {
                public static partial class GlossaryContent // Entity
                {
                    // Queries

                    public const string Assert = "Content.Glossaries.GlossaryContent.Assert";
                    public const string Retrieve = "Content.Glossaries.GlossaryContent.Retrieve";

                    public const string Collect = "Content.Glossaries.GlossaryContent.Collect";
                    public const string Count = "Content.Glossaries.GlossaryContent.Count";
                    public const string Search = "Content.Glossaries.GlossaryContent.Search";
                    public const string Download = "Content.Glossaries.GlossaryContent.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class GlossaryTerm // Entity
                {
                    // Queries

                    public const string Assert = "Content.Glossaries.GlossaryTerm.Assert";
                    public const string Retrieve = "Content.Glossaries.GlossaryTerm.Retrieve";

                    public const string Collect = "Content.Glossaries.GlossaryTerm.Collect";
                    public const string Count = "Content.Glossaries.GlossaryTerm.Count";
                    public const string Search = "Content.Glossaries.GlossaryTerm.Search";
                    public const string Download = "Content.Glossaries.GlossaryTerm.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }

            public static partial class Inputs // Subcomponent
            {
                public static partial class Input // Entity
                {
                    // Queries

                    public const string Assert = "Content.Inputs.Input.Assert";
                    public const string Retrieve = "Content.Inputs.Input.Retrieve";

                    public const string Collect = "Content.Inputs.Input.Collect";
                    public const string Count = "Content.Inputs.Input.Count";
                    public const string Search = "Content.Inputs.Input.Search";
                    public const string Download = "Content.Inputs.Input.Download";

                    // Commands

                    public const string Create = "Content.Inputs.Input.Create";
                    public const string Delete = "Content.Inputs.Input.Delete";
                    public const string Modify = "Content.Inputs.Input.Modify";
                }
            }
        }

        public static partial class Directory // Component
        {
            public static partial class Groups // Subcomponent
            {
                public static partial class Group // Entity
                {
                    // Queries

                    public const string Assert = "Directory.Groups.Group.Assert";
                    public const string Retrieve = "Directory.Groups.Group.Retrieve";

                    public const string Collect = "Directory.Groups.Group.Collect";
                    public const string Count = "Directory.Groups.Group.Count";
                    public const string Search = "Directory.Groups.Group.Search";
                    public const string Download = "Directory.Groups.Group.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class GroupAddress // Entity
                {
                    // Queries

                    public const string Assert = "Directory.Groups.GroupAddress.Assert";
                    public const string Retrieve = "Directory.Groups.GroupAddress.Retrieve";

                    public const string Collect = "Directory.Groups.GroupAddress.Collect";
                    public const string Count = "Directory.Groups.GroupAddress.Count";
                    public const string Search = "Directory.Groups.GroupAddress.Search";
                    public const string Download = "Directory.Groups.GroupAddress.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class GroupConnection // Entity
                {
                    // Queries

                    public const string Assert = "Directory.Groups.GroupConnection.Assert";
                    public const string Retrieve = "Directory.Groups.GroupConnection.Retrieve";

                    public const string Collect = "Directory.Groups.GroupConnection.Collect";
                    public const string Count = "Directory.Groups.GroupConnection.Count";
                    public const string Search = "Directory.Groups.GroupConnection.Search";
                    public const string Download = "Directory.Groups.GroupConnection.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class GroupField // Entity
                {
                    // Queries

                    public const string Assert = "Directory.Groups.GroupField.Assert";
                    public const string Retrieve = "Directory.Groups.GroupField.Retrieve";

                    public const string Collect = "Directory.Groups.GroupField.Collect";
                    public const string Count = "Directory.Groups.GroupField.Count";
                    public const string Search = "Directory.Groups.GroupField.Search";
                    public const string Download = "Directory.Groups.GroupField.Download";

                    // Commands

                    public const string Create = "Directory.Groups.GroupField.Create";
                    public const string Delete = "Directory.Groups.GroupField.Delete";
                    public const string Modify = "Directory.Groups.GroupField.Modify";
                }

                public static partial class GroupTag // Entity
                {
                    // Queries

                    public const string Assert = "Directory.Groups.GroupTag.Assert";
                    public const string Retrieve = "Directory.Groups.GroupTag.Retrieve";

                    public const string Collect = "Directory.Groups.GroupTag.Collect";
                    public const string Count = "Directory.Groups.GroupTag.Count";
                    public const string Search = "Directory.Groups.GroupTag.Search";
                    public const string Download = "Directory.Groups.GroupTag.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }

            public static partial class Memberships // Subcomponent
            {
                public static partial class Membership // Entity
                {
                    // Queries

                    public const string Assert = "Directory.Memberships.Membership.Assert";
                    public const string Retrieve = "Directory.Memberships.Membership.Retrieve";

                    public const string Collect = "Directory.Memberships.Membership.Collect";
                    public const string Count = "Directory.Memberships.Membership.Count";
                    public const string Search = "Directory.Memberships.Membership.Search";
                    public const string Download = "Directory.Memberships.Membership.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class MembershipDeletion // Entity
                {
                    // Queries

                    public const string Assert = "Directory.Memberships.MembershipDeletion.Assert";
                    public const string Retrieve = "Directory.Memberships.MembershipDeletion.Retrieve";

                    public const string Collect = "Directory.Memberships.MembershipDeletion.Collect";
                    public const string Count = "Directory.Memberships.MembershipDeletion.Count";
                    public const string Search = "Directory.Memberships.MembershipDeletion.Search";
                    public const string Download = "Directory.Memberships.MembershipDeletion.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class MembershipReason // Entity
                {
                    // Queries

                    public const string Assert = "Directory.Memberships.MembershipReason.Assert";
                    public const string Retrieve = "Directory.Memberships.MembershipReason.Retrieve";

                    public const string Collect = "Directory.Memberships.MembershipReason.Collect";
                    public const string Count = "Directory.Memberships.MembershipReason.Count";
                    public const string Search = "Directory.Memberships.MembershipReason.Search";
                    public const string Download = "Directory.Memberships.MembershipReason.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }

            public static partial class People // Subcomponent
            {
                public static partial class Person // Entity
                {
                    // Queries

                    public const string Assert = "Directory.People.Person.Assert";
                    public const string Retrieve = "Directory.People.Person.Retrieve";

                    public const string Collect = "Directory.People.Person.Collect";
                    public const string Count = "Directory.People.Person.Count";
                    public const string Search = "Directory.People.Person.Search";
                    public const string Download = "Directory.People.Person.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class PersonAddress // Entity
                {
                    // Queries

                    public const string Assert = "Directory.People.PersonAddress.Assert";
                    public const string Retrieve = "Directory.People.PersonAddress.Retrieve";

                    public const string Collect = "Directory.People.PersonAddress.Collect";
                    public const string Count = "Directory.People.PersonAddress.Count";
                    public const string Search = "Directory.People.PersonAddress.Search";
                    public const string Download = "Directory.People.PersonAddress.Download";

                    // Commands

                    public const string Create = "Directory.People.PersonAddress.Create";
                    public const string Delete = "Directory.People.PersonAddress.Delete";
                    public const string Modify = "Directory.People.PersonAddress.Modify";
                }

                public static partial class PersonField // Entity
                {
                    // Queries

                    public const string Assert = "Directory.People.PersonField.Assert";
                    public const string Retrieve = "Directory.People.PersonField.Retrieve";

                    public const string Collect = "Directory.People.PersonField.Collect";
                    public const string Count = "Directory.People.PersonField.Count";
                    public const string Search = "Directory.People.PersonField.Search";
                    public const string Download = "Directory.People.PersonField.Download";

                    // Commands

                    public const string Create = "Directory.People.PersonField.Create";
                    public const string Delete = "Directory.People.PersonField.Delete";
                    public const string Modify = "Directory.People.PersonField.Modify";
                }

                public static partial class PersonSecret // Entity
                {
                    // Queries

                    public const string Assert = "Directory.People.PersonSecret.Assert";
                    public const string Retrieve = "Directory.People.PersonSecret.Retrieve";

                    public const string Collect = "Directory.People.PersonSecret.Collect";
                    public const string Count = "Directory.People.PersonSecret.Count";
                    public const string Search = "Directory.People.PersonSecret.Search";
                    public const string Download = "Directory.People.PersonSecret.Download";

                    // Commands

                    public const string Create = "Directory.People.PersonSecret.Create";
                    public const string Delete = "Directory.People.PersonSecret.Delete";
                    public const string Modify = "Directory.People.PersonSecret.Modify";
                }
            }
        }

        public static partial class Feedback // Component
        {
            public static partial class Respondents // Subcomponent
            {
                public static partial class Respondent // Entity
                {
                    // Queries

                    public const string Assert = "Feedback.Respondents.Respondent.Assert";
                    public const string Retrieve = "Feedback.Respondents.Respondent.Retrieve";

                    public const string Collect = "Feedback.Respondents.Respondent.Collect";
                    public const string Count = "Feedback.Respondents.Respondent.Count";
                    public const string Search = "Feedback.Respondents.Respondent.Search";
                    public const string Download = "Feedback.Respondents.Respondent.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }

            public static partial class Responses // Subcomponent
            {
                public static partial class Response // Entity
                {
                    // Queries

                    public const string Assert = "Feedback.Responses.Response.Assert";
                    public const string Retrieve = "Feedback.Responses.Response.Retrieve";

                    public const string Collect = "Feedback.Responses.Response.Collect";
                    public const string Count = "Feedback.Responses.Response.Count";
                    public const string Search = "Feedback.Responses.Response.Search";
                    public const string Download = "Feedback.Responses.Response.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class ResponseAnswer // Entity
                {
                    // Queries

                    public const string Assert = "Feedback.Responses.ResponseAnswer.Assert";
                    public const string Retrieve = "Feedback.Responses.ResponseAnswer.Retrieve";

                    public const string Collect = "Feedback.Responses.ResponseAnswer.Collect";
                    public const string Count = "Feedback.Responses.ResponseAnswer.Count";
                    public const string Search = "Feedback.Responses.ResponseAnswer.Search";
                    public const string Download = "Feedback.Responses.ResponseAnswer.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class ResponseOption // Entity
                {
                    // Queries

                    public const string Assert = "Feedback.Responses.ResponseOption.Assert";
                    public const string Retrieve = "Feedback.Responses.ResponseOption.Retrieve";

                    public const string Collect = "Feedback.Responses.ResponseOption.Collect";
                    public const string Count = "Feedback.Responses.ResponseOption.Count";
                    public const string Search = "Feedback.Responses.ResponseOption.Search";
                    public const string Download = "Feedback.Responses.ResponseOption.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }

            public static partial class Surveys // Subcomponent
            {
                public static partial class Survey // Entity
                {
                    // Queries

                    public const string Assert = "Feedback.Surveys.Survey.Assert";
                    public const string Retrieve = "Feedback.Surveys.Survey.Retrieve";

                    public const string Collect = "Feedback.Surveys.Survey.Collect";
                    public const string Count = "Feedback.Surveys.Survey.Count";
                    public const string Search = "Feedback.Surveys.Survey.Search";
                    public const string Download = "Feedback.Surveys.Survey.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class SurveyCondition // Entity
                {
                    // Queries

                    public const string Assert = "Feedback.Surveys.SurveyCondition.Assert";
                    public const string Retrieve = "Feedback.Surveys.SurveyCondition.Retrieve";

                    public const string Collect = "Feedback.Surveys.SurveyCondition.Collect";
                    public const string Count = "Feedback.Surveys.SurveyCondition.Count";
                    public const string Search = "Feedback.Surveys.SurveyCondition.Search";
                    public const string Download = "Feedback.Surveys.SurveyCondition.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class SurveyOptionItem // Entity
                {
                    // Queries

                    public const string Assert = "Feedback.Surveys.SurveyOptionItem.Assert";
                    public const string Retrieve = "Feedback.Surveys.SurveyOptionItem.Retrieve";

                    public const string Collect = "Feedback.Surveys.SurveyOptionItem.Collect";
                    public const string Count = "Feedback.Surveys.SurveyOptionItem.Count";
                    public const string Search = "Feedback.Surveys.SurveyOptionItem.Search";
                    public const string Download = "Feedback.Surveys.SurveyOptionItem.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class SurveyOptionList // Entity
                {
                    // Queries

                    public const string Assert = "Feedback.Surveys.SurveyOptionList.Assert";
                    public const string Retrieve = "Feedback.Surveys.SurveyOptionList.Retrieve";

                    public const string Collect = "Feedback.Surveys.SurveyOptionList.Collect";
                    public const string Count = "Feedback.Surveys.SurveyOptionList.Count";
                    public const string Search = "Feedback.Surveys.SurveyOptionList.Search";
                    public const string Download = "Feedback.Surveys.SurveyOptionList.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class SurveyQuestion // Entity
                {
                    // Queries

                    public const string Assert = "Feedback.Surveys.SurveyQuestion.Assert";
                    public const string Retrieve = "Feedback.Surveys.SurveyQuestion.Retrieve";

                    public const string Collect = "Feedback.Surveys.SurveyQuestion.Collect";
                    public const string Count = "Feedback.Surveys.SurveyQuestion.Count";
                    public const string Search = "Feedback.Surveys.SurveyQuestion.Search";
                    public const string Download = "Feedback.Surveys.SurveyQuestion.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }
        }

        public static partial class Integration // Component
        {
            public static partial class Hub // Subcomponent
            {
                public static partial class ApiCall // Entity
                {
                    // Queries

                    public const string Assert = "Integration.Hub.ApiCall.Assert";
                    public const string Retrieve = "Integration.Hub.ApiCall.Retrieve";

                    public const string Collect = "Integration.Hub.ApiCall.Collect";
                    public const string Count = "Integration.Hub.ApiCall.Count";
                    public const string Search = "Integration.Hub.ApiCall.Search";
                    public const string Download = "Integration.Hub.ApiCall.Download";

                    // Commands

                    public const string Create = "Integration.Hub.ApiCall.Create";
                    public const string Delete = "Integration.Hub.ApiCall.Delete";
                    public const string Modify = "Integration.Hub.ApiCall.Modify";
                }
            }

            public static partial class Lti // Subcomponent
            {
                public static partial class LtiLink // Entity
                {
                    // Queries

                    public const string Assert = "Integration.Lti.LtiLink.Assert";
                    public const string Retrieve = "Integration.Lti.LtiLink.Retrieve";

                    public const string Collect = "Integration.Lti.LtiLink.Collect";
                    public const string Count = "Integration.Lti.LtiLink.Count";
                    public const string Search = "Integration.Lti.LtiLink.Search";
                    public const string Download = "Integration.Lti.LtiLink.Download";

                    // Commands

                    public const string Create = "Integration.Lti.LtiLink.Create";
                    public const string Delete = "Integration.Lti.LtiLink.Delete";
                    public const string Modify = "Integration.Lti.LtiLink.Modify";
                }
            }

            public static partial class Moodle // Subcomponent
            {
                public static partial class MoodleChange // Entity
                {
                    // Queries

                    public const string Assert = "Integration.Moodle.MoodleChange.Assert";
                    public const string Retrieve = "Integration.Moodle.MoodleChange.Retrieve";

                    public const string Collect = "Integration.Moodle.MoodleChange.Collect";
                    public const string Count = "Integration.Moodle.MoodleChange.Count";
                    public const string Search = "Integration.Moodle.MoodleChange.Search";
                    public const string Download = "Integration.Moodle.MoodleChange.Download";

                    // Commands

                    public const string Create = "Integration.Moodle.MoodleChange.Create";
                    public const string Delete = "Integration.Moodle.MoodleChange.Delete";
                    public const string Modify = "Integration.Moodle.MoodleChange.Modify";
                }
            }

            public static partial class Scorm // Subcomponent
            {
                public static partial class ScormChange // Entity
                {
                    // Queries

                    public const string Assert = "Integration.Scorm.ScormChange.Assert";
                    public const string Retrieve = "Integration.Scorm.ScormChange.Retrieve";

                    public const string Collect = "Integration.Scorm.ScormChange.Collect";
                    public const string Count = "Integration.Scorm.ScormChange.Count";
                    public const string Search = "Integration.Scorm.ScormChange.Search";
                    public const string Download = "Integration.Scorm.ScormChange.Download";

                    // Commands

                    public const string Create = "Integration.Scorm.ScormChange.Create";
                    public const string Delete = "Integration.Scorm.ScormChange.Delete";
                    public const string Modify = "Integration.Scorm.ScormChange.Modify";
                }

                public static partial class ScormRegistration // Entity
                {
                    // Queries

                    public const string Assert = "Integration.Scorm.ScormRegistration.Assert";
                    public const string Retrieve = "Integration.Scorm.ScormRegistration.Retrieve";

                    public const string Collect = "Integration.Scorm.ScormRegistration.Collect";
                    public const string Count = "Integration.Scorm.ScormRegistration.Count";
                    public const string Search = "Integration.Scorm.ScormRegistration.Search";
                    public const string Download = "Integration.Scorm.ScormRegistration.Download";

                    // Commands

                    public const string Create = "Integration.Scorm.ScormRegistration.Create";
                    public const string Delete = "Integration.Scorm.ScormRegistration.Delete";
                    public const string Modify = "Integration.Scorm.ScormRegistration.Modify";
                }

                public static partial class ScormRegistrationActivity // Entity
                {
                    // Queries

                    public const string Assert = "Integration.Scorm.ScormRegistrationActivity.Assert";
                    public const string Retrieve = "Integration.Scorm.ScormRegistrationActivity.Retrieve";

                    public const string Collect = "Integration.Scorm.ScormRegistrationActivity.Collect";
                    public const string Count = "Integration.Scorm.ScormRegistrationActivity.Count";
                    public const string Search = "Integration.Scorm.ScormRegistrationActivity.Search";
                    public const string Download = "Integration.Scorm.ScormRegistrationActivity.Download";

                    // Commands

                    public const string Create = "Integration.Scorm.ScormRegistrationActivity.Create";
                    public const string Delete = "Integration.Scorm.ScormRegistrationActivity.Delete";
                    public const string Modify = "Integration.Scorm.ScormRegistrationActivity.Modify";
                }
            }

            public static partial class Xapi // Subcomponent
            {
                public static partial class XapiChange // Entity
                {
                    // Queries

                    public const string Assert = "Integration.Xapi.XapiChange.Assert";
                    public const string Retrieve = "Integration.Xapi.XapiChange.Retrieve";

                    public const string Collect = "Integration.Xapi.XapiChange.Collect";
                    public const string Count = "Integration.Xapi.XapiChange.Count";
                    public const string Search = "Integration.Xapi.XapiChange.Search";
                    public const string Download = "Integration.Xapi.XapiChange.Download";

                    // Commands

                    public const string Create = "Integration.Xapi.XapiChange.Create";
                    public const string Delete = "Integration.Xapi.XapiChange.Delete";
                    public const string Modify = "Integration.Xapi.XapiChange.Modify";
                }
            }
        }

        public static partial class Job // Component
        {
            public static partial class Applicants // Subcomponent
            {
                public static partial class Applicant // Entity
                {
                    // Queries

                    public const string Assert = "Job.Applicants.Applicant.Assert";
                    public const string Retrieve = "Job.Applicants.Applicant.Retrieve";

                    public const string Collect = "Job.Applicants.Applicant.Collect";
                    public const string Count = "Job.Applicants.Applicant.Count";
                    public const string Search = "Job.Applicants.Applicant.Search";
                    public const string Download = "Job.Applicants.Applicant.Download";

                    // Commands

                    public const string Create = "Job.Applicants.Applicant.Create";
                    public const string Delete = "Job.Applicants.Applicant.Delete";
                    public const string Modify = "Job.Applicants.Applicant.Modify";
                }
            }

            public static partial class Candidates // Subcomponent
            {
                public static partial class CandidateEducation // Entity
                {
                    // Queries

                    public const string Assert = "Job.Candidates.CandidateEducation.Assert";
                    public const string Retrieve = "Job.Candidates.CandidateEducation.Retrieve";

                    public const string Collect = "Job.Candidates.CandidateEducation.Collect";
                    public const string Count = "Job.Candidates.CandidateEducation.Count";
                    public const string Search = "Job.Candidates.CandidateEducation.Search";
                    public const string Download = "Job.Candidates.CandidateEducation.Download";

                    // Commands

                    public const string Create = "Job.Candidates.CandidateEducation.Create";
                    public const string Delete = "Job.Candidates.CandidateEducation.Delete";
                    public const string Modify = "Job.Candidates.CandidateEducation.Modify";
                }

                public static partial class CandidateExperience // Entity
                {
                    // Queries

                    public const string Assert = "Job.Candidates.CandidateExperience.Assert";
                    public const string Retrieve = "Job.Candidates.CandidateExperience.Retrieve";

                    public const string Collect = "Job.Candidates.CandidateExperience.Collect";
                    public const string Count = "Job.Candidates.CandidateExperience.Count";
                    public const string Search = "Job.Candidates.CandidateExperience.Search";
                    public const string Download = "Job.Candidates.CandidateExperience.Download";

                    // Commands

                    public const string Create = "Job.Candidates.CandidateExperience.Create";
                    public const string Delete = "Job.Candidates.CandidateExperience.Delete";
                    public const string Modify = "Job.Candidates.CandidateExperience.Modify";
                }

                public static partial class CandidateExperienceItem // Entity
                {
                    // Queries

                    public const string Assert = "Job.Candidates.CandidateExperienceItem.Assert";
                    public const string Retrieve = "Job.Candidates.CandidateExperienceItem.Retrieve";

                    public const string Collect = "Job.Candidates.CandidateExperienceItem.Collect";
                    public const string Count = "Job.Candidates.CandidateExperienceItem.Count";
                    public const string Search = "Job.Candidates.CandidateExperienceItem.Search";
                    public const string Download = "Job.Candidates.CandidateExperienceItem.Download";

                    // Commands

                    public const string Create = "Job.Candidates.CandidateExperienceItem.Create";
                    public const string Delete = "Job.Candidates.CandidateExperienceItem.Delete";
                    public const string Modify = "Job.Candidates.CandidateExperienceItem.Modify";
                }

                public static partial class CandidateFile // Entity
                {
                    // Queries

                    public const string Assert = "Job.Candidates.CandidateFile.Assert";
                    public const string Retrieve = "Job.Candidates.CandidateFile.Retrieve";

                    public const string Collect = "Job.Candidates.CandidateFile.Collect";
                    public const string Count = "Job.Candidates.CandidateFile.Count";
                    public const string Search = "Job.Candidates.CandidateFile.Search";
                    public const string Download = "Job.Candidates.CandidateFile.Download";

                    // Commands

                    public const string Create = "Job.Candidates.CandidateFile.Create";
                    public const string Delete = "Job.Candidates.CandidateFile.Delete";
                    public const string Modify = "Job.Candidates.CandidateFile.Modify";
                }

                public static partial class CandidateLanguageProficiency // Entity
                {
                    // Queries

                    public const string Assert = "Job.Candidates.CandidateLanguageProficiency.Assert";
                    public const string Retrieve = "Job.Candidates.CandidateLanguageProficiency.Retrieve";

                    public const string Collect = "Job.Candidates.CandidateLanguageProficiency.Collect";
                    public const string Count = "Job.Candidates.CandidateLanguageProficiency.Count";
                    public const string Search = "Job.Candidates.CandidateLanguageProficiency.Search";
                    public const string Download = "Job.Candidates.CandidateLanguageProficiency.Download";

                    // Commands

                    public const string Create = "Job.Candidates.CandidateLanguageProficiency.Create";
                    public const string Delete = "Job.Candidates.CandidateLanguageProficiency.Delete";
                    public const string Modify = "Job.Candidates.CandidateLanguageProficiency.Modify";
                }
            }

            public static partial class Opportunities // Subcomponent
            {
                public static partial class Opportunity // Entity
                {
                    // Queries

                    public const string Assert = "Job.Opportunities.Opportunity.Assert";
                    public const string Retrieve = "Job.Opportunities.Opportunity.Retrieve";

                    public const string Collect = "Job.Opportunities.Opportunity.Collect";
                    public const string Count = "Job.Opportunities.Opportunity.Count";
                    public const string Search = "Job.Opportunities.Opportunity.Search";
                    public const string Download = "Job.Opportunities.Opportunity.Download";

                    // Commands

                    public const string Create = "Job.Opportunities.Opportunity.Create";
                    public const string Delete = "Job.Opportunities.Opportunity.Delete";
                    public const string Modify = "Job.Opportunities.Opportunity.Modify";
                }

                public static partial class OpportunityCategory // Entity
                {
                    // Queries

                    public const string Assert = "Job.Opportunities.OpportunityCategory.Assert";
                    public const string Retrieve = "Job.Opportunities.OpportunityCategory.Retrieve";

                    public const string Collect = "Job.Opportunities.OpportunityCategory.Collect";
                    public const string Count = "Job.Opportunities.OpportunityCategory.Count";
                    public const string Search = "Job.Opportunities.OpportunityCategory.Search";
                    public const string Download = "Job.Opportunities.OpportunityCategory.Download";

                    // Commands

                    public const string Create = "Job.Opportunities.OpportunityCategory.Create";
                    public const string Delete = "Job.Opportunities.OpportunityCategory.Delete";
                    public const string Modify = "Job.Opportunities.OpportunityCategory.Modify";
                }
            }
        }

        public static partial class Learning // Component
        {
            public static partial class Activities // Subcomponent
            {
                public static partial class Activity // Entity
                {
                    // Queries

                    public const string Assert = "Learning.Activities.Activity.Assert";
                    public const string Retrieve = "Learning.Activities.Activity.Retrieve";

                    public const string Collect = "Learning.Activities.Activity.Collect";
                    public const string Count = "Learning.Activities.Activity.Count";
                    public const string Search = "Learning.Activities.Activity.Search";
                    public const string Download = "Learning.Activities.Activity.Download";

                    // Commands

                    public const string Create = "Learning.Activities.Activity.Create";
                    public const string Delete = "Learning.Activities.Activity.Delete";
                    public const string Modify = "Learning.Activities.Activity.Modify";
                }

                public static partial class ActivityCompetency // Entity
                {
                    // Queries

                    public const string Assert = "Learning.Activities.ActivityCompetency.Assert";
                    public const string Retrieve = "Learning.Activities.ActivityCompetency.Retrieve";

                    public const string Collect = "Learning.Activities.ActivityCompetency.Collect";
                    public const string Count = "Learning.Activities.ActivityCompetency.Count";
                    public const string Search = "Learning.Activities.ActivityCompetency.Search";
                    public const string Download = "Learning.Activities.ActivityCompetency.Download";

                    // Commands

                    public const string Create = "Learning.Activities.ActivityCompetency.Create";
                    public const string Delete = "Learning.Activities.ActivityCompetency.Delete";
                    public const string Modify = "Learning.Activities.ActivityCompetency.Modify";
                }
            }

            public static partial class Catalogs // Subcomponent
            {
                public static partial class Catalog // Entity
                {
                    // Queries

                    public const string Assert = "Learning.Catalogs.Catalog.Assert";
                    public const string Retrieve = "Learning.Catalogs.Catalog.Retrieve";

                    public const string Collect = "Learning.Catalogs.Catalog.Collect";
                    public const string Count = "Learning.Catalogs.Catalog.Count";
                    public const string Search = "Learning.Catalogs.Catalog.Search";
                    public const string Download = "Learning.Catalogs.Catalog.Download";

                    // Commands

                    public const string Create = "Learning.Catalogs.Catalog.Create";
                    public const string Delete = "Learning.Catalogs.Catalog.Delete";
                    public const string Modify = "Learning.Catalogs.Catalog.Modify";
                }
            }

            public static partial class Courses // Subcomponent
            {
                public static partial class Course // Entity
                {
                    // Queries

                    public const string Assert = "Learning.Courses.Course.Assert";
                    public const string Retrieve = "Learning.Courses.Course.Retrieve";

                    public const string Collect = "Learning.Courses.Course.Collect";
                    public const string Count = "Learning.Courses.Course.Count";
                    public const string Search = "Learning.Courses.Course.Search";
                    public const string Download = "Learning.Courses.Course.Download";

                    // Commands

                    public const string Create = "Learning.Courses.Course.Create";
                    public const string Delete = "Learning.Courses.Course.Delete";
                    public const string Modify = "Learning.Courses.Course.Modify";
                }

                public static partial class CourseCategory // Entity
                {
                    // Queries

                    public const string Assert = "Learning.Courses.CourseCategory.Assert";
                    public const string Retrieve = "Learning.Courses.CourseCategory.Retrieve";

                    public const string Collect = "Learning.Courses.CourseCategory.Collect";
                    public const string Count = "Learning.Courses.CourseCategory.Count";
                    public const string Search = "Learning.Courses.CourseCategory.Search";
                    public const string Download = "Learning.Courses.CourseCategory.Download";

                    // Commands

                    public const string Create = "Learning.Courses.CourseCategory.Create";
                    public const string Delete = "Learning.Courses.CourseCategory.Delete";
                    public const string Modify = "Learning.Courses.CourseCategory.Modify";
                }
            }

            public static partial class Enrollments // Subcomponent
            {
                public static partial class CourseEnrollment // Entity
                {
                    // Queries

                    public const string Assert = "Learning.Enrollments.CourseEnrollment.Assert";
                    public const string Retrieve = "Learning.Enrollments.CourseEnrollment.Retrieve";

                    public const string Collect = "Learning.Enrollments.CourseEnrollment.Collect";
                    public const string Count = "Learning.Enrollments.CourseEnrollment.Count";
                    public const string Search = "Learning.Enrollments.CourseEnrollment.Search";
                    public const string Download = "Learning.Enrollments.CourseEnrollment.Download";

                    // Commands

                    public const string Create = "Learning.Enrollments.CourseEnrollment.Create";
                    public const string Delete = "Learning.Enrollments.CourseEnrollment.Delete";
                    public const string Modify = "Learning.Enrollments.CourseEnrollment.Modify";
                }

                public static partial class ProgramEnrollment // Entity
                {
                    // Queries

                    public const string Assert = "Learning.Enrollments.ProgramEnrollment.Assert";
                    public const string Retrieve = "Learning.Enrollments.ProgramEnrollment.Retrieve";

                    public const string Collect = "Learning.Enrollments.ProgramEnrollment.Collect";
                    public const string Count = "Learning.Enrollments.ProgramEnrollment.Count";
                    public const string Search = "Learning.Enrollments.ProgramEnrollment.Search";
                    public const string Download = "Learning.Enrollments.ProgramEnrollment.Download";

                    // Commands

                    public const string Create = "Learning.Enrollments.ProgramEnrollment.Create";
                    public const string Delete = "Learning.Enrollments.ProgramEnrollment.Delete";
                    public const string Modify = "Learning.Enrollments.ProgramEnrollment.Modify";
                }

                public static partial class TaskEnrollment // Entity
                {
                    // Queries

                    public const string Assert = "Learning.Enrollments.TaskEnrollment.Assert";
                    public const string Retrieve = "Learning.Enrollments.TaskEnrollment.Retrieve";

                    public const string Collect = "Learning.Enrollments.TaskEnrollment.Collect";
                    public const string Count = "Learning.Enrollments.TaskEnrollment.Count";
                    public const string Search = "Learning.Enrollments.TaskEnrollment.Search";
                    public const string Download = "Learning.Enrollments.TaskEnrollment.Download";

                    // Commands

                    public const string Create = "Learning.Enrollments.TaskEnrollment.Create";
                    public const string Delete = "Learning.Enrollments.TaskEnrollment.Delete";
                    public const string Modify = "Learning.Enrollments.TaskEnrollment.Modify";
                }
            }

            public static partial class Modules // Subcomponent
            {
                public static partial class Module // Entity
                {
                    // Queries

                    public const string Assert = "Learning.Modules.Module.Assert";
                    public const string Retrieve = "Learning.Modules.Module.Retrieve";

                    public const string Collect = "Learning.Modules.Module.Collect";
                    public const string Count = "Learning.Modules.Module.Count";
                    public const string Search = "Learning.Modules.Module.Search";
                    public const string Download = "Learning.Modules.Module.Download";

                    // Commands

                    public const string Create = "Learning.Modules.Module.Create";
                    public const string Delete = "Learning.Modules.Module.Delete";
                    public const string Modify = "Learning.Modules.Module.Modify";
                }
            }

            public static partial class Prerequisites // Subcomponent
            {
                public static partial class CoursePrerequisite // Entity
                {
                    // Queries

                    public const string Assert = "Learning.Prerequisites.CoursePrerequisite.Assert";
                    public const string Retrieve = "Learning.Prerequisites.CoursePrerequisite.Retrieve";

                    public const string Collect = "Learning.Prerequisites.CoursePrerequisite.Collect";
                    public const string Count = "Learning.Prerequisites.CoursePrerequisite.Count";
                    public const string Search = "Learning.Prerequisites.CoursePrerequisite.Search";
                    public const string Download = "Learning.Prerequisites.CoursePrerequisite.Download";

                    // Commands

                    public const string Create = "Learning.Prerequisites.CoursePrerequisite.Create";
                    public const string Delete = "Learning.Prerequisites.CoursePrerequisite.Delete";
                    public const string Modify = "Learning.Prerequisites.CoursePrerequisite.Modify";
                }

                public static partial class Prerequisite // Entity
                {
                    // Queries

                    public const string Assert = "Learning.Prerequisites.Prerequisite.Assert";
                    public const string Retrieve = "Learning.Prerequisites.Prerequisite.Retrieve";

                    public const string Collect = "Learning.Prerequisites.Prerequisite.Collect";
                    public const string Count = "Learning.Prerequisites.Prerequisite.Count";
                    public const string Search = "Learning.Prerequisites.Prerequisite.Search";
                    public const string Download = "Learning.Prerequisites.Prerequisite.Download";

                    // Commands

                    public const string Create = "Learning.Prerequisites.Prerequisite.Create";
                    public const string Delete = "Learning.Prerequisites.Prerequisite.Delete";
                    public const string Modify = "Learning.Prerequisites.Prerequisite.Modify";
                }
            }

            public static partial class Programs // Subcomponent
            {
                public static partial class Program // Entity
                {
                    // Queries

                    public const string Assert = "Learning.Programs.Program.Assert";
                    public const string Retrieve = "Learning.Programs.Program.Retrieve";

                    public const string Collect = "Learning.Programs.Program.Collect";
                    public const string Count = "Learning.Programs.Program.Count";
                    public const string Search = "Learning.Programs.Program.Search";
                    public const string Download = "Learning.Programs.Program.Download";

                    // Commands

                    public const string Create = "Learning.Programs.Program.Create";
                    public const string Delete = "Learning.Programs.Program.Delete";
                    public const string Modify = "Learning.Programs.Program.Modify";
                }

                public static partial class Task // Entity
                {
                    // Queries

                    public const string Assert = "Learning.Programs.Task.Assert";
                    public const string Retrieve = "Learning.Programs.Task.Retrieve";

                    public const string Collect = "Learning.Programs.Task.Collect";
                    public const string Count = "Learning.Programs.Task.Count";
                    public const string Search = "Learning.Programs.Task.Search";
                    public const string Download = "Learning.Programs.Task.Download";

                    // Commands

                    public const string Create = "Learning.Programs.Task.Create";
                    public const string Delete = "Learning.Programs.Task.Delete";
                    public const string Modify = "Learning.Programs.Task.Modify";
                }
            }

            public static partial class Units // Subcomponent
            {
                public static partial class Unit // Entity
                {
                    // Queries

                    public const string Assert = "Learning.Units.Unit.Assert";
                    public const string Retrieve = "Learning.Units.Unit.Retrieve";

                    public const string Collect = "Learning.Units.Unit.Collect";
                    public const string Count = "Learning.Units.Unit.Count";
                    public const string Search = "Learning.Units.Unit.Search";
                    public const string Download = "Learning.Units.Unit.Download";

                    // Commands

                    public const string Create = "Learning.Units.Unit.Create";
                    public const string Delete = "Learning.Units.Unit.Delete";
                    public const string Modify = "Learning.Units.Unit.Modify";
                }
            }
        }

        public static partial class Messaging // Component
        {
            public static partial class Mailouts // Subcomponent
            {
                public static partial class Mailout // Entity
                {
                    // Queries

                    public const string Assert = "Messaging.Mailouts.Mailout.Assert";
                    public const string Retrieve = "Messaging.Mailouts.Mailout.Retrieve";

                    public const string Collect = "Messaging.Mailouts.Mailout.Collect";
                    public const string Count = "Messaging.Mailouts.Mailout.Count";
                    public const string Search = "Messaging.Mailouts.Mailout.Search";
                    public const string Download = "Messaging.Mailouts.Mailout.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class MailoutClick // Entity
                {
                    // Queries

                    public const string Assert = "Messaging.Mailouts.MailoutClick.Assert";
                    public const string Retrieve = "Messaging.Mailouts.MailoutClick.Retrieve";

                    public const string Collect = "Messaging.Mailouts.MailoutClick.Collect";
                    public const string Count = "Messaging.Mailouts.MailoutClick.Count";
                    public const string Search = "Messaging.Mailouts.MailoutClick.Search";
                    public const string Download = "Messaging.Mailouts.MailoutClick.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class MailoutRecipient // Entity
                {
                    // Queries

                    public const string Assert = "Messaging.Mailouts.MailoutRecipient.Assert";
                    public const string Retrieve = "Messaging.Mailouts.MailoutRecipient.Retrieve";

                    public const string Collect = "Messaging.Mailouts.MailoutRecipient.Collect";
                    public const string Count = "Messaging.Mailouts.MailoutRecipient.Count";
                    public const string Search = "Messaging.Mailouts.MailoutRecipient.Search";
                    public const string Download = "Messaging.Mailouts.MailoutRecipient.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class MailoutRecipientCopy // Entity
                {
                    // Queries

                    public const string Assert = "Messaging.Mailouts.MailoutRecipientCopy.Assert";
                    public const string Retrieve = "Messaging.Mailouts.MailoutRecipientCopy.Retrieve";

                    public const string Collect = "Messaging.Mailouts.MailoutRecipientCopy.Collect";
                    public const string Count = "Messaging.Mailouts.MailoutRecipientCopy.Count";
                    public const string Search = "Messaging.Mailouts.MailoutRecipientCopy.Search";
                    public const string Download = "Messaging.Mailouts.MailoutRecipientCopy.Download";

                    // Commands

                    public const string Create = "Messaging.Mailouts.MailoutRecipientCopy.Create";
                    public const string Delete = "Messaging.Mailouts.MailoutRecipientCopy.Delete";
                    public const string Modify = "Messaging.Mailouts.MailoutRecipientCopy.Modify";
                }
            }

            public static partial class Messages // Subcomponent
            {
                public static partial class Message // Entity
                {
                    // Queries

                    public const string Assert = "Messaging.Messages.Message.Assert";
                    public const string Retrieve = "Messaging.Messages.Message.Retrieve";

                    public const string Collect = "Messaging.Messages.Message.Collect";
                    public const string Count = "Messaging.Messages.Message.Count";
                    public const string Search = "Messaging.Messages.Message.Search";
                    public const string Download = "Messaging.Messages.Message.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class MessageLink // Entity
                {
                    // Queries

                    public const string Assert = "Messaging.Messages.MessageLink.Assert";
                    public const string Retrieve = "Messaging.Messages.MessageLink.Retrieve";

                    public const string Collect = "Messaging.Messages.MessageLink.Collect";
                    public const string Count = "Messaging.Messages.MessageLink.Count";
                    public const string Search = "Messaging.Messages.MessageLink.Search";
                    public const string Download = "Messaging.Messages.MessageLink.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }

            public static partial class Subscribers // Subcomponent
            {
                public static partial class SubscriberFollower // Entity
                {
                    // Queries

                    public const string Assert = "Messaging.Subscribers.SubscriberFollower.Assert";
                    public const string Retrieve = "Messaging.Subscribers.SubscriberFollower.Retrieve";

                    public const string Collect = "Messaging.Subscribers.SubscriberFollower.Collect";
                    public const string Count = "Messaging.Subscribers.SubscriberFollower.Count";
                    public const string Search = "Messaging.Subscribers.SubscriberFollower.Search";
                    public const string Download = "Messaging.Subscribers.SubscriberFollower.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class SubscriberGroup // Entity
                {
                    // Queries

                    public const string Assert = "Messaging.Subscribers.SubscriberGroup.Assert";
                    public const string Retrieve = "Messaging.Subscribers.SubscriberGroup.Retrieve";

                    public const string Collect = "Messaging.Subscribers.SubscriberGroup.Collect";
                    public const string Count = "Messaging.Subscribers.SubscriberGroup.Count";
                    public const string Search = "Messaging.Subscribers.SubscriberGroup.Search";
                    public const string Download = "Messaging.Subscribers.SubscriberGroup.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class SubscriberUser // Entity
                {
                    // Queries

                    public const string Assert = "Messaging.Subscribers.SubscriberUser.Assert";
                    public const string Retrieve = "Messaging.Subscribers.SubscriberUser.Retrieve";

                    public const string Collect = "Messaging.Subscribers.SubscriberUser.Collect";
                    public const string Count = "Messaging.Subscribers.SubscriberUser.Count";
                    public const string Search = "Messaging.Subscribers.SubscriberUser.Search";
                    public const string Download = "Messaging.Subscribers.SubscriberUser.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }
        }

        public static partial class Metadata // Component
        {
            public static partial class Entities // Subcomponent
            {
                public static partial class Entity // Entity
                {
                    // Queries

                    public const string Assert = "Metadata.Entities.Entity.Assert";
                    public const string Retrieve = "Metadata.Entities.Entity.Retrieve";

                    public const string Collect = "Metadata.Entities.Entity.Collect";
                    public const string Count = "Metadata.Entities.Entity.Count";
                    public const string Search = "Metadata.Entities.Entity.Search";
                    public const string Download = "Metadata.Entities.Entity.Download";

                    // Commands

                    public const string Create = "Metadata.Entities.Entity.Create";
                    public const string Delete = "Metadata.Entities.Entity.Delete";
                    public const string Modify = "Metadata.Entities.Entity.Modify";
                }
            }

            public static partial class Sequences // Subcomponent
            {
                public static partial class Sequence // Entity
                {
                    // Queries

                    public const string Assert = "Metadata.Sequences.Sequence.Assert";
                    public const string Retrieve = "Metadata.Sequences.Sequence.Retrieve";

                    public const string Collect = "Metadata.Sequences.Sequence.Collect";
                    public const string Count = "Metadata.Sequences.Sequence.Count";
                    public const string Search = "Metadata.Sequences.Sequence.Search";
                    public const string Download = "Metadata.Sequences.Sequence.Download";

                    // Commands

                    public const string Create = "Metadata.Sequences.Sequence.Create";
                    public const string Delete = "Metadata.Sequences.Sequence.Delete";
                    public const string Modify = "Metadata.Sequences.Sequence.Modify";
                }
            }

            public static partial class Upgrades // Subcomponent
            {
                public static partial class Upgrade // Entity
                {
                    // Queries

                    public const string Assert = "Metadata.Upgrades.Upgrade.Assert";
                    public const string Retrieve = "Metadata.Upgrades.Upgrade.Retrieve";

                    public const string Collect = "Metadata.Upgrades.Upgrade.Collect";
                    public const string Count = "Metadata.Upgrades.Upgrade.Count";
                    public const string Search = "Metadata.Upgrades.Upgrade.Search";
                    public const string Download = "Metadata.Upgrades.Upgrade.Download";

                    // Commands

                    public const string Create = "Metadata.Upgrades.Upgrade.Create";
                    public const string Delete = "Metadata.Upgrades.Upgrade.Delete";
                    public const string Modify = "Metadata.Upgrades.Upgrade.Modify";
                }
            }
        }

        public static partial class Platform // Component
        {
            public static partial class Lookups // Subcomponent
            {
                public static partial class LookupItem // Entity
                {
                    // Queries

                    public const string Assert = "Platform.Lookups.LookupItem.Assert";
                    public const string Retrieve = "Platform.Lookups.LookupItem.Retrieve";

                    public const string Collect = "Platform.Lookups.LookupItem.Collect";
                    public const string Count = "Platform.Lookups.LookupItem.Count";
                    public const string Search = "Platform.Lookups.LookupItem.Search";
                    public const string Download = "Platform.Lookups.LookupItem.Download";

                    // Commands

                    public const string Create = "Platform.Lookups.LookupItem.Create";
                    public const string Delete = "Platform.Lookups.LookupItem.Delete";
                    public const string Modify = "Platform.Lookups.LookupItem.Modify";
                }

                public static partial class LookupList // Entity
                {
                    // Queries

                    public const string Assert = "Platform.Lookups.LookupList.Assert";
                    public const string Retrieve = "Platform.Lookups.LookupList.Retrieve";

                    public const string Collect = "Platform.Lookups.LookupList.Collect";
                    public const string Count = "Platform.Lookups.LookupList.Count";
                    public const string Search = "Platform.Lookups.LookupList.Search";
                    public const string Download = "Platform.Lookups.LookupList.Download";

                    // Commands

                    public const string Create = "Platform.Lookups.LookupList.Create";
                    public const string Delete = "Platform.Lookups.LookupList.Delete";
                    public const string Modify = "Platform.Lookups.LookupList.Modify";
                }
            }

            public static partial class Partitions // Subcomponent
            {
                public static partial class PartitionField // Entity
                {
                    // Queries

                    public const string Assert = "Platform.Partitions.PartitionField.Assert";
                    public const string Retrieve = "Platform.Partitions.PartitionField.Retrieve";

                    public const string Collect = "Platform.Partitions.PartitionField.Collect";
                    public const string Count = "Platform.Partitions.PartitionField.Count";
                    public const string Search = "Platform.Partitions.PartitionField.Search";
                    public const string Download = "Platform.Partitions.PartitionField.Download";

                    // Commands

                    public const string Create = "Platform.Partitions.PartitionField.Create";
                    public const string Delete = "Platform.Partitions.PartitionField.Delete";
                    public const string Modify = "Platform.Partitions.PartitionField.Modify";
                }
            }

            public static partial class Routes // Subcomponent
            {
                public static partial class Action // Entity
                {
                    // Queries

                    public const string Assert = "Platform.Routes.Action.Assert";
                    public const string Retrieve = "Platform.Routes.Action.Retrieve";

                    public const string Collect = "Platform.Routes.Action.Collect";
                    public const string Count = "Platform.Routes.Action.Count";
                    public const string Search = "Platform.Routes.Action.Search";
                    public const string Download = "Platform.Routes.Action.Download";

                    // Commands

                    public const string Create = "Platform.Routes.Action.Create";
                    public const string Delete = "Platform.Routes.Action.Delete";
                    public const string Modify = "Platform.Routes.Action.Modify";
                }
            }

            public static partial class Senders // Subcomponent
            {
                public static partial class Sender // Entity
                {
                    // Queries

                    public const string Assert = "Platform.Senders.Sender.Assert";
                    public const string Retrieve = "Platform.Senders.Sender.Retrieve";

                    public const string Collect = "Platform.Senders.Sender.Collect";
                    public const string Count = "Platform.Senders.Sender.Count";
                    public const string Search = "Platform.Senders.Sender.Search";
                    public const string Download = "Platform.Senders.Sender.Download";

                    // Commands

                    public const string Create = "Platform.Senders.Sender.Create";
                    public const string Delete = "Platform.Senders.Sender.Delete";
                    public const string Modify = "Platform.Senders.Sender.Modify";
                }

                public static partial class SenderOrganization // Entity
                {
                    // Queries

                    public const string Assert = "Platform.Senders.SenderOrganization.Assert";
                    public const string Retrieve = "Platform.Senders.SenderOrganization.Retrieve";

                    public const string Collect = "Platform.Senders.SenderOrganization.Collect";
                    public const string Count = "Platform.Senders.SenderOrganization.Count";
                    public const string Search = "Platform.Senders.SenderOrganization.Search";
                    public const string Download = "Platform.Senders.SenderOrganization.Download";

                    // Commands

                    public const string Create = "Platform.Senders.SenderOrganization.Create";
                    public const string Delete = "Platform.Senders.SenderOrganization.Delete";
                    public const string Modify = "Platform.Senders.SenderOrganization.Modify";
                }
            }
        }

        public static partial class Progress // Component
        {
            public static partial class Achievements // Subcomponent
            {
                public static partial class Achievement // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Achievements.Achievement.Assert";
                    public const string Retrieve = "Progress.Achievements.Achievement.Retrieve";

                    public const string Collect = "Progress.Achievements.Achievement.Collect";
                    public const string Count = "Progress.Achievements.Achievement.Count";
                    public const string Search = "Progress.Achievements.Achievement.Search";
                    public const string Download = "Progress.Achievements.Achievement.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class AchievementCategory // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Achievements.AchievementCategory.Assert";
                    public const string Retrieve = "Progress.Achievements.AchievementCategory.Retrieve";

                    public const string Collect = "Progress.Achievements.AchievementCategory.Collect";
                    public const string Count = "Progress.Achievements.AchievementCategory.Count";
                    public const string Search = "Progress.Achievements.AchievementCategory.Search";
                    public const string Download = "Progress.Achievements.AchievementCategory.Download";

                    // Commands

                    public const string Create = "Progress.Achievements.AchievementCategory.Create";
                    public const string Delete = "Progress.Achievements.AchievementCategory.Delete";
                    public const string Modify = "Progress.Achievements.AchievementCategory.Modify";
                }

                public static partial class AchievementGroup // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Achievements.AchievementGroup.Assert";
                    public const string Retrieve = "Progress.Achievements.AchievementGroup.Retrieve";

                    public const string Collect = "Progress.Achievements.AchievementGroup.Collect";
                    public const string Count = "Progress.Achievements.AchievementGroup.Count";
                    public const string Search = "Progress.Achievements.AchievementGroup.Search";
                    public const string Download = "Progress.Achievements.AchievementGroup.Download";

                    // Commands

                    public const string Create = "Progress.Achievements.AchievementGroup.Create";
                    public const string Delete = "Progress.Achievements.AchievementGroup.Delete";
                    public const string Modify = "Progress.Achievements.AchievementGroup.Modify";
                }

                public static partial class AchievementOrganization // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Achievements.AchievementOrganization.Assert";
                    public const string Retrieve = "Progress.Achievements.AchievementOrganization.Retrieve";

                    public const string Collect = "Progress.Achievements.AchievementOrganization.Collect";
                    public const string Count = "Progress.Achievements.AchievementOrganization.Count";
                    public const string Search = "Progress.Achievements.AchievementOrganization.Search";
                    public const string Download = "Progress.Achievements.AchievementOrganization.Download";

                    // Commands

                    public const string Create = "Progress.Achievements.AchievementOrganization.Create";
                    public const string Delete = "Progress.Achievements.AchievementOrganization.Delete";
                    public const string Modify = "Progress.Achievements.AchievementOrganization.Modify";
                }

                public static partial class AchievementPrerequisite // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Achievements.AchievementPrerequisite.Assert";
                    public const string Retrieve = "Progress.Achievements.AchievementPrerequisite.Retrieve";

                    public const string Collect = "Progress.Achievements.AchievementPrerequisite.Collect";
                    public const string Count = "Progress.Achievements.AchievementPrerequisite.Count";
                    public const string Search = "Progress.Achievements.AchievementPrerequisite.Search";
                    public const string Download = "Progress.Achievements.AchievementPrerequisite.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }

            public static partial class Certificates // Subcomponent
            {
                public static partial class Certificate // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Certificates.Certificate.Assert";
                    public const string Retrieve = "Progress.Certificates.Certificate.Retrieve";

                    public const string Collect = "Progress.Certificates.Certificate.Collect";
                    public const string Count = "Progress.Certificates.Certificate.Count";
                    public const string Search = "Progress.Certificates.Certificate.Search";
                    public const string Download = "Progress.Certificates.Certificate.Download";

                    // Commands

                    public const string Create = "Progress.Certificates.Certificate.Create";
                    public const string Delete = "Progress.Certificates.Certificate.Delete";
                    public const string Modify = "Progress.Certificates.Certificate.Modify";
                }
            }

            public static partial class Credentials // Subcomponent
            {
                public static partial class Credential // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Credentials.Credential.Assert";
                    public const string Retrieve = "Progress.Credentials.Credential.Retrieve";

                    public const string Collect = "Progress.Credentials.Credential.Collect";
                    public const string Count = "Progress.Credentials.Credential.Count";
                    public const string Search = "Progress.Credentials.Credential.Search";
                    public const string Download = "Progress.Credentials.Credential.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class CredentialChange // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Credentials.CredentialChange.Assert";
                    public const string Retrieve = "Progress.Credentials.CredentialChange.Retrieve";

                    public const string Collect = "Progress.Credentials.CredentialChange.Collect";
                    public const string Count = "Progress.Credentials.CredentialChange.Count";
                    public const string Search = "Progress.Credentials.CredentialChange.Search";
                    public const string Download = "Progress.Credentials.CredentialChange.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class LearnerExperience // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Credentials.LearnerExperience.Assert";
                    public const string Retrieve = "Progress.Credentials.LearnerExperience.Retrieve";

                    public const string Collect = "Progress.Credentials.LearnerExperience.Collect";
                    public const string Count = "Progress.Credentials.LearnerExperience.Count";
                    public const string Search = "Progress.Credentials.LearnerExperience.Search";
                    public const string Download = "Progress.Credentials.LearnerExperience.Download";

                    // Commands

                    public const string Create = "Progress.Credentials.LearnerExperience.Create";
                    public const string Delete = "Progress.Credentials.LearnerExperience.Delete";
                    public const string Modify = "Progress.Credentials.LearnerExperience.Modify";
                }
            }

            public static partial class Gradebooks // Subcomponent
            {
                public static partial class Gradebook // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Gradebooks.Gradebook.Assert";
                    public const string Retrieve = "Progress.Gradebooks.Gradebook.Retrieve";

                    public const string Collect = "Progress.Gradebooks.Gradebook.Collect";
                    public const string Count = "Progress.Gradebooks.Gradebook.Count";
                    public const string Search = "Progress.Gradebooks.Gradebook.Search";
                    public const string Download = "Progress.Gradebooks.Gradebook.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class GradebookEvent // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Gradebooks.GradebookEvent.Assert";
                    public const string Retrieve = "Progress.Gradebooks.GradebookEvent.Retrieve";

                    public const string Collect = "Progress.Gradebooks.GradebookEvent.Collect";
                    public const string Count = "Progress.Gradebooks.GradebookEvent.Count";
                    public const string Search = "Progress.Gradebooks.GradebookEvent.Search";
                    public const string Download = "Progress.Gradebooks.GradebookEvent.Download";

                    // Commands

                    public const string Create = "Progress.Gradebooks.GradebookEvent.Create";
                    public const string Delete = "Progress.Gradebooks.GradebookEvent.Delete";
                    public const string Modify = "Progress.Gradebooks.GradebookEvent.Modify";
                }

                public static partial class Gradeitem // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Gradebooks.Gradeitem.Assert";
                    public const string Retrieve = "Progress.Gradebooks.Gradeitem.Retrieve";

                    public const string Collect = "Progress.Gradebooks.Gradeitem.Collect";
                    public const string Count = "Progress.Gradebooks.Gradeitem.Count";
                    public const string Search = "Progress.Gradebooks.Gradeitem.Search";
                    public const string Download = "Progress.Gradebooks.Gradeitem.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class GradeitemCompetency // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Gradebooks.GradeitemCompetency.Assert";
                    public const string Retrieve = "Progress.Gradebooks.GradeitemCompetency.Retrieve";

                    public const string Collect = "Progress.Gradebooks.GradeitemCompetency.Collect";
                    public const string Count = "Progress.Gradebooks.GradeitemCompetency.Count";
                    public const string Search = "Progress.Gradebooks.GradeitemCompetency.Search";
                    public const string Download = "Progress.Gradebooks.GradeitemCompetency.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }

            public static partial class Logbooks // Subcomponent
            {
                public static partial class Logbook // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Logbooks.Logbook.Assert";
                    public const string Retrieve = "Progress.Logbooks.Logbook.Retrieve";

                    public const string Collect = "Progress.Logbooks.Logbook.Collect";
                    public const string Count = "Progress.Logbooks.Logbook.Count";
                    public const string Search = "Progress.Logbooks.Logbook.Search";
                    public const string Download = "Progress.Logbooks.Logbook.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class LogbookCompetency // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Logbooks.LogbookCompetency.Assert";
                    public const string Retrieve = "Progress.Logbooks.LogbookCompetency.Retrieve";

                    public const string Collect = "Progress.Logbooks.LogbookCompetency.Collect";
                    public const string Count = "Progress.Logbooks.LogbookCompetency.Count";
                    public const string Search = "Progress.Logbooks.LogbookCompetency.Search";
                    public const string Download = "Progress.Logbooks.LogbookCompetency.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class LogbookExperience // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Logbooks.LogbookExperience.Assert";
                    public const string Retrieve = "Progress.Logbooks.LogbookExperience.Retrieve";

                    public const string Collect = "Progress.Logbooks.LogbookExperience.Collect";
                    public const string Count = "Progress.Logbooks.LogbookExperience.Count";
                    public const string Search = "Progress.Logbooks.LogbookExperience.Search";
                    public const string Download = "Progress.Logbooks.LogbookExperience.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class RegulationUser // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Logbooks.RegulationUser.Assert";
                    public const string Retrieve = "Progress.Logbooks.RegulationUser.Retrieve";

                    public const string Collect = "Progress.Logbooks.RegulationUser.Collect";
                    public const string Count = "Progress.Logbooks.RegulationUser.Count";
                    public const string Search = "Progress.Logbooks.RegulationUser.Search";
                    public const string Download = "Progress.Logbooks.RegulationUser.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }

            public static partial class Periods // Subcomponent
            {
                public static partial class Period // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Periods.Period.Assert";
                    public const string Retrieve = "Progress.Periods.Period.Retrieve";

                    public const string Collect = "Progress.Periods.Period.Collect";
                    public const string Count = "Progress.Periods.Period.Count";
                    public const string Search = "Progress.Periods.Period.Search";
                    public const string Download = "Progress.Periods.Period.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }

            public static partial class Progressions // Subcomponent
            {
                public static partial class EnrollmentChange // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Progressions.EnrollmentChange.Assert";
                    public const string Retrieve = "Progress.Progressions.EnrollmentChange.Retrieve";

                    public const string Collect = "Progress.Progressions.EnrollmentChange.Collect";
                    public const string Count = "Progress.Progressions.EnrollmentChange.Count";
                    public const string Search = "Progress.Progressions.EnrollmentChange.Search";
                    public const string Download = "Progress.Progressions.EnrollmentChange.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class GradebookEnrollment // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Progressions.GradebookEnrollment.Assert";
                    public const string Retrieve = "Progress.Progressions.GradebookEnrollment.Retrieve";

                    public const string Collect = "Progress.Progressions.GradebookEnrollment.Collect";
                    public const string Count = "Progress.Progressions.GradebookEnrollment.Count";
                    public const string Search = "Progress.Progressions.GradebookEnrollment.Search";
                    public const string Download = "Progress.Progressions.GradebookEnrollment.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class LearnerProgramSummary // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Progressions.LearnerProgramSummary.Assert";
                    public const string Retrieve = "Progress.Progressions.LearnerProgramSummary.Retrieve";

                    public const string Collect = "Progress.Progressions.LearnerProgramSummary.Collect";
                    public const string Count = "Progress.Progressions.LearnerProgramSummary.Count";
                    public const string Search = "Progress.Progressions.LearnerProgramSummary.Search";
                    public const string Download = "Progress.Progressions.LearnerProgramSummary.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class Progression // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Progressions.Progression.Assert";
                    public const string Retrieve = "Progress.Progressions.Progression.Retrieve";

                    public const string Collect = "Progress.Progressions.Progression.Collect";
                    public const string Count = "Progress.Progressions.Progression.Count";
                    public const string Search = "Progress.Progressions.Progression.Search";
                    public const string Download = "Progress.Progressions.Progression.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class ProgressionChange // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Progressions.ProgressionChange.Assert";
                    public const string Retrieve = "Progress.Progressions.ProgressionChange.Retrieve";

                    public const string Collect = "Progress.Progressions.ProgressionChange.Collect";
                    public const string Count = "Progress.Progressions.ProgressionChange.Count";
                    public const string Search = "Progress.Progressions.ProgressionChange.Search";
                    public const string Download = "Progress.Progressions.ProgressionChange.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class ProgressionValidation // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Progressions.ProgressionValidation.Assert";
                    public const string Retrieve = "Progress.Progressions.ProgressionValidation.Retrieve";

                    public const string Collect = "Progress.Progressions.ProgressionValidation.Collect";
                    public const string Count = "Progress.Progressions.ProgressionValidation.Count";
                    public const string Search = "Progress.Progressions.ProgressionValidation.Search";
                    public const string Download = "Progress.Progressions.ProgressionValidation.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }

            public static partial class Regulations // Subcomponent
            {
                public static partial class Regulation // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Regulations.Regulation.Assert";
                    public const string Retrieve = "Progress.Regulations.Regulation.Retrieve";

                    public const string Collect = "Progress.Regulations.Regulation.Collect";
                    public const string Count = "Progress.Regulations.Regulation.Count";
                    public const string Search = "Progress.Regulations.Regulation.Search";
                    public const string Download = "Progress.Regulations.Regulation.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class RegulationCompetency // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Regulations.RegulationCompetency.Assert";
                    public const string Retrieve = "Progress.Regulations.RegulationCompetency.Retrieve";

                    public const string Collect = "Progress.Regulations.RegulationCompetency.Collect";
                    public const string Count = "Progress.Regulations.RegulationCompetency.Count";
                    public const string Search = "Progress.Regulations.RegulationCompetency.Search";
                    public const string Download = "Progress.Regulations.RegulationCompetency.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class RegulationField // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Regulations.RegulationField.Assert";
                    public const string Retrieve = "Progress.Regulations.RegulationField.Retrieve";

                    public const string Collect = "Progress.Regulations.RegulationField.Collect";
                    public const string Count = "Progress.Regulations.RegulationField.Count";
                    public const string Search = "Progress.Regulations.RegulationField.Search";
                    public const string Download = "Progress.Regulations.RegulationField.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class RegulationGroup // Entity
                {
                    // Queries

                    public const string Assert = "Progress.Regulations.RegulationGroup.Assert";
                    public const string Retrieve = "Progress.Regulations.RegulationGroup.Retrieve";

                    public const string Collect = "Progress.Regulations.RegulationGroup.Collect";
                    public const string Count = "Progress.Regulations.RegulationGroup.Count";
                    public const string Search = "Progress.Regulations.RegulationGroup.Search";
                    public const string Download = "Progress.Regulations.RegulationGroup.Download";

                    // Commands

                    public const string Create = "Progress.Regulations.RegulationGroup.Create";
                    public const string Delete = "Progress.Regulations.RegulationGroup.Delete";
                    public const string Modify = "Progress.Regulations.RegulationGroup.Modify";
                }
            }
        }

        public static partial class Reporting // Component
        {
            public static partial class Measurements // Subcomponent
            {
                public static partial class Measurement // Entity
                {
                    // Queries

                    public const string Assert = "Reporting.Measurements.Measurement.Assert";
                    public const string Retrieve = "Reporting.Measurements.Measurement.Retrieve";

                    public const string Collect = "Reporting.Measurements.Measurement.Collect";
                    public const string Count = "Reporting.Measurements.Measurement.Count";
                    public const string Search = "Reporting.Measurements.Measurement.Search";
                    public const string Download = "Reporting.Measurements.Measurement.Download";

                    // Commands

                    public const string Create = "Reporting.Measurements.Measurement.Create";
                    public const string Delete = "Reporting.Measurements.Measurement.Delete";
                    public const string Modify = "Reporting.Measurements.Measurement.Modify";
                }
            }

            public static partial class Reports // Subcomponent
            {
                public static partial class Report // Entity
                {
                    // Queries

                    public const string Assert = "Reporting.Reports.Report.Assert";
                    public const string Retrieve = "Reporting.Reports.Report.Retrieve";

                    public const string Collect = "Reporting.Reports.Report.Collect";
                    public const string Count = "Reporting.Reports.Report.Count";
                    public const string Search = "Reporting.Reports.Report.Search";
                    public const string Download = "Reporting.Reports.Report.Download";

                    // Commands

                    public const string Create = "Reporting.Reports.Report.Create";
                    public const string Delete = "Reporting.Reports.Report.Delete";
                    public const string Modify = "Reporting.Reports.Report.Modify";
                }
            }
        }

        public static partial class Security // Component
        {
            public static partial class Developers // Subcomponent
            {
                public static partial class Developer // Entity
                {
                    // Queries

                    public const string Assert = "Security.Developers.Developer.Assert";
                    public const string Retrieve = "Security.Developers.Developer.Retrieve";

                    public const string Collect = "Security.Developers.Developer.Collect";
                    public const string Count = "Security.Developers.Developer.Count";
                    public const string Search = "Security.Developers.Developer.Search";
                    public const string Download = "Security.Developers.Developer.Download";

                    // Commands

                    public const string Create = "Security.Developers.Developer.Create";
                    public const string Delete = "Security.Developers.Developer.Delete";
                    public const string Modify = "Security.Developers.Developer.Modify";
                }
            }

            public static partial class Impersonations // Subcomponent
            {
                public static partial class Impersonation // Entity
                {
                    // Queries

                    public const string Assert = "Security.Impersonations.Impersonation.Assert";
                    public const string Retrieve = "Security.Impersonations.Impersonation.Retrieve";

                    public const string Collect = "Security.Impersonations.Impersonation.Collect";
                    public const string Count = "Security.Impersonations.Impersonation.Count";
                    public const string Search = "Security.Impersonations.Impersonation.Search";
                    public const string Download = "Security.Impersonations.Impersonation.Download";

                    // Commands

                    public const string Create = "Security.Impersonations.Impersonation.Create";
                    public const string Delete = "Security.Impersonations.Impersonation.Delete";
                    public const string Modify = "Security.Impersonations.Impersonation.Modify";
                }
            }

            public static partial class Organizations // Subcomponent
            {
                public static partial class Organization // Entity
                {
                    // Queries

                    public const string Assert = "Security.Organizations.Organization.Assert";
                    public const string Retrieve = "Security.Organizations.Organization.Retrieve";

                    public const string Collect = "Security.Organizations.Organization.Collect";
                    public const string Count = "Security.Organizations.Organization.Count";
                    public const string Search = "Security.Organizations.Organization.Search";
                    public const string Download = "Security.Organizations.Organization.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }

            public static partial class Permissions // Subcomponent
            {
                public static partial class Permission // Entity
                {
                    // Queries

                    public const string Assert = "Security.Permissions.Permission.Assert";
                    public const string Retrieve = "Security.Permissions.Permission.Retrieve";

                    public const string Collect = "Security.Permissions.Permission.Collect";
                    public const string Count = "Security.Permissions.Permission.Count";
                    public const string Search = "Security.Permissions.Permission.Search";
                    public const string Download = "Security.Permissions.Permission.Download";

                    // Commands

                    public const string Create = "Security.Permissions.Permission.Create";
                    public const string Delete = "Security.Permissions.Permission.Delete";
                    public const string Modify = "Security.Permissions.Permission.Modify";
                }
            }

            public static partial class Users // Subcomponent
            {
                public static partial class User // Entity
                {
                    // Queries

                    public const string Assert = "Security.Users.User.Assert";
                    public const string Retrieve = "Security.Users.User.Retrieve";

                    public const string Collect = "Security.Users.User.Collect";
                    public const string Count = "Security.Users.User.Count";
                    public const string Search = "Security.Users.User.Search";
                    public const string Download = "Security.Users.User.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class UserComment // Entity
                {
                    // Queries

                    public const string Assert = "Security.Users.UserComment.Assert";
                    public const string Retrieve = "Security.Users.UserComment.Retrieve";

                    public const string Collect = "Security.Users.UserComment.Collect";
                    public const string Count = "Security.Users.UserComment.Count";
                    public const string Search = "Security.Users.UserComment.Search";
                    public const string Download = "Security.Users.UserComment.Download";

                    // Commands

                    public const string Create = "Security.Users.UserComment.Create";
                    public const string Delete = "Security.Users.UserComment.Delete";
                    public const string Modify = "Security.Users.UserComment.Modify";
                }

                public static partial class UserConnection // Entity
                {
                    // Queries

                    public const string Assert = "Security.Users.UserConnection.Assert";
                    public const string Retrieve = "Security.Users.UserConnection.Retrieve";

                    public const string Collect = "Security.Users.UserConnection.Collect";
                    public const string Count = "Security.Users.UserConnection.Count";
                    public const string Search = "Security.Users.UserConnection.Search";
                    public const string Download = "Security.Users.UserConnection.Download";

                    // Commands

                    public const string Create = "Security.Users.UserConnection.Create";
                    public const string Delete = "Security.Users.UserConnection.Delete";
                    public const string Modify = "Security.Users.UserConnection.Modify";
                }

                public static partial class UserField // Entity
                {
                    // Queries

                    public const string Assert = "Security.Users.UserField.Assert";
                    public const string Retrieve = "Security.Users.UserField.Retrieve";

                    public const string Collect = "Security.Users.UserField.Collect";
                    public const string Count = "Security.Users.UserField.Count";
                    public const string Search = "Security.Users.UserField.Search";
                    public const string Download = "Security.Users.UserField.Download";

                    // Commands

                    public const string Create = "Security.Users.UserField.Create";
                    public const string Delete = "Security.Users.UserField.Delete";
                    public const string Modify = "Security.Users.UserField.Modify";
                }

                public static partial class UserMock // Entity
                {
                    // Queries

                    public const string Assert = "Security.Users.UserMock.Assert";
                    public const string Retrieve = "Security.Users.UserMock.Retrieve";

                    public const string Collect = "Security.Users.UserMock.Collect";
                    public const string Count = "Security.Users.UserMock.Count";
                    public const string Search = "Security.Users.UserMock.Search";
                    public const string Download = "Security.Users.UserMock.Download";

                    // Commands

                    public const string Create = "Security.Users.UserMock.Create";
                    public const string Delete = "Security.Users.UserMock.Delete";
                    public const string Modify = "Security.Users.UserMock.Modify";
                }

                public static partial class UserSession // Entity
                {
                    // Queries

                    public const string Assert = "Security.Users.UserSession.Assert";
                    public const string Retrieve = "Security.Users.UserSession.Retrieve";

                    public const string Collect = "Security.Users.UserSession.Collect";
                    public const string Count = "Security.Users.UserSession.Count";
                    public const string Search = "Security.Users.UserSession.Search";
                    public const string Download = "Security.Users.UserSession.Download";

                    // Commands

                    public const string Create = "Security.Users.UserSession.Create";
                    public const string Delete = "Security.Users.UserSession.Delete";
                    public const string Modify = "Security.Users.UserSession.Modify";
                }

                public static partial class UserSessionCache // Entity
                {
                    // Queries

                    public const string Assert = "Security.Users.UserSessionCache.Assert";
                    public const string Retrieve = "Security.Users.UserSessionCache.Retrieve";

                    public const string Collect = "Security.Users.UserSessionCache.Collect";
                    public const string Count = "Security.Users.UserSessionCache.Count";
                    public const string Search = "Security.Users.UserSessionCache.Search";
                    public const string Download = "Security.Users.UserSessionCache.Download";

                    // Commands

                    public const string Create = "Security.Users.UserSessionCache.Create";
                    public const string Delete = "Security.Users.UserSessionCache.Delete";
                    public const string Modify = "Security.Users.UserSessionCache.Modify";
                }

                public static partial class UserToken // Entity
                {
                    // Queries

                    public const string Assert = "Security.Users.UserToken.Assert";
                    public const string Retrieve = "Security.Users.UserToken.Retrieve";

                    public const string Collect = "Security.Users.UserToken.Collect";
                    public const string Count = "Security.Users.UserToken.Count";
                    public const string Search = "Security.Users.UserToken.Search";
                    public const string Download = "Security.Users.UserToken.Download";

                    // Commands

                    public const string Create = "Security.Users.UserToken.Create";
                    public const string Delete = "Security.Users.UserToken.Delete";
                    public const string Modify = "Security.Users.UserToken.Modify";
                }
            }
        }

        public static partial class Timeline // Component
        {
            public static partial class Changes // Subcomponent
            {
                public static partial class Aggregate // Entity
                {
                    // Queries

                    public const string Assert = "Timeline.Changes.Aggregate.Assert";
                    public const string Retrieve = "Timeline.Changes.Aggregate.Retrieve";

                    public const string Collect = "Timeline.Changes.Aggregate.Collect";
                    public const string Count = "Timeline.Changes.Aggregate.Count";
                    public const string Search = "Timeline.Changes.Aggregate.Search";
                    public const string Download = "Timeline.Changes.Aggregate.Download";

                    // Commands

                    public const string Create = "Timeline.Changes.Aggregate.Create";
                    public const string Delete = "Timeline.Changes.Aggregate.Delete";
                    public const string Modify = "Timeline.Changes.Aggregate.Modify";
                }

                public static partial class Change // Entity
                {
                    // Queries

                    public const string Assert = "Timeline.Changes.Change.Assert";
                    public const string Retrieve = "Timeline.Changes.Change.Retrieve";

                    public const string Collect = "Timeline.Changes.Change.Collect";
                    public const string Count = "Timeline.Changes.Change.Count";
                    public const string Search = "Timeline.Changes.Change.Search";
                    public const string Download = "Timeline.Changes.Change.Download";

                    // Commands

                    public const string Create = "Timeline.Changes.Change.Create";
                    public const string Delete = "Timeline.Changes.Change.Delete";
                    public const string Modify = "Timeline.Changes.Change.Modify";
                }

                public static partial class Snapshot // Entity
                {
                    // Queries

                    public const string Assert = "Timeline.Changes.Snapshot.Assert";
                    public const string Retrieve = "Timeline.Changes.Snapshot.Retrieve";

                    public const string Collect = "Timeline.Changes.Snapshot.Collect";
                    public const string Count = "Timeline.Changes.Snapshot.Count";
                    public const string Search = "Timeline.Changes.Snapshot.Search";
                    public const string Download = "Timeline.Changes.Snapshot.Download";

                    // Commands

                    public const string Create = "Timeline.Changes.Snapshot.Create";
                    public const string Delete = "Timeline.Changes.Snapshot.Delete";
                    public const string Modify = "Timeline.Changes.Snapshot.Modify";
                }
            }

            public static partial class Commands // Subcomponent
            {
                public static partial class Command // Entity
                {
                    // Queries

                    public const string Assert = "Timeline.Commands.Command.Assert";
                    public const string Retrieve = "Timeline.Commands.Command.Retrieve";

                    public const string Collect = "Timeline.Commands.Command.Collect";
                    public const string Count = "Timeline.Commands.Command.Count";
                    public const string Search = "Timeline.Commands.Command.Search";
                    public const string Download = "Timeline.Commands.Command.Download";

                    // Commands

                    public const string Create = "Timeline.Commands.Command.Create";
                    public const string Delete = "Timeline.Commands.Command.Delete";
                    public const string Modify = "Timeline.Commands.Command.Modify";
                }
            }
        }

        public static partial class Variant // Component
        {
            public static partial class CMDS // Subcomponent
            {
                public static partial class CmdsInvoice // Entity
                {
                    // Queries

                    public const string Assert = "Variant.CMDS.CmdsInvoice.Assert";
                    public const string Retrieve = "Variant.CMDS.CmdsInvoice.Retrieve";

                    public const string Collect = "Variant.CMDS.CmdsInvoice.Collect";
                    public const string Count = "Variant.CMDS.CmdsInvoice.Count";
                    public const string Search = "Variant.CMDS.CmdsInvoice.Search";
                    public const string Download = "Variant.CMDS.CmdsInvoice.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class CmdsInvoiceFee // Entity
                {
                    // Queries

                    public const string Assert = "Variant.CMDS.CmdsInvoiceFee.Assert";
                    public const string Retrieve = "Variant.CMDS.CmdsInvoiceFee.Retrieve";

                    public const string Collect = "Variant.CMDS.CmdsInvoiceFee.Collect";
                    public const string Count = "Variant.CMDS.CmdsInvoiceFee.Count";
                    public const string Search = "Variant.CMDS.CmdsInvoiceFee.Search";
                    public const string Download = "Variant.CMDS.CmdsInvoiceFee.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class CmdsInvoiceItem // Entity
                {
                    // Queries

                    public const string Assert = "Variant.CMDS.CmdsInvoiceItem.Assert";
                    public const string Retrieve = "Variant.CMDS.CmdsInvoiceItem.Retrieve";

                    public const string Collect = "Variant.CMDS.CmdsInvoiceItem.Collect";
                    public const string Count = "Variant.CMDS.CmdsInvoiceItem.Count";
                    public const string Search = "Variant.CMDS.CmdsInvoiceItem.Search";
                    public const string Download = "Variant.CMDS.CmdsInvoiceItem.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class CollegeCertificate // Entity
                {
                    // Queries

                    public const string Assert = "Variant.CMDS.CollegeCertificate.Assert";
                    public const string Retrieve = "Variant.CMDS.CollegeCertificate.Retrieve";

                    public const string Collect = "Variant.CMDS.CollegeCertificate.Collect";
                    public const string Count = "Variant.CMDS.CollegeCertificate.Count";
                    public const string Search = "Variant.CMDS.CollegeCertificate.Search";
                    public const string Download = "Variant.CMDS.CollegeCertificate.Download";

                    // Commands

                    public const string Create = "Variant.CMDS.CollegeCertificate.Create";
                    public const string Delete = "Variant.CMDS.CollegeCertificate.Delete";
                    public const string Modify = "Variant.CMDS.CollegeCertificate.Modify";
                }

                public static partial class LearnerMeasurement // Entity
                {
                    // Queries

                    public const string Assert = "Variant.CMDS.LearnerMeasurement.Assert";
                    public const string Retrieve = "Variant.CMDS.LearnerMeasurement.Retrieve";

                    public const string Collect = "Variant.CMDS.LearnerMeasurement.Collect";
                    public const string Count = "Variant.CMDS.LearnerMeasurement.Count";
                    public const string Search = "Variant.CMDS.LearnerMeasurement.Search";
                    public const string Download = "Variant.CMDS.LearnerMeasurement.Download";

                    // Commands

                    public const string Create = "Variant.CMDS.LearnerMeasurement.Create";
                    public const string Delete = "Variant.CMDS.LearnerMeasurement.Delete";
                    public const string Modify = "Variant.CMDS.LearnerMeasurement.Modify";
                }

                public static partial class LearnerSnapshot // Entity
                {
                    // Queries

                    public const string Assert = "Variant.CMDS.LearnerSnapshot.Assert";
                    public const string Retrieve = "Variant.CMDS.LearnerSnapshot.Retrieve";

                    public const string Collect = "Variant.CMDS.LearnerSnapshot.Collect";
                    public const string Count = "Variant.CMDS.LearnerSnapshot.Count";
                    public const string Search = "Variant.CMDS.LearnerSnapshot.Search";
                    public const string Download = "Variant.CMDS.LearnerSnapshot.Download";

                    // Commands

                    public const string Create = "Variant.CMDS.LearnerSnapshot.Create";
                    public const string Delete = "Variant.CMDS.LearnerSnapshot.Delete";
                    public const string Modify = "Variant.CMDS.LearnerSnapshot.Modify";
                }

                public static partial class LearnerSnapshotSummary // Entity
                {
                    // Queries

                    public const string Assert = "Variant.CMDS.LearnerSnapshotSummary.Assert";
                    public const string Retrieve = "Variant.CMDS.LearnerSnapshotSummary.Retrieve";

                    public const string Collect = "Variant.CMDS.LearnerSnapshotSummary.Collect";
                    public const string Count = "Variant.CMDS.LearnerSnapshotSummary.Count";
                    public const string Search = "Variant.CMDS.LearnerSnapshotSummary.Search";
                    public const string Download = "Variant.CMDS.LearnerSnapshotSummary.Download";

                    // Commands

                    public const string Create = "Variant.CMDS.LearnerSnapshotSummary.Create";
                    public const string Delete = "Variant.CMDS.LearnerSnapshotSummary.Delete";
                    public const string Modify = "Variant.CMDS.LearnerSnapshotSummary.Modify";
                }

                public static partial class LearnerSummary // Entity
                {
                    // Queries

                    public const string Assert = "Variant.CMDS.LearnerSummary.Assert";
                    public const string Retrieve = "Variant.CMDS.LearnerSummary.Retrieve";

                    public const string Collect = "Variant.CMDS.LearnerSummary.Collect";
                    public const string Count = "Variant.CMDS.LearnerSummary.Count";
                    public const string Search = "Variant.CMDS.LearnerSummary.Search";
                    public const string Download = "Variant.CMDS.LearnerSummary.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }

            public static partial class NCSHA // Subcomponent
            {
                public static partial class Counter // Entity
                {
                    // Queries

                    public const string Assert = "Variant.NCSHA.Counter.Assert";
                    public const string Retrieve = "Variant.NCSHA.Counter.Retrieve";

                    public const string Collect = "Variant.NCSHA.Counter.Collect";
                    public const string Count = "Variant.NCSHA.Counter.Count";
                    public const string Search = "Variant.NCSHA.Counter.Search";
                    public const string Download = "Variant.NCSHA.Counter.Download";

                    // Commands

                    public const string Create = "Variant.NCSHA.Counter.Create";
                    public const string Delete = "Variant.NCSHA.Counter.Delete";
                    public const string Modify = "Variant.NCSHA.Counter.Modify";
                }

                public static partial class ReportChange // Entity
                {
                    // Queries

                    public const string Assert = "Variant.NCSHA.ReportChange.Assert";
                    public const string Retrieve = "Variant.NCSHA.ReportChange.Retrieve";

                    public const string Collect = "Variant.NCSHA.ReportChange.Collect";
                    public const string Count = "Variant.NCSHA.ReportChange.Count";
                    public const string Search = "Variant.NCSHA.ReportChange.Search";
                    public const string Download = "Variant.NCSHA.ReportChange.Download";

                    // Commands

                    public const string Create = "Variant.NCSHA.ReportChange.Create";
                    public const string Delete = "Variant.NCSHA.ReportChange.Delete";
                    public const string Modify = "Variant.NCSHA.ReportChange.Modify";
                }

                public static partial class ReportField // Entity
                {
                    // Queries

                    public const string Assert = "Variant.NCSHA.ReportField.Assert";
                    public const string Retrieve = "Variant.NCSHA.ReportField.Retrieve";

                    public const string Collect = "Variant.NCSHA.ReportField.Collect";
                    public const string Count = "Variant.NCSHA.ReportField.Count";
                    public const string Search = "Variant.NCSHA.ReportField.Search";
                    public const string Download = "Variant.NCSHA.ReportField.Download";

                    // Commands

                    public const string Create = "Variant.NCSHA.ReportField.Create";
                    public const string Delete = "Variant.NCSHA.ReportField.Delete";
                    public const string Modify = "Variant.NCSHA.ReportField.Modify";
                }

                public static partial class ReportFilter // Entity
                {
                    // Queries

                    public const string Assert = "Variant.NCSHA.ReportFilter.Assert";
                    public const string Retrieve = "Variant.NCSHA.ReportFilter.Retrieve";

                    public const string Collect = "Variant.NCSHA.ReportFilter.Collect";
                    public const string Count = "Variant.NCSHA.ReportFilter.Count";
                    public const string Search = "Variant.NCSHA.ReportFilter.Search";
                    public const string Download = "Variant.NCSHA.ReportFilter.Download";

                    // Commands

                    public const string Create = "Variant.NCSHA.ReportFilter.Create";
                    public const string Delete = "Variant.NCSHA.ReportFilter.Delete";
                    public const string Modify = "Variant.NCSHA.ReportFilter.Modify";
                }

                public static partial class ReportFolder // Entity
                {
                    // Queries

                    public const string Assert = "Variant.NCSHA.ReportFolder.Assert";
                    public const string Retrieve = "Variant.NCSHA.ReportFolder.Retrieve";

                    public const string Collect = "Variant.NCSHA.ReportFolder.Collect";
                    public const string Count = "Variant.NCSHA.ReportFolder.Count";
                    public const string Search = "Variant.NCSHA.ReportFolder.Search";
                    public const string Download = "Variant.NCSHA.ReportFolder.Download";

                    // Commands

                    public const string Create = "Variant.NCSHA.ReportFolder.Create";
                    public const string Delete = "Variant.NCSHA.ReportFolder.Delete";
                    public const string Modify = "Variant.NCSHA.ReportFolder.Modify";
                }

                public static partial class ReportFolderComment // Entity
                {
                    // Queries

                    public const string Assert = "Variant.NCSHA.ReportFolderComment.Assert";
                    public const string Retrieve = "Variant.NCSHA.ReportFolderComment.Retrieve";

                    public const string Collect = "Variant.NCSHA.ReportFolderComment.Collect";
                    public const string Count = "Variant.NCSHA.ReportFolderComment.Count";
                    public const string Search = "Variant.NCSHA.ReportFolderComment.Search";
                    public const string Download = "Variant.NCSHA.ReportFolderComment.Download";

                    // Commands

                    public const string Create = "Variant.NCSHA.ReportFolderComment.Create";
                    public const string Delete = "Variant.NCSHA.ReportFolderComment.Delete";
                    public const string Modify = "Variant.NCSHA.ReportFolderComment.Modify";
                }

                public static partial class ReportMapping // Entity
                {
                    // Queries

                    public const string Assert = "Variant.NCSHA.ReportMapping.Assert";
                    public const string Retrieve = "Variant.NCSHA.ReportMapping.Retrieve";

                    public const string Collect = "Variant.NCSHA.ReportMapping.Collect";
                    public const string Count = "Variant.NCSHA.ReportMapping.Count";
                    public const string Search = "Variant.NCSHA.ReportMapping.Search";
                    public const string Download = "Variant.NCSHA.ReportMapping.Download";

                    // Commands

                    public const string Create = "Variant.NCSHA.ReportMapping.Create";
                    public const string Delete = "Variant.NCSHA.ReportMapping.Delete";
                    public const string Modify = "Variant.NCSHA.ReportMapping.Modify";
                }

                public static partial class SurveyAB // Entity
                {
                    // Queries

                    public const string Assert = "Variant.NCSHA.SurveyAB.Assert";
                    public const string Retrieve = "Variant.NCSHA.SurveyAB.Retrieve";

                    public const string Collect = "Variant.NCSHA.SurveyAB.Collect";
                    public const string Count = "Variant.NCSHA.SurveyAB.Count";
                    public const string Search = "Variant.NCSHA.SurveyAB.Search";
                    public const string Download = "Variant.NCSHA.SurveyAB.Download";

                    // Commands

                    public const string Create = "Variant.NCSHA.SurveyAB.Create";
                    public const string Delete = "Variant.NCSHA.SurveyAB.Delete";
                    public const string Modify = "Variant.NCSHA.SurveyAB.Modify";
                }

                public static partial class SurveyHC // Entity
                {
                    // Queries

                    public const string Assert = "Variant.NCSHA.SurveyHC.Assert";
                    public const string Retrieve = "Variant.NCSHA.SurveyHC.Retrieve";

                    public const string Collect = "Variant.NCSHA.SurveyHC.Collect";
                    public const string Count = "Variant.NCSHA.SurveyHC.Count";
                    public const string Search = "Variant.NCSHA.SurveyHC.Search";
                    public const string Download = "Variant.NCSHA.SurveyHC.Download";

                    // Commands

                    public const string Create = "Variant.NCSHA.SurveyHC.Create";
                    public const string Delete = "Variant.NCSHA.SurveyHC.Delete";
                    public const string Modify = "Variant.NCSHA.SurveyHC.Modify";
                }

                public static partial class SurveyHI // Entity
                {
                    // Queries

                    public const string Assert = "Variant.NCSHA.SurveyHI.Assert";
                    public const string Retrieve = "Variant.NCSHA.SurveyHI.Retrieve";

                    public const string Collect = "Variant.NCSHA.SurveyHI.Collect";
                    public const string Count = "Variant.NCSHA.SurveyHI.Count";
                    public const string Search = "Variant.NCSHA.SurveyHI.Search";
                    public const string Download = "Variant.NCSHA.SurveyHI.Download";

                    // Commands

                    public const string Create = "Variant.NCSHA.SurveyHI.Create";
                    public const string Delete = "Variant.NCSHA.SurveyHI.Delete";
                    public const string Modify = "Variant.NCSHA.SurveyHI.Modify";
                }

                public static partial class SurveyMF // Entity
                {
                    // Queries

                    public const string Assert = "Variant.NCSHA.SurveyMF.Assert";
                    public const string Retrieve = "Variant.NCSHA.SurveyMF.Retrieve";

                    public const string Collect = "Variant.NCSHA.SurveyMF.Collect";
                    public const string Count = "Variant.NCSHA.SurveyMF.Count";
                    public const string Search = "Variant.NCSHA.SurveyMF.Search";
                    public const string Download = "Variant.NCSHA.SurveyMF.Download";

                    // Commands

                    public const string Create = "Variant.NCSHA.SurveyMF.Create";
                    public const string Delete = "Variant.NCSHA.SurveyMF.Delete";
                    public const string Modify = "Variant.NCSHA.SurveyMF.Modify";
                }

                public static partial class SurveyMR // Entity
                {
                    // Queries

                    public const string Assert = "Variant.NCSHA.SurveyMR.Assert";
                    public const string Retrieve = "Variant.NCSHA.SurveyMR.Retrieve";

                    public const string Collect = "Variant.NCSHA.SurveyMR.Collect";
                    public const string Count = "Variant.NCSHA.SurveyMR.Count";
                    public const string Search = "Variant.NCSHA.SurveyMR.Search";
                    public const string Download = "Variant.NCSHA.SurveyMR.Download";

                    // Commands

                    public const string Create = "Variant.NCSHA.SurveyMR.Create";
                    public const string Delete = "Variant.NCSHA.SurveyMR.Delete";
                    public const string Modify = "Variant.NCSHA.SurveyMR.Modify";
                }

                public static partial class SurveyPA // Entity
                {
                    // Queries

                    public const string Assert = "Variant.NCSHA.SurveyPA.Assert";
                    public const string Retrieve = "Variant.NCSHA.SurveyPA.Retrieve";

                    public const string Collect = "Variant.NCSHA.SurveyPA.Collect";
                    public const string Count = "Variant.NCSHA.SurveyPA.Count";
                    public const string Search = "Variant.NCSHA.SurveyPA.Search";
                    public const string Download = "Variant.NCSHA.SurveyPA.Download";

                    // Commands

                    public const string Create = "Variant.NCSHA.SurveyPA.Create";
                    public const string Delete = "Variant.NCSHA.SurveyPA.Delete";
                    public const string Modify = "Variant.NCSHA.SurveyPA.Modify";
                }
            }

            public static partial class SkilledTradesBC // Subcomponent
            {
                public static partial class Distribution // Entity
                {
                    // Queries

                    public const string Assert = "Variant.SkilledTradesBC.Distribution.Assert";
                    public const string Retrieve = "Variant.SkilledTradesBC.Distribution.Retrieve";

                    public const string Collect = "Variant.SkilledTradesBC.Distribution.Collect";
                    public const string Count = "Variant.SkilledTradesBC.Distribution.Count";
                    public const string Search = "Variant.SkilledTradesBC.Distribution.Search";
                    public const string Download = "Variant.SkilledTradesBC.Distribution.Download";

                    // Commands

                    public const string Create = "Variant.SkilledTradesBC.Distribution.Create";
                    public const string Delete = "Variant.SkilledTradesBC.Distribution.Delete";
                    public const string Modify = "Variant.SkilledTradesBC.Distribution.Modify";
                }

                public static partial class Individual // Entity
                {
                    // Queries

                    public const string Assert = "Variant.SkilledTradesBC.Individual.Assert";
                    public const string Retrieve = "Variant.SkilledTradesBC.Individual.Retrieve";

                    public const string Collect = "Variant.SkilledTradesBC.Individual.Collect";
                    public const string Count = "Variant.SkilledTradesBC.Individual.Count";
                    public const string Search = "Variant.SkilledTradesBC.Individual.Search";
                    public const string Download = "Variant.SkilledTradesBC.Individual.Download";

                    // Commands

                    public const string Create = "Variant.SkilledTradesBC.Individual.Create";
                    public const string Delete = "Variant.SkilledTradesBC.Individual.Delete";
                    public const string Modify = "Variant.SkilledTradesBC.Individual.Modify";
                }
            }
        }

        public static partial class Workflow // Component
        {
            public static partial class Cases // Subcomponent
            {
                public static partial class Case // Entity
                {
                    // Queries

                    public const string Assert = "Workflow.Cases.Case.Assert";
                    public const string Retrieve = "Workflow.Cases.Case.Retrieve";

                    public const string Collect = "Workflow.Cases.Case.Collect";
                    public const string Count = "Workflow.Cases.Case.Count";
                    public const string Search = "Workflow.Cases.Case.Search";
                    public const string Download = "Workflow.Cases.Case.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class CaseFile // Entity
                {
                    // Queries

                    public const string Assert = "Workflow.Cases.CaseFile.Assert";
                    public const string Retrieve = "Workflow.Cases.CaseFile.Retrieve";

                    public const string Collect = "Workflow.Cases.CaseFile.Collect";
                    public const string Count = "Workflow.Cases.CaseFile.Count";
                    public const string Search = "Workflow.Cases.CaseFile.Search";
                    public const string Download = "Workflow.Cases.CaseFile.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class CaseFileRequirement // Entity
                {
                    // Queries

                    public const string Assert = "Workflow.Cases.CaseFileRequirement.Assert";
                    public const string Retrieve = "Workflow.Cases.CaseFileRequirement.Retrieve";

                    public const string Collect = "Workflow.Cases.CaseFileRequirement.Collect";
                    public const string Count = "Workflow.Cases.CaseFileRequirement.Count";
                    public const string Search = "Workflow.Cases.CaseFileRequirement.Search";
                    public const string Download = "Workflow.Cases.CaseFileRequirement.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class CaseGroup // Entity
                {
                    // Queries

                    public const string Assert = "Workflow.Cases.CaseGroup.Assert";
                    public const string Retrieve = "Workflow.Cases.CaseGroup.Retrieve";

                    public const string Collect = "Workflow.Cases.CaseGroup.Collect";
                    public const string Count = "Workflow.Cases.CaseGroup.Count";
                    public const string Search = "Workflow.Cases.CaseGroup.Search";
                    public const string Download = "Workflow.Cases.CaseGroup.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }

                public static partial class CaseStatus // Entity
                {
                    // Queries

                    public const string Assert = "Workflow.Cases.CaseStatus.Assert";
                    public const string Retrieve = "Workflow.Cases.CaseStatus.Retrieve";

                    public const string Collect = "Workflow.Cases.CaseStatus.Collect";
                    public const string Count = "Workflow.Cases.CaseStatus.Count";
                    public const string Search = "Workflow.Cases.CaseStatus.Search";
                    public const string Download = "Workflow.Cases.CaseStatus.Download";

                    // Commands

                    public const string Create = "Workflow.Cases.CaseStatus.Create";
                    public const string Delete = "Workflow.Cases.CaseStatus.Delete";
                    public const string Modify = "Workflow.Cases.CaseStatus.Modify";
                }

                public static partial class CaseUser // Entity
                {
                    // Queries

                    public const string Assert = "Workflow.Cases.CaseUser.Assert";
                    public const string Retrieve = "Workflow.Cases.CaseUser.Retrieve";

                    public const string Collect = "Workflow.Cases.CaseUser.Collect";
                    public const string Count = "Workflow.Cases.CaseUser.Count";
                    public const string Search = "Workflow.Cases.CaseUser.Search";
                    public const string Download = "Workflow.Cases.CaseUser.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }
        }

        public static partial class Workspace // Component
        {
            public static partial class Pages // Subcomponent
            {
                public static partial class Page // Entity
                {
                    // Queries

                    public const string Assert = "Workspace.Pages.Page.Assert";
                    public const string Retrieve = "Workspace.Pages.Page.Retrieve";

                    public const string Collect = "Workspace.Pages.Page.Collect";
                    public const string Count = "Workspace.Pages.Page.Count";
                    public const string Search = "Workspace.Pages.Page.Search";
                    public const string Download = "Workspace.Pages.Page.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }

            public static partial class Sites // Subcomponent
            {
                public static partial class Site // Entity
                {
                    // Queries

                    public const string Assert = "Workspace.Sites.Site.Assert";
                    public const string Retrieve = "Workspace.Sites.Site.Retrieve";

                    public const string Collect = "Workspace.Sites.Site.Collect";
                    public const string Count = "Workspace.Sites.Site.Count";
                    public const string Search = "Workspace.Sites.Site.Search";
                    public const string Download = "Workspace.Sites.Site.Download";

                    // Commands

                    // This entity is a current-state projection of an aggregate event/change stream. This is the reason
                    // you do not see any controller actions implemented here to create, modify, or delete this entity. 
                    // Data changes to this entity are permitted only using Timeline commands.
                }
            }
        }

    }
}