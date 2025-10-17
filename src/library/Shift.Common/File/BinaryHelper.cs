using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Shift.Common
{
    public static class BinaryHelper
    {
        #region Delegates

        public delegate object ReaderMethod(BinaryReader reader);

        public delegate void WriterMethod(BinaryWriter writer, object value);

        #endregion

        #region Fields

        private static readonly Dictionary<Type, WriterMethod> _writerTypeMapping = new Dictionary<Type, WriterMethod>
        {
            { typeof(int), WriteInt32AsObj },
            { typeof(long), WriteInt64AsObj },
            { typeof(string), WriteStringAsObj },
            { typeof(Guid), WriteGuidAsObj },
            { typeof(DateTime), WriteDateTimeAsObj },
            { typeof(TimeSpan), WriteTimeSpanAsObj },
            { typeof(DateTimeOffset), WriteDateTimeOffsetAsObj },
            { typeof(bool), WriteBooleanAsObj },
            { typeof(decimal), WriteDecimalAsObj },
            { typeof(double), WriteDoubleAsObj },
            { typeof(IPAddress), WriteIpAddressAsObj }
        };

        private static readonly Dictionary<Type, WriterMethod> _writerNullableTypeMapping = new Dictionary<Type, WriterMethod>
        {
            { typeof(int), WriteInt32NullableAsObj },
            { typeof(long), WriteInt64NullableAsObj },
            { typeof(string), WriteStringNullableAsObj },
            { typeof(Guid), WriteGuidNullableAsObj },
            { typeof(DateTime), WriteDateTimeNullableAsObj },
            { typeof(TimeSpan), WriteTimeSpanNullableAsObj },
            { typeof(DateTimeOffset), WriteDateTimeOffsetNullableAsObj },
            { typeof(bool), WriteBooleanNullableAsObj },
            { typeof(decimal), WriteDecimalNullableAsObj },
            { typeof(double), WriteDoubleNullableAsObj },
            { typeof(IPAddress), WriteIpAddressNullableAsObj }
        };

        private static readonly Dictionary<Type, ReaderMethod> _readerTypeMapping = new Dictionary<Type, ReaderMethod>
        {
            { typeof(int), ReadInt32AsObj },
            { typeof(long), ReadInt64AsObj },
            { typeof(string), ReadStringAsObj },
            { typeof(Guid), ReadGuidAsObj },
            { typeof(DateTime), ReadDateTimeAsObj },
            { typeof(TimeSpan), ReadTimeSpanAsObj },
            { typeof(DateTimeOffset), ReadDateTimeOffsetAsObj },
            { typeof(bool), ReadBooleanAsObj },
            { typeof(decimal), ReadDecimalAsObj },
            { typeof(double), ReadDoubleAsObj },
            { typeof(IPAddress), ReadIpAddressAsObj }
        };

        private static readonly Dictionary<Type, ReaderMethod> _readerNullableTypeMapping = new Dictionary<Type, ReaderMethod>
        {
            { typeof(int), ReadInt32NullableAsObj },
            { typeof(long), ReadInt64NullableAsObj },
            { typeof(string), ReadStringNullableAsObj },
            { typeof(Guid), ReadGuidNullableAsObj },
            { typeof(DateTime), ReadDateTimeNullableAsObj },
            { typeof(TimeSpan), ReadTimeSpanNullableAsObj },
            { typeof(DateTimeOffset), ReadDateTimeOffsetNullableAsObj },
            { typeof(bool), ReadBooleanNullableAsObj },
            { typeof(decimal), ReadDecimalNullableAsObj },
            { typeof(double), ReadDoubleNullableAsObj },
            { typeof(IPAddress), ReadIpAddressNullableAsObj }
        };

        #endregion

        #region Methods 

        public static ReaderMethod GetReaderMethod(Type t, bool isNullable) => isNullable ? _readerNullableTypeMapping[t] : _readerTypeMapping[t];

        public static WriterMethod GetWriterMethod(Type t, bool isNullable) => isNullable ? _writerNullableTypeMapping[t] : _writerTypeMapping[t];

        #endregion

        #region Methods (writers)

        private static bool WriteIsNotNull(BinaryWriter writer, object value)
        {
            var hasValue = value != null && value != DBNull.Value;

            writer.Write(hasValue);

            return hasValue;
        }

        public static void WriteInt32(BinaryWriter writer, int value)
        {
            writer.Write(value);
        }

        private static void WriteInt32AsObj(BinaryWriter writer, object value)
        {
            WriteInt32(writer, (int)value);
        }

        public static void WriteInt32Nullable(BinaryWriter writer, int? value)
        {
            if (WriteIsNotNull(writer, value))
                WriteInt32(writer, value.Value);
        }

        private static void WriteInt32NullableAsObj(BinaryWriter writer, object value)
        {
            WriteInt32Nullable(writer, (int?)value);
        }

        public static void WriteInt64(BinaryWriter writer, long value)
        {
            writer.Write(value);
        }

        private static void WriteInt64AsObj(BinaryWriter writer, object value)
        {
            WriteInt64(writer, (long)value);
        }

        public static void WriteInt64Nullable(BinaryWriter writer, long? value)
        {
            if (WriteIsNotNull(writer, value))
                WriteInt64(writer, value.Value);
        }

        private static void WriteInt64NullableAsObj(BinaryWriter writer, object value)
        {
            WriteInt64Nullable(writer, (long?)value);
        }

        public static void WriteString(BinaryWriter writer, string value)
        {
            writer.Write(value);
        }

        private static void WriteStringAsObj(BinaryWriter writer, object value)
        {
            WriteString(writer, (string)value);
        }

        public static void WriteStringNullable(BinaryWriter writer, string value)
        {
            if (WriteIsNotNull(writer, value))
                WriteString(writer, value);
        }

        private static void WriteStringNullableAsObj(BinaryWriter writer, object value)
        {
            WriteStringNullable(writer, (string)value);
        }

        public static void WriteGuid(BinaryWriter writer, Guid value)
        {
            writer.Write(value.ToByteArray());
        }

        private static void WriteGuidAsObj(BinaryWriter writer, object value)
        {
            WriteGuid(writer, (Guid)value);
        }

        public static void WriteGuidNullable(BinaryWriter writer, Guid? value)
        {
            if (WriteIsNotNull(writer, value))
                WriteGuid(writer, value.Value);
        }

        private static void WriteGuidNullableAsObj(BinaryWriter writer, object value)
        {
            WriteGuidNullable(writer, (Guid?)value);
        }

        public static void WriteDateTime(BinaryWriter writer, DateTime value)
        {
            writer.Write(value.ToBinary());
        }

        private static void WriteDateTimeAsObj(BinaryWriter writer, object value)
        {
            WriteDateTime(writer, (DateTime)value);
        }

        public static void WriteDateTimeNullable(BinaryWriter writer, DateTime? value)
        {
            if (WriteIsNotNull(writer, value))
                WriteDateTime(writer, value.Value);
        }

        private static void WriteDateTimeNullableAsObj(BinaryWriter writer, object value)
        {
            WriteDateTimeNullable(writer, (DateTime?)value);
        }

        public static void WriteTimeSpan(BinaryWriter writer, TimeSpan value)
        {
            writer.Write(value.TotalMinutes);
        }

        private static void WriteTimeSpanAsObj(BinaryWriter writer, object value)
        {
            WriteTimeSpan(writer, (TimeSpan)value);
        }

        public static void WriteTimeSpanNullable(BinaryWriter writer, TimeSpan? value)
        {
            if (WriteIsNotNull(writer, value))
                WriteTimeSpan(writer, value.Value);
        }

        private static void WriteTimeSpanNullableAsObj(BinaryWriter writer, object value)
        {
            WriteTimeSpanNullable(writer, (TimeSpan?)value);
        }

        public static void WriteDateTimeOffset(BinaryWriter writer, DateTimeOffset value)
        {
            WriteDateTime(writer, value.DateTime);
            WriteTimeSpan(writer, value.Offset);
        }

        private static void WriteDateTimeOffsetAsObj(BinaryWriter writer, object value)
        {
            WriteDateTimeOffset(writer, (DateTimeOffset)value);
        }

        public static void WriteDateTimeOffsetNullable(BinaryWriter writer, DateTimeOffset? value)
        {
            if (WriteIsNotNull(writer, value))
                WriteDateTimeOffset(writer, value.Value);
        }

        private static void WriteDateTimeOffsetNullableAsObj(BinaryWriter writer, object value)
        {
            WriteDateTimeOffsetNullable(writer, (DateTimeOffset?)value);
        }

        public static void WriteBoolean(BinaryWriter writer, bool value)
        {
            writer.Write(value);
        }

        private static void WriteBooleanAsObj(BinaryWriter writer, object value)
        {
            WriteBoolean(writer, (bool)value);
        }

        public static void WriteBooleanNullable(BinaryWriter writer, bool? value)
        {
            writer.Write((byte)(!value.HasValue ? 0 : value.Value ? 1 : 2));
        }

        private static void WriteBooleanNullableAsObj(BinaryWriter writer, object value)
        {
            WriteBooleanNullable(writer, (bool?)value);
        }

        public static void WriteDecimal(BinaryWriter writer, decimal value)
        {
            writer.Write(value);
        }

        private static void WriteDecimalAsObj(BinaryWriter writer, object value)
        {
            WriteDecimal(writer, (decimal)value);
        }

        public static void WriteDecimalNullable(BinaryWriter writer, decimal? value)
        {
            if (WriteIsNotNull(writer, value))
                WriteDecimal(writer, value.Value);
        }

        private static void WriteDecimalNullableAsObj(BinaryWriter writer, object value)
        {
            WriteDecimalNullable(writer, (decimal?)value);
        }

        public static void WriteDouble(BinaryWriter writer, double value)
        {
            writer.Write(value);
        }

        private static void WriteDoubleAsObj(BinaryWriter writer, object value)
        {
            WriteDouble(writer, (double)value);
        }

        public static void WriteDoubleNullable(BinaryWriter writer, double? value)
        {
            if (WriteIsNotNull(writer, value))
                WriteDouble(writer, value.Value);
        }

        private static void WriteDoubleNullableAsObj(BinaryWriter writer, object value)
        {
            WriteDoubleNullable(writer, (double?)value);
        }

        public static void WriteIpAddress(BinaryWriter writer, IPAddress value)
        {
            var data = value.GetAddressBytes();
            writer.Write((byte)data.Length);
            writer.Write(data);
        }

        private static void WriteIpAddressAsObj(BinaryWriter writer, object value)
        {
            WriteIpAddress(writer, (IPAddress)value);
        }

        public static void WriteIpAddressNullable(BinaryWriter writer, IPAddress value)
        {
            if (WriteIsNotNull(writer, value))
                WriteIpAddress(writer, value);
        }

        private static void WriteIpAddressNullableAsObj(BinaryWriter writer, object value)
        {
            WriteIpAddressNullable(writer, (IPAddress)value);
        }

        #endregion

        #region Methods (reader)

        private static bool ReadIsNotNull(BinaryReader reader)
        {
            return reader.ReadBoolean();
        }

        public static int ReadInt32(BinaryReader reader)
        {
            return reader.ReadInt32();
        }

        private static object ReadInt32AsObj(BinaryReader reader)
        {
            return ReadInt32(reader);
        }

        public static int? ReadInt32Nullable(this BinaryReader reader)
        {
            return ReadIsNotNull(reader)
                ? ReadInt32(reader)
                : (int?)null;
        }

        private static object ReadInt32NullableAsObj(BinaryReader reader)
        {
            return ReadInt32Nullable(reader);
        }

        public static long ReadInt64(BinaryReader reader)
        {
            return reader.ReadInt64();
        }

        private static object ReadInt64AsObj(BinaryReader reader)
        {
            return ReadInt64(reader);
        }

        public static long? ReadInt64Nullable(this BinaryReader reader)
        {
            return ReadIsNotNull(reader)
                ? ReadInt64(reader)
                : (long?)null;
        }

        private static object ReadInt64NullableAsObj(BinaryReader reader)
        {
            return ReadInt64Nullable(reader);
        }

        public static string ReadString(BinaryReader reader)
        {
            return reader.ReadString();
        }

        private static string ReadStringAsObj(BinaryReader reader)
        {
            return ReadString(reader);
        }

        public static string ReadStringNullable(this BinaryReader reader)
        {
            return ReadIsNotNull(reader)
                ? ReadString(reader)
                : (string)null;
        }

        private static object ReadStringNullableAsObj(BinaryReader reader)
        {
            return ReadStringNullable(reader);
        }

        public static Guid ReadGuid(this BinaryReader reader)
        {
            return new Guid(reader.ReadBytes(16));
        }

        private static object ReadGuidAsObj(BinaryReader reader)
        {
            return ReadGuid(reader);
        }

        public static Guid? ReadGuidNullable(this BinaryReader reader)
        {
            return ReadIsNotNull(reader)
                ? ReadGuid(reader)
                : (Guid?)null;
        }

        private static object ReadGuidNullableAsObj(BinaryReader reader)
        {
            return ReadGuidNullable(reader);
        }

        public static DateTime ReadDateTime(this BinaryReader reader)
        {
            return DateTime.FromBinary(reader.ReadInt64());
        }

        private static object ReadDateTimeAsObj(BinaryReader reader)
        {
            return ReadDateTime(reader);
        }

        public static DateTime? ReadDateTimeNullable(this BinaryReader reader)
        {
            return ReadIsNotNull(reader)
                ? ReadDateTime(reader)
                : (DateTime?)null;
        }

        private static object ReadDateTimeNullableAsObj(BinaryReader reader)
        {
            return ReadDateTimeNullable(reader);
        }

        public static TimeSpan ReadTimeSpan(this BinaryReader reader)
        {
            return TimeSpan.FromMinutes(reader.ReadDouble());
        }

        private static object ReadTimeSpanAsObj(BinaryReader reader)
        {
            return ReadTimeSpan(reader);
        }

        public static TimeSpan? ReadTimeSpanNullable(this BinaryReader reader)
        {
            return ReadIsNotNull(reader)
                ? ReadTimeSpan(reader)
                : (TimeSpan?)null;
        }

        private static object ReadTimeSpanNullableAsObj(BinaryReader reader)
        {
            return ReadTimeSpanNullable(reader);
        }

        public static DateTimeOffset ReadDateTimeOffset(this BinaryReader reader)
        {
            return new DateTimeOffset(ReadDateTime(reader), ReadTimeSpan(reader));
        }

        private static object ReadDateTimeOffsetAsObj(BinaryReader reader)
        {
            return ReadDateTimeOffset(reader);
        }

        public static DateTimeOffset? ReadDateTimeOffsetNullable(this BinaryReader reader)
        {
            return ReadIsNotNull(reader)
                ? ReadDateTimeOffset(reader)
                : (DateTimeOffset?)null;
        }

        private static object ReadDateTimeOffsetNullableAsObj(BinaryReader reader)
        {
            return ReadDateTimeOffsetNullable(reader);
        }

        public static bool ReadBoolean(BinaryReader reader)
        {
            return reader.ReadBoolean();
        }

        private static object ReadBooleanAsObj(BinaryReader reader)
        {
            return ReadBoolean(reader);
        }

        public static bool? ReadBooleanNullable(this BinaryReader reader)
        {
            var value = reader.ReadByte();

            return value == 0 ? (bool?)null : value == 1;
        }

        private static object ReadBooleanNullableAsObj(BinaryReader reader)
        {
            return ReadBooleanNullable(reader);
        }

        public static decimal ReadDecimal(BinaryReader reader)
        {
            return reader.ReadDecimal();
        }

        private static object ReadDecimalAsObj(BinaryReader reader)
        {
            return ReadDecimal(reader);
        }

        public static decimal? ReadDecimalNullable(this BinaryReader reader)
        {
            return ReadIsNotNull(reader)
                ? ReadDecimal(reader)
                : (decimal?)null;
        }

        private static object ReadDecimalNullableAsObj(BinaryReader reader)
        {
            return ReadDecimalNullable(reader);
        }

        public static double ReadDouble(BinaryReader reader)
        {
            return reader.ReadDouble();
        }

        private static object ReadDoubleAsObj(BinaryReader reader)
        {
            return ReadDouble(reader);
        }

        public static double? ReadDoubleNullable(this BinaryReader reader)
        {
            return ReadIsNotNull(reader)
                ? ReadDouble(reader)
                : (double?)null;
        }

        private static object ReadDoubleNullableAsObj(BinaryReader reader)
        {
            return ReadDoubleNullable(reader);
        }

        public static IPAddress ReadIpAddress(this BinaryReader reader)
        {
            return new IPAddress(reader.ReadBytes((int)reader.ReadByte()));
        }

        private static object ReadIpAddressAsObj(BinaryReader reader)
        {
            return ReadIpAddress(reader);
        }

        public static IPAddress ReadIpAddressNullable(this BinaryReader reader)
        {
            return ReadIsNotNull(reader)
                ? ReadIpAddress(reader)
                : null;
        }

        private static object ReadIpAddressNullableAsObj(BinaryReader reader)
        {
            return ReadIpAddressNullable(reader);
        }

        #endregion

        #region Extensions (writer)

        public static void WriteNullable(this BinaryWriter writer, int? value)
        {
            WriteInt32Nullable(writer, value);
        }

        public static void WriteNullable(this BinaryWriter writer, long? value)
        {
            WriteInt64Nullable(writer, value);
        }

        public static void WriteNullable(this BinaryWriter writer, string value)
        {
            WriteStringNullable(writer, value);
        }

        public static void Write(this BinaryWriter writer, Guid value)
        {
            WriteGuid(writer, value);
        }

        public static void WriteNullable(this BinaryWriter writer, Guid? value)
        {
            WriteGuidNullable(writer, value);
        }

        public static void Write(this BinaryWriter writer, DateTime value)
        {
            WriteDateTime(writer, value);
        }

        public static void WriteNullable(this BinaryWriter writer, DateTime? value)
        {
            WriteDateTimeNullable(writer, value);
        }

        public static void Write(this BinaryWriter writer, TimeSpan value)
        {
            WriteTimeSpan(writer, value);
        }

        public static void WriteNullable(this BinaryWriter writer, TimeSpan? value)
        {
            WriteTimeSpanNullable(writer, value);
        }

        public static void Write(this BinaryWriter writer, DateTimeOffset value)
        {
            WriteDateTimeOffset(writer, value);
        }

        public static void WriteNullable(this BinaryWriter writer, DateTimeOffset? value)
        {
            WriteDateTimeOffsetNullable(writer, value);
        }

        public static void WriteNullable(this BinaryWriter writer, bool? value)
        {
            WriteBooleanNullable(writer, value);
        }

        public static void WriteNullable(this BinaryWriter writer, decimal? value)
        {
            WriteDecimalNullable(writer, value);
        }

        public static void WriteNullable(this BinaryWriter writer, double? value)
        {
            WriteDoubleNullable(writer, value);
        }

        public static void Write(this BinaryWriter writer, IPAddress value)
        {
            WriteIpAddress(writer, value);
        }

        public static void WriteNullable(this BinaryWriter writer, IPAddress value)
        {
            WriteIpAddressNullable(writer, value);
        }

        #endregion
    }
}
