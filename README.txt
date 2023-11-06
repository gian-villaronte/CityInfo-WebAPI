Web API .Net 6
HTTP GET POST PUT PATCH DELETE
Downloading of file
Error Handling and Logging
Inversion Of Control and Dependency Injection
Usage of Serilog Logging Service
Usage of Object Relational Mapping (ORM)
Repository Pattern in ORM (interfaces)
ORM Filtering and Searching functionality
Pagination (helps performance)
JWT Token Authentication (Bearer Token)
Authorization Policies
Versioning
API Documentation by Swashbuckle


builder.services
	-AddSingleton lifetime services created the first time they're created
	-AddTransient lifetime services created each time they're requested. Great for lightweight and stateless services
	-AddScoped lifetime services created once per request

from https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-usage
-Transient services are always different, a new instance is created with every retrieval of the service.
-Scoped services change only with a new scope, but are the same instance within a scope.
-Singleton services are always the same, a new instance is only created once.


When providing the connection string in production environment, it is unsafe to put it in appsettings. thats fine for development but when on prod,
safest place to put it is in Azure Vault Key or the Systems environment variables. those are outside the solution file for safekeeping

Authentication token based standards should use something like OAuth2 or OpenID Connect for better security when used in big enterprise apps