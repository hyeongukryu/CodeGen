# CodeGen Consumer Guide

## Purpose
This guide is for an AI coding agent working on an application that consumes `CodeGen.dll`. The rule is simple: change the API contract in the .NET server first, then regenerate the TypeScript SDK. Do not hand-edit generated API files.

## Standard Workflow
1. Update controllers, request DTOs, response DTOs, or route/query/body bindings in the .NET server.
2. Confirm the server exposes CodeGen with `builder.Services.AddCodeGen(...)` and `app.MapCodeGen()`.
3. Start the server.
4. In the web project that consumes the SDK, run `pnpm run api` or `pnpm run api:watch`.
5. Build or type-check the web project and fix any real call-site breakage.

## Server-Side Rules
- The source of truth is the ASP.NET contract: controller method signature, route template, HTTP method, and C# types.
- `[FromRoute]`, `[FromQuery]`, `[FromBody]`, and the action return type are reflected into the generated SDK.
- Use `[Command]` for actions that intentionally return no response body.
- Use `[CodeGenIgnore]` to exclude an action from generation.
- Use `Microsoft.AspNetCore.Http.TagsAttribute` when the consumer app generates SDKs by tag.

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

## Generation Mode Defaults
- For browser or React web clients, prefer split output. It is usually the better default for bundle-based apps, and `swr=true` is a good fit when the app already uses SWR hooks.
- For Node.js CLI tools, server-side TypeScript, or non-React consumers, use `swr=false`. Split output is optional there; a single generated file can be simpler if tree-shaking is irrelevant.
- Match the consuming app’s existing conventions before changing generation mode just because a different mode also works.

## Validation Checklist
- The server builds and starts successfully.
- `pnpm run api` completes successfully.
- The generated diff matches the intended contract change.
- The web project build or type-check passes.

## Common Mistakes
- Running `pnpm run api` before the server is up.
- Editing generated TypeScript files directly.
- Forcing frontend types to match without changing the server contract.
