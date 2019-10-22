namespace Domain
{
	public interface IBlockBuilder
	{
		Block BuildNextBlock(BlockItem[] transactions, Block previous, byte dificulty);
	}
}