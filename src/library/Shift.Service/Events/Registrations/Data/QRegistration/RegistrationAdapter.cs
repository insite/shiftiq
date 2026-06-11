using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Booking;

public class RegistrationAdapter : IEntityAdapter
{
    public void Copy(ModifyRegistration modify, RegistrationEntity entity)
    {
        entity.EventIdentifier = modify.EventIdentifier;
        entity.EventPotentialConflicts = modify.EventPotentialConflicts;
        entity.ApprovalProcess = modify.ApprovalProcess;
        entity.ApprovalReason = modify.ApprovalReason;
        entity.ApprovalStatus = modify.ApprovalStatus;
        entity.AttemptIdentifier = modify.AttemptIdentifier;
        entity.AttendanceStatus = modify.AttendanceStatus;
        entity.BillingCustomer = modify.BillingCustomer;
        entity.CandidateIdentifier = modify.CandidateIdentifier;
        entity.CustomerIdentifier = modify.CustomerIdentifier;
        entity.DistributionExpected = modify.DistributionExpected;
        entity.EligibilityProcess = modify.EligibilityProcess;
        entity.EligibilityStatus = modify.EligibilityStatus;
        entity.EmployerIdentifier = modify.EmployerIdentifier;
        entity.ExamFormIdentifier = modify.ExamFormIdentifier;
        entity.ExamTimeLimit = modify.ExamTimeLimit;
        entity.Grade = modify.Grade;
        entity.GradeAssigned = modify.GradeAssigned;
        entity.GradePublished = modify.GradePublished;
        entity.GradeReleased = modify.GradeReleased;
        entity.GradeWithheld = modify.GradeWithheld;
        entity.GradeWithheldReason = modify.GradeWithheldReason;
        entity.GradingProcess = modify.GradingProcess;
        entity.GradingStatus = modify.GradingStatus;
        entity.LastChangeTime = modify.LastChangeTime;
        entity.LastChangeType = modify.LastChangeType;
        entity.LastChangeUser = modify.LastChangeUser;
        entity.MaterialsIncludeDiagramBook = modify.MaterialsIncludeDiagramBook;
        entity.MaterialsPackagedForDistribution = modify.MaterialsPackagedForDistribution;
        entity.MaterialsPermittedToCandidates = modify.MaterialsPermittedToCandidates;
        entity.RegistrationComment = modify.RegistrationComment;
        entity.RegistrationFee = modify.RegistrationFee;
        entity.RegistrationPassword = modify.RegistrationPassword;
        entity.RegistrationRequestedOn = modify.RegistrationRequestedOn;
        entity.RegistrationSequence = modify.RegistrationSequence;
        entity.RegistrationSource = modify.RegistrationSource;
        entity.SchoolIdentifier = modify.SchoolIdentifier;
        entity.Score = modify.Score;
        entity.SeatIdentifier = modify.SeatIdentifier;
        entity.SynchronizationProcess = modify.SynchronizationProcess;
        entity.SynchronizationStatus = modify.SynchronizationStatus;
        entity.WorkBasedHoursToDate = modify.WorkBasedHoursToDate;
        entity.IncludeInT2202 = modify.IncludeInT2202;
        entity.PaymentIdentifier = modify.PaymentIdentifier;
        entity.CandidateType = modify.CandidateType;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.RegistrationRequestedBy = modify.RegistrationRequestedBy;
        entity.AttendanceTaken = modify.AttendanceTaken;
        entity.EligibilityUpdated = modify.EligibilityUpdated;
        entity.BillingCode = modify.BillingCode;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public RegistrationEntity ToEntity(CreateRegistration create)
    {
        var entity = new RegistrationEntity
        {
            EventIdentifier = create.EventIdentifier,
            EventPotentialConflicts = create.EventPotentialConflicts,
            ApprovalProcess = create.ApprovalProcess,
            ApprovalReason = create.ApprovalReason,
            ApprovalStatus = create.ApprovalStatus,
            AttemptIdentifier = create.AttemptIdentifier,
            AttendanceStatus = create.AttendanceStatus,
            BillingCustomer = create.BillingCustomer,
            CandidateIdentifier = create.CandidateIdentifier,
            CustomerIdentifier = create.CustomerIdentifier,
            DistributionExpected = create.DistributionExpected,
            EligibilityProcess = create.EligibilityProcess,
            EligibilityStatus = create.EligibilityStatus,
            EmployerIdentifier = create.EmployerIdentifier,
            ExamFormIdentifier = create.ExamFormIdentifier,
            ExamTimeLimit = create.ExamTimeLimit,
            Grade = create.Grade,
            GradeAssigned = create.GradeAssigned,
            GradePublished = create.GradePublished,
            GradeReleased = create.GradeReleased,
            GradeWithheld = create.GradeWithheld,
            GradeWithheldReason = create.GradeWithheldReason,
            GradingProcess = create.GradingProcess,
            GradingStatus = create.GradingStatus,
            LastChangeTime = create.LastChangeTime,
            LastChangeType = create.LastChangeType,
            LastChangeUser = create.LastChangeUser,
            MaterialsIncludeDiagramBook = create.MaterialsIncludeDiagramBook,
            MaterialsPackagedForDistribution = create.MaterialsPackagedForDistribution,
            MaterialsPermittedToCandidates = create.MaterialsPermittedToCandidates,
            RegistrationComment = create.RegistrationComment,
            RegistrationFee = create.RegistrationFee,
            RegistrationIdentifier = create.RegistrationIdentifier,
            RegistrationPassword = create.RegistrationPassword,
            RegistrationRequestedOn = create.RegistrationRequestedOn,
            RegistrationSequence = create.RegistrationSequence,
            RegistrationSource = create.RegistrationSource,
            SchoolIdentifier = create.SchoolIdentifier,
            Score = create.Score,
            SeatIdentifier = create.SeatIdentifier,
            SynchronizationProcess = create.SynchronizationProcess,
            SynchronizationStatus = create.SynchronizationStatus,
            WorkBasedHoursToDate = create.WorkBasedHoursToDate,
            IncludeInT2202 = create.IncludeInT2202,
            PaymentIdentifier = create.PaymentIdentifier,
            CandidateType = create.CandidateType,
            OrganizationIdentifier = create.OrganizationIdentifier,
            RegistrationRequestedBy = create.RegistrationRequestedBy,
            AttendanceTaken = create.AttendanceTaken,
            EligibilityUpdated = create.EligibilityUpdated,
            BillingCode = create.BillingCode
        };
        return entity;
    }

    public IEnumerable<RegistrationModel> ToModel(IEnumerable<RegistrationEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public RegistrationModel ToModel(RegistrationEntity entity)
    {
        var model = new RegistrationModel
        {
            EventIdentifier = entity.EventIdentifier,
            EventPotentialConflicts = entity.EventPotentialConflicts,
            ApprovalProcess = entity.ApprovalProcess,
            ApprovalReason = entity.ApprovalReason,
            ApprovalStatus = entity.ApprovalStatus,
            AttemptIdentifier = entity.AttemptIdentifier,
            AttendanceStatus = entity.AttendanceStatus,
            BillingCustomer = entity.BillingCustomer,
            CandidateIdentifier = entity.CandidateIdentifier,
            CustomerIdentifier = entity.CustomerIdentifier,
            DistributionExpected = entity.DistributionExpected,
            EligibilityProcess = entity.EligibilityProcess,
            EligibilityStatus = entity.EligibilityStatus,
            EmployerIdentifier = entity.EmployerIdentifier,
            ExamFormIdentifier = entity.ExamFormIdentifier,
            ExamTimeLimit = entity.ExamTimeLimit,
            Grade = entity.Grade,
            GradeAssigned = entity.GradeAssigned,
            GradePublished = entity.GradePublished,
            GradeReleased = entity.GradeReleased,
            GradeWithheld = entity.GradeWithheld,
            GradeWithheldReason = entity.GradeWithheldReason,
            GradingProcess = entity.GradingProcess,
            GradingStatus = entity.GradingStatus,
            LastChangeTime = entity.LastChangeTime,
            LastChangeType = entity.LastChangeType,
            LastChangeUser = entity.LastChangeUser,
            MaterialsIncludeDiagramBook = entity.MaterialsIncludeDiagramBook,
            MaterialsPackagedForDistribution = entity.MaterialsPackagedForDistribution,
            MaterialsPermittedToCandidates = entity.MaterialsPermittedToCandidates,
            RegistrationComment = entity.RegistrationComment,
            RegistrationFee = entity.RegistrationFee,
            RegistrationIdentifier = entity.RegistrationIdentifier,
            RegistrationPassword = entity.RegistrationPassword,
            RegistrationRequestedOn = entity.RegistrationRequestedOn,
            RegistrationSequence = entity.RegistrationSequence,
            RegistrationSource = entity.RegistrationSource,
            SchoolIdentifier = entity.SchoolIdentifier,
            Score = entity.Score,
            SeatIdentifier = entity.SeatIdentifier,
            SynchronizationProcess = entity.SynchronizationProcess,
            SynchronizationStatus = entity.SynchronizationStatus,
            WorkBasedHoursToDate = entity.WorkBasedHoursToDate,
            IncludeInT2202 = entity.IncludeInT2202,
            PaymentIdentifier = entity.PaymentIdentifier,
            CandidateType = entity.CandidateType,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            RegistrationRequestedBy = entity.RegistrationRequestedBy,
            AttendanceTaken = entity.AttendanceTaken,
            EligibilityUpdated = entity.EligibilityUpdated,
            BillingCode = entity.BillingCode
        };

        return model;
    }

    public IEnumerable<RegistrationMatch> ToMatch(IEnumerable<RegistrationEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public RegistrationMatch ToMatch(RegistrationEntity entity)
    {
        var match = new RegistrationMatch
        {
            RegistrationIdentifier = entity.RegistrationIdentifier

        };

        return match;
    }
}