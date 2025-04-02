using Logitar.Identity.Contracts.Users;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Users;

namespace PokeCraft.Application.Accounts;

public interface IMessageService
{
  Task<SentMessages> SendAsync(string template, IEmail email, string locale, IEnumerable<KeyValuePair<string, string>> variables, CancellationToken cancellationToken = default);
  Task<SentMessages> SendAsync(string template, UserModel user, ContactType contactType, string locale, IEnumerable<KeyValuePair<string, string>> variables, CancellationToken cancellationToken = default);
}
