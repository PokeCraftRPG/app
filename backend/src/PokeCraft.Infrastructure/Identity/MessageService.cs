using Logitar.Identity.Contracts.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Users;
using Microsoft.Extensions.Configuration;
using PokeCraft.Application.Accounts;

namespace PokeCraft.Infrastructure.Identity;

internal class MessageService : IMessageService
{
  private readonly IMessageClient _messageClient;
  private readonly Guid _phoneSenderId;

  public MessageService(IConfiguration configuration, IMessageClient messageClient)
  {
    _messageClient = messageClient;

    string? phoneSenderId = Environment.GetEnvironmentVariable("PORTAL_PHONE_SENDER_ID");
    if (!string.IsNullOrWhiteSpace(phoneSenderId))
    {
      _phoneSenderId = Guid.Parse(phoneSenderId);
    }
    _phoneSenderId = configuration.GetSection("Portal").GetValue<Guid?>("PhoneSenderId")
      ?? throw new ArgumentException("The configuration key 'Portal:PhoneSenderId' is required.", nameof(configuration));
  }

  public async Task<SentMessages> SendAsync(string template, IEmail email, string locale, IEnumerable<KeyValuePair<string, string>> variables, CancellationToken cancellationToken)
  {
    RecipientPayload recipient = new()
    {
      Address = email.Address
    };
    return await SendAsync(template, recipient, ContactType.Email, locale, variables, cancellationToken);
  }

  public async Task<SentMessages> SendAsync(string template, UserModel user, ContactType contactType, string locale, IEnumerable<KeyValuePair<string, string>> variables, CancellationToken cancellationToken)
  {
    RecipientPayload recipient = new()
    {
      UserId = user.Id
    };
    return await SendAsync(template, recipient, contactType, locale, variables, cancellationToken);
  }

  private async Task<SentMessages> SendAsync(string template, RecipientPayload recipient, ContactType contactType, string locale, IEnumerable<KeyValuePair<string, string>>? variables, CancellationToken cancellationToken)
  {
    SendMessagePayload payload = new(template)
    {
      Locale = locale
    };
    if (contactType == ContactType.Phone)
    {
      payload.SenderId = _phoneSenderId;
    }
    payload.Recipients.Add(recipient);
    if (variables is not null)
    {
      foreach (KeyValuePair<string, string> variable in variables)
      {
        payload.Variables.Add(new Variable(variable));
      }
    }
    RequestContext context = new(recipient.UserId?.ToString(), cancellationToken);
    return await _messageClient.SendAsync(payload, context);
  }
}
