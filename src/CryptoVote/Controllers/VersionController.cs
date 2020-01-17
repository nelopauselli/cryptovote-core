using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace CryptoVote.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VersionController : ControllerBase
	{
		public string Get()
		{
			var assemblyName = Assembly.GetExecutingAssembly().GetName();
			return $"{assemblyName.Version}";
		} 
	}
}