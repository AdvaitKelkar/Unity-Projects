using UnityEngine;

public class PlayerFollowCam : MonoBehaviour
{
    [SerializeField] Transform player;
    private void FollowPlayer()
    {
        if (player.position.y > transform.position.y)
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
    }
}
