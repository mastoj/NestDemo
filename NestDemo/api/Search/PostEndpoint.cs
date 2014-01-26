using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Nest;
using NestDemo.ElasticSearch;
using NestDemo.Model;
using Simple.Web;
using Simple.Web.Behaviors;

namespace NestDemo.api.Search
{
    [UriTemplate("/api/search")]
    public class PostEndpoint : IPost, IOutput<IQueryResponse<Customer>>, IInput<SearchModel>
    {
        private ElasticClientWrapper _client;

        public PostEndpoint()
        {
            _client = new ElasticClientWrapper();
        }

        public Status Post()
        {
            Output = 
            _client.Search<Customer>(sd => sd
                .Query(q => q
                    .Bool(b => b
                        .Should(new Func<QueryDescriptor<Customer>, BaseQuery>[]
                        {
                            _ => _.Match(m => m.OnField("_all").QueryString(Input.Query)),
                            _ => _.Fuzzy(fd => fd
                                .OnField("_all")
                                .MinSimilarity(0.6)
                                .PrefixLength(1)
                                .Value(Input.Query)
                                .Boost(0.1))
                        }))));

            return Status.OK;
        }

        public Dictionary<string, Func<IEnumerable<string>, BaseFilter>> FilterDesc =
            new Dictionary<string, Func<IEnumerable<string>, BaseFilter>>()
            {
                { "products.productName", ps => AddProductsFilter(ps, c => c.Products[0].ProductName) },
                { "products.categoryName", cs => AddProductsFilter(cs, c => c.Products[0].CategoryName) },
                { "country", cs => AddCustomerFilter(cs, c => c.Country)}
            };

        private static BaseFilter AddCustomerFilter(IEnumerable<string> items, Expression<Func<Customer, object>> propExpr)
        {
            return Filter<Customer>.Terms(propExpr, items.ToArray());
        }

        private static BaseFilter AddProductsFilter(IEnumerable<string> items, 
            Expression<Func<Customer, object>> propExpr)
        {
            return Filter<Customer>.Nested(sel => sel
                .Path(c => c.Products)
                .Query(q => q.Terms(propExpr, items.ToArray())));
        }

        public IQueryResponse<Customer> Output { get; private set; }
        public SearchModel Input { set; private get; }
    }

    public class SearchModel
    {
        private int? _numberToTake;
        public string Query { get; set; }
        public Dictionary<string, IEnumerable<string>> Filter { get; set; } 

        public int? NumberToTake
        {
            get { return _numberToTake.HasValue ? _numberToTake.Value : 25; }
            set { _numberToTake = value; }
        }
    }
}