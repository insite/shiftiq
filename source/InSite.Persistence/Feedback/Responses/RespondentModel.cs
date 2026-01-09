using System;
using System.Linq.Expressions;

using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class RespondentModel
    {
        #region Properties

        public Guid UserIdentifier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid Thumbprint { get; set; }
        public string Email { get; set; }

        public bool IsAnonymous { get; set; }

        #endregion

        #region Fields

        public static readonly Func<User, RespondentModel> BinderFunction;
        public static readonly Expression<Func<User, RespondentModel>> BinderExpression;

        #endregion

        #region Construction

        static RespondentModel()
        {
            BinderExpression = LinqExtensions1.Expr((User x) => new RespondentModel
            {
                UserIdentifier = x.UserIdentifier,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Thumbprint = x.UserIdentifier,
                Email = x.Email
            });
            BinderFunction = BinderExpression.Compile();
        }

        #endregion
    }
}
