namespace Shift.Service.Chatbot;

public enum EventType { Delta, DirectText, Table, Summary }

public record ChatStreamEvent(EventType Type);

public record DeltaEvent(string Text) : ChatStreamEvent(EventType.Delta);

public record DirectTextEvent(string Text) : ChatStreamEvent(EventType.DirectText);

public record TableEvent(string Text) : ChatStreamEvent(EventType.Table);

public record SummaryEvent(string Model, int InputTokenCount, int OutputTokenCount, string[] Tools) : ChatStreamEvent(EventType.Summary);