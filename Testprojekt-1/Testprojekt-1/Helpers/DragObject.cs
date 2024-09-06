using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		/// <summary>
		/// Creates a new Circle Object
		/// </summary>
		/// <param name="width"></param>
		/// <param name="posX"></param>
		/// <param name="posY"></param>
		/// <param name="color"></param>
		public DragObject(int width, double posX, double posY, int color)
		{
			this._type = ObjectType.Circle;
			this.SizeX = width;
			this.PosX = (int)posX;
			this.PosY = (int)posY;
			this._color = color;
		}

		/// <summary>
		/// Creates a new Rectangle object
		/// </summary>
		/// <param name="sizeX"></param>
		/// <param name="sizeY"></param>
		/// <param name="posX"></param>
		/// <param name="posY"></param>
		/// <param name="color"></param>
		public DragObject(int sizeX, int sizeY, double posX, double posY, int color)
		{
			this._type = ObjectType.Rectangle;
			this.SizeX = sizeX;
			this.SizeY = sizeY;
			this.PosX = (int)posX;
			this.PosY = (int)posY;
			this._color = color;
		}

		/// <summary>
		/// Create a new Image object, NOT IMPLEMENTED YET
		/// </summary>
		/// <param name="sizeX"></param>
		public DragObject()
		{

		}

		/// <summary>
		/// Create a Slider
		/// </summary>
		/// <param name="size"></param>
		/// <param name="horizontal"></param>
		/// <param name="posX"></param>
		/// <param name="posY"></param>
		public DragObject(int size, bool horizontal, double posX, double posY)
		{
			this._type = horizontal ? ObjectType.SliderHorizontal : ObjectType.SliderVertical;
			this.SizeX = size;
			this.PosX = (int)posX;
			this.PosY = (int)posY;
		}

		public void setPosition(double x, double y)
		{
			this.PosX = (int)x;
			this.PosY = (int)y;
		}

		public string getX()
		{
			return $"{PosX}px";
		}

		public string getY()
		{
			return $"{PosY}px";
		}

		public string getWidth()
		{
			return $"{SizeX}px";
		}

		public string getHeight()
		{
			if(_type == ObjectType.Circle) return $"{SizeX}px";

			return $"{SizeY}px";
		}
	}
}
