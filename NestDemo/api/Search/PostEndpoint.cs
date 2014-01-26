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
                            })))
                    .Highlight(h => h
                        .PreTags("<span class='highlight'>")
                        .PostTags("</span>")
                        .OnFields(new Action<HighlightFieldDescriptor<Customer>>[]
                        {
                            _ => _.OnField(c => c.CompanyName).NumberOfFragments(1).FragmentSize(100)
                        })));

            return Status.OK;
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