namespace Arkanodia.MapGenerators
{
    public interface IGridBocks
    {
        public void CreateMap(int[,] map);
        int[,] CurrentMap { get; set; }
    }
}