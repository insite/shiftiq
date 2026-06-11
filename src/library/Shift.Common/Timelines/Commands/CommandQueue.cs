using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Shift.Common.Timeline.Exceptions;
using Shift.Common.Timeline.Services;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Shift.Common;

namespace Shift.Common.Timeline.Commands
{
    /// <summary>
    /// Implements a basic command queue. The purpose of the queue is to route commands to subscriber methods; 
    /// validation of a command itself is the responsibility of its subscriber/handler.
    /// </summary>
    public class CommandQueue : ICommandQueue
    {
        private readonly IJsonSerializer _serializer;

        /// <summary>
        /// A command's full class name is used as the key to find the method that handles it.
        /// </summary>
        readonly Dictionary<string, Action<ICommand>> _subscribers;

        /// <summary>
        /// In a multi-organization system we need to allow an individual organization to override/customize the handling of a 
        /// command. In this case the class name and the organization identifier are used together as the unique key.
        /// </summary>
        readonly Dictionary<(string, Guid), Action<ICommand>> _overriders;

        /// <summary>
        /// Scheduled commands must be stored. Unscheduled commands can be stored, but this is optional.
        /// </summary>
        readonly ICommandStore _store;

        /// <summary>
        /// True if all commands (scheduled and unscheduled) are logged to the command store. False if only scheduled 
        /// commands are logged.
        /// </summary>
        readonly bool _saveAll;

        /// <summary>
        /// Constructs the queue.
        /// </summary>
        public CommandQueue(ICommandStore store, bool saveAll = false)
        {
            _subscribers = new Dictionary<string, Action<ICommand>>();
            _overriders = new Dictionary<(string, Guid), Action<ICommand>>();
            _store = store;
            _saveAll = saveAll;

            _serializer = ServiceLocator.Instance.GetService<IJsonSerializer>();
        }

        #region Methods (subscription)

        /// <summary>
        /// One and only one subscriber can register for each command. If a command is sent then it must have a handler.
        /// </summary>
        public void Subscribe<T>(Action<T> action) where T : ICommand
        {
            var name = _serializer.GetClassName(typeof(T));

            if (_subscribers.Any(x => x.Key == name))
                throw new AmbiguousCommandHandlerException(name);

            _subscribers.Add(name, (command) => action((T)command));
        }

        /// <summary>
        /// Registers a organization-specific custom handler for the command.
        /// </summary>
        public void Override<T>(Action<T> action, Guid organization) where T : ICommand
        {
            var name = _serializer.GetClassName(typeof(T));

            if (_overriders.Any(x => x.Key.Item1 == name && x.Key.Item2 == organization))
                throw new AmbiguousCommandHandlerException(name);

            _overriders.Add((name, organization), (command) => action((T)command));
        }

        #endregion

        #region Methods (sending synchronous commands)

        /// <summary>
        /// Executes the command synchronously.
        /// </summary>
        public void Send(ICommand command)
        {
            SerializedCommand serialized = null;

            if (_saveAll)
            {
                serialized = _store.Serialize(command);
                serialized.SendStarted = DateTimeOffset.UtcNow;
            }

            Execute(command, _serializer.GetClassName(command.GetType()));

            if (_saveAll)
            {
                serialized.SendCompleted = DateTimeOffset.UtcNow;
                serialized.SendStatus = "Completed";
                _store.Insert(serialized);
            }
        }

        #endregion

        #region Methods (scheduling asynchronous commands)

        /// <summary>
        /// Schedules the command for asynchronous execution.
        /// </summary>
        public void Schedule(ICommand command, DateTimeOffset? at = null)
        {
            var serialized = _store.Serialize(command);
            serialized.SendScheduled = at ?? DateTimeOffset.UtcNow;
            serialized.SendStatus = "Scheduled";
            _store.Insert(serialized);
        }

        /// <summary>
        /// Wakes the command queue to check for pending scheduled commands. Executes all commands for which the timer
        /// is now elapsed.
        /// </summary>
        public void Ping(Action<int> scheduledCommandRead)
        {
            var commands = _store.GetExpired(DateTimeOffset.UtcNow);
            scheduledCommandRead?.Invoke(commands.Length);

            foreach (var command in commands)
                Execute(command);
        }

        /// <summary>
        /// Forces a scheduled command to start.
        /// </summary>
        public bool Start(Guid command)
        {
            if (!_store.Exists(command))
                return false;

            Execute(_store.Get(command));
            return true;
        }

        /// <summary>
        /// Cancels a scheduled command.
        /// </summary>
        public void Cancel(Guid command)
        {
            var serialized = _store.Get(command);
            serialized.SendCancelled = DateTimeOffset.UtcNow;
            serialized.SendStatus = "Cancelled";
            _store.Update(serialized);
        }

