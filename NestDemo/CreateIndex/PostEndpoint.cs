using System.Threading.Tasks;
using NestDemo.ElasticSearch;
using Simple.Web;

namespace NestDemo.CreateIndex
{
    [UriTemplate("/createindex")]
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

        }
    }
}