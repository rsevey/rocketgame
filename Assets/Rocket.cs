using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    // Game State
    Rigidbody rigidBody;
    AudioSource audioSource;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThurst = 10f;

    enum State { Alive, Dying, Transcinding}
    State state = State.Alive;

    // Use this for initialization
    void Start ()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (state == State.Alive)
        {
            Rotate();
            Thrust();
        }
        

    }

    private void Rotate()
    {
        
        rigidBody.freezeRotation = true; // take manual control of rotation
        Thrust();
        float rotationThisFrame = rcsThrust * Time.deltaTime;


        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false; // resume physics control of rotation
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * mainThurst);
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; }

        {
           switch (collision.gameObject.tag)
            {
                case "Friendly":
                    break;
                case "Enemy":
                    state = State.Dying;                
                    Invoke("Restart", 1f);
                    break;
                case "Fuel":
                    break;
                case "Finish":
                    state = State.Transcinding;
                    Invoke("LoadNext", 1f); // parameterise time
                    break;
            }
        }
        if (collision.relativeVelocity.magnitude > 2)
            audioSource.Play();
    }

   private void Restart()
    {

        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    private void LoadNext()
    {
        state = State.Alive;
        SceneManager.LoadScene(1);
    }
}
