using Bookify.web.Core.Models;

namespace Bookify.web.Services
{
	public interface IEmailBodyBuilder
	{
		public string EmailBody(string imageurl,string header,string body,string? url ,string linkTitle);
		
	}
}
