using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Shift.Common
{
    public enum ExceptionActionType { Undefined, Ignore, Warning, Error }

    public abstract class ExceptionAction
    {
        public abstract ExceptionActionType Type { get; }
        public abstract string RedirectUrl { get; }
        public abstract bool ClearError { get; }
    }

    public class CustomExceptionAction : ExceptionAction
    {
        public static ExceptionAction Undefined { get; } = new CustomExceptionAction(ExceptionActionType.Undefined, null, false);

        public override ExceptionActionType Type { get; }
        public override string RedirectUrl { get; }
        public override bool ClearError { get; }

        public CustomExceptionAction(ExceptionActionType type, string redirectUrl, bool clearError)
        {
            Type = type;
            RedirectUrl = redirectUrl;
            ClearError = clearError;
        }
    }

    public class ExceptionHandlerAction : ExceptionAction
    {
        public override ExceptionActionType Type => _handler.Action;
        public override string RedirectUrl => _handler.RedirectUrl;
        public override bool ClearError => _handler.ClearError;

        private ExceptionHandler _handler;

        public ExceptionHandlerAction(ExceptionHandler handler)
        {
            _handler = handler;
        }
    }

    public class ExceptionHandler
    {
        #region Properties

        public ExceptionActionType Action { get; private set; }
        public string RedirectUrl { get; private set; }
        public bool ClearError { get; private set; }

        #endregion

        #region Fields

        private Type _typeEquals;
        private Type _typeAssignableFrom;
        private string _messageStartsWith;
        private Regex _messagePattern;
        private string _messageContains;
        private string _messageEquals;
        private string _requestPath;
        private Func<ExceptionHandler, ExceptionHandlerData, bool>[] _conditions;

        #endregion

        #region Construction

        private ExceptionHandler()
        {

        }

        #endregion

        #region Methods (match)

        public static ExceptionAction GetAction(Exception ex, string requestPath, IEnumerable<ExceptionHandler> handlers)
        {
            var data = new ExceptionHandlerData(ex, requestPath);

            foreach (var handler in handlers)
            {
                if (handler.IsMatch(data))
                    return new ExceptionHandlerAction(handler);
            }

            return ex.InnerException != null
                ? GetAction(ex.InnerException, requestPath, handlers)
                : CustomExceptionAction.Undefined;
        }

        public bool IsMatch(ExceptionHandlerData data) => _conditions.All(fn => fn(this, data));

        #endregion

        #region Methods (initialization)

        public static ExceptionHandler[] FromArray(ExceptionHandlerSettings[] infos)
        {
            var result = new List<ExceptionHandler>();

            foreach (var info in infos.EmptyIfNull())
            {
                if (TryCreate(info, out var handler))
                    result.Add(handler);
            }

            return result.ToArray();
        }

        public static bool TryCreate(ExceptionHandlerSettings info, out ExceptionHandler handler)
        {
            handler = new ExceptionHandler
            {
                Action = info.Action.ToEnum(ExceptionActionType.Undefined),
                RedirectUrl = info.RedirectUrl.NullIfEmpty(),
                ClearError = info.ClearError ?? false,

                _typeEquals = GetType(info.TypeEquals),
                _typeAssignableFrom = GetType(info.TypeAssignableFrom),
                _messageStartsWith = info.MessageStartsWith.NullIfEmpty(),
                _messagePattern = info.MessagePattern.IsNotEmpty()
                    ? new Regex(info.MessagePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase)
                    : null,
                _messageContains = info.MessageContains.NullIfEmpty(),
                _messageEquals = info.MessageEquals.NullIfEmpty(),
                _requestPath = info.RequestPath.NullIfEmpty(),
            };

            if (handler.Action == ExceptionActionType.Undefined)
                return false;

            handler._conditions = CreateConditionChain(handler);

            if (handler._conditions.Length == 0)
            {
                handler = null;
                return false;
            }

            return true;

            Type GetType(string typeName)
            {
                if (typeName.IsEmpty())
                    return null;

                var result = BuildManagerProxy.GetType(typeName, true);

                if (typeof(Exception).IsAssignableFrom(result))
                    return result;

                throw new Exception("The type must be assignable from System.Exception");
            }
        }

        private static Func<ExceptionHandler, ExceptionHandlerData, bool>[] CreateConditionChain(ExceptionHandler handler)
        {
            var conditions = new List<Func<ExceptionHandler, ExceptionHandlerData, bool>>();

            if (handler._typeEquals != null)
                conditions.Add(TypeEquals);

            if (handler._typeAssignableFrom != null)
                conditions.Add(TypeAssignableFrom);

            if (handler._messageStartsWith != null)
                conditions.Add(MessageStartsWith);

            if (handler._messagePattern != null)
                conditions.Add(MessagePatternMatch);

            if (handler._messageContains != null)
                conditions.Add(MessageContains);

            if (handler._messageEquals != null)
                conditions.Add(MessageEquals);

            if (handler._requestPath != null)
                conditions.Add(RequestPathEquals);

            return conditions.ToArray();
        }

        private static bool TypeEquals(ExceptionHandler handler, ExceptionHandlerData data) =>
            data.Type == handler._typeEquals;

        private static bool TypeAssignableFrom(ExceptionHandler handler, ExceptionHandlerData data) =>
            handler._typeAssignableFrom.IsAssignableFrom(data.Type);

        private static bool MessageStartsWith(ExceptionHandler handler, ExceptionHandlerData data) =>
            data.Message.StartsWith(handler._messageStartsWith, StringComparison.OrdinalIgnoreCase);

        private static bool MessagePatternMatch(ExceptionHandler handler, ExceptionHandlerData data) =>
            handler._messagePattern.IsMatch(data.Message);

        private static bool MessageContains(ExceptionHandler handler, ExceptionHandlerData data) =>
            data.Message.Contains(handler._messageContains, StringComparison.OrdinalIgnoreCase);

        private static bool MessageEquals(ExceptionHandler handler, ExceptionHandlerData data) =>
            data.Message.Equals(handler._messageEquals, StringComparison.OrdinalIgnoreCase);

        private static bool RequestPathEquals(ExceptionHandler handler, ExceptionHandlerData data) =>
            data.RequestPath.Equals(handler._requestPath, StringComparison.OrdinalIgnoreCase);

        #endregion
    }
}