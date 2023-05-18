using System.ComponentModel.DataAnnotations;

namespace TodoList.MVC.API.Models;

public class User
{
    public User(string email, string password)
    {
        Id = Guid.NewGuid();
        Email = email;
        Password = password;
    }

    [Key]
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}