---
name: dotnet-guidelines
description: C# and .NET workflow and conventions for working in a real project, covering SDK and target framework choices, language versions, platform-specific APIs, verification, naming, comments, control flow, composition with interfaces and extension methods, API design, error handling, async, project structure, and unit tests. Use this whenever C# or .NET comes up in any form, including writing, editing, refactoring, reviewing, or explaining C# code; changing or reviewing .cs, .csproj, .sln, global.json, or Directory.Build.props files; or answering a .NET API-design or style question, even if nobody asks for "guidelines" or a "style guide".
---

# .NET Guidelines

These are defaults. When one disagrees with the project's existing conventions or instructions, follow the project and mention the tension briefly. A project's `.editorconfig` is part of its conventions and wins over this file. The examples show patterns and aren't templates to copy.

## Workflow

### Before changing code

- Check the SDK with `dotnet --list-sdks`. If `dotnetup` is present, `dotnetup list` shows what it manages. If neither works, check `DOTNET_ROOT`. If the SDK is still missing or broken, stop and ask rather than working around it.
- Read the project file, plus `global.json`, `Directory.Build.props`, and `Directory.Packages.props` if they exist. Note the target frameworks, `LangVersion`, nullable setting, and whether warnings are errors.
- Read the code around the change and match its style.
- Check which frameworks and operating systems CI covers, so you know what your own run can't show.
- Ask when a design decision is open, such as which interface to use or whether an abstraction earns its keep. Expect first drafts of APIs and names to change.

### After changing code

1. Run `dotnet build` and `dotnet test`. Use `dotnet test --filter <expression>` while iterating, then run the whole suite. Read the warnings, since a successful build can still report real problems.
2. Run `dotnet format --verify-no-changes` if the project uses it.
3. Run the program when the change affects what it does. For a library, call it from a small scratch project outside the repository, which also proves the public API is enough. Passing tests alone don't show it works.
4. Report what you checked and what you couldn't, such as Windows behavior from a Linux machine.

### Reviewing C#

Beyond correctness, look for:

- New APIs or syntax that a declared target framework doesn't have.
- Changes to public names, signatures, or behavior that existing callers would notice. Adding an optional parameter breaks binary compatibility even though callers' source still compiles.
- `CA1416` platform warnings, and assumptions about paths, line endings, or culture.
- `!`, `.Result`, `async void`, and `catch (Exception)` without a stated reason.

### Larger changes

Break a change that spans many files or systems into named, sequential phases, starting with scaffolding. Leave the project working after each phase. If the project has its own process for this, follow that instead.

## Design

- Build things to be tried early, and rebuild the clumsy parts rather than patching around them. Detouring to build a throwaway tool is fine when it lightens the task.
- Apply Occam's razor: don't add language features or abstractions beyond what the problem needs. Weigh a feature by how easily a programmer can use its power, not by its theoretical ceiling.
- A method or tool should do one thing well. This limits scope, not size, and matters less for GUIs.
- Supporting older frameworks can force more complex code, such as `#if` branches or polyfills. That complexity is the cost of the constraint, not a failure of the razor.

## SDKs and Compatibility

Four settings are easy to mix up, and they fail differently:

| Setting | Controls | When it's too new |
| --- | --- | --- |
| SDK (`global.json`) | Which toolchain builds the project | `A compatible .NET SDK was not found`, or `NETSDK1045` if the SDK is older than the target framework |
| Target framework | Which APIs exist and which runtime is required | Compile errors on the missing API |
| `LangVersion` | Which syntax is allowed | Compile errors, or `CS1617` if the SDK doesn't know that version |
| Platform-specific APIs | Which operating systems an API works on | `CA1416` warning, then a runtime failure |

### Target frameworks

**Target the oldest framework you actually build and test against**, not the newest one installed. A framework nothing tests isn't really supported.

- **Multi-targeting needs each runtime.** A project with `net8.0;net10.0` compiles for both on one SDK, but running or testing `net8.0` needs that runtime installed. Check with `dotnet run -f net8.0`.
- **Building on a newer SDK proves little.** SDK 10 can compile `net8.0` code with `LangVersion` 14, but SDK 8 rejects that project. If older SDKs matter, build with one in CI or pin it in `global.json`.
- **Prefer real target frameworks over `netstandard2.0`** unless a real consumer needs it. It lacks helpers such as `ArgumentNullException.ThrowIfNull`, and it defaults to C# 7.3, where `<Nullable>enable</Nullable>` is an error until `LangVersion` is raised.
- **Only pin the SDK when reproducibility matters.** A `global.json` with `"rollForward": "disable"` fails outright on a machine without that exact version.

### Language version

`LangVersion` defaults from the target framework, not from the SDK, so `net8.0` gets C# 12 even on SDK 10. Raising it above the default can compile, but Microsoft doesn't support that combination, and features that need runtime support can fail. Leave it at the default unless the project has a reason, and write the number down rather than using `latest`, so a new SDK doesn't change the language under the project.

### Platform differences

The platform analyzer runs on every operating system, so a Linux build does warn when it reaches a Windows-only API. It's only a warning, though, so read it.

