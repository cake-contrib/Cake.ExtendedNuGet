using System;
using Cake.Xamarin.Tests.Fakes;
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
                "ZXing.Net.Mobile",
                "1.5.0",
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
        public void NuGetPackageIdFromFile ()
        {
            var f = new FilePath ("../../TestData/xamarin.android.support.v4.23.1.1.nupkg");

            var packageId = context.CakeContext.GetNuGetPackageId (f);

            Assert.Equal ("Xamarin.Android.Support.v4", packageId);
        }

        [Fact]
        public void GetAllPackageReferencesForThisProject()
        {
            var d = new DirectoryPath("../../");

            var packageReferences = context.CakeContext.GetPackageReferences(d);

            Assert.NotEmpty(packageReferences);
        }

        [Fact]
        public void GetPackageReferenceByIdForThisProject()
        {
            var d = new DirectoryPath("../../");

            var cakeCorePackageReferences = context.CakeContext.GetPackageReference(d, "Cake.Core");

            Assert.False(string.IsNullOrEmpty(cakeCorePackageReferences.Version.ToString()));
        }
    }
}

