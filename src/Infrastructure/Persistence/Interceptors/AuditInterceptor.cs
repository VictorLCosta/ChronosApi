using CrossCutting.Interfaces;

using Domain.Common;
using Domain.Contracts;

using Infrastructure.Audit;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

public class AuditInterceptor(ICurrentUserService currentUser, TimeProvider timeProvider) : SaveChangesInterceptor
{
    private static readonly AsyncLocal<bool> _isSaving = new();

    private static readonly HashSet<string> IgnoredPropertyNames =
    [
        nameof(BaseEntity.Created),
        nameof(BaseEntity.CreatedBy),
        nameof(BaseEntity.LastModified),
        nameof(BaseEntity.LastModifiedBy),
        "SearchVector"
    ];

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        return await base.SavedChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        if (_isSaving.Value)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);
        }

        try
        {
            UpdateEntities(eventData.Context);
            await PublishAuditTrailsAsync(eventData, cancellationToken);
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        finally
        {
            _isSaving.Value = false;
        }
    }

    private async Task PublishAuditTrailsAsync(DbContextEventData eventData, CancellationToken cancellationToken)
    {
        if (eventData.Context == null) return;

        var trails = new List<TrailDto>();
        var utcNow = timeProvider.GetUtcNow();
        var userId = currentUser.GetUserId();

        foreach (var entry in eventData.Context.ChangeTracker.Entries<BaseEntity>()
            .Where(x => x.State is EntityState.Added or EntityState.Deleted or EntityState.Modified))
        {
            var trail = new TrailDto()
            {
                Id = Guid.NewGuid(),
                TableName = entry.Entity.GetType().Name,
                UserId = userId,
                DateTime = utcNow
            };

            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    continue;
                }

                string propertyName = property.Metadata.Name;

                if (property.Metadata.IsPrimaryKey())
                {
                    trail.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                if (ShouldIgnoreProperty(propertyName))
                {
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        trail.Type = AuditType.Create;
                        trail.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        trail.Type = AuditType.Delete;
                        trail.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            var isTrashOperation =
                                entry.Entity is ITrashable &&
                                property.Metadata.Name == nameof(ITrashable.IsTrashed) &&
                                property.OriginalValue is false &&
                                property.CurrentValue is true;

                            if (isTrashOperation)
                            {
                                trail.ModifiedProperties.Add(propertyName);
                                trail.Type = AuditType.Delete;
                                trail.OldValues[propertyName] = property.OriginalValue;
                                trail.NewValues[propertyName] = property.CurrentValue;
                            }
                            else if (property.OriginalValue?.Equals(property.CurrentValue) == false)
                            {
                                trail.ModifiedProperties.Add(propertyName);
                                trail.Type = AuditType.Update;
                                trail.OldValues[propertyName] = property.OriginalValue;
                                trail.NewValues[propertyName] = property.CurrentValue;
                            }
                            else
                            {
                                property.IsModified = false;
                            }
                        }
                        break;
                }
            }

            if (HasAuditableChanges(trail))
            {
                trails.Add(trail);
            }
        }

        if (trails.Count == 0)
        {
            return;
        }

        var auditTrails = trails.Select(static trail => trail.ToAuditTrail()).ToList();

        await eventData.Context.Set<AuditTrail>().AddRangeAsync(auditTrails, cancellationToken).ConfigureAwait(false);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var utcNow = timeProvider.GetUtcNow();
        var currentUserId = currentUser.GetUserId();

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = currentUserId;
                    entry.Entity.Created = utcNow;
                }
                entry.Entity.LastModifiedBy = currentUserId;
                entry.Entity.LastModified = utcNow;
            }

        }
    }

    private static bool HasAuditableChanges(TrailDto trail) =>
        trail.OldValues.Count > 0 ||
        trail.NewValues.Count > 0 ||
        trail.ModifiedProperties.Count > 0;

    private static bool ShouldIgnoreProperty(string propertyName) =>
        IgnoredPropertyNames.Contains(propertyName);
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        return entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
    }
}