        /// <summary>
        /// Completes a scheduled command.
        /// </summary>
        public void Complete(Guid command)
        {
            var serialized = _store.Get(command);
            serialized.SendCompleted = DateTimeOffset.UtcNow;
            serialized.SendStatus = "Completed";
            _store.Update(serialized);
        }

        #endregion

        #region Methods (bookmarking commands)

        /// <summary>
        /// Bookmarks the command for future reference.
        /// </summary>
        public void Bookmark(ICommand command, DateTimeOffset expired)
        {
            var serialized = _store.Serialize(command);
            serialized.BookmarkAdded = DateTimeOffset.Now;
            serialized.BookmarkExpired = expired;
            serialized.SendStatus = "Bookmarked";
            _store.Insert(serialized);
        }

        #endregion

        #region Methods (execution)

        private static readonly Regex TypeFullNamePattern = new Regex("^(.+), (.+), (.+), (.+), (.+)$", RegexOptions.Compiled);

        /// <summary>
        /// Invokes the subscriber method registered to handle the command.
        /// </summary>
        private void Execute(ICommand command, string @class)
        {
            // If the class name is fully-qualified with a Version, Culture, and PublicKeyToken 
            // then strip the latter properties from the class name.

            var match = TypeFullNamePattern.Match(@class);
            if (match.Success)
                @class = $"{match.Groups[1].Value}, {match.Groups[2].Value}";

            if (_overriders.ContainsKey((@class, command.OriginOrganization)))
            {
                var customization = _overriders[(@class, command.OriginOrganization)];
                customization.Invoke(command);
            }
            else if (_subscribers.ContainsKey(@class))
            {
                ExecuteSubscriberAction(command, @class);
            }
            else
            {
                throw new UnhandledCommandException(@class);
            }
        }

        private void ExecuteSubscriberAction(ICommand command, string @class)
        {
            try
            {
                var action = _subscribers[@class];
                action.Invoke(command);
            }
            catch (Exception ex)
            {
                var message = $"An unexpected error occurred executing this command ({@class}) with AggregateIdentifier {command.AggregateIdentifier}. {ex.Message}";
                message += " -- Here are the properties of this command: " + SerializeCommandForError(command);

                throw new UnhandledCommandException(message, ex);
            }
        }

        private string SerializeCommandForError(ICommand command)
        {
            try
            {
                var serialized = _store.Serialize(command);

                var maskedCommandData = MaskSensitiveDataInErrorMessage(serialized.CommandData);

                return $"OriginOrganization = {serialized.OriginOrganization}"
                     + $", OriginUser = {serialized.OriginUser}"
                     + $", CommandData = {maskedCommandData}";
            }
            catch (Exception ex)
            {
                return "An unexpected problem occurred during serialization of command data for error reporting. " +
                    ex.Message;
            }
        }

        /// <summary>
        /// Executes the command synchronously.
        /// </summary>
        private void Execute(SerializedCommand serialized)
        {
            var now = DateTimeOffset.UtcNow;

            serialized.SendStarted = now;
            serialized.SendStatus = "Started";

            _store.Update(serialized);

            // Uncomment this code to implement recurring commands. We may never do this, because all our recurring
            // commands scheduled and managed in Octopus.

            // bool recur = serialized.RecurrenceInterval.HasValue && serialized.RecurrenceUnit != null;
            // bool skip = recur && !Shift.Utility.Calendar.WeekdaysContain(serialized.RecurrenceWeekdays, now.DayOfWeek);
            // if (!skip)
            Execute(serialized.Deserialize(false), serialized.CommandClass);

            serialized.SendCompleted = DateTimeOffset.UtcNow;
            serialized.SendStatus = "Completed";

            // Uncomment this code to implement recurring commands.
            /* 
            if (recur && serialized.SendScheduled.HasValue)
            {
                var next = Shift.Utility.Calendar.CalculateNextInterval(serialized.SendScheduled.Value, DateTimeOffset.UtcNow, serialized.RecurrenceUnit, serialized.RecurrenceInterval.Value);
                if (next > serialized.SendScheduled)
                {
                    serialized.SendScheduled = next;
                    serialized.SendStarted = null;
                    serialized.SendCompleted = null;
                    serialized.SendStatus = "Scheduled";
                }
            }
            */

            _store.Update(serialized);
        }

        #endregion

        #region Masking

        public string MaskSensitiveDataInErrorMessage(string error)
        {
            try
            {
                // If the error message is not JSON then use a simple pattern match. This has the potential to mask
                // strings that are not actually credit card numbers, but this is better than allowing any credit card
                // numbers to leak through into an error log.

                var jsonStart = error.IndexOf("{");
                if (jsonStart == -1)
                    return MaskCreditCardInErrorMessage(error);

                var jsonEnd = error.LastIndexOf("}") + 1;
                var jsonPart = error.Substring(jsonStart, jsonEnd - jsonStart);
                var beforeJson = error.Substring(0, jsonStart);
                var afterJson = error.Substring(jsonEnd);

                var jsonObj = JObject.Parse(jsonPart);
                MaskSensitiveFieldsRecursively(jsonObj);

                return beforeJson + jsonObj.ToString(Formatting.None) + afterJson;
            }
            catch
            {
                // Fallback to pattern match.

                return MaskCreditCardInErrorMessage(error);
            }
        }

