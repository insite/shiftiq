using System;
using System.Collections.Generic;

using Shift.Toolbox.Reporting.PerformanceReport.Models;

namespace Shift.Toolbox.Reporting.PerformanceReport.NCAS
{
    static class DocumentText
    {
        public class Text
        {
            public string TitleText { get; set; }
            public string AssessmentTakerText { get; set; }
            public string InspireIDText { get; set; }
            public string NursingRoleText { get; set; }
            public string ReportIDText { get; set; }
            public string DateReportIssuedText { get; set; }
            public string CBAAdministrationDateText { get; set; }
            public string SLAAdministrationDateText { get; set; }

            public string Description { get; set; }
            public string Figure1 { get; set; }
            public string Table1 { get; set; }
            public string NextSteps { get; set; }

            public string CompetencyDimension { get; set; }
            public string Undemonstrated { get; set; }
            public string Emergent { get; set; }
            public string Consistent { get; set; }

            public string OccupationInterest { get; set; }
            public string NursingRole { get; set; }
            public string DimensionColumnHeader { get; set; }
            public string DescriptionColumnHeader { get; set; }
            public string None { get; set; }
        }

        class English
        {
            public const string Title = "Inspire Performance Report";
            public const string AssessmentTaker = "Assessment Taker";
            public const string InspireId = "Inspire ID";
            public const string NursingRole = "Nursing Role";
            public const string ReportId = "Report ID";
            public const string DateReportIssued = "Date Report Issued";
            public const string CBAAdministrationDate = "CBA Administration Date";
            public const string SLAAdministrationDate = "SLA Administration Date";

            public const string Description = "The Inspire assessment evaluates the competencies that Canadian nursing regulators have deemed essential for entry to Canadian practice. The nursing regulator considers your assessment results, along with evidence about your education and professional experiences, to make a decision about your registration and/or required learning pathways."
                + "\n"
                + "To determine your competency–based readiness, this performance report blends your results from the computer based assessment (CBA) and simulation lab assessment (SLA). The results of your CBA and SLA are mapped to the core competency dimensions that regulators use to understand your performance. **Inspire does not report results on the basis of a pass or fail.** Rather, we report on competency gaps and strengths. Thus, performing well in one area cannot make up for not meeting expectations in another area. The Inspire assessment framework and competencies can be viewed on the Inspire website at http://www.inspireassessments.org."
                + "\n"
                + "**How to read your results.** Figure 1 displays your performance in each reporting dimension. The dotted lines represents the thresholds for Emergent and Consistent performance. Scores at or above \"Emergent\" indicate emerging but inconsistent evidence of competence in the dimension, while scores at or above \"Consistent\" indicates consistent evidence of competence in the dimension. Table 1 on the next page describes each dimension.";

            public const string Figure1 = "Figure 1: Your performance in each competency dimension";
            public const string Table1 = "Table 1: Description of Competency Dimensions";
            public const string NextSteps = "**Next steps.** Your report will be sent to the regulator(s), registry or employer you designated. They will consider this report, along with other documents, data and material you have submitted, to advise you on next steps in your registration or evaluation process, including on whether or not further education is required to address any competency gaps. Please contact that organization directly to determine the status of its decision.";

            public const string CompetencyDimension = "Competency dimension";
            public const string Undemonstrated = "Undemonstrated";
            public const string Emergent = "Emergent";
            public const string Consistent = "Consistent";

            public const string DimensionColumnHeader = "Dimension";
            public const string DescriptionColumnHeader = "Description";

            public const string None = "None";
        }

        class French
        {
            public const string Title = "Rapport Inspire Évaluations Mondiales";
            public const string AssessmentTaker = "Candidat(e)";
            public const string InspireId = "ID d’Inspire";
            public const string NursingRole = "Rôle en soins infirmiers";
            public const string ReportId = "ID du rapport";
            public const string DateReportIssued = "Date de publication du rapport";
            public const string CBAAdministrationDate = "Date du TAO";
            public const string SLAAdministrationDate = "Date de l’ELS";

