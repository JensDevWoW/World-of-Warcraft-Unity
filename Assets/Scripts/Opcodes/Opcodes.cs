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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opcodes
{
    public const int CMSG_CAST_SPELL = 0;
    public const int SMSG_SPELL_START = 1;
    public const int SMSG_SPELL_GO = 2;
    public const int SMSG_SEND_COMBAT_TEXT = 3;
    public const int CMSG_SELECT_TARGET = 4;
    public const int SMSG_UPDATE_STAT = 5;
    public const int SMSG_AURA_UPDATE = 6;
    public const int SMSG_CAST_CANCELED = 7;
    public const int SMSG_APPLY_AURA = 8;
    public const int SMSG_UPDATE_TARGET = 9;
    public const int SMSG_SPELL_FAILED = 10;
    public const int SMSG_UPDATE_UNIT_STATE = 11;
    public const int SMSG_INIT_BARS = 12; // TODO: Change to InitCharacter
    public const int CMSG_JOIN_WORLD = 13;
    public const int SMSG_UPDATE_CHARGES = 14;
    public const int SMSG_SPELL_COOLDOWN = 15;
    public const int SMSG_CHANNELED_START = 16;
    public const int SMSG_CHANNELED_UPDATE = 17;
    public const int CMSG_LOGIN_REQUEST = 18;
    public const int SMSG_ACCOUNT_INFO = 19;
    public const int CMSG_UPDATE_POS = 20;
    public const int SMSG_UPDATE_POS = 21;
    public const int CMSG_DUEL_REQUEST = 22;
    public const int CMSG_DUEL_RESPONSE = 23;
    public const int SMSG_DUEL_REQUEST = 23;
    public const int SMSG_REGISTER_UI = 24;
    public const int CMSG_REGISTER_UI = 25;
    public const int SMSG_START_DUEL = 25;
    public const int CMSG_CREATE_CHAR = 26;
}

