using UnityEngine;

public class AnimatorStateBehavior : StateMachineBehaviour
{
    // Declare parameters
    private Transform casterTransform;
    private Transform targetTransform;
    private int spellId;
    private float speed;
    private float timer = 0.15f;
    private bool VFXGoneOff = false;
    // This method is called when the state machine enters a state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Fetch parameters from the Animator's GameObject or a script attached to it
        AnimationHandler animationHandler = animator.GetComponentInParent<AnimationHandler>();
        if (animationHandler != null)
        {
            spellId = animationHandler.SpellId; // Assuming AnimationHandler has SpellId, etc.
            speed = animationHandler.SpellSpeed;
            casterTransform = animationHandler.CasterTransform;
            targetTransform = animationHandler.TargetTransform;

            VFXGoneOff = false;
            timer = 0.15f;
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else
        {
            if (!VFXGoneOff)
            {
                VFXManager.Instance.CastSpell(spellId, speed, casterTransform, targetTransform);
                timer = 0;
                VFXGoneOff = true;
                return;
            }
        }
    }

    // This method is called when the state machine exits a state
}
