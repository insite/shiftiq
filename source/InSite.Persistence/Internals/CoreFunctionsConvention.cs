using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace InSite.Persistence
{
    public class CoreFunctionsConvention : IStoreModelConvention<EntityContainer>
    {
        public void Apply(EntityContainer item, DbModel model)
        {
            var edmStringType = PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.String);
            var edmStringListType = edmStringType.GetCollectionType();
            var edmBooleanType = PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Boolean);
            var edmGuidType = PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Guid);

            var concatenateText = EdmFunction.Create("ConcatenateText", "toolbox", DataSpace.SSpace, new EdmFunctionPayload
            {
                ParameterTypeSemantics = ParameterTypeSemantics.AllowImplicitConversion,
                IsComposable = true,
                IsAggregate = true,
                Schema = "toolbox",
                ReturnParameters = new[] { FunctionParameter.Create("ReturnType", edmStringType, ParameterMode.ReturnValue) },
                Parameters = new[]
                {
                    FunctionParameter.Create("input", edmStringListType, ParameterMode.In),
                    FunctionParameter.Create("separator", edmStringType, ParameterMode.In)
                }
            }, null);
            
            var getBodyHtmlEn = EdmFunction.Create("GetBodyHtmlEn", "contents", DataSpace.SSpace, new EdmFunctionPayload
            {
                ParameterTypeSemantics = ParameterTypeSemantics.AllowImplicitConversion,
                IsComposable = true,
                Schema = "contents",
                ReturnParameters = new[] { FunctionParameter.Create("ReturnType", edmStringType, ParameterMode.ReturnValue) },
                Parameters = new[] { FunctionParameter.Create("json", edmStringType, ParameterMode.In) }
            }, null);

            var getBodyTextEn = EdmFunction.Create("GetBodyTextEn", "contents", DataSpace.SSpace, new EdmFunctionPayload
            {
                ParameterTypeSemantics = ParameterTypeSemantics.AllowImplicitConversion,
                IsComposable = true,
                Schema = "contents",
                ReturnParameters = new[] { FunctionParameter.Create("ReturnType", edmStringType, ParameterMode.ReturnValue) },
                Parameters = new[] { FunctionParameter.Create("json", edmStringType, ParameterMode.In) }
            }, null);

            var getContentText = EdmFunction.Create("GetContentText", "contents", DataSpace.SSpace, new EdmFunctionPayload
            {
                ParameterTypeSemantics = ParameterTypeSemantics.AllowImplicitConversion,
                IsComposable = true,
                Schema = "contents",
                ReturnParameters = new[] { FunctionParameter.Create("ReturnType", edmStringType, ParameterMode.ReturnValue) },
                Parameters = new[]
                {
                    FunctionParameter.Create("id", edmGuidType, ParameterMode.In),
                    FunctionParameter.Create("label", edmStringType, ParameterMode.In),
                    FunctionParameter.Create("language", edmStringType, ParameterMode.In)
                }
            }, null);

            var getContentTextEn = EdmFunction.Create("GetContentTextEn", "contents", DataSpace.SSpace, new EdmFunctionPayload
            {
                ParameterTypeSemantics = ParameterTypeSemantics.AllowImplicitConversion,
                IsComposable = true,
                Schema = "contents",
                ReturnParameters = new[] { FunctionParameter.Create("ReturnType", edmStringType, ParameterMode.ReturnValue) },
                Parameters = new[]
                {
                    FunctionParameter.Create("id", edmGuidType, ParameterMode.In),
                    FunctionParameter.Create("label", edmStringType, ParameterMode.In)
                }
            }, null);

            var getContentHtmlEn = EdmFunction.Create("GetContentHtmlEn", "contents", DataSpace.SSpace, new EdmFunctionPayload
            {
                ParameterTypeSemantics = ParameterTypeSemantics.AllowImplicitConversion,
                IsComposable = true,
                Schema = "contents",
                ReturnParameters = new[] { FunctionParameter.Create("ReturnType", edmStringType, ParameterMode.ReturnValue) },
                Parameters = new[]
                {
                    FunctionParameter.Create("id", edmGuidType, ParameterMode.In),
                    FunctionParameter.Create("label", edmStringType, ParameterMode.In)
                }
            }, null);

            var getDescriptionEn = EdmFunction.Create("GetDescriptionEn", "contents", DataSpace.SSpace, new EdmFunctionPayload
            {
                ParameterTypeSemantics = ParameterTypeSemantics.AllowImplicitConversion,
                IsComposable = true,
                Schema = "contents",
                ReturnParameters = new[] { FunctionParameter.Create("ReturnType", edmStringType, ParameterMode.ReturnValue) },
                Parameters = new[] { FunctionParameter.Create("json", edmStringType, ParameterMode.In) }
            }, null);

            var getSummaryEn = EdmFunction.Create("GetSummaryEn", "contents", DataSpace.SSpace, new EdmFunctionPayload
            {
                ParameterTypeSemantics = ParameterTypeSemantics.AllowImplicitConversion,
                IsComposable = true,
                Schema = "contents",
                ReturnParameters = new[] { FunctionParameter.Create("ReturnType", edmStringType, ParameterMode.ReturnValue) },
                Parameters = new[] { FunctionParameter.Create("json", edmStringType, ParameterMode.In) }
            }, null);

            var getTitleEn = EdmFunction.Create("GetTitleEn", "contents", DataSpace.SSpace, new EdmFunctionPayload
            {
                ParameterTypeSemantics = ParameterTypeSemantics.AllowImplicitConversion,
                IsComposable = true,
                Schema = "contents",
                ReturnParameters = new[] { FunctionParameter.Create("ReturnType", edmStringType, ParameterMode.ReturnValue) },
                Parameters = new[] { FunctionParameter.Create("json", edmStringType, ParameterMode.In) }
            }, null);

            var getTranslation = EdmFunction.Create("Translate", "contents", DataSpace.SSpace, new EdmFunctionPayload
            {
                ParameterTypeSemantics = ParameterTypeSemantics.AllowImplicitConversion,
                IsComposable = true,
                Schema = "contents",
                ReturnParameters = new[] { FunctionParameter.Create("ReturnType", edmStringType, ParameterMode.ReturnValue) },
                Parameters = new[]
                {
                    FunctionParameter.Create("Json", edmStringType, ParameterMode.In),
                    FunctionParameter.Create("Language", edmStringType, ParameterMode.In)
                }
            }, null);

            var isContentContains = EdmFunction.Create("IsContentContains", "contents", DataSpace.SSpace, new EdmFunctionPayload
            {
                ParameterTypeSemantics = ParameterTypeSemantics.AllowImplicitConversion,
                IsComposable = true,
                Schema = "contents",
                ReturnParameters = new[] { FunctionParameter.Create("ReturnType", edmBooleanType, ParameterMode.ReturnValue) },
                Parameters = new[]
                {
                    FunctionParameter.Create("id", edmGuidType, ParameterMode.In),
                    FunctionParameter.Create("label", edmStringType, ParameterMode.In),
                    FunctionParameter.Create("language", edmStringType, ParameterMode.In),
                    FunctionParameter.Create("keyword", edmStringType, ParameterMode.In)
                }
            }, null);

            var isContentTranslated = EdmFunction.Create("IsContentTranslated", "contents", DataSpace.SSpace, new EdmFunctionPayload
            {
                ParameterTypeSemantics = ParameterTypeSemantics.AllowImplicitConversion,
                IsComposable = true,
                Schema = "contents",
                ReturnParameters = new[] { FunctionParameter.Create("ReturnType", edmBooleanType, ParameterMode.ReturnValue) },
                Parameters = new[]
                {
                    FunctionParameter.Create("id", edmGuidType, ParameterMode.In),
                    FunctionParameter.Create("label", edmStringType, ParameterMode.In),
                }
            }, null);

            var isRegexMatch = EdmFunction.Create("IsRegexMatch", "toolbox", DataSpace.SSpace, new EdmFunctionPayload
            {
                ParameterTypeSemantics = ParameterTypeSemantics.AllowImplicitConversion,
                IsComposable = true,
                Schema = "toolbox",
                ReturnParameters = new[] { FunctionParameter.Create("ReturnType", edmBooleanType, ParameterMode.ReturnValue) },
                Parameters = new[] { FunctionParameter.Create("input", edmStringType, ParameterMode.In), FunctionParameter.Create("pattern", edmStringType, ParameterMode.In) }
            }, null);

            var getTrimHtml = EdmFunction.Create("TrimHtml", "utilities", DataSpace.SSpace, new EdmFunctionPayload
            {
                ParameterTypeSemantics = ParameterTypeSemantics.AllowImplicitConversion,
                IsComposable = true,
                Schema = "utilities",
                ReturnParameters = new[] { FunctionParameter.Create("ReturnType", edmStringType, ParameterMode.ReturnValue) },
                Parameters = new[] { FunctionParameter.Create("HTMLText", edmStringType, ParameterMode.In) }
            }, null);

            model.StoreModel.AddItem(concatenateText);
            model.StoreModel.AddItem(getBodyHtmlEn);
            model.StoreModel.AddItem(getBodyTextEn);
            model.StoreModel.AddItem(getContentText);
            model.StoreModel.AddItem(getContentTextEn);
            model.StoreModel.AddItem(getContentHtmlEn);
            model.StoreModel.AddItem(getDescriptionEn);
            model.StoreModel.AddItem(getSummaryEn);
            model.StoreModel.AddItem(getTitleEn);
            model.StoreModel.AddItem(getTranslation);
            model.StoreModel.AddItem(isContentContains);
            model.StoreModel.AddItem(isContentTranslated);
            model.StoreModel.AddItem(isRegexMatch);
            model.StoreModel.AddItem(getTrimHtml);

            model.Compile();
        }
    }
}
