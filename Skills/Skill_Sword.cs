using System;
using UnityEngine;

public enum SwordType
{
    Regular,
    Ricochet,
    Pierce,
    Spin
}


public class Skill_Sword : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("Ricochet Info")]
    [SerializeField] private int numberOfRicochets;
    [SerializeField] private float ricochetGravity;
    [SerializeField] private float ricochetSpeed;

    [Header("Pierce Info")]
    [SerializeField] private int piercedTargets;
    [SerializeField] private float pierceGravity;

    [Header("Spin Info")]
    [SerializeField] private float hitCooldown = .35f;
    [SerializeField] private float maxTravelDistance = 7;
    [SerializeField] private float spinDuration = 2;
    [SerializeField] private float spinGravity = 1;

    [Header("Skill Info")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

    private Vector2 finalDir;

    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();

        GenerateDots();

        SetupGravity();
    }

    private void SetupGravity()
    {
        if (swordType == SwordType.Ricochet)
            swordGravity = ricochetGravity;
        else if (swordType == SwordType.Pierce)
            swordGravity = pierceGravity;
        else if (swordType == SwordType.Spin)
            swordGravity = spinGravity;
    }

    protected override void Update()
    {
        if (Input.GetKeyUp(KeyCode.Joystick1Button4))
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);

        if (Input.GetKey(KeyCode.Joystick1Button4))
        {
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);

        if (Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }
    }

    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        Skill_Sword_Controller newSwordScript = newSword.GetComponent<Skill_Sword_Controller>();

        if (swordType == SwordType.Ricochet)
            newSwordScript.SetupRicochet(true, numberOfRicochets, ricochetSpeed);
        else if (swordType == SwordType.Pierce)
            newSwordScript.SetupPierce(piercedTargets);
        else if (swordType == SwordType.Spin)
            newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration, hitCooldown);
        
        newSwordScript.SetupSword(finalDir, swordGravity, player, freezeTimeDuration, returnSpeed);

        player.AssignNewSword(newSword);

        DotsActive(false);
    }

    #region Aim Region
    //public Vector2 AimDirection()
    //{
    //    // Controller input
    //    Vector2 targetPosition = new Vector2(Input.GetAxis("RHorizontal"), Input.GetAxis("RVertical"));
    //    Vector2 direction = targetPosition.normalized;

    //    return direction;
    //}

    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 direction = Vector2.zero;

        // Check if a game controller is connected and has input
        float controllerHorizontalInput = Input.GetAxis("RHorizontal");
        float controllerVerticalInput = Input.GetAxis("RVertical");

        if (Mathf.Abs(controllerHorizontalInput) > 0.1f || Mathf.Abs(controllerVerticalInput) > 0.1f)
        {
            // Use controller input
            direction = new Vector2(controllerHorizontalInput, controllerVerticalInput).normalized;
        }
        else
        {
            // Use mouse input
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = mousePosition - playerPosition;
        }

        return direction;
    }

    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y) * t + .5f * (Physics2D.gravity * swordGravity) * (t * t); // Formula for uniformly accelerated motion: s = ut + 1/2at^2

        return position;
    }
    #endregion
}
