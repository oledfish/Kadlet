# Kadlet

A .NET implementation of a parser for the [KDL Document Language](https://github.com/kdl-org/kdl). Most of its logic
and structure is based around [kdl4j](https://github.com/hkolbeck/kdl4j) by [hkolbeck](https://github.com/hkolbeck).

# Install

The package is available at [NuGet](https://www.nuget.org/packages/Kadlet).

# Usage

```csharp
using System.IO;
using Kadlet;

// ...
KdlReader reader = new KdlReader(KdlPrintOptions.PrettyPrint);

using (FileStream fs = File.OpenRead("file.kdl")) {
    KdlDocument document = reader.Parse(fs);
    using (StringWriter sw = new StringWriter()) {
        document.Write(sw);
        Console.Write(sw.ToString());
    };
}
```

The resulting ``KdlDocument`` can be traversed or written back into a stream or string with its methods ``Write`` or ``ToKdlString``. The output format can be customized by changing the fields of a ``KdlPrintOptions`` instance and passing it as an argument.

The values in properties and arguments are encapsulated in a wrapper ``KdlValue<T>`` object, of which there are derivations for most commonly used types. Integers are returned as a ``KdlInt32`` (int), a ``KdlInt64`` (long), or a ``KdlBigInteger`` depending on their size. Floating point numbers are returned as a ``KdlFloat64`` by default.

## Type annotations

Kadlet supports [type annotation overrides](https://github.com/kdl-org/kdl/blob/main/SPEC.md#type-annotation), instructing the parser to interpret annotated values differently according to their type.

```csharp
KdlDocument overrides = reader.Parse(@"
    node (u8)123
    node (f32)123
");

Console.Write(overrides.ToKdlString());

// Output:
//
// node (u8)123
// node (f32)123.0
```

All numeric overrides are supported save for ``isize`` and ``usize``, which are ignored. The following string overrides are supported.

* ``date-time``
* ``date``
* ``time``
* ``decimal``
* ``ipv4``
* ``ìpv6``
* ``regex``
* ``base64``

## Custom type annotations

It's possible to define string overrides for custom types.

```csharp
public enum Color { Red = 1, Green = 2, Blue = 3 };
        
// ...

Dictionary<string, KdlTypeConverter> converters = new Dictionary<string, KdlTypeConverter> {
    {"hex", (input, type) => new KdlByteArray(Convert.FromHexString(input), type)},
    {"color", (input, type) => new KdlEnum(Enum.Parse<Color>(input, true), type)} // slow due to Enum.Parse
};

KdlReader reader = new KdlReader(new KdlReaderOptions {
    TypeConverters = converters
});

string kdl = "node (hex)\"5F3759DF\" (color)\"green\"";

KdlDocument doc = reader.Parse(kdl);
Console.Write(doc.ToKdlString());

// Output:
// node (hex)"XzdZ3w== (color)"Green"
```

# Remarks

This library is in an early state and bugs may be found. Issues and pull requests are welcome and encouraged.

# License

Kadlet is made available under the MIT license.
