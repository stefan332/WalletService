Controllers/: Contains all API controllers, including WalletController, which handles wallet transactions.
Properties/: Contains project properties and metadata.
Program.cs: The main entry point for the API, configuring services, middleware, authentication, and authorization.
WalletService.csproj: The project file defining dependencies and project settings.
WalletService.sln: The solution file to open the project in Visual Studio.
appsettings.json: Main configuration file for setting up JWT, connection strings, and other service configurations.
appsettings.Development.json: Additional configuration specific to the development environment.
Dockerfile: The Dockerfile for containerizing the Wallet Service API, including instructions for building and running the service in a Docker container.
.gitignore & .dockerignore: Defines which files and folders should be ignored in Git and Docker

Prerequisites
.NET SDK: Version 6.0 or higher
SQL Server: Database for storing wallet and transaction data
Docker (optional): For containerization

1. Clone the Repository
Clone the repository locally: git clone https://github.com/your-repo/wallet-service.git
cd wallet-service

2. Configure appsettings.json
In appsettings.json, you’ll need to set up:
- Database Connection: Set the DefaultConnection to your SQL Server instance ;
"ConnectionStrings": {
  "DefaultConnection": "Your_SQL_Server_Connection_String"
}
-JWT Secret Key: Set up the JwtConfig:Secret for token signing.
"JwtConfig": {
  "Secret": "your_jwt_secret_key"
}
- Origins: Specifies allowed sites for Cross-Origin Resource Sharing (CORS).
  AllowedSites: An array of domains that are permitted to access this API. Replace "https://example.com" and other sample URLs with your actual trusted domains.
    "Origins": {
    "AllowedSites": [ "https://production-site.com" ]
  },

3. Run Migrations (if using Entity Framework)
  -dotnet ef database update or Update-Database.
  -Build the Docker image: docker build -t wallet-service .


4. Running the Application :
   -dotnet run

5.Using Docker (Optional)
-To run the service in Docker:
 --Build the Docker image: docker build -t wallet-service .
 --Run the container: docker run -p 5000:5000 wallet-service.

 6. Accessing the API Documentation
Navigate to https://localhost:5001/swagger to view and interact with the API endpoints in Swagger.

7. Testing Authorization
To access endpoints secured with JWT:

Use /api/auth/login to log in and retrieve a token.
For subsequent requests, add an Authorization header with Bearer <your_token>.
