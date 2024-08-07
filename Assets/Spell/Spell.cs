using JetBrains.Annotations;
using Org.BouncyCastle.Security;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using SpellFlags;
using Mirror;
using static UnityEditor.PlayerSettings;
using Org.BouncyCastle.Asn1;
using UnityEditor.UI;
using Unity.VisualScripting;
using System;
public class Spell : SpellManager
{
    public int spellId;
    public Unit caster, target;
    public bool isPreparing = false;
    public bool isPositive = true;
    public int m_spellState;
    public SpellInfo m_spellInfo { get; protected set; }
    public int CastFlags;
    public float CastTime;
    public float GCD = 1.5f;
    public bool AnimationEnabled = true;
    public bool IsSpellQueueSpell;
    public Vector3 AoEPosition;
    public bool VOC;
    public float m_spellTime = 0f;
    public float m_initialSpellTime = 0f;
    public float m_speed;
    public float m_elapsedTime = 0f;
    public int m_minDistanceToTarget = 3;
    public Spell(int spellId, Unit caster, Unit target, SpellInfo spellInfo)
    {
        this.spellId = spellId;
        this.caster = caster;
        this.target = target;
        this.m_spellInfo = spellInfo;
        SpellManager.Instance.AddSpell(this);

        this.CastTime = spellInfo.CastTime;

        if (spellInfo.HasFlag("SPELL_FLAG_NEEDS_TARGET"))
        {
            if (spellInfo.SpellTime == true)
            {
                if (target)
                {
                    float distance = caster.ToLocation().GetDistanceFrom(target);
                    float travelTime = distance / spellInfo.Speed;

                    this.m_spellTime = travelTime;
                    this.m_initialSpellTime = travelTime;
                }
            }
        }
    }

