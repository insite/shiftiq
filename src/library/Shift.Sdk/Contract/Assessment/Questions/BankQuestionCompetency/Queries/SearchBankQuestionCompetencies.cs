using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchBankQuestionCompetencies : Query<IEnumerable<BankQuestionCompetencyMatch>>, IBankQuestionCompetencyCriteria
    {

    }
}