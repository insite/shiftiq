using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Files.Read;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

using IFFmpegProbeStream = Shift.Common.FFmpeg.IProbeStream;

namespace InSite.Common.Web.UI
{
    [ValidationProperty(nameof(VideoCapture))]
    public class InputVideo : InputMedia
    {
        #region Constants

        public const int MaximumRecordingTime = 10 * 60 + 59;
        public const int MinimumBitrateValue = 30000;
        private const double MinimumQualityValue = 0.2;

        private static readonly ResolutionInfo[] Resoltuions = new ResolutionInfo[]
        {
            new ResolutionInfo(144, 150000, 250000),
            new ResolutionInfo(240, 450000, 700000),
            new ResolutionInfo(360, 1000000, 1500000),
            new ResolutionInfo(480, 2500000, 4000000),
            new ResolutionInfo(720, 5000000, 7500000),
            new ResolutionInfo(1080, 8000000, 12000000),
            new ResolutionInfo(1440, 16000000, 24000000),
            new ResolutionInfo(2160, 35000000, 54000000)
        };

        #endregion

        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class VideoClientData : BaseClientData
        {
            [JsonProperty(PropertyName = "source", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public string Source { get; set; }
        }

        private class ResolutionInfo
        {
            public int Height { get; private set; }

            private int _bitrate30;
            private int _bitrate60;

            public ResolutionInfo(int height, int bitrate30, int bitrate60)
            {
                Height = height;

                _bitrate30 = bitrate30;
                _bitrate60 = bitrate60;
            }

            public int GetBitrate(VideoFrameRate frameRate, double quality)
            {
                double baseBitrate;

                switch (frameRate)
                {
                    case VideoFrameRate.fps_30: baseBitrate = _bitrate30; break;
                    case VideoFrameRate.fps_60: baseBitrate = _bitrate60; break;
                    default: throw ApplicationError.Create("Unexpected frame rate value: {0}", frameRate.GetName());
                }

                return (int)Math.Round(baseBitrate * (quality * (1 - MinimumQualityValue) + MinimumQualityValue), MidpointRounding.AwayFromZero);
            }
        }

        public interface IVideoFile
        {
            string Name { get; }
            int Size { get; }

            IFFmpegProbeStream AudioStream { get; }
            IFFmpegProbeStream VideoStream { get; }
            TimeSpan Duration { get; }
            int AudioBitrate { get; }
            int VideoBitrate { get; }

            string ValidationError { get; }
            bool IsValid { get; }

            void Open(Action<Stream> action);
            FileStorageModel Save(Guid objectIdentifier, FileObjectType objectType, string fileName = null, bool checkFileValid = true);
            void Delete();
        }

        private class VideoFile : IVideoFile
        {
            public IMediaFile MediaFile => _file;
            public string Name => _file.Name;
            public int Size => _file.Size;
            public string ValidationError => _file.ValidationError;
            public bool IsValid => _file.IsValid;

            public IFFmpegProbeStream AudioStream { get; set; }
            public IFFmpegProbeStream VideoStream { get; set; }
            public TimeSpan Duration { get; set; }
            public int AudioBitrate { get; set; }
            public int VideoBitrate { get; set; }

            IMediaFile _file;

            public VideoFile(IMediaFile file)
            {
                _file = file;
            }

            public void Delete() => _file.Delete();

            public void Open(Action<Stream> action) => _file.Open(action);

            public FileStorageModel Save(Guid objectIdentifier, FileObjectType objectType, string fileName = null, bool checkFileValid = true) =>
                _file.Save(objectIdentifier, objectType, fileName, checkFileValid);
        }

        #endregion

        #region Properties

        public bool AllowInitCamera
        {
            get => (bool)(ViewState[nameof(AllowInitCamera)] ?? true);
            set => ViewState[nameof(AllowInitCamera)] = value;
        }

        public int TimeLimit
        {
            get => (int)(ViewState[nameof(TimeLimit)] ?? 0);
            set => ViewState[nameof(TimeLimit)] = Number.CheckRange(value, 0, MaximumRecordingTime);
        }

        public AudioBitrateMode AudioBitrate
        {
            get => (AudioBitrateMode)(ViewState[nameof(AudioBitrate)] ?? AudioBitrateMode.kb_64);
            set => ViewState[nameof(AudioBitrate)] = value;
        }

        public VideoBitrateMode VideoBitrateMode
        {
            get => (VideoBitrateMode)(ViewState[nameof(VideoBitrateMode)] ?? VideoBitrateMode.Auto);
            set => ViewState[nameof(VideoBitrateMode)] = value;
        }

        public double VideoQuality
        {
            get => (double)(ViewState[nameof(VideoQuality)] ?? 0.75);
            set => ViewState[nameof(VideoQuality)] = Number.CheckRange(value, 0, 1);
        }

        public VideoFrameRate CameraFrameRate
        {
            get => (VideoFrameRate)(ViewState[nameof(CameraFrameRate)] ?? VideoFrameRate.fps_30);
            set => ViewState[nameof(CameraFrameRate)] = value;
        }

        public VideoResolution CameraResolution
        {
            get => (VideoResolution)(ViewState[nameof(CameraResolution)] ?? VideoResolution.p_360);
            set => ViewState[nameof(CameraResolution)] = value;
        }

        public int CameraBitrate
        {
            get => (int)(ViewState[nameof(CameraBitrate)] ?? MinimumBitrateValue);
            set => ViewState[nameof(CameraBitrate)] = Number.CheckRange(value, MinimumBitrateValue);
        }

        public VideoFrameRate ScreenFrameRate
        {
            get => (VideoFrameRate)(ViewState[nameof(ScreenFrameRate)] ?? VideoFrameRate.fps_30);
            set => ViewState[nameof(ScreenFrameRate)] = value;
        }

        public VideoResolution ScreenResolution
        {
            get => (VideoResolution)(ViewState[nameof(ScreenResolution)] ?? VideoResolution.p_1440);
            set => ViewState[nameof(ScreenResolution)] = value;
        }

        public int ScreenBitrate
        {
            get => (int)(ViewState[nameof(ScreenBitrate)] ?? MinimumBitrateValue);
            set => ViewState[nameof(ScreenBitrate)] = Number.CheckRange(value, MinimumBitrateValue);
        }

        public Unit Width
        {
            get => (Unit)(ViewState[nameof(Width)] ?? Unit.Empty);
            set => ViewState[nameof(Width)] = value;
        }

        public IVideoFile VideoCapture => _videoFile;

        #endregion

        #region Fields

        private VideoFile _videoFile;

        #endregion

        #region Loading

        protected override void OnPreRender(EventArgs e)
        {
            ScriptManager.RegisterStartupScript(
                Page,
                typeof(InputVideo),
                "init_" + ClientID,
                $"inSite.common.inputVideo.init('{ClientID}');",
                true);

            base.OnPreRender(e);
        }

        #endregion

        #region IPostBackDataHandler

        protected override BaseClientData DeserializeClientData(string data)
        {
            return JsonConvert.DeserializeObject<VideoClientData>(data);
        }

        #endregion

        #region Validation

        protected override void SetMediaFile(BaseMediaFile file, BaseClientData data)
        {
            var videoData = (VideoClientData)data;
            var isScreenCapture = videoData.Source == "screen";
            var audioBitrate = (int)AudioBitrate * 1000;
            var videoBitrate = -1;

            if (VideoBitrateMode == VideoBitrateMode.Auto)
            {
                videoBitrate = isScreenCapture
                    ? GetResolutionInfo(ScreenResolution).GetBitrate(ScreenFrameRate, 1)
                    : GetResolutionInfo(CameraResolution).GetBitrate(CameraFrameRate, 1);
            }
            else if (VideoBitrateMode == VideoBitrateMode.Manual)
            {
                videoBitrate = isScreenCapture ? ScreenBitrate : CameraBitrate;
            }
            else
            {
                throw ApplicationError.Create("Unexpected bitrate mode: " + VideoBitrateMode.GetName());
            }

            _videoFile = new VideoFile(file);

            InitVideoFile(_videoFile, audioBitrate, videoBitrate, TimeLimit);
        }

        private static void InitVideoFile(VideoFile file, int audioBitrate, int videoBitrate, int timeLimit)
        {
            var data = ProbeMedia(file.MediaFile);
            if (data == null)
                return;

            var streamCount = data.Streams.Length;
            var hasAudioStream = streamCount == 2;

            if (streamCount != 1 && !hasAudioStream)
            {
                file.MediaFile.ValidationError = $"Unexpected stream count: {data.Streams.Length} while expected 1 or 2";
                return;
            }

            if (!data.Streams.Any(x => x.CodecType == "video"))
            {
                file.MediaFile.ValidationError = $"Unexpected stream type: video stream not found";
                return;
            }

            if (hasAudioStream && !data.Streams.Any(x => x.CodecType == "audio"))
            {
                file.MediaFile.ValidationError = $"Unexpected stream type: audio stream not found";
                return;
            }

            file.AudioStream = data.Streams.FirstOrDefault(x => x.CodecType == "audio");
            file.VideoStream = data.Streams.FirstOrDefault(x => x.CodecType == "video");
            file.Duration = FFmpeg.CalculateDuration(data.Packets);
            file.AudioBitrate = FFmpeg.CalculateBitrate(data.Packets.Where(x => x.CodecType == "audio"), file.Duration);
            file.VideoBitrate = FFmpeg.CalculateBitrate(data.Packets.Where(x => x.CodecType == "video"), file.Duration);

            var maxDuration = timeLimit > 0
                ? TimeSpan.FromSeconds(timeLimit + 0.999)
                : TimeSpan.FromSeconds(MaximumRecordingTime + 0.999);

            if (file.Duration >= maxDuration)
            {
                file.MediaFile.ValidationError = $"Unexpected video capture duration: {file.Duration.Humanize()} while expected maximum duration is {maxDuration.Humanize()}";
                return;
            }

            var expectedVideoBitrate = videoBitrate;
            var maxVideoBitrate = (int)(expectedVideoBitrate * 1.25);

            if (file.VideoBitrate >= maxVideoBitrate)
            {
                file.MediaFile.ValidationError = $"Unexpected video bitrate: {file.VideoBitrate:n0} while expected maximum bitrate is {expectedVideoBitrate:n0}";
                return;
            }

            if (hasAudioStream)
            {
                var expectedAudioBitrate = audioBitrate;
                var maxAudioBitrate = (int)(expectedAudioBitrate * 1.1);

                if (file.AudioBitrate >= maxAudioBitrate)
                {
                    file.MediaFile.ValidationError = $"Unexpected audio bitrate: {file.AudioBitrate:n0} while expected maximum bitrate is {expectedAudioBitrate:n0}";
                    return;
                }
            }
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            var videoBitrate = VideoBitrateMode == VideoBitrateMode.Auto
                ? "a." + GetResolutionInfo(CameraResolution).GetBitrate(CameraFrameRate, VideoQuality) +
                  "." + GetResolutionInfo(ScreenResolution).GetBitrate(ScreenFrameRate, VideoQuality)
                : "m." + CameraBitrate + "." + ScreenBitrate;
            var config = $"{(int)AudioBitrate}.{videoBitrate}.{(int)CameraResolution}.{(int)CameraFrameRate}.{(int)ScreenResolution}.{(int)ScreenFrameRate}";

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "insite-input-video");
            writer.AddAttribute("data-video", config);
            writer.AddAttribute("data-upload", UploadMode.GetDescription());
            writer.AddAttribute("data-submit", SubmitScript);

            if (!Enabled)
                writer.AddAttribute("data-disabled", "1");

            if (ReadOnly)
                writer.AddAttribute("data-read-only", "1");

            if (AutoUpload)
                writer.AddAttribute("data-auto-upload", "1");

            if (AutoPostBack)
                writer.AddAttribute("data-auto-submit", "1");

            var style = GetStyle();
            if (style != null)
                writer.AddAttribute(HtmlTextWriterAttribute.Style, style);

            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                RenderVideoContainer(writer);
                RenderControlContainer(writer);
            }
            writer.RenderEndTag();

            if (TimeLimit > 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "form-text my-3");

                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.Write($"The recording time is limited to <strong>{TimeLimit.Seconds().Humanize(5).Replace(", ", " ")}</strong>");

                writer.RenderEndTag();
            }
        }

