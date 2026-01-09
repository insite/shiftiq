using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Shift.Common.File;

using IOFile = System.IO.File;

namespace Shift.Common
{
    public class FFmpeg
    {
        #region Constants

        private const int ProbeTimeout = 15000;

        [Flags]
        public enum ProbeType
        {
            Format = 1,
            Streams = 2,
            Packets = 4,
            Frames = 8
        }

        #endregion

        #region Interfaces (probe result)

        public interface IProbeResult
        {
            IProbeFormat Format { get; }
            IProbeStream[] Streams { get; }
            IProbePacket[] Packets { get; }
            IProbeFrame[] Frames { get; }
        }

        public interface IProbeEntity
        {
            IReadOnlyDictionary<string, string> Other { get; }
        }

        public interface IProbeFormat : IProbeEntity
        {
            string FileName { get; }
            int StreamsCount { get; }
            int ProgramsCount { get; }
            string ShortName { get; }
            string LongName { get; }
            TimeSpan? StartTime { get; }
            TimeSpan? Duration { get; }
            int? Size { get; }
            int? Bitrate { get; }
        }

        public interface IProbeStream : IProbeEntity
        {
            int Index { get; }
            string CodecShortName { get; }
            string CodecLongName { get; }
            string Profile { get; }
            string CodecType { get; }
            TimeSpan? StartTime { get; }
            TimeSpan? Duration { get; }
            int? Bitrate { get; }
            int? MaxBitrate { get; }

            IProbeAudioStream Audio { get; }
            IProbeVideoStream Video { get; }
        }

        public interface IProbeAudioStream
        {
            string SampleFormat { get; }
            int SampleRate { get; }
            int ChannelsCount { get; }
            string ChannelLayout { get; }
        }

        public interface IProbeVideoStream
        {
            int? Width { get; }
            int? Height { get; }
            int? CodedWidth { get; }
            int? CodedHeight { get; }
            string SampleAspectRatio { get; }
            string DisplayAspectRatio { get; }
            string PixelFormat { get; }
            string ColorSpace { get; }
        }

        public interface IProbePacket : IProbeEntity
        {
            string CodecType { get; }
            int StreamIndex { get; }
            TimeSpan? PresentationTimestamp { get; }
            TimeSpan? DecodingTimestamp { get; }
            TimeSpan? Duration { get; }
            long? Position { get; }
            long? Size { get; }
        }

        public interface IProbeFrame : IProbeEntity
        {
            string MediaType { get; }
            int StreamIndex { get; }
            int KeyFrame { get; }
            TimeSpan? PresentationTimestamp { get; }
            TimeSpan? PacketDecodingTimestamp { get; }
            TimeSpan? BestEffortTimestamp { get; }
            TimeSpan? PacketDuration { get; }
            TimeSpan? Duration { get; }
            long? PacketPosition { get; }
            int? PacketSize { get; }

            IProbeAudioFrame Audio { get; }
            IProbeVideoFrame Video { get; }
        }

        public interface IProbeAudioFrame
        {
            string SampleFormat { get; }
            string SamplesCount { get; }
            int Channels { get; }
            string ChannelLayout { get; }
        }

        public interface IProbeVideoFrame
        {
            int Width { get; }
            int Height { get; }
            string PixelFormat { get; }
            string SampleAspectRatio { get; }
            string ColorSpace { get; }
        }

        #endregion

        #region Classes (probe result)

        private class ProbeResult : IProbeResult
        {
            public IProbeFormat Format { get; set; }
            public IProbeStream[] Streams { get; set; }
            public IProbePacket[] Packets { get; set; }
            public IProbeFrame[] Frames { get; set; }
        }

        private abstract class ProbeEntity
        {
            public IReadOnlyDictionary<string, string> Other => _otherReadonly;

            private Dictionary<string, string> _other;
            private IReadOnlyDictionary<string, string> _otherReadonly;

