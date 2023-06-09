using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoList.MVC.API.Models.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<UserAggregateRoot>
{
    public void Configure(EntityTypeBuilder<UserAggregateRoot> builder)
    {
        builder.HasKey(u => u.Id);
    }
}