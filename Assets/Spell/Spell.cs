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

using JetBrains.Annotations;
using Org.BouncyCastle.Security;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Org.BouncyCastle.Asn1;
using Unity.VisualScripting;
using System;
using UnityEngine.UIElements;

public class Spell : MonoBehaviour
{
    public const int SPELL_STATE_PREPARING = 1, SPELL_STATE_DELAYED = 2, SPELL_STATE_FINISHED = 3, SPELL_STATE_CASTING = 4, SPELL_STATE_NULL = 5, SPELL_STATE_QUEUED = 6;
    public const int SPELL_FAILED_MOVING = 1, SPELL_CAST_OK = 2;
    public int spellId;
    public Unit caster, target;
    public bool isPreparing = false;
    public bool isPositive;
    public int m_spellState;
    public SpellInfo m_spellInfo { get; protected set; }
    public int CastFlags;
    public float GCD;
    public bool AnimationEnabled;
    public bool IsSpellQueueSpell;
    public Vector3 position;
    public bool VOC;
    public float m_spellTime = 0f;
    public float m_initialSpellTime = 0f;
    public float m_speed;
    public float m_elapsedTime = 0f;
    public int m_minDistanceToTarget = 1;
    public int m_manaCost;
    public int m_charges;
    public int m_maxCharges;
    public int cooldownTime;
    public float cooldownLeft;
    public float modBasePoints = 0;
    public float m_channeledDuration;
    public float m_timer;
    public bool effectsHandled = false;
    public GameObject trigger;
    public SpellEffectHandler effectHandler {  get; protected set; }

    private SpellScript spellScript;

    private List<Unit> targetList;
    public void Initialize(int spellId, Unit caster, SpellInfo spellInfo, GameObject triggerObject)
    {
        if (spellInfo == null || caster == null)
        {
            Debug.LogError("SpellInfo or Caster is null during Spell instantiation.");
            return;
        }

        this.spellId = spellId;
        this.caster = caster;
        this.targetList = new List<Unit>();
        this.m_spellInfo = spellInfo;
        this.target = caster.GetTarget();
        this.m_timer = spellInfo.CastTime;
        this.isPositive = spellInfo.Positive;
        this.m_speed = spellInfo.Speed;
        this.m_manaCost = spellInfo.ManaCost;
        this.cooldownTime = spellInfo.Cooldown;
        this.cooldownLeft = spellInfo.Cooldown;
        this.trigger = triggerObject;

        // Stacks
        if (m_spellInfo.Stacks > 1)
        {
            this.m_charges = m_spellInfo.Stacks;
            this.m_maxCharges = m_spellInfo.Stacks;
        }
        else
        {
            this.m_charges = 1;
            this.m_maxCharges = 1;
        }

        this.AnimationEnabled = !spellInfo.HasFlag(SpellFlags.SPELL_FLAG_DISABLE_ANIM);
        this.GCD = HasFlag(SpellFlags.SPELL_FLAG_IGNORES_GCD) ? 0f : 1.5f;
        AttachSpellScript(spellId);

        if (spellInfo.HasFlag(SpellFlags.SPELL_FLAG_NEEDS_TARGET))
        {
            if (spellInfo.SpellTime == true && target != null)
            {
                float distance = LocationHandler.Instance.GetDistanceFrom(caster,target);
                float travelTime = distance / spellInfo.Speed;

                this.m_spellTime = travelTime;
                this.m_initialSpellTime = travelTime;
            }
        }

        // Handle SpellEffects;
        effectHandler = new SpellEffectHandler();
    }

    public float GetModPct()
    {
        return modBasePoints;
    }

    public void SetModPct(float customPct)
    {
        modBasePoints = customPct;
    }
    public int CheckMovement()
    {
        if (IsInstant())
            return SPELL_CAST_OK;

        if (m_spellState == SPELL_STATE_PREPARING)
        {
            if (m_spellInfo.CastTime > 0)
                if (!HasFlag(SpellFlags.SPELL_FLAG_CAST_WHILE_MOVING)) // TODO: Add InterruptFlags
                    return SPELL_FAILED_MOVING;
        }
        else if (m_spellState == SPELL_STATE_CASTING)
            if (HasFlag(SpellFlags.SPELL_FLAG_CAST_WHILE_MOVING))
                return SPELL_FAILED_MOVING;

        return SPELL_FAILED_MOVING;
    }