            public ProbeEntity(string data, int offset, bool saveOther)
            {
                if (saveOther)
                {
                    _other = new Dictionary<string, string>();
                    _otherReadonly = new ReadOnlyDictionary<string, string>(_other);
                }

                int startIndex = offset, separatorIndex, endIndex;
                string name, value;

                while (startIndex != 0)
                {
                    separatorIndex = data.IndexOf('=', startIndex);
                    endIndex = data.IndexOf('|', startIndex);

                    name = data.Substring(startIndex, separatorIndex - startIndex);
                    value = endIndex == -1
                        ? data.Substring(separatorIndex + 1)
                        : data.Substring(separatorIndex + 1, endIndex - separatorIndex - 1);

                    if (value == "N/A")
                        value = null;

                    if (!Set(name, value) && saveOther)
                        _other.Add(name, value);

                    startIndex = endIndex + 1;
                }
            }

            protected abstract bool Set(string name, string value);

            protected static int? GetInt32(string value) => value == null ? (int?)null : int.Parse(value);

            protected static long? GetInt64(string value) => value == null ? (long?)null : long.Parse(value);

            protected static decimal? GetDecimal(string value) => value == null ? (decimal?)null : decimal.Parse(value);

            protected static bool? GetBoolean(string value) => value == "0" ? false : value == "1" ? true : (bool?)null;

            protected static TimeSpan? GetTimeSpan(string value) => value == null ? (TimeSpan?)null : TimeSpan.FromTicks((long)(decimal.Parse(value) * TimeSpan.TicksPerSecond));
        }

        private class ProbeFormat : ProbeEntity, IProbeFormat
        {
            public string FileName { get; set; }
            public int StreamsCount { get; set; }
            public int ProgramsCount { get; set; }
            public string ShortName { get; set; }
            public string LongName { get; set; }
            public TimeSpan? StartTime { get; set; }
            public TimeSpan? Duration { get; set; }
            public int? Size { get; set; }
            public int? Bitrate { get; set; }

            public ProbeFormat(string data, int offset, bool saveOther) : base(data, offset, saveOther)
            {
            }

            protected override bool Set(string name, string value)
            {
                if (name == "filename")
                    FileName = value;

                else if (name == "nb_streams")
                    StreamsCount = int.Parse(value);

                else if (name == "nb_programs")
                    ProgramsCount = int.Parse(value);

                else if (name == "format_name")
                    ShortName = value;

                else if (name == "format_long_name")
                    LongName = value;

                else if (name == "start_time")
                    StartTime = GetTimeSpan(value);

                else if (name == "duration")
                    Duration = GetTimeSpan(value);

                else if (name == "size")
                    Size = GetInt32(value);

                else if (name == "bit_rate")
                    Bitrate = GetInt32(value);

                else
                    return false;

                return true;
            }
        }

        private class ProbeStream : ProbeEntity, IProbeStream
        {
            public int Index { get; set; }
            public string CodecShortName { get; set; }
            public string CodecLongName { get; set; }
            public string Profile { get; set; }
            public string CodecType { get; set; }
            public TimeSpan? StartTime { get; set; }
            public TimeSpan? Duration { get; set; }
            public int? Bitrate { get; set; }
            public int? MaxBitrate { get; set; }

            public ProbeAudioStream Audio { get; } = new ProbeAudioStream();
            public ProbeVideoStream Video { get; } = new ProbeVideoStream();

            IProbeAudioStream IProbeStream.Audio => Audio;
            IProbeVideoStream IProbeStream.Video => Video;

            public ProbeStream(string data, int offset, bool saveOther) : base(data, offset, saveOther)
            {
            }

