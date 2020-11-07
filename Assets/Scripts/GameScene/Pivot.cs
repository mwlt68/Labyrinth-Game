namespace Assets.Scripts
{
    public enum Direction
    {
        North, //Kuzey
        South, //Güney
        East,  //Dogu
        West   // Batı
    }
    public class Pivot
    {
        public Direction direction;
        public int layerNo;  // layer zero is main way
        public System.Drawing.Point point;

        public Pivot(int layerNo, System.Drawing.Point point)
        {
            this.layerNo = layerNo;
            this.point = point;
        }

        public Pivot(Direction direction, int layerNo, System.Drawing.Point point)
        {
            this.direction = direction;
            this.layerNo = layerNo;
            this.point = point;
        }
    }
}