using System;
using Cake.Core.Annotations;
using Cake.Core;
using Cake.Core.IO;
using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace Cake.ExtendedNuGet
{
    /// <summary>
    /// Extended NuGet Aliases
    /// </summary>
    [CakeAliasCategory("NuGet")]
    [CakeNamespaceImport("NuGet")]
    public static class ExtendedNuGetAliases
    {
        const string DefaultNuGetSource = "https://www.nuget.org/api/v2";

        /// <summary>
        /// Gets the Package Id from a .nupkg file
        /// </summary>
        /// <returns>The package Id.</returns>
        /// <param name="context">The context.</param>
        /// <param name="file">The .nupkg file to read.</param>
        [CakeMethodAlias]
        public static string GetNuGetPackageId (this ICakeContext context, FilePath file)
        {
            var f = file.MakeAbsolute (context.Environment).FullPath;

            var p = new ZipPackage (f);

            return p.Id;
        }

        /// <summary>
        /// Gets the Package Version from a .nupkg file
        /// </summary>
        /// <returns>The package version.</returns>
        /// <param name="context">The context.</param>
        /// <param name="file">The .nupkg file to read.</param>
        [CakeMethodAlias]
        public static SemanticVersion GetNuGetPackageVersion (this ICakeContext context, FilePath file)
        {
            var f = file.MakeAbsolute (context.Environment).FullPath;

            var p = new ZipPackage (f);

            return p.Version;
        }

        /// <summary>
        /// Determines if a .nupkg is already published at the given NuGet package source.
        /// </summary>
        /// <returns><c>true</c> if the .nupkg is published at the given NuGet package source.</returns>
        /// <param name="context">The context.</param>
        /// <param name="file">The .nupkg file.</param>
        /// <param name="nugetSource">The NuGet package source.</param>
        [CakeMethodAlias]
        public static bool IsNuGetPublished (this ICakeContext context, FilePath file, string nugetSource = DefaultNuGetSource)
        {
            var f = file.MakeAbsolute (context.Environment).FullPath;

            var pkg = new ZipPackage (f);

            return IsNuGetPublished (context, pkg.Id, pkg.Version, nugetSource);
        }

        /// <summary>
        /// Determines if a .nupkg is already published at the given NuGet package source.
        /// </summary>
        /// <returns><c>true</c> if the .nupkg is published at the given NuGet package source.</returns>
        /// <param name="context">The context.</param>
        /// <param name="packageId">The NuGet package Id.</param>
        /// <param name="version">The NuGet package Version.</param>
        /// <param name="nugetSource">The NuGet package source.</param>
        [CakeMethodAlias]
        public static bool IsNuGetPublished (this ICakeContext context, string packageId, string version, string nugetSource = DefaultNuGetSource)
        {
            var v = SemanticVersion.Parse (version);

            return IsNuGetPublished (context, packageId, v, nugetSource);
        }

        /// <summary>
        /// Determines if a .nupkg is already published at the given NuGet package source.
        /// </summary>
        /// <returns><c>true</c> if the .nupkg is published at the given NuGet package source.</returns>
        /// <param name="context">The context.</param>
        /// <param name="packageId">The NuGet package Id.</param>
        /// <param name="version">The NuGet package Version.</param>
        /// <param name="nugetSource">The NuGet package source.</param>
        [CakeMethodAlias]
        public static bool IsNuGetPublished (this ICakeContext context, string packageId, SemanticVersion version, string nugetSource = DefaultNuGetSource)
        {
            var repo = PackageRepositoryFactory.Default.CreateRepository (nugetSource);

            var packages = repo.FindPackagesById (packageId);

            //Filter the list of packages that are not Release (Stable) versions
            var exists = packages.Any (p => p.Version == version);

            return exists;
        }
    }
}
