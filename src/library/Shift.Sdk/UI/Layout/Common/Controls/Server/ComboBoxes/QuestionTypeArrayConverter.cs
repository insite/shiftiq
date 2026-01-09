using System;
using System.ComponentModel;
using System.Globalization;

using Shift.Common;
using Shift.Constant;

namespace Shift.Sdk.UI
{
    public class SurveyQuestionTypeArrayConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var strValue = value as string;
            if (string.IsNullOrEmpty(strValue))
                return base.ConvertFrom(context, culture, value);

            var values = strValue.Split(',');

            var result = new SurveyQuestionType[values.Length];

            for (var i = 0; i < values.Length; i++)
                result[i] = values[i].ToEnum<SurveyQuestionType>();

            return result;
        }
    }
}