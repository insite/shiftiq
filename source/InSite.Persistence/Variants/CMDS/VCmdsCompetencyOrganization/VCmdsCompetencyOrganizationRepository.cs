using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application.Standards.Write;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class VCmdsCompetencyOrganizationRepository
    {
        #region Initialization

        private static Action<ICommand> _sendCommand;
        private static Action<IEnumerable<ICommand>> _sendCommands;

        public static void Initialize(Action<ICommand> sendCommand, Action<IEnumerable<ICommand>> sendCommands)
        {
            _sendCommand = sendCommand;
            _sendCommands = sendCommands;
        }

        #endregion

        public static VCmdsCompetencyOrganization Select(Guid organizationId, Guid competencyId)
        {
            using (var db = new InternalDbContext())
                return db.VCmdsCompetencyOrganizations.FirstOrDefault(x => x.OrganizationIdentifier == organizationId && x.CompetencyStandardIdentifier == competencyId);
        }

        public static int CountByOrganization(Guid organizationId)
        {
            using (var db = new InternalDbContext())
                return db.VCmdsCompetencyOrganizations.Where(x => x.OrganizationIdentifier == organizationId).Count();
        }

        public static void DeleteUnusedByOrganizationIdentifier(Guid organizationId)
        {
            var commands = new List<ICommand>();

            using (var db = new InternalDbContext())
            {
                var entities = db.Database
                   .SqlQuery<StandardOrganization>(
                        "EXEC custom_cmds.SelectUnusedCompanyCompetencyByOrganization @OrganizationIdentifier",
                        new SqlParameter("@OrganizationIdentifier", organizationId))
                   .ToArray();

                foreach (var group in entities.GroupBy(x => x.StandardIdentifier))
                    commands.Add(new RemoveStandardOrganization(
                        group.Key,
                        group.Select(x => x.OrganizationIdentifier).ToArray()));
            }

            if (commands.IsNotEmpty())
                _sendCommands(commands);
        }

        public static void DeleteUnusedByProfileId(Guid profileStandardId)
        {
            var commands = new List<ICommand>();

            using (var db = new InternalDbContext())
            {
                var entities = db.Database
                   .SqlQuery<StandardOrganization>(
                        "EXEC custom_cmds.SelectUnusedCompanyCompetencyByProfile @ProfileStandardIdentifier",
                        new SqlParameter("@ProfileStandardIdentifier", profileStandardId))
                   .ToArray();

                foreach (var group in entities.GroupBy(x => x.StandardIdentifier))
                    commands.Add(new RemoveStandardOrganization(
                        group.Key,
                        group.Select(x => x.OrganizationIdentifier).ToArray()));
            }

            if (commands.IsNotEmpty())
                _sendCommands(commands);
        }

        public static void InsertProfileCompetencies(Guid organizationId, Guid profileStandardIdentifier)
        {
            var commands = new List<ICommand>();

            using (var db = new InternalDbContext())
            {
                var standardIds = db.Database
                   .SqlQuery<Guid>(
                        "EXEC custom_cmds.SelectCompanyProfileCompetenciesForInsert @OrganizationIdentifier, @ProfileStandardIdentifier",
                        new SqlParameter("@OrganizationIdentifier", organizationId),
                        new SqlParameter("@ProfileStandardIdentifier", profileStandardIdentifier))
                   .ToArray();

                foreach (var standardId in standardIds)
                    commands.Add(new AddStandardOrganization(standardId, organizationId));
            }

            if (commands.IsNotEmpty())
                _sendCommands(commands);
        }

        public static void InsertProfileCompetencies(Guid profileStandardIdentifier)
        {
            var commands = new List<ICommand>();

            using (var db = new InternalDbContext())
            {
                db.Database.ExecuteSqlCommand(
                    "EXEC custom_cmds.InsertDepartmentProfileCompetenciesByProfile @ProfileStandardIdentifier",
                    new SqlParameter("@ProfileStandardIdentifier", profileStandardIdentifier));

                var entities = db.Database
                   .SqlQuery<StandardOrganization>(
                        "EXEC custom_cmds.SelectProfileCompetenciesForInsert @ProfileStandardIdentifier",
                        new SqlParameter("@ProfileStandardIdentifier", profileStandardIdentifier))
                   .ToArray();

                foreach (var group in entities.GroupBy(x => x.StandardIdentifier))
                    commands.Add(new AddStandardOrganization(
                        group.Key,
                        group.Select(x => x.OrganizationIdentifier).ToArray()));
            }

            if (commands.IsNotEmpty())
                _sendCommands(commands);
        }

        public static int UpdateCompanyCompetencies()
        {
            var result = 0;
            var commands = new List<ICommand>();

            using (var db = new InternalDbContext())
            {
                var entities = db.Database
                   .SqlQuery<StandardOrganization>("EXEC custom_cmds.SelectProfileCompetenciesForInsert NULL")
                   .ToArray();

                foreach (var group in entities.GroupBy(x => x.StandardIdentifier))
                    commands.Add(new AddStandardOrganization(
                        group.Key,
                        group.Select(x => x.OrganizationIdentifier).ToArray()));

                result = entities.Length;
            }

            if (commands.IsNotEmpty())
                _sendCommands(commands);

            return result;
        }
    }
}
