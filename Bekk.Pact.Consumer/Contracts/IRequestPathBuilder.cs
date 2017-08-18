﻿namespace Bekk.Pact.Consumer.Contracts
{
    public interface IRequestPathBuilder
    {
        /// <summary>
        /// Provide a relative path to the service endpoint.
        /// </summary>
        /// <param name="path">A path (relative url) to the service.</param>
        /// <returns>A builder for defining request parameters.</returns>
        IRequestBuilder WhenRequesting(string path);
    }
}