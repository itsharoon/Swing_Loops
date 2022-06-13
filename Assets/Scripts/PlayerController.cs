using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static bool GameStarted = false;
    private bool Grounded = true;

    private Animator PlayerAnimator;

    [HideInInspector]
    public float MoveSpeed;

    [HideInInspector]
    public int Cash = 0;
    [HideInInspector]
    public int ScoreMultiplier = 0;

    [HideInInspector]
    public Text CashText;
    [HideInInspector]
    public Text ScoreText;

    [HideInInspector]
    public AudioClip PickUp;
    [HideInInspector]
    public AudioClip Groundimpact;

    [HideInInspector]
    public GameObject StartPanel;
    [HideInInspector]
    public GameObject FailPanel;
    [HideInInspector]
    public GameObject CompletePanel;
    [HideInInspector]
    public GameObject LandingDustEffect;
    [HideInInspector]
    public GameObject[] FinishLine;
    [HideInInspector]
    public GameObject[] Compliments;


    #region DEFAULT FUNCTIONS
    private void Awake()
    {
        PlayerAnimator = GetComponent<Animator>();
        GrapplingHook.FallAnim = false;
        StartPanel.SetActive(true);
        FailPanel.SetActive(false);
        CompletePanel.SetActive(false);
        Time.timeScale = 1.0f;
        GameStarted = false;
    }

    void Update()
    {
        CashText.text = "Cash: " + Cash.ToString();
        ScoreText.text = "Score: " + ScoreMultiplier.ToString();

        MovementController();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Grounded = true;
            GrapplingHook.FallAnim = false;

            if (GameStarted)
            {
                LandingDustEffect.GetComponent<ParticleSystem>().Play();
                gameObject.GetComponent<AudioSource>().PlayOneShot(Groundimpact);
                Compliments[Random.Range(0, Compliments.Length)].SetActive(true);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Grounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dollar"))
        {
            Cash = Cash + 10;
            other.GetComponent<AudioSource>().PlayOneShot(PickUp);
            Destroy(other.gameObject, 0.25f);
        }

        if (other.CompareTag("Boost"))
        {
            MoveSpeed = 15;
        }

        if (other.CompareTag("Finish"))
        {
            MoveSpeed = 0;
            PlayerAnimator.enabled = false;
            CompletePanel.SetActive(true);
            AddRandomForce();
        }

        if (other.CompareTag("KillCollider"))
        {
            Time.timeScale = 0.0f;
            FailPanel.SetActive(true);
        }
    }
    #endregion

    #region CUSTOM FUNCTIONS

    public void TapTpPlay()
    {
        PlayerAnimator.enabled = true;
        GameStarted = true;
        PlayerAnimator.SetBool("Run", true);
        PlayerAnimator.SetBool("Hang", false);
        PlayerAnimator.SetBool("Fall", false);
    }

    public void MovementController()
    {
        if (GameStarted)
        {
            if (Grounded)
            {
                transform.position += transform.forward * Time.deltaTime * MoveSpeed;
                PlayerAnimator.SetBool("Run", true);
                PlayerAnimator.SetBool("Hang", false);
                PlayerAnimator.SetBool("Fall", false);
            }
            else
            {
                if (!GrapplingHook.FallAnim)
                {
                    PlayerAnimator.SetBool("Run", false);
                    PlayerAnimator.SetBool("Hang", true);
                    PlayerAnimator.SetBool("Fall", false);

                    ScoreMultiplier++;
                }
                else
                {
                    PlayerAnimator.SetBool("Run", false);
                    PlayerAnimator.SetBool("Hang", false);
                    PlayerAnimator.SetBool("Fall", true);
                }
            }
        }
    }

    void AddRandomForce()
    {
        for (int i = 0; i < FinishLine.Length; i++)
        {
            FinishLine[i].GetComponent<Rigidbody>().AddRelativeForce(Vector3.left * 10f, ForceMode.Impulse);
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }
    #endregion
}
