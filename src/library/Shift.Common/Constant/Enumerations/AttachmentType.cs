namespace Shift.Constant
{
    public enum AttachmentType
    {
        /// <summary>
        /// An attachment for which the type is undefined and/or unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// An attachment for which the file name extension indicates an archive (.zip, .7z).
        /// </summary>
        Archive,

        /// <summary>
        /// An attachment for which the file name extension indicates a document (.doc, .docx, .htm, .html, .pdf, .txt, .xls, xlsx).
        /// </summary>
        Document,

        /// <summary>
        /// An attachment for which the file name extension indicates an image (.bmp, .gif, .jpg, .png).
        /// </summary>
        Image
    }
}