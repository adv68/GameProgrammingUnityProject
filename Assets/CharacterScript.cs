using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CharacterScript : MonoBehaviour
{
    [SerializeField]
    GameObject bullet;

    AudioSource soundSource;

    [SerializeField]
    AudioClip fireSound;

    [SerializeField]
    AudioClip reloadSound;

    [SerializeField]
    AudioClip wolfSound;

    CharacterController charController;
    Animator animator;
    Camera mainCamera;
    Transform cameraTransform;

    GameObject crosshair;
    GameObject canvasPanel;
    Image canvasPanelImage;
    GameObject canvasText;

    Vector3 originalCameraPos;
    Vector3 shootingCameraPos = new(0.08f, 1.7f, -0.1f);

    Quaternion originalCameraRot;
    Quaternion shootingCameraRot = Quaternion.Euler(10.0f, 0.0f, 0.0f);

    Vector3 cameraTargetPos = new();
    Vector3 cameraSourcePos = new();

    Quaternion cameraTargetRot = new();
    Quaternion cameraSourceRot = new();

    float cameraMotionTime = 0.0f;
    bool shiftingCamera = false;

    bool gunUp = false;
    bool gunUpPressed = false;
    bool gunFirePressed = false;
    bool shooting = false;

    const int magazineCapacity = 10;
    int numInMagazine = magazineCapacity;
    bool reloading = false;

    Vector3 bulletStart = new(0.1f, 1.5f, 1.5f);

    bool dying = false;
    bool winning = false;

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        cameraTransform = mainCamera.transform;

        crosshair = GameObject.Find("Crosshair");
        crosshair.SetActive(false);

        canvasPanel = GameObject.Find("Panel");
        canvasPanel.SetActive(false);
        canvasPanelImage = canvasPanel.GetComponent<Image>();

        canvasText = GameObject.Find("Text");
        canvasText.SetActive(false);

        originalCameraPos = cameraTransform.localPosition;
        originalCameraRot = cameraTransform.localRotation;

        soundSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dying)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.RotateAround(transform.position, new Vector3(0, 1, 0), mouseX * 10.0f);

        cameraTransform.localRotation = Quaternion.Euler(cameraTransform.localRotation.eulerAngles.x - mouseY, 0.0f, 0.0f);
    }

    void FixedUpdate()
    {
        if (dying)
        {
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float toggleGunBtn = Input.GetAxis("Fire2");
        float shoot = Input.GetAxis("Fire1");

        if (gunUp && shoot > 0.0f && !gunFirePressed && !reloading && !shooting)
        {
            animator.SetTrigger("PistolFire");

            gunFirePressed = true;

            soundSource.PlayOneShot(fireSound, 10.0f);

            Quaternion bulletRotation = transform.rotation * Quaternion.Euler(90.0f + cameraTransform.rotation.eulerAngles.x - 2.0f, 0.0f, 0.0f);

            Instantiate(bullet, transform.position + (transform.rotation * bulletStart), bulletRotation);

            numInMagazine--;

            StartCoroutine(ShootCoroutine());

            if (numInMagazine <= 0)
            {
                numInMagazine = magazineCapacity;
                StartCoroutine(ReloadCoroutine());
            }
        }
        else if (shoot <= 0.0f && gunFirePressed)
        {
            gunFirePressed = false;
        }

        bool newGunUp = gunUp;

        if (toggleGunBtn > 0.0f && !gunUpPressed)
        {
            gunUpPressed = true;

            newGunUp = !gunUp;
        }
        else if (toggleGunBtn <= 0.0f && gunUpPressed)
        {
            gunUpPressed = false;
        }

        if (newGunUp != gunUp)
        {
            if (newGunUp)
            {
                ShiftCamera(shootingCameraPos, shootingCameraRot);
            }
            else
            {
                ShiftCamera(originalCameraPos, originalCameraRot);
            }

            gunUp = newGunUp;

            crosshair.SetActive(gunUp);
        }
        else
        {
            animator.SetBool("PistolUp", gunUp);

            if (!gunUp)
            {
                bool running = Input.GetKey(KeyCode.LeftShift);
                if (h > 0.05f)
                {
                    animator.SetInteger("SideMotion", 1);
                    running = false;
                }
                else if (h < -0.05f)
                {
                    animator.SetInteger("SideMotion", -1);
                    running = false;
                }
                else
                {
                    animator.SetInteger("SideMotion", 0);
                }


                var tempVect = new Vector3(h, 0, v).normalized;

                tempVect = transform.rotation * tempVect * (running ? 14 : 7) * Time.fixedDeltaTime * 35;

                charController.SimpleMove(tempVect);

                if (v > 0.05f)
                {
                    animator.SetInteger("ForwardMotion", 1);
                }
                else if (v < -0.05f)
                {
                    animator.SetInteger("ForwardMotion", -1);
                }
                else
                {
                    animator.SetInteger("ForwardMotion", 0);
                }

                animator.SetBool("Running", running);
            }
            else
            {
                var tempVect = new Vector3(h, 0, v).normalized;

                tempVect = transform.rotation * tempVect * 20 * Time.fixedDeltaTime;

                charController.SimpleMove(tempVect);
            }
        }
    }

    void ShiftCamera(Vector3 targetPos, Quaternion targetRot)
    {
        cameraSourcePos = cameraTransform.localPosition;
        cameraSourceRot = cameraTransform.localRotation;

        if (shiftingCamera)
        {
            cameraMotionTime = 1.0f - cameraMotionTime;
        }
        else
        {
            cameraMotionTime = 0.0f;
        }

        cameraTargetPos = targetPos;
        cameraTargetRot = targetRot;

        StartCoroutine(CameraShiftCoroutine());
    }

    IEnumerator CameraShiftCoroutine()
    {
        while (cameraMotionTime < 1.0f)
        {
            cameraTransform.SetLocalPositionAndRotation(
                Vector3.Lerp(cameraSourcePos, cameraTargetPos, cameraMotionTime),
                Quaternion.Lerp(cameraSourceRot, cameraTargetRot, cameraMotionTime));

            cameraMotionTime += (Time.deltaTime * 2.0f); // so the camera moves in 0.5 sec

            yield return null;
        }

        cameraMotionTime = 1.0f;
        shiftingCamera = false;
    }

    IEnumerator ShootCoroutine()
    {
        shooting = true;

        yield return new WaitForSeconds(0.75f);

        shooting = false;
    }

    IEnumerator ReloadCoroutine()
    {
        reloading = true;

        yield return new WaitForSeconds(1.0f);

        soundSource.PlayOneShot(reloadSound, 10.0f);

        yield return new WaitForSeconds(0.75f);

        reloading = false;
    }

    IEnumerator WinCoroutine()
    {
        if (dying || winning)
        {
            yield break;
        }

        winning = true;
        float timer = 0.0f;

        Color c1 = new(1.0f, 1.0f, 1.0f, 0.0f);
        Color c2 = new(0.4f, 0.4f, 0.4f, 1.0f);
        Color c3 = new(0.0f, 0.0f, 0.0f, 1.0f);
        canvasPanelImage.color = c1;

        soundSource.Stop();

        canvasPanel.SetActive(true);

        while (timer < 1.0f)
        {
            canvasPanelImage.color = Color.Lerp(c1, c2, timer);
            timer += Time.deltaTime;

            yield return null;
        }

        canvasText.GetComponent<TMP_Text>().text = "You Win";
        canvasText.SetActive(true);

        timer = 0.0f;

        while (timer < 1.0f)
        {
            canvasPanelImage.color = Color.Lerp(c2, c3, timer);
            timer += Time.deltaTime;

            yield return null;
        }

        yield return new WaitForSeconds(2.0f);

        SceneManager.LoadScene("StartScene");
    }

    IEnumerator DieCoroutine()
    {
        if (dying || winning)
        {
            yield break;
        }

        dying = true;
        float timer = 0.0f;

        Color c1 = new(1.0f, 0.0f, 0.0f, 0.0f);
        Color c2 = new(0.8f, 0.25f, 0.33f, 1.0f);
        Color c3 = new(0.0f, 0.0f, 0.0f, 1.0f);
        canvasPanelImage.color = c1;

        soundSource.Stop();
        soundSource.PlayOneShot(wolfSound, 20.0f);

        canvasPanel.SetActive(true);

        while (timer < 1.0f)
        {
            canvasPanelImage.color = Color.Lerp(c1, c2, timer);
            timer += Time.deltaTime;

            yield return null;
        }

        canvasText.GetComponent<TMP_Text>().text = "You Lose";
        canvasText.SetActive(true);

        timer = 0.0f;

        while (timer < 1.0f)
        {
            canvasPanelImage.color = Color.Lerp(c2, c3, timer);
            timer += Time.deltaTime;

            yield return null;
        }

        yield return new WaitForSeconds(5.0f);

        SceneManager.LoadScene("StartScene");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wolf"))
        {
            StartCoroutine(DieCoroutine());
        }
        else if (other.gameObject.CompareTag("Cabin"))
        {
            StartCoroutine(WinCoroutine());
        }
    }
}
