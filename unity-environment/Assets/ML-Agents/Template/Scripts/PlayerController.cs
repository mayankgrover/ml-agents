using UnityEngine;

public class PlayerController: MonoBehaviour
{
    public float north, south;
    public float east, west;
    public float northeast, northwest;
    public float southeast, southwest;

    private float moveSpeed = 0.25f;

    public void MoveLeft() { transform.localPosition += moveSpeed * Vector3.left; }
    public void MoveRight() { transform.localPosition += moveSpeed * Vector3.right; }
    public void MoveUp() { transform.localPosition += moveSpeed * Vector3.forward; }
    public void MoveDown() { transform.localPosition += moveSpeed * Vector3.back; }

    //public void MoveUpRight() { transform.localPosition += moveSpeed *(Vector3.forward + Vector3.right); }
    //public void MoveDownRight() { transform.localPosition += moveSpeed *(Vector3.back + Vector3.right); }
    //public void MoveUpLeft() { transform.localPosition += moveSpeed *(Vector3.forward + Vector3.left); }
    //public void MoveDownLeft() { transform.localPosition += moveSpeed *(Vector3.back + Vector3.left); }

    public void UpdateState()
    {
        north = CheckNextBlockInDirection(Vector3.forward);
        south = CheckNextBlockInDirection(Vector3.back);
        east = CheckNextBlockInDirection(Vector3.right);
        west = CheckNextBlockInDirection(Vector3.left);

        northeast = CheckNextBlockInDirection(Vector3.forward + Vector3.right);
        northwest = CheckNextBlockInDirection(Vector3.forward + Vector3.left);
        southeast = CheckNextBlockInDirection(Vector3.back + Vector3.right);
        southwest = CheckNextBlockInDirection(Vector3.back + Vector3.left);
    }

    private float CheckNextBlockInDirection(Vector3 dir)
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, dir, Color.red);
        Physics.Raycast(transform.position, dir, out hit, 1f);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Obstacle")
            {
                return 1;
            }
            //else if (hit.collider.tag == "Finish")
            //{
            //    return 1;
            //}
        }
        return 0;
    }
}
