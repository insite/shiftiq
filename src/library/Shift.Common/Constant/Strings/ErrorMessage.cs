namespace Shift.Constant
{
    public class ErrorMessage
    {
        public static string AssetNotFound => "The system could not find asset number {0}. Please contact your administrator with the steps you followed to arrive at this error message.";
        public static string CannotResolveUri => "Failed to resolve URI: {0}";
        public static string ColumnNotFound => "Column not found: {0}";
        public static string ConnectionStringNotFound => "Connection string not found: {0}";
        public static string ConnectionStringNotSpecified => "Connection string not specified: {0}";
        public static string DataTypeMismatch => "Data type mismatch {0}: {1}";
        public static string DataTypeMismatchOnApplicationSetting => @"The data type for the application setting value named ""{0}"" is unexpected: {2} is not {1}";
        public static string FailedLookupOnXmlNamespace => "Failed lookup on XML namespace";
        public static string FailedToCreateXPathNavigator => "Failed to create XPathNavigator";
        public static string FailedToSingularizeWord => "Failed to singularize word: {0}";
        public static string FileNotFound => "File not found: {0}";
        public static string HttpError => "HTTP Error {0}: {1}";
        public static string InvalidContextExpression => "Invalid context expression format: {0}. Instead you must use this format: name = value.";
        public static string InvalidTimeInterval => "Invalid interval: {0}";
        public static string InvalidTimeIntervalInMonths => "Invalid Interval: {0} Months -- The number of months specified in a time interval cannot exceed 11.";
        public static string InvalidTimeIntervalInSeconds => "Invalid Interval: {0:n0} Seconds -- The total number of seconds defined by a time interval cannot exceed {1:n0}.";
        public static string InvalidTypeConversion => "Invalid Conversion: Cannot cast from {0} to {1}";
        public static string MissingApplicationSetting => "Missing application setting: {0}";
        public static string MissingPropertyValue => "Missing property value: {0}";
        public static string MissingRequiredField => "Required field: {0}";
        public static string ParsingFailed => "Parsing failed: {0}";
        public static string PropertyValueMustContainAlphanumerics => "Property value must contain at least one alphanumeric character: {0}";
        public static string RandomStringTooShort => "You must request a value with a length of 2 or more characters";
        public static string UnexpectedDataType => "Unexpected data type: {0}";
        public static string UnexpectedFormat => "Unexpected format: {0}";
        public static string UnexpectedNullParameterValue => "Unexpected null parameter value: {0}";
        public static string UnexpectedToken => "Unexpected token: {0}";
        public static string UnexpectedTokenDisplayExpected => "Unexpected token: {0} (expected {1})";
        public static string UnexpectedValue => "Unexpected value: {0}";
        public static string UnsupportedClass => "Unsupported class";
        public static string ValidationFailedOnAppSettingHealthCheck => "There {0} {1} setting{2} in this checklist that failed validation.";
    }
}
