using System;

namespace Shift.Sdk.UI
{
    public class StatementModel
    {
        public Guid StatementIdentifier { get; set; }
        public string statementData { private get; set; }

        private StatementData _statementData;
        public StatementData StatementDataObject
        {
            get
            {
                if (_statementData == null)
                    _statementData = Newtonsoft.Json.JsonConvert.DeserializeObject<StatementData>(statementData);
                return _statementData;
            }
        }
    }
}