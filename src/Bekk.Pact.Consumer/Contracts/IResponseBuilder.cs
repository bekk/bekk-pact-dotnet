using System.Net;
using System.Threading.Tasks;

namespace Bekk.Pact.Consumer.Contracts
{
    public interface IResponseBuilder
    {
        /// <summary>
        /// Define header to require in the response from the provider.
        /// </summary>
        IResponseBuilder WithHeader(string key, params string[] values);
        /// <summary>
        /// Define the message body of the response from the provider.
        /// </summary>
        /// <param name="body">An object serializable to json.</param>
        IResponseBuilder WithBody(object body);
        /// <summary>
        /// Define the message body of the response from the provider as an array of items.
        /// </summary>
        /// <param name="elements">The elements in the array body</param>
        IResponseBuilder WithBodyArray(params object[] elements);
        /// <summary>
        /// Call this method to create a pact object and start listening for a request.
        /// This method must be awaited.
        /// </summary>
        /// <returns>A pact object. Dispose this object at the end of the test.</returns>
        Task<IPact> InPact();
    }
}