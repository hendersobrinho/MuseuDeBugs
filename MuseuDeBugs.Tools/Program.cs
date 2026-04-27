using Microsoft.AspNetCore.Identity;

Console.Write("Username do admin: ");
var username = Console.ReadLine();

Console.Write("Senha do admin: ");
var password = Console.ReadLine();

if (string.IsNullOrWhiteSpace(username))
{
    Console.WriteLine("Username nao pode ficar vazio.");
    return;
}

if (string.IsNullOrWhiteSpace(password))
{
    Console.WriteLine("Senha nao pode ficar vazia.");
    return;
}

var passwordHasher = new PasswordHasher<string>();
var hash = passwordHasher.HashPassword(username, password);

Console.WriteLine();
Console.WriteLine("Cole este valor em Admin:PasswordHash:");
Console.WriteLine(hash);