            public const string Description = "L’évaluation d’Inspire permet d’évaluer les compétences jugées essentielles par les organismes canadiens de réglementation des soins infirmiers pour l’entrée dans la pratique canadienne. L’organisme de réglementation des soins infirmiers tient compte des résultats de votre évaluation, ainsi que des données sur votre formation et vos expériences professionnelles, pour prendre une décision au sujet de votre inscription et des parcours d’apprentissage requis."
                + "\n"
                + "Afin de déterminer votre niveau de préparation en fonction des compétences, ce rapport sur votre rendement combine les résultats du test assisté par ordinateur (TAO) ainsi que ceux de l’évaluation orale et de l’évaluation en laboratoire de simulation (EO et ELS). Les résultats du TAO, de l’ELS et de l’EO correspondent aux compétences de base utilisées par les organismes de réglementation pour comprendre votre rendement. **Inspire ne présente pas les résultats sous forme de réussite ou d’échec.** Au contraire, l’organisme présente un rapport sur vos forces et vos lacunes en matière de compétences. Par conséquent, le rendement dans un domaine ne peut compenser le fait de ne pas répondre aux attentes dans un autre. Le cadre d’évaluation et les compétences d’Inspire peuvent être consultés sur le site Web d’Inspire à l’adresse suivante http://www.inspireassessments.org/fr."
                + "\n"
                + "**La façon d’interpréter vos résultats.** La Figure 1 affiche votre rendement pour chaque compétence évaluée dans le rapport. Les lignes en pointillés représentent les seuils de rendement émergent et constant. Un rendement \" Émergent \" ou supérieur indique qu’il s’agit d’un niveau émergent, mais non constant pour la compétence évaluée, tandis qu’un rendement \" Constant \" ou supérieur indique un niveau constant pour la compétence évaluée. Le Tableau 1 de la page suivante décrit chaque compétence.";

            public const string Figure1 = "Figure 1 : Votre rendement pour chaque compétence";
            public const string Table1 = "Tableau 1 : Description des compétences";
            public const string NextSteps = "**Prochaines étapes.** Votre rapport sera envoyé à l’organisme ou aux organismes de réglementation, au registre ou à l’employeur que vous avez désigné. Ils l’examineront, ainsi que d’autres documents, données et matériel que vous avez présentés, afin de vous conseiller sur les prochaines étapes du processus d’inscription ou d’évaluation, y compris sur la question de savoir si une formation supplémentaire est nécessaire pour combler vos lacunes en matière de compétences. Veuillez communiquer directement avec cette organisation pour déterminer l’état d’avancement de sa décision.";

            public const string CompetencyDimension = "Compétence";
            public const string Undemonstrated = "Non demontrée";
            public const string Emergent = "Émergente";
            public const string Consistent = "Constante";

            public const string DimensionColumnHeader = "Dimension";
            public const string DescriptionColumnHeader = "Description";

            public const string None = "Aucune";
        }

