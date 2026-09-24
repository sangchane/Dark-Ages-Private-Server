#region

using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Common;
using Darkages.Network.Object;

#endregion

namespace Darkages.Types
{
    public class Party : ObjectManager
    {
        public int Id { get; set; }
        public string LeaderName { get; set; }

        public List<Aisling> PartyMembers => GetObjects<Aisling>(null, sprite => sprite.GroupId == Id)
            .DistinctBy(i => i.Username.ToLower()).ToList();

        public static bool AddPartyMember(Aisling partyLeader, Aisling playerToAdd)
        {
            if (playerToAdd == null)
                return false;

            if (partyLeader.GroupId != 0)
            {
                if (playerToAdd.GroupId != 0 && playerToAdd.GroupId != partyLeader.GroupId)
                {
                    partyLeader.Client.SystemMessage($"{playerToAdd.Username}님은 이미 그룹 중 입니다.");
                    playerToAdd.Client.SystemMessage("이미 그룹 중 입니다.");

                    return false;
                }

                if (playerToAdd.GroupId != 0 || partyLeader.GroupId == 0)
                    return false;

                playerToAdd.GroupId = partyLeader.GroupId;

                partyLeader.Client.SystemMessage($"{playerToAdd.Username}님 그룹에 참여");
                playerToAdd.Client.SystemMessage($"{partyLeader.Username}님의 그룹에 참여");

                return true;
            }

            if (playerToAdd.GroupId != 0 && partyLeader.GroupId == 0)
            {
                // 전에는 두 말이 뒤바뀌어 갔다(그룹에 든 쪽은 청한 이가, 청한 이는 제 객체 이름을 들었다).
                playerToAdd.Client.SystemMessage("이미 그룹 중 입니다.");

                partyLeader.Client.SystemMessage($"{playerToAdd.Username}님은 이미 그룹 중 입니다.");

                return false;
            }

            if (playerToAdd.GroupId != 0 || partyLeader.GroupId != 0)
                return false;

            var party = CreateParty(partyLeader);
            playerToAdd.GroupId = party.Id;

            foreach (var player in party.PartyMembers)
                player.Client.SystemMessage($"{playerToAdd.Username}님 그룹에 참여");

            playerToAdd.Client.SystemMessage($"{partyLeader.Username}님의 그룹에 참여");
            playerToAdd.GroupId = party.Id;

            return true;
        }

        public static Party CreateParty(Aisling partyLeader)
        {
            if (partyLeader == null) throw new ArgumentNullException(nameof(partyLeader));

            if (partyLeader.GroupId != 0)
                return null;

            var party = new Party {LeaderName = partyLeader.Username};

            lock (Generator.Random)
            {
                var pendingId = Generator.GenerateNumber();

                party.Id = pendingId;
                party.LeaderName = partyLeader.Username;
                partyLeader.GroupId = party.Id;

                if (!ServerContext.GlobalGroupCache.ContainsKey(party.Id))
                    ServerContext.GlobalGroupCache.Add(party.Id, party);
            }

            return party;
        }

        public static void DisbandParty(Party group)
        {
            if (!ServerContext.GlobalGroupCache.ContainsKey(group.Id)) return;
            if (!ServerContext.GlobalGroupCache.Remove(group.Id)) return;
            foreach (var player in group.PartyMembers)
            {
                player.GroupId = 0;
                player.Client.SendMessage("그룹 해체");
            }
        }

        public static void RemovePartyMember(Aisling playerToRemove)
        {
            if (ServerContext.GlobalGroupCache.ContainsKey(playerToRemove.GroupId))
            {
                var group = ServerContext.GlobalGroupCache[playerToRemove.GroupId];

                if (group != null)
                {
                    foreach (var player in group.PartyMembers)
                        player.Client.SendMessage($"{playerToRemove.Username}님 그룹 해체");

                    playerToRemove.GroupId = 0;

                    if (group.PartyMembers.Count <= 1)
                    {
                        DisbandParty(group);
                    }
                    else
                    {
                        var nextPlayer = group.PartyMembers.FirstOrDefault();

                        if (nextPlayer == null)
                            return;

                        group.LeaderName = nextPlayer.Username;

                        foreach (var player in group.PartyMembers)
                            player.Client.SendMessage($"{nextPlayer.Username}님이 그룹장이 되셨습니다");
                    }
                }
            }
        }

        public bool Has(Aisling that)
        {
            return Id == that.GroupId;
        }
    }
}