```csharp
if (OperatingSystem.IsWindows())
{
    Console.Beep(440, 200);
}
```

- Guard calls with `OperatingSystem.Is...()`, mark your own platform-specific APIs with `[SupportedOSPlatform]`, and keep both paths behind the same interface.
- The analyzer doesn't cover behavior. Paths are case-sensitive on Linux, default line endings differ, and `Environment.NewLine` isn't always what a file contains. Use `Path.Combine`, and handle `\r\n`, `\r`, and `\n` when reading text from elsewhere.
- Format and parse machine-readable data with `CultureInfo.InvariantCulture`.
- Legacy encodings aren't built in. `Encoding.GetEncoding(1252)` throws `NotSupportedException` until `Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)` has run.

### dotnetup

`dotnetup` is .NET's rootless SDK manager, similar to `rustup`, and is still in preview. Use `-h` on it or any subcommand for options, since they may change.

- It tracks channels such as `8` or `lts`, so name a channel rather than an exact patch: `dotnetup sdk install 8`.
- `dotnetup update` applies servicing updates, and `dotnetup self update` updates the tool.
- Installing or updating changes the user's machine, so ask first.

## Naming

Clarity at the point of use comes first, even at the cost of a few extra characters.

- Use .NET casing: PascalCase for namespaces, types, methods, properties, events, and constants; camelCase for parameters and locals; an `I` prefix for interfaces and a `T` prefix for type parameters. Private fields follow the codebase, usually `_camelCase` or `camelCase`.
- Name variables, parameters, and type parameters after their role, not their type. A `string` might be `title` or `identifier`, and with more than one type parameter, use `TKey` and `TResult` over bare `T`.
- Include every word needed to remove ambiguity at the call site, and drop words that carry no information there. Don't repeat the type in its members: `Order.Total`, not `Order.OrderTotal`.
- When a type is too weak to convey a role, such as two `int` parameters in a row, the name has to do that work.
- If the natural name collides with a C# keyword, rename it rather than use a verbatim identifier: `eventName`, not `@event`.

### Terminology

- Prefer a common word over an obscure one with the same meaning. Keep terms of art for when they capture something that would otherwise be lost, and use them in their established sense.
- Avoid abbreviations, especially nonstandard ones.
- Follow .NET precedent rather than simplifying a name at the cost of conflicting with convention. Two-letter acronyms stay uppercase (`IOException`, `IPAddress`), and longer ones are PascalCase (`HtmlParser`, `XmlReader`).

## Comments

- Keep comments sparse, since well-named code needs fewer of them.
- Write XML documentation (`///`) for all `public` APIs, with `<summary>` plus `<param>`, `<returns>`, and `<exception>` where they add information. Elsewhere, use it only when naming can't carry the meaning.
- Describe what the code does now, not its history. "Renamed from X" and "previously did Y" belong in commit messages.
- Write plain, short sentences without em dashes.

## Control Flow

Prefer flat, linear logic over deep branching.

- Use `switch` statements and expressions with pattern matching where they fit.
- Otherwise use `if`, and invert it when that removes a level of nesting.
- Filter loops with `if (...) continue;` at the top of the body, or with LINQ where performance doesn't matter: `foreach (var value in values.Where(v => v > 500)) { }`.
- Use pattern matching over a type check and a cast: `if (shape is Circle circle)`.

## Composition

Compose a type's behavior from small interfaces rather than sharing it through class inheritance. Each interface should represent one capability, and a type implements as many as it needs. This gives shared behavior and interchangeable types without a forced single lineage or a base class bloated with behavior only some subclasses use. Seal classes that aren't designed for inheritance.

```csharp
public interface IAdoptable
{
    string OwnerName { get; }
    string Name { get; }
}

public interface IProtecting
{
    void Protect();
}

public sealed class Cat(string ownerName, string name) : IAdoptable, IProtecting
{
    public string OwnerName { get; } = ownerName;
    public string Name { get; } = name;

    public void Protect() => Console.WriteLine($"{Name} protects the home.");
}

public static class AdoptableExtensions
{
    public static string Describe(this IAdoptable pet) => $"{pet.OwnerName} adopted {pet.Name}.";
}
```

### Interfaces

Always use the `I` prefix.

- If the type *is something*, use a noun: `IRepository`, `ILogger`.
- If *something is done to* it, or it can do something, use an `able` or `ible` adjective: `IComparable`, `IDisposable`.
- If it *is doing something*, an `ing` adjective can work (`IProtecting`), though .NET precedent leans toward nouns and `able`, so check the codebase first.

Using them:

- Accept the narrowest interface that does the job, such as `IEnumerable<T>` or `IReadOnlyList<T>` over `List<T>`, and return `IReadOnlyList<T>` to keep the concrete collection free to change.
- Use constrained generics sparingly (`where T : IComparable<T>`). They avoid casts and boxing. Static abstract members such as `INumber<T>` cover operators and factories, and need .NET 7 or later.
- Take care with `object`, `dynamic`, and unconstrained type parameters, which make ambiguous overloads easy to create. `dynamic` also gives up compile-time checking.

