namespace Testprojekt_1.Helpers
{
	internal class DragObject
	{
		public enum ObjectType
		{
			Circle = 0,
			Rectangle = 1,
			SliderHorizontal = 2,
			SliderVertical = 3,
			Image = 4
		}

		private ObjectType _type { get; }
		public int SizeX;
		public int SizeY;
		public int PosX;
		public int PosY;
		protected int _color { get; set; }
		public string ClassName { get; set; }  // New property to store the class name

		// Constructor for Circle
		public DragObject(int width, double posX, double posY, int color, string className = "shape-circle")
		{
			this._type = ObjectType.Circle;
			this.SizeX = width;
			this.PosX = (int)posX;
			this.PosY = (int)posY;
			this._color = color;
			this.ClassName = className;  // Assign the class name
		}

		// Constructor for Rectangle
		public DragObject(int sizeX, int sizeY, double posX, double posY, int color, string className = "shape-rectangle")
		{
			this._type = ObjectType.Rectangle;
			this.SizeX = sizeX;
			this.SizeY = sizeY;
			this.PosX = (int)posX;
			this.PosY = (int)posY;
			this._color = color;
			this.ClassName = className;  // Assign the class name
		}

		// Constructor for Sliders
		public DragObject(int sizeX, int sizeY, double posX, double posY, string className)
		{
			this._type = className.Contains("horizontal-slider") ? ObjectType.SliderHorizontal : ObjectType.SliderVertical;
			this.SizeX = sizeX;
			this.SizeY = sizeY;
			this.PosX = (int)posX;
			this.PosY = (int)posY;
			this.ClassName = className;
		}

		// Method to update position
		public void setPosition(double x, double y)
		{
			this.PosX = (int)x;
			this.PosY = (int)y;
		}

		// Getters for position and size
		public string getX() => $"{PosX}px";
		public string getY() => $"{PosY}px";
		public string getWidth() => $"{SizeX}px";
		public string getHeight() => (_type == ObjectType.Circle) ? $"{SizeX}px" : $"{SizeY}px";
	}
}
