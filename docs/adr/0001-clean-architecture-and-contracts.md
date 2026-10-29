Last updated: 2026-10-29

# ADR 0001 — Physical `src/` + `tests/` Layout and Shared `GameIt.Contracts`

- **Status:** Proposed (Phase 2)
- **Context:** The repository is currently nested inside a redundant `GameIt/GameIt.Api/` folder with all 9 projects in a flat directory. Furthermore, `GameIt.BlazorUI` relies on a 2,000-line NSwag-generated `ServiceClient.cs` and duplicate `AutoMapper` ViewModels that are out of sync with the backend API and fail at runtime (`UpdateGameCommand` unmapped, `AddBearerToken` never called).
- **Decision:** Flatten the repository to the true git root (`GameIt/`), organize projects under `src/Core`, `src/Infrastructure`, `src/Presentation`, and `tests/` with centralized `Directory.Build.props`, and introduce `src/Core/GameIt.Contracts` referenced by both `GameIt.Application`/`GameIt.API` and `GameIt.BlazorUI`.
- **Consequences:** Eliminates code-generation drift and runtime AutoMapper exceptions in Blazor while preserving strict Clean Architecture boundaries (`GameIt.BlazorUI` never references `GameIt.Application` or `GameIt.Domain`).
