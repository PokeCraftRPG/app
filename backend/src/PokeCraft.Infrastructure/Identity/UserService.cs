using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;
using PokeCraft.Application.Accounts;
using PokeCraft.Application.Accounts.Models;

namespace PokeCraft.Infrastructure.Identity;

internal class UserService : IUserService
{
  private readonly IUserClient _userClient;

  public UserService(IUserClient userClient)
  {
    _userClient = userClient;
  }

  public async Task<UserModel> AuthenticateAsync(UserModel user, string password, CancellationToken cancellationToken)
  {
    AuthenticateUserPayload payload = new(user.UniqueName, password);
    RequestContext context = new(cancellationToken);
    return await _userClient.AuthenticateAsync(payload, context);
  }

  public async Task<UserModel> CompleteProfileAsync(UserModel user, CompleteProfilePayload profile, CancellationToken cancellationToken)
  {
    UpdateUserPayload payload = profile.ToUpdateUserPayload();
    if (profile.Password is not null)
    {
      payload.Password = new ChangePasswordPayload(profile.Password);
    }
    payload.CompleteProfile();
    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _userClient.UpdateAsync(user.Id, payload, context) ?? throw new InvalidOperationException($"The user 'Id={user.Id}' could not be found.");
  }

  public async Task<UserModel> CreateAsync(string emailAddress, CancellationToken cancellationToken)
  {
    CreateUserPayload payload = new(emailAddress)
    {
      Email = new EmailPayload(emailAddress, isVerified: true)
    };
    RequestContext context = new(cancellationToken);
    return await _userClient.CreateAsync(payload, context);
  }

  public async Task<UserModel?> FindAsync(string emailAddress, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await _userClient.ReadAsync(id: null, emailAddress, identifier: null, context);
  }

  public async Task<UserModel> FindAsync(Guid id, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await _userClient.ReadAsync(id, uniqueName: null, identifier: null, context) ?? throw new InvalidOperationException($"The user 'Id={id}' could not be found.");
  }

  public async Task<IReadOnlyCollection<UserModel>> FindAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
  {
    SearchUsersPayload payload = new();
    payload.Ids.AddRange(ids);
    RequestContext context = new(cancellationToken);
    SearchResults<UserModel> users = await _userClient.SearchAsync(payload, context);
    return users.Items.AsReadOnly();
  }

  public async Task<UserModel> UpdateAsync(UserModel user, EmailPayload email, CancellationToken cancellationToken)
  {
    UpdateUserPayload payload = new()
    {
      Email = new ChangeModel<EmailPayload>(email)
    };
    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _userClient.UpdateAsync(user.Id, payload, context) ?? throw new InvalidOperationException($"The user 'Id={user.Id}' could not be found.");
  }
}
