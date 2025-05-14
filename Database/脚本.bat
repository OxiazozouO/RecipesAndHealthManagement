cd .
cd ..
cd WebServer
dotnet tool install --global dotnet-ef --version 7.0.16
dotnet ef dbcontext scaffold "server=localhost;user=root;password=123456;database=recipe_and_health_system" MySql.EntityFrameworkCore -o DatabaseModel -f -v
pause