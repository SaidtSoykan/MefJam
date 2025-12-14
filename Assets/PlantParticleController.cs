using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MoreMountains.Feedbacks;

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
        public MMF_Player feedback;
    }

    [SerializeField] StateParticle[] stateParticles;
    [SerializeField] float fadeDuration = 0.5f;
    [SerializeField] Ease fadeEase = Ease.InOutSine;

    PlantState currentState;
    ParticleSystem currentParticle;
    Tween currentTween;
    Dictionary<ParticleSystem, Tween> _particleTweens = new();
    
    
    // public void SetState(PlantState newState)
    // {
    //     if (newState.Equals(currentState))
    //         return;
    //
    //     ParticleSystem nextParticle = GetParticle(newState);
    //     if (nextParticle == null)
    //         return;
    //
    //     currentTween?.Kill();
    //
    //     // Fade OUT current
    //     if (currentParticle != null)
    //     {
    //         currentTween = DOTween.To(
    //             () => GetAlpha(currentParticle),
    //             a => SetAlpha(currentParticle, a),
    //             0f,
    //             fadeDuration
    //         ).SetEase(fadeEase)
    //          .OnComplete(() =>
    //          {
    //              currentParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    //          });
    //     }
    //
    //     // Fade IN next
    //     SetAlpha(nextParticle, 0f);
    //     nextParticle.Play();
    //
    //     DOTween.To(
    //         () => GetAlpha(nextParticle),
    //         a => SetAlpha(nextParticle, a),
    //         1f,
    //         fadeDuration
    //     ).SetEase(fadeEase);
    //
    //     currentParticle = nextParticle;
    //     currentState = newState;
    // }
    //
     MMF_Player GetFeedback(PlantState state)
     {
         foreach (var sp in stateParticles)
             if (sp.state == state)
                 return sp.feedback;
    
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
        MMF_Player ps = GetFeedback(state);
        if (ps == null)
            return;
        
        if (!value)
        {
            ps.PlayFeedbacks();
            ps.Revert();
        }
        else
        {
            ps.PlayFeedbacks();
            ps.Revert();
        }
    }
    [SerializeField] MMF_Player gameEndFeedback;
    public void GiveGameEndReward()
    {
        gameEndFeedback.PlayFeedbacks();
    }
}
