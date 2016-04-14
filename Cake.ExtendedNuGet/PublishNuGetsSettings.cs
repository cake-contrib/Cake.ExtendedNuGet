using System;

namespace Cake.ExtendedNuGet
{
    /// <summary>
    /// Settings for PublishNuGets alias
    /// </summary>
    public class PublishNuGetsSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cake.ExtendedNuGet.PublishNuGetsSettings"/> class.
        /// </summary>
        public PublishNuGetsSettings ()
        {
            MaxAttempts = 3;
            ForcePush = false;
        }

        /// <summary>
        /// How many attempts should be made to publish before failing
        /// </summary>
        /// <value>The max attempts.</value>
        public int MaxAttempts { get; set; }

        /// <summary>
        /// Should a publish be attempted even if the server already has the same NuGet package version?
        /// </summary>
        /// <value><c>true</c> if force push; otherwise, <c>false</c>.</value>
        public bool ForcePush { get;set; }
    }
}

