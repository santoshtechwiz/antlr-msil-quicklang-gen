 

# QuickLang MSIL Code Generator

This project is a code generator for the QuickLang programming language, using the ANTLR parser generator. It translates QuickLang source code into MSIL (Microsoft Intermediate Language) for execution on the .NET platform.

## Table of Contents

-   [QuickLang MSIL Code Generator](#quicklang-msil-code-generator)
    -   [Table of Contents](#table-of-contents)
    -   [Getting Started](#getting-started)
        -   [Prerequisites](#prerequisites)
        -   [Installation](#installation)
    -   [Usage](#usage)
    -   [Examples](#examples)
    -   [Contributing](#contributing)
    -   [License](#license)

## Getting Started

### Prerequisites

-   [.NET SDK](https://dotnet.microsoft.com/download)

### Installation


```bash
git clone https://github.com/santoshtechwiz/antlr-msil-quicklang-gen.git
``` 

Build the project:

```bash
cd quicklang-msil-gen
dotnet build
```

## Usage

To generate MSIL code from a QuickLang source file, use the following command:

```bash
dotnet run
```

This will produce an MSIL assembly file that can be executed on the .NET runtime.

## Examples

```csharp
int a;
int b;
a = 3;
b = a + 1;
if (a == 3)  
    a = a + 1; 
    b = b + 6;
endif 
print a;
print b;
```

This will generate the corresponding MSIL code:

```csharp
.assembly extern mscorlib { }
.assembly hello { }
.module hello.exe
.method public static void main() cil managed
{
.entrypoint
.maxstack 8
.locals init (
[0]  int32  a
, [1]  int32  b
)
ldc.i4 0
stloc a
ldc.i4 0
stloc b

ldc.i4 0
stloc a
ldc.i4 0
stloc b
ldc.i4 3
stloc a
ldloc a
ldc.i4 1
add
stloc b
ldloc a
ldc.i4 3
beq L0
br L1
L0:
ldloc a
ldc.i4 1
add
stloc a
ldloc b
ldc.i4 6
add
stloc b
L1:
ldloc a
call void [mscorlib]System.Console::WriteLine(int32)
ldloc b
call void [mscorlib]System.Console::WriteLine(int32)

ret
}

```

## Contributing

Contributions are welcome! Feel free to open an issue or create a pull request.

## License

This project is licensed under the [MIT License]