            protected override bool Set(string name, string value)
            {
                if (name == "index")
                    Index = int.Parse(value);

                else if (name == "codec_name")
                    CodecShortName = value;

                else if (name == "codec_long_name")
                    CodecLongName = value;

                else if (name == "profile")
                    Profile = value;

                else if (name == "codec_type")
                    CodecType = value;

                else if (name == "start_time")
                    StartTime = GetTimeSpan(value);

                else if (name == "duration")
                    Duration = GetTimeSpan(value);

                else if (name == "bit_rate")
                    Bitrate = GetInt32(value);

                else if (name == "max_bit_rate")
                    MaxBitrate = GetInt32(value);

                if (name == "sample_fmt")
                    Audio.SampleFormat = value;

                else if (name == "sample_rate")
                    Audio.SampleRate = GetInt32(value).Value;

                else if (name == "channels")
                    Audio.ChannelsCount = GetInt32(value).Value;

                else if (name == "channel_layout")
                    Audio.ChannelLayout = value;

                else if (name == "width")
                    Video.Width = GetInt32(value);

                else if (name == "height")
                    Video.Height = GetInt32(value);

                else if (name == "coded_width")
                    Video.CodedWidth = GetInt32(value);

                else if (name == "coded_height")
                    Video.CodedHeight = GetInt32(value);

                else if (name == "sample_aspect_ratio")
                    Video.SampleAspectRatio = value;

                else if (name == "display_aspect_ratio")
                    Video.DisplayAspectRatio = value;

                else if (name == "pix_fmt")
                    Video.PixelFormat = value;

                else if (name == "color_space")
                    Video.ColorSpace = value;

                else
                    return false;

                return true;
            }
        }

        private class ProbeAudioStream : IProbeAudioStream
        {
            public string SampleFormat { get; set; }
            public int SampleRate { get; set; }
            public int ChannelsCount { get; set; }
            public string ChannelLayout { get; set; }
        }

        private class ProbeVideoStream : IProbeVideoStream
        {
            public int? Width { get; set; }
            public int? Height { get; set; }
            public int? CodedWidth { get; set; }
            public int? CodedHeight { get; set; }
            public string SampleAspectRatio { get; set; }
            public string DisplayAspectRatio { get; set; }
            public string PixelFormat { get; set; }
            public string ColorSpace { get; set; }
        }

        private class ProbePacket : ProbeEntity, IProbePacket
        {
            public string CodecType { get; set; }
            public int StreamIndex { get; set; }
            public TimeSpan? PresentationTimestamp { get; set; }
            public TimeSpan? DecodingTimestamp { get; set; }
            public TimeSpan? Duration { get; set; }
            public long? Position { get; set; }
            public long? Size { get; set; }

            public ProbePacket(string data, int offset, bool saveOther) : base(data, offset, saveOther)
            {
            }

            protected override bool Set(string name, string value)
            {
                if (name == "codec_type")
                    CodecType = value;

                else if (name == "stream_index")
                    StreamIndex = int.Parse(value);

                else if (name == "pts_time")
                    PresentationTimestamp = GetTimeSpan(value);

                else if (name == "dts_time")
                    DecodingTimestamp = GetTimeSpan(value);

                else if (name == "duration_time")
                    Duration = GetTimeSpan(value);

                else if (name == "size")
                    Size = GetInt64(value);

                else if (name == "pos")
                    Position = GetInt64(value);

                else
                    return false;

                return true;
            }
        }

        private class ProbeFrame : ProbeEntity, IProbeFrame
        {
            public string MediaType { get; set; }
            public int StreamIndex { get; set; }
            public int KeyFrame { get; set; }
            public TimeSpan? PresentationTimestamp { get; set; }
            public TimeSpan? PacketDecodingTimestamp { get; set; }
            public TimeSpan? BestEffortTimestamp { get; set; }
            public TimeSpan? PacketDuration { get; set; }
            public TimeSpan? Duration { get; set; }
            public long? PacketPosition { get; set; }
            public int? PacketSize { get; set; }

            public ProbeAudioFrame Audio { get; } = new ProbeAudioFrame();
            public ProbeVideoFrame Video { get; } = new ProbeVideoFrame();

            IProbeAudioFrame IProbeFrame.Audio => Audio;
            IProbeVideoFrame IProbeFrame.Video => Video;

            public ProbeFrame(string data, int offset, bool saveOther) : base(data, offset, saveOther)
            {
            }

