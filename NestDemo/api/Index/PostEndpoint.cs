using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using NestDemo.ElasticSearch;
using NestDemo.Model;
using Simple.Web;

namespace NestDemo.CreateIndex
{
    [UriTemplate("/api/index")]
    public class PostEndpoint : IPost
    {
        private ElasticClientWrapper _client;
        private CustomerRepository _customerRepo;

        public PostEndpoint()
        {
            _client = new ElasticClientWrapper();
            _customerRepo = new CustomerRepository();
        }
        
        public Status Post()
        {
            CreateIndex();
            return Status.OK;
        }

        public void CreateIndex()
        {
            var customers = _customerRepo.GetCustomers();
            var nextIndex = GetNextIndex();

            _client.CreateIndex(nextIndex, s => s.AddMapping<Customer>(m => m.MapFromAttributes()));
            _client.IndexMany(customers, nextIndex);

            SetAlias(nextIndex, Settings.Alias);
        }

        private void SetAlias(string nextIndex, string @alias)
        {
            var indicesForAlias = _client.GetIndicesPointingToAlias(Settings.Alias).ToList();
            var newIndices = new[] {nextIndex};
            _client.Swap(@alias, indicesForAlias, newIndices);
        }

        private string GetNextIndex()
        {
            var indexNumber = GetCurrentIndexNumber();
            return Settings.Alias + "_" + (indexNumber + 1);
        }

        private int GetCurrentIndexNumber()
        {
            var indices = _client.GetIndicesPointingToAlias(Settings.Alias);//.ToList();
            var index = indices.ToList();
            var indexNumber = index.Count > 0
                ? index.Max(y =>
                {
                    var tokens = y.Split('_');
                    return int.Parse(tokens.Last());
                })
                : 0;
            return indexNumber;
        }
    }
}