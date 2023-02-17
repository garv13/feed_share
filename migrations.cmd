dotnet ef migrations add InitialDbMigration -c PostShareContext -o Data/Migrations
dotnet ef database update