    public void Update()
    {

        if (m_timer > 0 && caster.IsMoving())
            Cancel();

        switch (m_spellState)
        {
            case SPELL_STATE_QUEUED:
                if (!caster.IsCasting() && caster.GetGCDTime() == 0)
                    prepare();
                break;
            case SPELL_STATE_PREPARING:
                bool shouldCancel =
                    !caster.IsAlive() ||
                    (caster.IsMoving() && !HasFlag(SpellFlags.SPELL_FLAG_CAST_WHILE_MOVING)); /* ||
                    caster.HasState("UNIT_STATE_SILENCED") ||
                    caster.HasState("UNIT_STATE_STUNNED") ||
                    caster.HasState("UNIT_STATE_FEARED");*/

                if (shouldCancel)
                {
                    Cancel();
                    break;
                }

                if (m_timer > 0f)
                {
                    m_timer -= Time.deltaTime;
                }
                else
                {
                    Cast();
                }
                break;
            case SPELL_STATE_CASTING:
                if (m_timer > 0)
                {
                    if (!UpdateChanneledTargetList())
                    {
                        Debug.Log($"Channeled spell: {m_spellInfo.Id} has been removed due to: No targets");
                        m_timer = 0;

                    }

                    m_timer -= Time.deltaTime;
                }
                else 
                {
                    SendChannelUpdate(0);
                    finish(true);
                }
                break;
            case SPELL_STATE_DELAYED:
                Unit target = this.target;

                if (target != null)
                {
                    if (m_initialSpellTime == 0f)
                    {
                        m_initialSpellTime = m_spellTime;
                        m_elapsedTime = 0f;
                    }

                    m_elapsedTime += Time.deltaTime;
                    float distanceTraveled = m_speed * m_elapsedTime;
                    float totalDistance = LocationHandler.Instance.GetDistanceFrom(caster, target);
                    float remainingDistance = totalDistance - distanceTraveled;

                    if (remainingDistance <= m_minDistanceToTarget)
                    {
                        m_spellTime = 0f;
                        OnHit();
                        HandleEffects();
                        m_spellState = SPELL_STATE_NULL;
                        //m_isPreparing = false;
                        return;
                    }

                    float travelTime = remainingDistance / m_speed;
                    m_spellTime = travelTime;
                }

                m_spellTime -= Time.deltaTime;
                break;
            case SPELL_STATE_FINISHED:
                m_spellTime = 0f;
                OnHit();
                HandleEffects();
                m_spellState = SPELL_STATE_NULL;
                //m_isPreparing = false;
                //return true;
                caster.RemoveSpellFromList(this);
                break;
            case SPELL_STATE_NULL:
                //caster.m_spellList.Remove(this); // Assuming m_spellList is a List<Spell>
                break; // Cast complete, nothing left to do

            default:
                break;
        }
    }

    public bool UpdateChanneledTargetList()
    {
        foreach (Unit target in targetList)
        {
            if (target.IsAlive())
                return true; // We know we have at least one target so we can skip the rest
        }

        return false;
    }

    public void HandleMana()
    {
        if (!caster) { return; }

        int manaCost = m_spellInfo.ManaCost;

        if (manaCost == 0 ) { return; }

        if (caster.GetMana() - manaCost > 0)
            caster.SetMana(caster.GetMana() - (float)manaCost);
        else
            caster.SetMana(0);

        print($"{caster.GetMana()} is how much mana he has left.");

    }
    public int GetAmount()
    {
        return m_spellInfo.BasePoints;
    }

    public void ModBasePoints(float amount)
    {
        this.modBasePoints = m_spellInfo.BasePoints * amount;
    }

    public void SendChanneledStart(float duration)
    {
        Unit unitCaster = caster.ToUnit();
        if (unitCaster == null)
            return;

        NetworkWriter writer = new NetworkWriter();

        writer.WriteNetworkIdentity(caster.Identity);
        writer.WriteInt(m_spellInfo.Id);
        writer.WriteFloat(duration);

        OpcodeMessage packet = new OpcodeMessage
        {
            opcode = Opcodes.SMSG_CHANNELED_START,
            payload = writer.ToArray()
        };

        NetworkServer.SendToAll(packet);

        m_timer = duration;

        unitCaster.SetChanneledSpellId(m_spellInfo.Id);
    }

