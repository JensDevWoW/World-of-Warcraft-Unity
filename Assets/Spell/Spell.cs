using JetBrains.Annotations;
using Org.BouncyCastle.Security;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using Mirror;
using static UnityEditor.PlayerSettings;
using Org.BouncyCastle.Asn1;
using UnityEditor.UI;
using Unity.VisualScripting;
using System;
using UnityEngine.UIElements;

public class Spell : MonoBehaviour
{
    public const int SPELL_STATE_PREPARING = 1, SPELL_STATE_DELAYED = 2, SPELL_STATE_FINISHED = 3, SPELL_STATE_CASTING = 4, SPELL_STATE_NULL = 5;

    public int spellId;
    public Unit caster, target;
    public bool isPreparing = false;
    public bool isPositive;
    public int m_spellState;
    public SpellInfo m_spellInfo { get; protected set; }
    public int CastFlags;
    public float CastTime;
    public float GCD = 1.5f;
    public bool AnimationEnabled = true;
    public bool IsSpellQueueSpell;
    public Vector3 position;
    public bool VOC;
    public float m_spellTime = 0f;
    public float m_initialSpellTime = 0f;
    public float m_speed;
    public float m_elapsedTime = 0f;
    public int m_minDistanceToTarget = 1;
    public int m_manaCost;
    public int cooldownTime;
    public float cooldownLeft;
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
        this.CastTime = spellInfo.CastTime;
        this.isPositive = spellInfo.Positive;
        this.m_speed = spellInfo.Speed;
        this.m_manaCost = spellInfo.ManaCost;
        this.cooldownTime = spellInfo.Cooldown;
        this.cooldownLeft = spellInfo.Cooldown;
        this.trigger = triggerObject;

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
                    float totalDistance = LocationHandler.Instance.GetDistanceFrom(caster, target);
                    float remainingDistance = totalDistance - distanceTraveled;

                    if (remainingDistance <= m_minDistanceToTarget)
                    {
                        m_spellTime = 0f;
                        OnHit();
                        HandleEffects();
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
                OnHit();
                HandleEffects();
                //SetState(SPELL_STATE_NULL);
                //m_isPreparing = false;
                //return true;
                caster.RemoveSpellFromList(this);
                break;
            case SPELL_STATE_NULL:
                //caster.m_spellList.Remove(this); // Assuming m_spellList is a List<Spell>
                break; // Cast complete, nothing left to do

            default:
                // Handle other possible states or add a default case
                break;
        }
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

    public void OnHit()
    {
        if (spellScript != null)
        {
            spellScript.OnHit(caster, target);
        }
    }

    public void OnCast()
    {
        if (spellScript != null)
        {
            spellScript.OnCast(caster, target);
        }
    }

    private void HandleEffects()
    {
        effectHandler.HandleEffects(this);
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

            string canCast = CheckCast();
            if (canCast != "")
            {
                HandleFailed(canCast);
                return;
            }
            caster.SetCasting();
            m_spellState = SPELL_STATE_PREPARING;
            SendSpellStartPacket();

            //TODO: Add spell flags to check for GCD immunity
            if (caster.ToPlayer() != null)
                caster.ToPlayer().SetOnGCD();

            if (IsInstant())
                Cast();
        }
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

    public string CheckCast()
    {
        if (!caster)
            return "";

        if (caster.cdHandler.IsCooldownActive(spellId))
            return "cooldown";

        if (caster.ToPlayer() != null)
            if (caster.ToPlayer().IsOnGCD())
                return "global cooldown";

        if (caster.IsCasting())
            return "casting";

        if (NeedsTarget() && target == null)
            return "target";

        return "";
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

        // Add the primary target if needed
        if (needsTarget && target != null && target.IsAlive())
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
        writer.WriteFloat(CastTime);
        writer.WriteFloat(GCD);
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
        writer.WriteFloat(CastTime); 
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

        if (HasCooldown())
            caster.cdHandler.StartCooldown(spellId, cooldownTime);
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

        if (HasHitDelay())
            m_spellState = SPELL_STATE_DELAYED;
        else
            m_spellState = SPELL_STATE_FINISHED;

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
                //caster.CancelCast(this);
                return;
            }
            else if(!target.IsAlive())
            {
                HandleFailed("dead_target");
                m_spellState = SPELL_STATE_NULL;
                //caster.CancelCast(this);
                return;
            }
        }

        if (!caster.IsAlive())
        {
            HandleFailed("dead_target");
            m_spellState = SPELL_STATE_NULL;
            //caster.CancelCast(this);
            return;
        }

        SendSpellGo();
        HandleMana();
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

        /*if (HasCharge())
            DropCharge();
        else if (HasCooldown())
        {
            // Toggled spells shouldn't apply cooldown on first cast
            if (Toggled())
                return;

            if (!HasFlag("SPELL_FLAG_OVERRIDE"))
            {
                SetOnCooldown();
            }
        }*/
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
        // Check what 'this' is referring to
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