            protected override bool Set(string name, string value)
            {
                if (name == "media_type")
                    MediaType = value;

                else if (name == "stream_index")
                    StreamIndex = int.Parse(value);

                else if (name == "key_frame")
                    KeyFrame = int.Parse(value);

                else if (name == "pts_time")
                    PresentationTimestamp = GetTimeSpan(value);

                else if (name == "pkt_dts_time")
                    PacketDecodingTimestamp = GetTimeSpan(value);

                else if (name == "best_effort_timestamp_time")
                    BestEffortTimestamp = GetTimeSpan(value);

                else if (name == "pkt_duration_time")
                    PacketDuration = GetTimeSpan(value);

                else if (name == "duration_time")
                    Duration = GetTimeSpan(value);

                else if (name == "pkt_pos")
                    PacketPosition = GetInt64(value);

                else if (name == "pkt_size")
                    PacketSize = GetInt32(value);

                else if (name == "sample_fmt")
                    Audio.SampleFormat = value;

                else if (name == "nb_samples")
                    Audio.SamplesCount = value;

                else if (name == "channels")
                    Audio.Channels = int.Parse(value);

                else if (name == "channel_layout")
                    Audio.ChannelLayout = value;

                else if (name == "width")
                    Video.Width = int.Parse(value);

                else if (name == "height")
                    Video.Height = int.Parse(value);

                else if (name == "pix_fmt")
                    Video.PixelFormat = value;

                else if (name == "sample_aspect_ratio")
                    Video.SampleAspectRatio = value;

                else if (name == "color_space")
                    Video.ColorSpace = value;

                else
                    return false;

                return true;
            }
        }

        private class ProbeAudioFrame : IProbeAudioFrame
        {
            public string SampleFormat { get; set; }
            public string SamplesCount { get; set; }
            public int Channels { get; set; }
            public string ChannelLayout { get; set; }
        }

        private class ProbeVideoFrame : IProbeVideoFrame
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public string PixelFormat { get; set; }
            public string SampleAspectRatio { get; set; }
            public string ColorSpace { get; set; }
        }

        #endregion

        #region Classes (probe other)

        private class ProbeParser
        {
            private bool _saveOther;

            private ProbeFormat _format;
            private List<ProbeStream> _streams = new List<ProbeStream>();
            private List<ProbePacket> _packets = new List<ProbePacket>();
            private List<ProbeFrame> _frames = new List<ProbeFrame>();

            public ProbeParser(bool saveOther)
            {
                _saveOther = saveOther;
            }

            public void Read(string data)
            {
                var index = data.IndexOf('|');
                if (index == -1)
                    throw InvalidDataLineException(data);

                var name = data.Substring(0, index);

                if (name == "format")
                    _format = new ProbeFormat(data, index + 1, _saveOther);
                else if (name == "stream")
                    _streams.Add(new ProbeStream(data, index + 1, _saveOther));
                else if (name == "packet")
                    _packets.Add(new ProbePacket(data, index + 1, _saveOther));
                else if (name == "frame")
                    _frames.Add(new ProbeFrame(data, index + 1, _saveOther));
                else
                    throw InvalidDataLineException(data);
            }

            public IProbeResult GetResult()
            {
                var result = new ProbeResult();

                if (_format != null)
                    result.Format = _format;

                if (_streams.IsNotEmpty())
                    result.Streams = _streams.ToArray();

                if (_packets.IsNotEmpty())
                    result.Packets = _packets.ToArray();

                if (_frames.IsNotEmpty())
                    result.Frames = _frames.ToArray();

                return result;
            }

            private static ApplicationError InvalidDataLineException(string data) =>
                ApplicationError.Create("Invalid data line: " + data);
        }

        #endregion

        #region Fields & properties

        private readonly string _ffmpegToolPath;

        private string _ffprobePath;
        private string FFProbePath
        {
            get
            {
                if (string.IsNullOrEmpty(_ffprobePath))
                {
                    if (_ffmpegToolPath.IsEmpty())
                        throw ApplicationError.Create("FFmpeg.ToolPath is not defined");

                    _ffprobePath = Path.Combine(_ffmpegToolPath, "ffprobe.exe");
                    if (!IOFile.Exists(_ffprobePath))
                        throw ApplicationError.Create("File not found: " + _ffprobePath);
                }
                return _ffprobePath;
            }
        }

        #endregion

        #region Construction

        public FFmpeg(string ffmpegToolPath)
        {
            _ffmpegToolPath = ffmpegToolPath;
        }