    public void SendChannelUpdate(float time)
    {
        if (time == 0)
            caster.SetChanneledSpellId(0);

        NetworkWriter writer = new NetworkWriter();

        writer.WriteNetworkIdentity(caster.Identity);
        writer.WriteFloat(time);

        OpcodeMessage packet = new OpcodeMessage
        {
            opcode = Opcodes.SMSG_CHANNELED_UPDATE,
            payload = writer.ToArray()
        };

        NetworkServer.SendToAll(packet);
    }

    public void HandleDelayedChannel()
    {
        // Ensure the caster is a player, and the spell is currently channeling
     //   Player playerCaster = caster as Player;
     //   if (playerCaster == null || m_spellState != SPELL_STATE_CASTING)
      //      return;

        // Check if the spell can be delayed
     //   if (!m_spellInfo.ChannelInterruptFlags.HasFlag(ChannelInterruptFlags.DELAY))
     //       return;

        // Check if the spell has already been delayed the maximum allowed number of times
       // if (IsDelayableNoMore())
       //     return;

        // Calculate the initial delay time (25% of the remaining duration)
        float delayTime = m_channeledDuration * 0.25f;

        // Apply player-specific modifiers to the delay time
      //  float delayReduce = playerCaster.GetTotalAuraModifier(AuraType.REDUCE_PUSHBACK); // Example modifier
       // delayTime *= (1f - delayReduce / 100f);

        // Ensure the delay doesn't reduce the channel time below zero
        if (m_timer <= delayTime)
        {
            delayTime = m_timer;
            m_timer = 0;
        }
        else
        {
            m_timer -= delayTime;
        }

        // Update effects for all valid targets affected by the channel
        foreach (Unit target in targetList)
        {
            if (target != null && target.IsAlive())
            {
                // Delay target-specific effects here, if applicable
               // target.DelayAura(m_spellInfo.Id, delayTime); 
            }
        }

        // If there is any area effect associated, adjust its duration as well
       /* if (areaEffect != null)
        {
            areaEffect.Delay(delayTime);
        }*/

        // Send an update to reflect the new channel duration on the client
        SendChannelUpdate(m_timer);
    }



    public void HandleImmediate()
    {
        if (m_spellInfo.IsChanneled())
        {
            float duration = m_spellInfo.GetDuration();
            if (duration > 0)
            {
                m_channeledDuration = duration;
                SendChanneledStart(duration);
            }
            else if (duration == -1) // Infinite cast
                SendChanneledStart(duration);

            if (duration != 0)
                this.m_spellState = SPELL_STATE_CASTING;
        }

        HandleEffects();

        if (this.m_spellState != SPELL_STATE_CASTING)
            finish(true);
    }

    private void finish(bool ok)
    {
        if (m_spellState == SPELL_STATE_FINISHED)
            return;

        m_spellState = SPELL_STATE_FINISHED;

        if (caster == null)
            return;

        if (m_spellInfo.IsChanneled())
            caster.InterruptChanneled();

        if (caster.IsCasting())
            caster.StopCasting();

        if (!ok)
            return;


    }

    public void OnHit()
    {
        if (spellScript != null)
        {
            spellScript.OnHit(this, caster, target);
        }
    }

    public void OnCast()
    {
        if (spellScript != null)
        {
            spellScript.OnCast(this, caster, target);
        }
    }

    public void Modify()
    {
        if (spellScript != null)
        {
            spellScript.Modify(this, caster, target);
        }
    }

    public void SetCastTime(float time)
    {
        this.m_timer = time;
    }

