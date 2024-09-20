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
using UnityEngine;

public class Player : MonoBehaviour
{
    public string playerName;
    public int level;
    public Duel duelObj = null;
    private float gcdTimer = 0;

    public Unit unit { get; set; }

    public void Start()
    {
        unit = gameObject.GetComponent<Unit>();
    }

    public bool IsDueling(Player player)
    {
        if (player == null)
            return false;

        Duel duel = ToActiveDuel();

        if (duel != null && duel.GetPlayer(player) && duel.HasStarted())
            return true;

        return false;
    }

    public void StopCasting()
    {
        unit.StopCasting(); // Optionally call the base class implementation
        Debug.Log($"{playerName} has stopped casting.");
        // Add any additional logic specific to the Player class here
    }

    public Unit ToUnit()
    {
        return unit;
    }

    public Duel ToActiveDuel()
    {
        return duelObj;
    }

    public void AssignDuel(Duel duel)
    {
        duelObj = duel;
    }

    public void RequestDuel(Unit target)
    {

        this.duelObj = DuelHandler.Instance.CreateDuel(ToUnit(), target);
        target.ToPlayer().AssignDuel(this.duelObj);

        NetworkWriter writer = new NetworkWriter();
        writer.WriteNetworkIdentity(ToUnit().Identity);
        writer.WriteNetworkIdentity(target.Identity);

        // Send duel request to the server
        NetworkServer.SendToAll(new OpcodeMessage
        {
            opcode = Opcodes.SMSG_DUEL_REQUEST,
            payload = writer.ToArray()
        });
    }


    public bool IsOnGCD()
    {
        return gcdTimer > 0;
    }

    public void SetOnGCD(float timer)
    {
        gcdTimer = timer;
    }

    public float GetGCDTime()
    {
        return gcdTimer;
    }
    public void Update()
    {
        // Only players have GCD so update in Player object
        // TODO: Implement haste increase
        if (gcdTimer > 0)
            gcdTimer -= Time.deltaTime;
        else
            gcdTimer = 0;
    }
}
