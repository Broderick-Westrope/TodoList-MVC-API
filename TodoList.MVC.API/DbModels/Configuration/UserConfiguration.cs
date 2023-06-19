using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoList.MVC.API.Models.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<UserAggregate>
{
    public void Configure(EntityTypeBuilder<UserAggregate> builder)
    {
        builder.HasKey(u => u.Id);

        builder.OwnsMany(x => x.TodoItems, ownedBuilder =>
        {
            //Owns has cascade on delete by default
            ownedBuilder.HasKey(x => x.Id);
            ownedBuilder.WithOwner().HasForeignKey("UserId");
        });
        builder.OwnsMany(x => x.Projects, ownedBuilder =>
        {
            //Owns has cascade on delete by default
            ownedBuilder.HasKey(x => x.Id);
            ownedBuilder.WithOwner().HasForeignKey("UserId");
        });
    }
}