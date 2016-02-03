using NUnit.Framework;
using System;
using Cake.Xamarin.Tests.Fakes;
using Cake.Core.IO;
using Cake.ExtendedNuGet;
using NuGet;

namespace Cake.FileHelpers.Tests
{
    [TestFixture]
    public class FileHelperTests
    {
        FakeCakeContext context;

        [SetUp]
        public void Setup ()
        {
            context = new FakeCakeContext ();
        }

        [TearDown]
        public void Teardown ()
        {
            context.DumpLogs ();
        }

        [Test]
        public void IsNuGetPublishedShouldBeTrue ()
        {
            var p = context.CakeContext.IsNuGetPublished (
                "Xamarin.Android.Support.v4",
                "23.1.1.0"
            );

            Assert.IsTrue (p);
        }

        [Test]
        public void IsNuGetPublishedShouldBeFalse ()
        {
            var p = context.CakeContext.IsNuGetPublished (
                "Xamarin.Android.Support.v4",
                "999.9.9.0"
            );

            Assert.IsFalse (p);
        }

        [Test]
        public void IsNuGetPublishedAltSrcShouldBeTrue ()
        {
            var p = context.CakeContext.IsNuGetPublished (
                "ZXing.Net.Mobile",
                "1.5.0",
                "https://www.myget.org/F/redth/api/v2"
            );

            Assert.IsTrue (p);
        }

        [Test]
        public void IsNuGetPublishedAltSrcShouldBeFalse ()
        {
            var p = context.CakeContext.IsNuGetPublished (
                "ZXing.Net.Mobile",
                "999.9.9.0",
                "https://www.myget.org/F/redth/api/v2"
            );

            Assert.IsFalse (p);
        }

        [Test]
        public void NuGetPackageIdFromFile ()
        {
            var f = new FilePath ("../../TestData/xamarin.android.support.v4.23.1.1.nupkg");

            var packageId = context.CakeContext.GetNuGetPackageId (f);

            Assert.AreEqual ("Xamarin.Android.Support.v4", packageId);
        }

        [Test]
        public void NuGetPackageVersionFromFile ()
        {
            var f = new FilePath ("../../TestData/xamarin.android.support.v4.23.1.1.nupkg");

            var packageVersion = context.CakeContext.GetNuGetPackageVersion (f);

            Assert.AreEqual (new SemanticVersion ("23.1.1"), packageVersion);
        }
    }
}

