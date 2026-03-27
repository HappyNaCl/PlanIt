namespace PlanIt.Application.Common.Interfaces.Authentication;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Validate(string password, string hashedPassword);
}