using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;

namespace EfCore.FirestoreProvider.Extensions;

public class FirestoreDbContextOptionsExtensionInfo : DbContextOptionsExtensionInfo
{
    public FirestoreDbContextOptionsExtensionInfo(IDbContextOptionsExtension extension) : base(extension)
    {
    }

    public override bool IsDatabaseProvider => true;

    public override string LogFragment => "Firestore";

    public override int GetServiceProviderHashCode()
    {
        var extension = (FirestoreDbContextOptionsExtension)Extension;
        return HashCode.Combine(extension.ProjectId, extension.ServiceAccountPath);
    }

    public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
    {
        if (other is not FirestoreDbContextOptionsExtensionInfo otherInfo)
            return false;

        var thisExtension = (FirestoreDbContextOptionsExtension)Extension;
        var otherExtension = (FirestoreDbContextOptionsExtension)otherInfo.Extension;

        return thisExtension.ProjectId == otherExtension.ProjectId &&
               thisExtension.ServiceAccountPath == otherExtension.ServiceAccountPath;
    }

    public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
    {
        var extension = (FirestoreDbContextOptionsExtension)Extension;
        debugInfo["Firestore:ProjectId"] = extension.ProjectId ?? "<null>";
        debugInfo["Firestore:ServiceAccountPath"] = extension.ServiceAccountPath ?? "<null>";
    }
}
