using UnityEngine;

namespace ProceduralCave.Generator.Mesh
{
	/// <summary>
	/// Marching cube diagram:
	///			|X|O|X|
	///			|O| |O|
	///			|X|O|X|
	///	La X representa un nodo de control (ControlNode)
	///	La O representa un nodo normal (Node)
	/// </summary>
	public class MarchingCubesSquare
	{
		// Véase el diagrama en el comentario que define esta misma clase
		private ControlNode _topLeft, _topRight, _bottomRight, _bottomLeft;
		private Node _centreTop, _centreRight, _centreBottom, _centreLeft;

		#region Gettters & Setters

		public ControlNode TopLeft { get => _topLeft; set => _topLeft = value; }
		public ControlNode TopRight { get => _topRight; set => _topRight = value; }
		public ControlNode BottomRight { get => _bottomRight; set => _bottomRight = value; }
		public ControlNode BottomLeft { get => _bottomLeft; set => _bottomLeft = value; }
		public Node CentreTop { get => _topLeft.Right; set => _centreTop = value; }
		public Node CentreRight { get => _bottomRight.Above; set => _centreRight = value; }
		public Node CentreBottom { get => _bottomLeft.Right; set => _centreBottom = value; }
		public Node CentreLeft { get => _bottomLeft.Above; set => _centreLeft = value; }

		#endregion

		public MarchingCubesSquare()
        {

        }

		public MarchingCubesSquare(ControlNode topLeft, ControlNode topRight,
			ControlNode bottomRight, ControlNode bottomLeft)
		{
			_topLeft = topLeft;
			_topRight = topRight;
			_bottomRight = bottomRight;
			_bottomLeft = bottomLeft;

			_centreTop = topLeft.Right;
			_centreRight = bottomRight.Above;
			_centreBottom = bottomLeft.Right;
			_centreLeft = bottomLeft.Above;
		}
    }
}
