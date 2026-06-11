using OfficeOpenXml.Style;

using Shift.Constant;

namespace Shift.Toolbox
{
    public class XlsxBorder
    {
        public XlsxBorderItem Top { get; } = new XlsxBorderItem();
        public XlsxBorderItem Right { get; } = new XlsxBorderItem();
        public XlsxBorderItem Bottom { get; } = new XlsxBorderItem();
        public XlsxBorderItem Left { get; } = new XlsxBorderItem();

        internal void Apply(Border border)
        {
            Top.Apply(border.Top);
            Right.Apply(border.Right);
            Bottom.Apply(border.Bottom);
            Left.Apply(border.Left);
        }

        public void BorderAround(XlsxBorderStyle style, System.Drawing.Color color)
        {
            Top.Style = style;
            Top.Color = color;
            Right.Style = style;
            Right.Color = color;
            Bottom.Style = style;
            Bottom.Color = color;
            Left.Style = style;
            Left.Color = color;
        }

        public void BorderAround(XlsxBorderStyle style)
        {
            Top.Style = style;
            Right.Style = style;
            Bottom.Style = style;
            Left.Style = style;
        }

        public XlsxBorder Copy()
        {
            var copy = new XlsxBorder();
            copy.Top.Style = Top.Style;
            copy.Top.Color = Top.Color;
            copy.Right.Style = Right.Style;
            copy.Right.Color = Right.Color;
            copy.Bottom.Style = Bottom.Style;
            copy.Bottom.Color = Bottom.Color;
            copy.Left.Style = Left.Style;
            copy.Left.Color = Left.Color;

            return copy;
        }
    }
}
