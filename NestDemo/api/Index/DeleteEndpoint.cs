using System.Linq;
using NestDemo.CreateIndex;
using NestDemo.ElasticSearch;
using Simple.Web;

namespace NestDemo.api.deleteindex
{
    [UriTemplate("/api/index")]
    public class DeleteEndpoint : IDelete
    {
        private ElasticClientWrapper _client;

        public DeleteEndpoint()
        {
            _client = new ElasticClientWrapper();
        }

        public Status Delete()
        {
            DeleteIndex();
            return Status.OK;
        }

        private void DeleteIndex()
        {
            var indicesForAlias = _client.GetIndicesPointingToAlias(Settings.Alias).ToList();
            foreach (var index in indicesForAlias)
            {
                _client.DeleteIndex(index);
            }
        }
    }
}