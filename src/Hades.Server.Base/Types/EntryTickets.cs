#region

using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Security;

#endregion

namespace Darkages.Types
{
    /// <summary>
    /// 로그인 서버가 게임 서버로 보내며 내준 입장권. 이름만 적어 두던 때는 그 이름을 아는 아무 접속이나 먼저 와서 들어갈 수 있었다
    /// (리뷰 2026-10-08 #1). 로그인한 그 접속이 받은 Id·암호 매개변수를 그대로 가져와야 하고, 한 번만, 유효기간 안에만 들인다.
    /// </summary>
    public static class EntryTickets
    {
        public static readonly TimeSpan Lifetime = TimeSpan.FromSeconds(60);

        private sealed class Ticket
        {
            public int Id;
            public byte Seed;
            public byte[] Salt;
            public DateTime Expires;
        }

        private static readonly Dictionary<string, Ticket> Issued = new Dictionary<string, Ticket>(StringComparer.OrdinalIgnoreCase);

        /// <summary>이름마다 한 장 — 다시 로그인하면 앞의 것은 무효.</summary>
        public static void Issue(string name, int id, SecurityParameters parameters, DateTime now)
        {
            lock (Issued)
            {
                foreach (string old in Issued.Where(pair => pair.Value.Expires <= now).Select(pair => pair.Key).ToList())
                    Issued.Remove(old);

                Issued[name] = new Ticket { Id = id, Seed = parameters.Seed, Salt = parameters.Salt.ToArray(), Expires = now + Lifetime };
            }
        }

        /// <summary>
        /// 모두 맞으면 쓰고 true. 틀린 시도는 입장권을 지우지 않는다 — 남의 엉터리 시도가 주인의 입장을 막지 못하게.
        /// </summary>
        public static bool TryConsume(string name, int id, SecurityParameters parameters, DateTime now)
        {
            if (string.IsNullOrEmpty(name) || parameters?.Salt == null)
                return false;

            lock (Issued)
            {
                if (!Issued.TryGetValue(name, out Ticket ticket))
                    return false;

                if (ticket.Expires <= now)
                {
                    Issued.Remove(name);
                    return false;
                }

                if (ticket.Id != id || ticket.Seed != parameters.Seed || !ticket.Salt.SequenceEqual(parameters.Salt))
                    return false;

                Issued.Remove(name);
                return true;
            }
        }

        public static void Revoke(string name)
        {
            lock (Issued)
                Issued.Remove(name ?? string.Empty);
        }
    }
}
