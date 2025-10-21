using System;

using InSite.Application.Records.Read;

namespace InSite.UI.Portal.Records.Logbooks.Controls
{
    interface IExperienceField
    {
        string Title { get; set; }
        string Help { get; set; }
        bool IsRequired { get; set; }
        string ValidationGroup { get; set; }
    }

    interface IExperienceTextField : IExperienceField
    {
        string Value { get; set; }
    }

    interface IExperienceTextInputField : IExperienceTextField
    {
        int MaxLength { get; set; }
    }

    interface IExperienceTextEditorField : IExperienceTextField
    {
        string UploadPath { get; set; }
    }

    interface IExperienceDateField : IExperienceField
    {
        DateTime? Value { get; set; }
    }

    interface IExperienceDropDownField : IExperienceField
    {
        string Value { get; set; }
    }

    interface IExperienceTwoDatesField : IExperienceField
    {
        DateTime? Value1 { get; set; }
        DateTime? Value2 { get; set; }
    }

    interface IExperienceDecimalField : IExperienceField
    {
        decimal? Value { get; set; }
    }

    interface IExperienceMediaField : IExperienceField
    {
        void SetData(QExperience entity);
        IExperienceMediaFieldData GetData(QExperience experience);
    }

    public interface IExperienceMediaFieldData
    {
        string Name { get; }
        string Type { get; }
        Guid? FileIdentifier { get; }
    }
}
