using System;

using Shift.Common;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class RegisterFormValidationKey
    {
        public Guid Id { get; }

        public bool IsSubmitted
        {
            get => _isSubmitted;
            set => _isSubmitted = _isSubmitted || value;
        }

        private bool _isSubmitted;

        public RegisterFormValidationKey()
        {
            Id = UniqueIdentifier.Create();
            IsSubmitted = false;
        }
    }
}