        #endregion

        #region Methods (probe)

        public IProbeResult Probe(Stream stream, ProbeType probeType)
        {
            var isArgsValid = false;
            var args = "-v warning -of compact";

            if (probeType.HasFlag(ProbeType.Format))
            {
                args += " -show_format";
                isArgsValid = true;
            }

            if (probeType.HasFlag(ProbeType.Streams))
            {
                args += " -show_streams";
                isArgsValid = true;
            }

            if (probeType.HasFlag(ProbeType.Packets))
            {
                args += " -show_packets";
                isArgsValid = true;
            }

            if (probeType.HasFlag(ProbeType.Frames))
            {
                args += " -show_frames";
                isArgsValid = true;
            }

            if (!isArgsValid)
                throw ApplicationError.Create("Invalid probe type");

            return Probe(stream, args);
        }

        private IProbeResult Probe(Stream stream, string args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = FFProbePath,
                WorkingDirectory = Path.GetDirectoryName(FFProbePath),
                StandardOutputEncoding = Encoding.Default,
                StandardErrorEncoding = Encoding.Default,
            };

            var parser = new ProbeParser(false);

            PipeHelper.CreatePipe("ffprobe", null, ProbeTimeout, stream, async (path, token) =>
            {
                psi.Arguments = args + $" \"{path}\"";

                using (var process = AsyncProcess.Start(psi, parser.Read, token))
                {
                    await process.WaitForExitAsync();

                    if (process.ExitCode != 0)
                    {
                        var message = $"The process exit code is {process.ExitCode}.";

                        if (process.StandardError.Count > 0)
                            message += System.Environment.NewLine + string.Join(System.Environment.NewLine, process.StandardError);

                        throw ApplicationError.Create(message);
                    }
                }
            });

            return parser.GetResult();
        }

        public static TimeSpan CalculateDuration(IEnumerable<IProbePacket> packets)
        {
            var lastPacket = packets
                .Where(x => x.DecodingTimestamp.HasValue && x.DecodingTimestamp.Value.Ticks > 0 && x.Duration.HasValue && x.Duration.Value.Ticks > 0)
                .OrderByDescending(x => x.DecodingTimestamp.Value)
                .FirstOrDefault();

            return lastPacket == null
                ? TimeSpan.Zero
                : lastPacket.DecodingTimestamp.Value + lastPacket.Duration.Value;
        }

        public static TimeSpan CalculateDuration(IEnumerable<IProbeFrame> frames)
        {
            var lastFrame = frames
                .Where(x => x.PacketDecodingTimestamp.HasValue && x.PacketDecodingTimestamp.Value.Ticks > 0 && x.PacketDuration.HasValue && x.PacketDuration.Value.Ticks > 0)
                .OrderByDescending(x => x.PacketDecodingTimestamp.Value)
                .FirstOrDefault();

            return lastFrame == null
                ? TimeSpan.Zero
                : lastFrame.PacketDecodingTimestamp.Value + lastFrame.PacketDuration.Value;
        }

        public static int CalculateBitrate(IEnumerable<IProbePacket> packets, TimeSpan duration)
        {
            var dataSize = packets
                .Where(x => x.DecodingTimestamp.HasValue
                         && x.Size.HasValue
                         && x.Size.Value > 0)
                .Sum(x => x.Size.Value);

            return CalculateBitrate(dataSize, duration);
        }

        public static int CalculateBitrate(IEnumerable<IProbeFrame> frames, TimeSpan duration)
        {
            var dataSize = frames
                .Where(x => x.PacketDecodingTimestamp.HasValue
                         && x.PacketSize.HasValue
                         && x.PacketSize.Value > 0)
                .Sum(x => x.PacketSize.Value);

            return CalculateBitrate(dataSize, duration);
        }

        public static int CalculateBitrate(long dataSize, TimeSpan duration)
        {
            if (dataSize == 0)
                return -1;

            var seconds = duration.TotalSeconds;
            if (seconds < 1)
                seconds = 1;

            return (int)Math.Round(dataSize * 8 / seconds, MidpointRounding.AwayFromZero);
        }

        #endregion
    }
}
