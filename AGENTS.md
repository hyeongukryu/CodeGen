# Repository Guidelines

## Project Structure & Module Organization

`CodeGen/CodeGen/` is the main .NET 10 library/web host. `Analysis/` inspects ASP.NET APIs, `Generation/` emits TypeScript SDK output, and `Web/` holds the embedded web UI asset. `CodeGen/CodeGen.Tests/` contains xUnit tests that mirror runtime areas, including `Generation/` coverage for TypeScript output. `CodeGen/CodeGen.Example/` is the sample ASP.NET app for manual validation. `code-gen-cli/` contains the TypeScript CLI consumer, and `code-gen-web/` contains the Vite/React UI.

## Build, Test, and Development Commands

Run `dotnet build CodeGen/CodeGen.sln` to compile the .NET projects and `dotnet test CodeGen/CodeGen.sln` to run xUnit tests. Use `dotnet run --project CodeGen/CodeGen.Example` to exercise the example API locally. In `code-gen-cli/`, run `pnpm build` to create `out/bundle.js`; `./build.sh` also copies that bundle into `code-gen-web/src/bundle.js`. In `code-gen-web/`, run `pnpm dev` for local UI work, `pnpm build` for a production build, and `./build.sh` to copy `dist/index.html` into `CodeGen/CodeGen/Web/index.html`. If you change the TypeScript API result contract or UI/CLI consumption flow, rebuild both `code-gen-cli/` and `code-gen-web/`.

## Coding Style & Naming Conventions

C# files use 4-space indentation, file-scoped namespaces, nullable reference types, and PascalCase for public types and members. TypeScript follows the surrounding file style: `code-gen-web/` currently uses mostly 2-space indentation, while `code-gen-cli/` uses mostly 4 spaces. Keep React components in PascalCase files such as `CodeGenApi.tsx`; use camelCase for helpers and locals. No dedicated formatter or linter config is committed, so keep diffs small and let `dotnet build` and `tsc` catch issues.

## API Contract Notes

`/code-gen-api` returns structured JSON responses. `format=typescript-api` returns `typeScriptApi`, `files`, and `errorMessages`. Split output is represented through `files[]`. The generator validates required `JsonSerializerOptions` assumptions and reports unsupported configurations through `errorMessages` instead of adapting converter behavior at runtime.

## Generated Assets

Do not hand-edit `code-gen-web/src/bundle.js` or `CodeGen/CodeGen/Web/index.html`; regenerate them through the package build scripts. When a source change alters generated output, commit both the source edit and the refreshed generated asset. Do not hand-edit consumer-side generated SDK files either; update the server contract and regenerate.
