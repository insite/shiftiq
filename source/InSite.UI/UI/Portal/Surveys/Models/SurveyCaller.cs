using System;
using System.IO;

using InSite.Common.Web;

using Shift.Common;
using Shift.Constant;

namespace InSite.Portal.Surveys.Pages
{
    public sealed class SurveyCaller
    {
        public SurveyCallerType Type { get; private set; }

        public int? CourseNumber { get; private set; }

        public int? ModuleNumber { get; private set; }

        public string PortalTab { get; private set; }

        private SurveyCaller()
        {

        }

        public static SurveyCaller CreateCourse(int course, int? module) => new SurveyCaller
        {
            Type = SurveyCallerType.Course1,
            CourseNumber = course,
            ModuleNumber = module
        };

        public static SurveyCaller CreatePortal(string tab) => new SurveyCaller
        {
            Type = SurveyCallerType.Portal,
            PortalTab = tab
        };

        public string GetReturnUrl()
        {
            if (Type == SurveyCallerType.Portal)
            {
                var url = "/ui/portal/home";

                if (PortalTab.IsNotEmpty())
                    url = HttpResponseHelper.BuildUrl(url, $"tab={PortalTab}");

                return url;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public string Serialize() => StringHelper.EncodeBase64Url(stream =>
        {
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write((byte)Type);

                if (Type == SurveyCallerType.Course1)
                {
                    writer.Write(CourseNumber.Value);
                    writer.WriteNullable(ModuleNumber);
                }
                else if (Type == SurveyCallerType.Portal)
                {
                    writer.WriteNullable(PortalTab);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        });

        public static SurveyCaller Deserialize(string data) => StringHelper.DecodeBase64Url(data, stream =>
        {
            var result = new SurveyCaller();

            using (var reader = new BinaryReader(stream))
            {
                result.Type = (SurveyCallerType)(int)reader.ReadByte();

                if (result.Type == SurveyCallerType.Course1)
                {
                    result.CourseNumber = reader.ReadInt32();
                    result.ModuleNumber = reader.ReadInt32Nullable();
                }
                else if (result.Type == SurveyCallerType.Portal)
                {
                    result.PortalTab = reader.ReadStringNullable();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return result;
        });
    }
}