    private void HandleEffects()
    {
        if (!effectsHandled)
            effectHandler.HandleEffects(this);

        effectsHandled = true;
    }
    public void prepare()
    {
        if (isPreparing)
            return;

        // Check for positive spell cast on enemy
        if (NeedsTarget())
            if (target)
            {
                if (caster.IsHostileTo(target))
                    if (isPositive) { target = caster; }
            }
            else
            {// We have no target, check if positive spell, cast on self
                if (isPositive)
                {
                    target = caster;
                }
            }
        
        if (caster)
        {
            isPreparing = true;

            // Load SpellScript Mods
            Modify();

            string canCast = CheckCast();
            if (canCast != "")
            {
                // Only players have spell queue of course
                if (caster.ToPlayer() == null)
                {
                    HandleFailed(canCast);
                    return;
                }

                float queueTime = 0.4f; // TODO: Make this adjustable later client-side
                if (canCast == "global cooldown")
                {
                    //we must first overlap any other queued spell we have
                    foreach (Spell spell in caster.GetSpellList())
                    {
                        if (spell.GetSpellState() == SPELL_STATE_QUEUED)
                            spell.SetSpellState(SPELL_STATE_NULL);
                    }

                    // Check if the time left on our gcd is less than our spell queue time
                    if (caster.GetGCDTime() < queueTime)
                    {
                        m_spellState = SPELL_STATE_QUEUED;
                        isPreparing = false;
                        HandleFailed(canCast);
                        return;
                    }
                }
                else if (canCast == "casting")
                {
                    // same as before, overwrite any queued spell
                    foreach (Spell spell in caster.GetSpellList())
                    {
                        if (spell.GetSpellState() == SPELL_STATE_QUEUED)
                            spell.SetSpellState(SPELL_STATE_NULL);
                    }

                    // Check if the time left on our gcd is less than our spell queue time
                    if (caster.GetCastedSpellTimeLeft() < queueTime)
                    {
                        m_spellState = SPELL_STATE_QUEUED;
                        isPreparing = false;
                        HandleFailed(canCast);
                        return;
                    }
                }

                HandleFailed(canCast);
                return;
            }

            caster.SetCasting();
            m_spellState = SPELL_STATE_PREPARING;
            SendSpellStartPacket();

            if (caster.ToPlayer() != null && (!HasFlag(SpellFlags.SPELL_FLAG_IGNORES_GCD)))
                caster.ToPlayer().SetOnGCD(GetGCD());

            if (IsInstant())
                Cast();
        }
    }

    public float GetCastTimeLeft()
    {
        return m_timer;
    }

    public void SetSpellState(int state)
    {
        m_spellState = state;
    }

    public bool NeedsTarget()
    {
        return HasFlag(SpellFlags.SPELL_FLAG_NEEDS_TARGET);
    }

    public bool HasFlag(SpellFlags flag)
    {
        return m_spellInfo.HasFlag(flag);
    }

    private void HandleFailed(string reason)
    {
        NetworkWriter writer = new NetworkWriter();

        writer.WriteNetworkIdentity(caster.Identity);
        writer.WriteInt(m_spellInfo.Id);
        writer.WriteString(reason);

        OpcodeMessage packet = new OpcodeMessage
        {
            opcode = Opcodes.SMSG_SPELL_FAILED,
            payload = writer.ToArray()
        };

        NetworkServer.SendToAll(packet);

        isPreparing = false;

        Debug.LogError($"{reason} is why you can't cast, idiot!");
    }
    private void AttachSpellScript(int spellId)
    {
        // Get the script type from the registry using the spellId
        Type scriptType = SpellScriptRegistry.GetSpellScriptType(spellId);

        if (scriptType != null)
        {
            // Attach the script as a component
            spellScript = gameObject.AddComponent(scriptType) as SpellScript;
            if (spellScript != null)
            {
                Debug.Log($"Attached script {scriptType.Name} to {gameObject.name}");
            }
            else
            {
                Debug.LogError($"Failed to attach script for spell ID {spellId}.");
            }
        }
    }

    public float GetGCD()
    {
        // We need to eventually add in a better way to get this data, but this will work for now
        float gcdTime = 1.5f;

        if (m_spellInfo != null)
        {
            switch (m_spellInfo.Id)
            {
                case 20: // Fire Blast
                    gcdTime = 0.75f;
                    break;
            }

            if (HasFlag(SpellFlags.SPELL_FLAG_IGNORES_GCD))
                return 0f;
        }

        return gcdTime;
    }

    public string CheckCast()
    {
        if (!caster)
            return "";

        if (!HasCharges() && caster.IsCooldownActive(spellId) || (HasCharges() && GetCharges() < 1))
            return "cooldown";

        if (caster.ToPlayer() != null)
            if (caster.ToPlayer().IsOnGCD())
                return "global cooldown";

        if (caster.IsCasting() && !HasFlag(SpellFlags.SPELL_FLAG_CAST_WHILE_CASTING))
            return "casting";

        if (NeedsTarget() && target == null)
            return "target";

        if (caster.IsMoving() && !HasFlag(SpellFlags.SPELL_FLAG_CAST_WHILE_MOVING))
            return "moving";

        return "";
    }

    public void CancelEffects()
    {
        if (m_timer > 0)
        {
            // remove the aura from the target
            foreach (Unit target in targetList)
            {
                if (target.HasAuraFrom(this))
                    target.RemoveAura(m_spellInfo.Id);
            }

            m_timer = 0;
        }
    }

