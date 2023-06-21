using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoList.MVC.API.Models.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<UserAggregate>
{
    public void Configure(EntityTypeBuilder<UserAggregate> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(x => x.Id).ValueGeneratedNever().IsRequired();
        
        builder.Navigation(s => s.TodoItems).Metadata.SetField("_todoItems");
        builder.Navigation(s => s.TodoItems).UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.Navigation(s => s.Projects).Metadata.SetField("_projects");
        builder.Navigation(s => s.Projects).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(x => x.TodoItems, ownedBuilder =>
        {
            //Owns has cascade on delete by default
            ownedBuilder.HasKey(x => x.Id);
            ownedBuilder.WithOwner().HasForeignKey("UserId");
            ownedBuilder.Property(x => x.Id).ValueGeneratedNever().IsRequired();
        });
        builder.OwnsMany(x => x.Projects, ownedBuilder =>
        {
            //Owns has cascade on delete by default
            ownedBuilder.HasKey(x => x.Id);
            ownedBuilder.WithOwner().HasForeignKey("UserId");
            ownedBuilder.Property(x => x.Id).ValueGeneratedNever().IsRequired();
        });
    }
}