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
public class Spell
{
    public int spellId;
    public Unit caster, target;
    public bool isPreparing = false;
    public bool isPositive = true;
    public SpellState m_spellState;
    public SpellInfo m_spellInfo { get; protected set; }
    public int CastFlags;
    public float CastTime;
    public float GCD = 1.5f;
    public bool AnimationEnabled = true;
    public bool IsSpellQueueSpell;
    public Vector3 AoEPosition;
    public bool VOC;
    public float SpellTime = 0f;
    public Spell(int spellId, Unit caster, Unit target, SpellInfo spellInfo)
    {
        this.spellId = spellId;
        this.caster = caster;
        this.target = target;
        this.m_spellInfo = spellInfo;
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
            m_spellState = SpellState.Preparing;
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
        writer.WriteFloat(SpellTime); // Assuming SpellTime is another property in your Spell class
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