        private void MaskSensitiveFieldsRecursively(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    var obj = (JObject)token;
                    foreach (var property in obj.Properties().ToList())
                    {
                        if (IsSensitiveField(property.Name))
                        {
                            property.Value = MaskSensitiveValue(property.Name, property.Value?.ToString());
                        }
                        else
                        {
                            MaskSensitiveFieldsRecursively(property.Value);
                        }
                    }
                    break;

                case JTokenType.Array:
                    var array = (JArray)token;
                    foreach (var item in array)
                    {
                        MaskSensitiveFieldsRecursively(item);
                    }
                    break;

                case JTokenType.String:
                    var stringValue = token.ToString();
                    if (LooksLikeCreditCard(stringValue))
                    {
                        token.Replace(MaskCardNumber(stringValue));
                    }
                    break;
            }
        }

        private bool IsSensitiveField(string fieldName)
        {
            var sensitiveFields = new[]
            {
                "cardnumber", "card_number", "ccnumber", "cc_number",
                "securitycode", "security_code", "cvv", "cvc", "cvv2",
                "pin", "password",
                "sin", "ssn",
                "socialinsurancenumber", "socialsecuritynumber",
                "social_insurance_number", "social_security_number"
            };

            return sensitiveFields.Contains(fieldName.ToLowerInvariant());
        }

        private string MaskSensitiveValue(string fieldName, string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            var lowerFieldName = fieldName.ToLowerInvariant();

            if (lowerFieldName.Contains("card") || lowerFieldName.Contains("cc"))
            {
                return MaskCardNumber(value);
            }
            else if (lowerFieldName.Contains("security") || lowerFieldName.Contains("cvv") || lowerFieldName.Contains("cvc"))
            {
                return "XXX";
            }
            else if (lowerFieldName.Contains("pin"))
            {
                return "XXXX";
            }
            else if (lowerFieldName.Contains("password"))
            {
                return "********";
            }
            else if (lowerFieldName.Contains("sin") || lowerFieldName.Contains("ssn") || lowerFieldName.Contains("social"))
            {
                return "XXX-XXX-XXX";
            }
            else
            {
                // Default to card masking for unknown sensitive fields
                return MaskCardNumber(value);
            }
        }

        private bool LooksLikeCreditCard(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;

            // Remove spaces and hyphens
            var digitsOnly = Regex.Replace(value, @"[\s\-]", "");

            // Check if it's all digits and has typical credit card length
            return Regex.IsMatch(digitsOnly, @"^\d{13,19}$") &&
                   (digitsOnly.Length == 15 || digitsOnly.Length == 16 || digitsOnly.Length == 14);
        }

        private string MaskCardNumber(string cardNumber)
        {
            // Remove all non-digits
            var digitsOnly = Regex.Replace(cardNumber, @"\D", "");

            if (digitsOnly.Length < 13) // Invalid card number
                return "XXXX XXXX XXXX XXXX";

            // Detect card type by first digits and length
            if (IsAmexCard(digitsOnly))
            {
                // Amex: Show last 4, format as XXXX XXXXXX XXXXX
                var lastFour = digitsOnly.Substring(digitsOnly.Length - 4);
                return $"XXXX XXXXXX X{lastFour.Substring(0, 1)}{lastFour.Substring(1)}";
            }
            else
            {
                // Visa/MC/Discover: Show last 4, format as XXXX XXXX XXXX XXXX
                var lastFour = digitsOnly.Substring(digitsOnly.Length - 4);
                return $"XXXX XXXX XXXX {lastFour}";
            }
        }

        private bool IsAmexCard(string digitsOnly)
        {
            return digitsOnly.Length == 15 &&
                   (digitsOnly.StartsWith("34") || digitsOnly.StartsWith("37"));
        }

        public string MaskCreditCardInErrorMessage(string errorMessage)
        {
            // Pattern for standard 16-digit cards (with spaces)
            var standardPattern = @"\b\d{4}\s\d{4}\s\d{4}\s\d{4}\b";
            errorMessage = Regex.Replace(errorMessage, standardPattern, "XXXX XXXX XXXX XXXX");

            // Pattern for Amex 15-digit cards (with spaces: 4-6-5 format)
            var amexPattern = @"\b\d{4}\s\d{6}\s\d{5}\b";
            errorMessage = Regex.Replace(errorMessage, amexPattern, "XXXX XXXXXX XXXXX");

            return errorMessage;
        }

        #endregion
    }
}
