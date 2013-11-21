using Simple.Web;
namespace NestDemo
{
    [UriTemplate("/")]
    public class Index : IGet
    {
        public Status Get()
        {
            return Status.OK;
        }
    }
}
