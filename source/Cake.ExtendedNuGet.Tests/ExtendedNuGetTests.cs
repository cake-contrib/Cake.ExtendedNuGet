using System;
using System.IO;
using Cake.ExtendedNuGet.Tests.Fakes;
using Cake.Core.IO;
using Cake.ExtendedNuGet;
using NuGet;
using Xunit;

namespace Cake.ExtendedNuGet.Tests
{
    public class ExtendedNuGetTests : IDisposable
    {
        FakeCakeContext context;

        public ExtendedNuGetTests ()
        {
            context = new FakeCakeContext ();
        }


        public void Dispose ()
        {
            context.DumpLogs ();
        }

        [Fact]
        public void IsNuGetPublishedShouldBeTrue ()
        {
            var p = context.CakeContext.IsNuGetPublished (
                "Xamarin.Android.Support.v4",
                "23.1.1.0"
            );

            Assert.True (p);
        }

        [Fact]
        public void IsNuGetPublishedShouldBeFalse ()
        {
            var p = context.CakeContext.IsNuGetPublished (
                "Xamarin.Android.Support.v4",
                "999.9.9.0"
            );

            Assert.False (p);
        }

        [Fact]
        public void IsNuGetPublishedAltSrcShouldBeTrue ()
        {
            var p = context.CakeContext.IsNuGetPublished (
                "Cake.MsCognitiveServices.ComputerVision",
                "1.0.1",
                "https://www.myget.org/F/redth/api/v2"
            );

            Assert.True (p);
        }

        [Fact]
        public void IsNuGetPublishedAltSrcShouldBeFalse ()
        {
            var p = context.CakeContext.IsNuGetPublished (
                "ZXing.Net.Mobile",
                "999.9.9.0",
                "https://www.myget.org/F/redth/api/v2"
            );

            Assert.False (p);
        }

        [Fact]
        public void IsNugetPublishedInvalidSourceShouldBeFalse ()
        {
            var p = context.CakeContext.IsNuGetPublished (
                "Xamarin.Android.Support.v4",
                "23.1.1.0",
                "https://api.nuget.org/v3/wrong.json"
            );

            Assert.False (p);
        }

        [Fact]
        public void NuGetPackageIdFromFile ()
        {
            var f = new FilePath ("./TestData/xamarin.android.support.v4.23.1.1.nupkg");

            var packageId = context.CakeContext.GetNuGetPackageId (f);

            Assert.Equal ("Xamarin.Android.Support.v4", packageId);
        }

        [Fact]
        public void GetAllPackageReferencesForThisProject()
        {
            var d = new DirectoryPath("./TestData/");

            var packageReferences = context.CakeContext.GetPackageReferences(d);

            Assert.NotEmpty(packageReferences);
        }

        [Fact]
        public void GetPackageReferenceByIdForThisProject()
        {
            var d = new DirectoryPath("./TestData/");

            var cakeCorePackageReferences = context.CakeContext.GetPackageReference(d, "Cake.Core");

            Assert.False(string.IsNullOrEmpty(cakeCorePackageReferences.PackageIdentity.Version.ToString()));
        }
        
        [Fact]
        public void NuGetPackageIdFromFile_Releases_After_Read ()
        {
            const string copiedFile = "./TestData/xamarin.android.support.v4.23.1.1-beta001.nupkg";
            File.Copy(
                "./TestData/xamarin.android.support.v4.23.1.1.nupkg",
                copiedFile,
                true);
            var f = new FilePath (copiedFile);

            context.CakeContext.GetNuGetPackageId (f);

            File.Delete(copiedFile);

            Assert.False(File.Exists(copiedFile));
        }
    }
}

