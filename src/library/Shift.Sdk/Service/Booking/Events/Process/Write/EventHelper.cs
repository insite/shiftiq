using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using InSite.Application.Events.Read;

using Shift.Common;

namespace InSite.Application.Events.Write
{
    public static class EventHelper
    {
        private static readonly Regex DurationCodePattern = new Regex(@".+\((\d)hr?\)", RegexOptions.Compiled);

        public static int GetDuration(string code)
        {
            var result = 0;

            if (!string.IsNullOrEmpty(code))
            {
                var match = DurationCodePattern.Match(code);
                if (match.Success)
                    result = 60 * int.Parse(match.Groups[1].Value);
            }

            if (result == 0)
            {
                if (StringHelper.EqualsAny(code, new[] { "031", "032" }))
                    result = 60 * 3;
                else if (StringHelper.EqualsAny(code, new[] { "041", "042", "043", "044", "045" }))
                    result = 60 * 4;
                else if (StringHelper.EqualsAny(code, new[] { "054", "055" }))
                    result = 60 * 5;
            }

            return result;
        }

        public static string GetTitle(Guid id, string venueName, IEventSearch eventSearch)
        {
            if (string.IsNullOrEmpty(venueName))
                venueName = "No Venue";

            var examTitle = "No Exam";
            if (id != Guid.Empty)
            {
                var exams = eventSearch.GetEventAssessmentForms(id);
                if (exams.Count == 1)
                    examTitle = exams[0].Form.FormTitle;
                else if (exams.Count > 1)
                    examTitle = "Various";
            }

            return $"{venueName} / {examTitle}";
        }

        public static string IntArrayToBase64(IEnumerable<int> values)
        {
            return StringHelper.EncodeBase64Url(stream =>
            {
                using (var bw = new BinaryWriter(stream))
                    foreach (var value in values)
                        bw.Write(value);
            });
        }

        public static int[] IntArrayFromBase64(string data)
        {
            return StringHelper.DecodeBase64Url(data, stream =>
            {
                var list = new List<int>();
                var length = stream.Length;

                using (var bw = new BinaryReader(stream))
                    while (stream.Position != length)
                        list.Add(bw.ReadInt32());

                return list.ToArray();
            });
        }
    }
}
