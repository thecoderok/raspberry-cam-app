```
dotnet user-secrets set Auth:UserName <username>
dotnet user-secrets set Auth:Password <password>
dotnet user-secrets set ConnectionStrings:DefaultConnection ""
dotnet ef migrations add InitialCreate -c ApplicationDbContext
dotnet ef  migrations script
```
