using System;

namespace Shift.Sdk.UI
{
    public class AttemptSessionInfo
    {
        #region Properties

        public string SessionId { get; private set; }

        public DateTime Timestamp { get; private set; }

        public bool IsRemoved { get; private set; }

        public object SyncRoot { get; }

        #endregion

        #region Construction

        public AttemptSessionInfo(string id)
        {
            SyncRoot = new object();

            Update(id);
        }

        #endregion

        #region Methods

        public void Update()
        {
            Timestamp = DateTime.UtcNow;
        }

        public void Update(string sessionId)
        {
            SessionId = sessionId;

            Update();
        }

        public void Remove()
        {
            IsRemoved = true;
        }

        #endregion
    }
}