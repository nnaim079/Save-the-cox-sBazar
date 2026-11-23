using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AnimalRescue : MonoBehaviour
{
    private bool isHealed = false;
    private bool readyToMove = false;
    private bool isPlayerNear = false; 

    public GameObject trashObject;
    public Transform targetLocation;
    public float moveSpeed = 5f;
    public bool isBird = true;

    [Header("Animation Settings")]
    public Animator birdAnimator; 
    public string flyAnimationName = "flyStraight"; 

    [Header("Sound & Movement Settings")]
    public AudioClip song1;
    public AudioClip song2;
    private AudioSource audioSource;
    private float nextActionTime;
    private Quaternion targetRotation;
    
    private bool isHopping = false;
    private Vector3 startPosition;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        targetRotation = transform.rotation;
        startPosition = transform.position; // শুরুর জায়গা মনে রাখবে যেন দূরে হারিয়ে না যায়
        SetNextActionTime();
    }

    void Update()
    {
        // মাটিতে থাকার সময় সাউন্ড ও ডানে-বাঁয়ে লাফানো
        if (!isHealed && !isHopping)
        {
            if (Time.time >= nextActionTime)
            {
                PlayRandomSong();
                StartCoroutine(HopAround()); // লাফানোর কমান্ড
                SetNextActionTime();
            }
            // স্মুথলি ঘোরার কোড
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // রেসকিউ শুরু
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !isHealed)
        {
            isHealed = true;
            StopAllCoroutines(); // রেসকিউ শুরু হলে লাফানো বন্ধ করবে
            StartCoroutine(RescueSequence());
        }

        // রেসকিউয়ের পর আকাশে উড়ে যাওয়া
        if (readyToMove && targetLocation != null)
        {
            transform.LookAt(targetLocation);
            transform.position = Vector3.MoveTowards(transform.position, targetLocation.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetLocation.position) < 1f)
            {
                Destroy(gameObject);
            }
        }
    }

    // লাফানোর আসল ফিজিক্স কোড
    IEnumerator HopAround()
    {
        isHopping = true;
        
        // ডানে, বাঁয়ে বা সামনে লাফানোর জন্য ছোট জায়গা ঠিক করা
        float randomX = Random.Range(-0.3f, 0.3f);
        float randomZ = Random.Range(-0.3f, 0.3f);
        Vector3 hopOffset = new Vector3(randomX, 0, randomZ);
        
        // শুরুর জায়গা থেকে বেশি দূরে যেন না যায়
        Vector3 endPos = transform.position + hopOffset;
        if (Vector3.Distance(startPosition, endPos) > 1.5f)
        {
            endPos = startPosition; // দূরে গেলে আবার আগের জায়গায় লাফ দিয়ে ফিরবে
        }

        // যেদিকে লাফাবে সেদিকে তাকানো
        if (hopOffset != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(endPos - transform.position);
        }

        float hopDuration = 0.25f; // লাফাতে কতক্ষণ লাগবে (খুব দ্রুত লাফাবে)
        float elapsed = 0f;
        Vector3 currentPos = transform.position;

        while (elapsed < hopDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / hopDuration;
            
            // একটু ওপরের দিকে লাফ দেওয়ার হিসাব (প্যারাবলিক আর্ক)
            float height = Mathf.Sin(t * Mathf.PI) * 0.15f; 
            
            transform.position = Vector3.Lerp(currentPos, endPos, t) + new Vector3(0, height, 0);
            yield return null;
        }

        transform.position = endPos; // মাটিতে ঠিকমতো নামানো
        isHopping = false;
    }

    void PlayRandomSong()
    {
        if (audioSource != null && (song1 != null || song2 != null))
        {
            AudioClip clipToPlay = (Random.value > 0.5f && song2 != null) ? song2 : song1;
            if (clipToPlay != null)
            {
                audioSource.PlayOneShot(clipToPlay, 0.6f);
            }
        }
    }

    void SetNextActionTime()
    {
        // ২ থেকে ৪ সেকেন্ড পর পর নতুন করে লাফাবে ও ডাকবে
        nextActionTime = Time.time + Random.Range(2f, 4f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerNear = false;
    }

    IEnumerator RescueSequence()
    {
        if (trashObject != null)
        {
            Renderer trashRenderer = trashObject.GetComponent<Renderer>();
            if (trashRenderer != null)
            {
                Material trashMat = trashRenderer.material;
                Color color = trashMat.color;
                float fadeSpeed = 1.5f;

                while (color.a > 0)
                {
                    color.a -= Time.deltaTime * fadeSpeed;
                    trashMat.color = color;
                    yield return null;
                }
            }
            Destroy(trashObject);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        if (birdAnimator != null)
        {
            birdAnimator.Play(flyAnimationName);
        }
        
        readyToMove = true;
    }

    // --- Animation Event Errors Fix ---
    // এই ফাঁকা ফাংশনগুলো ইউনিটির লাল এররগুলো বন্ধ করবে
    
    void ResetHopInt()
    {
    }

    void ResetFlyingLandingVariables()
    {
    }

    void PlaySong()
    {
    }
}