/*
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License as published by the
 * Free Software Foundation; either version 2 of the License, or (at your
 * option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program. If not, see <http://www.gnu.org/licenses/>.
 */

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

    public int m_stacks { get; private set; }
    public int m_maxStacks { get; private set; }
    public bool m_stackable = false;
    private float m_timeBetweenTicks;
    private bool finished = false;
    public bool m_effectHit = false;
    public void Initialize(AuraInfo auraInfo, Unit caster, Unit target, Spell spell)
    {

        DontDestroyOnLoad(gameObject);

        this.auraInfo = auraInfo;
        this.target = target;
        this.duration = auraInfo.Duration;
        this.caster = caster;
        this.spell = spell;
        this.m_stacks = auraInfo.Stacks;
        this.m_maxStacks = auraInfo.Stacks;
        if (auraInfo.Stacks > 1)
            this.m_stackable = true;
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
        if (!m_isPeriodic)
            m_effectHit = true;
    }

    private void HandleEffects()
    {
        // Implement logic to apply aura effects to the target
        effectHandler.HandleEffects(this);
    }

    public void Finish()
    {
        CancelEffects();
        this.duration = 0;
        UpdateClient();
        // TODO: Handle OnRemove aurascript
        //auraInfo.AuraScript.OnRemove();
        finished = true;
        // Remove the aura from the target's aura list
        target.DestAura(this);
        // Destroy the aura object
        Destroy(gameObject);
    }

    public void CancelEffects()
    {
        // first we get the effect
        // Since some spells have multiple, we might have to make a list
        List<AuraEffect> auraEffects = new List<AuraEffect>();
        foreach (AuraEffect effect in auraInfo.Effects)
        {
            auraEffects.Add(effect);
        }

        // Now we can just find out which each one does. We have a choice here, we can either handle it here or in Aura.cs
        // To handle it here, it's as simple as:

        foreach (AuraEffect effect in auraEffects)
        {
            if (effect == AuraEffect.AURA_EFFECT_ROOT)
            {
                // So we know the target is being rooted, so we just have to cancel that root for that target
                if (!target)
                    return;

                if (target.HasUnitState(UnitState.UNIT_STATE_ROOTED))
                    target.RemoveUnitState(UnitState.UNIT_STATE_ROOTED);

                // And that's literally it
            }
        }
    }

    public void Refresh()
    {
        // Reset the duration to the original value
        duration = auraInfo.Duration;
        UpdateClient();
    }

    public void AddStack()
    {
        if (m_stacks < m_maxStacks)
            m_stacks++;

        Refresh();
    }

    public void DropStack()
    {
        // drop stacks by 1
        if (m_stacks > 0)
            m_stacks--;
        UpdateClient();
    }

    public void DropStacks()
    {
        // drop stacks
        if (m_stacks > 0)
            m_stacks = 0;
        UpdateClient();
    }

    public Spell ToSpell()
    {
        return spell;
    }

    public void UpdateClient()
    {
        // Ensure Caster is not null before proceeding
        if (caster == null || caster.Identity == null)
        {
            Debug.LogError("Caster or Caster's NetworkIdentity is null. Cannot send spell start packet.");
            return;
        }

        NetworkWriter writer = new NetworkWriter();

        if (auraInfo.Id == 12 && duration == 0)
            print("Potato");

        writer.WriteNetworkIdentity(caster.Identity); // Caster
        writer.WriteNetworkIdentity(target.Identity); // Target
        writer.WriteInt(auraInfo.Id);
        writer.WriteFloat(duration);
        writer.WriteInt(m_stacks);

        OpcodeMessage packet = new OpcodeMessage
        {
            opcode = Opcodes.SMSG_AURA_UPDATE,
            payload = writer.ToArray()
        };

        NetworkServer.SendToAll(packet);
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

                // Custom handle for Agony, stacks will continue up to 10
                if (m_stacks < 10 && auraInfo.Id == 100 /* agony ID placeholder */)
                    AddStack();
            }
        }
        if (duration > 0)
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                DropStacks();
                if (!m_effectHit)
                    HandleEffects();

                m_effectHit = true;
                Finish();
            }
        }
    }
}