    public void Update()
    {
        switch (m_spellState)
        {
            case SPELL_STATE_PREPARING:
                bool shouldCancel =
                    !caster.IsAlive(); /* ||
                    (caster.IsMoving() && !HasFlag("SPELL_FLAG_CAST_WHILE_MOVING")) ||
                    caster.HasState("UNIT_STATE_SILENCED") ||
                    caster.HasState("UNIT_STATE_STUNNED") ||
                    caster.HasState("UNIT_STATE_FEARED");*/

                if (shouldCancel)
                {
                    Cancel();
                    break;
                }

                if (CastTime > 0f)
                {
                    CastTime -= Time.deltaTime;
                }
                else
                {
                    print("Got here!");
                    Cast();
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
                    float totalDistance = caster.ToLocation().GetDistanceFrom(target);
                    float remainingDistance = totalDistance - distanceTraveled;

                    if (remainingDistance <= m_minDistanceToTarget)
                    {
                        m_spellTime = 0f;
                        //OnHit();
                        //HandleEffects();
                        //SetState(SPELL_STATE_NULL);
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
                //OnHit();
                //HandleEffects();
                //SetState(SPELL_STATE_NULL);
                //m_isPreparing = false;
                //return true;
                break;
            case SPELL_STATE_NULL:
                //caster.m_spellList.Remove(this); // Assuming m_spellList is a List<Spell>
                break; // Cast complete, nothing left to do

            default:
                // Handle other possible states or add a default case
                break;
        }



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
            else // We have no target, check if positive spell, cast on self
                if (isPositive) { target = caster; }

        if (caster && target)
        {
            isPreparing = true;

            string canCast = CheckCast();
            if (canCast != "")
                Debug.Log("OOPS!");
            caster.SetCasting();
            m_spellState = SPELL_STATE_PREPARING;
            SendSpellStartPacket();
        }
    }

    public bool NeedsTarget()
    {
        return true;
    }

    public string CheckCast()
    {
        return "";
    }

    public void Cancel()
    {
        if (m_spellState == SPELL_STATE_FINISHED)
            return;

        int m_oldState = m_spellState;
        m_spellState = SPELL_STATE_FINISHED;

        switch (m_oldState)
        {
            case SPELL_STATE_PREPARING:
            case SPELL_STATE_DELAYED:
                CancelGlobalCooldown();

                break;
            case SPELL_STATE_CASTING:
                break;
        }
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

    public int GetSpellState()
    {
        return m_spellState;
    }

    [Server]  // Ensure this runs only on the server
    private void SendSpellStartPacket()
    {
        // Ensure Caster and Target are not null before proceeding
        if (caster == null || caster.Identity == null)
        {
            Debug.LogError("Caster or Caster's NetworkIdentity is null. Cannot send spell start packet.");
            return;
        }

        if (target == null || target.Identity == null)
        {
            Debug.LogWarning("Target or Target's NetworkIdentity is null. Sending spell start packet without a target.");
        }

        NetworkWriter writer = new NetworkWriter();

        // Writing data to the NetworkWriter
        writer.WriteInt(CastFlags);
        writer.WriteNetworkIdentity(caster.Identity); // Use Caster's NetworkIdentity
        writer.WriteNetworkIdentity(target != null ? target.Identity : null); // Use Target's NetworkIdentity, if available
        writer.WriteFloat(CastTime);
        writer.WriteFloat(GCD);
        writer.WriteInt(spellId);
        writer.WriteBool(AnimationEnabled);
        writer.WriteBool(IsSpellQueueSpell);
        writer.WriteVector3(AoEPosition);
        writer.WriteBool(VOC);

        OpcodeMessage msg = new OpcodeMessage
        {
            opcode = Opcodes.SMSG_SPELL_START,  // Assuming this opcode is defined
            payload = writer.ToArray()
        };

        NetworkServer.SendToAll(msg);
    }

    [Server] // Ensure this runs only on the server
    private void SendSpellGo()
    {
        // Ensure Caster and Target are not null before proceeding
        if (caster == null || caster.Identity == null)
        {
            Debug.LogError("Caster or Caster's NetworkIdentity is null. Cannot send spell go packet.");
            return;
        }

        if (target == null || target.Identity == null)
        {
            Debug.LogWarning("Target or Target's NetworkIdentity is null. Sending spell go packet without a target.");
        }

        NetworkWriter writer = new NetworkWriter();

        // Write the data to the NetworkWriter
        writer.WriteInt(0); // CastFlags, assuming 0 as in the original script
        writer.WriteNetworkIdentity(caster.Identity); // Caster's NetworkIdentity
        writer.WriteNetworkIdentity(target != null ? target.Identity : null); // Target's NetworkIdentity
        writer.WriteFloat(CastTime); // CastTime from the spell
        writer.WriteInt(spellId); // Spell ID
        writer.WriteFloat(m_spellTime); // Assuming SpellTime is another property in your Spell class
        writer.WriteBool(AnimationEnabled); // Animation enabled flag
        writer.WriteVector3(AoEPosition); // AoE position
        writer.WriteInt(0); // Assuming ManaCost is an integer or float property in your Spell class
        writer.WriteBool(false); // VOC, set to false unless specific conditions are met

        // Determine the VOC value based on spell conditions
        bool voc = false;
        if (spellId == 48) // Assuming 48 is the spell ID for Ring of Frost
        {
            voc = true; // Set VOC to true for this specific spell
        }
        writer.WriteBool(voc); // Write the VOC value

        writer.WriteBool(false); // IsToggled flag

        OpcodeMessage msg = new OpcodeMessage
        {
            opcode = Opcodes.SMSG_SPELL_GO,  // Assuming this opcode is defined
            payload = writer.ToArray()
        };

        // Send the message to all clients
        NetworkServer.SendToAll(msg);
    }

    private void Cast()
    {
        if (!caster)
            { return; }
        //SelectSpellTargets();

        //if (HasHitDelay())
        //m_spellState = SPELL_STATE_DELAYED;
        //else
        //m_spellState = SPELL_STATE_FINISHED;
        Execute();
        OnCast();
    }

    private void Execute()
    {
        if (!caster)
            { return; }

        if ((NeedsTarget() && !target)) // || !caster.IsWithinLOS(target))
        {
            //HandleFailed("target");
            //m_spellState = SPELL_STATE_FAILED;
            //caster.CancelCast(this);
            //return;
        }

        if (!target.IsAlive() || !caster.IsAlive())
        {
            //HandleFailed("dead_target");
            //m_spellState = SPELL_STATE_FAILED;
            //caster.CancelCast(this);
            //return;
        }

        SendSpellGo();
        //HandleMana();
        //if (IsNegative())
        //    caster.SetInCombatWith(target);

        /*--Consume a charge if you have charges and set on cooldown that way so we can handle it already being on cooldown

            if self:HasCharges() then
                self:DropCharge(); --Drop 1 charge;

            else
                if self:HasCooldown() then
                    -- Toggled spells shouldn't apply cooldown on first cast

                    if self.spell:findFirstChild("Toggle") then

                        return;
        end
        -- Overridden spells need to go on cooldown manually

                    if not self: HasFlag("SPELL_FLAG_OVERRIDE") then
                        self:SetOnCooldown();
        end*/


    }

    private void OnCast()
    {

    }
    private bool IsInstant()
    {
        return true;
    }

    private bool HasHitDelay()
    {
        return false;
    }

    

    private int GetFlags()
    {
        // Implement this based on your spell flags logic
        return 0; // Placeholder
    }

}
