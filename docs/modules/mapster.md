# Fast.Mapster

Zero-configuration object mapping integration built on [Mapster](https://github.com/MapsterMapper/Mapster).

## Installation

```bash
dotnet add package Fast.Mapster
```

## Registration

```csharp
builder.Services.AddMapster();
```

This automatically:
- Scans all assemblies for `IRegister` implementations
- Configures flexible name matching (case-insensitive)
- Enables reference preservation
- Registers `IMapper` for dependency injection

## Usage

### Via DI

```csharp
public class UserService
{
    private readonly IMapper _mapper;

    public UserService(IMapper mapper) => _mapper = mapper;

    public UserDto GetUser(User entity)
    {
        return _mapper.Map<UserDto>(entity);
    }
}
```

### Via Extension Methods

```csharp
// Map a single object
var dto = user.Adapt<UserDto>();

// Map a collection
var dtos = users.Adapt<List<UserDto>>();
```

### Custom Mapping Configuration

Create a class implementing `IRegister`:

```csharp
public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserDto>()
            .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
            .Ignore(dest => dest.Password);
    }
}
```
