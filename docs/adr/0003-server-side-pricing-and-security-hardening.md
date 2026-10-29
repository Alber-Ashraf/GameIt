Last updated: 2026-10-29

# ADR 0003 — Server-Side Price Calculation, RBAC Enforcement & IDOR Prevention

- **Status:** Proposed (Phase 2)
- **Context:** Phase 1 audit uncovered critical vulnerabilities: `CreatePurchaseCommandHandler` trusts client-supplied `AmountPaid` and skips `Library` provisioning for free games; `CategoryController` and `DiscountController` lack `[Authorize(Roles = "Administrator")]`; `DeleteReviewCommandHandler` has its ownership check commented out; `AuthService.Register` assigns `"Employee"` role instead of `"User"`; and `ExceptionMiddleware` leaks stack traces in 500 responses.
- **Decision:** Compute purchase price exclusively on the server from `Game.Price`, `Game.IsFree`, and active `Discount.Percentage`; provision `Library` entries immediately for `$0.00`/free games and idempotently on Stripe webhooks; enforce `[Authorize(Roles = "Administrator")]` on all catalog/category/discount mutations; enforce review ownership or `Administrator` role on review deletion; assign `"User"` role on registration; and redact stack traces outside Development.
- **Consequences:** Eliminates client-side price tampering, privilege escalation, and BOLA/IDOR vulnerabilities while making free and paid purchase flows reliable end-to-end.
