using UnityEngine;


namespace Strategy2D
{
    public class Cell : MonoBehaviour
    {
        public SpriteRenderer Renderer; // Sprite of cell
        public Color NoCollisionColor;
        public Color CollisinColor;


        // When there is no collision with cell
        public void SetValid()
        {
            Renderer.color = NoCollisionColor;
        }

        // When there is a collision with cell
        public void SetInvalid()
        {
            Renderer.color = CollisinColor;
        }
    }
}