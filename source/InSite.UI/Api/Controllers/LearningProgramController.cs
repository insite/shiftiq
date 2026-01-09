using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Application.Records.Read;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Api.Controllers
{
    [DisplayName("Learning")]
    public partial class ProgramsController : ApiBaseController
    {
        private static HashSet<Guid> _processingIds = new HashSet<Guid>();

        /// <summary>
        /// Update an existing program
        /// </summary>
        /// <remarks>
        /// The program ID is provided as part of the request URL. The following parameters must be provided in the request body:
        /// <para>ProgramName, ProgramType, AssessmentFormCode, AssessmentFormName, Created, Modified</para>
        /// </remarks>
        [Route("api/programs/{program}")]
        [HttpPut]
        public HttpResponseMessage PutProgram([FromUri] Guid program, UpdateProgram update)
        {
            try
            {
                var created = false;
                var organization = GetOrganization();
                TProgram p;

                LockId(program);

                try
                {
                    p = ProgramSearch.GetProgram(program);
                    if (p == null)
                    {
                        p = new TProgram
                        {
                            OrganizationIdentifier = organization.OrganizationIdentifier,
                            ProgramIdentifier = program,
                            ProgramName = update.ProgramName,
                            ProgramCode = update.ProgramType
                        };
                        ProgramStore.Insert(p, CurrentUser?.Identifier ?? UserIdentifiers.Someone);
                        created = true;
                    }
                }
                finally
                {
                    UnlockId(program);
                }

                if (p != null)
                {
                    if (!StringHelper.Equals(p.ProgramName, update.ProgramName))
                        p.ProgramName = update.ProgramName;

                    if (!StringHelper.Equals(p.ProgramCode, update.ProgramType))
                        p.ProgramCode = update.ProgramType;
                }

                p.ProgramDescription = $"Assessment Form {update.AssessmentFormCode}: {update.AssessmentFormName}";

                ProgramStore.Update(p, CurrentUser?.Identifier ?? UserIdentifiers.Someone);

                return JsonSuccess(update, created ? HttpStatusCode.Created : HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return JsonError(ex.GetAllMessages());
            }
        }

        private static void LockId(Guid id)
        {
            const int msPerTick = 50;
            const int ticksPerSecond = 20;

            for (int i = 0; i < 5 * ticksPerSecond; i++) // Wait up to 5 seconds
            {
                lock (_processingIds)
                {
                    if (_processingIds.Add(id))
                        return;
                }

                Thread.Sleep(msPerTick);
            }

            throw new ApplicationException($"The program {id} is locked");
        }

        private static void UnlockId(Guid id)
        {
            lock (_processingIds)
            {
                _processingIds.Remove(id);
            }
        }
    }
}