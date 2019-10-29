using System;
using System.Net.Http;
using System.Web.Http;
using MongoDB.Driver;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
    public class IntegrationTestServer : IDisposable
    {
        private const string DummyUrl = "https://localhost:44380/";
        private readonly HttpConfiguration _config;
        private static readonly IntegrationTestServer This;

        static IntegrationTestServer()
        {
            This = new IntegrationTestServer();
        }
        private IntegrationTestServer()
        {
            _config = new HttpConfiguration();
            WebApiConfig.Register(_config);
            _config.EnsureInitialized();
            Server = new HttpServer(_config);
        }

        public HttpServer Server { get; }

        public IMongoDatabase MongoDatabase
            => _config.DependencyResolver.GetService(typeof(IMongoDatabase)) as IMongoDatabase;

        public void Dispose()
        {
            Server?.Dispose();
        }

        public HttpClient GetClient()
        {
            return new HttpClient(Server) {BaseAddress = new Uri(DummyUrl)};
        }

        public static IntegrationTestServer GetInstance()
        {
            return This;
        }
    }
}