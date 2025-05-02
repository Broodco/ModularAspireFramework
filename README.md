# ModularAspireFramework

A template for creating modular monolith applications with .NET Aspire, following Clean Architecture principles and modular design patterns.

## Features

- **Modular Architecture**: Pre-configured modules with clear separation of concerns
- **Clean Architecture**: Domain, Application, Infrastructure, and Presentation layers for each module
- **Event-Driven Design**: Domain events and integration events for module communication
- **.NET Aspire Integration**: Cloud-native capabilities with built-in observability
- **CQRS Pattern**: Separation of command and query responsibilities
- **Organized Structure**: Clear separation between source code and tests

## Getting Started

### Prerequisites

- .NET 9.0 SDK

### Building and Installing the Template

This repository contains the source code for the template. You need to build and install it before using:

```bash
# Clone the repository
git clone https://github.com/Broodco/ModularAspireFramework.git
cd ModularAspireFramework

# Build the template package
dotnet pack -c Release

# Install the template
dotnet new install bin/Release/ModularAspireFramework.1.0.0.nupkg

# Alternatively, install directly from the source directory during development
dotnet new install .
```

### Creating a New Project

Create a new project using the template:

```bash
dotnet new modaspire -n YourCompany.YourProduct --company "YourCompany"
```

Available parameters:

- `--company`: Your company name (used for namespaces and assembly info)

## Solution Structure

The generated solution follows this structure:

```
YourCompany.YourProduct/
├── src/
│   ├── API/
│   │   └── YourCompany.YourProduct.Api/
│   ├── Common/
│   │   ├── YourCompany.YourProduct.Common.Application/
│   │   ├── YourCompany.YourProduct.Common.Domain/
│   │   ├── YourCompany.YourProduct.Common.Infrastructure/
│   │   └── YourCompany.YourProduct.Common.Presentation/
│   ├── Modules/
│   │   ├── Identity/
│   │   │   ├── YourCompany.YourProduct.Identity.Application/
│   │   │   ├── YourCompany.YourProduct.Identity.Domain/
│   │   │   ├── YourCompany.YourProduct.Identity.Infrastructure/
│   │   │   └── YourCompany.YourProduct.Identity.Presentation/

│   ├── YourCompany.YourProduct.MigrationService/
│   ├── YourCompany.YourProduct.AppHost/
│   └── YourCompany.YourProduct.ServiceDefaults/
└── YourCompany.YourProduct.sln
```

### Key Components

- **API**: The gateway that coordinates requests to the appropriate modules
- **Common**: Shared abstractions and utilities across all modules
- **Modules**: Independent functional units with their own domain logic
- **AppHost**: .NET Aspire orchestration of services and resources
- **ServiceDefaults**: Shared configuration and middleware for all services
- **MigrationService**: Database migration utilities for all modules

## Module Structure

Each module follows the same Clean Architecture structure:

- **Domain**: Entities, aggregates, value objects, domain events, and domain services
- **Application**: Use cases, commands/queries, and domain event handlers
- **Infrastructure**: Repositories, external services, and integration with infrastructure
- **Presentation**: API controllers, middleware, and API-specific models

## Development Workflow

1. **Navigate to your solution directory**: `cd YourCompany.YourProduct`
2. **Restore dependencies**: `dotnet restore`
3. **Build the solution**: `dotnet build`
4. **Run the solution**: `dotnet run --project src/YourCompany.YourProduct.AppHost`

## Customization

The template provides a solid foundation, but you can customize it to fit your needs:

- Add new modules by following the same structure
- Extend the common functionality in the Common projects
- Configure additional infrastructure services in the AppHost project
- Add new API endpoints to the API project

## Contributing

Contributions to improve the template are welcome. Please feel free to submit issues or pull requests.

## Acknowledgements

This template is based on knowledge gained from Milan Jovanović's courses and teachings on modular monolith architecture and clean design principles. Special thanks to Milan for sharing his expertise and providing the foundational concepts that inspired this work.
See more in the CREDITS.md file.

## License

This template is licensed under the MIT License - see the LICENSE file for details.