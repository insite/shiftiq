using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

using Humanizer;

using InSite.Application.Files.Read;


using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

using IFFmpegProbeStream = Shift.Common.FFmpeg.IProbeStream;

namespace InSite.Common.Web.UI
{
    [ValidationProperty(nameof(AudioCapture))]
    public class InputAudio : InputMedia
    {
        #region Constants

        public const int MaximumRecordingTime = 99 * 60 + 59;
        public const int MaximumAttemptLimit = 99;

        #endregion

        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class AudioClientData : BaseClientData
        {
            [JsonProperty(PropertyName = "attempt", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public int CurrentAttempt { get; set; }

            public bool IsAttemptChanged { get; set; }
        }

        public interface IAudioFile
        {
            string Name { get; }
            int Size { get; }

            IFFmpegProbeStream AudioStream { get; }
            TimeSpan Duration { get; }
            int Bitrate { get; }

            string ValidationError { get; }
            bool IsValid { get; }

            void Open(Action<Stream> action);
            FileStorageModel Save(Guid objectIdentifier, FileObjectType objectType, string fileName = null, bool checkFileValid = true);
            void Delete();
        }

        private class AudioFile : IAudioFile
        {
            public IMediaFile MediaFile => _file;
            public string Name => _file.Name;
            public int Size => _file.Size;
            public string ValidationError => _file.ValidationError;
            public bool IsValid => _file.IsValid;

            public IFFmpegProbeStream AudioStream { get; set; }
            public TimeSpan Duration { get; set; }
            public int Bitrate { get; set; }

            IMediaFile _file;

            public AudioFile(IMediaFile file)
            {
                _file = file;
            }

            public void Delete() => _file.Delete();

            public void Open(Action<Stream> action) => _file.Open(action);

            public FileStorageModel Save(Guid objectIdentifier, FileObjectType objectType, string fileName = null, bool checkFileValid = true) =>
                _file.Save(objectIdentifier, objectType, fileName, checkFileValid);
        }

        #endregion

        #region Events

        public event EventHandler AttemptChanged;
        private void OnAttemptChanged() =>
            AttemptChanged?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        public override bool ReadOnly
        {
            get => IsAttemptLimitExceeded || base.ReadOnly;
            set => base.ReadOnly = value;
        }

        public int AttemptLimit
        {
            get => (int)(ViewState[nameof(AttemptLimit)] ?? 0);
            set
            {
                var limit = Number.CheckRange(value, 0, MaximumAttemptLimit);

                ViewState[nameof(AttemptLimit)] = limit;

                if (CurrentAttempt > limit)
                    CurrentAttempt = limit;
            }
        }

        public int CurrentAttempt
        {
            get => (int)(ViewState[nameof(CurrentAttempt)] ?? 0);
            set
            {
                var attemptValue = Number.CheckRange(value, 0, AttemptLimit);
                ViewState[nameof(CurrentAttempt)] = attemptValue;
                IsAttemptLimitExceeded = AttemptLimit > 0 && attemptValue >= AttemptLimit;
            }
        }

        public int TimeLimit
        {
            get => (int)(ViewState[nameof(TimeLimit)] ?? 0);
            set => ViewState[nameof(TimeLimit)] = Number.CheckRange(value, 0, MaximumRecordingTime);
        }

        public AudioBitrateMode Bitrate
        {
            get => (AudioBitrateMode)(ViewState[nameof(Bitrate)] ?? AudioBitrateMode.kb_64);
            set => ViewState[nameof(Bitrate)] = value;
        }

        private bool IsAttemptLimitExceeded
        {
            get => (bool)(ViewState[nameof(IsAttemptLimitExceeded)] ?? false);
            set => ViewState[nameof(IsAttemptLimitExceeded)] = value;
        }

        public IAudioFile AudioCapture => _audioFile;

        #endregion

        #region Fields

        private AudioClientData _clientData;
        private AudioFile _audioFile;

        #endregion

        #region Loading

        protected override void OnPreRender(EventArgs e)
        {
            _clientData = new AudioClientData
            {
                CurrentAttempt = CurrentAttempt
            };

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(InputAudio),
                "init_" + ClientID,
                $"inSite.common.inputAudio.init('{ClientID}');",
                true);

            base.OnPreRender(e);
        }

        #endregion

        #region IPostBackDataHandler

        protected override BaseClientData DeserializeClientData(string data)
        {
            return JsonConvert.DeserializeObject<AudioClientData>(data);
        }

        protected override bool LoadPostData(BaseClientData clientData)
        {
            var audioData = (AudioClientData)clientData;

            if (audioData.IsAttemptChanged = audioData.CurrentAttempt > CurrentAttempt)
                CurrentAttempt = audioData.CurrentAttempt;

            if (AttemptLimit > 0 && audioData.IsMediaCaptured && !audioData.IsAttemptChanged)
            {
                CurrentAttempt++;
                audioData.IsAttemptChanged = true;
            }

            return audioData.IsAttemptChanged;
        }

        protected override void RaisePostDataChangedEvent(BaseClientData clientData)
        {
            var audioData = (AudioClientData)clientData;

            if (audioData.IsAttemptChanged)
                OnAttemptChanged();
        }

        protected override void SetMediaFile(BaseMediaFile file, BaseClientData data)
        {
            _audioFile = new AudioFile(file);

            InitAudioFile(_audioFile, Bitrate, TimeLimit);
        }

        private static void InitAudioFile(AudioFile file, AudioBitrateMode bitrate, int timeLimit)
        {
            var data = ProbeMedia(file.MediaFile);
            if (data == null)
                return;

            if (data.Streams.Length != 1)
            {
                file.MediaFile.ValidationError = $"Unexpected stream count: {data.Streams.Length} while expected 1";
                return;
            }

            if (data.Streams[0].CodecType != "audio")
            {
                file.MediaFile.ValidationError = $"Unexpected stream type: '{data.Streams[0].CodecType}' while expected 'audio'";
                return;
            }

            file.AudioStream = data.Streams[0];
            file.Duration = FFmpeg.CalculateDuration(data.Packets);
            file.Bitrate = FFmpeg.CalculateBitrate(data.Packets.Where(x => x.CodecType == "audio"), file.Duration);

            var maxDuration = timeLimit > 0
                ? TimeSpan.FromSeconds(timeLimit + 0.5)
                : TimeSpan.FromSeconds(MaximumRecordingTime + 0.5);

            if (file.Duration >= maxDuration)
            {
                file.MediaFile.ValidationError = $"Unexpected audio capture duration: {file.Duration.Humanize()} while expected maximum duration is {maxDuration.Humanize()}";
                return;
            }

            var expectedBitrate = (int)bitrate * 1000;
            var maxBitrate = (int)(expectedBitrate * 1.1);

            if (file.Bitrate >= maxBitrate)
            {
                file.MediaFile.ValidationError = $"Unexpected audio bitrate: {file.Bitrate:n0} while expected maximum bitrate is {expectedBitrate:n0}";
                return;
            }
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "insite-input-audio input-group");
            writer.AddAttribute("data-bitrate", ((int)Bitrate).ToString());
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

            RenderTime(writer);

            if (AttemptLimit > 0)
                RenderAttempts(writer);

            RenderButtons(writer);

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

            if (Hidden)
                sb.Append("display:none;");

            return sb.Length == 0 ? null : sb.ToString();
        }

