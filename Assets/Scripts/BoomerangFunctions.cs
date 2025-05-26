using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangFunctions : MonoBehaviour
{
    private enum BoomerangState { Rest, Launched, Returning, Melee }
    private BoomerangState state = BoomerangState.Rest;

    [Header("References")]
    public GameObject restBoomerang;
    public GameObject launchBoomerang;
    public GameObject meleeBoomerang;
    public Collider catchZoneCollider;
    public HUDController hud;

    [Header("Movement Settings")]
    public float launchSpeed = 10f;
    public float returnSpeed = 8f;
    public float maxDistance = 15f;
    public float collisionDetectRadius = 0.2f;
    public LayerMask collisionLayers;
    public float catchDistance = 0.5f;

    [Header("Rest Position Offset")]
    public Vector3 restOffset = new Vector3(0, 0, -1);

    [Header("Launch Offset")]
    public Vector3 launchOffset = new Vector3(0, 0, 1);

    [Header("Melee Settings")]
    [Tooltip("Cooldown (seconds) después de un ataque melee/cargado")]
    public float meleeCooldown = 2f;
    [Tooltip("Segundos necesarios para carga de ataque cargado")]
    public float chargeThreshold = 1f;
    private float meleeCooldownTimer = 0f;

    private bool rightClicked = false;
    private Vector3 launchDirection;
    private Vector3 launchStartPosition;

    // Melee state vars
    private bool isHolding = false;
    private bool isCharging = false;
    private float holdTime = 0f;
    public float multi = 0f;
    private Camera mainCamera;
    private Animator meleeAnimator;

    void Start()
    {
        state = BoomerangState.Rest;
        restBoomerang.SetActive(true);
        launchBoomerang.SetActive(false);
        meleeBoomerang.SetActive(false);
        restBoomerang.transform.localPosition = restOffset;
        restBoomerang.transform.localRotation = Quaternion.identity;
        if (catchZoneCollider != null)
            catchZoneCollider.enabled = true;

        mainCamera = Camera.main;
        meleeAnimator = meleeBoomerang.GetComponent<Animator>();
        multi = 1f;
    }

    void Update()
    {
        if (meleeCooldownTimer > 0f)
            meleeCooldownTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(1)) rightClicked = true;
        if (Input.GetMouseButtonUp(1)) rightClicked = false;

        switch (state)
        {
            case BoomerangState.Rest:
                HandleRestState();
                break;

            case BoomerangState.Melee:
                HandleMeleeState();
                break;

            case BoomerangState.Launched:
            case BoomerangState.Returning:
                MoveLaunchBoomerang();
                break;
        }
    }

    private void HandleRestState()
    {
        restBoomerang.transform.localPosition = restOffset;
        restBoomerang.transform.localRotation = Quaternion.identity;

        // Launch
        if (rightClicked && Input.GetMouseButtonDown(0) && meleeCooldownTimer <= 0f)
        {
            Launch();
            return;
        }

        // Begin Melee
        if (Input.GetMouseButtonDown(0) && !rightClicked && meleeCooldownTimer <= 0f)
        {
            state = BoomerangState.Melee;
            hud.barraCarga.gameObject.SetActive(true);
            isHolding = true;
            isCharging = false;
            holdTime = 0f;
            // Play normal attack
            meleeBoomerang.SetActive(true);
            if (meleeAnimator != null)
                meleeAnimator.Play("Boomerang Attack");
        }
    }

    private void HandleMeleeState()
    {
        if (!isHolding) return;
        // 1) Acumular tiempo de hold
        holdTime += Time.deltaTime;
        // Limitar a threshold para no crecer más
        holdTime = Mathf.Min(holdTime, chargeThreshold);

        // 2) Actualizar barra de carga de 0 a 1 de forma totalmente lineal
        float progress = holdTime / chargeThreshold;
        hud?.UpdateChargeBar(progress);

        // Obtener estado de animación actual
        AnimatorStateInfo info = meleeAnimator.GetCurrentAnimatorStateInfo(0);

        // 3) Al completar animación normal y si mantiene presionado, entrar en carga
        if (info.IsName("Boomerang Attack") && info.normalizedTime >= 1f && Input.GetMouseButton(0) && !isCharging)
        {
            isCharging = true;
            meleeAnimator?.Play("Boomerang Attack Charge");
            hud?.UpdateChargeBar(1f); // lleno
            return;
        }

        // 4) Si suelta después de cargar o después de iniciar carga
        if (Input.GetMouseButtonUp(0) && (info.IsName("Boomerang Attack Charge") || isCharging))
        {
            isHolding = false;
            isCharging = false;
            // Release
            multi = Mathf.Lerp(1f, 2f, Mathf.Clamp01(holdTime / chargeThreshold));
            meleeAnimator?.Play("Boomerang Attack Release");
            hud?.HideChargeBar();
            StartCoroutine(HideMeleeAfterRelease());
            return;
        }

        // Release
        if (Input.GetMouseButtonUp(0))
        {
            if (isCharging)
            {
                if (meleeAnimator != null)
                    multi = Mathf.Lerp(1f, 2f, Mathf.Clamp01(holdTime / chargeThreshold));
                meleeAnimator.Play("Boomerang Attack Release");
            }
            // Reset
            isHolding = false;
            state = BoomerangState.Rest;
            meleeCooldownTimer = meleeCooldown;
            // Hide after animation
            StartCoroutine(HideMeleeAfterRelease());
        }
    }

    private IEnumerator HideMeleeAfterRelease()
    {
        // Wait for release clip length
        AnimatorStateInfo info = meleeAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(info.length);
        meleeBoomerang.SetActive(false);
        hud.barraCarga.gameObject.SetActive(false);
        state = BoomerangState.Rest;
        multi = 1f;
    }

    private void Launch()
    {
        state = BoomerangState.Launched;
        restBoomerang.SetActive(false);
        launchBoomerang.SetActive(true);
        launchBoomerang.transform.SetParent(null);
        launchBoomerang.transform.rotation = Quaternion.identity;
        Vector3 startPos = transform.position + transform.TransformVector(launchOffset);
        launchBoomerang.transform.position = startPos;
        launchStartPosition = startPos;
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 target = ray.origin + ray.direction * maxDistance;
        launchDirection = (target - startPos).normalized;
        if (catchZoneCollider != null)
            catchZoneCollider.enabled = false;
    }

    private void MoveLaunchBoomerang()
    {
        Vector3 currentPos = launchBoomerang.transform.position;
        if (state == BoomerangState.Launched)
        {
            launchBoomerang.transform.position = currentPos + launchDirection * launchSpeed * Time.deltaTime;
            bool hitEnv = Physics.CheckSphere(currentPos, collisionDetectRadius, collisionLayers);
            float traveled = Vector3.Distance(launchStartPosition, currentPos);
            if (hitEnv || traveled >= maxDistance)
            {
                state = BoomerangState.Returning;
                if (catchZoneCollider != null)
                    catchZoneCollider.enabled = true;
            }
        }
        else // Returning
        {
            Vector3 returnDir = (transform.position - currentPos).normalized;
            launchBoomerang.transform.position = currentPos + returnDir * returnSpeed * Time.deltaTime;
            if (Vector3.Distance(currentPos, transform.position) <= catchDistance)
                CatchBoomerang();
        }
    }

    private void CatchBoomerang()
    {
        state = BoomerangState.Rest;
        launchBoomerang.SetActive(false);
        restBoomerang.SetActive(true);
        restBoomerang.transform.localPosition = restOffset;
        restBoomerang.transform.localRotation = Quaternion.identity;
        launchBoomerang.transform.SetParent(transform);
        launchBoomerang.transform.localPosition = launchOffset;
        if (catchZoneCollider != null)
            catchZoneCollider.enabled = true;
    }
}
