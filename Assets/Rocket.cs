using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Rocket : MonoBehaviour
{
    // Game State

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThurst = 10f;
    [SerializeField] float levelLoadDelay = 1f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip crash;
    [SerializeField] AudioClip WinLevel;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    bool isTransitioning = false;
    bool collisionsAreEnabled = true;

    // enum State { Alive, Dying, Transcinding }
    // State state = State.Alive;
    
    // Use this for initialization
    void Start()
    {
        
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTransitioning)
        {
            RespondToThrustInput();
            RespondToRotateInput();
            if (UnityEngine.Debug.isDebugBuild)
            {
                OnDebugKey();
            }
        }
    }
   private void OnDebugKey()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsAreEnabled = !collisionsAreEnabled;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (isTransitioning || !collisionsAreEnabled) { return; }

        {
            switch (collision.gameObject.tag)
            {
                case "Friendly":
                    break;
                case "Enemy":
                    deathParticles.Play();
                    isTransitioning = true;
                    Invoke("Restart", 1f);
                    break;
                case "Fuel":
                    break;
                case "Finish":
                    StartWinLevel();
                    break;
            }
        }
        if (collision.gameObject.tag == "Enemy")
        {
            audioSource.Stop();
            audioSource.PlayOneShot(crash);
        }
        else
        {
            audioSource.Stop();
            if(collision.gameObject.tag == "Finish")
            {
                audioSource.PlayOneShot(WinLevel);
            }
        }
    }

    private void StartWinLevel()
    {
        isTransitioning = true;
        successParticles.Play();
        audioSource.Stop();
        audioSource.PlayOneShot(WinLevel);
        Invoke("LoadNextLevel", levelLoadDelay); // parameterise time
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {            
            ApplyThrust();            
         }
        else
        {
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust()
    {
        audioSource.Stop(); //stop audio
        mainEngineParticles.Stop(); //stop particles
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThurst * Time.deltaTime);
        
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        if (!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }
        
    }

    private void RespondToRotateInput()
    {
        rigidBody.angularVelocity = Vector3.zero; // remove rotation due to physics
             
        

        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
    }
    private void Restart()
    {
        
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    private void LoadNextLevel()
    {
        isTransitioning = true;
        int scene = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = scene + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; // loop back to start.
        }
        SceneManager.LoadScene(nextSceneIndex);
    }
}
