using System;

namespace InSite.Admin.Assets.Contents.Controls.ContentEditor
{
    public delegate void TranslationRequestEventHandler(object sender, TranslationRequestEventArgs args);

    public class TranslationRequestEventArgs : EventArgs
    {
        public Field.ContentType InputContent { get; }

        public string FromLanguage { get; }

        public string[] ToLanguages { get; }

        public TranslationRequestEventArgs(Field.ContentType content, string fromLanguage, string[] toLanguages)
        {
            InputContent = content;
            FromLanguage = fromLanguage;
            ToLanguages = toLanguages;
        }
    }
}