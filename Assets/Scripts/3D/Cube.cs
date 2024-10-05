using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// using System.Diagnostics;

// using System.Numerics;
using DG.Tweening;

// using System.Numerics;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Cube : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
{
    // [SerializeField] TMP_InputField _debugInput; // TODO: remove;
    private MeshRenderer _meshRenderer;
    private Canvas _numberCanvas;
    private Rigidbody _rigidBody;
    private BoxCollider _collider;
    private MergeEffect _mergeEffect;
    private NumberDisplay[] _numberTexts;
    private Settings _settings;
    private Tweener[] _mergeTweeners = new Tweener[2];
    private Effect.Factory _collisionEffectFactory;
    private SignalBus _signalBus;
    private float _elapsedTime = 0f; //TODO: calculate when needed
    private int _powerOfTwo = 1;
    private IMemoryPool _pool;

    public int Value { get; private set; }
    public bool UseGravity => _rigidBody.useGravity;
    public bool Isinteractable { get; set; } = true;

    public UnityAction<Transform> PositionChanged;
    public static int CubeCount { get; private set; } = 0;

    [Inject]
    private void Construct(Canvas numberCanvas, SignalBus signalBus, CollisionEffect.Factory collisionEffectFactory, MergeEffect mergeEffect, Settings settings)
    {
        _numberCanvas = numberCanvas;
        _signalBus = signalBus;
        _collisionEffectFactory = collisionEffectFactory;
        _mergeEffect = mergeEffect;
        _settings = settings;
    }

    private void Awake()
    {
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        _rigidBody = gameObject.GetComponent<Rigidbody>();
        _collider = gameObject.GetComponent<BoxCollider>();
        _numberTexts = _numberCanvas.GetComponentsInChildren<NumberDisplay>();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (transform.hasChanged)
        {
            PositionChanged?.Invoke(transform);
            transform.hasChanged = false;
        }
    }

    private void ChangeCubeCount(int newCount)
    {
        CubeCount = newCount;
        _signalBus.Fire<CubeCountChangedSignal>(new CubeCountChangedSignal() {Count = newCount});
    }

    private void RandomNumberBetweenTwoAndFour()
    {
        int randomValue = UnityEngine.Random.Range(1, 5);
        Value = randomValue > 1 ? 2 : 4;
        _powerOfTwo = Value / 2;
    }

    public static void ResetCubeCount()
    {
        CubeCount = 0;
    }

    public void FireCube(Vector3 force)
    {
        _rigidBody.AddForce(force, ForceMode.Impulse);
    }

    public void EnableGravity()
    {
        _rigidBody.useGravity = true;
    }
    
    public void DisableGravity()
    {
        _rigidBody.useGravity = false;
    }

    private void RenderValue()
    {
        foreach(NumberDisplay display in _numberTexts)
        {
            display.SetTextValue(Value);
        }

        if(_powerOfTwo < _settings.Materials.Count)
        {
            _meshRenderer.material.DOColor(_settings.Materials[_powerOfTwo - 1].color, _settings.MergeTime);
        }
        else
        {
            _meshRenderer.material.DOColor(_settings.DefaultMaterial.color, _settings.MergeTime);
        }
    }

    private void OnCollisionEnter(Collision collision) //TODO: make aproach with better performance
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Effect effect = _collisionEffectFactory.Create();
            effect.transform.position = contact.point;
            effect.Play();
        }
    }

    private void OnCollisionStay(Collision collision) //TODO: make aproach with better performance
    {
        Cube otherCube;

        if(Isinteractable && collision.gameObject.TryGetComponent<Cube>(out otherCube) && otherCube.Isinteractable && Value == otherCube.Value)
        {
            ContactPoint contact = collision.contacts[0];
            MergeCubes(otherCube, contact.point);
        }
    }

    private void MergeCubes(Cube otherCube, Vector3 contactPoint)
    {
        _signalBus.Fire<ScoreChangedSignal>(new ScoreChangedSignal() {Score = Value / 2});

        Cube cubeThatEnter, cubeThatIsEntered;
        Vector3 thisCounterVelocity = CalculateCounterVelocity(transform.position, contactPoint, GetVelocity());
        Vector3 otherCounterVelocity  = CalculateCounterVelocity(otherCube.transform.position, contactPoint, otherCube.GetVelocity());

        if(thisCounterVelocity.magnitude > otherCounterVelocity.magnitude)
        {
            cubeThatIsEntered = this;
            cubeThatEnter = otherCube;
        }
        else
        {
            cubeThatEnter = this;
            cubeThatIsEntered = otherCube;
        }
         
        // Debug.Log("2 same cubes collision");
        cubeThatEnter.Isinteractable = false;
        cubeThatIsEntered.Isinteractable = false;
        cubeThatIsEntered.BeginMergeAnimation(cubeThatEnter);
        cubeThatEnter.DoubleValue();
        cubeThatIsEntered.DoubleValue();
    }

    public void BeginMergeAnimation(Cube target)
    {
        DisableCollider();
        _mergeTweeners[0] = transform.DOMove(target.transform.position, _settings.MergeTime);
        _mergeTweeners[1] = transform.DORotate(target.transform.rotation.eulerAngles, _settings.MergeTime);
        _mergeTweeners[0].OnComplete(() => OnMergeComplete(target));

        target.PositionChanged += OnTargetTransformChanged;
        _elapsedTime = 0f;
    }

    private void OnMergeComplete(Cube target)
    {
        target.PlayMergeEffect();
        target.Isinteractable = true;
        target.PositionChanged -= OnTargetTransformChanged;
        Dispose();
    }

    public void PlayMergeEffect()
    {
        _mergeEffect.gameObject.SetActive(true); // TODO: rewrite strange activation
        _mergeEffect.Play();
    }

    private void OnTargetTransformChanged(Transform newTransform)
    {   
        
        float timeLeft = Math.Max(_settings.MergeTime - _elapsedTime, 0f);
        
        _mergeTweeners[0].ChangeEndValue(newTransform.position, timeLeft, true);
        _mergeTweeners[1].ChangeEndValue(newTransform.rotation.eulerAngles, timeLeft, true);
    }

    private static Vector3 CalculateCounterVelocity(Vector3 vectorStart, Vector3 vectorEnd, Vector3 velocity)
    {
        return Vector3.Project(velocity, vectorEnd - vectorStart);
    }

    public void DisableCollider()
    {
        _collider.enabled = false;
    }

    public Vector3 GetVelocity()
    {
        return _rigidBody.velocity;
    }

    public void DoubleValue()
    {
        ChangeValue(Value * 2);
        _powerOfTwo++;
        RenderValue();
    }

    private void ChangeValue(int newValue)
    {
        Value = newValue;
    }

    public void OnDespawned()
    {
        _pool = null;
        
        transform.rotation = quaternion.identity;
        _rigidBody.velocity = Vector3.zero;
        _rigidBody.angularVelocity = Vector3.zero;
        _collider.enabled = true;
        
        foreach(Tweener tweener in _mergeTweeners)
        {
            tweener.Kill(true);
        }

        ChangeCubeCount(CubeCount - 1);
    }

    public void OnSpawned(IMemoryPool pool)
    {
        _pool = pool;
        transform.rotation = quaternion.identity;
        RandomNumberBetweenTwoAndFour();
        RenderValue();
        ChangeCubeCount(CubeCount + 1);
        Isinteractable = true;
    }

    public void Dispose()
    {
        _pool.Despawn(this);
    }

    public class Factory : PlaceholderFactory<Cube>
    {

    }

    [Serializable]
    public class Settings
    {
        public Material DefaultMaterial;
        public List<Material> Materials;
        public float MergeTime;
    }
}
