using UnityEngine;

public class MetalDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 10f;
    public float collectDistance = 4f;
    public LayerMask metalLayer;
    public KeyCode collectKey = KeyCode.F;

    [Header("Collection Counter")]
    public int totalCollected = 0;

    private Collider targetCollider;
    private float targetDist;

    private void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, metalLayer);

        if (hits.Length > 0)
        {
            Collider closest = hits[0];
            float closestDist = Vector3.Distance(transform.position, closest.transform.position);

            for (int i = 1; i < hits.Length; i++)
            {
                float d = Vector3.Distance(transform.position, hits[i].transform.position);
                if (d < closestDist)
                {
                    closest = hits[i];
                    closestDist = d;
                }
            }

            targetCollider = closest;
            targetDist = closestDist;

            if (closestDist <= collectDistance)
            {
                if (Input.GetKeyDown(collectKey) || Input.GetKeyDown(KeyCode.F))
                {
                    GameObject rootObj = closest.transform.root.gameObject;
                    
                    if (rootObj.CompareTag("Player"))
                    {
                        closest.gameObject.SetActive(false);
                    }
                    else
                    {
                        rootObj.SetActive(false);
                    }
                    
                    totalCollected++;
                    Debug.Log("Collected! Total Metal: " + totalCollected);
                    targetCollider = null;
                }
            }
        }
        else
        {
            targetCollider = null;
        }
    }

    private void OnGUI()
    {
        GUIStyle promptStyle = new GUIStyle();
        promptStyle.fontSize = 24;
        promptStyle.fontStyle = FontStyle.Bold;

        if (targetCollider != null)
        {
            promptStyle.normal.textColor = (targetDist <= collectDistance) ? Color.green : Color.yellow;

            string msg = (targetDist <= collectDistance) 
                ? "Press [F] to Collect" 
                : $"Metal Detected! Distance: {targetDist:F1}m";

            GUI.Label(new Rect(Screen.width / 2 - 130, Screen.height - 110, 350, 40), msg, promptStyle);
        }

        GUIStyle counterStyle = new GUIStyle();
        counterStyle.fontSize = 22;
        counterStyle.fontStyle = FontStyle.Bold;
        counterStyle.normal.textColor = Color.white;

        GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height - 50, 300, 35), $"Total Metal Collected: {totalCollected}", counterStyle);
    }
}