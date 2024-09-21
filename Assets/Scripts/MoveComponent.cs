using UnityEngine;

public class MoveComponent : MonoBehaviour
{
    [SerializeField] float MoveSpeed;

    void FixedUpdate()
    {
        var Direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        transform.position += (Vector3)Direction * MoveSpeed * Time.fixedDeltaTime;
    }
}