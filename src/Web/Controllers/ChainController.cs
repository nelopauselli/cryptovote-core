using System.Threading.Tasks;
using Domain;
using Domain.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ChainController : ControllerBase
	{
		private readonly NodeAdapter nodeAdapter;

		public ChainController(NodeAdapter nodeAdapter)
		{
			this.nodeAdapter = nodeAdapter;
		}

		public async Task<ActionResult<Block>> Get(string hash)
		{
				
			var block = string.IsNullOrWhiteSpace(hash)
				? await nodeAdapter.Blocks.GetLastBlock()
				: await nodeAdapter.Blocks.GetBlockByHash(Base58.Decode(hash));

			if (block == null)
				return NotFound();

			return Ok(block);
		}
	}
}