using System.Linq;
using System.Threading.Tasks;
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
            var nextIndex = GetIndex(offset: 1);

            _client.CreateIndex(nextIndex, s => s.AddMapping<Customer>(m => m.MapFromAttributes()));
            _client.IndexMany(customers, nextIndex);

            _client.RemoveAlias(Settings.Alias);
            _client.Alias(nextIndex, Settings.Alias);
            CleanUpIndices(nextIndex);
        }

        private void CleanUpIndices(string indexToExclude)
        {
            var indicesForAlias = _client.GetIndicesPointingToAlias(Settings.Alias).ToList();
            foreach (var index in indicesForAlias)
            {
                if (index != indexToExclude)
                {
                    _client.DeleteIndex(index);
                }
            }
        }

        private string GetIndex(int offset)
        {
            var indexNumber = GetCurrentIndexNumber();
            return Settings.Alias + "_" + (indexNumber + offset);
        }

        private int GetCurrentIndexNumber()
        {
            var index = _client.GetIndicesPointingToAlias(Settings.Alias).ToList();
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