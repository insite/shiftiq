using System;
using System.ComponentModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Shift.Constant;

namespace Shift.Common
{
    [JsonConverter(typeof(QuestionItemTypeJsonConverter))]
    public enum QuestionItemType
    {
        /// <summary>
        /// A single-select radio-button list with one correct answer.
        /// </summary>
        [Description("Multiple Choice"), Icon("check-circle")]
        SingleCorrect,

        /// <summary>
        /// A single-select radio-button list with one correct answer out of two possible answers.
        /// </summary>
        [Description("True or False"), Icon("toggle-on")]
        TrueOrFalse,

        /// <summary>
        /// A multi-select check-box list with the potential for multiple correct answers.
        /// </summary>
        [Description("Multiple Correct"), Icon("check-square")]
        MultipleCorrect,

        /// <summary>
        /// A Boolean table is similar to a MultipleCorrect question, in that it allows for multiple correct answers. 
        /// However, instead of a check-box it displays a radio-button-list (True and False) for each option contained
        /// by the question. Points may be awarded for selecting True or for selecting False beside each item.
        /// </summary>
        [Description("Multiple True/False List"), Icon("th")]
        BooleanTable,

        /// <summary>
        /// An essay question that asks for an open-ended qualitative response (which cannot be marked by the system
        /// and must be marked manually by an instructor or administrator).
        /// </summary>
        [Description("Composed Essay"), Icon("file-lines")]
        ComposedEssay,

        /// <summary>
        /// A matching question is two adjacent lists of related words, phrases, pictures, or symbols. Each item in one 
        /// list is paired with one item in another list.
        /// </summary>
        [Description("Matching"), Icon("exchange")]
        Matching,

        /// <summary>
        /// 
        /// </summary>
        [Description("Likert"), Icon("list-ul")]
        Likert,

        /// <summary>
        /// 
        /// </summary>
        [Description("Hotspot Standard"), Icon("bullseye-pointer")]
        HotspotStandard,

        /// <summary>
        /// 
        /// </summary>
        [Description("Hotspot Image Captcha"), Icon("bullseye-pointer")]
        HotspotImageCaptcha,

        /// <summary>
        /// 
        /// </summary>
        [Description("Hotspot Multiple Choice"), Icon("bullseye-pointer")]
        HotspotMultipleChoice,

        /// <summary>
        /// 
        /// </summary>
        [Description("Hotspot Multiple Answer"), Icon("bullseye-pointer")]
        HotspotMultipleAnswer,

        /// <summary>
        /// 
        /// </summary>
        [Description("Hotspot"), Icon("bullseye-pointer")]
        HotspotCustom,

        /// <summary>
        /// 
        /// </summary>
        [Description("Composed Voice"), Icon("microphone")]
        ComposedVoice,

        /// <summary>
        /// 
        /// </summary>
        [Description("Ordering"), Icon("arrow-down-1-9")]
        Ordering,
    }

    public static class QuestionItemTypeExtensions
    {
        public static bool IsRadioList(this QuestionItemType value)
        {
            return value == QuestionItemType.SingleCorrect || value == QuestionItemType.TrueOrFalse;
        }

        public static bool IsCheckList(this QuestionItemType value)
        {
            return value == QuestionItemType.MultipleCorrect || value == QuestionItemType.BooleanTable;
        }

        public static bool IsComposed(this QuestionItemType value)
        {
            return value == QuestionItemType.ComposedEssay || value == QuestionItemType.ComposedVoice;
        }

        public static bool IsHotspot(this QuestionItemType value)
        {
            return value == QuestionItemType.HotspotStandard
                || value == QuestionItemType.HotspotImageCaptcha
                || value == QuestionItemType.HotspotMultipleChoice
                || value == QuestionItemType.HotspotMultipleAnswer
                || value == QuestionItemType.HotspotCustom;
        }
    }

    internal class QuestionItemTypeJsonConverter : StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var value = reader.Value?.ToString();

                if (value == "Composition" || value == "Composed")
                    return QuestionItemType.ComposedEssay;

                if (value == "Hotspot")
                    return QuestionItemType.HotspotStandard;
            }

            return base.ReadJson(reader, objectType, existingValue, serializer);
        }
    }
}