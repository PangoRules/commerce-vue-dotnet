# Quality Checklists

## PR Checklist

Use this checklist before creating a pull request.

### Code Quality

- [ ] Code compiles without errors
- [ ] No TypeScript `any` types (frontend)
- [ ] Nullable reference types handled (backend)
- [ ] No hardcoded values (use config/constants)
- [ ] No commented-out code
- [ ] No console.log statements (except intentional debugging)

### Testing

- [ ] Unit tests pass: `dotnet test` (backend)
- [ ] Unit tests pass: `npm run test:run` (frontend)
- [ ] Coverage maintained: 75%+ on changed code
- [ ] New features have corresponding tests
- [ ] Edge cases and error scenarios tested

### Backend Specific

- [ ] Input validation with FluentValidation
- [ ] Proper HTTP status codes returned
- [ ] DTOs used (entities not exposed)
- [ ] Async/await used for I/O
- [ ] ILogger used for logging

### Frontend Specific

- [ ] Build passes: `npm run build`
- [ ] Components under 200 lines
- [ ] Composables used for business logic
- [ ] ApiResult pattern followed
- [ ] i18n strings added for new text
- [ ] Vuetify components used consistently

### Database

- [ ] Migration created for schema changes
- [ ] Migration tested locally
- [ ] No breaking changes without migration strategy

### Documentation

- [ ] Complex logic has comments
- [ ] Public APIs have JSDoc/XML comments
- [ ] README updated if needed

### Security

- [ ] No secrets in code
- [ ] Input sanitized/validated
- [ ] SQL injection prevented (EF Core)
- [ ] XSS prevented (Vue escaping)

## Feature Completion Checklist

Use this when completing a full feature.

### Backend Feature

- [ ] Entity created in `Commerce.Repositories/Entities/`
- [ ] EF configuration in `Commerce.Repositories/Configurations/`
- [ ] Migration created and applied
- [ ] Repository interface and implementation
- [ ] DTOs in `Commerce.Services/DTOs/`
- [ ] Service interface in `Commerce.Services/Interfaces/`
- [ ] Service implementation with business logic
- [ ] Controller with proper routes and status codes
- [ ] Unit tests for service layer
- [ ] Integration tests for API endpoints
- [ ] Swagger documentation appears correctly

### Frontend Feature

- [ ] Types defined in `src/types/api/`
- [ ] API service in `src/services/`
- [ ] API service tests
- [ ] Composable in `src/composables/`
- [ ] Composable tests
- [ ] Components in `src/components/`
- [ ] Component tests for key interactions
- [ ] Page created (auto-routes)
- [ ] i18n translations (en.json, es.json)
- [ ] Loading states handled
- [ ] Error states handled
- [ ] Success feedback (toast/notification)

### Integration

- [ ] End-to-end flow works
- [ ] API contract matches frontend expectations
- [ ] Error messages user-friendly

## Release Checklist (TODO)

This checklist is for future production releases.

### Pre-Release

- [ ] All tests pass in CI
- [ ] No critical security vulnerabilities
- [ ] Dependencies up to date
- [ ] Database migrations reviewed
- [ ] Feature flags configured
- [ ] Rollback plan documented

### Deployment

- [ ] Backup database
- [ ] Run migrations
- [ ] Deploy backend
- [ ] Verify health checks
- [ ] Deploy frontend
- [ ] Smoke test critical paths

### Post-Release

- [ ] Monitor error rates
- [ ] Monitor performance metrics
- [ ] Check logs for anomalies
- [ ] Verify user-facing features
- [ ] Update release notes

## Code Review Guidelines

### What to Check

1. **Functionality**: Does the code do what it's supposed to?
2. **Design**: Is the code well-structured and maintainable?
3. **Patterns**: Are established patterns followed?
4. **Tests**: Are tests comprehensive and meaningful?
5. **Performance**: Any obvious performance issues?
6. **Security**: Any security concerns?

### Review Etiquette

- Be constructive and specific
- Explain the "why" for suggestions
- Distinguish between blocking issues and suggestions
- Acknowledge good work

### Common Issues to Flag

- Missing error handling
- Hardcoded strings
- Missing tests
- Overly complex logic
- Inconsistent naming
- Missing validation

## Definition of Done

A feature is "done" when:

1. ✓ Code complete and reviewed
2. ✓ Tests written and passing
3. ✓ Coverage target met (75%+)
4. ✓ No known bugs
5. ✓ Documentation updated (if applicable)
6. ✓ i18n complete (both languages)
7. ✓ Works in Docker environment
8. ✓ PR approved and merged
