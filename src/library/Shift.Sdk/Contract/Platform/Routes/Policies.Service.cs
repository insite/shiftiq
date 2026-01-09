namespace Shift.Contract
{
    ///<summary>
    /// Defines authorization policies for every entity
    /// </summary>
    /// <remarks>
    /// ## Queries
    /// - Assert = **HEAD**: Check for the existence of a single item
    /// - Retrieve = **GET**:  Retrieve a single item
    /// - Collect = **GET or POST**: Retrieve multiple items that match criteria (paging enabled)
    /// - Count = **GET or POST**: Count the number of items that match criteria
    /// - Download = **GET or POST**: Retrieve multiple items that match criteria (paging disabled)
    /// - Search = **GET or POST**: Find multiple items that match criteria (paging enabled)
    /// ## Commands
    /// - Create = **POST**: Insert a single item
    /// - Delete = **DELETE**: Delete a single item
    /// - Modify = **PUT**: Update a single item
    /// </remarks>
    public static partial class Policies
    {
        public static partial class Billing // Component
        {
            public static partial class Discounts // Subcomponent
            {
                public static partial class Discount // Entity
                {
                    // Queries

                    public const string Assert = "billing/discounts/assert";
                    public const string Retrieve = "billing/discounts/retrieve";

                    public const string Collect = "billing/discounts/collect";
                    public const string Count = "billing/discounts/count";
                    public const string Search = "billing/discounts/search";
                    public const string Download = "billing/discounts/download";

                    // Commands

                    public const string Create = "billing/discounts/create";
                    public const string Delete = "billing/discounts/delete";
                    public const string Modify = "billing/discounts/modify";
                }
            }

            public static partial class Invoices // Subcomponent
            {
                public static partial class Invoice // Entity
                {
                    // Queries

                    public const string Assert = "billing/invoices/assert";
                    public const string Retrieve = "billing/invoices/retrieve";

                    public const string Collect = "billing/invoices/collect";
                    public const string Count = "billing/invoices/count";
                    public const string Search = "billing/invoices/search";
                    public const string Download = "billing/invoices/download";
                }

                public static partial class InvoiceItem // Entity
                {
                    // Queries

                    public const string Assert = "billing/invoices-items/assert";
                    public const string Retrieve = "billing/invoices-items/retrieve";

                    public const string Collect = "billing/invoices-items/collect";
                    public const string Count = "billing/invoices-items/count";
                    public const string Search = "billing/invoices-items/search";
                    public const string Download = "billing/invoices-items/download";
                }
            }

            public static partial class Orders // Subcomponent
            {
                public static partial class Order // Entity
                {
                    // Queries

                    public const string Assert = "billing/orders/assert";
                    public const string Retrieve = "billing/orders/retrieve";

                    public const string Collect = "billing/orders/collect";
                    public const string Count = "billing/orders/count";
                    public const string Search = "billing/orders/search";
                    public const string Download = "billing/orders/download";

                    // Commands

                    public const string Create = "billing/orders/create";
                    public const string Delete = "billing/orders/delete";
                    public const string Modify = "billing/orders/modify";
                }
            }

            public static partial class Payments // Subcomponent
            {
                public static partial class Payment // Entity
                {
                    // Queries

                    public const string Assert = "billing/payments/assert";
                    public const string Retrieve = "billing/payments/retrieve";

                    public const string Collect = "billing/payments/collect";
                    public const string Count = "billing/payments/count";
                    public const string Search = "billing/payments/search";
                    public const string Download = "billing/payments/download";
                }
            }

            public static partial class Products // Subcomponent
            {
                public static partial class Product // Entity
                {
                    // Queries

                    public const string Assert = "billing/products/assert";
                    public const string Retrieve = "billing/products/retrieve";

                    public const string Collect = "billing/products/collect";
                    public const string Count = "billing/products/count";
                    public const string Search = "billing/products/search";
                    public const string Download = "billing/products/download";

                    // Commands

                    public const string Create = "billing/products/create";
                    public const string Delete = "billing/products/delete";
                    public const string Modify = "billing/products/modify";
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

                    public const string Assert = "booking/events/assert";
                    public const string Retrieve = "booking/events/retrieve";

                    public const string Collect = "booking/events/collect";
                    public const string Count = "booking/events/count";
                    public const string Search = "booking/events/search";
                    public const string Download = "booking/events/download";
                }

                public static partial class EventForm // Entity
                {
                    // Queries

                    public const string Assert = "booking/events-forms/assert";
                    public const string Retrieve = "booking/events-forms/retrieve";

                    public const string Collect = "booking/events-forms/collect";
                    public const string Count = "booking/events-forms/count";
                    public const string Search = "booking/events-forms/search";
                    public const string Download = "booking/events-forms/download";
                }

                public static partial class EventSeat // Entity
                {
                    // Queries

                    public const string Assert = "booking/events-seats/assert";
                    public const string Retrieve = "booking/events-seats/retrieve";

                    public const string Collect = "booking/events-seats/collect";
                    public const string Count = "booking/events-seats/count";
                    public const string Search = "booking/events-seats/search";
                    public const string Download = "booking/events-seats/download";
                }

                public static partial class EventTimer // Entity
                {
                    // Queries

                    public const string Assert = "booking/events-timers/assert";
                    public const string Retrieve = "booking/events-timers/retrieve";

                    public const string Collect = "booking/events-timers/collect";
                    public const string Count = "booking/events-timers/count";
                    public const string Search = "booking/events-timers/search";
                    public const string Download = "booking/events-timers/download";
                }

                public static partial class EventUser // Entity
                {
                    // Queries

                    public const string Assert = "booking/events-users/assert";
                    public const string Retrieve = "booking/events-users/retrieve";

                    public const string Collect = "booking/events-users/collect";
                    public const string Count = "booking/events-users/count";
                    public const string Search = "booking/events-users/search";
                    public const string Download = "booking/events-users/download";
                }
            }

            public static partial class Registrations // Subcomponent
            {
                public static partial class Registration // Entity
                {
                    // Queries

                    public const string Assert = "booking/registrations/assert";
                    public const string Retrieve = "booking/registrations/retrieve";

                    public const string Collect = "booking/registrations/collect";
                    public const string Count = "booking/registrations/count";
                    public const string Search = "booking/registrations/search";
                    public const string Download = "booking/registrations/download";
                }

                public static partial class RegistrationAccommodation // Entity
                {
                    // Queries

                    public const string Assert = "booking/registrations-accommodations/assert";
                    public const string Retrieve = "booking/registrations-accommodations/retrieve";

                    public const string Collect = "booking/registrations-accommodations/collect";
                    public const string Count = "booking/registrations-accommodations/count";
                    public const string Search = "booking/registrations-accommodations/search";
                    public const string Download = "booking/registrations-accommodations/download";
                }

                public static partial class RegistrationInstructor // Entity
                {
                    // Queries

                    public const string Assert = "booking/registrations-instructors/assert";
                    public const string Retrieve = "booking/registrations-instructors/retrieve";

                    public const string Collect = "booking/registrations-instructors/collect";
                    public const string Count = "booking/registrations-instructors/count";
                    public const string Search = "booking/registrations-instructors/search";
                    public const string Download = "booking/registrations-instructors/download";
                }

                public static partial class RegistrationTimer // Entity
                {
                    // Queries

                    public const string Assert = "booking/registrations-timers/assert";
                    public const string Retrieve = "booking/registrations-timers/retrieve";

                    public const string Collect = "booking/registrations-timers/collect";
                    public const string Count = "booking/registrations-timers/count";
                    public const string Search = "booking/registrations-timers/search";
                    public const string Download = "booking/registrations-timers/download";
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

                    public const string Assert = "competency/documents/assert";
                    public const string Retrieve = "competency/documents/retrieve";

                    public const string Collect = "competency/documents/collect";
                    public const string Count = "competency/documents/count";
                    public const string Search = "competency/documents/search";
                    public const string Download = "competency/documents/download";
                }

                public static partial class DocumentCompetency // Entity
                {
                    // Queries

                    public const string Assert = "competency/documents-competencies/assert";
                    public const string Retrieve = "competency/documents-competencies/retrieve";

                    public const string Collect = "competency/documents-competencies/collect";
                    public const string Count = "competency/documents-competencies/count";
                    public const string Search = "competency/documents-competencies/search";
                    public const string Download = "competency/documents-competencies/download";
                }

                public static partial class DocumentConnection // Entity
                {
                    // Queries

                    public const string Assert = "competency/documents-connections/assert";
                    public const string Retrieve = "competency/documents-connections/retrieve";

                    public const string Collect = "competency/documents-connections/collect";
                    public const string Count = "competency/documents-connections/count";
                    public const string Search = "competency/documents-connections/search";
                    public const string Download = "competency/documents-connections/download";
                }
            }

            public static partial class Standards // Subcomponent
            {
                public static partial class ProfileGroupCompetency // Entity
                {
                    // Queries

                    public const string Assert = "competency/profiles-groups-competencies/assert";
                    public const string Retrieve = "competency/profiles-groups-competencies/retrieve";

                    public const string Collect = "competency/profiles-groups-competencies/collect";
                    public const string Count = "competency/profiles-groups-competencies/count";
                    public const string Search = "competency/profiles-groups-competencies/search";
                    public const string Download = "competency/profiles-groups-competencies/download";

                    // Commands

                    public const string Create = "competency/profiles-groups-competencies/create";
                    public const string Delete = "competency/profiles-groups-competencies/delete";
                    public const string Modify = "competency/profiles-groups-competencies/modify";
                }

                public static partial class ProfileGroupLearner // Entity
                {
                    // Queries

                    public const string Assert = "competency/profiles-groups-learners/assert";
                    public const string Retrieve = "competency/profiles-groups-learners/retrieve";

                    public const string Collect = "competency/profiles-groups-learners/collect";
                    public const string Count = "competency/profiles-groups-learners/count";
                    public const string Search = "competency/profiles-groups-learners/search";
                    public const string Download = "competency/profiles-groups-learners/download";

                    // Commands

                    public const string Create = "competency/profiles-groups-learners/create";
                    public const string Delete = "competency/profiles-groups-learners/delete";
                    public const string Modify = "competency/profiles-groups-learners/modify";
                }

                public static partial class Standard // Entity
                {
                    // Queries

                    public const string Assert = "competency/standards/assert";
                    public const string Retrieve = "competency/standards/retrieve";

                    public const string Collect = "competency/standards/collect";
                    public const string Count = "competency/standards/count";
                    public const string Search = "competency/standards/search";
                    public const string Download = "competency/standards/download";
                }

                public static partial class StandardAchievement // Entity
                {
                    // Queries

                    public const string Assert = "competency/standards-achievements/assert";
                    public const string Retrieve = "competency/standards-achievements/retrieve";

                    public const string Collect = "competency/standards-achievements/collect";
                    public const string Count = "competency/standards-achievements/count";
                    public const string Search = "competency/standards-achievements/search";
                    public const string Download = "competency/standards-achievements/download";
                }

                public static partial class StandardCategory // Entity
                {
                    // Queries

                    public const string Assert = "competency/standards-categories/assert";
                    public const string Retrieve = "competency/standards-categories/retrieve";

                    public const string Collect = "competency/standards-categories/collect";
                    public const string Count = "competency/standards-categories/count";
                    public const string Search = "competency/standards-categories/search";
                    public const string Download = "competency/standards-categories/download";
                }

                public static partial class StandardConnection // Entity
                {
                    // Queries

                    public const string Assert = "competency/standards-connections/assert";
                    public const string Retrieve = "competency/standards-connections/retrieve";

                    public const string Collect = "competency/standards-connections/collect";
                    public const string Count = "competency/standards-connections/count";
                    public const string Search = "competency/standards-connections/search";
                    public const string Download = "competency/standards-connections/download";
                }

                public static partial class StandardContainment // Entity
                {
                    // Queries

                    public const string Assert = "competency/standards-containments/assert";
                    public const string Retrieve = "competency/standards-containments/retrieve";

                    public const string Collect = "competency/standards-containments/collect";
                    public const string Count = "competency/standards-containments/count";
                    public const string Search = "competency/standards-containments/search";
                    public const string Download = "competency/standards-containments/download";
                }

                public static partial class StandardGroup // Entity
                {
                    // Queries

                    public const string Assert = "competency/standards-groups/assert";
                    public const string Retrieve = "competency/standards-groups/retrieve";

                    public const string Collect = "competency/standards-groups/collect";
                    public const string Count = "competency/standards-groups/count";
                    public const string Search = "competency/standards-groups/search";
                    public const string Download = "competency/standards-groups/download";
                }

                public static partial class StandardOrganization // Entity
                {
                    // Queries

                    public const string Assert = "competency/standards-organizations/assert";
                    public const string Retrieve = "competency/standards-organizations/retrieve";

                    public const string Collect = "competency/standards-organizations/collect";
                    public const string Count = "competency/standards-organizations/count";
                    public const string Search = "competency/standards-organizations/search";
                    public const string Download = "competency/standards-organizations/download";
                }
            }

            public static partial class Tiers // Subcomponent
            {
                public static partial class StandardTier // Entity
                {
                    // Queries

                    public const string Assert = "competency/standards-tiers/assert";
                    public const string Retrieve = "competency/standards-tiers/retrieve";

                    public const string Collect = "competency/standards-tiers/collect";
                    public const string Count = "competency/standards-tiers/count";
                    public const string Search = "competency/standards-tiers/search";
                    public const string Download = "competency/standards-tiers/download";
                }
            }

            public static partial class Validations // Subcomponent
            {
                public static partial class Validation // Entity
                {
                    // Queries

                    public const string Assert = "competency/validations/assert";
                    public const string Retrieve = "competency/validations/retrieve";

                    public const string Collect = "competency/validations/collect";
                    public const string Count = "competency/validations/count";
                    public const string Search = "competency/validations/search";
                    public const string Download = "competency/validations/download";
                }

                public static partial class ValidationChange // Entity
                {
                    // Queries

                    public const string Assert = "competency/validations-changes/assert";
                    public const string Retrieve = "competency/validations-changes/retrieve";

                    public const string Collect = "competency/validations-changes/collect";
                    public const string Count = "competency/validations-changes/count";
                    public const string Search = "competency/validations-changes/search";
                    public const string Download = "competency/validations-changes/download";
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

                    public const string Assert = "content/comments/assert";
                    public const string Retrieve = "content/comments/retrieve";

                    public const string Collect = "content/comments/collect";
                    public const string Count = "content/comments/count";
                    public const string Search = "content/comments/search";
                    public const string Download = "content/comments/download";
                }
            }

            public static partial class Files // Subcomponent
            {
                public static partial class File // Entity
                {
                    // Queries

                    public const string Assert = "content/files/assert";
                    public const string Retrieve = "content/files/retrieve";

                    public const string Collect = "content/files/collect";
                    public const string Count = "content/files/count";
                    public const string Search = "content/files/search";
                    public const string Download = "content/files/download";

                    // Commands

                    public const string Create = "content/files/create";
                    public const string Delete = "content/files/delete";
                    public const string Modify = "content/files/modify";
                }

                public static partial class FileActivity // Entity
                {
                    // Queries

                    public const string Assert = "content/files-activities/assert";
                    public const string Retrieve = "content/files-activities/retrieve";

                    public const string Collect = "content/files-activities/collect";
                    public const string Count = "content/files-activities/count";
                    public const string Search = "content/files-activities/search";
                    public const string Download = "content/files-activities/download";

                    // Commands

                    public const string Create = "content/files-activities/create";
                    public const string Delete = "content/files-activities/delete";
                    public const string Modify = "content/files-activities/modify";
                }

                public static partial class FileClaim // Entity
                {
                    // Queries

                    public const string Assert = "content/files-claims/assert";
                    public const string Retrieve = "content/files-claims/retrieve";

                    public const string Collect = "content/files-claims/collect";
                    public const string Count = "content/files-claims/count";
                    public const string Search = "content/files-claims/search";
                    public const string Download = "content/files-claims/download";

                    // Commands

                    public const string Create = "content/files-claims/create";
                    public const string Delete = "content/files-claims/delete";
                    public const string Modify = "content/files-claims/modify";
                }

                public static partial class Upload // Entity
                {
                    // Queries

                    public const string Assert = "content/uploads/assert";
                    public const string Retrieve = "content/uploads/retrieve";

                    public const string Collect = "content/uploads/collect";
                    public const string Count = "content/uploads/count";
                    public const string Search = "content/uploads/search";
                    public const string Download = "content/uploads/download";

                    // Commands

                    public const string Create = "content/uploads/create";
                    public const string Delete = "content/uploads/delete";
                    public const string Modify = "content/uploads/modify";
                }

                public static partial class UploadObject // Entity
                {
                    // Queries

                    public const string Assert = "content/uploads-objects/assert";
                    public const string Retrieve = "content/uploads-objects/retrieve";

                    public const string Collect = "content/uploads-objects/collect";
                    public const string Count = "content/uploads-objects/count";
                    public const string Search = "content/uploads-objects/search";
                    public const string Download = "content/uploads-objects/download";

                    // Commands

                    public const string Create = "content/uploads-objects/create";
                    public const string Delete = "content/uploads-objects/delete";
                    public const string Modify = "content/uploads-objects/modify";
                }
            }

            public static partial class Glossaries // Subcomponent
            {
                public static partial class GlossaryContent // Entity
                {
                    // Queries

                    public const string Assert = "content/glossaries-contents/assert";
                    public const string Retrieve = "content/glossaries-contents/retrieve";

                    public const string Collect = "content/glossaries-contents/collect";
                    public const string Count = "content/glossaries-contents/count";
                    public const string Search = "content/glossaries-contents/search";
                    public const string Download = "content/glossaries-contents/download";
                }

                public static partial class GlossaryTerm // Entity
                {
                    // Queries

                    public const string Assert = "content/glossaries-terms/assert";
                    public const string Retrieve = "content/glossaries-terms/retrieve";

                    public const string Collect = "content/glossaries-terms/collect";
                    public const string Count = "content/glossaries-terms/count";
                    public const string Search = "content/glossaries-terms/search";
                    public const string Download = "content/glossaries-terms/download";
                }
            }

            public static partial class Inputs // Subcomponent
            {
                public static partial class Input // Entity
                {
                    // Queries

                    public const string Assert = "content/inputs/assert";
                    public const string Retrieve = "content/inputs/retrieve";

                    public const string Collect = "content/inputs/collect";
                    public const string Count = "content/inputs/count";
                    public const string Search = "content/inputs/search";
                    public const string Download = "content/inputs/download";

                    // Commands

                    public const string Create = "content/inputs/create";
                    public const string Delete = "content/inputs/delete";
                    public const string Modify = "content/inputs/modify";
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

                    public const string Assert = "directory/groups/assert";
                    public const string Retrieve = "directory/groups/retrieve";

                    public const string Collect = "directory/groups/collect";
                    public const string Count = "directory/groups/count";
                    public const string Search = "directory/groups/search";
                    public const string Download = "directory/groups/download";
                }

                public static partial class GroupAddress // Entity
                {
                    // Queries

                    public const string Assert = "directory/groups-addresses/assert";
                    public const string Retrieve = "directory/groups-addresses/retrieve";

                    public const string Collect = "directory/groups-addresses/collect";
                    public const string Count = "directory/groups-addresses/count";
                    public const string Search = "directory/groups-addresses/search";
                    public const string Download = "directory/groups-addresses/download";
                }

                public static partial class GroupAttribute // Entity
                {
                    // Queries

                    public const string Assert = "directory/groups-attributes/assert";
                    public const string Retrieve = "directory/groups-attributes/retrieve";

                    public const string Collect = "directory/groups-attributes/collect";
                    public const string Count = "directory/groups-attributes/count";
                    public const string Search = "directory/groups-attributes/search";
                    public const string Download = "directory/groups-attributes/download";

                    // Commands

                    public const string Create = "directory/groups-attributes/create";
                    public const string Delete = "directory/groups-attributes/delete";
                    public const string Modify = "directory/groups-attributes/modify";
                }

                public static partial class GroupConnection // Entity
                {
                    // Queries

                    public const string Assert = "directory/groups-connections/assert";
                    public const string Retrieve = "directory/groups-connections/retrieve";

                    public const string Collect = "directory/groups-connections/collect";
                    public const string Count = "directory/groups-connections/count";
                    public const string Search = "directory/groups-connections/search";
                    public const string Download = "directory/groups-connections/download";
                }

                public static partial class GroupTag // Entity
                {
                    // Queries

                    public const string Assert = "directory/groups-tags/assert";
                    public const string Retrieve = "directory/groups-tags/retrieve";

                    public const string Collect = "directory/groups-tags/collect";
                    public const string Count = "directory/groups-tags/count";
                    public const string Search = "directory/groups-tags/search";
                    public const string Download = "directory/groups-tags/download";
                }
            }

            public static partial class Memberships // Subcomponent
            {
                public static partial class Membership // Entity
                {
                    // Queries

                    public const string Assert = "directory/memberships/assert";
                    public const string Retrieve = "directory/memberships/retrieve";

                    public const string Collect = "directory/memberships/collect";
                    public const string Count = "directory/memberships/count";
                    public const string Search = "directory/memberships/search";
                    public const string Download = "directory/memberships/download";
                }

                public static partial class MembershipDeletion // Entity
                {
                    // Queries

                    public const string Assert = "directory/memberships-deletions/assert";
                    public const string Retrieve = "directory/memberships-deletions/retrieve";

                    public const string Collect = "directory/memberships-deletions/collect";
                    public const string Count = "directory/memberships-deletions/count";
                    public const string Search = "directory/memberships-deletions/search";
                    public const string Download = "directory/memberships-deletions/download";
                }

                public static partial class MembershipReason // Entity
                {
                    // Queries

                    public const string Assert = "directory/memberships-reasons/assert";
                    public const string Retrieve = "directory/memberships-reasons/retrieve";

                    public const string Collect = "directory/memberships-reasons/collect";
                    public const string Count = "directory/memberships-reasons/count";
                    public const string Search = "directory/memberships-reasons/search";
                    public const string Download = "directory/memberships-reasons/download";
                }
            }

            public static partial class People // Subcomponent
            {
                public static partial class Person // Entity
                {
                    // Queries

                    public const string Assert = "directory/people/assert";
                    public const string Retrieve = "directory/people/retrieve";

                    public const string Collect = "directory/people/collect";
                    public const string Count = "directory/people/count";
                    public const string Search = "directory/people/search";
                    public const string Download = "directory/people/download";
                }

                public static partial class PersonAddress // Entity
                {
                    // Queries

                    public const string Assert = "directory/people-addresses/assert";
                    public const string Retrieve = "directory/people-addresses/retrieve";

                    public const string Collect = "directory/people-addresses/collect";
                    public const string Count = "directory/people-addresses/count";
                    public const string Search = "directory/people-addresses/search";
                    public const string Download = "directory/people-addresses/download";
                }

                public static partial class PersonAttribute // Entity
                {
                    // Queries

                    public const string Assert = "directory/people-attributes/assert";
                    public const string Retrieve = "directory/people-attributes/retrieve";

                    public const string Collect = "directory/people-attributes/collect";
                    public const string Count = "directory/people-attributes/count";
                    public const string Search = "directory/people-attributes/search";
                    public const string Download = "directory/people-attributes/download";

                    // Commands

                    public const string Create = "directory/people-attributes/create";
                    public const string Delete = "directory/people-attributes/delete";
                    public const string Modify = "directory/people-attributes/modify";
                }

                public static partial class PersonSecret // Entity
                {
                    // Queries

                    public const string Assert = "directory/people-secrets/assert";
                    public const string Retrieve = "directory/people-secrets/retrieve";

                    public const string Collect = "directory/people-secrets/collect";
                    public const string Count = "directory/people-secrets/count";
                    public const string Search = "directory/people-secrets/search";
                    public const string Download = "directory/people-secrets/download";
                }
            }
        }

        public static partial class Evaluation // Component
        {
            public static partial class Answers // Subcomponent
            {
                public static partial class Attempt // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/attempts/assert";
                    public const string Retrieve = "evaluation/attempts/retrieve";

                    public const string Collect = "evaluation/attempts/collect";
                    public const string Count = "evaluation/attempts/count";
                    public const string Search = "evaluation/attempts/search";
                    public const string Download = "evaluation/attempts/download";
                }

                public static partial class AttemptMatch // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/attempts-matches/assert";
                    public const string Retrieve = "evaluation/attempts-matches/retrieve";

                    public const string Collect = "evaluation/attempts-matches/collect";
                    public const string Count = "evaluation/attempts-matches/count";
                    public const string Search = "evaluation/attempts-matches/search";
                    public const string Download = "evaluation/attempts-matches/download";
                }

                public static partial class AttemptOption // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/attempts-options/assert";
                    public const string Retrieve = "evaluation/attempts-options/retrieve";

                    public const string Collect = "evaluation/attempts-options/collect";
                    public const string Count = "evaluation/attempts-options/count";
                    public const string Search = "evaluation/attempts-options/search";
                    public const string Download = "evaluation/attempts-options/download";
                }

                public static partial class AttemptPin // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/attempts-pins/assert";
                    public const string Retrieve = "evaluation/attempts-pins/retrieve";

                    public const string Collect = "evaluation/attempts-pins/collect";
                    public const string Count = "evaluation/attempts-pins/count";
                    public const string Search = "evaluation/attempts-pins/search";
                    public const string Download = "evaluation/attempts-pins/download";
                }

                public static partial class AttemptQuestion // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/attempts-questions/assert";
                    public const string Retrieve = "evaluation/attempts-questions/retrieve";

                    public const string Collect = "evaluation/attempts-questions/collect";
                    public const string Count = "evaluation/attempts-questions/count";
                    public const string Search = "evaluation/attempts-questions/search";
                    public const string Download = "evaluation/attempts-questions/download";
                }

                public static partial class AttemptSection // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/attempts-sections/assert";
                    public const string Retrieve = "evaluation/attempts-sections/retrieve";

                    public const string Collect = "evaluation/attempts-sections/collect";
                    public const string Count = "evaluation/attempts-sections/count";
                    public const string Search = "evaluation/attempts-sections/search";
                    public const string Download = "evaluation/attempts-sections/download";
                }

                public static partial class AttemptSolution // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/attempts-solutions/assert";
                    public const string Retrieve = "evaluation/attempts-solutions/retrieve";

                    public const string Collect = "evaluation/attempts-solutions/collect";
                    public const string Count = "evaluation/attempts-solutions/count";
                    public const string Search = "evaluation/attempts-solutions/search";
                    public const string Download = "evaluation/attempts-solutions/download";
                }

                public static partial class LearnerAssessment // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/learners-assessments/assert";
                    public const string Retrieve = "evaluation/learners-assessments/retrieve";

                    public const string Collect = "evaluation/learners-assessments/collect";
                    public const string Count = "evaluation/learners-assessments/count";
                    public const string Search = "evaluation/learners-assessments/search";
                    public const string Download = "evaluation/learners-assessments/download";

                    // Commands

                    public const string Create = "evaluation/learners-assessments/create";
                    public const string Delete = "evaluation/learners-assessments/delete";
                    public const string Modify = "evaluation/learners-assessments/modify";
                }

                public static partial class QuizAttempt // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/quizzes-attempts/assert";
                    public const string Retrieve = "evaluation/quizzes-attempts/retrieve";

                    public const string Collect = "evaluation/quizzes-attempts/collect";
                    public const string Count = "evaluation/quizzes-attempts/count";
                    public const string Search = "evaluation/quizzes-attempts/search";
                    public const string Download = "evaluation/quizzes-attempts/download";

                    // Commands

                    public const string Create = "evaluation/quizzes-attempts/create";
                    public const string Delete = "evaluation/quizzes-attempts/delete";
                    public const string Modify = "evaluation/quizzes-attempts/modify";
                }
            }

            public static partial class Questions // Subcomponent
            {
                public static partial class Assessment // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/assessments/assert";
                    public const string Retrieve = "evaluation/assessments/retrieve";

                    public const string Collect = "evaluation/assessments/collect";
                    public const string Count = "evaluation/assessments/count";
                    public const string Search = "evaluation/assessments/search";
                    public const string Download = "evaluation/assessments/download";
                }

                public static partial class AssessmentQuestionGradeitem // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/assessments-questions-gradeitems/assert";
                    public const string Retrieve = "evaluation/assessments-questions-gradeitems/retrieve";

                    public const string Collect = "evaluation/assessments-questions-gradeitems/collect";
                    public const string Count = "evaluation/assessments-questions-gradeitems/count";
                    public const string Search = "evaluation/assessments-questions-gradeitems/search";
                    public const string Download = "evaluation/assessments-questions-gradeitems/download";
                }

                public static partial class Bank // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/banks/assert";
                    public const string Retrieve = "evaluation/banks/retrieve";

                    public const string Collect = "evaluation/banks/collect";
                    public const string Count = "evaluation/banks/count";
                    public const string Search = "evaluation/banks/search";
                    public const string Download = "evaluation/banks/download";
                }

                public static partial class BankOption // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/banks-options/assert";
                    public const string Retrieve = "evaluation/banks-options/retrieve";

                    public const string Collect = "evaluation/banks-options/collect";
                    public const string Count = "evaluation/banks-options/count";
                    public const string Search = "evaluation/banks-options/search";
                    public const string Download = "evaluation/banks-options/download";
                }

                public static partial class BankQuestion // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/banks-questions/assert";
                    public const string Retrieve = "evaluation/banks-questions/retrieve";

                    public const string Collect = "evaluation/banks-questions/collect";
                    public const string Count = "evaluation/banks-questions/count";
                    public const string Search = "evaluation/banks-questions/search";
                    public const string Download = "evaluation/banks-questions/download";
                }

                public static partial class BankQuestionAttachment // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/banks-questions-attachments/assert";
                    public const string Retrieve = "evaluation/banks-questions-attachments/retrieve";

                    public const string Collect = "evaluation/banks-questions-attachments/collect";
                    public const string Count = "evaluation/banks-questions-attachments/count";
                    public const string Search = "evaluation/banks-questions-attachments/search";
                    public const string Download = "evaluation/banks-questions-attachments/download";
                }

                public static partial class BankQuestionCompetency // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/banks-questions-competencies/assert";
                    public const string Retrieve = "evaluation/banks-questions-competencies/retrieve";

                    public const string Collect = "evaluation/banks-questions-competencies/collect";
                    public const string Count = "evaluation/banks-questions-competencies/count";
                    public const string Search = "evaluation/banks-questions-competencies/search";
                    public const string Download = "evaluation/banks-questions-competencies/download";
                }

                public static partial class BankSpecification // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/banks-specifications/assert";
                    public const string Retrieve = "evaluation/banks-specifications/retrieve";

                    public const string Collect = "evaluation/banks-specifications/collect";
                    public const string Count = "evaluation/banks-specifications/count";
                    public const string Search = "evaluation/banks-specifications/search";
                    public const string Download = "evaluation/banks-specifications/download";
                }

                public static partial class Quiz // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/quizzes/assert";
                    public const string Retrieve = "evaluation/quizzes/retrieve";

                    public const string Collect = "evaluation/quizzes/collect";
                    public const string Count = "evaluation/quizzes/count";
                    public const string Search = "evaluation/quizzes/search";
                    public const string Download = "evaluation/quizzes/download";

                    // Commands

                    public const string Create = "evaluation/quizzes/create";
                    public const string Delete = "evaluation/quizzes/delete";
                    public const string Modify = "evaluation/quizzes/modify";
                }
            }

            public static partial class Rubrics // Subcomponent
            {
                public static partial class Rubric // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/rubrics/assert";
                    public const string Retrieve = "evaluation/rubrics/retrieve";

                    public const string Collect = "evaluation/rubrics/collect";
                    public const string Count = "evaluation/rubrics/count";
                    public const string Search = "evaluation/rubrics/search";
                    public const string Download = "evaluation/rubrics/download";
                }

                public static partial class RubricCriterion // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/rubrics-criteria/assert";
                    public const string Retrieve = "evaluation/rubrics-criteria/retrieve";

                    public const string Collect = "evaluation/rubrics-criteria/collect";
                    public const string Count = "evaluation/rubrics-criteria/count";
                    public const string Search = "evaluation/rubrics-criteria/search";
                    public const string Download = "evaluation/rubrics-criteria/download";
                }

                public static partial class RubricRating // Entity
                {
                    // Queries

                    public const string Assert = "evaluation/rubrics-ratings/assert";
                    public const string Retrieve = "evaluation/rubrics-ratings/retrieve";

                    public const string Collect = "evaluation/rubrics-ratings/collect";
                    public const string Count = "evaluation/rubrics-ratings/count";
                    public const string Search = "evaluation/rubrics-ratings/search";
                    public const string Download = "evaluation/rubrics-ratings/download";
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

                    public const string Assert = "integration/hub/api-calls/assert";
                    public const string Retrieve = "integration/hub/api-calls/retrieve";

                    public const string Collect = "integration/hub/api-calls/collect";
                    public const string Count = "integration/hub/api-calls/count";
                    public const string Search = "integration/hub/api-calls/search";
                    public const string Download = "integration/hub/api-calls/download";

                    // Commands

                    public const string Create = "integration/hub/api-calls/create";
                    public const string Delete = "integration/hub/api-calls/delete";
                    public const string Modify = "integration/hub/api-calls/modify";
                }
            }

            public static partial class Lti // Subcomponent
            {
                public static partial class LtiLink // Entity
                {
                    // Queries

                    public const string Assert = "integration/lti/links/assert";
                    public const string Retrieve = "integration/lti/links/retrieve";

                    public const string Collect = "integration/lti/links/collect";
                    public const string Count = "integration/lti/links/count";
                    public const string Search = "integration/lti/links/search";
                    public const string Download = "integration/lti/links/download";

                    // Commands

                    public const string Create = "integration/lti/links/create";
                    public const string Delete = "integration/lti/links/delete";
                    public const string Modify = "integration/lti/links/modify";
                }
            }

            public static partial class Moodle // Subcomponent
            {
                public static partial class MoodleChange // Entity
                {
                    // Queries

                    public const string Assert = "integration/moodle/changes/assert";
                    public const string Retrieve = "integration/moodle/changes/retrieve";

                    public const string Collect = "integration/moodle/changes/collect";
                    public const string Count = "integration/moodle/changes/count";
                    public const string Search = "integration/moodle/changes/search";
                    public const string Download = "integration/moodle/changes/download";

                    // Commands

                    public const string Create = "integration/moodle/changes/create";
                    public const string Delete = "integration/moodle/changes/delete";
                    public const string Modify = "integration/moodle/changes/modify";
                }
            }

            public static partial class Scorm // Subcomponent
            {
                public static partial class ScormChange // Entity
                {
                    // Queries

                    public const string Assert = "integration/scorm/changes/assert";
                    public const string Retrieve = "integration/scorm/changes/retrieve";

                    public const string Collect = "integration/scorm/changes/collect";
                    public const string Count = "integration/scorm/changes/count";
                    public const string Search = "integration/scorm/changes/search";
                    public const string Download = "integration/scorm/changes/download";

                    // Commands

                    public const string Create = "integration/scorm/changes/create";
                    public const string Delete = "integration/scorm/changes/delete";
                    public const string Modify = "integration/scorm/changes/modify";
                }

                public static partial class ScormRegistration // Entity
                {
                    // Queries

                    public const string Assert = "integration/scorm/registrations/assert";
                    public const string Retrieve = "integration/scorm/registrations/retrieve";

                    public const string Collect = "integration/scorm/registrations/collect";
                    public const string Count = "integration/scorm/registrations/count";
                    public const string Search = "integration/scorm/registrations/search";
                    public const string Download = "integration/scorm/registrations/download";

                    // Commands

                    public const string Create = "integration/scorm/registrations/create";
                    public const string Delete = "integration/scorm/registrations/delete";
                    public const string Modify = "integration/scorm/registrations/modify";
                }

                public static partial class ScormRegistrationActivity // Entity
                {
                    // Queries

                    public const string Assert = "integration/scorm/registrations-activities/assert";
                    public const string Retrieve = "integration/scorm/registrations-activities/retrieve";

                    public const string Collect = "integration/scorm/registrations-activities/collect";
                    public const string Count = "integration/scorm/registrations-activities/count";
                    public const string Search = "integration/scorm/registrations-activities/search";
                    public const string Download = "integration/scorm/registrations-activities/download";

                    // Commands

                    public const string Create = "integration/scorm/registrations-activities/create";
                    public const string Delete = "integration/scorm/registrations-activities/delete";
                    public const string Modify = "integration/scorm/registrations-activities/modify";
                }
            }

            public static partial class Xapi // Subcomponent
            {
                public static partial class XapiChange // Entity
                {
                    // Queries

                    public const string Assert = "integration/xapi/changes/assert";
                    public const string Retrieve = "integration/xapi/changes/retrieve";

                    public const string Collect = "integration/xapi/changes/collect";
                    public const string Count = "integration/xapi/changes/count";
                    public const string Search = "integration/xapi/changes/search";
                    public const string Download = "integration/xapi/changes/download";

                    // Commands

                    public const string Create = "integration/xapi/changes/create";
                    public const string Delete = "integration/xapi/changes/delete";
                    public const string Modify = "integration/xapi/changes/modify";
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

                    public const string Assert = "learning/activities/assert";
                    public const string Retrieve = "learning/activities/retrieve";

                    public const string Collect = "learning/activities/collect";
                    public const string Count = "learning/activities/count";
                    public const string Search = "learning/activities/search";
                    public const string Download = "learning/activities/download";
                }

                public static partial class ActivityCompetency // Entity
                {
                    // Queries

                    public const string Assert = "learning/activities-competencies/assert";
                    public const string Retrieve = "learning/activities-competencies/retrieve";

                    public const string Collect = "learning/activities-competencies/collect";
                    public const string Count = "learning/activities-competencies/count";
                    public const string Search = "learning/activities-competencies/search";
                    public const string Download = "learning/activities-competencies/download";
                }
            }

            public static partial class Catalogs // Subcomponent
            {
                public static partial class Catalog // Entity
                {
                    // Queries

                    public const string Assert = "learning/catalogs/assert";
                    public const string Retrieve = "learning/catalogs/retrieve";

                    public const string Collect = "learning/catalogs/collect";
                    public const string Count = "learning/catalogs/count";
                    public const string Search = "learning/catalogs/search";
                    public const string Download = "learning/catalogs/download";

                    // Commands

                    public const string Create = "learning/catalogs/create";
                    public const string Delete = "learning/catalogs/delete";
                    public const string Modify = "learning/catalogs/modify";
                }
            }

            public static partial class Courses // Subcomponent
            {
                public static partial class Course // Entity
                {
                    // Queries

                    public const string Assert = "learning/courses/assert";
                    public const string Retrieve = "learning/courses/retrieve";

                    public const string Collect = "learning/courses/collect";
                    public const string Count = "learning/courses/count";
                    public const string Search = "learning/courses/search";
                    public const string Download = "learning/courses/download";
                }

                public static partial class CourseCategory // Entity
                {
                    // Queries

                    public const string Assert = "learning/courses-categories/assert";
                    public const string Retrieve = "learning/courses-categories/retrieve";

                    public const string Collect = "learning/courses-categories/collect";
                    public const string Count = "learning/courses-categories/count";
                    public const string Search = "learning/courses-categories/search";
                    public const string Download = "learning/courses-categories/download";

                    // Commands

                    public const string Create = "learning/courses-categories/create";
                    public const string Delete = "learning/courses-categories/delete";
                    public const string Modify = "learning/courses-categories/modify";
                }
            }

            public static partial class Enrollments // Subcomponent
            {
                public static partial class CourseEnrollment // Entity
                {
                    // Queries

                    public const string Assert = "learning/courses-enrollments/assert";
                    public const string Retrieve = "learning/courses-enrollments/retrieve";

                    public const string Collect = "learning/courses-enrollments/collect";
                    public const string Count = "learning/courses-enrollments/count";
                    public const string Search = "learning/courses-enrollments/search";
                    public const string Download = "learning/courses-enrollments/download";
                }

                public static partial class ProgramEnrollment // Entity
                {
                    // Queries

                    public const string Assert = "learning/programs-enrollments/assert";
                    public const string Retrieve = "learning/programs-enrollments/retrieve";

                    public const string Collect = "learning/programs-enrollments/collect";
                    public const string Count = "learning/programs-enrollments/count";
                    public const string Search = "learning/programs-enrollments/search";
                    public const string Download = "learning/programs-enrollments/download";

                    // Commands

                    public const string Create = "learning/programs-enrollments/create";
                    public const string Delete = "learning/programs-enrollments/delete";
                    public const string Modify = "learning/programs-enrollments/modify";
                }

                public static partial class TaskEnrollment // Entity
                {
                    // Queries

                    public const string Assert = "learning/tasks-enrollments/assert";
                    public const string Retrieve = "learning/tasks-enrollments/retrieve";

                    public const string Collect = "learning/tasks-enrollments/collect";
                    public const string Count = "learning/tasks-enrollments/count";
                    public const string Search = "learning/tasks-enrollments/search";
                    public const string Download = "learning/tasks-enrollments/download";

                    // Commands

                    public const string Create = "learning/tasks-enrollments/create";
                    public const string Delete = "learning/tasks-enrollments/delete";
                    public const string Modify = "learning/tasks-enrollments/modify";
                }
            }

            public static partial class Modules // Subcomponent
            {
                public static partial class Module // Entity
                {
                    // Queries

                    public const string Assert = "learning/modules/assert";
                    public const string Retrieve = "learning/modules/retrieve";

                    public const string Collect = "learning/modules/collect";
                    public const string Count = "learning/modules/count";
                    public const string Search = "learning/modules/search";
                    public const string Download = "learning/modules/download";
                }
            }

            public static partial class Prerequisites // Subcomponent
            {
                public static partial class CoursePrerequisite // Entity
                {
                    // Queries

                    public const string Assert = "learning/courses-prerequisites/assert";
                    public const string Retrieve = "learning/courses-prerequisites/retrieve";

                    public const string Collect = "learning/courses-prerequisites/collect";
                    public const string Count = "learning/courses-prerequisites/count";
                    public const string Search = "learning/courses-prerequisites/search";
                    public const string Download = "learning/courses-prerequisites/download";
                }

                public static partial class Prerequisite // Entity
                {
                    // Queries

                    public const string Assert = "learning/prerequisites/assert";
                    public const string Retrieve = "learning/prerequisites/retrieve";

                    public const string Collect = "learning/prerequisites/collect";
                    public const string Count = "learning/prerequisites/count";
                    public const string Search = "learning/prerequisites/search";
                    public const string Download = "learning/prerequisites/download";

                    // Commands

                    public const string Create = "learning/prerequisites/create";
                    public const string Delete = "learning/prerequisites/delete";
                    public const string Modify = "learning/prerequisites/modify";
                }
            }

            public static partial class Programs // Subcomponent
            {
                public static partial class Program // Entity
                {
                    // Queries

                    public const string Assert = "learning/programs/assert";
                    public const string Retrieve = "learning/programs/retrieve";

                    public const string Collect = "learning/programs/collect";
                    public const string Count = "learning/programs/count";
                    public const string Search = "learning/programs/search";
                    public const string Download = "learning/programs/download";

                    // Commands

                    public const string Create = "learning/programs/create";
                    public const string Delete = "learning/programs/delete";
                    public const string Modify = "learning/programs/modify";
                }

                public static partial class Task // Entity
                {
                    // Queries

                    public const string Assert = "learning/tasks/assert";
                    public const string Retrieve = "learning/tasks/retrieve";

                    public const string Collect = "learning/tasks/collect";
                    public const string Count = "learning/tasks/count";
                    public const string Search = "learning/tasks/search";
                    public const string Download = "learning/tasks/download";

                    // Commands

                    public const string Create = "learning/tasks/create";
                    public const string Delete = "learning/tasks/delete";
                    public const string Modify = "learning/tasks/modify";
                }
            }

            public static partial class Units // Subcomponent
            {
                public static partial class Unit // Entity
                {
                    // Queries

                    public const string Assert = "learning/units/assert";
                    public const string Retrieve = "learning/units/retrieve";

                    public const string Collect = "learning/units/collect";
                    public const string Count = "learning/units/count";
                    public const string Search = "learning/units/search";
                    public const string Download = "learning/units/download";
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

                    public const string Assert = "messaging/mailouts/assert";
                    public const string Retrieve = "messaging/mailouts/retrieve";

                    public const string Collect = "messaging/mailouts/collect";
                    public const string Count = "messaging/mailouts/count";
                    public const string Search = "messaging/mailouts/search";
                    public const string Download = "messaging/mailouts/download";
                }

                public static partial class MailoutClick // Entity
                {
                    // Queries

                    public const string Assert = "messaging/mailouts-clicks/assert";
                    public const string Retrieve = "messaging/mailouts-clicks/retrieve";

                    public const string Collect = "messaging/mailouts-clicks/collect";
                    public const string Count = "messaging/mailouts-clicks/count";
                    public const string Search = "messaging/mailouts-clicks/search";
                    public const string Download = "messaging/mailouts-clicks/download";
                }

                public static partial class MailoutRecipient // Entity
                {
                    // Queries

                    public const string Assert = "messaging/mailouts-recipients/assert";
                    public const string Retrieve = "messaging/mailouts-recipients/retrieve";

                    public const string Collect = "messaging/mailouts-recipients/collect";
                    public const string Count = "messaging/mailouts-recipients/count";
                    public const string Search = "messaging/mailouts-recipients/search";
                    public const string Download = "messaging/mailouts-recipients/download";
                }

                public static partial class MailoutRecipientCopy // Entity
                {
                    // Queries

                    public const string Assert = "messaging/mailouts-recipients-copies/assert";
                    public const string Retrieve = "messaging/mailouts-recipients-copies/retrieve";

                    public const string Collect = "messaging/mailouts-recipients-copies/collect";
                    public const string Count = "messaging/mailouts-recipients-copies/count";
                    public const string Search = "messaging/mailouts-recipients-copies/search";
                    public const string Download = "messaging/mailouts-recipients-copies/download";
                }
            }

            public static partial class Messages // Subcomponent
            {
                public static partial class Message // Entity
                {
                    // Queries

                    public const string Assert = "messaging/messages/assert";
                    public const string Retrieve = "messaging/messages/retrieve";

                    public const string Collect = "messaging/messages/collect";
                    public const string Count = "messaging/messages/count";
                    public const string Search = "messaging/messages/search";
                    public const string Download = "messaging/messages/download";
                }

                public static partial class MessageLink // Entity
                {
                    // Queries

                    public const string Assert = "messaging/messages-links/assert";
                    public const string Retrieve = "messaging/messages-links/retrieve";

                    public const string Collect = "messaging/messages-links/collect";
                    public const string Count = "messaging/messages-links/count";
                    public const string Search = "messaging/messages-links/search";
                    public const string Download = "messaging/messages-links/download";
                }
            }

            public static partial class Subscribers // Subcomponent
            {
                public static partial class SubscriberFollower // Entity
                {
                    // Queries

                    public const string Assert = "messaging/subscribers-followers/assert";
                    public const string Retrieve = "messaging/subscribers-followers/retrieve";

                    public const string Collect = "messaging/subscribers-followers/collect";
                    public const string Count = "messaging/subscribers-followers/count";
                    public const string Search = "messaging/subscribers-followers/search";
                    public const string Download = "messaging/subscribers-followers/download";
                }

                public static partial class SubscriberGroup // Entity
                {
                    // Queries

                    public const string Assert = "messaging/subscribers-groups/assert";
                    public const string Retrieve = "messaging/subscribers-groups/retrieve";

                    public const string Collect = "messaging/subscribers-groups/collect";
                    public const string Count = "messaging/subscribers-groups/count";
                    public const string Search = "messaging/subscribers-groups/search";
                    public const string Download = "messaging/subscribers-groups/download";
                }

                public static partial class SubscriberUser // Entity
                {
                    // Queries

                    public const string Assert = "messaging/subscribers-users/assert";
                    public const string Retrieve = "messaging/subscribers-users/retrieve";

                    public const string Collect = "messaging/subscribers-users/collect";
                    public const string Count = "messaging/subscribers-users/count";
                    public const string Search = "messaging/subscribers-users/search";
                    public const string Download = "messaging/subscribers-users/download";
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

                    public const string Assert = "metadata/entities/assert";
                    public const string Retrieve = "metadata/entities/retrieve";

                    public const string Collect = "metadata/entities/collect";
                    public const string Count = "metadata/entities/count";
                    public const string Search = "metadata/entities/search";
                    public const string Download = "metadata/entities/download";

                    // Commands

                    public const string Create = "metadata/entities/create";
                    public const string Delete = "metadata/entities/delete";
                    public const string Modify = "metadata/entities/modify";
                }
            }

            public static partial class Sequences // Subcomponent
            {
                public static partial class Sequence // Entity
                {
                    // Queries

                    public const string Assert = "metadata/sequences/assert";
                    public const string Retrieve = "metadata/sequences/retrieve";

                    public const string Collect = "metadata/sequences/collect";
                    public const string Count = "metadata/sequences/count";
                    public const string Search = "metadata/sequences/search";
                    public const string Download = "metadata/sequences/download";

                    // Commands

                    public const string Create = "metadata/sequences/create";
                    public const string Delete = "metadata/sequences/delete";
                    public const string Modify = "metadata/sequences/modify";
                }
            }

            public static partial class Upgrades // Subcomponent
            {
                public static partial class Upgrade // Entity
                {
                    // Queries

                    public const string Assert = "metadata/upgrades/assert";
                    public const string Retrieve = "metadata/upgrades/retrieve";

                    public const string Collect = "metadata/upgrades/collect";
                    public const string Count = "metadata/upgrades/count";
                    public const string Search = "metadata/upgrades/search";
                    public const string Download = "metadata/upgrades/download";

                    // Commands

                    public const string Create = "metadata/upgrades/create";
                    public const string Delete = "metadata/upgrades/delete";
                    public const string Modify = "metadata/upgrades/modify";
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

                    public const string Assert = "progress/achievements/assert";
                    public const string Retrieve = "progress/achievements/retrieve";

                    public const string Collect = "progress/achievements/collect";
                    public const string Count = "progress/achievements/count";
                    public const string Search = "progress/achievements/search";
                    public const string Download = "progress/achievements/download";
                }

                public static partial class AchievementCategory // Entity
                {
                    // Queries

                    public const string Assert = "progress/achievements-categories/assert";
                    public const string Retrieve = "progress/achievements-categories/retrieve";

                    public const string Collect = "progress/achievements-categories/collect";
                    public const string Count = "progress/achievements-categories/count";
                    public const string Search = "progress/achievements-categories/search";
                    public const string Download = "progress/achievements-categories/download";

                    // Commands

                    public const string Create = "progress/achievements-categories/create";
                    public const string Delete = "progress/achievements-categories/delete";
                    public const string Modify = "progress/achievements-categories/modify";
                }

                public static partial class AchievementGroup // Entity
                {
                    // Queries

                    public const string Assert = "progress/achievements-groups/assert";
                    public const string Retrieve = "progress/achievements-groups/retrieve";

                    public const string Collect = "progress/achievements-groups/collect";
                    public const string Count = "progress/achievements-groups/count";
                    public const string Search = "progress/achievements-groups/search";
                    public const string Download = "progress/achievements-groups/download";

                    // Commands

                    public const string Create = "progress/achievements-groups/create";
                    public const string Delete = "progress/achievements-groups/delete";
                    public const string Modify = "progress/achievements-groups/modify";
                }

                public static partial class AchievementOrganization // Entity
                {
                    // Queries

                    public const string Assert = "progress/achievements-organizations/assert";
                    public const string Retrieve = "progress/achievements-organizations/retrieve";

                    public const string Collect = "progress/achievements-organizations/collect";
                    public const string Count = "progress/achievements-organizations/count";
                    public const string Search = "progress/achievements-organizations/search";
                    public const string Download = "progress/achievements-organizations/download";

                    // Commands

                    public const string Create = "progress/achievements-organizations/create";
                    public const string Delete = "progress/achievements-organizations/delete";
                    public const string Modify = "progress/achievements-organizations/modify";
                }

                public static partial class AchievementPrerequisite // Entity
                {
                    // Queries

                    public const string Assert = "progress/achievements-prerequisites/assert";
                    public const string Retrieve = "progress/achievements-prerequisites/retrieve";

                    public const string Collect = "progress/achievements-prerequisites/collect";
                    public const string Count = "progress/achievements-prerequisites/count";
                    public const string Search = "progress/achievements-prerequisites/search";
                    public const string Download = "progress/achievements-prerequisites/download";
                }
            }

            public static partial class Certificates // Subcomponent
            {
                public static partial class Certificate // Entity
                {
                    // Queries

                    public const string Assert = "progress/certificates/assert";
                    public const string Retrieve = "progress/certificates/retrieve";

                    public const string Collect = "progress/certificates/collect";
                    public const string Count = "progress/certificates/count";
                    public const string Search = "progress/certificates/search";
                    public const string Download = "progress/certificates/download";

                    // Commands

                    public const string Create = "progress/certificates/create";
                    public const string Delete = "progress/certificates/delete";
                    public const string Modify = "progress/certificates/modify";
                }
            }

            public static partial class Credentials // Subcomponent
            {
                public static partial class Credential // Entity
                {
                    // Queries

                    public const string Assert = "progress/credentials/assert";
                    public const string Retrieve = "progress/credentials/retrieve";

                    public const string Collect = "progress/credentials/collect";
                    public const string Count = "progress/credentials/count";
                    public const string Search = "progress/credentials/search";
                    public const string Download = "progress/credentials/download";
                }

                public static partial class CredentialChange // Entity
                {
                    // Queries

                    public const string Assert = "progress/credentials-changes/assert";
                    public const string Retrieve = "progress/credentials-changes/retrieve";

                    public const string Collect = "progress/credentials-changes/collect";
                    public const string Count = "progress/credentials-changes/count";
                    public const string Search = "progress/credentials-changes/search";
                    public const string Download = "progress/credentials-changes/download";
                }

                public static partial class LearnerExperience // Entity
                {
                    // Queries

                    public const string Assert = "progress/learners-experiences/assert";
                    public const string Retrieve = "progress/learners-experiences/retrieve";

                    public const string Collect = "progress/learners-experiences/collect";
                    public const string Count = "progress/learners-experiences/count";
                    public const string Search = "progress/learners-experiences/search";
                    public const string Download = "progress/learners-experiences/download";

                    // Commands

                    public const string Create = "progress/learners-experiences/create";
                    public const string Delete = "progress/learners-experiences/delete";
                    public const string Modify = "progress/learners-experiences/modify";
                }
            }

            public static partial class Gradebooks // Subcomponent
            {
                public static partial class Gradebook // Entity
                {
                    // Queries

                    public const string Assert = "progress/gradebooks/assert";
                    public const string Retrieve = "progress/gradebooks/retrieve";

                    public const string Collect = "progress/gradebooks/collect";
                    public const string Count = "progress/gradebooks/count";
                    public const string Search = "progress/gradebooks/search";
                    public const string Download = "progress/gradebooks/download";
                }

                public static partial class GradebookEvent // Entity
                {
                    // Queries

                    public const string Assert = "progress/gradebooks-events/assert";
                    public const string Retrieve = "progress/gradebooks-events/retrieve";

                    public const string Collect = "progress/gradebooks-events/collect";
                    public const string Count = "progress/gradebooks-events/count";
                    public const string Search = "progress/gradebooks-events/search";
                    public const string Download = "progress/gradebooks-events/download";
                }

                public static partial class Gradeitem // Entity
                {
                    // Queries

                    public const string Assert = "progress/gradeitems/assert";
                    public const string Retrieve = "progress/gradeitems/retrieve";

                    public const string Collect = "progress/gradeitems/collect";
                    public const string Count = "progress/gradeitems/count";
                    public const string Search = "progress/gradeitems/search";
                    public const string Download = "progress/gradeitems/download";
                }

                public static partial class GradeitemCompetency // Entity
                {
                    // Queries

                    public const string Assert = "progress/gradeitems-competencies/assert";
                    public const string Retrieve = "progress/gradeitems-competencies/retrieve";

                    public const string Collect = "progress/gradeitems-competencies/collect";
                    public const string Count = "progress/gradeitems-competencies/count";
                    public const string Search = "progress/gradeitems-competencies/search";
                    public const string Download = "progress/gradeitems-competencies/download";
                }
            }

            public static partial class Logbooks // Subcomponent
            {
                public static partial class Logbook // Entity
                {
                    // Queries

                    public const string Assert = "progress/logbooks/assert";
                    public const string Retrieve = "progress/logbooks/retrieve";

                    public const string Collect = "progress/logbooks/collect";
                    public const string Count = "progress/logbooks/count";
                    public const string Search = "progress/logbooks/search";
                    public const string Download = "progress/logbooks/download";
                }

                public static partial class LogbookCompetency // Entity
                {
                    // Queries

                    public const string Assert = "progress/logbooks-competencies/assert";
                    public const string Retrieve = "progress/logbooks-competencies/retrieve";

                    public const string Collect = "progress/logbooks-competencies/collect";
                    public const string Count = "progress/logbooks-competencies/count";
                    public const string Search = "progress/logbooks-competencies/search";
                    public const string Download = "progress/logbooks-competencies/download";
                }

                public static partial class LogbookExperience // Entity
                {
                    // Queries

                    public const string Assert = "progress/logbooks-experiences/assert";
                    public const string Retrieve = "progress/logbooks-experiences/retrieve";

                    public const string Collect = "progress/logbooks-experiences/collect";
                    public const string Count = "progress/logbooks-experiences/count";
                    public const string Search = "progress/logbooks-experiences/search";
                    public const string Download = "progress/logbooks-experiences/download";
                }

                public static partial class RegulationUser // Entity
                {
                    // Queries

                    public const string Assert = "progress/regulations-users/assert";
                    public const string Retrieve = "progress/regulations-users/retrieve";

                    public const string Collect = "progress/regulations-users/collect";
                    public const string Count = "progress/regulations-users/count";
                    public const string Search = "progress/regulations-users/search";
                    public const string Download = "progress/regulations-users/download";
                }
            }

            public static partial class Periods // Subcomponent
            {
                public static partial class Period // Entity
                {
                    // Queries

                    public const string Assert = "progress/periods/assert";
                    public const string Retrieve = "progress/periods/retrieve";

                    public const string Collect = "progress/periods/collect";
                    public const string Count = "progress/periods/count";
                    public const string Search = "progress/periods/search";
                    public const string Download = "progress/periods/download";
                }
            }

            public static partial class Progressions // Subcomponent
            {
                public static partial class EnrollmentChange // Entity
                {
                    // Queries

                    public const string Assert = "progress/enrollments-changes/assert";
                    public const string Retrieve = "progress/enrollments-changes/retrieve";

                    public const string Collect = "progress/enrollments-changes/collect";
                    public const string Count = "progress/enrollments-changes/count";
                    public const string Search = "progress/enrollments-changes/search";
                    public const string Download = "progress/enrollments-changes/download";
                }

                public static partial class GradebookEnrollment // Entity
                {
                    // Queries

                    public const string Assert = "progress/gradebooks-enrollments/assert";
                    public const string Retrieve = "progress/gradebooks-enrollments/retrieve";

                    public const string Collect = "progress/gradebooks-enrollments/collect";
                    public const string Count = "progress/gradebooks-enrollments/count";
                    public const string Search = "progress/gradebooks-enrollments/search";
                    public const string Download = "progress/gradebooks-enrollments/download";
                }

                public static partial class LearnerProgramSummary // Entity
                {
                    // Queries

                    public const string Assert = "progress/learners-programs-summaries/assert";
                    public const string Retrieve = "progress/learners-programs-summaries/retrieve";

                    public const string Collect = "progress/learners-programs-summaries/collect";
                    public const string Count = "progress/learners-programs-summaries/count";
                    public const string Search = "progress/learners-programs-summaries/search";
                    public const string Download = "progress/learners-programs-summaries/download";
                }

                public static partial class Progression // Entity
                {
                    // Queries

                    public const string Assert = "progress/progressions/assert";
                    public const string Retrieve = "progress/progressions/retrieve";

                    public const string Collect = "progress/progressions/collect";
                    public const string Count = "progress/progressions/count";
                    public const string Search = "progress/progressions/search";
                    public const string Download = "progress/progressions/download";
                }

                public static partial class ProgressionChange // Entity
                {
                    // Queries

                    public const string Assert = "progress/progressions-changes/assert";
                    public const string Retrieve = "progress/progressions-changes/retrieve";

                    public const string Collect = "progress/progressions-changes/collect";
                    public const string Count = "progress/progressions-changes/count";
                    public const string Search = "progress/progressions-changes/search";
                    public const string Download = "progress/progressions-changes/download";
                }

                public static partial class ProgressionValidation // Entity
                {
                    // Queries

                    public const string Assert = "progress/progressions-validations/assert";
                    public const string Retrieve = "progress/progressions-validations/retrieve";

                    public const string Collect = "progress/progressions-validations/collect";
                    public const string Count = "progress/progressions-validations/count";
                    public const string Search = "progress/progressions-validations/search";
                    public const string Download = "progress/progressions-validations/download";
                }
            }

            public static partial class Regulations // Subcomponent
            {
                public static partial class Regulation // Entity
                {
                    // Queries

                    public const string Assert = "progress/regulations/assert";
                    public const string Retrieve = "progress/regulations/retrieve";

                    public const string Collect = "progress/regulations/collect";
                    public const string Count = "progress/regulations/count";
                    public const string Search = "progress/regulations/search";
                    public const string Download = "progress/regulations/download";
                }

                public static partial class RegulationCompetency // Entity
                {
                    // Queries

                    public const string Assert = "progress/regulations-competencies/assert";
                    public const string Retrieve = "progress/regulations-competencies/retrieve";

                    public const string Collect = "progress/regulations-competencies/collect";
                    public const string Count = "progress/regulations-competencies/count";
                    public const string Search = "progress/regulations-competencies/search";
                    public const string Download = "progress/regulations-competencies/download";
                }

                public static partial class RegulationField // Entity
                {
                    // Queries

                    public const string Assert = "progress/regulations-fields/assert";
                    public const string Retrieve = "progress/regulations-fields/retrieve";

                    public const string Collect = "progress/regulations-fields/collect";
                    public const string Count = "progress/regulations-fields/count";
                    public const string Search = "progress/regulations-fields/search";
                    public const string Download = "progress/regulations-fields/download";
                }

                public static partial class RegulationGroup // Entity
                {
                    // Queries

                    public const string Assert = "progress/regulations-groups/assert";
                    public const string Retrieve = "progress/regulations-groups/retrieve";

                    public const string Collect = "progress/regulations-groups/collect";
                    public const string Count = "progress/regulations-groups/count";
                    public const string Search = "progress/regulations-groups/search";
                    public const string Download = "progress/regulations-groups/download";
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

                    public const string Assert = "reporting/measurements/assert";
                    public const string Retrieve = "reporting/measurements/retrieve";

                    public const string Collect = "reporting/measurements/collect";
                    public const string Count = "reporting/measurements/count";
                    public const string Search = "reporting/measurements/search";
                    public const string Download = "reporting/measurements/download";

                    // Commands

                    public const string Create = "reporting/measurements/create";
                    public const string Delete = "reporting/measurements/delete";
                    public const string Modify = "reporting/measurements/modify";
                }
            }

            public static partial class Reports // Subcomponent
            {
                public static partial class Report // Entity
                {
                    // Queries

                    public const string Assert = "reporting/reports/assert";
                    public const string Retrieve = "reporting/reports/retrieve";

                    public const string Collect = "reporting/reports/collect";
                    public const string Count = "reporting/reports/count";
                    public const string Search = "reporting/reports/search";
                    public const string Download = "reporting/reports/download";

                    // Commands

                    public const string Create = "reporting/reports/create";
                    public const string Delete = "reporting/reports/delete";
                    public const string Modify = "reporting/reports/modify";
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

                    public const string Assert = "security/developers/assert";
                    public const string Retrieve = "security/developers/retrieve";

                    public const string Collect = "security/developers/collect";
                    public const string Count = "security/developers/count";
                    public const string Search = "security/developers/search";
                    public const string Download = "security/developers/download";

                    // Commands

                    public const string Create = "security/developers/create";
                    public const string Delete = "security/developers/delete";
                    public const string Modify = "security/developers/modify";
                }
            }

            public static partial class Impersonations // Subcomponent
            {
                public static partial class Impersonation // Entity
                {
                    // Queries

                    public const string Assert = "security/impersonations/assert";
                    public const string Retrieve = "security/impersonations/retrieve";

                    public const string Collect = "security/impersonations/collect";
                    public const string Count = "security/impersonations/count";
                    public const string Search = "security/impersonations/search";
                    public const string Download = "security/impersonations/download";

                    // Commands

                    public const string Create = "security/impersonations/create";
                    public const string Delete = "security/impersonations/delete";
                    public const string Modify = "security/impersonations/modify";
                }
            }

            public static partial class Organizations // Subcomponent
            {
                public static partial class Organization // Entity
                {
                    // Queries

                    public const string Assert = "security/organizations/assert";
                    public const string Retrieve = "security/organizations/retrieve";

                    public const string Collect = "security/organizations/collect";
                    public const string Count = "security/organizations/count";
                    public const string Search = "security/organizations/search";
                    public const string Download = "security/organizations/download";
                }
            }

            public static partial class Permissions // Subcomponent
            {
                public static partial class Permission // Entity
                {
                    // Queries

                    public const string Assert = "security/permissions/assert";
                    public const string Retrieve = "security/permissions/retrieve";

                    public const string Collect = "security/permissions/collect";
                    public const string Count = "security/permissions/count";
                    public const string Search = "security/permissions/search";
                    public const string Download = "security/permissions/download";

                    // Commands

                    public const string Create = "security/permissions/create";
                    public const string Delete = "security/permissions/delete";
                    public const string Modify = "security/permissions/modify";
                }
            }

            public static partial class Users // Subcomponent
            {
                public static partial class User // Entity
                {
                    // Queries

                    public const string Assert = "security/users/assert";
                    public const string Retrieve = "security/users/retrieve";

                    public const string Collect = "security/users/collect";
                    public const string Count = "security/users/count";
                    public const string Search = "security/users/search";
                    public const string Download = "security/users/download";
                }

                public static partial class UserAttribute // Entity
                {
                    // Queries

                    public const string Assert = "security/users-attributes/assert";
                    public const string Retrieve = "security/users-attributes/retrieve";

                    public const string Collect = "security/users-attributes/collect";
                    public const string Count = "security/users-attributes/count";
                    public const string Search = "security/users-attributes/search";
                    public const string Download = "security/users-attributes/download";

                    // Commands

                    public const string Create = "security/users-attributes/create";
                    public const string Delete = "security/users-attributes/delete";
                    public const string Modify = "security/users-attributes/modify";
                }

                public static partial class UserConnection // Entity
                {
                    // Queries

                    public const string Assert = "security/users-connections/assert";
                    public const string Retrieve = "security/users-connections/retrieve";

                    public const string Collect = "security/users-connections/collect";
                    public const string Count = "security/users-connections/count";
                    public const string Search = "security/users-connections/search";
                    public const string Download = "security/users-connections/download";
                }

                public static partial class UserMock // Entity
                {
                    // Queries

                    public const string Assert = "security/users-mocks/assert";
                    public const string Retrieve = "security/users-mocks/retrieve";

                    public const string Collect = "security/users-mocks/collect";
                    public const string Count = "security/users-mocks/count";
                    public const string Search = "security/users-mocks/search";
                    public const string Download = "security/users-mocks/download";

                    // Commands

                    public const string Create = "security/users-mocks/create";
                    public const string Delete = "security/users-mocks/delete";
                    public const string Modify = "security/users-mocks/modify";
                }

                public static partial class UserSession // Entity
                {
                    // Queries

                    public const string Assert = "security/users-sessions/assert";
                    public const string Retrieve = "security/users-sessions/retrieve";

                    public const string Collect = "security/users-sessions/collect";
                    public const string Count = "security/users-sessions/count";
                    public const string Search = "security/users-sessions/search";
                    public const string Download = "security/users-sessions/download";

                    // Commands

                    public const string Create = "security/users-sessions/create";
                    public const string Delete = "security/users-sessions/delete";
                    public const string Modify = "security/users-sessions/modify";
                }

                public static partial class UserSessionCache // Entity
                {
                    // Queries

                    public const string Assert = "security/users-sessions-caches/assert";
                    public const string Retrieve = "security/users-sessions-caches/retrieve";

                    public const string Collect = "security/users-sessions-caches/collect";
                    public const string Count = "security/users-sessions-caches/count";
                    public const string Search = "security/users-sessions-caches/search";
                    public const string Download = "security/users-sessions-caches/download";

                    // Commands

                    public const string Create = "security/users-sessions-caches/create";
                    public const string Delete = "security/users-sessions-caches/delete";
                    public const string Modify = "security/users-sessions-caches/modify";
                }

                public static partial class UserToken // Entity
                {
                    // Queries

                    public const string Assert = "security/users-tokens/assert";
                    public const string Retrieve = "security/users-tokens/retrieve";

                    public const string Collect = "security/users-tokens/collect";
                    public const string Count = "security/users-tokens/count";
                    public const string Search = "security/users-tokens/search";
                    public const string Download = "security/users-tokens/download";

                    // Commands

                    public const string Create = "security/users-tokens/create";
                    public const string Delete = "security/users-tokens/delete";
                    public const string Modify = "security/users-tokens/modify";
                }
            }
        }

        public static partial class Setup // Component
        {
            public static partial class Lookups // Subcomponent
            {
                public static partial class LookupItem // Entity
                {
                    // Queries

                    public const string Assert = "setup/lookups-items/assert";
                    public const string Retrieve = "setup/lookups-items/retrieve";

                    public const string Collect = "setup/lookups-items/collect";
                    public const string Count = "setup/lookups-items/count";
                    public const string Search = "setup/lookups-items/search";
                    public const string Download = "setup/lookups-items/download";

                    // Commands

                    public const string Create = "setup/lookups-items/create";
                    public const string Delete = "setup/lookups-items/delete";
                    public const string Modify = "setup/lookups-items/modify";
                }

                public static partial class LookupList // Entity
                {
                    // Queries

                    public const string Assert = "setup/lookups-lists/assert";
                    public const string Retrieve = "setup/lookups-lists/retrieve";

                    public const string Collect = "setup/lookups-lists/collect";
                    public const string Count = "setup/lookups-lists/count";
                    public const string Search = "setup/lookups-lists/search";
                    public const string Download = "setup/lookups-lists/download";

                    // Commands

                    public const string Create = "setup/lookups-lists/create";
                    public const string Delete = "setup/lookups-lists/delete";
                    public const string Modify = "setup/lookups-lists/modify";
                }
            }

            public static partial class Partitions // Subcomponent
            {
                public static partial class PartitionSetting // Entity
                {
                    // Queries

                    public const string Assert = "setup/partitions-settings/assert";
                    public const string Retrieve = "setup/partitions-settings/retrieve";

                    public const string Collect = "setup/partitions-settings/collect";
                    public const string Count = "setup/partitions-settings/count";
                    public const string Search = "setup/partitions-settings/search";
                    public const string Download = "setup/partitions-settings/download";

                    // Commands

                    public const string Create = "setup/partitions-settings/create";
                    public const string Delete = "setup/partitions-settings/delete";
                    public const string Modify = "setup/partitions-settings/modify";
                }
            }

            public static partial class Routes // Subcomponent
            {
                public static partial class Action // Entity
                {
                    // Queries

                    public const string Assert = "setup/actions/assert";
                    public const string Retrieve = "setup/actions/retrieve";

                    public const string Collect = "setup/actions/collect";
                    public const string Count = "setup/actions/count";
                    public const string Search = "setup/actions/search";
                    public const string Download = "setup/actions/download";

                    // Commands

                    public const string Create = "setup/actions/create";
                    public const string Delete = "setup/actions/delete";
                    public const string Modify = "setup/actions/modify";
                }
            }

            public static partial class Senders // Subcomponent
            {
                public static partial class Sender // Entity
                {
                    // Queries

                    public const string Assert = "setup/senders/assert";
                    public const string Retrieve = "setup/senders/retrieve";

                    public const string Collect = "setup/senders/collect";
                    public const string Count = "setup/senders/count";
                    public const string Search = "setup/senders/search";
                    public const string Download = "setup/senders/download";

                    // Commands

                    public const string Create = "setup/senders/create";
                    public const string Delete = "setup/senders/delete";
                    public const string Modify = "setup/senders/modify";
                }

                public static partial class SenderOrganization // Entity
                {
                    // Queries

                    public const string Assert = "setup/senders-organizations/assert";
                    public const string Retrieve = "setup/senders-organizations/retrieve";

                    public const string Collect = "setup/senders-organizations/collect";
                    public const string Count = "setup/senders-organizations/count";
                    public const string Search = "setup/senders-organizations/search";
                    public const string Download = "setup/senders-organizations/download";

                    // Commands

                    public const string Create = "setup/senders-organizations/create";
                    public const string Delete = "setup/senders-organizations/delete";
                    public const string Modify = "setup/senders-organizations/modify";
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

                    public const string Assert = "timeline/aggregates/assert";
                    public const string Retrieve = "timeline/aggregates/retrieve";

                    public const string Collect = "timeline/aggregates/collect";
                    public const string Count = "timeline/aggregates/count";
                    public const string Search = "timeline/aggregates/search";
                    public const string Download = "timeline/aggregates/download";

                    // Commands

                    public const string Create = "timeline/aggregates/create";
                    public const string Delete = "timeline/aggregates/delete";
                    public const string Modify = "timeline/aggregates/modify";
                }

                public static partial class Change // Entity
                {
                    // Queries

                    public const string Assert = "timeline/changes/assert";
                    public const string Retrieve = "timeline/changes/retrieve";

                    public const string Collect = "timeline/changes/collect";
                    public const string Count = "timeline/changes/count";
                    public const string Search = "timeline/changes/search";
                    public const string Download = "timeline/changes/download";

                    // Commands

                    public const string Create = "timeline/changes/create";
                    public const string Delete = "timeline/changes/delete";
                    public const string Modify = "timeline/changes/modify";
                }

                public static partial class Snapshot // Entity
                {
                    // Queries

                    public const string Assert = "timeline/snapshots/assert";
                    public const string Retrieve = "timeline/snapshots/retrieve";

                    public const string Collect = "timeline/snapshots/collect";
                    public const string Count = "timeline/snapshots/count";
                    public const string Search = "timeline/snapshots/search";
                    public const string Download = "timeline/snapshots/download";

                    // Commands

                    public const string Create = "timeline/snapshots/create";
                    public const string Delete = "timeline/snapshots/delete";
                    public const string Modify = "timeline/snapshots/modify";
                }
            }

            public static partial class Commands // Subcomponent
            {
                public static partial class Command // Entity
                {
                    // Queries

                    public const string Assert = "timeline/commands/assert";
                    public const string Retrieve = "timeline/commands/retrieve";

                    public const string Collect = "timeline/commands/collect";
                    public const string Count = "timeline/commands/count";
                    public const string Search = "timeline/commands/search";
                    public const string Download = "timeline/commands/download";

                    // Commands

                    public const string Create = "timeline/commands/create";
                    public const string Delete = "timeline/commands/delete";
                    public const string Modify = "timeline/commands/modify";
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

                    public const string Assert = "variant/cmds/invoices/assert";
                    public const string Retrieve = "variant/cmds/invoices/retrieve";

                    public const string Collect = "variant/cmds/invoices/collect";
                    public const string Count = "variant/cmds/invoices/count";
                    public const string Search = "variant/cmds/invoices/search";
                    public const string Download = "variant/cmds/invoices/download";
                }

                public static partial class CmdsInvoiceFee // Entity
                {
                    // Queries

                    public const string Assert = "variant/cmds/invoices-fees/assert";
                    public const string Retrieve = "variant/cmds/invoices-fees/retrieve";

                    public const string Collect = "variant/cmds/invoices-fees/collect";
                    public const string Count = "variant/cmds/invoices-fees/count";
                    public const string Search = "variant/cmds/invoices-fees/search";
                    public const string Download = "variant/cmds/invoices-fees/download";
                }

                public static partial class CmdsInvoiceItem // Entity
                {
                    // Queries

                    public const string Assert = "variant/cmds/invoices-items/assert";
                    public const string Retrieve = "variant/cmds/invoices-items/retrieve";

                    public const string Collect = "variant/cmds/invoices-items/collect";
                    public const string Count = "variant/cmds/invoices-items/count";
                    public const string Search = "variant/cmds/invoices-items/search";
                    public const string Download = "variant/cmds/invoices-items/download";
                }

                public static partial class CollegeCertificate // Entity
                {
                    // Queries

                    public const string Assert = "variant/cmds/colleges-certificates/assert";
                    public const string Retrieve = "variant/cmds/colleges-certificates/retrieve";

                    public const string Collect = "variant/cmds/colleges-certificates/collect";
                    public const string Count = "variant/cmds/colleges-certificates/count";
                    public const string Search = "variant/cmds/colleges-certificates/search";
                    public const string Download = "variant/cmds/colleges-certificates/download";

                    // Commands

                    public const string Create = "variant/cmds/colleges-certificates/create";
                    public const string Delete = "variant/cmds/colleges-certificates/delete";
                    public const string Modify = "variant/cmds/colleges-certificates/modify";
                }

                public static partial class LearnerMeasurement // Entity
                {
                    // Queries

                    public const string Assert = "variant/cmds/learners-measurements/assert";
                    public const string Retrieve = "variant/cmds/learners-measurements/retrieve";

                    public const string Collect = "variant/cmds/learners-measurements/collect";
                    public const string Count = "variant/cmds/learners-measurements/count";
                    public const string Search = "variant/cmds/learners-measurements/search";
                    public const string Download = "variant/cmds/learners-measurements/download";

                    // Commands

                    public const string Create = "variant/cmds/learners-measurements/create";
                    public const string Delete = "variant/cmds/learners-measurements/delete";
                    public const string Modify = "variant/cmds/learners-measurements/modify";
                }

                public static partial class LearnerSnapshot // Entity
                {
                    // Queries

                    public const string Assert = "variant/cmds/learners-snapshots/assert";
                    public const string Retrieve = "variant/cmds/learners-snapshots/retrieve";

                    public const string Collect = "variant/cmds/learners-snapshots/collect";
                    public const string Count = "variant/cmds/learners-snapshots/count";
                    public const string Search = "variant/cmds/learners-snapshots/search";
                    public const string Download = "variant/cmds/learners-snapshots/download";

                    // Commands

                    public const string Create = "variant/cmds/learners-snapshots/create";
                    public const string Delete = "variant/cmds/learners-snapshots/delete";
                    public const string Modify = "variant/cmds/learners-snapshots/modify";
                }

                public static partial class LearnerSnapshotSummary // Entity
                {
                    // Queries

                    public const string Assert = "variant/cmds/learners-snapshots-summaries/assert";
                    public const string Retrieve = "variant/cmds/learners-snapshots-summaries/retrieve";

                    public const string Collect = "variant/cmds/learners-snapshots-summaries/collect";
                    public const string Count = "variant/cmds/learners-snapshots-summaries/count";
                    public const string Search = "variant/cmds/learners-snapshots-summaries/search";
                    public const string Download = "variant/cmds/learners-snapshots-summaries/download";

                    // Commands

                    public const string Create = "variant/cmds/learners-snapshots-summaries/create";
                    public const string Delete = "variant/cmds/learners-snapshots-summaries/delete";
                    public const string Modify = "variant/cmds/learners-snapshots-summaries/modify";
                }

                public static partial class LearnerSummary // Entity
                {
                    // Queries

                    public const string Assert = "variant/cmds/learners-summaries/assert";
                    public const string Retrieve = "variant/cmds/learners-summaries/retrieve";

                    public const string Collect = "variant/cmds/learners-summaries/collect";
                    public const string Count = "variant/cmds/learners-summaries/count";
                    public const string Search = "variant/cmds/learners-summaries/search";
                    public const string Download = "variant/cmds/learners-summaries/download";
                }
            }

            public static partial class NCSHA // Subcomponent
            {
                public static partial class Counter // Entity
                {
                    // Queries

                    public const string Assert = "variant/ncsha/counters/assert";
                    public const string Retrieve = "variant/ncsha/counters/retrieve";

                    public const string Collect = "variant/ncsha/counters/collect";
                    public const string Count = "variant/ncsha/counters/count";
                    public const string Search = "variant/ncsha/counters/search";
                    public const string Download = "variant/ncsha/counters/download";

                    // Commands

                    public const string Create = "variant/ncsha/counters/create";
                    public const string Delete = "variant/ncsha/counters/delete";
                    public const string Modify = "variant/ncsha/counters/modify";
                }

                public static partial class ReportChange // Entity
                {
                    // Queries

                    public const string Assert = "variant/ncsha/reports-changes/assert";
                    public const string Retrieve = "variant/ncsha/reports-changes/retrieve";

                    public const string Collect = "variant/ncsha/reports-changes/collect";
                    public const string Count = "variant/ncsha/reports-changes/count";
                    public const string Search = "variant/ncsha/reports-changes/search";
                    public const string Download = "variant/ncsha/reports-changes/download";

                    // Commands

                    public const string Create = "variant/ncsha/reports-changes/create";
                    public const string Delete = "variant/ncsha/reports-changes/delete";
                    public const string Modify = "variant/ncsha/reports-changes/modify";
                }

                public static partial class ReportField // Entity
                {
                    // Queries

                    public const string Assert = "variant/ncsha/reports-fields/assert";
                    public const string Retrieve = "variant/ncsha/reports-fields/retrieve";

                    public const string Collect = "variant/ncsha/reports-fields/collect";
                    public const string Count = "variant/ncsha/reports-fields/count";
                    public const string Search = "variant/ncsha/reports-fields/search";
                    public const string Download = "variant/ncsha/reports-fields/download";

                    // Commands

                    public const string Create = "variant/ncsha/reports-fields/create";
                    public const string Delete = "variant/ncsha/reports-fields/delete";
                    public const string Modify = "variant/ncsha/reports-fields/modify";
                }

                public static partial class ReportFilter // Entity
                {
                    // Queries

                    public const string Assert = "variant/ncsha/reports-filters/assert";
                    public const string Retrieve = "variant/ncsha/reports-filters/retrieve";

                    public const string Collect = "variant/ncsha/reports-filters/collect";
                    public const string Count = "variant/ncsha/reports-filters/count";
                    public const string Search = "variant/ncsha/reports-filters/search";
                    public const string Download = "variant/ncsha/reports-filters/download";

                    // Commands

                    public const string Create = "variant/ncsha/reports-filters/create";
                    public const string Delete = "variant/ncsha/reports-filters/delete";
                    public const string Modify = "variant/ncsha/reports-filters/modify";
                }

                public static partial class ReportFolder // Entity
                {
                    // Queries

                    public const string Assert = "variant/ncsha/reports-folders/assert";
                    public const string Retrieve = "variant/ncsha/reports-folders/retrieve";

                    public const string Collect = "variant/ncsha/reports-folders/collect";
                    public const string Count = "variant/ncsha/reports-folders/count";
                    public const string Search = "variant/ncsha/reports-folders/search";
                    public const string Download = "variant/ncsha/reports-folders/download";

                    // Commands

                    public const string Create = "variant/ncsha/reports-folders/create";
                    public const string Delete = "variant/ncsha/reports-folders/delete";
                    public const string Modify = "variant/ncsha/reports-folders/modify";
                }

                public static partial class ReportFolderComment // Entity
                {
                    // Queries

                    public const string Assert = "variant/ncsha/reports-folders-comments/assert";
                    public const string Retrieve = "variant/ncsha/reports-folders-comments/retrieve";

                    public const string Collect = "variant/ncsha/reports-folders-comments/collect";
                    public const string Count = "variant/ncsha/reports-folders-comments/count";
                    public const string Search = "variant/ncsha/reports-folders-comments/search";
                    public const string Download = "variant/ncsha/reports-folders-comments/download";

                    // Commands

                    public const string Create = "variant/ncsha/reports-folders-comments/create";
                    public const string Delete = "variant/ncsha/reports-folders-comments/delete";
                    public const string Modify = "variant/ncsha/reports-folders-comments/modify";
                }

                public static partial class ReportMapping // Entity
                {
                    // Queries

                    public const string Assert = "variant/ncsha/reports-mappings/assert";
                    public const string Retrieve = "variant/ncsha/reports-mappings/retrieve";

                    public const string Collect = "variant/ncsha/reports-mappings/collect";
                    public const string Count = "variant/ncsha/reports-mappings/count";
                    public const string Search = "variant/ncsha/reports-mappings/search";
                    public const string Download = "variant/ncsha/reports-mappings/download";

                    // Commands

                    public const string Create = "variant/ncsha/reports-mappings/create";
                    public const string Delete = "variant/ncsha/reports-mappings/delete";
                    public const string Modify = "variant/ncsha/reports-mappings/modify";
                }

                public static partial class SurveyAB // Entity
                {
                    // Queries

                    public const string Assert = "variant/ncsha/surveys-ab/assert";
                    public const string Retrieve = "variant/ncsha/surveys-ab/retrieve";

                    public const string Collect = "variant/ncsha/surveys-ab/collect";
                    public const string Count = "variant/ncsha/surveys-ab/count";
                    public const string Search = "variant/ncsha/surveys-ab/search";
                    public const string Download = "variant/ncsha/surveys-ab/download";

                    // Commands

                    public const string Create = "variant/ncsha/surveys-ab/create";
                    public const string Delete = "variant/ncsha/surveys-ab/delete";
                    public const string Modify = "variant/ncsha/surveys-ab/modify";
                }

                public static partial class SurveyHC // Entity
                {
                    // Queries

                    public const string Assert = "variant/ncsha/surveys-hc/assert";
                    public const string Retrieve = "variant/ncsha/surveys-hc/retrieve";

                    public const string Collect = "variant/ncsha/surveys-hc/collect";
                    public const string Count = "variant/ncsha/surveys-hc/count";
                    public const string Search = "variant/ncsha/surveys-hc/search";
                    public const string Download = "variant/ncsha/surveys-hc/download";

                    // Commands

                    public const string Create = "variant/ncsha/surveys-hc/create";
                    public const string Delete = "variant/ncsha/surveys-hc/delete";
                    public const string Modify = "variant/ncsha/surveys-hc/modify";
                }

                public static partial class SurveyHI // Entity
                {
                    // Queries

                    public const string Assert = "variant/ncsha/surveys-hi/assert";
                    public const string Retrieve = "variant/ncsha/surveys-hi/retrieve";

                    public const string Collect = "variant/ncsha/surveys-hi/collect";
                    public const string Count = "variant/ncsha/surveys-hi/count";
                    public const string Search = "variant/ncsha/surveys-hi/search";
                    public const string Download = "variant/ncsha/surveys-hi/download";

                    // Commands

                    public const string Create = "variant/ncsha/surveys-hi/create";
                    public const string Delete = "variant/ncsha/surveys-hi/delete";
                    public const string Modify = "variant/ncsha/surveys-hi/modify";
                }

                public static partial class SurveyMF // Entity
                {
                    // Queries

                    public const string Assert = "variant/ncsha/surveys-mf/assert";
                    public const string Retrieve = "variant/ncsha/surveys-mf/retrieve";

                    public const string Collect = "variant/ncsha/surveys-mf/collect";
                    public const string Count = "variant/ncsha/surveys-mf/count";
                    public const string Search = "variant/ncsha/surveys-mf/search";
                    public const string Download = "variant/ncsha/surveys-mf/download";

                    // Commands

                    public const string Create = "variant/ncsha/surveys-mf/create";
                    public const string Delete = "variant/ncsha/surveys-mf/delete";
                    public const string Modify = "variant/ncsha/surveys-mf/modify";
                }

                public static partial class SurveyMR // Entity
                {
                    // Queries

                    public const string Assert = "variant/ncsha/surveys-mr/assert";
                    public const string Retrieve = "variant/ncsha/surveys-mr/retrieve";

                    public const string Collect = "variant/ncsha/surveys-mr/collect";
                    public const string Count = "variant/ncsha/surveys-mr/count";
                    public const string Search = "variant/ncsha/surveys-mr/search";
                    public const string Download = "variant/ncsha/surveys-mr/download";

                    // Commands

                    public const string Create = "variant/ncsha/surveys-mr/create";
                    public const string Delete = "variant/ncsha/surveys-mr/delete";
                    public const string Modify = "variant/ncsha/surveys-mr/modify";
                }

                public static partial class SurveyPA // Entity
                {
                    // Queries

                    public const string Assert = "variant/ncsha/surveys-pa/assert";
                    public const string Retrieve = "variant/ncsha/surveys-pa/retrieve";

                    public const string Collect = "variant/ncsha/surveys-pa/collect";
                    public const string Count = "variant/ncsha/surveys-pa/count";
                    public const string Search = "variant/ncsha/surveys-pa/search";
                    public const string Download = "variant/ncsha/surveys-pa/download";

                    // Commands

                    public const string Create = "variant/ncsha/surveys-pa/create";
                    public const string Delete = "variant/ncsha/surveys-pa/delete";
                    public const string Modify = "variant/ncsha/surveys-pa/modify";
                }
            }

            public static partial class SkilledTradesBC // Subcomponent
            {
                public static partial class Distribution // Entity
                {
                    // Queries

                    public const string Assert = "variant/skilledtradesbc/distributions/assert";
                    public const string Retrieve = "variant/skilledtradesbc/distributions/retrieve";

                    public const string Collect = "variant/skilledtradesbc/distributions/collect";
                    public const string Count = "variant/skilledtradesbc/distributions/count";
                    public const string Search = "variant/skilledtradesbc/distributions/search";
                    public const string Download = "variant/skilledtradesbc/distributions/download";

                    // Commands

                    public const string Create = "variant/skilledtradesbc/distributions/create";
                    public const string Delete = "variant/skilledtradesbc/distributions/delete";
                    public const string Modify = "variant/skilledtradesbc/distributions/modify";
                }

                public static partial class Individual // Entity
                {
                    // Queries

                    public const string Assert = "variant/skilledtradesbc/individuals/assert";
                    public const string Retrieve = "variant/skilledtradesbc/individuals/retrieve";

                    public const string Collect = "variant/skilledtradesbc/individuals/collect";
                    public const string Count = "variant/skilledtradesbc/individuals/count";
                    public const string Search = "variant/skilledtradesbc/individuals/search";
                    public const string Download = "variant/skilledtradesbc/individuals/download";

                    // Commands

                    public const string Create = "variant/skilledtradesbc/individuals/create";
                    public const string Delete = "variant/skilledtradesbc/individuals/delete";
                    public const string Modify = "variant/skilledtradesbc/individuals/modify";
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

                    public const string Assert = "workflow/cases/assert";
                    public const string Retrieve = "workflow/cases/retrieve";

                    public const string Collect = "workflow/cases/collect";
                    public const string Count = "workflow/cases/count";
                    public const string Search = "workflow/cases/search";
                    public const string Download = "workflow/cases/download";
                }

                public static partial class CaseDocument // Entity
                {
                    // Queries

                    public const string Assert = "workflow/cases-documents/assert";
                    public const string Retrieve = "workflow/cases-documents/retrieve";

                    public const string Collect = "workflow/cases-documents/collect";
                    public const string Count = "workflow/cases-documents/count";
                    public const string Search = "workflow/cases-documents/search";
                    public const string Download = "workflow/cases-documents/download";
                }

                public static partial class CaseDocumentRequest // Entity
                {
                    // Queries

                    public const string Assert = "workflow/cases-documents-requests/assert";
                    public const string Retrieve = "workflow/cases-documents-requests/retrieve";

                    public const string Collect = "workflow/cases-documents-requests/collect";
                    public const string Count = "workflow/cases-documents-requests/count";
                    public const string Search = "workflow/cases-documents-requests/search";
                    public const string Download = "workflow/cases-documents-requests/download";
                }

                public static partial class CaseGroup // Entity
                {
                    // Queries

                    public const string Assert = "workflow/cases-groups/assert";
                    public const string Retrieve = "workflow/cases-groups/retrieve";

                    public const string Collect = "workflow/cases-groups/collect";
                    public const string Count = "workflow/cases-groups/count";
                    public const string Search = "workflow/cases-groups/search";
                    public const string Download = "workflow/cases-groups/download";
                }

                public static partial class CaseStatus // Entity
                {
                    // Queries

                    public const string Assert = "workflow/cases-statuses/assert";
                    public const string Retrieve = "workflow/cases-statuses/retrieve";

                    public const string Collect = "workflow/cases-statuses/collect";
                    public const string Count = "workflow/cases-statuses/count";
                    public const string Search = "workflow/cases-statuses/search";
                    public const string Download = "workflow/cases-statuses/download";

                    // Commands

                    public const string Create = "workflow/cases-statuses/create";
                    public const string Delete = "workflow/cases-statuses/delete";
                    public const string Modify = "workflow/cases-statuses/modify";
                }

                public static partial class CaseUser // Entity
                {
                    // Queries

                    public const string Assert = "workflow/cases-users/assert";
                    public const string Retrieve = "workflow/cases-users/retrieve";

                    public const string Collect = "workflow/cases-users/collect";
                    public const string Count = "workflow/cases-users/count";
                    public const string Search = "workflow/cases-users/search";
                    public const string Download = "workflow/cases-users/download";
                }
            }

            public static partial class Forms // Subcomponent
            {
                public static partial class Form // Entity
                {
                    // Queries

                    public const string Assert = "workflow/forms/assert";
                    public const string Retrieve = "workflow/forms/retrieve";

                    public const string Collect = "workflow/forms/collect";
                    public const string Count = "workflow/forms/count";
                    public const string Search = "workflow/forms/search";
                    public const string Download = "workflow/forms/download";
                }

                public static partial class FormCondition // Entity
                {
                    // Queries

                    public const string Assert = "workflow/forms-conditions/assert";
                    public const string Retrieve = "workflow/forms-conditions/retrieve";

                    public const string Collect = "workflow/forms-conditions/collect";
                    public const string Count = "workflow/forms-conditions/count";
                    public const string Search = "workflow/forms-conditions/search";
                    public const string Download = "workflow/forms-conditions/download";
                }

                public static partial class FormOptionItem // Entity
                {
                    // Queries

                    public const string Assert = "workflow/forms-options-items/assert";
                    public const string Retrieve = "workflow/forms-options-items/retrieve";

                    public const string Collect = "workflow/forms-options-items/collect";
                    public const string Count = "workflow/forms-options-items/count";
                    public const string Search = "workflow/forms-options-items/search";
                    public const string Download = "workflow/forms-options-items/download";
                }

                public static partial class FormOptionList // Entity
                {
                    // Queries

                    public const string Assert = "workflow/forms-options-lists/assert";
                    public const string Retrieve = "workflow/forms-options-lists/retrieve";

                    public const string Collect = "workflow/forms-options-lists/collect";
                    public const string Count = "workflow/forms-options-lists/count";
                    public const string Search = "workflow/forms-options-lists/search";
                    public const string Download = "workflow/forms-options-lists/download";
                }

                public static partial class FormQuestion // Entity
                {
                    // Queries

                    public const string Assert = "workflow/forms-questions/assert";
                    public const string Retrieve = "workflow/forms-questions/retrieve";

                    public const string Collect = "workflow/forms-questions/collect";
                    public const string Count = "workflow/forms-questions/count";
                    public const string Search = "workflow/forms-questions/search";
                    public const string Download = "workflow/forms-questions/download";
                }
            }

            public static partial class Submissions // Subcomponent
            {
                public static partial class Submission // Entity
                {
                    // Queries

                    public const string Assert = "workflow/submissions/assert";
                    public const string Retrieve = "workflow/submissions/retrieve";

                    public const string Collect = "workflow/submissions/collect";
                    public const string Count = "workflow/submissions/count";
                    public const string Search = "workflow/submissions/search";
                    public const string Download = "workflow/submissions/download";
                }

                public static partial class SubmissionAnswer // Entity
                {
                    // Queries

                    public const string Assert = "workflow/submissions-answers/assert";
                    public const string Retrieve = "workflow/submissions-answers/retrieve";

                    public const string Collect = "workflow/submissions-answers/collect";
                    public const string Count = "workflow/submissions-answers/count";
                    public const string Search = "workflow/submissions-answers/search";
                    public const string Download = "workflow/submissions-answers/download";
                }

                public static partial class SubmissionOption // Entity
                {
                    // Queries

                    public const string Assert = "workflow/submissions-options/assert";
                    public const string Retrieve = "workflow/submissions-options/retrieve";

                    public const string Collect = "workflow/submissions-options/collect";
                    public const string Count = "workflow/submissions-options/count";
                    public const string Search = "workflow/submissions-options/search";
                    public const string Download = "workflow/submissions-options/download";
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

                    public const string Assert = "workspace/pages/assert";
                    public const string Retrieve = "workspace/pages/retrieve";

                    public const string Collect = "workspace/pages/collect";
                    public const string Count = "workspace/pages/count";
                    public const string Search = "workspace/pages/search";
                    public const string Download = "workspace/pages/download";
                }
            }

            public static partial class Sites // Subcomponent
            {
                public static partial class Site // Entity
                {
                    // Queries

                    public const string Assert = "workspace/sites/assert";
                    public const string Retrieve = "workspace/sites/retrieve";

                    public const string Collect = "workspace/sites/collect";
                    public const string Count = "workspace/sites/count";
                    public const string Search = "workspace/sites/search";
                    public const string Download = "workspace/sites/download";
                }
            }
        }

    }
}