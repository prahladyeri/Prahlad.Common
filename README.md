# Prahlad.Common

Prahlad.Common is a small collection of reusable helper utilities for .NET projects.  
It provides common functionality like math helpers, theme helpers, and other utility classes that can be shared across applications.

## Features

- **MathHelper** – Common math operations and safe calculations.  
- **ThemeHelper** – Basic utilities for theme or UI customization (Windows-only).  
- **More helpers** – Growing collection of general-purpose utilities.

## Supported Frameworks

This library is multi-targeted and works on:

- .NET Framework 4.6.1+
- .NET Standard 2.0
- .NET 6 (LTS)
- .NET 8 (LTS)

## Installation

Install via NuGet:

```bash
dotnet add package Prahlad.Common
````

Or with the Package Manager Console:

```powershell
Install-Package Prahlad.Common
```

## Usage

```csharp
using Prahlad.Common;

var result = StringHelper.EncryptRomanShift("PRAHLAD");
// result = ZBKRVKN
```

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).