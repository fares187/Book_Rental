using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;

namespace Bookify.web.Core.ViewModel
{
    public class ReturnCopyViewModel
    {
        public int Id { get; set; } 
        public bool? IsReturned { get; set; }
    }
}
