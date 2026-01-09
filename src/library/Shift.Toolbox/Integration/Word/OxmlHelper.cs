using System;
using System.Linq;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Shift.Toolbox
{
    public static class OxmlHelper
    {
        #region MS Word Styles

        public static class MicrosoftWordStyles
        {
            public static class Paragraph
            {
                public static readonly OxmlStyleInfo Normal = new OxmlStyleInfo("Normal");
                public static readonly OxmlStyleInfo Title = new OxmlStyleInfo("Title");
                public static readonly OxmlStyleInfo Heading1 = new OxmlStyleInfo("Heading1", "Custom Heading 1");
                public static readonly OxmlStyleInfo Heading2 = new OxmlStyleInfo("Heading2", "Custom Heading 2");
                public static readonly OxmlStyleInfo Heading3 = new OxmlStyleInfo("Heading3", "Custom Heading 3");
                public static readonly OxmlStyleInfo Heading4 = new OxmlStyleInfo("Heading4", "Custom Heading 4");
                public static readonly OxmlStyleInfo Heading5 = new OxmlStyleInfo("Heading5", "Custom Heading 5");
                public static readonly OxmlStyleInfo Heading6 = new OxmlStyleInfo("Heading6", "Custom Heading 6");
                public static readonly OxmlStyleInfo Heading7 = new OxmlStyleInfo("Heading7", "Custom Heading 7");
                public static readonly OxmlStyleInfo Heading8 = new OxmlStyleInfo("Heading8", "Custom Heading 8");
                public static readonly OxmlStyleInfo Heading9 = new OxmlStyleInfo("Heading9", "Custom Heading 9");
            }

            public static Styles CreateDefaultStyles()
            {
                return new Styles(@"<w:styles xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006"" xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships"" xmlns:w=""http://schemas.openxmlformats.org/wordprocessingml/2006/main"" xmlns:w14=""http://schemas.microsoft.com/office/word/2010/wordml"" xmlns:w15=""http://schemas.microsoft.com/office/word/2012/wordml"" xmlns:w16cex=""http://schemas.microsoft.com/office/word/2018/wordml/cex"" xmlns:w16cid=""http://schemas.microsoft.com/office/word/2016/wordml/cid"" xmlns:w16=""http://schemas.microsoft.com/office/word/2018/wordml"" xmlns:w16se=""http://schemas.microsoft.com/office/word/2015/wordml/symex"" mc:Ignorable=""w14 w15 w16se w16cid w16 w16cex"">
    <w:docDefaults>
        <w:rPrDefault>
            <w:rPr>
                <w:rFonts w:asciiTheme=""minorHAnsi"" w:eastAsiaTheme=""minorHAnsi"" w:hAnsiTheme=""minorHAnsi"" w:cstheme=""minorBidi""/>
                <w:sz w:val=""22""/>
                <w:szCs w:val=""22""/>
                <w:lang w:val=""en-US"" w:eastAsia=""en-US"" w:bidi=""ar-SA""/>
            </w:rPr>
        </w:rPrDefault>
        <w:pPrDefault>
            <w:pPr>
                <w:spacing w:after=""160"" w:line=""259"" w:lineRule=""auto""/>
            </w:pPr>
        </w:pPrDefault>
    </w:docDefaults>
    <w:latentStyles w:defLockedState=""0"" w:defUIPriority=""99"" w:defSemiHidden=""0"" w:defUnhideWhenUsed=""0"" w:defQFormat=""0"" w:count=""376"">
        <w:lsdException w:name=""Normal"" w:uiPriority=""0"" w:qFormat=""1""/>
        <w:lsdException w:name=""heading 1"" w:uiPriority=""9"" w:qFormat=""1""/>
        <w:lsdException w:name=""heading 2"" w:semiHidden=""1"" w:uiPriority=""9"" w:unhideWhenUsed=""1"" w:qFormat=""1""/>
        <w:lsdException w:name=""heading 3"" w:semiHidden=""1"" w:uiPriority=""9"" w:unhideWhenUsed=""1"" w:qFormat=""1""/>
        <w:lsdException w:name=""heading 4"" w:semiHidden=""1"" w:uiPriority=""9"" w:unhideWhenUsed=""1"" w:qFormat=""1""/>
        <w:lsdException w:name=""heading 5"" w:semiHidden=""1"" w:uiPriority=""9"" w:unhideWhenUsed=""1"" w:qFormat=""1""/>
        <w:lsdException w:name=""heading 6"" w:semiHidden=""1"" w:uiPriority=""9"" w:unhideWhenUsed=""1"" w:qFormat=""1""/>
        <w:lsdException w:name=""heading 7"" w:semiHidden=""1"" w:uiPriority=""9"" w:unhideWhenUsed=""1"" w:qFormat=""1""/>
        <w:lsdException w:name=""heading 8"" w:semiHidden=""1"" w:uiPriority=""9"" w:unhideWhenUsed=""1"" w:qFormat=""1""/>
        <w:lsdException w:name=""heading 9"" w:semiHidden=""1"" w:uiPriority=""9"" w:unhideWhenUsed=""1"" w:qFormat=""1""/>
        <w:lsdException w:name=""index 1"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""index 2"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""index 3"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""index 4"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""index 5"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""index 6"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""index 7"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""index 8"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""index 9"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""toc 1"" w:semiHidden=""1"" w:uiPriority=""39"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""toc 2"" w:semiHidden=""1"" w:uiPriority=""39"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""toc 3"" w:semiHidden=""1"" w:uiPriority=""39"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""toc 4"" w:semiHidden=""1"" w:uiPriority=""39"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""toc 5"" w:semiHidden=""1"" w:uiPriority=""39"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""toc 6"" w:semiHidden=""1"" w:uiPriority=""39"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""toc 7"" w:semiHidden=""1"" w:uiPriority=""39"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""toc 8"" w:semiHidden=""1"" w:uiPriority=""39"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""toc 9"" w:semiHidden=""1"" w:uiPriority=""39"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Normal Indent"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""footnote text"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""annotation text"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""header"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""footer"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""index heading"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""caption"" w:semiHidden=""1"" w:uiPriority=""35"" w:unhideWhenUsed=""1"" w:qFormat=""1""/>
        <w:lsdException w:name=""table of figures"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""envelope address"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""envelope return"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""footnote reference"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""annotation reference"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""line number"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""page number"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""endnote reference"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""endnote text"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""table of authorities"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""macro"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""toa heading"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List Bullet"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List Number"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List 2"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List 3"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List 4"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List 5"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List Bullet 2"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List Bullet 3"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List Bullet 4"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List Bullet 5"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List Number 2"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List Number 3"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List Number 4"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List Number 5"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Title"" w:uiPriority=""10"" w:qFormat=""1""/>
        <w:lsdException w:name=""Closing"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Signature"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Default Paragraph Font"" w:semiHidden=""1"" w:uiPriority=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Body Text"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Body Text Indent"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List Continue"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List Continue 2"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List Continue 3"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List Continue 4"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""List Continue 5"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Message Header"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Subtitle"" w:uiPriority=""11"" w:qFormat=""1""/>
        <w:lsdException w:name=""Salutation"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Date"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Body Text First Indent"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Body Text First Indent 2"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Note Heading"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Body Text 2"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Body Text 3"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Body Text Indent 2"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Body Text Indent 3"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Block Text"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Hyperlink"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""FollowedHyperlink"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Strong"" w:uiPriority=""22"" w:qFormat=""1""/>
        <w:lsdException w:name=""Emphasis"" w:uiPriority=""20"" w:qFormat=""1""/>
        <w:lsdException w:name=""Document Map"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Plain Text"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""E-mail Signature"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""HTML Top of Form"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""HTML Bottom of Form"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Normal (Web)"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""HTML Acronym"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""HTML Address"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""HTML Cite"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""HTML Code"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""HTML Definition"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""HTML Keyboard"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""HTML Preformatted"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""HTML Sample"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""HTML Typewriter"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""HTML Variable"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Normal Table"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""annotation subject"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""No List"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Outline List 1"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Outline List 2"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Outline List 3"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Simple 1"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Simple 2"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Simple 3"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Classic 1"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Classic 2"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Classic 3"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Classic 4"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Colorful 1"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Colorful 2"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Colorful 3"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Columns 1"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Columns 2"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Columns 3"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Columns 4"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Columns 5"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Grid 1"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Grid 2"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Grid 3"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Grid 4"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Grid 5"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Grid 6"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Grid 7"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Grid 8"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table List 1"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table List 2"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table List 3"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table List 4"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table List 5"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table List 6"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table List 7"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table List 8"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table 3D effects 1"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table 3D effects 2"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table 3D effects 3"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Contemporary"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Elegant"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Professional"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Subtle 1"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Subtle 2"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Web 1"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Web 2"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Web 3"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Balloon Text"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Table Grid"" w:uiPriority=""39""/>
        <w:lsdException w:name=""Table Theme"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Placeholder Text"" w:semiHidden=""1""/>
        <w:lsdException w:name=""No Spacing"" w:uiPriority=""1"" w:qFormat=""1""/>
        <w:lsdException w:name=""Light Shading"" w:uiPriority=""60""/>
        <w:lsdException w:name=""Light List"" w:uiPriority=""61""/>
        <w:lsdException w:name=""Light Grid"" w:uiPriority=""62""/>
        <w:lsdException w:name=""Medium Shading 1"" w:uiPriority=""63""/>
        <w:lsdException w:name=""Medium Shading 2"" w:uiPriority=""64""/>
        <w:lsdException w:name=""Medium List 1"" w:uiPriority=""65""/>
        <w:lsdException w:name=""Medium List 2"" w:uiPriority=""66""/>
        <w:lsdException w:name=""Medium Grid 1"" w:uiPriority=""67""/>
        <w:lsdException w:name=""Medium Grid 2"" w:uiPriority=""68""/>
        <w:lsdException w:name=""Medium Grid 3"" w:uiPriority=""69""/>
        <w:lsdException w:name=""Dark List"" w:uiPriority=""70""/>
        <w:lsdException w:name=""Colorful Shading"" w:uiPriority=""71""/>
        <w:lsdException w:name=""Colorful List"" w:uiPriority=""72""/>
        <w:lsdException w:name=""Colorful Grid"" w:uiPriority=""73""/>
        <w:lsdException w:name=""Light Shading Accent 1"" w:uiPriority=""60""/>
        <w:lsdException w:name=""Light List Accent 1"" w:uiPriority=""61""/>
        <w:lsdException w:name=""Light Grid Accent 1"" w:uiPriority=""62""/>
        <w:lsdException w:name=""Medium Shading 1 Accent 1"" w:uiPriority=""63""/>
        <w:lsdException w:name=""Medium Shading 2 Accent 1"" w:uiPriority=""64""/>
        <w:lsdException w:name=""Medium List 1 Accent 1"" w:uiPriority=""65""/>
        <w:lsdException w:name=""Revision"" w:semiHidden=""1""/>
        <w:lsdException w:name=""List Paragraph"" w:uiPriority=""34"" w:qFormat=""1""/>
        <w:lsdException w:name=""Quote"" w:uiPriority=""29"" w:qFormat=""1""/>
        <w:lsdException w:name=""Intense Quote"" w:uiPriority=""30"" w:qFormat=""1""/>
        <w:lsdException w:name=""Medium List 2 Accent 1"" w:uiPriority=""66""/>
        <w:lsdException w:name=""Medium Grid 1 Accent 1"" w:uiPriority=""67""/>
        <w:lsdException w:name=""Medium Grid 2 Accent 1"" w:uiPriority=""68""/>
        <w:lsdException w:name=""Medium Grid 3 Accent 1"" w:uiPriority=""69""/>
        <w:lsdException w:name=""Dark List Accent 1"" w:uiPriority=""70""/>
        <w:lsdException w:name=""Colorful Shading Accent 1"" w:uiPriority=""71""/>
        <w:lsdException w:name=""Colorful List Accent 1"" w:uiPriority=""72""/>
        <w:lsdException w:name=""Colorful Grid Accent 1"" w:uiPriority=""73""/>
        <w:lsdException w:name=""Light Shading Accent 2"" w:uiPriority=""60""/>
        <w:lsdException w:name=""Light List Accent 2"" w:uiPriority=""61""/>
        <w:lsdException w:name=""Light Grid Accent 2"" w:uiPriority=""62""/>
        <w:lsdException w:name=""Medium Shading 1 Accent 2"" w:uiPriority=""63""/>
        <w:lsdException w:name=""Medium Shading 2 Accent 2"" w:uiPriority=""64""/>
        <w:lsdException w:name=""Medium List 1 Accent 2"" w:uiPriority=""65""/>
        <w:lsdException w:name=""Medium List 2 Accent 2"" w:uiPriority=""66""/>
        <w:lsdException w:name=""Medium Grid 1 Accent 2"" w:uiPriority=""67""/>
        <w:lsdException w:name=""Medium Grid 2 Accent 2"" w:uiPriority=""68""/>
        <w:lsdException w:name=""Medium Grid 3 Accent 2"" w:uiPriority=""69""/>
        <w:lsdException w:name=""Dark List Accent 2"" w:uiPriority=""70""/>
        <w:lsdException w:name=""Colorful Shading Accent 2"" w:uiPriority=""71""/>
        <w:lsdException w:name=""Colorful List Accent 2"" w:uiPriority=""72""/>
        <w:lsdException w:name=""Colorful Grid Accent 2"" w:uiPriority=""73""/>
        <w:lsdException w:name=""Light Shading Accent 3"" w:uiPriority=""60""/>
        <w:lsdException w:name=""Light List Accent 3"" w:uiPriority=""61""/>
        <w:lsdException w:name=""Light Grid Accent 3"" w:uiPriority=""62""/>
        <w:lsdException w:name=""Medium Shading 1 Accent 3"" w:uiPriority=""63""/>
        <w:lsdException w:name=""Medium Shading 2 Accent 3"" w:uiPriority=""64""/>
        <w:lsdException w:name=""Medium List 1 Accent 3"" w:uiPriority=""65""/>
        <w:lsdException w:name=""Medium List 2 Accent 3"" w:uiPriority=""66""/>
        <w:lsdException w:name=""Medium Grid 1 Accent 3"" w:uiPriority=""67""/>
        <w:lsdException w:name=""Medium Grid 2 Accent 3"" w:uiPriority=""68""/>
        <w:lsdException w:name=""Medium Grid 3 Accent 3"" w:uiPriority=""69""/>
        <w:lsdException w:name=""Dark List Accent 3"" w:uiPriority=""70""/>
        <w:lsdException w:name=""Colorful Shading Accent 3"" w:uiPriority=""71""/>
        <w:lsdException w:name=""Colorful List Accent 3"" w:uiPriority=""72""/>
        <w:lsdException w:name=""Colorful Grid Accent 3"" w:uiPriority=""73""/>
        <w:lsdException w:name=""Light Shading Accent 4"" w:uiPriority=""60""/>
        <w:lsdException w:name=""Light List Accent 4"" w:uiPriority=""61""/>
        <w:lsdException w:name=""Light Grid Accent 4"" w:uiPriority=""62""/>
        <w:lsdException w:name=""Medium Shading 1 Accent 4"" w:uiPriority=""63""/>
        <w:lsdException w:name=""Medium Shading 2 Accent 4"" w:uiPriority=""64""/>
        <w:lsdException w:name=""Medium List 1 Accent 4"" w:uiPriority=""65""/>
        <w:lsdException w:name=""Medium List 2 Accent 4"" w:uiPriority=""66""/>
        <w:lsdException w:name=""Medium Grid 1 Accent 4"" w:uiPriority=""67""/>
        <w:lsdException w:name=""Medium Grid 2 Accent 4"" w:uiPriority=""68""/>
        <w:lsdException w:name=""Medium Grid 3 Accent 4"" w:uiPriority=""69""/>
        <w:lsdException w:name=""Dark List Accent 4"" w:uiPriority=""70""/>
        <w:lsdException w:name=""Colorful Shading Accent 4"" w:uiPriority=""71""/>
        <w:lsdException w:name=""Colorful List Accent 4"" w:uiPriority=""72""/>
        <w:lsdException w:name=""Colorful Grid Accent 4"" w:uiPriority=""73""/>
        <w:lsdException w:name=""Light Shading Accent 5"" w:uiPriority=""60""/>
        <w:lsdException w:name=""Light List Accent 5"" w:uiPriority=""61""/>
        <w:lsdException w:name=""Light Grid Accent 5"" w:uiPriority=""62""/>
        <w:lsdException w:name=""Medium Shading 1 Accent 5"" w:uiPriority=""63""/>
        <w:lsdException w:name=""Medium Shading 2 Accent 5"" w:uiPriority=""64""/>
        <w:lsdException w:name=""Medium List 1 Accent 5"" w:uiPriority=""65""/>
        <w:lsdException w:name=""Medium List 2 Accent 5"" w:uiPriority=""66""/>
        <w:lsdException w:name=""Medium Grid 1 Accent 5"" w:uiPriority=""67""/>
        <w:lsdException w:name=""Medium Grid 2 Accent 5"" w:uiPriority=""68""/>
        <w:lsdException w:name=""Medium Grid 3 Accent 5"" w:uiPriority=""69""/>
        <w:lsdException w:name=""Dark List Accent 5"" w:uiPriority=""70""/>
        <w:lsdException w:name=""Colorful Shading Accent 5"" w:uiPriority=""71""/>
        <w:lsdException w:name=""Colorful List Accent 5"" w:uiPriority=""72""/>
        <w:lsdException w:name=""Colorful Grid Accent 5"" w:uiPriority=""73""/>
        <w:lsdException w:name=""Light Shading Accent 6"" w:uiPriority=""60""/>
        <w:lsdException w:name=""Light List Accent 6"" w:uiPriority=""61""/>
        <w:lsdException w:name=""Light Grid Accent 6"" w:uiPriority=""62""/>
        <w:lsdException w:name=""Medium Shading 1 Accent 6"" w:uiPriority=""63""/>
        <w:lsdException w:name=""Medium Shading 2 Accent 6"" w:uiPriority=""64""/>
        <w:lsdException w:name=""Medium List 1 Accent 6"" w:uiPriority=""65""/>
        <w:lsdException w:name=""Medium List 2 Accent 6"" w:uiPriority=""66""/>
        <w:lsdException w:name=""Medium Grid 1 Accent 6"" w:uiPriority=""67""/>
        <w:lsdException w:name=""Medium Grid 2 Accent 6"" w:uiPriority=""68""/>
        <w:lsdException w:name=""Medium Grid 3 Accent 6"" w:uiPriority=""69""/>
        <w:lsdException w:name=""Dark List Accent 6"" w:uiPriority=""70""/>
        <w:lsdException w:name=""Colorful Shading Accent 6"" w:uiPriority=""71""/>
        <w:lsdException w:name=""Colorful List Accent 6"" w:uiPriority=""72""/>
        <w:lsdException w:name=""Colorful Grid Accent 6"" w:uiPriority=""73""/>
        <w:lsdException w:name=""Subtle Emphasis"" w:uiPriority=""19"" w:qFormat=""1""/>
        <w:lsdException w:name=""Intense Emphasis"" w:uiPriority=""21"" w:qFormat=""1""/>
        <w:lsdException w:name=""Subtle Reference"" w:uiPriority=""31"" w:qFormat=""1""/>
        <w:lsdException w:name=""Intense Reference"" w:uiPriority=""32"" w:qFormat=""1""/>
        <w:lsdException w:name=""Book Title"" w:uiPriority=""33"" w:qFormat=""1""/>
        <w:lsdException w:name=""Bibliography"" w:semiHidden=""1"" w:uiPriority=""37"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""TOC Heading"" w:semiHidden=""1"" w:uiPriority=""39"" w:unhideWhenUsed=""1"" w:qFormat=""1""/>
        <w:lsdException w:name=""Plain Table 1"" w:uiPriority=""41""/>
        <w:lsdException w:name=""Plain Table 2"" w:uiPriority=""42""/>
        <w:lsdException w:name=""Plain Table 3"" w:uiPriority=""43""/>
        <w:lsdException w:name=""Plain Table 4"" w:uiPriority=""44""/>
        <w:lsdException w:name=""Plain Table 5"" w:uiPriority=""45""/>
        <w:lsdException w:name=""Grid Table Light"" w:uiPriority=""40""/>
        <w:lsdException w:name=""Grid Table 1 Light"" w:uiPriority=""46""/>
        <w:lsdException w:name=""Grid Table 2"" w:uiPriority=""47""/>
        <w:lsdException w:name=""Grid Table 3"" w:uiPriority=""48""/>
        <w:lsdException w:name=""Grid Table 4"" w:uiPriority=""49""/>
        <w:lsdException w:name=""Grid Table 5 Dark"" w:uiPriority=""50""/>
        <w:lsdException w:name=""Grid Table 6 Colorful"" w:uiPriority=""51""/>
        <w:lsdException w:name=""Grid Table 7 Colorful"" w:uiPriority=""52""/>
        <w:lsdException w:name=""Grid Table 1 Light Accent 1"" w:uiPriority=""46""/>
        <w:lsdException w:name=""Grid Table 2 Accent 1"" w:uiPriority=""47""/>
        <w:lsdException w:name=""Grid Table 3 Accent 1"" w:uiPriority=""48""/>
        <w:lsdException w:name=""Grid Table 4 Accent 1"" w:uiPriority=""49""/>
        <w:lsdException w:name=""Grid Table 5 Dark Accent 1"" w:uiPriority=""50""/>
        <w:lsdException w:name=""Grid Table 6 Colorful Accent 1"" w:uiPriority=""51""/>
        <w:lsdException w:name=""Grid Table 7 Colorful Accent 1"" w:uiPriority=""52""/>
        <w:lsdException w:name=""Grid Table 1 Light Accent 2"" w:uiPriority=""46""/>
        <w:lsdException w:name=""Grid Table 2 Accent 2"" w:uiPriority=""47""/>
        <w:lsdException w:name=""Grid Table 3 Accent 2"" w:uiPriority=""48""/>
        <w:lsdException w:name=""Grid Table 4 Accent 2"" w:uiPriority=""49""/>
        <w:lsdException w:name=""Grid Table 5 Dark Accent 2"" w:uiPriority=""50""/>
        <w:lsdException w:name=""Grid Table 6 Colorful Accent 2"" w:uiPriority=""51""/>
        <w:lsdException w:name=""Grid Table 7 Colorful Accent 2"" w:uiPriority=""52""/>
        <w:lsdException w:name=""Grid Table 1 Light Accent 3"" w:uiPriority=""46""/>
        <w:lsdException w:name=""Grid Table 2 Accent 3"" w:uiPriority=""47""/>
        <w:lsdException w:name=""Grid Table 3 Accent 3"" w:uiPriority=""48""/>
        <w:lsdException w:name=""Grid Table 4 Accent 3"" w:uiPriority=""49""/>
        <w:lsdException w:name=""Grid Table 5 Dark Accent 3"" w:uiPriority=""50""/>
        <w:lsdException w:name=""Grid Table 6 Colorful Accent 3"" w:uiPriority=""51""/>
        <w:lsdException w:name=""Grid Table 7 Colorful Accent 3"" w:uiPriority=""52""/>
        <w:lsdException w:name=""Grid Table 1 Light Accent 4"" w:uiPriority=""46""/>
        <w:lsdException w:name=""Grid Table 2 Accent 4"" w:uiPriority=""47""/>
        <w:lsdException w:name=""Grid Table 3 Accent 4"" w:uiPriority=""48""/>
        <w:lsdException w:name=""Grid Table 4 Accent 4"" w:uiPriority=""49""/>
        <w:lsdException w:name=""Grid Table 5 Dark Accent 4"" w:uiPriority=""50""/>
        <w:lsdException w:name=""Grid Table 6 Colorful Accent 4"" w:uiPriority=""51""/>
        <w:lsdException w:name=""Grid Table 7 Colorful Accent 4"" w:uiPriority=""52""/>
        <w:lsdException w:name=""Grid Table 1 Light Accent 5"" w:uiPriority=""46""/>
        <w:lsdException w:name=""Grid Table 2 Accent 5"" w:uiPriority=""47""/>
        <w:lsdException w:name=""Grid Table 3 Accent 5"" w:uiPriority=""48""/>
        <w:lsdException w:name=""Grid Table 4 Accent 5"" w:uiPriority=""49""/>
        <w:lsdException w:name=""Grid Table 5 Dark Accent 5"" w:uiPriority=""50""/>
        <w:lsdException w:name=""Grid Table 6 Colorful Accent 5"" w:uiPriority=""51""/>
        <w:lsdException w:name=""Grid Table 7 Colorful Accent 5"" w:uiPriority=""52""/>
        <w:lsdException w:name=""Grid Table 1 Light Accent 6"" w:uiPriority=""46""/>
        <w:lsdException w:name=""Grid Table 2 Accent 6"" w:uiPriority=""47""/>
        <w:lsdException w:name=""Grid Table 3 Accent 6"" w:uiPriority=""48""/>
        <w:lsdException w:name=""Grid Table 4 Accent 6"" w:uiPriority=""49""/>
        <w:lsdException w:name=""Grid Table 5 Dark Accent 6"" w:uiPriority=""50""/>
        <w:lsdException w:name=""Grid Table 6 Colorful Accent 6"" w:uiPriority=""51""/>
        <w:lsdException w:name=""Grid Table 7 Colorful Accent 6"" w:uiPriority=""52""/>
        <w:lsdException w:name=""List Table 1 Light"" w:uiPriority=""46""/>
        <w:lsdException w:name=""List Table 2"" w:uiPriority=""47""/>
        <w:lsdException w:name=""List Table 3"" w:uiPriority=""48""/>
        <w:lsdException w:name=""List Table 4"" w:uiPriority=""49""/>
        <w:lsdException w:name=""List Table 5 Dark"" w:uiPriority=""50""/>
        <w:lsdException w:name=""List Table 6 Colorful"" w:uiPriority=""51""/>
        <w:lsdException w:name=""List Table 7 Colorful"" w:uiPriority=""52""/>
        <w:lsdException w:name=""List Table 1 Light Accent 1"" w:uiPriority=""46""/>
        <w:lsdException w:name=""List Table 2 Accent 1"" w:uiPriority=""47""/>
        <w:lsdException w:name=""List Table 3 Accent 1"" w:uiPriority=""48""/>
        <w:lsdException w:name=""List Table 4 Accent 1"" w:uiPriority=""49""/>
        <w:lsdException w:name=""List Table 5 Dark Accent 1"" w:uiPriority=""50""/>
        <w:lsdException w:name=""List Table 6 Colorful Accent 1"" w:uiPriority=""51""/>
        <w:lsdException w:name=""List Table 7 Colorful Accent 1"" w:uiPriority=""52""/>
        <w:lsdException w:name=""List Table 1 Light Accent 2"" w:uiPriority=""46""/>
        <w:lsdException w:name=""List Table 2 Accent 2"" w:uiPriority=""47""/>
        <w:lsdException w:name=""List Table 3 Accent 2"" w:uiPriority=""48""/>
        <w:lsdException w:name=""List Table 4 Accent 2"" w:uiPriority=""49""/>
        <w:lsdException w:name=""List Table 5 Dark Accent 2"" w:uiPriority=""50""/>
        <w:lsdException w:name=""List Table 6 Colorful Accent 2"" w:uiPriority=""51""/>
        <w:lsdException w:name=""List Table 7 Colorful Accent 2"" w:uiPriority=""52""/>
        <w:lsdException w:name=""List Table 1 Light Accent 3"" w:uiPriority=""46""/>
        <w:lsdException w:name=""List Table 2 Accent 3"" w:uiPriority=""47""/>
        <w:lsdException w:name=""List Table 3 Accent 3"" w:uiPriority=""48""/>
        <w:lsdException w:name=""List Table 4 Accent 3"" w:uiPriority=""49""/>
        <w:lsdException w:name=""List Table 5 Dark Accent 3"" w:uiPriority=""50""/>
        <w:lsdException w:name=""List Table 6 Colorful Accent 3"" w:uiPriority=""51""/>
        <w:lsdException w:name=""List Table 7 Colorful Accent 3"" w:uiPriority=""52""/>
        <w:lsdException w:name=""List Table 1 Light Accent 4"" w:uiPriority=""46""/>
        <w:lsdException w:name=""List Table 2 Accent 4"" w:uiPriority=""47""/>
        <w:lsdException w:name=""List Table 3 Accent 4"" w:uiPriority=""48""/>
        <w:lsdException w:name=""List Table 4 Accent 4"" w:uiPriority=""49""/>
        <w:lsdException w:name=""List Table 5 Dark Accent 4"" w:uiPriority=""50""/>
        <w:lsdException w:name=""List Table 6 Colorful Accent 4"" w:uiPriority=""51""/>
        <w:lsdException w:name=""List Table 7 Colorful Accent 4"" w:uiPriority=""52""/>
        <w:lsdException w:name=""List Table 1 Light Accent 5"" w:uiPriority=""46""/>
        <w:lsdException w:name=""List Table 2 Accent 5"" w:uiPriority=""47""/>
        <w:lsdException w:name=""List Table 3 Accent 5"" w:uiPriority=""48""/>
        <w:lsdException w:name=""List Table 4 Accent 5"" w:uiPriority=""49""/>
        <w:lsdException w:name=""List Table 5 Dark Accent 5"" w:uiPriority=""50""/>
        <w:lsdException w:name=""List Table 6 Colorful Accent 5"" w:uiPriority=""51""/>
        <w:lsdException w:name=""List Table 7 Colorful Accent 5"" w:uiPriority=""52""/>
        <w:lsdException w:name=""List Table 1 Light Accent 6"" w:uiPriority=""46""/>
        <w:lsdException w:name=""List Table 2 Accent 6"" w:uiPriority=""47""/>
        <w:lsdException w:name=""List Table 3 Accent 6"" w:uiPriority=""48""/>
        <w:lsdException w:name=""List Table 4 Accent 6"" w:uiPriority=""49""/>
        <w:lsdException w:name=""List Table 5 Dark Accent 6"" w:uiPriority=""50""/>
        <w:lsdException w:name=""List Table 6 Colorful Accent 6"" w:uiPriority=""51""/>
        <w:lsdException w:name=""List Table 7 Colorful Accent 6"" w:uiPriority=""52""/>
        <w:lsdException w:name=""Mention"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Smart Hyperlink"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Hashtag"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Unresolved Mention"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
        <w:lsdException w:name=""Smart Link"" w:semiHidden=""1"" w:unhideWhenUsed=""1""/>
    </w:latentStyles>
    <w:style w:type=""paragraph"" w:default=""1"" w:styleId=""Normal"">
        <w:name w:val=""Normal""/>
        <w:qFormat/>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""Heading1"">
        <w:name w:val=""heading 1""/>
        <w:basedOn w:val=""Normal""/>
        <w:next w:val=""Normal""/>
        <w:link w:val=""Heading1Char""/>
        <w:uiPriority w:val=""9""/>
        <w:qFormat/>
        <w:pPr>
            <w:keepNext/>
            <w:keepLines/>
            <w:spacing w:before=""240"" w:after=""0""/>
            <w:outlineLvl w:val=""0""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:color w:val=""2F5496"" w:themeColor=""accent1"" w:themeShade=""BF""/>
            <w:sz w:val=""32""/>
            <w:szCs w:val=""32""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""Heading2"">
        <w:name w:val=""heading 2""/>
        <w:basedOn w:val=""Normal""/>
        <w:next w:val=""Normal""/>
        <w:link w:val=""Heading2Char""/>
        <w:uiPriority w:val=""9""/>
        <w:unhideWhenUsed/>
        <w:qFormat/>
        <w:pPr>
            <w:keepNext/>
            <w:keepLines/>
            <w:spacing w:before=""40"" w:after=""0""/>
            <w:outlineLvl w:val=""1""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:color w:val=""2F5496"" w:themeColor=""accent1"" w:themeShade=""BF""/>
            <w:sz w:val=""26""/>
            <w:szCs w:val=""26""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""Heading3"">
        <w:name w:val=""heading 3""/>
        <w:basedOn w:val=""Normal""/>
        <w:next w:val=""Normal""/>
        <w:link w:val=""Heading3Char""/>
        <w:uiPriority w:val=""9""/>
        <w:unhideWhenUsed/>
        <w:qFormat/>
        <w:pPr>
            <w:keepNext/>
            <w:keepLines/>
            <w:spacing w:before=""40"" w:after=""0""/>
            <w:outlineLvl w:val=""2""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:color w:val=""1F3763"" w:themeColor=""accent1"" w:themeShade=""7F""/>
            <w:sz w:val=""24""/>
            <w:szCs w:val=""24""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""Heading4"">
        <w:name w:val=""heading 4""/>
        <w:basedOn w:val=""Normal""/>
        <w:next w:val=""Normal""/>
        <w:link w:val=""Heading4Char""/>
        <w:uiPriority w:val=""9""/>
        <w:unhideWhenUsed/>
        <w:qFormat/>
        <w:pPr>
            <w:keepNext/>
            <w:keepLines/>
            <w:spacing w:before=""40"" w:after=""0""/>
            <w:outlineLvl w:val=""3""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:i/>
            <w:iCs/>
            <w:color w:val=""2F5496"" w:themeColor=""accent1"" w:themeShade=""BF""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""Heading5"">
        <w:name w:val=""heading 5""/>
        <w:basedOn w:val=""Normal""/>
        <w:next w:val=""Normal""/>
        <w:link w:val=""Heading5Char""/>
        <w:uiPriority w:val=""9""/>
        <w:unhideWhenUsed/>
        <w:qFormat/>
        <w:pPr>
            <w:keepNext/>
            <w:keepLines/>
            <w:spacing w:before=""40"" w:after=""0""/>
            <w:outlineLvl w:val=""4""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:color w:val=""2F5496"" w:themeColor=""accent1"" w:themeShade=""BF""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""Heading6"">
        <w:name w:val=""heading 6""/>
        <w:basedOn w:val=""Normal""/>
        <w:next w:val=""Normal""/>
        <w:link w:val=""Heading6Char""/>
        <w:uiPriority w:val=""9""/>
        <w:unhideWhenUsed/>
        <w:qFormat/>
        <w:pPr>
            <w:keepNext/>
            <w:keepLines/>
            <w:spacing w:before=""40"" w:after=""0""/>
            <w:outlineLvl w:val=""5""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:color w:val=""1F3763"" w:themeColor=""accent1"" w:themeShade=""7F""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""Heading7"">
        <w:name w:val=""heading 7""/>
        <w:basedOn w:val=""Normal""/>
        <w:next w:val=""Normal""/>
        <w:link w:val=""Heading7Char""/>
        <w:uiPriority w:val=""9""/>
        <w:unhideWhenUsed/>
        <w:qFormat/>
        <w:pPr>
            <w:keepNext/>
            <w:keepLines/>
            <w:spacing w:before=""40"" w:after=""0""/>
            <w:outlineLvl w:val=""6""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:i/>
            <w:iCs/>
            <w:color w:val=""1F3763"" w:themeColor=""accent1"" w:themeShade=""7F""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""Heading8"">
        <w:name w:val=""heading 8""/>
        <w:basedOn w:val=""Normal""/>
        <w:next w:val=""Normal""/>
        <w:link w:val=""Heading8Char""/>
        <w:uiPriority w:val=""9""/>
        <w:unhideWhenUsed/>
        <w:qFormat/>
        <w:pPr>
            <w:keepNext/>
            <w:keepLines/>
            <w:spacing w:before=""40"" w:after=""0""/>
            <w:outlineLvl w:val=""7""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:color w:val=""272727"" w:themeColor=""text1"" w:themeTint=""D8""/>
            <w:sz w:val=""21""/>
            <w:szCs w:val=""21""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""Heading9"">
        <w:name w:val=""heading 9""/>
        <w:basedOn w:val=""Normal""/>
        <w:next w:val=""Normal""/>
        <w:link w:val=""Heading9Char""/>
        <w:uiPriority w:val=""9""/>
        <w:unhideWhenUsed/>
        <w:qFormat/>
        <w:pPr>
            <w:keepNext/>
            <w:keepLines/>
            <w:spacing w:before=""40"" w:after=""0""/>
            <w:outlineLvl w:val=""8""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:i/>
            <w:iCs/>
            <w:color w:val=""272727"" w:themeColor=""text1"" w:themeTint=""D8""/>
            <w:sz w:val=""21""/>
            <w:szCs w:val=""21""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:default=""1"" w:styleId=""DefaultParagraphFont"">
        <w:name w:val=""Default Paragraph Font""/>
        <w:uiPriority w:val=""1""/>
        <w:semiHidden/>
        <w:unhideWhenUsed/>
    </w:style>
    <w:style w:type=""table"" w:default=""1"" w:styleId=""TableNormal"">
        <w:name w:val=""Normal Table""/>
        <w:uiPriority w:val=""99""/>
        <w:semiHidden/>
        <w:unhideWhenUsed/>
        <w:tblPr>
            <w:tblInd w:w=""0"" w:type=""dxa""/>
            <w:tblCellMar>
                <w:top w:w=""0"" w:type=""dxa""/>
                <w:left w:w=""108"" w:type=""dxa""/>
                <w:bottom w:w=""0"" w:type=""dxa""/>
                <w:right w:w=""108"" w:type=""dxa""/>
            </w:tblCellMar>
        </w:tblPr>
    </w:style>
    <w:style w:type=""numbering"" w:default=""1"" w:styleId=""NoList"">
        <w:name w:val=""No List""/>
        <w:uiPriority w:val=""99""/>
        <w:semiHidden/>
        <w:unhideWhenUsed/>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""NoSpacing"">
        <w:name w:val=""No Spacing""/>
        <w:uiPriority w:val=""1""/>
        <w:qFormat/>
        <w:pPr>
            <w:spacing w:after=""0"" w:line=""240"" w:lineRule=""auto""/>
        </w:pPr>
    </w:style>
    <w:style w:type=""character"" w:customStyle=""1"" w:styleId=""Heading1Char"">
        <w:name w:val=""Heading 1 Char""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:link w:val=""Heading1""/>
        <w:uiPriority w:val=""9""/>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:color w:val=""2F5496"" w:themeColor=""accent1"" w:themeShade=""BF""/>
            <w:sz w:val=""32""/>
            <w:szCs w:val=""32""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:customStyle=""1"" w:styleId=""Heading2Char"">
        <w:name w:val=""Heading 2 Char""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:link w:val=""Heading2""/>
        <w:uiPriority w:val=""9""/>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:color w:val=""2F5496"" w:themeColor=""accent1"" w:themeShade=""BF""/>
            <w:sz w:val=""26""/>
            <w:szCs w:val=""26""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:customStyle=""1"" w:styleId=""Heading3Char"">
        <w:name w:val=""Heading 3 Char""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:link w:val=""Heading3""/>
        <w:uiPriority w:val=""9""/>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:color w:val=""1F3763"" w:themeColor=""accent1"" w:themeShade=""7F""/>
            <w:sz w:val=""24""/>
            <w:szCs w:val=""24""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:customStyle=""1"" w:styleId=""Heading4Char"">
        <w:name w:val=""Heading 4 Char""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:link w:val=""Heading4""/>
        <w:uiPriority w:val=""9""/>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:i/>
            <w:iCs/>
            <w:color w:val=""2F5496"" w:themeColor=""accent1"" w:themeShade=""BF""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:customStyle=""1"" w:styleId=""Heading5Char"">
        <w:name w:val=""Heading 5 Char""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:link w:val=""Heading5""/>
        <w:uiPriority w:val=""9""/>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:color w:val=""2F5496"" w:themeColor=""accent1"" w:themeShade=""BF""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:customStyle=""1"" w:styleId=""Heading6Char"">
        <w:name w:val=""Heading 6 Char""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:link w:val=""Heading6""/>
        <w:uiPriority w:val=""9""/>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:color w:val=""1F3763"" w:themeColor=""accent1"" w:themeShade=""7F""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:customStyle=""1"" w:styleId=""Heading7Char"">
        <w:name w:val=""Heading 7 Char""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:link w:val=""Heading7""/>
        <w:uiPriority w:val=""9""/>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:i/>
            <w:iCs/>
            <w:color w:val=""1F3763"" w:themeColor=""accent1"" w:themeShade=""7F""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:customStyle=""1"" w:styleId=""Heading8Char"">
        <w:name w:val=""Heading 8 Char""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:link w:val=""Heading8""/>
        <w:uiPriority w:val=""9""/>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:color w:val=""272727"" w:themeColor=""text1"" w:themeTint=""D8""/>
            <w:sz w:val=""21""/>
            <w:szCs w:val=""21""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:customStyle=""1"" w:styleId=""Heading9Char"">
        <w:name w:val=""Heading 9 Char""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:link w:val=""Heading9""/>
        <w:uiPriority w:val=""9""/>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:i/>
            <w:iCs/>
            <w:color w:val=""272727"" w:themeColor=""text1"" w:themeTint=""D8""/>
            <w:sz w:val=""21""/>
            <w:szCs w:val=""21""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:customStyle=""1"" w:styleId=""TitleChar"">
        <w:name w:val=""Title Char""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:link w:val=""Title""/>
        <w:uiPriority w:val=""10""/>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:spacing w:val=""-10""/>
            <w:kern w:val=""28""/>
            <w:sz w:val=""56""/>
            <w:szCs w:val=""56""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""Title"">
        <w:name w:val=""Title""/>
        <w:basedOn w:val=""Normal""/>
        <w:next w:val=""Normal""/>
        <w:link w:val=""TitleChar""/>
        <w:uiPriority w:val=""10""/>
        <w:qFormat/>
        <w:pPr>
            <w:spacing w:after=""0"" w:line=""240"" w:lineRule=""auto""/>
            <w:contextualSpacing/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:asciiTheme=""majorHAnsi"" w:eastAsiaTheme=""majorEastAsia"" w:hAnsiTheme=""majorHAnsi"" w:cstheme=""majorBidi""/>
            <w:spacing w:val=""-10""/>
            <w:kern w:val=""28""/>
            <w:sz w:val=""56""/>
            <w:szCs w:val=""56""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:customStyle=""1"" w:styleId=""SubtitleChar"">
        <w:name w:val=""Subtitle Char""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:link w:val=""Subtitle""/>
        <w:uiPriority w:val=""11""/>
        <w:rPr>
            <w:rFonts w:eastAsiaTheme=""minorEastAsia""/>
            <w:color w:val=""5A5A5A"" w:themeColor=""text1"" w:themeTint=""A5""/>
            <w:spacing w:val=""15""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""Subtitle"">
        <w:name w:val=""Subtitle""/>
        <w:basedOn w:val=""Normal""/>
        <w:next w:val=""Normal""/>
        <w:link w:val=""SubtitleChar""/>
        <w:uiPriority w:val=""11""/>
        <w:qFormat/>
        <w:pPr>
            <w:numPr>
                <w:ilvl w:val=""1""/>
            </w:numPr>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:eastAsiaTheme=""minorEastAsia""/>
            <w:color w:val=""5A5A5A"" w:themeColor=""text1"" w:themeTint=""A5""/>
            <w:spacing w:val=""15""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:styleId=""SubtleEmphasis"">
        <w:name w:val=""Subtle Emphasis""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:uiPriority w:val=""19""/>
        <w:qFormat/>
        <w:rPr>
            <w:i/>
            <w:iCs/>
            <w:color w:val=""404040"" w:themeColor=""text1"" w:themeTint=""BF""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:styleId=""Emphasis"">
        <w:name w:val=""Emphasis""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:uiPriority w:val=""20""/>
        <w:qFormat/>
        <w:rPr>
            <w:i/>
            <w:iCs/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:styleId=""IntenseEmphasis"">
        <w:name w:val=""Intense Emphasis""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:uiPriority w:val=""21""/>
        <w:qFormat/>
        <w:rPr>
            <w:i/>
            <w:iCs/>
            <w:color w:val=""4472C4"" w:themeColor=""accent1""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:styleId=""Strong"">
        <w:name w:val=""Strong""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:uiPriority w:val=""22""/>
        <w:qFormat/>
        <w:rPr>
            <w:b/>
            <w:bCs/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:customStyle=""1"" w:styleId=""QuoteChar"">
        <w:name w:val=""Quote Char""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:link w:val=""Quote""/>
        <w:uiPriority w:val=""29""/>
        <w:rPr>
            <w:i/>
            <w:iCs/>
            <w:color w:val=""404040"" w:themeColor=""text1"" w:themeTint=""BF""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""Quote"">
        <w:name w:val=""Quote""/>
        <w:basedOn w:val=""Normal""/>
        <w:next w:val=""Normal""/>
        <w:link w:val=""QuoteChar""/>
        <w:uiPriority w:val=""29""/>
        <w:qFormat/>
        <w:pPr>
            <w:spacing w:before=""200""/>
            <w:ind w:left=""864"" w:right=""864""/>
            <w:jc w:val=""center""/>
        </w:pPr>
        <w:rPr>
            <w:i/>
            <w:iCs/>
            <w:color w:val=""404040"" w:themeColor=""text1"" w:themeTint=""BF""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:customStyle=""1"" w:styleId=""IntenseQuoteChar"">
        <w:name w:val=""Intense Quote Char""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:link w:val=""IntenseQuote""/>
        <w:uiPriority w:val=""30""/>
        <w:rPr>
            <w:i/>
            <w:iCs/>
            <w:color w:val=""4472C4"" w:themeColor=""accent1""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""IntenseQuote"">
        <w:name w:val=""Intense Quote""/>
        <w:basedOn w:val=""Normal""/>
        <w:next w:val=""Normal""/>
        <w:link w:val=""IntenseQuoteChar""/>
        <w:uiPriority w:val=""30""/>
        <w:qFormat/>
        <w:pPr>
            <w:pBdr>
                <w:top w:val=""single"" w:sz=""4"" w:space=""10"" w:color=""4472C4"" w:themeColor=""accent1""/>
                <w:bottom w:val=""single"" w:sz=""4"" w:space=""10"" w:color=""4472C4"" w:themeColor=""accent1""/>
            </w:pBdr>
            <w:spacing w:before=""360"" w:after=""360""/>
            <w:ind w:left=""864"" w:right=""864""/>
            <w:jc w:val=""center""/>
        </w:pPr>
        <w:rPr>
            <w:i/>
            <w:iCs/>
            <w:color w:val=""4472C4"" w:themeColor=""accent1""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:styleId=""SubtleReference"">
        <w:name w:val=""Subtle Reference""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:uiPriority w:val=""31""/>
        <w:qFormat/>
        <w:rPr>
            <w:smallCaps/>
            <w:color w:val=""5A5A5A"" w:themeColor=""text1"" w:themeTint=""A5""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:styleId=""IntenseReference"">
        <w:name w:val=""Intense Reference""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:uiPriority w:val=""32""/>
        <w:qFormat/>
        <w:rPr>
            <w:b/>
            <w:bCs/>
            <w:smallCaps/>
            <w:color w:val=""4472C4"" w:themeColor=""accent1""/>
            <w:spacing w:val=""5""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""character"" w:styleId=""BookTitle"">
        <w:name w:val=""Book Title""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:uiPriority w:val=""33""/>
        <w:qFormat/>
        <w:rPr>
            <w:b/>
            <w:bCs/>
            <w:i/>
            <w:iCs/>
            <w:spacing w:val=""5""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""ListParagraph"">
        <w:name w:val=""List Paragraph""/>
        <w:basedOn w:val=""Normal""/>
        <w:uiPriority w:val=""34""/>
        <w:qFormat/>
        <w:pPr>
            <w:ind w:left=""720""/>
            <w:contextualSpacing/>
        </w:pPr>
    </w:style>
    <w:style w:type=""table"" w:styleId=""TableGrid"">
        <w:name w:val=""Table Grid""/>
        <w:basedOn w:val=""TableNormal""/>
        <w:uiPriority w:val=""59""/>
        <w:rsid w:val=""00FB4123""/>
        <w:pPr>
            <w:spacing w:after=""0"" w:line=""240"" w:lineRule=""auto""/>
        </w:pPr>
        <w:tblPr>
            <w:tblBorders>
                <w:top w:val=""single"" w:sz=""4"" w:space=""0"" w:color=""000000"" w:themeColor=""text1""/>
                <w:left w:val=""single"" w:sz=""4"" w:space=""0"" w:color=""000000"" w:themeColor=""text1""/>
                <w:bottom w:val=""single"" w:sz=""4"" w:space=""0"" w:color=""000000"" w:themeColor=""text1""/>
                <w:right w:val=""single"" w:sz=""4"" w:space=""0"" w:color=""000000"" w:themeColor=""text1""/>
                <w:insideH w:val=""single"" w:sz=""4"" w:space=""0"" w:color=""000000"" w:themeColor=""text1""/>
                <w:insideV w:val=""single"" w:sz=""4"" w:space=""0"" w:color=""000000"" w:themeColor=""text1""/>
            </w:tblBorders>
        </w:tblPr>
    </w:style>
    <w:style w:type=""character"" w:customStyle=""1"" w:styleId=""HeaderChar"">
        <w:name w:val=""Header Char""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:link w:val=""Header""/>
        <w:uiPriority w:val=""99""/>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""Header"">
        <w:name w:val=""header""/>
        <w:basedOn w:val=""Normal""/>
        <w:link w:val=""HeaderChar""/>
        <w:uiPriority w:val=""99""/>
        <w:unhideWhenUsed/>
        <w:pPr>
            <w:tabs>
                <w:tab w:val=""center"" w:pos=""4680""/>
                <w:tab w:val=""right"" w:pos=""9360""/>
            </w:tabs>
            <w:spacing w:after=""0"" w:line=""240"" w:lineRule=""auto""/>
        </w:pPr>
    </w:style>
    <w:style w:type=""character"" w:customStyle=""1"" w:styleId=""FooterChar"">
        <w:name w:val=""Footer Char""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:link w:val=""Footer""/>
        <w:uiPriority w:val=""99""/>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""Footer"">
        <w:name w:val=""footer""/>
        <w:basedOn w:val=""Normal""/>
        <w:link w:val=""FooterChar""/>
        <w:uiPriority w:val=""99""/>
        <w:unhideWhenUsed/>
        <w:pPr>
            <w:tabs>
                <w:tab w:val=""center"" w:pos=""4680""/>
                <w:tab w:val=""right"" w:pos=""9360""/>
            </w:tabs>
            <w:spacing w:after=""0"" w:line=""240"" w:lineRule=""auto""/>
        </w:pPr>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""TOC1"">
        <w:name w:val=""toc 1""/>
        <w:basedOn w:val=""Normal""/>
        <w:next w:val=""Normal""/>
        <w:autoRedefine/>
        <w:uiPriority w:val=""39""/>
        <w:unhideWhenUsed/>
        <w:rsid w:val=""00D44326""/>
        <w:pPr>
            <w:spacing w:after=""100""/>
        </w:pPr>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""TOC2"">
        <w:name w:val=""toc 2""/>
        <w:basedOn w:val=""Normal""/>
        <w:next w:val=""Normal""/>
        <w:autoRedefine/>
        <w:uiPriority w:val=""39""/>
        <w:unhideWhenUsed/>
        <w:rsid w:val=""00D44326""/>
        <w:pPr>
            <w:spacing w:after=""100""/>
            <w:ind w:left=""220""/>
        </w:pPr>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""TOC3"">
        <w:name w:val=""toc 3""/>
        <w:basedOn w:val=""Normal""/>
        <w:next w:val=""Normal""/>
        <w:autoRedefine/>
        <w:uiPriority w:val=""39""/>
        <w:unhideWhenUsed/>
        <w:rsid w:val=""00D44326""/>
        <w:pPr>
            <w:spacing w:after=""100""/>
            <w:ind w:left=""440""/>
        </w:pPr>
    </w:style>
    <w:style w:type=""character"" w:styleId=""Hyperlink"">
        <w:name w:val=""Hyperlink""/>
        <w:basedOn w:val=""DefaultParagraphFont""/>
        <w:uiPriority w:val=""99""/>
        <w:unhideWhenUsed/>
        <w:rsid w:val=""00D44326""/>
        <w:rPr>
            <w:color w:val=""0563C1"" w:themeColor=""hyperlink""/>
            <w:u w:val=""single""/>
        </w:rPr>
    </w:style>
    <w:style w:type=""paragraph"" w:styleId=""TOCHeading"">
        <w:name w:val=""TOC Heading""/>
        <w:basedOn w:val=""Heading1""/>
        <w:next w:val=""Normal""/>
        <w:uiPriority w:val=""39""/>
        <w:unhideWhenUsed/>
        <w:qFormat/>
        <w:rsid w:val=""00A824B2""/>
        <w:pPr>
            <w:outlineLvl w:val=""9""/>
        </w:pPr>
    </w:style>
</w:styles>");
            }

            public static Theme CreateDefaultTheme()
            {
                return new Theme(@"<a:theme xmlns:a=""http://schemas.openxmlformats.org/drawingml/2006/main"" name=""Office Theme"">
    <a:themeElements>
        <a:clrScheme name=""Office"">
            <a:dk1>
                <a:sysClr val=""windowText"" lastClr=""000000""/>
            </a:dk1>
            <a:lt1>
                <a:sysClr val=""window"" lastClr=""FFFFFF""/>
            </a:lt1>
            <a:dk2>
                <a:srgbClr val=""44546A""/>
            </a:dk2>
            <a:lt2>
                <a:srgbClr val=""E7E6E6""/>
            </a:lt2>
            <a:accent1>
                <a:srgbClr val=""4472C4""/>
            </a:accent1>
            <a:accent2>
                <a:srgbClr val=""ED7D31""/>
            </a:accent2>
            <a:accent3>
                <a:srgbClr val=""A5A5A5""/>
            </a:accent3>
            <a:accent4>
                <a:srgbClr val=""FFC000""/>
            </a:accent4>
            <a:accent5>
                <a:srgbClr val=""5B9BD5""/>
            </a:accent5>
            <a:accent6>
                <a:srgbClr val=""70AD47""/>
            </a:accent6>
            <a:hlink>
                <a:srgbClr val=""0563C1""/>
            </a:hlink>
            <a:folHlink>
                <a:srgbClr val=""954F72""/>
            </a:folHlink>
        </a:clrScheme>
        <a:fontScheme name=""Office"">
            <a:majorFont>
                <a:latin typeface=""Calibri Light"" panose=""020F0302020204030204""/>
                <a:ea typeface=""""/>
                <a:cs typeface=""""/>
                <a:font script=""Jpan"" typeface=""游ゴシック Light""/>
                <a:font script=""Hang"" typeface=""맑은 고딕""/>
                <a:font script=""Hans"" typeface=""等线 Light""/>
                <a:font script=""Hant"" typeface=""新細明體""/>
                <a:font script=""Arab"" typeface=""Times New Roman""/>
                <a:font script=""Hebr"" typeface=""Times New Roman""/>
                <a:font script=""Thai"" typeface=""Angsana New""/>
                <a:font script=""Ethi"" typeface=""Nyala""/>
                <a:font script=""Beng"" typeface=""Vrinda""/>
                <a:font script=""Gujr"" typeface=""Shruti""/>
                <a:font script=""Khmr"" typeface=""MoolBoran""/>
                <a:font script=""Knda"" typeface=""Tunga""/>
                <a:font script=""Guru"" typeface=""Raavi""/>
                <a:font script=""Cans"" typeface=""Euphemia""/>
                <a:font script=""Cher"" typeface=""Plantagenet Cherokee""/>
                <a:font script=""Yiii"" typeface=""Microsoft Yi Baiti""/>
                <a:font script=""Tibt"" typeface=""Microsoft Himalaya""/>
                <a:font script=""Thaa"" typeface=""MV Boli""/>
                <a:font script=""Deva"" typeface=""Mangal""/>
                <a:font script=""Telu"" typeface=""Gautami""/>
                <a:font script=""Taml"" typeface=""Latha""/>
                <a:font script=""Syrc"" typeface=""Estrangelo Edessa""/>
                <a:font script=""Orya"" typeface=""Kalinga""/>
                <a:font script=""Mlym"" typeface=""Kartika""/>
                <a:font script=""Laoo"" typeface=""DokChampa""/>
                <a:font script=""Sinh"" typeface=""Iskoola Pota""/>
                <a:font script=""Mong"" typeface=""Mongolian Baiti""/>
                <a:font script=""Viet"" typeface=""Times New Roman""/>
                <a:font script=""Uigh"" typeface=""Microsoft Uighur""/>
                <a:font script=""Geor"" typeface=""Sylfaen""/>
                <a:font script=""Armn"" typeface=""Arial""/>
                <a:font script=""Bugi"" typeface=""Leelawadee UI""/>
                <a:font script=""Bopo"" typeface=""Microsoft JhengHei""/>
                <a:font script=""Java"" typeface=""Javanese Text""/>
                <a:font script=""Lisu"" typeface=""Segoe UI""/>
                <a:font script=""Mymr"" typeface=""Myanmar Text""/>
                <a:font script=""Nkoo"" typeface=""Ebrima""/>
                <a:font script=""Olck"" typeface=""Nirmala UI""/>
                <a:font script=""Osma"" typeface=""Ebrima""/>
                <a:font script=""Phag"" typeface=""Phagspa""/>
                <a:font script=""Syrn"" typeface=""Estrangelo Edessa""/>
                <a:font script=""Syrj"" typeface=""Estrangelo Edessa""/>
                <a:font script=""Syre"" typeface=""Estrangelo Edessa""/>
                <a:font script=""Sora"" typeface=""Nirmala UI""/>
                <a:font script=""Tale"" typeface=""Microsoft Tai Le""/>
                <a:font script=""Talu"" typeface=""Microsoft New Tai Lue""/>
                <a:font script=""Tfng"" typeface=""Ebrima""/>
            </a:majorFont>
            <a:minorFont>
                <a:latin typeface=""Calibri"" panose=""020F0502020204030204""/>
                <a:ea typeface=""""/>
                <a:cs typeface=""""/>
                <a:font script=""Jpan"" typeface=""游明朝""/>
                <a:font script=""Hang"" typeface=""맑은 고딕""/>
                <a:font script=""Hans"" typeface=""等线""/>
                <a:font script=""Hant"" typeface=""新細明體""/>
                <a:font script=""Arab"" typeface=""Arial""/>
                <a:font script=""Hebr"" typeface=""Arial""/>
                <a:font script=""Thai"" typeface=""Cordia New""/>
                <a:font script=""Ethi"" typeface=""Nyala""/>
                <a:font script=""Beng"" typeface=""Vrinda""/>
                <a:font script=""Gujr"" typeface=""Shruti""/>
                <a:font script=""Khmr"" typeface=""DaunPenh""/>
                <a:font script=""Knda"" typeface=""Tunga""/>
                <a:font script=""Guru"" typeface=""Raavi""/>
                <a:font script=""Cans"" typeface=""Euphemia""/>
                <a:font script=""Cher"" typeface=""Plantagenet Cherokee""/>
                <a:font script=""Yiii"" typeface=""Microsoft Yi Baiti""/>
                <a:font script=""Tibt"" typeface=""Microsoft Himalaya""/>
                <a:font script=""Thaa"" typeface=""MV Boli""/>
                <a:font script=""Deva"" typeface=""Mangal""/>
                <a:font script=""Telu"" typeface=""Gautami""/>
                <a:font script=""Taml"" typeface=""Latha""/>
                <a:font script=""Syrc"" typeface=""Estrangelo Edessa""/>
                <a:font script=""Orya"" typeface=""Kalinga""/>
                <a:font script=""Mlym"" typeface=""Kartika""/>
                <a:font script=""Laoo"" typeface=""DokChampa""/>
                <a:font script=""Sinh"" typeface=""Iskoola Pota""/>
                <a:font script=""Mong"" typeface=""Mongolian Baiti""/>
                <a:font script=""Viet"" typeface=""Arial""/>
                <a:font script=""Uigh"" typeface=""Microsoft Uighur""/>
                <a:font script=""Geor"" typeface=""Sylfaen""/>
                <a:font script=""Armn"" typeface=""Arial""/>
                <a:font script=""Bugi"" typeface=""Leelawadee UI""/>
                <a:font script=""Bopo"" typeface=""Microsoft JhengHei""/>
                <a:font script=""Java"" typeface=""Javanese Text""/>
                <a:font script=""Lisu"" typeface=""Segoe UI""/>
                <a:font script=""Mymr"" typeface=""Myanmar Text""/>
                <a:font script=""Nkoo"" typeface=""Ebrima""/>
                <a:font script=""Olck"" typeface=""Nirmala UI""/>
                <a:font script=""Osma"" typeface=""Ebrima""/>
                <a:font script=""Phag"" typeface=""Phagspa""/>
                <a:font script=""Syrn"" typeface=""Estrangelo Edessa""/>
                <a:font script=""Syrj"" typeface=""Estrangelo Edessa""/>
                <a:font script=""Syre"" typeface=""Estrangelo Edessa""/>
                <a:font script=""Sora"" typeface=""Nirmala UI""/>
                <a:font script=""Tale"" typeface=""Microsoft Tai Le""/>
                <a:font script=""Talu"" typeface=""Microsoft New Tai Lue""/>
                <a:font script=""Tfng"" typeface=""Ebrima""/>
            </a:minorFont>
        </a:fontScheme>
        <a:fmtScheme name=""Office"">
            <a:fillStyleLst>
                <a:solidFill>
                    <a:schemeClr val=""phClr""/>
                </a:solidFill>
                <a:gradFill rotWithShape=""1"">
                    <a:gsLst>
                        <a:gs pos=""0"">
                            <a:schemeClr val=""phClr"">
                                <a:lumMod val=""110000""/>
                                <a:satMod val=""105000""/>
                                <a:tint val=""67000""/>
                            </a:schemeClr>
                        </a:gs>
                        <a:gs pos=""50000"">
                            <a:schemeClr val=""phClr"">
                                <a:lumMod val=""105000""/>
                                <a:satMod val=""103000""/>
                                <a:tint val=""73000""/>
                            </a:schemeClr>
                        </a:gs>
                        <a:gs pos=""100000"">
                            <a:schemeClr val=""phClr"">
                                <a:lumMod val=""105000""/>
                                <a:satMod val=""109000""/>
                                <a:tint val=""81000""/>
                            </a:schemeClr>
                        </a:gs>
                    </a:gsLst>
                    <a:lin ang=""5400000"" scaled=""0""/>
                </a:gradFill>
                <a:gradFill rotWithShape=""1"">
                    <a:gsLst>
                        <a:gs pos=""0"">
                            <a:schemeClr val=""phClr"">
                                <a:satMod val=""103000""/>
                                <a:lumMod val=""102000""/>
                                <a:tint val=""94000""/>
                            </a:schemeClr>
                        </a:gs>
                        <a:gs pos=""50000"">
                            <a:schemeClr val=""phClr"">
                                <a:satMod val=""110000""/>
                                <a:lumMod val=""100000""/>
                                <a:shade val=""100000""/>
                            </a:schemeClr>
                        </a:gs>
                        <a:gs pos=""100000"">
                            <a:schemeClr val=""phClr"">
                                <a:lumMod val=""99000""/>
                                <a:satMod val=""120000""/>
                                <a:shade val=""78000""/>
                            </a:schemeClr>
                        </a:gs>
                    </a:gsLst>
                    <a:lin ang=""5400000"" scaled=""0""/>
                </a:gradFill>
            </a:fillStyleLst>
            <a:lnStyleLst>
                <a:ln w=""6350"" cap=""flat"" cmpd=""sng"" algn=""ctr"">
                    <a:solidFill>
                        <a:schemeClr val=""phClr""/>
                    </a:solidFill>
                    <a:prstDash val=""solid""/>
                    <a:miter lim=""800000""/>
                </a:ln>
                <a:ln w=""12700"" cap=""flat"" cmpd=""sng"" algn=""ctr"">
                    <a:solidFill>
                        <a:schemeClr val=""phClr""/>
                    </a:solidFill>
                    <a:prstDash val=""solid""/>
                    <a:miter lim=""800000""/>
                </a:ln>
                <a:ln w=""19050"" cap=""flat"" cmpd=""sng"" algn=""ctr"">
                    <a:solidFill>
                        <a:schemeClr val=""phClr""/>
                    </a:solidFill>
                    <a:prstDash val=""solid""/>
                    <a:miter lim=""800000""/>
                </a:ln>
            </a:lnStyleLst>
            <a:effectStyleLst>
                <a:effectStyle>
                    <a:effectLst/>
                </a:effectStyle>
                <a:effectStyle>
                    <a:effectLst/>
                </a:effectStyle>
                <a:effectStyle>
                    <a:effectLst>
                        <a:outerShdw blurRad=""57150"" dist=""19050"" dir=""5400000"" algn=""ctr"" rotWithShape=""0"">
                            <a:srgbClr val=""000000"">
                                <a:alpha val=""63000""/>
                            </a:srgbClr>
                        </a:outerShdw>
                    </a:effectLst>
                </a:effectStyle>
            </a:effectStyleLst>
            <a:bgFillStyleLst>
                <a:solidFill>
                    <a:schemeClr val=""phClr""/>
                </a:solidFill>
                <a:solidFill>
                    <a:schemeClr val=""phClr"">
                        <a:tint val=""95000""/>
                        <a:satMod val=""170000""/>
                    </a:schemeClr>
                </a:solidFill>
                <a:gradFill rotWithShape=""1"">
                    <a:gsLst>
                        <a:gs pos=""0"">
                            <a:schemeClr val=""phClr"">
                                <a:tint val=""93000""/>
                                <a:satMod val=""150000""/>
                                <a:shade val=""98000""/>
                                <a:lumMod val=""102000""/>
                            </a:schemeClr>
                        </a:gs>
                        <a:gs pos=""50000"">
                            <a:schemeClr val=""phClr"">
                                <a:tint val=""98000""/>
                                <a:satMod val=""130000""/>
                                <a:shade val=""90000""/>
                                <a:lumMod val=""103000""/>
                            </a:schemeClr>
                        </a:gs>
                        <a:gs pos=""100000"">
                            <a:schemeClr val=""phClr"">
                                <a:shade val=""63000""/>
                                <a:satMod val=""120000""/>
                            </a:schemeClr>
                        </a:gs>
                    </a:gsLst>
                    <a:lin ang=""5400000"" scaled=""0""/>
                </a:gradFill>
            </a:bgFillStyleLst>
        </a:fmtScheme>
    </a:themeElements>
    <a:objectDefaults/>
    <a:extraClrSchemeLst/>
    <a:extLst>
        <a:ext uri=""{05A4C25C-085E-4340-85A3-A5531E510DB2}"">
            <thm15:themeFamily xmlns:thm15=""http://schemas.microsoft.com/office/thememl/2012/main"" name=""Office Theme"" id=""{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}"" vid=""{4A3C46E8-61CC-4603-A589-7422A47A8E4A}""/>
        </a:ext>
    </a:extLst>
</a:theme>");
            }

            public static DocumentFormat.OpenXml.Wordprocessing.Fonts CreateDefaultFonts()
            {
                return new DocumentFormat.OpenXml.Wordprocessing.Fonts(@"<w:fonts xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006"" xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships"" xmlns:w=""http://schemas.openxmlformats.org/wordprocessingml/2006/main"" xmlns:w14=""http://schemas.microsoft.com/office/word/2010/wordml"" xmlns:w15=""http://schemas.microsoft.com/office/word/2012/wordml"" xmlns:w16cex=""http://schemas.microsoft.com/office/word/2018/wordml/cex"" xmlns:w16cid=""http://schemas.microsoft.com/office/word/2016/wordml/cid"" xmlns:w16=""http://schemas.microsoft.com/office/word/2018/wordml"" xmlns:w16se=""http://schemas.microsoft.com/office/word/2015/wordml/symex"" mc:Ignorable=""w14 w15 w16se w16cid w16 w16cex"">
    <w:font w:name=""Calibri"">
        <w:panose1 w:val=""020F0502020204030204""/>
        <w:charset w:val=""00""/>
        <w:family w:val=""swiss""/>
        <w:pitch w:val=""variable""/>
        <w:sig w:usb0=""E4002EFF"" w:usb1=""C000247B"" w:usb2=""00000009"" w:usb3=""00000000"" w:csb0=""000001FF"" w:csb1=""00000000""/>
    </w:font>
    <w:font w:name=""Times New Roman"">
        <w:panose1 w:val=""02020603050405020304""/>
        <w:charset w:val=""00""/>
        <w:family w:val=""roman""/>
        <w:pitch w:val=""variable""/>
        <w:sig w:usb0=""E0002EFF"" w:usb1=""C000785B"" w:usb2=""00000009"" w:usb3=""00000000"" w:csb0=""000001FF"" w:csb1=""00000000""/>
    </w:font>
    <w:font w:name=""Calibri Light"">
        <w:panose1 w:val=""020F0302020204030204""/>
        <w:charset w:val=""00""/>
        <w:family w:val=""swiss""/>
        <w:pitch w:val=""variable""/>
        <w:sig w:usb0=""E4002EFF"" w:usb1=""C000247B"" w:usb2=""00000009"" w:usb3=""00000000"" w:csb0=""000001FF"" w:csb1=""00000000""/>
    </w:font>
    <w:font w:name=""&quot;Open Sans&quot;"">
        <w:altName w:val=""Cambria""/>
        <w:panose1 w:val=""00000000000000000000""/>
        <w:charset w:val=""00""/>
        <w:family w:val=""roman""/>
        <w:notTrueType/>
        <w:pitch w:val=""default""/>
    </w:font>
</w:fonts>");
            }

            public static void SetupDefault(MainDocumentPart doc)
            {
                if (doc.StyleDefinitionsPart == null)
                    doc.AddNewPart<StyleDefinitionsPart>().Styles = CreateDefaultStyles();

                if (doc.FontTablePart == null)
                    doc.AddNewPart<FontTablePart>().Fonts = CreateDefaultFonts();

                if (doc.ThemePart == null)
                    doc.AddNewPart<ThemePart>().Theme = CreateDefaultTheme();
            }
        }

        #endregion

        public const double PointsPerDxa = 20d;
        public const double InchesPerDxa = PointsPerDxa * 72d;
        public const double MillimetersPerDxa = InchesPerDxa / 25.4d;
        public const double CentimetersPerDxa = MillimetersPerDxa * 10;

        public static T PointsToDxa<T>(double pt) where T : IConvertible => (T)Convert.ChangeType(Math.Round(pt * PointsPerDxa), typeof(T));
        public static T InchesToDxa<T>(double @in) where T : IConvertible => (T)Convert.ChangeType(Math.Round(@in * InchesPerDxa), typeof(T));
        public static T MillimetersToDxa<T>(double mm) where T : IConvertible => (T)Convert.ChangeType(Math.Round(mm * MillimetersPerDxa), typeof(T));
        public static T CentimetersToDxa<T>(double cm) where T : IConvertible => (T)Convert.ChangeType(Math.Round(cm * CentimetersPerDxa), typeof(T));

        public static void SetupHeaderFooter(OpenXmlElement el)
        {
            el.MCAttributes = new MarkupCompatibilityAttributes
            {
                Ignorable = "w14 w15 w16se w16cid w16 w16cex wp14"
            };
            el.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
            el.AddNamespaceDeclaration("cx", "http://schemas.microsoft.com/office/drawing/2014/chartex");
            el.AddNamespaceDeclaration("cx1", "http://schemas.microsoft.com/office/drawing/2015/9/8/chartex");
            el.AddNamespaceDeclaration("cx2", "http://schemas.microsoft.com/office/drawing/2015/10/21/chartex");
            el.AddNamespaceDeclaration("cx3", "http://schemas.microsoft.com/office/drawing/2016/5/9/chartex");
            el.AddNamespaceDeclaration("cx4", "http://schemas.microsoft.com/office/drawing/2016/5/10/chartex");
            el.AddNamespaceDeclaration("cx5", "http://schemas.microsoft.com/office/drawing/2016/5/11/chartex");
            el.AddNamespaceDeclaration("cx6", "http://schemas.microsoft.com/office/drawing/2016/5/12/chartex");
            el.AddNamespaceDeclaration("cx7", "http://schemas.microsoft.com/office/drawing/2016/5/13/chartex");
            el.AddNamespaceDeclaration("cx8", "http://schemas.microsoft.com/office/drawing/2016/5/14/chartex");
            el.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            el.AddNamespaceDeclaration("aink", "http://schemas.microsoft.com/office/drawing/2016/ink");
            el.AddNamespaceDeclaration("am3d", "http://schemas.microsoft.com/office/drawing/2017/model3d");
            el.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
            el.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            el.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            el.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
            el.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
            el.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            el.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
            el.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            el.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
            el.AddNamespaceDeclaration("w15", "http://schemas.microsoft.com/office/word/2012/wordml");
            el.AddNamespaceDeclaration("w16cex", "http://schemas.microsoft.com/office/word/2018/wordml/cex");
            el.AddNamespaceDeclaration("w16cid", "http://schemas.microsoft.com/office/word/2016/wordml/cid");
            el.AddNamespaceDeclaration("w16", "http://schemas.microsoft.com/office/word/2018/wordml");
            el.AddNamespaceDeclaration("w16se", "http://schemas.microsoft.com/office/word/2015/wordml/symex");
            el.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
            el.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
            el.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
            el.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");
        }

        public static void EnsureNumberingExists(MainDocumentPart doc)
        {
            if (doc.NumberingDefinitionsPart == null)
                doc.AddNewPart<NumberingDefinitionsPart>();

            if (doc.NumberingDefinitionsPart.Numbering != null)
                return;

            var numbering = new Numbering
            {
                MCAttributes = new MarkupCompatibilityAttributes { Ignorable = "w14 w15 w16se w16cid w16 w16cex wp14" }
            };
            numbering.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
            numbering.AddNamespaceDeclaration("cx", "http://schemas.microsoft.com/office/drawing/2014/chartex");
            numbering.AddNamespaceDeclaration("cx1", "http://schemas.microsoft.com/office/drawing/2015/9/8/chartex");
            numbering.AddNamespaceDeclaration("cx2", "http://schemas.microsoft.com/office/drawing/2015/10/21/chartex");
            numbering.AddNamespaceDeclaration("cx3", "http://schemas.microsoft.com/office/drawing/2016/5/9/chartex");
            numbering.AddNamespaceDeclaration("cx4", "http://schemas.microsoft.com/office/drawing/2016/5/10/chartex");
            numbering.AddNamespaceDeclaration("cx5", "http://schemas.microsoft.com/office/drawing/2016/5/11/chartex");
            numbering.AddNamespaceDeclaration("cx6", "http://schemas.microsoft.com/office/drawing/2016/5/12/chartex");
            numbering.AddNamespaceDeclaration("cx7", "http://schemas.microsoft.com/office/drawing/2016/5/13/chartex");
            numbering.AddNamespaceDeclaration("cx8", "http://schemas.microsoft.com/office/drawing/2016/5/14/chartex");
            numbering.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            numbering.AddNamespaceDeclaration("aink", "http://schemas.microsoft.com/office/drawing/2016/ink");
            numbering.AddNamespaceDeclaration("am3d", "http://schemas.microsoft.com/office/drawing/2017/model3d");
            numbering.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
            numbering.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            numbering.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            numbering.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
            numbering.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
            numbering.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            numbering.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
            numbering.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            numbering.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
            numbering.AddNamespaceDeclaration("w15", "http://schemas.microsoft.com/office/word/2012/wordml");
            numbering.AddNamespaceDeclaration("w16cex", "http://schemas.microsoft.com/office/word/2018/wordml/cex");
            numbering.AddNamespaceDeclaration("w16cid", "http://schemas.microsoft.com/office/word/2016/wordml/cid");
            numbering.AddNamespaceDeclaration("w16", "http://schemas.microsoft.com/office/word/2018/wordml");
            numbering.AddNamespaceDeclaration("w16se", "http://schemas.microsoft.com/office/word/2015/wordml/symex");
            numbering.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
            numbering.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
            numbering.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
            numbering.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

            doc.NumberingDefinitionsPart.Numbering = numbering;
        }

        public static AbstractNum InsertAbstractNum(MainDocumentPart doc, AbstractNum newAbstractNum)
        {
            EnsureNumberingExists(doc);

            newAbstractNum.AbstractNumberId = 0;

            AbstractNum lastAbstractNum = null;

            var abstractNums = doc.NumberingDefinitionsPart.Numbering.Elements<AbstractNum>().ToArray();
            if (abstractNums.Length > 0)
            {
                newAbstractNum.AbstractNumberId = abstractNums.Max(x => x.AbstractNumberId) + 1;
                lastAbstractNum = abstractNums.LastOrDefault();
            }

            if (lastAbstractNum == null)
                doc.NumberingDefinitionsPart.Numbering.InsertAt(newAbstractNum, 0);
            else
                doc.NumberingDefinitionsPart.Numbering.InsertAfter(newAbstractNum, lastAbstractNum);

            return newAbstractNum;
        }

        public static NumberingInstance InsertNumInstance(MainDocumentPart doc, NumberingInstance newNumInstance)
        {
            EnsureNumberingExists(doc);

            newNumInstance.NumberID = 1;

            NumberingInstance lastNumInstance = null;

            var numInstances = doc.NumberingDefinitionsPart.Numbering.Elements<NumberingInstance>().ToArray();

            if (numInstances.Length > 0)
            {
                newNumInstance.NumberID = numInstances.Max(x => x.NumberID) + 1;
                lastNumInstance = numInstances.LastOrDefault();
            }

            if (lastNumInstance == null)
                doc.NumberingDefinitionsPart.Numbering.AppendChild(newNumInstance);
            else
                doc.NumberingDefinitionsPart.Numbering.InsertAfter(newNumInstance, lastNumInstance);

            return newNumInstance;
        }
    }
}