    public void Cancel()
    {
        if (m_spellState == SPELL_STATE_FINISHED)
            return;

        int m_oldState = m_spellState;
        m_spellState = SPELL_STATE_NULL;

        switch (m_oldState)
        {
            case SPELL_STATE_PREPARING:
            case SPELL_STATE_DELAYED:
                CancelGlobalCooldown();

                break;
            case SPELL_STATE_CASTING:
                CancelEffects();
                break;
        }

        HandleFailed("cancelled");
    }

    public void CancelGlobalCooldown()
    {
        return;
        /*if (!CanHaveGlobalCooddown(caster))
            return;

        if (caster.ToUnit().GetCurrentSpell() != this)
            return;

        if (caster.ToPlayer())
        {
            caster.ToPlayer().GetGlobalCooldownMgr().CancelGlobalCooldown(m_spellInfo);
        }*/
    }

    public bool HasCharges()
    {
        return m_spellInfo.Stacks > 1;
    }

    public int GetCharges()
    {
        return caster.GetCharges(m_spellInfo.Id);
    }

    public void DropCharge()
    {
        caster.DropCharge(m_spellInfo.Id);
    }

    public void AddCharge()
    {
        caster.AddCharge(m_spellInfo.Id);
    }

    public void SendUpdateCharges()
    {
        NetworkWriter writer = new NetworkWriter();

        writer.WriteNetworkIdentity(caster.Identity);
        writer.WriteInt(m_spellInfo.Id);
        writer.WriteInt(m_charges);

        OpcodeMessage packet = new OpcodeMessage
        {
            opcode = Opcodes.SMSG_UPDATE_CHARGES,
            payload = writer.ToArray()
        };

        NetworkServer.SendToAll(packet);
    }

    public int GetSpellState()
    {
        return m_spellState;
    }

    public List<Unit> GetTargets()
    {
        return targetList;
    }

    public Unit GetTarget()
    {
        if (targetList.Count == 1)
            return targetList[0];
        else
            return null;
    }

    public List<Unit> SelectSpellTargets()
    {
        bool needsTarget = NeedsTarget();
        bool posNeg = isPositive;

        if (HasFlag(SpellFlags.SPELL_FLAG_SELF_TARGET)) // Self-applied requirement
        {
            targetList.Clear();
            targetList.Add(caster);
            return targetList;
        }

        // Add the primary target if needed
        if (needsTarget && target != null && target.IsAlive() && target != caster)
        {
            targetList.Add(target);
        }

        // Add the caster if the spell is positive (buff or self-cast)
        if (posNeg)
        {
            targetList.Add(caster);
        }

        // Check for AoE, mouse targeting, and cone targeting flags
        bool hasAoE = HasFlag(SpellFlags.SPELL_FLAG_AOE);
        bool hasMouse = HasFlag(SpellFlags.SPELL_FLAG_MOUSE);
        bool hasCone = HasFlag(SpellFlags.SPELL_FLAG_CONE);
        Vector3 pos = position;
        int range = m_spellInfo.Range;
        LocationHandler LH = LocationHandler.Instance;
        if (hasAoE)
        {
            if (hasMouse)
            {
                // Get enemies near the specified position (mouse-click position)
                targetList = LH.GetNearestEnemiesFromPosition(caster, pos, range);
            }
            else if (needsTarget)
            {
                // Get enemies near the primary target
                targetList = LH.GetNearestEnemyUnitsFromUnit(caster, target, range);
            }
            else
            {
                // Get enemies near the caster
                targetList = LH.GetNearestEnemyUnitList(caster, range);
            }
        }
        else if (hasCone)
        {
            // Get enemies in a cone in front of the caster
            targetList = LH.GetEnemiesInCone(caster, 30, range);
        }

        return targetList;
    }
    private void SendSpellStartPacket()
    {
        // Ensure Caster and Target are not null before proceeding
        if (caster == null || caster.Identity == null)
        {
            Debug.LogError("Caster or Caster's NetworkIdentity is null. Cannot send spell start packet.");
            return;
        }

        NetworkWriter writer = new NetworkWriter();

        // Writing data to the NetworkWriter
        writer.WriteInt(CastFlags);
        writer.WriteNetworkIdentity(caster.Identity); // Use Caster's NetworkIdentity
        // TODO: Somehow put an array in this bith
        writer.WriteNetworkIdentity(target != null ? target.Identity : null); // Use Target's NetworkIdentity, if available
        writer.WriteFloat(m_timer);
        writer.WriteFloat(GetGCD());
        writer.WriteInt(spellId);
        writer.WriteBool(AnimationEnabled);
        writer.WriteBool(IsSpellQueueSpell);
        writer.WriteVector3(position);
        writer.WriteBool(VOC);

        OpcodeMessage msg = new OpcodeMessage
        {
            opcode = Opcodes.SMSG_SPELL_START,  // Assuming this opcode is defined
            payload = writer.ToArray()
        };

        NetworkServer.SendToAll(msg);
    }

