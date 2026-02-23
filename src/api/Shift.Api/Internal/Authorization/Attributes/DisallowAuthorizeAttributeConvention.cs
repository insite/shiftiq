using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Shift.Api
{
    /// <summary>
    /// Enforces the use of specific authorization attributes on all API controllers and methods
    /// </summary>
    public class DisallowAuthorizeAttributeConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            var allowed = new[] { "Hybrid", "Bearer", "Cookie", "Secret" };

            foreach (var controller in application.Controllers)
            {
                // Check controller-level attributes
                var controllerUnauthorizedAttributes = controller.Attributes
                    .Where(attr => attr.GetType() == typeof(AuthorizeAttribute))
                    .ToList();

                if (controllerUnauthorizedAttributes.Any())
                {
                    var error = $"The controller class \"{controller.DisplayName}\" uses the [Authorize] attribute,"
                        + " which is not allowed. Use one of these authorization attributes instead: "
                        + string.Join(" | ", allowed);
                    throw new InvalidOperationException(error);
                }

                // Check method-level attributes
                foreach (var action in controller.Actions)
                {
                    var actionUnauthorizedAttributes = action.Attributes
                        .Where(attr => attr.GetType() == typeof(AuthorizeAttribute))
                        .ToList();

                    if (actionUnauthorizedAttributes.Any())
                    {
                        var error = $"The action method \"{action.DisplayName}\" in controller \"{controller.DisplayName}\" uses the [Authorize] attribute,"
                            + " which is not allowed. Use one of these authorization attributes instead: "
                            + string.Join(" | ", allowed);
                        throw new InvalidOperationException(error);
                    }
                }
            }
        }
    }
}
