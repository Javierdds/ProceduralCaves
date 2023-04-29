using UnityEngine;

namespace ProceduralCave.Generator.Mesh
{
	/// <summary>
	/// Nodo de control para aplicar la técnica "Marching Cubes"
	/// 
	/// Diferencia entre nodo y nodo de control en la clase "MarchingCubesSquare"
	/// </summary>
	public class ControlNode : Node
	{
		// Los nodos above y right se emplean para crear nodos auxiliares de refencia,
		// ya que el grid de cubos para el que se emplea, se va generando desde la
		// esquina inferior izquierda de una matriz.
		private Node _above;
		private Node _right;
		private bool _isActive;

		public Node Above { get => _above; set => _above = value; }
		public Node Right { get => _right; set => _right = value; }
		public bool IsActive { get => _isActive; set => _isActive = value; }

		public ControlNode(Vector2 nodePos, bool _active, float squareSize) : base(nodePos)
		{
			_isActive = _active;
			_above = new Node(_position + Vector2.up * squareSize / 2f);
			_right = new Node(_position + Vector2.right * squareSize / 2f);
		}

    }
}