        private void RenderTime(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "input-group-text");
            writer.AddAttribute("data-time", TimeLimit.ToString());
            writer.RenderBeginTag(HtmlTextWriterTag.Span);

            {
                writer.RenderBeginTag(HtmlTextWriterTag.Span);

                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "text-body-secondary");
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.Write("Recording Time");
                    writer.RenderEndTag();
                }

                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);

                    if (TimeLimit > 0)
                    {
                        var time = TimeLimit.Seconds();
                        writer.Write($"{Math.Floor(time.TotalMinutes):00}:{time.Seconds:00}.000");
                    }
                    else
                    {
                        writer.Write("00:00.000");
                    }

                    writer.RenderEndTag();
                }

                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
                    writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, JsonConvert.SerializeObject(_clientData));
                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag();
                }

                writer.RenderEndTag();
            }

            writer.RenderEndTag();
        }

        private void RenderAttempts(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "input-group-text border-start-0");
            writer.AddAttribute("data-attempt", AttemptLimit.ToString());
            writer.RenderBeginTag(HtmlTextWriterTag.Span);

            {
                writer.RenderBeginTag(HtmlTextWriterTag.Span);

                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "text-body-secondary");
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.Write("Attempts");
                    writer.RenderEndTag();
                }

                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.Write($"{CurrentAttempt}/{AttemptLimit}");
                    writer.RenderEndTag();
                }

                writer.RenderEndTag();
            }

            writer.RenderEndTag();
        }

        private void RenderButtons(HtmlTextWriter writer)
        {
            if (AllowPause)
                RenderButton(writer, "pause", "Pause", "secondary", "pause");

            RenderButton(writer, "start", "Start", "danger", "microphone");
        }

        private void RenderButton(HtmlTextWriter writer, string action, string title, string color, string icon)
        {
            writer.AddAttribute("data-action", action);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
            writer.AddAttribute(HtmlTextWriterAttribute.Title, title);
            writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, $"btn btn-outline-{color} fs-2");
            writer.RenderBeginTag(HtmlTextWriterTag.Button);
            writer.Write($"<i class='fas fa-{icon}'></i>");
            writer.RenderEndTag();
        }

        #endregion

        #region Helper methods

        public static IAudioFile GetAudioCapture(string fileName, Stream stream, AudioBitrateMode bitrate, int timeLimit)
        {
            var mediaFile = new StreamMediaFile(null, fileName, stream);
            var audioFile = new AudioFile(mediaFile);

            InitAudioFile(audioFile, bitrate, Number.CheckRange(timeLimit, 0, MaximumRecordingTime));

            return audioFile;
        }

        public static IAudioFile GetAudioCapture(HttpPostedFile file, AudioBitrateMode bitrate, int timeLimit)
        {
            var mediaFile = new HttpMediaFile(null, file);
            var audioFile = new AudioFile(mediaFile);

            InitAudioFile(audioFile, bitrate, Number.CheckRange(timeLimit, 0, MaximumRecordingTime));

            return audioFile;
        }

        // DON'T REMOVE!
        private static string SetupSettings()
        {
            return $"inSite.common.inputAudio.settings({MaximumRecordingTime},{MaximumAttemptLimit});";
        }

        #endregion
    }
}