using Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;

namespace Infrastructure.Adapters.Postgres.Inbox;

public interface IInbox
{
    Task<bool> Save(IConvertibleToCommand convertibleToCommand);
}