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
            Input.Query = string.IsNullOrEmpty(Input.Query) ? null : Input.Query;
            Output =
                _client.Search<Customer>(sd => sd
                    .Query(q => q
                        .Filtered(fq => fq
                            .Query(qs => 
                                // if Input.Query is null or empty neither the match or fuzzy will be 'rendered'
                                // thus acting as a match_all as fall back
                                // in the next version of Nest you can wrap this inside a q.Conditionless() 
                                // and choose another fallback instead of nothing (match_all effectively)
                                qs.Match(m => m.OnField("_all").Query(Input.Query)) ||
                                qs.Fuzzy(fd => fd
                                    .OnField("_all")
                                    .MinSimilarity(0.6)
                                    .PrefixLength(1)
                                    .Value(Input.Query)
                                    .Boost(0.1)
                                )
                            )
                            //Here we aggrate to one BaseFilter with &=
                            //If Input.Filters has more then one it will 'render' a bool
                            //If it only has one it will 'render' that filter directly (without wrapping it in a bool)
                            //If its empty the filter won't be rendered effectively doing a match_all (just as with query).
                            .Filter(f=> Input.Filter.Aggregate(new BaseFilter(),(s,ff)=>s &= FilterDesc[ff.Key](ff.Value)))
                        )
                    )
                    .Highlight(h => h
                        .PreTags("<span class='highlight'>")
                        .PostTags("</span>")
                        .OnFields(
                            _ => _.OnField(c => c.CompanyName).NumberOfFragments(1).FragmentSize(100)
                        ))
                    .FacetTerm(f => f.Nested(c => c.Products).OnField(c => c.Products[0].ProductName).Size(1000))
                    .FacetTerm(f => f.Nested(c => c.Products).OnField(c => c.Products[0].CategoryName).Size(1000))
                    .FacetTerm(f => f.OnField(c => c.Country).Size(1000)));

            return Status.OK;
        }

        public static Dictionary<string, Func<IEnumerable<string>, BaseFilter>> FilterDesc =
            new Dictionary<string, Func<IEnumerable<string>, BaseFilter>>()
            {
                {"products.productName", ps => AddProductsFilter(ps, c => c.Products[0].ProductName)},
                {"products.categoryName", cs => AddProductsFilter(cs, c => c.Products[0].CategoryName)},
                {"country", cs => AddCustomerFilter(cs, c => c.Country)}
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