        private static readonly Dictionary<UserReportType, Text> _text = new Dictionary<UserReportType, Text>
        {
            {
                UserReportType.Report1,
                new Text
                {
                    TitleText = English.Title,
                    AssessmentTakerText = English.AssessmentTaker,
                    InspireIDText = English.InspireId,
                    NursingRoleText = English.NursingRole,
                    ReportIDText = English.ReportId,
                    DateReportIssuedText = English.DateReportIssued,
                    CBAAdministrationDateText = English.CBAAdministrationDate,
                    SLAAdministrationDateText = English.SLAAdministrationDate,
                    Description = English.Description,
                    Figure1 = English.Figure1,
                    Table1 = English.Table1,
                    NextSteps = English.NextSteps,
                    CompetencyDimension = English.CompetencyDimension,
                    Undemonstrated = English.Undemonstrated,
                    Emergent = English.Emergent,
                    Consistent = English.Consistent,
                    OccupationInterest = "HCA",
                    NursingRole = "Health Care Assistant",
                    DimensionColumnHeader = English.DimensionColumnHeader,
                    DescriptionColumnHeader = English.DescriptionColumnHeader,
                    None = English.None,
                }
            },
            {
                UserReportType.Report2,
                new Text
                {
                    TitleText = English.Title,
                    AssessmentTakerText = English.AssessmentTaker,
                    InspireIDText = English.InspireId,
                    NursingRoleText = English.NursingRole,
                    ReportIDText = English.ReportId,
                    DateReportIssuedText = English.DateReportIssued,
                    CBAAdministrationDateText = English.CBAAdministrationDate,
                    SLAAdministrationDateText = English.SLAAdministrationDate,
                    Description = English.Description,
                    Figure1 = English.Figure1,
                    Table1 = English.Table1,
                    NextSteps = English.NextSteps,
                    CompetencyDimension = English.CompetencyDimension,
                    Undemonstrated = null,
                    Emergent = English.Emergent,
                    Consistent = English.Consistent,
                    OccupationInterest = "LPN",
                    NursingRole = "Licensed Practical Nurse",
                    DimensionColumnHeader = English.DimensionColumnHeader,
                    DescriptionColumnHeader = English.DescriptionColumnHeader,
                    None = English.None,
                }
            },
            {
                UserReportType.Report3,
                new Text
                {
                    TitleText = English.Title,
                    AssessmentTakerText = English.AssessmentTaker,
                    InspireIDText = English.InspireId,
                    NursingRoleText = English.NursingRole,
                    ReportIDText = English.ReportId,
                    DateReportIssuedText = English.DateReportIssued,
                    CBAAdministrationDateText = English.CBAAdministrationDate,
                    SLAAdministrationDateText = English.SLAAdministrationDate,
                    Description = English.Description,
                    Figure1 = English.Figure1,
                    Table1 = English.Table1,
                    NextSteps = English.NextSteps,
                    CompetencyDimension = English.CompetencyDimension,
                    Undemonstrated = null,
                    Emergent = English.Emergent,
                    Consistent = English.Consistent,
                    OccupationInterest = "RN",
                    NursingRole = "Registered Nurse",
                    DimensionColumnHeader = English.DimensionColumnHeader,
                    DescriptionColumnHeader = English.DescriptionColumnHeader,
                    None = English.None,
                }
            },
            {
                UserReportType.Report4,
                new Text
                {
                    TitleText = French.Title,
                    AssessmentTakerText = French.AssessmentTaker,
                    InspireIDText = French.InspireId,
                    NursingRoleText = French.NursingRole,
                    ReportIDText = French.ReportId,
                    DateReportIssuedText = French.DateReportIssued,
                    CBAAdministrationDateText = French.CBAAdministrationDate,
                    SLAAdministrationDateText = French.SLAAdministrationDate,
                    Description = French.Description,
                    Figure1 = French.Figure1,
                    Table1 = French.Table1,
                    NextSteps = French.NextSteps,
                    CompetencyDimension = French.CompetencyDimension,
                    Undemonstrated = French.Undemonstrated,
                    Emergent = French.Emergent,
                    Consistent = French.Consistent,
                    OccupationInterest = "PAS",
                    NursingRole = "PAS",
                    DimensionColumnHeader = French.DimensionColumnHeader,
                    DescriptionColumnHeader = French.DescriptionColumnHeader,
                    None = French.None,
                }
            },
            {
                UserReportType.Report5,
                new Text
                {
                    TitleText = French.Title,
                    AssessmentTakerText = French.AssessmentTaker,
                    InspireIDText = French.InspireId,
                    NursingRoleText = French.NursingRole,
                    ReportIDText = French.ReportId,
                    DateReportIssuedText = French.DateReportIssued,
                    CBAAdministrationDateText = French.CBAAdministrationDate,
                    SLAAdministrationDateText = French.SLAAdministrationDate,
                    Description = French.Description,
                    Figure1 = French.Figure1,
                    Table1 = French.Table1,
                    NextSteps = French.NextSteps,
                    CompetencyDimension = French.CompetencyDimension,
                    Undemonstrated = null,
                    Emergent = French.Emergent,
                    Consistent = French.Consistent,
                    OccupationInterest = "IAA",
                    NursingRole = "IAA",
                    DimensionColumnHeader = French.DimensionColumnHeader,
                    DescriptionColumnHeader = French.DescriptionColumnHeader,
                    None = French.None,
                }
            },
            {
                UserReportType.Report6,
                new Text
                {
                    TitleText = French.Title,
                    AssessmentTakerText = French.AssessmentTaker,
                    InspireIDText = French.InspireId,
                    NursingRoleText = French.NursingRole,
                    ReportIDText = French.ReportId,
                    DateReportIssuedText = French.DateReportIssued,
                    CBAAdministrationDateText = French.CBAAdministrationDate,
                    SLAAdministrationDateText = French.SLAAdministrationDate,
                    Description = French.Description,
                    Figure1 = French.Figure1,
                    Table1 = French.Table1,
                    NextSteps = French.NextSteps,
                    CompetencyDimension = French.CompetencyDimension,
                    Undemonstrated = null,
                    Emergent = French.Emergent,
                    Consistent = French.Consistent,
                    OccupationInterest = "IA",
                    NursingRole = "IA",
                    DimensionColumnHeader = French.DimensionColumnHeader,
                    DescriptionColumnHeader = French.DescriptionColumnHeader,
                    None = French.None,
                }
            },
        };

        public static Text GetText(UserReportType reportType)
        {
            return _text.TryGetValue(reportType, out var text)
                ? text
                : throw new ArgumentException($"reportType: {reportType}");
        }
    };
}
