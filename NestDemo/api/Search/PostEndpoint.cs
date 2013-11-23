using System;
using Nest;
using NestDemo.ElasticSearch;
using NestDemo.Model;
using Simple.Web;
using Simple.Web.Behaviors;

namespace NestDemo.api.Search
{
    [UriTemplate("/api/search")]//"?Query={Query}")]
    public class PostEndpoint : IPost, IOutput<IQueryResponse<Customer>>, IInput<SearchModel>
    {
        private ElasticClientWrapper _client;

        public PostEndpoint()
        {
            _client = new ElasticClientWrapper();
        }

        public Status Post()
        {
            Func<SearchDescriptor<Customer>, SearchDescriptor<Customer>> searchDescriptor = sd =>
                sd.Query(bq =>
                    bq.Filtered(fq =>
                    {
                        if (Input.Query != null)
                        {
                            bq.QueryString(y => y.Query(Input.Query));
                        }
                    }));

            
            Func<SearchDescriptor<Customer>, SearchDescriptor<Customer>> facetDescriptor = fd =>
            {
                return searchDescriptor(fd
                    .FacetTerm(ft => ft.Nested(c => c.Products).OnField(c => c.Products[0].ProductName).Size(1000))
                    .FacetTerm(ft => ft.Nested(c => c.Products).OnField(c => c.Products[0].CategoryName).Size(1000)));
            };

            Output = _client.Search(facetDescriptor);
            return Status.OK;
        }

        public IQueryResponse<Customer> Output { get; private set; }
        public SearchModel Input { set; private get; }
    }

    public class SearchModel
    {
        public string Query { get; set; }
    }
}