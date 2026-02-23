using System;
using System.Text.RegularExpressions;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// A simple version number that follows the convention "Major.Minor". For example: 
    ///     009.900
    ///       1.2 
    ///    2019.Q4
    ///       A.101
    /// </summary>
    [Serializable]
    public class Edition
    {
        public string Major { get; set; }
        public string Minor { get; set; }

        public Edition() { }

        public Edition(int major, int minor) { Major = major.ToString(); Minor = minor.ToString(); }

        public Edition(string major, string minor) { Major = major; Minor = minor; }

        public Edition(string version) { var v = Parse(version); Major = v.Major; Minor = v.Minor; }

        public Edition Clone()
        {
            return new Edition
            {
                Major = Major,
                Minor = Minor
            };
        }

        public Edition Parse(string version)
        {
            const string pattern = @"(\w+)\.(\w+)";

            var match = Regex.Match(version, pattern);

            if (!match.Success)
                throw new ArgumentException($"The version number {version} does not have the expected format. Please use this format: Major.Minor");

            Major = match.Groups[1].Value;
            Minor = match.Groups[2].Value;

            return new Edition { Major = Major, Minor = Minor };
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}";
        }
    }
}
