using System.IO;

namespace ConsoleApp1
{
    internal class Program
    {
        const string CodeModule1 = @"<CodeModule>SqlToolbox, Version=1.2.6262.20539, Culture=neutral, PublicKeyToken=null</CodeModule>";
        const string CodeModule2 = @"<CodeModule>SqlToolbox, Version=1.2.6379.27137, Culture=neutral, PublicKeyToken=null</CodeModule>";
        const string CodeModule3 = @"<CodeModule>SqlToolbox, Version=1.2.6414.41457, Culture=neutral, PublicKeyToken=null</CodeModule>";

        const string Code = @"
Function ConvertStringToNumber(ByVal value As String) As Decimal
    If IsNumeric(value) Then
        Return Convert.ToDecimal(value)
    Else
        Return 0
    End If
End Function

Function ConvertStringToObject(ByVal value As String) As Object
    If IsNumeric(value) Then
        Return Convert.ToDecimal(value)
    Else
        Return System.DBNull.Value
    End If
End Function

Function Divide(ByVal numerator As String, ByVal denominator As String) As String
    Return Divide(numerator, denominator, 0)
End Function

Function Divide(ByVal numerator As String, ByVal denominator As String, ByVal decimalPlaces As Integer) As String
    Dim x = ConvertStringToNumber(numerator)
    Dim y = ConvertStringToNumber(denominator)
    If y &lt;&gt; 0 Then
        Return FormatNumber(Convert.ToString(x / y), decimalPlaces)
    Else
        Return ""0""
    End If
End Function

Function FormatNumber(ByVal value As String) As String
    Return FormatNumber(value, 0)
End Function

Function FormatNumber(ByVal value As String, ByVal decimalPlaces As Integer) As String
    If IsNumeric(value) Then
        Select Case decimalPlaces
            Case 0
                Return Convert.ToDecimal(value).ToString(""#,##0"")
            Case 1
                Return Convert.ToDecimal(value).ToString(""#,##0.0"")
            Case Else
                Return Convert.ToDecimal(value).ToString(""#,##0.00"")
        End Select
    Else
        Return value
    End If
End Function

Function FormatPercent(ByVal value As String) As String
    Return FormatPercent(value, 0)
End Function

Function FormatPercent(ByVal value As String, ByVal decimalPlaces As Integer) As String
    If IsNumeric(value) Then
        Select Case decimalPlaces
            Case 0
                Return Convert.ToDecimal(value).ToString(""#,##0"") &amp; "" %""
            Case 1
                Return Convert.ToDecimal(value).ToString(""#,##0.0"") &amp; "" %""
            Case Else
                Return Convert.ToDecimal(value).ToString(""#,##0.00"") &amp; "" %""
        End Select
    Else
        Return value
    End If
End Function

Function IsNumeric(ByVal text As String) As Boolean
    Dim ok = False

    Try
        If Not Equals(text, Nothing) AndAlso text.Length &gt; 0 Then
            text = text.Trim()
            text = text.Replace(""$"", String.Empty)
            text = text.Replace(""%"", String.Empty)
            text = text.Replace("","", String.Empty)
            Convert.ToDecimal(text)
            ok = True
        End If
    Catch __unusedFormatException1__ As FormatException
    End Try

    Return ok
End Function
";

        static void Main(string[] args)
        {
            var root = @"C:\Base\Repos\InSite\Microsoft.SSRS\Source\Addons\NCSHA\Reports";
            var folder = new DirectoryInfo(root);
            var files = folder.GetFiles("*.rdl");

            foreach (var file in files)
            {
                var text = File.ReadAllText(file.FullName);

                text = text.Replace("<CodeModules>", "<Code>");
                text = text.Replace(CodeModule1, Code);
                text = text.Replace(CodeModule2, Code);
                text = text.Replace(CodeModule3, Code);
                text = text.Replace("</CodeModules>", "</Code>");
                text = text.Replace("SqlToolbox.NumberHelper.", "Code.");
                text = text.Replace("<DataSourceReference>/NCSHA/InSite</DataSourceReference>", "<DataSourceReference>InSite</DataSourceReference>");

                File.WriteAllText(file.FullName, text);
            }
        }
    }
}