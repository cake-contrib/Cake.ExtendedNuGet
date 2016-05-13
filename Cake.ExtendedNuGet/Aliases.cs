using System;
using Cake.Core.Annotations;
using Cake.Core;
using Cake.Core.IO;
using System.Collections.Generic;
using System.Linq;
using NuGet;
using Cake.Common.Tools.NuGet.Push;
using Cake.Common.Tools.NuGet;
using Cake.Common.Diagnostics;
using Cake.Common.IO;

namespace Cake.ExtendedNuGet
{
    /// <summary>
    /// Extended NuGet Aliases
    /// </summary>
    [CakeAliasCategory("NuGet")]
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
        [CakeNamespaceImport("NuGet")]
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
        [CakeNamespaceImport("NuGet")]
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
        [CakeNamespaceImport("NuGet")]
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
        [CakeNamespaceImport("NuGet")]
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
        [CakeNamespaceImport("NuGet")]
        public static bool IsNuGetPublished (this ICakeContext context, string packageId, SemanticVersion version, string nugetSource = DefaultNuGetSource)
        {
            var repo = PackageRepositoryFactory.Default.CreateRepository (nugetSource);

            var packages = repo.FindPackagesById (packageId);

            //Filter the list of packages that are not Release (Stable) versions
            var exists = packages.Any (p => p.Version == version);

            return exists;
        }


        /// <summary>
        /// Looks for and attempts to publish NuGet packages matching the globbing patterns
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="nugetSource">The NuGet Server.</param>
        /// <param name="apiKey">The NuGet API key.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="nupkgFileGlobbingPatterns">The file globbing patterns to find NuGet packages with.</param>
        [CakeMethodAlias]
        public static void PublishNuGets (this ICakeContext context, string nugetSource, string apiKey, PublishNuGetsSettings settings, params string[] nupkgFileGlobbingPatterns)
        {
            foreach (var pattern in nupkgFileGlobbingPatterns)
            {
                var files = context.GetFiles(pattern);
                if (files == null || !files.Any())
                    continue;

                foreach (var file in files)
                {
                    if (!settings.ForcePush 
                        && !string.IsNullOrEmpty (nugetSource) ? IsNuGetPublished (context, file, nugetSource) : IsNuGetPublished (context, file))
                    {
                        context.Information("Already published: {0}", file);
                        continue;
                    }

                    context.Information("Attempting to publish {0}", file);

                    int attempts = 0;
                    bool success = false;

                    while (attempts < settings.MaxAttempts)
                    {
                        attempts++;

                        try {
                            var ns = new NuGetPushSettings {
                                ApiKey = apiKey
                            };

                            if (!string.IsNullOrEmpty (nugetSource))
                                ns.Source = nugetSource;
                            
                            context.NuGetPush (file, ns);
                            success = true;
                            break;
                        } catch {
                            context.Warning("Attempt #{0} of {1} failed...", attempts, settings.MaxAttempts);
                        }
                    }

                    if (!success)
                    {
                        var msg = "Maximum # of attempts ({0}) to publish exceeded";
                        context.Error(msg, settings.MaxAttempts);
                        throw new Exception(string.Format (msg, settings.MaxAttempts));
                    }

                    context.Information("Published Package successfully: {0}", file);
                }
            }
        }
    }
}
