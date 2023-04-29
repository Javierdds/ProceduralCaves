using UnityEngine;

namespace ProceduralCave.Generator.Mesh
{
    /// <summary>
    /// Nodo b�sico para aplicar la t�cnica de "Marching Cubes"
    /// </summary>
    public class Node
    {
        protected Vector2 _position;
        protected int _vertexIndex = -1;

        public Vector2 Position { get => _position; set => _position = value; }
        public int VertexIndex { get => _vertexIndex; set => _vertexIndex = value; }

        public Node(Vector2 nodePosition)
        {
            _position = nodePosition;
        }

    }

}
