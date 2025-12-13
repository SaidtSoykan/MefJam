using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlantParticleController : MonoBehaviour
{
    public enum PlantState
    {
        Growing,
        Reversing,
        Absorbing
    }

    [System.Serializable]
    public class StateParticle
    {
        public PlantState state;
        public ParticleSystem particle;
    }

    [SerializeField] StateParticle[] stateParticles;
    [SerializeField] float fadeDuration = 0.5f;
    [SerializeField] Ease fadeEase = Ease.InOutSine;

    PlantState currentState;
    ParticleSystem currentParticle;
    Tween currentTween;
    Dictionary<ParticleSystem, Tween> _particleTweens = new();

    void Awake()
    {
        foreach (var sp in stateParticles)
        {
            if (sp.particle == null) continue;

            SetAlpha(sp.particle, 0f);
            sp.particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    public void SetState(PlantState newState)
    {
        if (newState.Equals(currentState))
            return;

        ParticleSystem nextParticle = GetParticle(newState);
        if (nextParticle == null)
            return;

        currentTween?.Kill();

        // Fade OUT current
        if (currentParticle != null)
        {
            currentTween = DOTween.To(
                () => GetAlpha(currentParticle),
                a => SetAlpha(currentParticle, a),
                0f,
                fadeDuration
            ).SetEase(fadeEase)
             .OnComplete(() =>
             {
                 currentParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
             });
        }

        // Fade IN next
        SetAlpha(nextParticle, 0f);
        nextParticle.Play();

        DOTween.To(
            () => GetAlpha(nextParticle),
            a => SetAlpha(nextParticle, a),
            1f,
            fadeDuration
        ).SetEase(fadeEase);

        currentParticle = nextParticle;
        currentState = newState;
    }

    ParticleSystem GetParticle(PlantState state)
    {
        foreach (var sp in stateParticles)
            if (sp.state == state)
                return sp.particle;

        return null;
    }

    float GetAlpha(ParticleSystem ps)
    {
        return ps.main.startColor.color.a;
    }

    void SetAlpha(ParticleSystem ps, float alpha)
    {
        var main = ps.main;
        Color c = main.startColor.color;
        c.a = alpha;
        main.startColor = c;
    }


    public void Play(bool value, PlantState state)
    {
        ParticleSystem ps = GetParticle(state);
        if (ps == null)
            return;
        
        if (!value)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            ps.transform.localScale = Vector3.zero;
        }
        else
        {
            ps.Play();
            ps.transform.localScale = Vector3.one;
        }
    }
}
