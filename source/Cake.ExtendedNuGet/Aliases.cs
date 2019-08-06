using System;
using Cake.Core.Annotations;
using Cake.Core;
using Cake.Core.IO;
using System.Collections.Generic;
using System.Linq;

using Cake.Common.Tools.NuGet.Push;
using Cake.Common.Tools.NuGet;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using NuGet.Versioning;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol;
using NuGet.Common;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using Cake.Core.Packaging;
using System.Xml.Linq;
using NuGet.Packaging;

namespace Cake.ExtendedNuGet
{
    /// <summary>
    /// Extended NuGet Aliases
    /// </summary>
    [CakeAliasCategory("NuGet")]
    public static class ExtendedNuGetAliases
    {
        const string DefaultNuGetSource = "https://api.nuget.org/v3/index.json";

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
            var par = new NuGet.Packaging.PackageArchiveReader(System.IO.File.OpenRead(f));
            var id = par.GetIdentity();
            return id.Id;
        }

        /// <summary>
        /// Gets the Package Version from a .nupkg file
        /// </summary>
        /// <returns>The package version.</returns>
        /// <param name="context">The context.</param>
        /// <param name="file">The .nupkg file to read.</param>
        [CakeMethodAlias]
        [CakeNamespaceImport("NuGet")]
        public static NuGetVersion GetNuGetPackageVersion (this ICakeContext context, FilePath file)
        {
            var f = file.MakeAbsolute(context.Environment).FullPath;
            var par = new NuGet.Packaging.PackageArchiveReader(System.IO.File.OpenRead(f));
            var id = par.GetIdentity();
            return id.Version;
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
            var f = file.MakeAbsolute(context.Environment).FullPath;
            var par = new NuGet.Packaging.PackageArchiveReader(System.IO.File.OpenRead(f));
            var id = par.GetIdentity();

            return IsNuGetPublished (context, id.Id, id.Version, nugetSource);
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
            var v = NuGetVersion.Parse (version);

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
        public static bool IsNuGetPublished (this ICakeContext context, string packageId, NuGetVersion version, string nugetSource = DefaultNuGetSource)
        {
            var nuSource = Repository.Factory.GetCoreV3(nugetSource);
            var nuCache = new SourceCacheContext();
            var nuLogger = NullLogger.Instance;

            var tcsPublished = new TaskCompletionSource<bool>();

            Task.Run(async () =>
            {
                var pkgRes = await nuSource.GetResourceAsync<FindPackageByIdResource>();

                try
                {
                    var pkgInfo = await pkgRes.GetDependencyInfoAsync(packageId, new NuGetVersion(version.ToString()), nuCache, nuLogger, default);
                    tcsPublished.TrySetResult(pkgInfo?.PackageIdentity?.Id?.Equals(packageId, StringComparison.OrdinalIgnoreCase) ?? false);
                }
                catch
                {
                    tcsPublished.TrySetResult(false);
                }
            });

            return tcsPublished.Task.Result;
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
        public static void PublishNuGets (this ICakeContext context, string nugetSource, string apiKey, PublishNuGetsSettings settings, params string [] nupkgFileGlobbingPatterns)
        {
            PublishNuGets (context, nugetSource, nugetSource, apiKey, settings, nupkgFileGlobbingPatterns);
        }

        /// <summary>
        /// Looks for and attempts to publish NuGet packages matching the globbing patterns
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="readSource">The NuGet Server to check for existing packages on.</param>
        /// <param name="publishSource">The NuGet Server to push packages to.</param>
        /// <param name="apiKey">The NuGet API key.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="nupkgFileGlobbingPatterns">The file globbing patterns to find NuGet packages with.</param>
        [CakeMethodAlias]
        public static void PublishNuGets (this ICakeContext context, string readSource, string publishSource, string apiKey, PublishNuGetsSettings settings, params string[] nupkgFileGlobbingPatterns)
        {
            foreach (var pattern in nupkgFileGlobbingPatterns)
            {
                var files = context.GetFiles(pattern);
                if (files == null || !files.Any())
                    continue;

                foreach (var file in files)
                {
                    if (!settings.ForcePush 
                        && !string.IsNullOrEmpty (readSource) ? IsNuGetPublished (context, file, readSource) : IsNuGetPublished (context, file))
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

                            if (!string.IsNullOrEmpty (publishSource))
                                ns.Source = publishSource;
                            
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

        /// <summary>
        /// NuGet project dependencies.
        /// </summary>
        /// <param name="context">
        /// The <c>context</c>.
        /// </param>
        /// <param name="path">
        /// A relative <see cref="DirectoryPath"/> where packages.config resides.
        /// </param>
        /// <returns>
        /// A <see cref="IEnumerable{PackageReference}"/>.
        /// </returns>
        [CakeMethodAlias]
        public static IEnumerable<NuGet.Packaging.PackageReference> GetPackageReferences(this ICakeContext context, DirectoryPath path)
        {
            if (!path.IsRelative)
            {
                throw new CakeException("DirectoryPath must be relative!");
            }

            var packagePath = path.CombineWithFilePath(new FilePath("packages.config"));
            if (!System.IO.File.Exists(packagePath.FullPath))
            {
                throw new CakeException(string.Format("Could not find a packages.config file in '{0}'", path.FullPath));
            }

            var document = XDocument.Load(packagePath.FullPath);
            var reader = new PackagesConfigReader(document);

            return reader.GetPackages();
        }

        /// <summary>
        /// Get a NuGet project dependency by <paramref name="packageId"/>.
        /// </summary>
        /// <param name="context">
        /// This <see cref="ICakeContext"/>.
        /// </param>
        /// <param name="path">
        /// A relative <see cref="DirectoryPath"/> where packages.config resides.
        /// </param>
        /// <param name="packageId">
        /// The package Name.
        /// </param>
        /// <returns>
        /// A <see cref="IEnumerable{PackageReference}"/>.
        /// </returns>
        [CakeMethodAlias]
        public static NuGet.Packaging.PackageReference GetPackageReference(this ICakeContext context, DirectoryPath path, string packageId)
        {
            if (!path.IsRelative)
            {
                throw new CakeException("DirectoryPath must be relative!");
            }

            var packagePath = path.CombineWithFilePath(new FilePath("packages.config"));
            if (!System.IO.File.Exists(packagePath.FullPath))
            {
                throw new CakeException(string.Format("Could not find a packages.config file in '{0}'", path.FullPath));
            }

            var document = XDocument.Load(packagePath.FullPath);
            var reader = new PackagesConfigReader(document);

            return reader.GetPackages().FirstOrDefault(x => x.PackageIdentity.Id.Equals(packageId, StringComparison.OrdinalIgnoreCase));
        }
    }
}
