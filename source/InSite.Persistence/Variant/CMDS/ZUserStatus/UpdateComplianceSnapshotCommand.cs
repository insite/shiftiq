using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    public sealed class UpdateComplianceSnapshotCommand
    {
        #region Classes

        private interface IQueryDataModel
        {
            Guid DepartmentOrganizationIdentifier { get; }
            Guid DepartmentIdentifier { get; }
            Guid UserIdentifier { get; }
            Guid? PrimaryProfileIdentifier { get; }
        }

        private class QueryCompetencyDataModel : IQueryDataModel
        {
            public Guid DepartmentOrganizationIdentifier { get; set; }
            public Guid DepartmentIdentifier { get; set; }
            public Guid UserIdentifier { get; set; }
            public Guid? PrimaryProfileIdentifier { get; set; }

            public Guid? EmploymentCompetencyStandardIdentifier { get; set; }
            public string EmploymentCompetencyValidationStatus { get; set; }
            public bool IsCritical { get; set; }
            public Guid? EmploymentProfileId { get; set; }
            public bool? IsComplianceRequired { get; set; }
        }

        private class QueryResourceDataModel : IQueryDataModel
        {
            public Guid DepartmentOrganizationIdentifier { get; set; }
            public Guid DepartmentIdentifier { get; set; }
            public Guid UserIdentifier { get; set; }
            public Guid? PrimaryProfileIdentifier { get; set; }

            public string ResourceSubType { get; set; }
            public string ResourceValidationStatus { get; set; }
        }

        private class SnaphotModel
        {
            #region Properties

            public Guid OrganizationIdentifier { get; set; }
            public Guid DepartmentIdentifier { get; set; }
            public Guid UserIdentifier { get; set; }
            public Guid? PrimaryProfileIdentifier { get; set; }

            public int CompetencyCountRequired { get; private set; }
            public int CompetencyCountExpired { get; private set; }
            public int CompetencyCountNotCompleted { get; private set; }
            public int CompetencyCountNotApplicable { get; private set; }
            public int CompetencyCountNeedsTraining { get; private set; }
            public int CompetencyCountSelfAssessed { get; private set; }
            public int CompetencyCountSubmitted { get; private set; }
            public int CompetencyCountValidated { get; private set; }

            public int CriticalCompetencyCountRequired { get; private set; }
            public int CriticalCompetencyCountExpired { get; private set; }
            public int CriticalCompetencyCountNotCompleted { get; private set; }
            public int CriticalCompetencyCountNotApplicable { get; private set; }
            public int CriticalCompetencyCountNeedsTraining { get; private set; }
            public int CriticalCompetencyCountSelfAssessed { get; private set; }
            public int CriticalCompetencyCountSubmitted { get; private set; }
            public int CriticalCompetencyCountValidated { get; private set; }

            public int NonCriticalCompetencyCountRequired { get; private set; }
            public int NonCriticalCompetencyCountExpired { get; private set; }
            public int NonCriticalCompetencyCountNotCompleted { get; private set; }
            public int NonCriticalCompetencyCountNotApplicable { get; private set; }
            public int NonCriticalCompetencyCountNeedsTraining { get; private set; }
            public int NonCriticalCompetencyCountSelfAssessed { get; private set; }
            public int NonCriticalCompetencyCountSubmitted { get; private set; }
            public int NonCriticalCompetencyCountValidated { get; private set; }

            public int CriticalComplianceCompetencyCountRequired { get; private set; }
            public int CriticalComplianceCompetencyCountExpired { get; private set; }
            public int CriticalComplianceCompetencyCountNotCompleted { get; private set; }
            public int CriticalComplianceCompetencyCountNotApplicable { get; private set; }
            public int CriticalComplianceCompetencyCountNeedsTraining { get; private set; }
            public int CriticalComplianceCompetencyCountSelfAssessed { get; private set; }
            public int CriticalComplianceCompetencyCountSubmitted { get; private set; }
            public int CriticalComplianceCompetencyCountValidated { get; private set; }

            public int NonCriticalComplianceCompetencyCountRequired { get; private set; }
            public int NonCriticalComplianceCompetencyCountExpired { get; private set; }
            public int NonCriticalComplianceCompetencyCountNotCompleted { get; private set; }
            public int NonCriticalComplianceCompetencyCountNotApplicable { get; private set; }
            public int NonCriticalComplianceCompetencyCountNeedsTraining { get; private set; }
            public int NonCriticalComplianceCompetencyCountSelfAssessed { get; private set; }
            public int NonCriticalComplianceCompetencyCountSubmitted { get; private set; }
            public int NonCriticalComplianceCompetencyCountValidated { get; private set; }

            public int TimeSensitiveSafetyCertificateCountRequired { get; private set; }
            public int TimeSensitiveSafetyCertificateCountValidated { get; private set; }

            public int AdditionalComplianceRequirementCountRequired { get; private set; }
            public int AdditionalComplianceRequirementCountValidated { get; private set; }

            public int CodesOfPracticeCountRequired { get; private set; }
            public int CodesOfPracticeCountValidated { get; private set; }

            public int SafeOperatingPracticeCountRequired { get; private set; }
            public int SafeOperatingPracticeCountValidated { get; private set; }

            public int HumanResourceDocumentsCountRequired { get; private set; }
            public int HumanResourceDocumentsCountValidated { get; private set; }

            public int ModuleCountRequired { get; private set; }
            public int ModuleCountValidated { get; private set; }

            public int TrainingGuideCountRequired { get; private set; }
            public int TrainingGuideCountValidated { get; private set; }

            public int SiteSpecificOperatingProcedureCountRequired { get; private set; }
            public int SiteSpecificOperatingProcedureCountValidated { get; private set; }

            public int OrientationCountRequired { get; private set; }
            public int OrientationCountValidated { get; private set; }

            #endregion

            #region Fields

            private List<SnaphotCompetencyModel> _competencies = new List<SnaphotCompetencyModel>();
            private List<SnaphotResourceModel> _resources = new List<SnaphotResourceModel>();

            #endregion

            #region Construction

            internal SnaphotModel(IQueryDataModel data)
            {
                OrganizationIdentifier = data.DepartmentOrganizationIdentifier;
                DepartmentIdentifier = data.DepartmentIdentifier;
                UserIdentifier = data.UserIdentifier;
                PrimaryProfileIdentifier = data.PrimaryProfileIdentifier;
            }

            #endregion

            #region Public methods

            internal void AddData(QueryCompetencyDataModel data)
            {
                if (_competencies == null || !data.EmploymentCompetencyStandardIdentifier.HasValue)
                    return;

                var model = new SnaphotCompetencyModel(data);

                _competencies.Add(model);
            }

            internal void AddData(QueryResourceDataModel data)
            {
                if (_resources == null)
                    return;

                var model = new SnaphotResourceModel(data);

                _resources.Add(model);
            }

            internal void Calculate()
            {
                if (_competencies == null || _resources == null)
                    return;

                // CompetencyCount

                CompetencyCountRequired = _competencies.Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                CompetencyCountExpired = _competencies.Where(x => x.ValidationStatus == "expired").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                CompetencyCountNotCompleted = _competencies.Where(x => x.ValidationStatus == "not completed").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                CompetencyCountNotApplicable = _competencies.Where(x => x.ValidationStatus == "not applicable").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                CompetencyCountNeedsTraining = _competencies.Where(x => x.ValidationStatus == "needs training").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                CompetencyCountSelfAssessed = _competencies.Where(x => x.ValidationStatus == "self-assessed").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                CompetencyCountSubmitted = _competencies.Where(x => x.ValidationStatus == "submitted for validation").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                CompetencyCountValidated = _competencies.Where(x => x.ValidationStatus == "validated").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();

                // CriticalCompetencyCount

                if (PrimaryProfileIdentifier.HasValue)
                {
                    var criticalCompetencies = _competencies.Where(x => x.IsCritical && x.ProfileStandardIdentifier == PrimaryProfileIdentifier.Value);

                    CriticalCompetencyCountRequired = criticalCompetencies.Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                    CriticalCompetencyCountExpired = criticalCompetencies.Where(x => x.ValidationStatus == "expired").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                    CriticalCompetencyCountNotCompleted = criticalCompetencies.Where(x => x.ValidationStatus == "not completed").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                    CriticalCompetencyCountNotApplicable = criticalCompetencies.Where(x => x.ValidationStatus == "not applicable").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                    CriticalCompetencyCountNeedsTraining = criticalCompetencies.Where(x => x.ValidationStatus == "needs training").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                    CriticalCompetencyCountSelfAssessed = criticalCompetencies.Where(x => x.ValidationStatus == "self-assessed").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                    CriticalCompetencyCountSubmitted = criticalCompetencies.Where(x => x.ValidationStatus == "submitted for validation").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                    CriticalCompetencyCountValidated = criticalCompetencies.Where(x => x.ValidationStatus == "validated").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                }
                else
                {
                    CriticalCompetencyCountRequired = 0;
                    CriticalCompetencyCountExpired = 0;
                    CriticalCompetencyCountNotCompleted = 0;
                    CriticalCompetencyCountNotApplicable = 0;
                    CriticalCompetencyCountNeedsTraining = 0;
                    CriticalCompetencyCountSelfAssessed = 0;
                    CriticalCompetencyCountSubmitted = 0;
                    CriticalCompetencyCountValidated = 0;
                }

                // NonCriticalCompetencyCount

                if (PrimaryProfileIdentifier.HasValue)
                {
                    var nonCriticalCompetency = _competencies.Where(x => !x.IsCritical && x.ProfileStandardIdentifier == PrimaryProfileIdentifier.Value);

                    NonCriticalCompetencyCountRequired = nonCriticalCompetency.Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                    NonCriticalCompetencyCountExpired = nonCriticalCompetency.Where(x => x.ValidationStatus == "expired").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                    NonCriticalCompetencyCountNotCompleted = nonCriticalCompetency.Where(x => x.ValidationStatus == "not completed").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                    NonCriticalCompetencyCountNotApplicable = nonCriticalCompetency.Where(x => x.ValidationStatus == "not applicable").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                    NonCriticalCompetencyCountNeedsTraining = nonCriticalCompetency.Where(x => x.ValidationStatus == "needs training").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                    NonCriticalCompetencyCountSelfAssessed = nonCriticalCompetency.Where(x => x.ValidationStatus == "self-assessed").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                    NonCriticalCompetencyCountSubmitted = nonCriticalCompetency.Where(x => x.ValidationStatus == "submitted for validation").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                    NonCriticalCompetencyCountValidated = nonCriticalCompetency.Where(x => x.ValidationStatus == "validated").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                }
                else
                {
                    NonCriticalCompetencyCountRequired = 0;
                    NonCriticalCompetencyCountExpired = 0;
                    NonCriticalCompetencyCountNotCompleted = 0;
                    NonCriticalCompetencyCountNotApplicable = 0;
                    NonCriticalCompetencyCountNeedsTraining = 0;
                    NonCriticalCompetencyCountSelfAssessed = 0;
                    NonCriticalCompetencyCountSubmitted = 0;
                    NonCriticalCompetencyCountValidated = 0;
                }

                // CriticalComplianceCompetency

                var criticalComplianceCompetency = _competencies.Where(x => x.IsCritical && x.IsComplianceRequired == true);

                CriticalComplianceCompetencyCountRequired = criticalComplianceCompetency.Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                CriticalComplianceCompetencyCountExpired = criticalComplianceCompetency.Where(x => x.ValidationStatus == "expired").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                CriticalComplianceCompetencyCountNotCompleted = criticalComplianceCompetency.Where(x => x.ValidationStatus == "not completed").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                CriticalComplianceCompetencyCountNotApplicable = criticalComplianceCompetency.Where(x => x.ValidationStatus == "not applicable").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                CriticalComplianceCompetencyCountNeedsTraining = criticalComplianceCompetency.Where(x => x.ValidationStatus == "needs training").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                CriticalComplianceCompetencyCountSelfAssessed = criticalComplianceCompetency.Where(x => x.ValidationStatus == "self-assessed").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                CriticalComplianceCompetencyCountSubmitted = criticalComplianceCompetency.Where(x => x.ValidationStatus == "submitted for validation").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                CriticalComplianceCompetencyCountValidated = criticalComplianceCompetency.Where(x => x.ValidationStatus == "validated").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();

                // NonCriticalComplianceCompetencyCount

                var nonCriticalComplianceCompetencies = _competencies.Where(x => !x.IsCritical && x.IsComplianceRequired == true);

                NonCriticalComplianceCompetencyCountRequired = nonCriticalComplianceCompetencies.Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                NonCriticalComplianceCompetencyCountExpired = nonCriticalComplianceCompetencies.Where(x => x.ValidationStatus == "expired").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                NonCriticalComplianceCompetencyCountNotCompleted = nonCriticalComplianceCompetencies.Where(x => x.ValidationStatus == "not completed").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                NonCriticalComplianceCompetencyCountNotApplicable = nonCriticalComplianceCompetencies.Where(x => x.ValidationStatus == "not applicable").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                NonCriticalComplianceCompetencyCountNeedsTraining = nonCriticalComplianceCompetencies.Where(x => x.ValidationStatus == "needs training").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                NonCriticalComplianceCompetencyCountSelfAssessed = nonCriticalComplianceCompetencies.Where(x => x.ValidationStatus == "self-assessed").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                NonCriticalComplianceCompetencyCountSubmitted = nonCriticalComplianceCompetencies.Where(x => x.ValidationStatus == "submitted for validation").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();
                NonCriticalComplianceCompetencyCountValidated = nonCriticalComplianceCompetencies.Where(x => x.ValidationStatus == "validated").Select(x => x.CompetencyStandardIdentifier).Distinct().Count();

                // TimeSensitiveSafetyCertificateCount

                var timeSensitiveSafetyCertificates = _resources.Where(x => x.SubType == "time-sensitive safety certificate");

                TimeSensitiveSafetyCertificateCountRequired = timeSensitiveSafetyCertificates.Count();
                TimeSensitiveSafetyCertificateCountValidated = timeSensitiveSafetyCertificates.Where(x => x.ValidationStatus == "valid").Count();

                // AdditionalComplianceRequirementCount

                var additionalComplianceRequirements = _resources.Where(x => x.SubType == "additional compliance requirement");

                AdditionalComplianceRequirementCountRequired = additionalComplianceRequirements.Count();
                AdditionalComplianceRequirementCountValidated = additionalComplianceRequirements.Where(x => x.ValidationStatus == "valid").Count();

                // CodesOfPracticeCount

                var codesOfPractices = _resources.Where(x => x.SubType == "code of practice");

                CodesOfPracticeCountRequired = codesOfPractices.Count();
                CodesOfPracticeCountValidated = codesOfPractices.Where(x => x.ValidationStatus == "valid").Count();

                // SafeOperatingPracticeCount

                var safeOperatingPractices = _resources.Where(x => x.SubType == "safe operating practice");

                SafeOperatingPracticeCountRequired = safeOperatingPractices.Count();
                SafeOperatingPracticeCountValidated = safeOperatingPractices.Where(x => x.ValidationStatus == "valid").Count();

                // HumanResourceDocumentsCount

                var humanResourceDocuments = _resources.Where(x => x.SubType == "human resources document");

                HumanResourceDocumentsCountRequired = humanResourceDocuments.Count();
                HumanResourceDocumentsCountValidated = humanResourceDocuments.Where(x => x.ValidationStatus == "valid").Count();

                // Module

                var modules = _resources.Where(x => x.SubType == "module");

                ModuleCountRequired = modules.Count();
                ModuleCountValidated = modules.Where(x => x.ValidationStatus == "valid").Count();

                // Training Guide

                var trainingGuides = _resources.Where(x => x.SubType == "training guide");

                TrainingGuideCountRequired = trainingGuides.Count();
                TrainingGuideCountValidated = trainingGuides.Where(x => x.ValidationStatus == "valid").Count();

                // Site-Specific Operating Procedure

                var siteSpecificOperatingProcedures = _resources.Where(x => x.SubType == "site-specific operating procedure");

                SiteSpecificOperatingProcedureCountRequired = siteSpecificOperatingProcedures.Count();
                SiteSpecificOperatingProcedureCountValidated = siteSpecificOperatingProcedures.Where(x => x.ValidationStatus == "valid").Count();

                var orientations = _resources.Where(x => x.SubType == "orientation");

                OrientationCountRequired = orientations.Count();
                OrientationCountValidated = orientations.Where(x => x.ValidationStatus == "valid").Count();

                // Clear

                _competencies = null;
                _resources = null;
            }

            #endregion
        }

        private class SnaphotCompetencyModel
        {
            #region Properties

            public Guid? CompetencyStandardIdentifier { get; set; }
            public string ValidationStatus { get; set; }
            public bool IsCritical { get; set; }
            public Guid? ProfileStandardIdentifier { get; set; }
            public bool? IsComplianceRequired { get; set; }

            #endregion

            #region Construction

            public SnaphotCompetencyModel(QueryCompetencyDataModel data)
            {
                CompetencyStandardIdentifier = data.EmploymentCompetencyStandardIdentifier;
                ValidationStatus = data.EmploymentCompetencyValidationStatus;
                IsCritical = data.IsCritical;
                ProfileStandardIdentifier = data.EmploymentProfileId;
                IsComplianceRequired = data.IsComplianceRequired;
            }

            #endregion
        }

        private class SnaphotResourceModel
        {
            #region Properties

            public string SubType { get; set; }
            public string ValidationStatus { get; set; }

            #endregion

            #region Construction

            public SnaphotResourceModel(QueryResourceDataModel data)
            {
                SubType = data.ResourceSubType;
                ValidationStatus = data.ResourceValidationStatus;
            }

            #endregion
        }

        #endregion

        #region Constants

        private const string DeleteTodaySnapshotsQuery = @"DELETE custom_cmds.ZUserStatus WHERE AsAt = @Today;";

        private const string DeleteOutdatedSnapshotsQuery = "DELETE FROM custom_cmds.ZUserStatus WHERE DATEDIFF(MONTH,AsAt,GETDATE()) > 12;";

        private const string CompetencyDataQuery = @"
SELECT
    d.OrganizationIdentifier AS DepartmentOrganizationIdentifier
   ,d.DepartmentIdentifier
   ,contacts.UserIdentifier
   ,e.ProfileStandardIdentifier AS PrimaryProfileIdentifier

   ,ec.StandardIdentifier AS EmploymentCompetencyStandardIdentifier
   ,LOWER(ec.ValidationStatus) AS EmploymentCompetencyValidationStatus
   ,case when cs.Criticality = 'Critical' then cast(1 as bit) else cast(0 as bit) end as IsCritical
   ,ep.ProfileStandardIdentifier AS EmploymentProfileID
   ,ep.IsRequired AS IsComplianceRequired

FROM
    identities.[User] AS contacts
    INNER JOIN contacts.Membership AS m ON m.UserIdentifier = contacts.UserIdentifier
    INNER JOIN identities.Department AS d ON d.DepartmentIdentifier = m.GroupIdentifier
    INNER JOIN accounts.QOrganization AS DepartmentOrganization ON DepartmentOrganization.OrganizationIdentifier = d.OrganizationIdentifier
    LEFT JOIN standards.DepartmentProfileUser AS e ON e.UserIdentifier = contacts.UserIdentifier
                                                AND e.DepartmentIdentifier = d.DepartmentIdentifier
                                                AND e.IsPrimary = 1
    LEFT JOIN (
        standards.DepartmentProfileUser AS ep
        INNER JOIN standards.StandardContainment AS pc ON pc.ParentStandardIdentifier = ep.ProfileStandardIdentifier
        INNER JOIN standards.StandardValidation AS ec ON ec.UserIdentifier = ep.UserIdentifier
                                    AND ec.StandardIdentifier = pc.ChildStandardIdentifier
        INNER JOIN standards.[Standard] AS c ON c.StandardIdentifier = ec.StandardIdentifier
                                AND c.StandardType = 'Competency'
                                AND c.IsHidden = 0
        INNER JOIN accounts.QOrganization AS StandardOrganization ON StandardOrganization.OrganizationIdentifier = c.OrganizationIdentifier
        INNER JOIN custom_cmds.DepartmentProfileCompetency AS cs ON cs.DepartmentIdentifier = ep.DepartmentIdentifier
                                                        AND cs.ProfileStandardIdentifier = pc.ParentStandardIdentifier
                                                        AND cs.CompetencyStandardIdentifier = pc.ChildStandardIdentifier
    ) ON ep.UserIdentifier = contacts.UserIdentifier
         AND ep.DepartmentIdentifier = m.GroupIdentifier

WHERE
    contacts.UtcArchived IS NULL
    AND m.MembershipType = 'Department';";

        private const string ResourceDataQuery = @"
SELECT
    departments.OrganizationIdentifier AS DepartmentOrganizationIdentifier
   ,departments.DepartmentIdentifier
   ,e.UserIdentifier 
   ,e.ProfileStandardIdentifier AS PrimaryProfileIdentifier

   ,LOWER(ec.AchievementLabel) AS ResourceSubType
   ,LOWER(ec.CredentialStatus) AS ResourceValidationStatus
FROM
    standards.DepartmentProfileUser AS e
    INNER JOIN custom_cmds.VCmdsCredential AS ec ON e.UserIdentifier = ec.UserIdentifier
    INNER JOIN contacts.Membership AS m ON m.UserIdentifier = e.UserIdentifier AND m.GroupIdentifier = e.DepartmentIdentifier
    INNER JOIN identities.[User] AS c ON c.UserIdentifier = e.UserIdentifier
    INNER JOIN identities.Department AS departments ON departments.DepartmentIdentifier = e.DepartmentIdentifier
      
WHERE
    e.IsPrimary = 1
    AND c.UtcArchived IS NULL
    AND m.MembershipType = 'Department'
    AND ec.CredentialNecessity = 'Mandatory';";

        private const string InsertQuery = @"
INSERT INTO custom_cmds.ZUserStatus (
    OrganizationIdentifier
   ,DepartmentIdentifier
   ,UserIdentifier
   ,PrimaryProfileIdentifier
   ,AsAt

   ,CountRQ_Competency
   ,CountEX_Competency
   ,CountNC_Competency
   ,CountNA_Competency
   ,CountNT_Competency
   ,CountSA_Competency
   ,CountSV_Competency
   ,CountVA_Competency

   ,ScoreEX_Competency
   ,ScoreNC_Competency
   ,ScoreNA_Competency
   ,ScoreNT_Competency
   ,ScoreSA_Competency
   ,ScoreSV_Competency
   ,ScoreVA_Competency

   ,CountEX_CompetencyCritical
   ,CountNC_CompetencyCritical
   ,CountNA_CompetencyCritical
   ,CountNT_CompetencyCritical
   ,CountSA_CompetencyCritical
   ,CountSV_CompetencyCritical
   ,CountEX_CompetencyNoncritical
   ,CountNC_CompetencyNoncritical
   ,CountNA_CompetencyNoncritical
   ,CountNT_CompetencyNoncritical
   ,CountSA_CompetencyNoncritical
   ,CountSV_CompetencyNoncritical

   ,ScoreEX_CompetencyCritical
   ,ScoreNC_CompetencyCritical
   ,ScoreNA_CompetencyCritical
   ,ScoreNT_CompetencyCritical
   ,ScoreSA_CompetencyCritical
   ,ScoreSV_CompetencyCritical
   ,ScoreVA_CompetencyCritical
   ,ScoreEX_CompetencyNoncritical
   ,ScoreNC_CompetencyNoncritical
   ,ScoreNA_CompetencyNoncritical
   ,ScoreNT_CompetencyNoncritical
   ,ScoreSA_CompetencyNoncritical
   ,ScoreSV_CompetencyNoncritical
   ,ScoreVA_CompetencyNoncritical

   ,CountRQ_CompetencyCriticalMandatory
   ,CountEX_CompetencyCriticalMandatory
   ,CountNC_CompetencyCriticalMandatory
   ,CountNA_CompetencyCriticalMandatory
   ,CountNT_CompetencyCriticalMandatory
   ,CountSA_CompetencyCriticalMandatory
   ,CountSV_CompetencyCriticalMandatory
   ,CountVA_CompetencyCriticalMandatory
   ,CountRQ_CompetencyNoncriticalMandatory
   ,CountEX_CompetencyNoncriticalMandatory
   ,CountNC_CompetencyNoncriticalMandatory
   ,CountNA_CompetencyNoncriticalMandatory
   ,CountNT_CompetencyNoncriticalMandatory
   ,CountSA_CompetencyNoncriticalMandatory
   ,CountSV_CompetencyNoncriticalMandatory
   ,CountVA_CompetencyNoncriticalMandatory

   ,ScoreEX_CompetencyCriticalMandatory
   ,ScoreNC_CompetencyCriticalMandatory
   ,ScoreNA_CompetencyCriticalMandatory
   ,ScoreNT_CompetencyCriticalMandatory
   ,ScoreSA_CompetencyCriticalMandatory
   ,ScoreSV_CompetencyCriticalMandatory
   ,ScoreVA_CompetencyCriticalMandatory
   ,ScoreEX_CompetencyNoncriticalMandatory
   ,ScoreNC_CompetencyNoncriticalMandatory
   ,ScoreNA_CompetencyNoncriticalMandatory
   ,ScoreNT_CompetencyNoncriticalMandatory
   ,ScoreSA_CompetencyNoncriticalMandatory
   ,ScoreSV_CompetencyNoncriticalMandatory
   ,ScoreVA_CompetencyNoncriticalMandatory

   ,Score_CompetencyCriticalMandatory
   ,Score_CompetencyNoncriticalMandatory

   ,CountRQ_Certificate
   ,CountVA_Certificate
   ,Score_Certificate

   ,CountRQ_Requirement
   ,CountVA_Requirement
   ,Score_Requirement

   ,CountRQ_CompetencyCritical
   ,CountVA_CompetencyCritical
   ,Score_CompetencyCritical

   ,CountRQ_CompetencyNoncritical
   ,CountVA_CompetencyNoncritical
   ,Score_CompetencyNoncritical

   ,CountRQ_Code
   ,CountVA_Code
   ,Score_Code

   ,CountRQ_Practice
   ,CountVA_Practice
   ,Score_Practice

   ,CountRQ_Document
   ,CountVA_Document
   ,Score_Document

    ,CountRQ_Module
    ,CountVA_Module
    ,Score_Module

    ,CountRQ_Guide
    ,CountVA_Guide
    ,Score_Guide

    ,CountRQ_Procedure
    ,CountVA_Procedure
    ,Score_Procedure

    ,CountRQ_Orientation
    ,CountVA_Orientation
    ,Score_Orientation

) VALUES (
    @OrganizationIdentifier
   ,@DepartmentIdentifier
   ,@UserIdentifier
   ,@PrimaryProfileIdentifier
   ,@SnapshotDate

   ,@CompetencyCountRequired
   ,@CompetencyCountExpired
   ,@CompetencyCountNotCompleted
   ,@CompetencyCountNotApplicable
   ,@CompetencyCountNeedsTraining
   ,@CompetencyCountSelfAssessed
   ,@CompetencyCountSubmitted
   ,@CompetencyCountValidated

   ,@CompetencyPercentExpired
   ,@CompetencyPercentNotCompleted
   ,@CompetencyPercentNotApplicable
   ,@CompetencyPercentNeedsTraining
   ,@CompetencyPercentSelfAssessed
   ,@CompetencyPercentSubmitted
   ,@CompetencyPercentValidated

   ,@CriticalCompetencyCountExpired
   ,@CriticalCompetencyCountNotCompleted
   ,@CriticalCompetencyCountNotApplicable
   ,@CriticalCompetencyCountNeedsTraining
   ,@CriticalCompetencyCountSelfAssessed
   ,@CriticalCompetencyCountSubmitted
   ,@NonCriticalCompetencyCountExpired
   ,@NonCriticalCompetencyCountNotCompleted
   ,@NonCriticalCompetencyCountNotApplicable
   ,@NonCriticalCompetencyCountNeedsTraining
   ,@NonCriticalCompetencyCountSelfAssessed
   ,@NonCriticalCompetencyCountSubmitted

   ,@CriticalCompetencyPercentExpired
   ,@CriticalCompetencyPercentNotCompleted
   ,@CriticalCompetencyPercentNotApplicable
   ,@CriticalCompetencyPercentNeedsTraining
   ,@CriticalCompetencyPercentSelfAssessed
   ,@CriticalCompetencyPercentSubmitted
   ,@CriticalCompetencyPercentValidated
   ,@NonCriticalCompetencyPercentExpired
   ,@NonCriticalCompetencyPercentNotCompleted
   ,@NonCriticalCompetencyPercentNotApplicable
   ,@NonCriticalCompetencyPercentNeedsTraining
   ,@NonCriticalCompetencyPercentSelfAssessed
   ,@NonCriticalCompetencyPercentSubmitted
   ,@NonCriticalCompetencyPercentValidated

   ,@CriticalComplianceCompetencyCountRequired
   ,@CriticalComplianceCompetencyCountExpired
   ,@CriticalComplianceCompetencyCountNotCompleted
   ,@CriticalComplianceCompetencyCountNotApplicable
   ,@CriticalComplianceCompetencyCountNeedsTraining
   ,@CriticalComplianceCompetencyCountSelfAssessed
   ,@CriticalComplianceCompetencyCountSubmitted
   ,@CriticalComplianceCompetencyCountValidated
   ,@NonCriticalComplianceCompetencyCountRequired
   ,@NonCriticalComplianceCompetencyCountExpired
   ,@NonCriticalComplianceCompetencyCountNotCompleted
   ,@NonCriticalComplianceCompetencyCountNotApplicable
   ,@NonCriticalComplianceCompetencyCountNeedsTraining
   ,@NonCriticalComplianceCompetencyCountSelfAssessed
   ,@NonCriticalComplianceCompetencyCountSubmitted
   ,@NonCriticalComplianceCompetencyCountValidated

   ,@CriticalComplianceCompetencyPercentExpired
   ,@CriticalComplianceCompetencyPercentNotCompleted
   ,@CriticalComplianceCompetencyPercentNotApplicable
   ,@CriticalComplianceCompetencyPercentNeedsTraining
   ,@CriticalComplianceCompetencyPercentSelfAssessed
   ,@CriticalComplianceCompetencyPercentSubmitted
   ,@CriticalComplianceCompetencyPercentValidated
   ,@NonCriticalComplianceCompetencyPercentExpired
   ,@NonCriticalComplianceCompetencyPercentNotCompleted
   ,@NonCriticalComplianceCompetencyPercentNotApplicable
   ,@NonCriticalComplianceCompetencyPercentNeedsTraining
   ,@NonCriticalComplianceCompetencyPercentSelfAssessed
   ,@NonCriticalComplianceCompetencyPercentSubmitted
   ,@NonCriticalComplianceCompetencyPercentValidated

   ,@CriticalComplianceCompetencyScore
   ,@NonCriticalComplianceCompetencyScore

   ,@TimeSensitiveSafetyCertificateCountRequired
   ,@TimeSensitiveSafetyCertificateCountValidated
   ,@TimeSensitiveSafetyCertificateScore

   ,@AdditionalComplianceRequirementCountRequired
   ,@AdditionalComplianceRequirementCountValidated
   ,@AdditionalComplianceRequirementScore

   ,@CriticalCompetencyCountRequired
   ,@CriticalCompetencyCountValidated
   ,@CriticalCompetencyScore

   ,@NonCriticalCompetencyCountRequired
   ,@NonCriticalCompetencyCountValidated
   ,@NonCriticalCompetencyScore

   ,@CodesOfPracticeCountRequired
   ,@CodesOfPracticeCountValidated
   ,@CodesOfPracticeScore

   ,@SafeOperatingPracticeCountRequired
   ,@SafeOperatingPracticeCountValidated
   ,@SafeOperatingPracticeScore

   ,@HumanResourceDocumentsCountRequired
   ,@HumanResourceDocumentsCountValidated
   ,@HumanResourceDocumentsScore

    ,@ModuleCountRequired
    ,@ModuleCountValidated
    ,@ModuleScore

    ,@TrainingGuideCountRequired
    ,@TrainingGuideCountValidated
    ,@TrainingGuideScore

    ,@SiteSpecificOperatingProcedureCountRequired
    ,@SiteSpecificOperatingProcedureCountValidated
    ,@SiteSpecificOperatingProcedureScore

    ,@OrientationCountRequired
    ,@OrientationCountValidated
    ,@OrientationScore
);";

        private const string InsertSummaryQuery = @"
        INSERT INTO custom_cmds.ZUserStatusSummary
            (
                OrganizationIdentifier
              , DepartmentIdentifier
              , UserIdentifier
              , PrimaryProfileIdentifier
              , SnapshotDate
              , Sequence
              , Heading
              , PrimaryNotApplicable
              , PrimaryRequired
              , PrimarySatisfied
              , PrimaryScore
              , PrimarySubmitted
              , ComplianceNotApplicable
              , ComplianceRequired
              , ComplianceSatisfied
              , ComplianceScore
              , ComplianceSubmitted
            )
        SELECT OrganizationIdentifier
              , DepartmentIdentifier
              , UserIdentifier
              , PrimaryProfileIdentifier
              , SnapshotDate
              , Sequence
              , Heading
              , PrimaryNotApplicable
              , PrimaryRequired
              , PrimarySatisfied
              , PrimaryScore
              , PrimarySubmitted
              , ComplianceNotApplicable
              , ComplianceRequired
              , ComplianceSatisfied
              , ComplianceScore
              , ComplianceSubmitted 
        FROM custom_cmds.ZUserStatusSummaryCalc
        WHERE SnapshotDate NOT IN (SELECT DISTINCT SnapshotDate FROM custom_cmds.ZUserStatusSummary)
";

        #endregion

        public void Execute()
        {
            var result = new Dictionary<Tuple<Guid, Guid, Guid>, SnaphotModel>();

            using (var context = new InternalDbContext())
            {
                context.Database.CommandTimeout = 30 * 60; // 30 minutes

                var competencyData = context.Database.SqlQuery<QueryCompetencyDataModel>(CompetencyDataQuery).ToArray();
                foreach (var cd in competencyData)
                {
                    var key = new Tuple<Guid, Guid, Guid>(cd.DepartmentOrganizationIdentifier, cd.DepartmentIdentifier, cd.UserIdentifier);
                    if (!result.ContainsKey(key))
                        result.Add(key, new SnaphotModel(cd));

                    result[key].AddData(cd);
                }
            }

            using (var context = new InternalDbContext())
            {
                context.Database.CommandTimeout = 30 * 60; // 30 minutes

                var resourceData = context.Database.SqlQuery<QueryResourceDataModel>(ResourceDataQuery).ToArray();

                foreach (var rd in resourceData)
                {
                    var key = new Tuple<Guid, Guid, Guid>(rd.DepartmentOrganizationIdentifier, rd.DepartmentIdentifier, rd.UserIdentifier);
                    if (!result.ContainsKey(key))
                        result.Add(key, new SnaphotModel(rd));

                    result[key].AddData(rd);
                }
            }

            var today = DateTime.UtcNow.Date;

            using (var context = new InternalDbContext())
            {
                context.Database.CommandTimeout = 5 * 60; // 5 minutes

                context.Database.ExecuteSqlCommand(DeleteTodaySnapshotsQuery, new SqlParameter("@Today", today));

                foreach (var r in result.Values)
                {
                    r.Calculate();

                    context.Database.ExecuteSqlCommand(
                        InsertQuery,
                        new SqlParameter("@OrganizationIdentifier", r.OrganizationIdentifier),
                        new SqlParameter("@DepartmentIdentifier", r.DepartmentIdentifier),
                        new SqlParameter("@UserIdentifier", r.UserIdentifier),
                        new SqlParameter("@PrimaryProfileIdentifier", (object)r.PrimaryProfileIdentifier ?? DBNull.Value),
                        new SqlParameter("@SnapshotDate", today),

                        new SqlParameter("@CompetencyCountRequired", r.CompetencyCountRequired),
                        new SqlParameter("@CompetencyCountExpired", r.CompetencyCountExpired),
                        new SqlParameter("@CompetencyCountNotCompleted", r.CompetencyCountNotCompleted),
                        new SqlParameter("@CompetencyCountNotApplicable", r.CompetencyCountNotApplicable),
                        new SqlParameter("@CompetencyCountNeedsTraining", r.CompetencyCountNeedsTraining),
                        new SqlParameter("@CompetencyCountSelfAssessed", r.CompetencyCountSelfAssessed),
                        new SqlParameter("@CompetencyCountSubmitted", r.CompetencyCountSubmitted),
                        new SqlParameter("@CompetencyCountValidated", r.CompetencyCountValidated),

                        new SqlParameter("@CompetencyPercentExpired", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CompetencyCountExpired, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@CompetencyPercentNotCompleted", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CompetencyCountNotCompleted, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@CompetencyPercentNotApplicable", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CompetencyCountNotApplicable, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@CompetencyPercentNeedsTraining", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CompetencyCountNeedsTraining, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@CompetencyPercentSelfAssessed", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CompetencyCountSelfAssessed, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@CompetencyPercentSubmitted", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CompetencyCountSubmitted, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@CompetencyPercentValidated", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CompetencyCountValidated, r.CompetencyCountRequired, 4))),

                        new SqlParameter("@CriticalCompetencyCountExpired", r.CriticalCompetencyCountExpired),
                        new SqlParameter("@CriticalCompetencyCountNotCompleted", r.CriticalCompetencyCountNotCompleted),
                        new SqlParameter("@CriticalCompetencyCountNotApplicable", r.CriticalCompetencyCountNotApplicable),
                        new SqlParameter("@CriticalCompetencyCountNeedsTraining", r.CriticalCompetencyCountNeedsTraining),
                        new SqlParameter("@CriticalCompetencyCountSelfAssessed", r.CriticalCompetencyCountSelfAssessed),
                        new SqlParameter("@CriticalCompetencyCountSubmitted", r.CriticalCompetencyCountSubmitted),
                        new SqlParameter("@NonCriticalCompetencyCountExpired", r.NonCriticalCompetencyCountExpired),
                        new SqlParameter("@NonCriticalCompetencyCountNotCompleted", r.NonCriticalCompetencyCountNotCompleted),
                        new SqlParameter("@NonCriticalCompetencyCountNotApplicable", r.NonCriticalCompetencyCountNotApplicable),
                        new SqlParameter("@NonCriticalCompetencyCountNeedsTraining", r.NonCriticalCompetencyCountNeedsTraining),
                        new SqlParameter("@NonCriticalCompetencyCountSelfAssessed", r.NonCriticalCompetencyCountSelfAssessed),
                        new SqlParameter("@NonCriticalCompetencyCountSubmitted", r.NonCriticalCompetencyCountSubmitted),

                        new SqlParameter("@CriticalCompetencyPercentExpired", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CriticalCompetencyCountExpired, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@CriticalCompetencyPercentNotCompleted", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CriticalCompetencyCountNotCompleted, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@CriticalCompetencyPercentNotApplicable", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CriticalCompetencyCountNotApplicable, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@CriticalCompetencyPercentNeedsTraining", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CriticalCompetencyCountNeedsTraining, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@CriticalCompetencyPercentSelfAssessed", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CriticalCompetencyCountSelfAssessed, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@CriticalCompetencyPercentSubmitted", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CriticalCompetencyCountSubmitted, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@CriticalCompetencyPercentValidated", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CriticalCompetencyCountValidated, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@NonCriticalCompetencyPercentExpired", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.NonCriticalCompetencyCountExpired, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@NonCriticalCompetencyPercentNotCompleted", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.NonCriticalCompetencyCountNotCompleted, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@NonCriticalCompetencyPercentNotApplicable", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.NonCriticalCompetencyCountNotApplicable, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@NonCriticalCompetencyPercentNeedsTraining", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.NonCriticalCompetencyCountNeedsTraining, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@NonCriticalCompetencyPercentSelfAssessed", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.NonCriticalCompetencyCountSelfAssessed, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@NonCriticalCompetencyPercentSubmitted", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.NonCriticalCompetencyCountSubmitted, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@NonCriticalCompetencyPercentValidated", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.NonCriticalCompetencyCountValidated, r.CompetencyCountRequired, 4))),

                        new SqlParameter("@CriticalComplianceCompetencyCountRequired", r.CriticalComplianceCompetencyCountRequired),
                        new SqlParameter("@CriticalComplianceCompetencyCountExpired", r.CriticalComplianceCompetencyCountExpired),
                        new SqlParameter("@CriticalComplianceCompetencyCountNotCompleted", r.CriticalComplianceCompetencyCountNotCompleted),
                        new SqlParameter("@CriticalComplianceCompetencyCountNotApplicable", r.CriticalComplianceCompetencyCountNotApplicable),
                        new SqlParameter("@CriticalComplianceCompetencyCountNeedsTraining", r.CriticalComplianceCompetencyCountNeedsTraining),
                        new SqlParameter("@CriticalComplianceCompetencyCountSelfAssessed", r.CriticalComplianceCompetencyCountSelfAssessed),
                        new SqlParameter("@CriticalComplianceCompetencyCountSubmitted", r.CriticalComplianceCompetencyCountSubmitted),
                        new SqlParameter("@CriticalComplianceCompetencyCountValidated", r.CriticalComplianceCompetencyCountValidated),
                        new SqlParameter("@NonCriticalComplianceCompetencyCountRequired", r.NonCriticalComplianceCompetencyCountRequired),
                        new SqlParameter("@NonCriticalComplianceCompetencyCountExpired", r.NonCriticalComplianceCompetencyCountExpired),
                        new SqlParameter("@NonCriticalComplianceCompetencyCountNotCompleted", r.NonCriticalComplianceCompetencyCountNotCompleted),
                        new SqlParameter("@NonCriticalComplianceCompetencyCountNotApplicable", r.NonCriticalComplianceCompetencyCountNotApplicable),
                        new SqlParameter("@NonCriticalComplianceCompetencyCountNeedsTraining", r.NonCriticalComplianceCompetencyCountNeedsTraining),
                        new SqlParameter("@NonCriticalComplianceCompetencyCountSelfAssessed", r.NonCriticalComplianceCompetencyCountSelfAssessed),
                        new SqlParameter("@NonCriticalComplianceCompetencyCountSubmitted", r.NonCriticalComplianceCompetencyCountSubmitted),
                        new SqlParameter("@NonCriticalComplianceCompetencyCountValidated", r.NonCriticalComplianceCompetencyCountValidated),

                        new SqlParameter("@CriticalComplianceCompetencyPercentExpired", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CriticalComplianceCompetencyCountExpired, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@CriticalComplianceCompetencyPercentNotCompleted", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CriticalComplianceCompetencyCountNotCompleted, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@CriticalComplianceCompetencyPercentNotApplicable", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CriticalComplianceCompetencyCountNotApplicable, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@CriticalComplianceCompetencyPercentNeedsTraining", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CriticalComplianceCompetencyCountNeedsTraining, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@CriticalComplianceCompetencyPercentSelfAssessed", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CriticalComplianceCompetencyCountSelfAssessed, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@CriticalComplianceCompetencyPercentSubmitted", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CriticalComplianceCompetencyCountSubmitted, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@CriticalComplianceCompetencyPercentValidated", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CriticalComplianceCompetencyCountValidated, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@NonCriticalComplianceCompetencyPercentExpired", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.NonCriticalComplianceCompetencyCountExpired, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@NonCriticalComplianceCompetencyPercentNotCompleted", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.NonCriticalComplianceCompetencyCountNotCompleted, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@NonCriticalComplianceCompetencyPercentNotApplicable", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.NonCriticalComplianceCompetencyCountNotApplicable, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@NonCriticalComplianceCompetencyPercentNeedsTraining", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.NonCriticalComplianceCompetencyCountNeedsTraining, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@NonCriticalComplianceCompetencyPercentSelfAssessed", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.NonCriticalComplianceCompetencyCountSelfAssessed, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@NonCriticalComplianceCompetencyPercentSubmitted", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.NonCriticalComplianceCompetencyCountSubmitted, r.CompetencyCountRequired, 4))),
                        new SqlParameter("@NonCriticalComplianceCompetencyPercentValidated", r.CompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.NonCriticalComplianceCompetencyCountValidated, r.CompetencyCountRequired, 4))),

                        new SqlParameter("@CriticalComplianceCompetencyScore", r.CriticalComplianceCompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CriticalComplianceCompetencyCountValidated + r.CriticalComplianceCompetencyCountNotApplicable, r.CriticalComplianceCompetencyCountRequired, 4))),
                        new SqlParameter("@NonCriticalComplianceCompetencyScore", r.NonCriticalComplianceCompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.NonCriticalComplianceCompetencyCountValidated + r.NonCriticalComplianceCompetencyCountNotApplicable, r.NonCriticalComplianceCompetencyCountRequired, 4))),

                        new SqlParameter("@TimeSensitiveSafetyCertificateCountRequired", r.TimeSensitiveSafetyCertificateCountRequired),
                        new SqlParameter("@TimeSensitiveSafetyCertificateCountValidated", r.TimeSensitiveSafetyCertificateCountValidated),
                        new SqlParameter("@TimeSensitiveSafetyCertificateScore", r.TimeSensitiveSafetyCertificateCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.TimeSensitiveSafetyCertificateCountValidated, r.TimeSensitiveSafetyCertificateCountRequired, 4))),

                        new SqlParameter("@AdditionalComplianceRequirementCountRequired", r.AdditionalComplianceRequirementCountRequired),
                        new SqlParameter("@AdditionalComplianceRequirementCountValidated", r.AdditionalComplianceRequirementCountValidated),
                        new SqlParameter("@AdditionalComplianceRequirementScore", r.AdditionalComplianceRequirementCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.AdditionalComplianceRequirementCountValidated, r.AdditionalComplianceRequirementCountRequired, 4))),

                        new SqlParameter("@CriticalCompetencyCountRequired", r.CriticalCompetencyCountRequired),
                        new SqlParameter("@CriticalCompetencyCountValidated", r.CriticalCompetencyCountValidated),
                        new SqlParameter("@CriticalCompetencyScore", r.CriticalCompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CriticalCompetencyCountValidated + r.CriticalCompetencyCountNotApplicable, r.CriticalCompetencyCountRequired, 4))),

                        new SqlParameter("@NonCriticalCompetencyCountRequired", r.NonCriticalCompetencyCountRequired),
                        new SqlParameter("@NonCriticalCompetencyCountValidated", r.NonCriticalCompetencyCountValidated),
                        new SqlParameter("@NonCriticalCompetencyScore", r.NonCriticalCompetencyCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.NonCriticalCompetencyCountValidated + r.NonCriticalCompetencyCountNotApplicable, r.NonCriticalCompetencyCountRequired, 4))),

                        new SqlParameter("@CodesOfPracticeCountRequired", r.CodesOfPracticeCountRequired),
                        new SqlParameter("@CodesOfPracticeCountValidated", r.CodesOfPracticeCountValidated),
                        new SqlParameter("@CodesOfPracticeScore", r.CodesOfPracticeCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.CodesOfPracticeCountValidated, r.CodesOfPracticeCountRequired, 4))),

                        new SqlParameter("@SafeOperatingPracticeCountRequired", r.SafeOperatingPracticeCountRequired),
                        new SqlParameter("@SafeOperatingPracticeCountValidated", r.SafeOperatingPracticeCountValidated),
                        new SqlParameter("@SafeOperatingPracticeScore", r.SafeOperatingPracticeCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.SafeOperatingPracticeCountValidated, r.SafeOperatingPracticeCountRequired, 4))),

                        new SqlParameter("@HumanResourceDocumentsCountRequired", r.HumanResourceDocumentsCountRequired),
                        new SqlParameter("@HumanResourceDocumentsCountValidated", r.HumanResourceDocumentsCountValidated),
                        new SqlParameter("@HumanResourceDocumentsScore", r.HumanResourceDocumentsCountRequired == 0 ? 1.0M : Math.Max(0.0001M, Calculator.GetPercentDecimal(r.HumanResourceDocumentsCountValidated, r.HumanResourceDocumentsCountRequired, 4))),

                        new SqlParameter("@ModuleCountRequired", r.ModuleCountRequired),
                        new SqlParameter("@ModuleCountValidated", r.ModuleCountValidated),
                        new SqlParameter("@ModuleScore", r.ModuleCountRequired == 0 ? 1M : Calculator.GetPercentDecimal(r.ModuleCountValidated, r.ModuleCountRequired, 1)),

                        new SqlParameter("@TrainingGuideCountRequired", r.TrainingGuideCountRequired),
                        new SqlParameter("@TrainingGuideCountValidated", r.TrainingGuideCountValidated),
                        new SqlParameter("@TrainingGuideScore", r.TrainingGuideCountRequired == 0 ? 1M : Calculator.GetPercentDecimal(r.TrainingGuideCountValidated, r.TrainingGuideCountRequired, 1)),

                        new SqlParameter("@SiteSpecificOperatingProcedureCountRequired", r.SiteSpecificOperatingProcedureCountRequired),
                        new SqlParameter("@SiteSpecificOperatingProcedureCountValidated", r.SiteSpecificOperatingProcedureCountValidated),
                        new SqlParameter("@SiteSpecificOperatingProcedureScore", r.SiteSpecificOperatingProcedureCountRequired == 0 ? 1M : Calculator.GetPercentDecimal(r.SiteSpecificOperatingProcedureCountValidated, r.SiteSpecificOperatingProcedureCountRequired, 1)),

                        new SqlParameter("@OrientationCountRequired", r.OrientationCountRequired),
                        new SqlParameter("@OrientationCountValidated", r.OrientationCountValidated),
                        new SqlParameter("@OrientationScore", r.OrientationCountRequired == 0 ? 1M : Calculator.GetPercentDecimal(r.OrientationCountValidated, r.OrientationCountRequired, 1))
                    );
                }

                context.Database.ExecuteSqlCommand(DeleteOutdatedSnapshotsQuery);

                context.Database.ExecuteSqlCommand(InsertSummaryQuery);
            }
        }
    }
}
