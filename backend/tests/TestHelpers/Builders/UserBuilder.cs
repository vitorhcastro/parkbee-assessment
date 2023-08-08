using Domain.Entities;
using Infrastructure;

namespace TestHelpers.Builders;

public class UserBuilder
{
    private Guid id = Guid.NewGuid();

    public static UserBuilder AUser()
    {
        return new UserBuilder();
    }

    public UserBuilder WithId(Guid id)
    {
        this.id = id;
        return this;
    }

    public User Build()
    {
        return new User()
        {
            Id = this.id,
            PartnerId = ApplicationConstants.Authentication.DefaultPartnerId,
        };
    }
}
