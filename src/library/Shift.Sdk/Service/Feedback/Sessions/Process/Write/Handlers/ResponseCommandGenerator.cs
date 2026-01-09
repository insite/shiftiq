using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application.Responses.Write;
using InSite.Domain.Surveys.Sessions;

using Shift.Common;

namespace InSite.Application.Surveys.Write
{
    public class ResponseCommandGenerator
    {
        /// <summary>
        /// Returns the list of commands to create an existing survey response session.
        /// </summary>
        public ICommand[] GetCommands(ResponseSession session)
        {
            var id = session.Identifier;
            var script = new List<ICommand>
            {
                new CreateResponseSession(id, session.Source, session.Tenant, session.Form, session.User),
            };

            foreach (var answer in session.Answers)
            {
                script.Add(new AddResponseAnswer(id, answer.Question));
                if (answer.Options.IsNotEmpty())
                    script.Add(new AddResponseOptions(id, answer.Question, answer.Options.Select(x => x.Item).ToArray()));
            }

            if (session.Started.HasValue)
                script.Add(new StartResponseSession(id, session.Started, false));

            foreach (var answer in session.Answers)
            {
                if (answer.Answer != null)
                    script.Add(new ChangeResponseAnswer(id, answer.Question, answer.Answer));

                if (answer.Options != null)
                    foreach (var option in answer.Options)
                        if (option.IsSelected)
                            script.Add(new SelectResponseOption(id, option.Item));
            }

            if (session.Completed.HasValue)
                script.Add(new CompleteResponseSession(id, session.Completed));

            return script.ToArray();
        }
    }
}
