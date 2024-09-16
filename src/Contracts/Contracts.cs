namespace Contracts;

public sealed record SmsRequested(int Id);

public sealed record SmsProcessed(int Id, DateTime StartedAt);