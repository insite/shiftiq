using System;

namespace Shift.Sdk.UI
{
    [Serializable]
    internal enum TranslateEntityType
    {
        EvaluationTitle,
        InstructionStarted,
        InstructionCompleted,
        InstructionClosed,
        QuestionText,
        FieldText,
        OptionText
    }

    [Serializable]
    internal class TranslateEntity
    {
        public int Key { get; private set; }
        public TranslateEntityType Type { get; private set; }

        public static string Serialize(int key, TranslateEntityType type) => $"{key}:{(int)type}";

        public static TranslateEntity Deserialize(string value)
        {
            var parts = value.Split(new[] { ':' });

            return new TranslateEntity
            {
                Key = int.Parse(parts[0]),
                Type = (TranslateEntityType)int.Parse(parts[1])
            };
        }
    }
}