        protected string GetStyle()
        {
            var sb = new StringBuilder();

            if (!Width.IsEmpty)
                sb.Append("width:").Append(Width.ToString()).Append(";");

            if (Hidden)
                sb.Append("display:none;");

            return sb.Length == 0 ? null : sb.ToString();
        }

        private void RenderVideoContainer(HtmlTextWriter writer)
        {
            writer.Write("<div class=\"iv-video\"><i class='fas fa-video iv-icon'></i></div>");
        }

        private void RenderControlContainer(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "iv-control");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                RenderTimer(writer);
                RenderButtons(writer);
            }
            writer.RenderEndTag();
        }

        private void RenderTimer(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "iv-timer");
            writer.AddAttribute("data-time", TimeLimit.ToString());
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                var time = TimeLimit > 0 ? TimeLimit.Seconds() : TimeSpan.Zero;
                var timeValue = $"{Math.Floor(time.TotalMinutes):00}:{time.Seconds:00}.000";

                writer.Write($"<span class=\"iv-label\">Recording Time</span><span>{timeValue}</span>");

                writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
                writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
                writer.AddAttribute(HtmlTextWriterAttribute.Value, "{}");
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }

        private void RenderButtons(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "btn-group");
            writer.AddAttribute("role", "group");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                if (AllowInitCamera)
                    RenderButton(writer, "camera", "Camera", "secondary", "far fa-camera-web");

                RenderButton(writer, "settings", "Settings", "secondary", "far fa-gear");

                if (AllowPause)
                    RenderButton(writer, "pause", "Pause", "secondary", "fas fa-pause");

                RenderButton(writer, "start", "Start", "danger", "fas fa-circle");
            }
            writer.RenderEndTag();
        }

        private void RenderButton(HtmlTextWriter writer, string action, string title, string color, string icon)
        {
            writer.AddAttribute("data-action", action);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
            writer.AddAttribute(HtmlTextWriterAttribute.Title, title);
            writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, $"btn btn-outline-{color}");
            writer.RenderBeginTag(HtmlTextWriterTag.Button);
            writer.Write($"<i class='{icon}'></i>");
            writer.RenderEndTag();
        }

        #endregion

        #region Helper methods

        // DON'T REMOVE!
        private static string SetupSettings()
        {
            var frameRates = Enum.GetValues(typeof(VideoFrameRate)).Cast<VideoFrameRate>().ToArray();

            var resolutionsData = Enum.GetValues(typeof(VideoResolution)).Cast<VideoResolution>()
                .ToDictionary(x => (int)x, x =>
                {
                    var r = GetResolutionInfo(x);

                    return new
                    {
                        height = r.Height,
                        bitrate = frameRates.ToDictionary(y => (int)y, y => new
                        {
                            min = r.GetBitrate(y, 0),
                            max = r.GetBitrate(y, 1)
                        })
                    };
                });
            var frameRatesData = frameRates.ToDictionary(x => (int)x, x => GetFrameRateValue(x));

            var resolutionsJson = JsonConvert.SerializeObject(resolutionsData);
            var frameRatesJson = JsonConvert.SerializeObject(frameRatesData);

            return $"inSite.common.inputVideo.settings({MaximumRecordingTime},{MinimumBitrateValue},{resolutionsJson},{frameRatesJson});";
        }

        private static ResolutionInfo GetResolutionInfo(VideoResolution resolution)
        {
            switch (resolution)
            {
                case VideoResolution.p_144: return Resoltuions[0];
                case VideoResolution.p_240: return Resoltuions[1];
                case VideoResolution.p_360: return Resoltuions[2];
                case VideoResolution.p_480: return Resoltuions[3];
                case VideoResolution.p_720: return Resoltuions[4];
                case VideoResolution.p_1080: return Resoltuions[5];
                case VideoResolution.p_1440: return Resoltuions[6];
                case VideoResolution.p_2160: return Resoltuions[7];
            }

            throw ApplicationError.Create("Unexpected resoltuion value: {0}", resolution.GetName());
        }

        private static int GetFrameRateValue(VideoFrameRate frameRate)
        {
            switch (frameRate)
            {
                case VideoFrameRate.fps_30: return 30;
                case VideoFrameRate.fps_60: return 60;
            }

            throw ApplicationError.Create("Unexpected frame rate value: {0}", frameRate.GetName());
        }

        #endregion
    }
}