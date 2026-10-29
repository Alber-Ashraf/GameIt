Last updated: 2026-10-29

# ADR 0002 — Standalone Blazor WebAssembly with `Core -> Shared -> Features` Vertical Slices

- **Status:** Proposed (Phase 2)
- **Context:** `GameIt.BlazorUI` is a standalone Blazor WebAssembly project (`net8.0`) where 5 of 7 services are empty `{}` stubs, JWT tokens are never attached to outgoing HTTP requests, and 27 of 33 backend capabilities have no UI.
- **Decision:** Standardize on standalone Blazor WebAssembly structured into `Core/` (HTTP `DelegatingHandler` for automatic JWT attachment, UTC-safe `CustomAuthStateProvider`, `ProblemDetailsParser`, `MainLayout`/`AuthLayout`, `ToastService`/`ModalService`), `Shared/UI/` (10 reusable enterprise primitives), and `Features/<Module>/` vertical slices (`Auth`, `Catalog`, `CartAndCheckout`, `Library`, `Admin`).
- **Consequences:** Every API call automatically carries the user's Bearer token via the `HttpClient` pipeline, UI markup stays decoupled from HTTP/state orchestration, and every view enforces the 4 mandatory UX states (Skeleton, Empty, Error, Populated).