### Sharing behavior

Extension methods are the usual way to give every implementer of an interface the same behavior, as `Describe` does above. They work on any target framework, and on types you don't own. Put a utility on the most general type that fits.

Default interface methods are the closer match to Swift's protocol defaults, but they have limits. They can only be called through the interface, so `new Cat().Feed()` doesn't compile when `Feed` is a default that `Cat` doesn't implement itself, and `netstandard2.0` can't use them. Use them mainly to add a member to a published interface without breaking its implementers.

## API Design

### Names

- Use verb phrases for methods (`order.Cancel()`, `stream.CopyTo(destination)`) and nouns for properties and types.
- Follow the common patterns:
  - `Create` for factories: `Widget.Create(cogCount: 47)`.
  - `Get` for methods that compute or fetch a value.
  - `To` for conversions that produce a new object (`ToArray()`), and `As` for views over the same data (`AsSpan()`).
  - `Try` for operations expected to fail sometimes, returning `bool` with an `out` result: `int.TryParse(text, out var value)`.
- For mutating and nonmutating pairs, the mutating form is an imperative verb (`list.Sort()`), and the nonmutating form describes the result (`items.Order()`, `point.WithX(3)`). Records get this from `with` expressions.
- Booleans read as assertions about the receiver: `IsEmpty`, `HasValue`, `CanSeek`.

### Properties

Keep properties cheap, since readers assume access is free. If getting a value is O(n), does I/O, or can throw for reasons beyond invalid state, make it a method such as `ComputeChecksum()`. If a property can't avoid being expensive, document its complexity.

### Parameters

- Order parameters from most to least important, and keep the same order across overloads. Put `out` parameters and the `CancellationToken` last.
- Use named arguments when a literal's meaning isn't obvious: `Save(path, overwrite: true)`. If it still isn't clear, replace the `bool` with an enum.
- Skip names when the arguments can't usefully be told apart: `Math.Min(a, b)`.
- Past three or four parameters, consider an options type or record.

## Error Handling

- Use guard clauses at the top of a method so the main path stays flat.
- Validate arguments with the throw helpers where the target framework has them: `ArgumentNullException.ThrowIfNull(value)`, `ArgumentException.ThrowIfNullOrEmpty(name)`, `ArgumentOutOfRangeException.ThrowIfNegative(count)`.
- Throw the most specific exception that fits, and never `Exception` or `SystemException` themselves. Use the `Try` pattern for failures callers should expect.
- Catch only what you can handle. Catching `Exception` belongs at a top-level boundary, such as a request handler or command loop, where the failure is logged or reported. Rethrow with `throw;`, not `throw ex;`, to keep the stack trace.
- Enable nullable reference types and avoid the null-forgiving operator (`!`). Handle the `null` case or fix the annotation instead.
- Use `using` declarations and `await using` to dispose resources, and `try`/`finally` for other cleanup that has to run.
- Validate input and report failures explicitly. Don't return a success-shaped value that hides one.

## Async

- Don't make a method `async` unless it actually awaits something, and don't wrap synchronous work in `Task.Run` to look asynchronous.
- Add the `Async` suffix to methods returning `Task` or `ValueTask`, and accept a `CancellationToken` last.
- Await rather than blocking with `.Result` or `.Wait()`, which can deadlock.
- Use `async void` only for event handlers, since nothing can await it or catch its exceptions.
- Use `ConfigureAwait(false)` in library code that may run under a synchronization context.

## Project Structure

- Library conventions, such as a minimal `public` surface, full XML docs, and binary compatibility, assume other code will reference the project. In an executable that nothing references, keep them only where they aid clarity.
- A monolithic solution has one project holding all of its code, across as many files and folders as make sense. Small tools and GUI apps are often monolithic. This describes the current structure, so revisit it as the project grows.
- Put one top-level type in each file, named after the type. Folders mirror namespaces, and namespaces are file-scoped.
- Keep shared `using` directives in a `GlobalUsings.cs` file or `<Using>` items rather than repeating them.
- Default to `internal` and widen to `public` only when another project needs it. Use `InternalsVisibleTo` for test projects rather than making things public for tests.
- Once a solution has several projects, move shared settings into `Directory.Build.props` and package versions into `Directory.Packages.props`.
- For a NuGet package, set `EnablePackageValidation` so `dotnet pack` catches accidental breaking changes between versions.

## Tests

- Match the framework the solution already uses. For a new test project, default to xUnit, and name it after the project it tests with a `.Tests` suffix.
- Reduce duplication with shared helpers and fixtures, and generate cases with `[Theory]` and `[InlineData]`, or `[MemberData]` for computed cases. Each `InlineData` row is one case, so build combinations of two lists in a `[MemberData]` source.
- Prefer a hand-written fake implementing a small interface over a mocking library.
- Keep tests deterministic, with no real clocks, random seeds, network, or shared mutable state. `TimeProvider` makes time injectable on .NET 8 and later.
- Cover both the internals and the public API, since each can hide bugs the other misses.
- Keep fixtures realistic. Hand-written samples tend to contain only the shapes their author already knows about, so test against real data when it's available.
