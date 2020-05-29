# BFSourceGen
![Build Status](https://github.com/ScottHolden/BFSourceGen/workflows/publish%20to%20nuget/badge.svg)
[![NuGet Version](https://img.shields.io/nuget/v/BFSourceGen.svg?style=flat)](https://www.nuget.org/packages/BFSourceGen/) 

Ever wanted to write a C# console app in Brainf**k? Well now you can!

Thanks to C# Source Generators, we can now convert our BF to C#, and then compile it!

![Hello World Example](images/helloworld.png)

Ever had a piece of business logic or home-rolled insecure encryption that was too simple to work on and understand in C#? This is the answer. You can also mix up your projects, calling BF code whenever you want!

![Mixed Code Example](images/mixed.png)

If your BF code reads input, we'll ask for a parameter, if it has output, we'll return a string. You can even write parameterless void BF code where the only side-effects are CPU heat & processing time!!!

We can even optimize if required (clumping together adjacent op's), but it's **totally optional** and **turned off by default**! Live the true BF experience in having every individual increment & decrement call map to an individual operation.

With minimal error checking, you can be sure that we will only tell you if something is critically wrong.

![Error Example](images/error.png)

Finally, everyone loves magic code that performs a crucial operation, is never documented in the readme, and the author just assumes you should know what to do. An example of this is class names that are not explicitly defined, or mystical namespaces. Good luck.

## How to use/install/integrate into my enterprise codebase

It's on NuGet! Just install the NuGet package and it should appear as an analyzer:

![NuGet Package Install](images/package.png)

Once it's installed you'll need to either **mark your .bf files as _"C# analyzer additional file"_**, or **add a line to your .csproj to include all .bf files** as additional files. You'll also need to bump your langversion to preview!

![NuGet Package Install](images/addfiles.png)

From here, spend the next 3 hours writing some complex BF, hit F5 and marvel at your coding superiority!

![NuGet Package Install](images/run.png)

##  .bf options:

| Flag Example | Description                        | Valid Values                          | Default Value |
|--------------|------------------------------------|---------------------------------------|---------------|
| #memsize=100 | Sets the size of the memory array  | > 0                                   | 1024          |
| #memtype=int | Sets the type for the memory array | _byte_, _int_, _long_                 | int           |
| #eofvalue=-1 | Sets the End of File value         | _same_, or any valid value of memtype | 0             |
| #optimize=true | Turns on optimization at compile time |  _true_, _false_, or a flag without a value is treated as true | false |
| #class=true | Instead of emitting an entry-point, just make it a normal class |  _true_, _false_, or a flag without a value is treated as true | false |

## Not-so FAQ:

### What is a Source Generator?

https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/

### What's Brainf**k?

https://en.wikipedia.org/wiki/Brainfuck

### Why???

_This Page is Intentionally Left Blank_

### I installed the package, and it isn't working?

Did you remeber to mark the file as an analyzer additional file and bump your langVersion? :)