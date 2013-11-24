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
            Func<SearchDescriptor<Customer>, SearchDescriptor<Customer>> searcher = sd =>
                sd.Query(bq =>
                    bq.Filtered(fq =>
                    {
                        if (!string.IsNullOrEmpty(Input.Query))
                        {
                            fq.Query(qs => qs.QueryString(y => y.Query(Input.Query)));
                        }
                        else
                        {
                            fq.Query(qs => qs.MatchAll());
                        }
                        if (Input.Filter.Count > 0)
                        {
                            var filters = Input.Filter.Select(_ => FilterDesc[_.Key](_.Value)).ToArray();
                            fq.Filter(sel => sel.And(filters));
                        }

                    })).Size(Input.NumberToTake.Value);


            Func<SearchDescriptor<Customer>, SearchDescriptor<Customer>> facetDescriptor = fd =>
            {
                return searcher(fd
                    .FacetTerm(f => f.Nested(c => c.Products).OnField(c => c.Products[0].ProductName).Size(1000))
                    .FacetTerm(f => f.Nested(c => c.Products).OnField(c => c.Products[0].CategoryName).Size(1000))
                    .FacetTerm(f => f.OnField(c => c.Country).Size(1000)));
            };

            Output = _client.Search(facetDescriptor);
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

        private static BaseFilter AddProductsFilter(IEnumerable<string> items, Expression<Func<Customer, object>> propExpr)
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