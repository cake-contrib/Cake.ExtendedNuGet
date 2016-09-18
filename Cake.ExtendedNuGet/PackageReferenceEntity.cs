namespace Cake.ExtendedNuGet
{
    using System;
    using System.Runtime.Versioning;

    using NuGet;

    public class PackageReferenceEntity : IEquatable<PackageReferenceEntity>
    {
        public string Id { get; private set; }

        public string Version { get; private set; }

        public IVersionSpec VersionConstraint { get; set; }

        public FrameworkName TargetFramework { get; private set; }

        public bool IsDevelopmentDependency { get; private set; }

        public bool RequireReinstallation { get; private set; }

        public PackageReferenceEntity(PackageReference reference)
        {
            Id = reference.Id;
            Version = reference.Version.ToString();
            VersionConstraint = reference.VersionConstraint;
            TargetFramework = reference.TargetFramework;
            IsDevelopmentDependency = reference.IsDevelopmentDependency;
            RequireReinstallation = reference.RequireReinstallation;
        }

        public override bool Equals(object obj)
        {
            PackageReferenceEntity other = obj as PackageReferenceEntity;
            if (other != null)
            {
                return Equals(other);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() * 3137 + (Version == null ? 0 : Version.GetHashCode());
        }

        public override string ToString()
        {
            if (Version == null)
            {
                return Id;
            }

            if (VersionConstraint == null)
            {
                return Id + " " + Version;
            }

            return Id + " " + Version + " (" + VersionConstraint + ")";
        }

        public bool Equals(PackageReferenceEntity other)
        {
            if (other == null)
            {
                return false;
            }

            if (Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase))
            {
                return Version == other.Version;
            }

            return false;
        }
    }
}
