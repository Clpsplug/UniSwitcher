using System;
using UniSwitcher.Domain;
using UnityEngine;

namespace UniSwitcher.Sample
{
    /// <summary>
    /// Sample Transition Background
    /// </summary>
    public class TransitionBackgroundController : MonoBehaviour, ITransitionBackgroundController
    {
        [SerializeField] private Animator transitionAnimator = default;

        [SerializeField] private Canvas mainCanvas = default;

        private static readonly int TransitionIn = Animator.StringToHash("TransitionIn");
        private static readonly int TransitionOut = Animator.StringToHash("TransitionOut");
        private static readonly int ToTransitionWait = Animator.StringToHash("ToTransitionWait");
        private readonly int _transitionReadyState = Animator.StringToHash("Base Layer.TransitionReady");
        private readonly int _transitionInState = Animator.StringToHash("Base Layer.TransitionIn");
        private readonly int _transitionWaitState = Animator.StringToHash("Base Layer.TransitionWait");
        private readonly int _transitionOutState = Animator.StringToHash("Base Layer.TransitionOut");

        private void Start()
        {
            DetectMainCamera();
        }

        public void DetectMainCamera()
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeCameraMain
            var mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCanvas.worldCamera = mainCamera;
                return;
            }

            Debug.LogWarning("No main camera found for transition object. It might work in weird way");
        }

        public bool IsTransitionAllowed()
        {
            return GetTransitionState() == TransitionState.Ready || GetTransitionState() == TransitionState.Wait;
        }

        public TransitionState GetTransitionState()
        {
            var stateHash = transitionAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;
            if (stateHash == _transitionReadyState)
            {
                return TransitionState.Ready;
            }
            else if (stateHash == _transitionInState)
            {
                return TransitionState.In;
            }
            else if (stateHash == _transitionWaitState)
            {
                return TransitionState.Wait;
            }
            else if (stateHash == _transitionOutState)
            {
                return TransitionState.Out;
            }
            else
            {
                throw new Exception("None of the state hash match, this should not happen");
            }
        }

        public void TriggerTransitionIn()
        {
            if (GetTransitionState() == TransitionState.Ready || GetTransitionState() == TransitionState.Out)
            {
                transitionAnimator.SetTrigger(TransitionIn);
            }
        }

        public void TriggerTransitionOut()
        {
            if (GetTransitionState() == TransitionState.Wait || GetTransitionState() == TransitionState.In)
            {
                transitionAnimator.SetTrigger(TransitionOut);
            }
        }

        public void ForceTransitionWait()
        {
            transitionAnimator.SetTrigger(ToTransitionWait);
        }
    }
}