    private void SendSpellGo()
    {
        // Ensure Caster and Target are not null before proceeding
        if (caster == null || caster.Identity == null)
        {
            Debug.LogError("Caster or Caster's NetworkIdentity is null. Cannot send spell go packet.");
            return;
        }

        NetworkWriter writer = new NetworkWriter();

        writer.WriteInt(0); // Cast Flags
        writer.WriteNetworkIdentity(caster.Identity); 
        writer.WriteNetworkIdentity(target != null ? target.Identity : null); 
        writer.WriteFloat(m_timer);
        writer.WriteFloat(cooldownTime);
        writer.WriteInt(spellId);
        writer.WriteFloat(m_speed);
        writer.WriteFloat(m_spellTime); 
        writer.WriteBool(AnimationEnabled); 
        writer.WriteVector3(position);
        writer.WriteInt(m_manaCost);

        // Determine the VOC value based on spell conditions
        bool voc = false;
        if (spellId == 48) // Ring of Frost
        {
            voc = true; 
        }
        writer.WriteBool(voc); 

        writer.WriteBool(false); // IsToggled flag

        OpcodeMessage msg = new OpcodeMessage
        {
            opcode = Opcodes.SMSG_SPELL_GO,
            payload = writer.ToArray()
        };

        // Send the message to all clients
        NetworkServer.SendToAll(msg);
    }

    private void SetOnCooldown()
    {
        if (!caster)
            return;

        if (caster.IsOnCooldown(m_spellInfo.Id))
            return;

        if (HasCooldown())
            caster.StartCooldown(spellId);
    }

    private bool IsOnCooldown()
    {
        if (!caster)
            return false;

        if (HasCooldown())
            return caster.IsCooldownActive(spellId);

        return false;
    }

    private bool HasCooldown()
    {
        return cooldownTime > 0;
    }

    private void Cast()
    {
        if (!caster)
            { return; }
        SelectSpellTargets();

        Execute();
        OnCast();
    }

    public bool IsNegative()
    {
        return !m_spellInfo.Positive;
    }

    private void Execute()
    {
        if (!caster)
            { return; }

        if (NeedsTarget())
        {
            if (!target) // || !caster.IsWithinLOS(target))
            {
                HandleFailed("target");
                m_spellState = SPELL_STATE_NULL;
                caster.CancelCast(this);
                return;
            }
            else if(!target.IsAlive())
            {
                HandleFailed("dead_target");
                m_spellState = SPELL_STATE_NULL;
                caster.CancelCast(this);
                return;
            }
        }

        if (!caster.IsAlive())
        {
            HandleFailed("dead_target");
            m_spellState = SPELL_STATE_NULL;
            caster.CancelCast(this);
            return;
        }

        SendSpellGo();
        HandleMana();

        if (HasHitDelay() && !m_spellInfo.IsChanneled())
        {
            m_spellState = SPELL_STATE_DELAYED;

            if (caster.HasUnitState(UnitState.UNIT_STATE_CASTING))
                caster.RemoveUnitState(UnitState.UNIT_STATE_CASTING);
        }
        else
            HandleImmediate();

        if (IsNegative())
        {
            foreach (Unit target in targetList)
            {
                if (target != null)
                {
                    caster.SetInCombatWith(target);
                }
            }
        }
            

        SetOnCooldown();

        if (GetCharges() > 0)
            DropCharge();
       // else if (HasCooldown())
        //{
            // Toggled spells shouldn't apply cooldown on first cast
            //if (Toggled())
                //return;

            //if (!HasFlag("SPELL_FLAG_OVERRIDE"))
           // {
            //    SetOnCooldown();
           // }
       // }
        
    }
    private bool IsInstant()
    {
        return m_spellInfo.CastTime == 0;
    }


    private bool HasHitDelay()
    {
        return m_spellInfo.SpellTime;
    }

    

    private int GetFlags()
    {
        // Implement this based on your spell flags logic
        return 0; // Placeholder
    }

}
