using System;
using System.IO;

using InSite.Common.Web;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Workflow.Forms.Models
{
    public sealed class FormCaller
    {
        public FormCallerType Type { get; private set; }

        public int? CourseNumber { get; private set; }

        public int? ModuleNumber { get; private set; }

        public string PortalTab { get; private set; }

        private FormCaller()
        {

        }

        public static FormCaller CreateCourse(int course, int? module) => new FormCaller
        {
            Type = FormCallerType.Course1,
            CourseNumber = course,
            ModuleNumber = module
        };

        public static FormCaller CreatePortal(string tab) => new FormCaller
        {
            Type = FormCallerType.Portal,
            PortalTab = tab
        };

        public string GetReturnUrl()
        {
            if (Type == FormCallerType.Portal)
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

                if (Type == FormCallerType.Course1)
                {
                    writer.Write(CourseNumber.Value);
                    writer.WriteNullable(ModuleNumber);
                }
                else if (Type == FormCallerType.Portal)
                {
                    writer.WriteNullable(PortalTab);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        });

        public static FormCaller Deserialize(string data) => StringHelper.DecodeBase64Url(data, stream =>
        {
            var result = new FormCaller();

            using (var reader = new BinaryReader(stream))
            {
                result.Type = (FormCallerType)(int)reader.ReadByte();

                if (result.Type == FormCallerType.Course1)
                {
                    result.CourseNumber = reader.ReadInt32();
                    result.ModuleNumber = reader.ReadInt32Nullable();
                }
                else if (result.Type == FormCallerType.Portal)
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