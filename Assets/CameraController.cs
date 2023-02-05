using UnityEngine;

public class CameraController : MonoBehaviour
{
  // Singleton
  public static CameraController instance;

  void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
    else
    {
      Destroy(gameObject);
    }
  }

  public GameObject minPivot;
  public GameObject maxPivot;

  // public float scrollSpeed = 20f;

  // void Update()
  // {

  //   float scroll = Input.GetAxis("Mouse ScrollWheel");

  //   pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;

  //   pos.x = Mathf.Clamp(pos.x, -80f, 80f);
  //   pos.y = Mathf.Clamp(pos.y, minY, maxY);
  //   pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

  //   transform.position = pos;
  // }
}