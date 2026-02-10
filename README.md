# JK.Fixed

JK.Fixed is a small library to help read and write fixed-width (column-aligned) text files from .NET applications.

The project provides two approaches:

- A runtime reflection-based serializer/deserializer (`FixedSerializer`) that works on any POCO decorated with `FixedColumnAttribute`.
- A compile-time source generator that produces high-performance, allocation-friendly serializer helpers for types marked with `FixedSerializableAttribute`.

This repository contains the library source, the source-generator, tests and examples demonstrating common usage patterns.

## Features

- Read fixed-width records into POCOs or dictionaries
- Write objects back to fixed-width files
- Configurable column widths, trimming and padding rules
- Optional source-generation for more efficient serializers

## Installation

Install from NuGet (package id: `JK.Fixed`). Example using the .NET CLI:

```
dotnet add package JK.Fixed
```

If you prefer to build from source:

```
git clone https://github.com/jeremyknight-me/fixed.git
cd fixed
dotnet build
```

## Quick start

Below are examples showing both the runtime (reflection) API and the source-generated helpers.

1) Using the runtime serializer (reflection-based)

```csharp
public class Person
{
    [JK.Fixed.Configuration.FixedColumn(10, Order = 1)]
    public string Name { get; set; }

    [JK.Fixed.Configuration.FixedColumn(3, Order = 2)]
    public int Age { get; set; }
}

IEnumerable<string> lines = File.ReadLines("people.txt");
IEnumerable<Person> people = JK.Fixed.FixedSerializer.Deserialize<Person>(lines);

// Serialize back to fixed lines
IEnumerable<string> output = JK.Fixed.FixedSerializer.Serialize(people);
File.WriteAllLines("out.txt", output);
```

2) Using the source generator (compile-time generated serializers)

Mark the type with `FixedSerializableAttribute` in addition to per-property `FixedColumn` attributes. When the project is built the source generator emits a static helper named `{TypeName}FixedSerializer` with `Serialize` and `Deserialize` methods.

```csharp
[JK.Fixed.Configuration.FixedSerializable]
public class Person
{
    [JK.Fixed.Configuration.FixedColumn(10, Order = 1)]
    public string Name { get; set; }

    [JK.Fixed.Configuration.FixedColumn(3, Order = 2)]
    public int Age { get; set; }
}

// After build, the generator emits `PersonFixedSerializer` with the methods below:
var people = PersonFixedSerializer.Deserialize(File.ReadLines("people.txt"));
var lines = PersonFixedSerializer.Serialize(people);
```

## Contributing

Contributions are welcome. Please follow these guidelines:

- Open an issue to discuss larger changes before implementing them.
- Follow the existing repository coding conventions and add unit tests for new features or bug fixes.
- Keep changes small and focused; use descriptive commit messages.
