using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    //private void Start()
    //{
    //    Debug.Log("Lander Start");
    //}

    public event EventHandler OnUpForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnBeforeForce;
    public event EventHandler OnFuelChanged;
    public event EventHandler OnScoreChanged;

    [SerializeField] private float maxFuelAmount = 20f;
    [SerializeField] private float startingFuelAmount = 10f;
    [SerializeField] private float fuelPickupAmount = 5f;
    [SerializeField] private int scorePerCoin = 1;

    private Rigidbody2D landerRigidbody2D;

    private float fuelAmount;
    private int coinScore;
    private int landingScore;

    private void Awake() {
        landerRigidbody2D = GetComponent<Rigidbody2D>();
        fuelAmount = Mathf.Clamp(startingFuelAmount, 0f, maxFuelAmount);
    }

    private void Start()
    {
        OnFuelChanged?.Invoke(this, EventArgs.Empty);
        OnScoreChanged?.Invoke(this, EventArgs.Empty);
    }

    private void FixedUpdate() {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        if (Keyboard.current == null) return;

        if (Keyboard.current.upArrowKey.isPressed ||
            Keyboard.current.wKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed ||
            Keyboard.current.aKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed ||
            Keyboard.current.dKey.isPressed)
        {
            float fuelConsumptionRate = 1f;
            consumeFuel(fuelConsumptionRate);
        }
        if (fuelAmount <= 0f) return;

        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
        {
            float turnSpeed = +100f;
            landerRigidbody2D.AddTorque(turnSpeed * Time.fixedDeltaTime);
            OnLeftForce?.Invoke(this, EventArgs.Empty);
            
        }
        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
        {
            float turnSpeed = -100f;
            landerRigidbody2D.AddTorque(turnSpeed * Time.fixedDeltaTime);
            OnRightForce?.Invoke(this, EventArgs.Empty);

        }
        if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed) {
            float force = 700f;
            landerRigidbody2D.AddForce(force * transform.up * Time.fixedDeltaTime);
            OnUpForce?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (!collision2D.gameObject.TryGetComponent(out LandingPad landingPad))
        {
            Debug.Log("Crashed on the terrain!");
            return;
        }
        float softLandingVelocityMagnitude = 4f;
        float relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;
        if (relativeVelocityMagnitude > softLandingVelocityMagnitude)
        {
            Debug.Log("Hard landing! Velocity: " + collision2D.relativeVelocity.magnitude);
            return;

        }
        Vector2 transformUp2D = new Vector2(transform.up.x, transform.up.y);
        float dotVector = Vector2.Dot(Vector2.up, transformUp2D);
        float minDotVector = .90f;
        if (dotVector < minDotVector)
        {
            Debug.Log("Landed at a bad angle!");
            return;
        }
        Debug.Log("Landing success, Velocity: " + collision2D.relativeVelocity.magnitude);

        float maxScoreAmountLandingAngle = 100;
        float scoreDotVectorMultiplier = 10f;
        float landingAngleScore = maxScoreAmountLandingAngle - Mathf.Abs(dotVector - 1f) * scoreDotVectorMultiplier * maxScoreAmountLandingAngle;

        float maxScoreAmountLandingVelocity = 100;
        float landingSpeedScore = (softLandingVelocityMagnitude - relativeVelocityMagnitude) * maxScoreAmountLandingVelocity;

        landingScore = Mathf.RoundToInt((landingSpeedScore + landingAngleScore) * landingPad.GetScoreMultiplier());
        Debug.Log("Score: " + landingScore);
        OnScoreChanged?.Invoke(this, EventArgs.Empty);

    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.TryGetComponent(out FuelPickup fuelpickup))
        {   
            Debug.Log("Fuel pickup!");
            AddFuel(fuelPickupAmount);
            fuelpickup.DestroySelf();
        }

        if (collider2D.gameObject.TryGetComponent(out CoinPickup coinPickup))
        {
            AddCoinScore(scorePerCoin);
            coinPickup.DestroySelf();
        }
    }

    private void consumeFuel(float fuelConsumptionRate)
    {
        fuelAmount = Mathf.Max(0f, fuelAmount - fuelConsumptionRate * Time.fixedDeltaTime);
        OnFuelChanged?.Invoke(this, EventArgs.Empty);
    }

    private void AddFuel(float addFuelAmount)
    {
        fuelAmount = Mathf.Clamp(fuelAmount + addFuelAmount, 0f, maxFuelAmount);
        OnFuelChanged?.Invoke(this, EventArgs.Empty);
    }

    private void AddCoinScore(int scoreAmount)
    {
        coinScore += scoreAmount;
        OnScoreChanged?.Invoke(this, EventArgs.Empty);
    }

    public float GetFuelNormalized()
    {
        if (maxFuelAmount <= 0f) return 0f;
        return fuelAmount / maxFuelAmount;
    }

    public float GetFuelAmount()
    {
        return fuelAmount;
    }

    public float GetMaxFuelAmount()
    {
        return maxFuelAmount;
    }

    public int GetCoinScore()
    {
        return coinScore;
    }

    public int GetLandingScore()
    {
        return landingScore;
    }

    public int GetTotalScore()
    {
        return coinScore + landingScore;
    }

}
