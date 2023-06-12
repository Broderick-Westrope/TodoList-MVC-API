using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoList.MVC.API.Models.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<UserAggregate>
{
    public void Configure(EntityTypeBuilder<UserAggregate> builder)
    {
        builder.HasKey(u => u.Id);

        //TODO: Add more EF config (required, etc).
        builder.OwnsMany(x => x.TodoItems, ownedBuilder =>
        {
            //? TODO: How to configure the OnDelete(Cascade)?
            ownedBuilder.HasKey(x => x.Id);
            ownedBuilder.WithOwner().HasForeignKey("UserId");
        });
        builder.OwnsMany(x => x.Projects, ownedBuilder =>
        {
            //? TODO: How to configure the OnDelete(Cascade)?
            ownedBuilder.HasKey(x => x.Id);
            ownedBuilder.WithOwner().HasForeignKey("UserId");
        });
    }
}