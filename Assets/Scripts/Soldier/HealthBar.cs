using UnityEngine;


namespace Strategy2D
{
    public class HealthBar : MonoBehaviour
    {

        MaterialPropertyBlock matBlock;
        MeshRenderer meshRenderer;
        Camera mainCamera;
       // SoldierItem soldierItem;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            matBlock = new MaterialPropertyBlock();
      
         
        }

        private void Start()
        {
            // Cache since Camera.main is super slow
            mainCamera = Camera.main;
        }

        public void Damage(float maxHealth, float healthPoint)
        {
            meshRenderer.enabled = true;
            //AlignCamera();
            UpdateParams(maxHealth,healthPoint);
        }

        private void UpdateParams(float maxHealth, float healthPoint)
        {
            meshRenderer.GetPropertyBlock(matBlock);
            matBlock.SetFloat("_Fill", healthPoint / (float)maxHealth);
            meshRenderer.SetPropertyBlock(matBlock);
        }

        private void AlignCamera()
        {
            if (mainCamera != null)
            {
                var camXform = mainCamera.transform;
                var forward = transform.position - camXform.position;
                forward.Normalize();
                var up = Vector3.Cross(forward, camXform.right);
                transform.rotation = Quaternion.LookRotation(forward, up);
            }
        }

    }
}