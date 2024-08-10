using Mirror;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class Aura : MonoBehaviour
{
    public AuraInfo auraInfo { get; private set; }
    public Unit target { get; private set; }
    private float duration;
    public Unit caster { get; private set; }
    public AuraEffectHandler effectHandler { get; private set; }
    public Spell spell { get; private set; }

    public bool m_hasTimer {  get; private set; }

    public bool m_isPeriodic {  get; private set; }

    public float m_tickTime { get; private set; }

    public float m_stacks { get; private set; }
    private float m_timeBetweenTicks;
    private bool finished = false;
    public bool m_effectHit = false;
    public void Initialize(AuraInfo auraInfo, Unit caster, Unit target, Spell spell)
    {

        DontDestroyOnLoad(gameObject);

        this.auraInfo = auraInfo;
        this.target = target;
        this.duration = auraInfo.Duration;
        print($"HIS DURATION IS: {this.duration}");
        this.caster = caster;
        this.spell = spell;
        this.m_stacks = auraInfo.Stacks;

        this.m_isPeriodic = auraInfo.Periodic;
        this.m_hasTimer = duration < 9999;
        this.m_tickTime = auraInfo.TickTime;
        this.m_timeBetweenTicks = this.m_tickTime;

        // Add this aura to the target's aura list
        target.InitAura(this);

        // Handle SpellEffects;
        effectHandler = new AuraEffectHandler();

        // Apply initial effects if necessary
        HandleEffects();
    }

    private void HandleEffects()
    {
        // Implement logic to apply aura effects to the target
        effectHandler.HandleEffects(this);
    }

    private void Finish()
    {
        if (m_stacks == 0)
        {
            CancelEffects();

            // TODO: Handle OnRemove aurascript
            // auraInfo.AuraScript.OnRemove();
            finished = true;
            // Remove the aura from the target's aura list
            target.DestAura(this);

            // Destroy the aura object
            Destroy(gameObject);
            print("Aura has dropped off!");
        }

        
    }

    public void CancelEffects()
    {
        //TODO: Cancel effects
    }

    public void Refresh()
    {
        // Reset the duration to the original value
        duration = auraInfo.Duration;
        Debug.Log($"{auraInfo.Name} refreshed for {target.name}");
    }

    public void DropStack()
    {
        // drop stacks by 1
        if (m_stacks > 0)
            m_stacks--;
    }

    public void DropStacks()
    {
        // drop stacks by 1
        if (m_stacks > 0)
            m_stacks = 0;
    }

    public Spell ToSpell()
    {
        return spell;
    }
    void Update()
    {
        if (finished)
            return;

        if (!m_isPeriodic)
        {
            if (!m_effectHit)
            {
                HandleEffects();
                m_effectHit = true;
            }
        }
        else
        {
            if (m_timeBetweenTicks > 0)
                m_timeBetweenTicks -= Time.deltaTime;
            else
            {
                HandleEffects();
                m_timeBetweenTicks = m_tickTime; // Reset tick timer;
            }
        }
        if (duration > 0)
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                DropStacks();
                HandleEffects();
                Finish();
            }
        }
    }
}
