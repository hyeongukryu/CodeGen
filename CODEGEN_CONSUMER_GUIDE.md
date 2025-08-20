# CodeGen Consumer Guide

## Purpose

This guide is for an AI coding agent working on an application that consumes `CodeGen.dll`. The rule is simple: change the API contract in the .NET server first, then regenerate the TypeScript SDK. Do not hand-edit generated API files.

## Standard Workflow

1. Update controllers, request DTOs, response DTOs, or route/query/body bindings in the .NET server.
2. Confirm the server exposes CodeGen with `builder.Services.AddCodeGen(...)` and `app.MapCodeGen()`.
3. Start the server.
4. In the web project that consumes the SDK, run `pnpm run api` or `pnpm run api:watch`.
5. Build or type-check the consumer project and fix any real call-site breakage.

## Server-Side Rules

- The source of truth is the ASP.NET contract: controller method signature, route template, HTTP method, and C# types.
- `[FromRoute]`, `[FromQuery]`, `[FromBody]`, and the action return type are reflected into the generated SDK.
- Use `[Command]` for actions that intentionally return no response body.
- Use `[CodeGenIgnore]` to exclude an action from generation.
- Use `Microsoft.AspNetCore.Http.TagsAttribute` when the consumer app generates SDKs by tag.
- If DTO or controller short names collide, CodeGen may expand names with namespace prefixes rather than failing generation.

## SDK Regeneration

Most consumer apps wire scripts like this:

```json
{
  "scripts": {
    "api": "node update-api.js http://localhost:5000/",
    "api:watch": "CODEGEN_CLI_WATCH=Y CODEGEN_CLI_WATCH_INTERVAL=2000 node update-api.js http://localhost:5000/"
  }
}
```

- `update-api.js` calls the server’s `code-gen-api` endpoint and rewrites generated files under `src/api/client` and `src/api/server`.
- Those generated files are outputs, not authoring surfaces. If the result is wrong, fix the server contract and regenerate.
- The CLI commonly passes `../client.config` and `../server.config` as `configFilePath`, but follow the consuming app’s existing script layout.
- The generator expects the target directories to already exist.

## Starting the Server for Generation

The SDK generator talks to a live app, so "the server builds" is not enough. The app must start in the same environment and network context that the SDK update command can reach.

- Some apps expose CodeGen only in Development, for example inside `if (env.IsDevelopment())`. If you bypass `launchSettings.json` with `--no-launch-profile`, set the environment explicitly, such as `ASPNETCORE_ENVIRONMENT=Development dotnet run --project <server.csproj> --no-launch-profile --urls http://localhost:5000`.
- If `launchSettings.json` is malformed or contains values with the wrong JSON type, `dotnet run` can fail before the app starts. Using `--no-launch-profile` is fine, but only if you supply the environment variables and configuration that the app normally gets from the launch profile.
- In sandboxed agent environments, localhost servers and clients may not see each other across sandbox boundaries. If `pnpm run api` cannot connect even though the server appears to be running, run both the server and the SDK regeneration command in the same approved network context.
- Before regenerating, verify the endpoint directly when possible. A request to `/code-gen-api` with the same query parameters used by the consumer should return a successful JSON response. If the CLI only says `Failed to get code`, first determine whether the server is unreachable, CodeGen was not mapped, or CodeGen returned generation errors.

## CodeGen API Contract

`/code-gen-api` is a structured JSON API.

For `format=typescript-api`, the response shape is:

```json
{
  "typeScriptApi": "string",
  "files": [
    {
      "fileName": "_types.ts",
      "content": "..."
    }
  ],
  "errorMessages": []
}
```

- When `split=false`, consumers usually read `typeScriptApi`.
- When `split=true`, consumers should use `files[]` as the source of truth.
- `errorMessages` is the official failure channel for invalid controller definitions or unsupported serializer settings.

## Serializer Assumptions

CodeGen assumes a specific `JsonSerializerOptions` configuration. It does not adapt generated converters to arbitrary serializer settings.

- Property names must serialize as camelCase.
- `DefaultIgnoreCondition` must be `Never`.
- `NumberHandling` must include both `WriteAsString` and `AllowReadingFromString`.
- `ReferenceHandler` must match the `AddCodeGen(preserveReferences: ...)` setting.
- Custom `JsonConverter`, `JsonPropertyName`, conditional `JsonIgnore`, and polymorphic DTO shapes are not supported for generation.

If these assumptions are violated, generation fails with explicit `errorMessages`.

## Generation Mode Defaults

- For browser or React web clients, prefer split output. It is usually the better default for bundle-based apps, and `swr=true` is a good fit when the app already uses SWR hooks.
- For Node.js CLI tools, server-side TypeScript, or non-React consumers, use `swr=false`. Split output is optional there; a single generated file can be simpler if tree-shaking is irrelevant.
- Match the consuming app’s existing conventions before changing generation mode just because a different mode also works.

## Validation Checklist

- The server builds and starts successfully.
- The `/code-gen-api` endpoint is reachable from the SDK regeneration process.
- `pnpm run api` completes successfully.
- The generated diff matches the intended contract change.
- The consumer project build or type-check passes.
- If generation fails, inspect `errorMessages` before trying to patch generated files by hand.

## Common Mistakes

- Running `pnpm run api` before the server is up.
- Starting the server outside Development when CodeGen is only mapped in Development.
- Letting the server and SDK update command run in different sandbox or network contexts.
- Editing generated TypeScript files directly.
- Forcing frontend types to match without changing the server contract.
- Reading `typeScriptApi` instead of `files[]` when `split=true`.
- Trying to “fix” unsupported JSON serialization behavior in the consumer instead of aligning the server serializer settings.
