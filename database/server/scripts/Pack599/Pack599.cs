using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Darkages.Network.Object;
using Darkages.Network.ServerFormats;
using Darkages.Storage.locales.Buffs;
using Darkages.Storage.locales.debuffs;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 5.99 스크립트의 값 하나. 스크립트 언어는 수와 글자를 가리지 않으므로 둘 다 담는다.
    /// 비교는 1/0 을 돌려주고, <c>if</c> 는 0 이 아니면 참으로 본다 — 5.99 가 그렇게 쓴다.
    /// </summary>
    public readonly struct V
    {
        private readonly long _n;
        private readonly string _s;

        private V(long n, string s)
        {
            _n = n;
            _s = s;
        }

        public long Num => _s == null ? _n : long.TryParse(_s, out var x) ? x : 0;
        public bool IsText => _s != null;
        public bool Truth => _s == null ? _n != 0 : _s.Length > 0;

        public static implicit operator V(long n) => new V(n, null);
        public static implicit operator V(int n) => new V(n, null);
        public static implicit operator V(string s) => new V(0, s ?? "");
        public static explicit operator long(V v) => v.Num;

        public override string ToString() => _s ?? _n.ToString();
        public override bool Equals(object obj) => obj is V v && (this == v).Truth;
        public override int GetHashCode() => ToString().GetHashCode();

        public static V operator +(V a, V b) => a.IsText || b.IsText ? a.ToString() + b : a._n + b._n;
        public static V operator -(V a, V b) => a.Num - b.Num;
        public static V operator *(V a, V b) => a.Num * b.Num;
        public static V operator /(V a, V b) => b.Num == 0 ? 0 : a.Num / b.Num;
        public static V operator %(V a, V b) => b.Num == 0 ? 0 : a.Num % b.Num;
        public static V operator -(V a) => -a.Num;
        public static V operator !(V a) => a.Truth ? 0 : 1;

        public static V operator ==(V a, V b) =>
            (a.IsText || b.IsText ? a.ToString() == b.ToString() : a._n == b._n) ? 1 : 0;

        public static V operator !=(V a, V b) => (a == b).Truth ? 0 : 1;
        public static V operator <(V a, V b) => a.Num < b.Num ? 1 : 0;
        public static V operator >(V a, V b) => a.Num > b.Num ? 1 : 0;
        public static V operator <=(V a, V b) => a.Num <= b.Num ? 1 : 0;
        public static V operator >=(V a, V b) => a.Num >= b.Num ? 1 : 0;
        public static V operator &(V a, V b) => a.Truth && b.Truth ? 1 : 0;
        public static V operator |(V a, V b) => a.Truth || b.Truth ? 1 : 0;
        public static bool operator true(V a) => a.Truth;
        public static bool operator false(V a) => !a.Truth;

        /// <summary>조건 자리. 변환기가 <c>if</c>·<c>for</c>·<c>&amp;&amp;</c> 에 쓴다.</summary>
        public static bool T(V a) => a.Truth;

        public static V B(bool b) => b ? 1 : 0;
    }

    /// <summary>
    /// 5.99 스크립트 명령을 하데스 동작으로 옮기는 통역. `scripts/build-pack-abilities.py` 가 5.99 의
    /// <c>SKILL_이름</c>·<c>SPELL_이름</c> 블록을 C# 으로 옮기고, 그 안의 명령은 전부 <see cref="Call" /> 로 온다.
    ///
    /// **아직 옮기지 않은 명령은 서버를 멈추지 않는다** — 한 번 로그를 남기고 0 을 돌려준다. 그래서 명령을
    /// 채우는 만큼 기술이 살아난다. 무엇이 비었는지는 생성기가 센다.
    ///
    /// 오브젝트는 5.99 처럼 번호(serial)로 오간다. <c>istype</c> 은 괴물 1 · NPC 2 · 사람 3.
    /// </summary>
    public sealed class Pack599
    {
        private static readonly ConcurrentDictionary<string, byte> Missing = new ConcurrentDictionary<string, byte>();
        private static readonly Random Dice = new Random();

        /// <summary>
        /// 5.99 가 캐릭터 한 칸에 적어 두는 상태(에나르마·집중·호르라마 …). 하데스에 같은 칸이 없어 여기 둔다.
        /// 값은 끝나는 때다.
        /// </summary>
        private static readonly ConcurrentDictionary<(int, string), DateTime> States =
            new ConcurrentDictionary<(int, string), DateTime>();

        private const ObjectManager.Get Living =
            ObjectManager.Get.Monsters | ObjectManager.Get.Aislings | ObjectManager.Get.Mundanes;

        private readonly Aisling _me;
        private readonly Sprite _chosen;

        public Pack599(Sprite sprite, Sprite chosen)
        {
            _me = sprite as Aisling;
            _chosen = chosen;
        }

        public bool Ready => _me != null && !_me.Dead;

        /// <summary>하데스의 기술 수련. 5.99 에는 없지만 기술 레벨이 오르는 길이 이것뿐이다.</summary>
        public void Train(Skill skill)
        {
            if (skill.Level < skill.Template.MaxLevel)
                _me.Client.TrainSkill(skill);
        }

        public V Call(string name, params V[] a)
        {
            switch (name)
            {
                // ── 누구·어디 ─────────────────────────────────────────────
                case "get_myid": return _me.Serial;
                case "skill_target": return Front<Monster>(_me)?.Serial ?? 0;
                case "get_front_char": return Front<Aisling>(Who(a, 0))?.Serial ?? 0;
                case "spell_target": return _chosen?.Serial ?? 0;
                case "istype": return Kind(Find(a, 0));
                case "get_mobxy": return At<Monster>(Arg(a, 0), Arg(a, 1))?.Serial ?? 0;
                case "get_char_serial": return At<Aisling>(Arg(a, 1), Arg(a, 2))?.Serial ?? 0;
                case "get_xs": return Who(a, 0).XPos;
                case "get_ys": return Who(a, 0).YPos;
                case "get_mobxs": return Find(a, 0)?.XPos ?? 0;
                case "get_mobys": return Find(a, 0)?.YPos ?? 0;
                case "get_side": return Who(a, 0).Direction;
                case "get_mapxs": return _me.Map.Cols;
                case "get_mapys": return _me.Map.Rows;
                case "get_map_pk": return _me.Map.Flags.HasFlag(MapFlags.PlayerKill) ? 1 : 0;
                case "get_mapname":
                case "get_mapname1": return _me.Map.Name ?? "";
                case "get_xy_block":
                    return _me.Map.IsWall((int) Arg(a, 1), (int) Arg(a, 2)) ||
                           At<Sprite>(Arg(a, 1), Arg(a, 2)) != null ? 1 : 0;
                case "set_xs": return Move(Arg(a, 0), _me.YPos);
                case "set_ys": return Move(_me.XPos, Arg(a, 0));
                case "set_side":
                    _me.Direction = (byte) (Arg(a, 0) % 4);
                    _me.Client.Refresh();
                    return 0;

                // ── 능력치 ────────────────────────────────────────────────
                case "get_name": return Who(a, 0) is Aisling named ? named.Username ?? "" : (Who(a, 0) as Monster)?.Template?.Name ?? "";
                case "get_class": return (long) ((Who(a, 0) as Aisling)?.Path ?? 0);
                case "get_level": return (Who(a, 0) as Aisling)?.ExpLevel ?? 0;
                case "get_str": return Who(a, 0).Str;
                case "get_int": return Who(a, 0).Int;
                case "get_wis": return Who(a, 0).Wis;
                case "get_con": return Who(a, 0).Con;
                case "get_dex": return Who(a, 0).Dex;
                case "get_vita": return Who(a, 0).CurrentHp;
                case "get_basevita":
                case "get_basevita2": return Who(a, 0).MaximumHp;
                case "get_mana": return Who(a, 0).CurrentMp;
                case "get_basemana": return Who(a, 0).MaximumMp;
                case "get_hide": return (Who(a, 0) as Aisling)?.Invisible == true ? 1 : 0;
                case "get_critical": return Who(a, 0) is Aisling lucky && Dice.Next(100) < Critical(lucky) ? 1 : 0;
                case "rand": return Dice.Next((int) Arg(a, 0), (int) Arg(a, 1) + 1);
                case "spell_exist": return _me.SpellBook.Has(a.Length > 0 ? a[0].ToString() : "") ? 1 : 0;
                case "group_exist": return (_me.PartyMembers?.Count ?? 0) > 1 ? 1 : 0;

                case "get_att_damage": return Who(a, 0) is Aisling hitter ? AttackPower(hitter) : 0;
                case "get_mgc_damage": return Who(a, 0) is Aisling caster ? MagicPower(caster) : 0;

                // ── 보이기·들리기 ─────────────────────────────────────────
                case "effect":
                {
                    var target = Find(a, 0) ?? _me;
                    _me.Show(Scope.NearbyAislings, new ServerFormat29((uint) _me.Serial, (uint) target.Serial,
                        (ushort) Arg(a, 1), (ushort) Arg(a, 2), (ushort) Math.Max(1, Arg(a, 3))));
                    return 0;
                }
                case "motion":
                    _me.Show(Scope.NearbyAislings, new ServerFormat1A
                    {
                        Serial = _me.Serial,
                        Number = (byte) Arg(a, 0),
                        Speed = (short) Math.Max(1, Arg(a, 1))
                    });
                    return 0;
                case "game_sound":
                    _me.Show(Scope.NearbyAislings, new ServerFormat19 { Number = (short) Arg(a, 0) });
                    return 0;
                case "message":
                    _me.Client.SendMessage((byte) Arg(a, 0), Text(a, 1));
                    return 0;
                case "message1":
                    (Find(a, 0) as Aisling)?.Client.SendMessage((byte) Arg(a, 1), Text(a, 2));
                    return 0;
                case "group_message":
                    foreach (var member in Party())
                        member.Client.SendMessage((byte) Arg(a, 0), Text(a, 1));
                    return 0;

                // ── 피해·회복·마력 ────────────────────────────────────────
                case "damaged":
                case "char_damaged":
                case "char_damaged2":
                case "char_damaged3":
                {
                    var target = Find(a, 0);
                    if (target == null || target.Serial == _me.Serial || !target.Attackable)
                        return 0;
                    if (target is Aisling && name == "damaged")
                        return 0;
                    target.ApplyDamage(_me, (int) Math.Min(int.MaxValue, Math.Max(0, Arg(a, 1))), (byte) 0);
                    return 0;
                }
                case "set_vital": return SetHealth(_me, Arg(a, 0));
                case "set_vita": return SetHealth(Find(a, 0), Arg(a, 1));
                case "group_hill":
                    foreach (var member in Party())
                    {
                        SetHealth(member, member.CurrentHp + Arg(a, 0));
                        if (Arg(a, 1) > 0)
                            _me.Show(Scope.NearbyAislings, new ServerFormat29((uint) _me.Serial,
                                (uint) member.Serial, 0, (ushort) Arg(a, 1), 100));
                    }
                    return 0;
                case "manal_del": return SetMana(_me, _me.CurrentMp - Arg(a, 0));
                case "set_manal": return SetMana(_me, Arg(a, 0));
                case "set_mana": return SetMana(Find(a, 0), Arg(a, 1));

                // ── 상태 ─────────────────────────────────────────────────
                case "mob_strabismus": return Afflict(Find(a, 0), new debuff_blind(), Arg(a, 1));
                case "mobsor_delay": return Afflict(Find(a, 0), new debuff_frozen(), Arg(a, 1));
                case "mobnar_delay": return Afflict(Find(a, 0), new debuff_sleep(), Arg(a, 1));
                case "hide":
                {
                    var buff = new buff_hide();
                    if ((Find(a, 0) ?? _me).HasBuff(buff.Name))
                        return 0;
                    buff.Timer.Tick = buff.Length - (int) Arg(a, 1);
                    buff.OnApplied(Find(a, 0) ?? _me, buff);
                    return 1;
                }
                // 5.99 의 상태 번호 `magic 번호, 대상, 이름, 초, 세기, 건 사람`. 번호마다 칸이 하나라 한 번에 하나만 걸린다.
                //   1 저주 — 렌토 15 · 바르도 25 · 데프레코 35 · 프라보 45 · 어둠의각인 60 (세기만큼 방어가 나빠진다)
                //   2 수면 — 나르콜룸
                //   5 칸의눈 — 마법 방어 20% 상승(사용자 확인)
                //   6 포효 — 바투처럼 움직이지 못한다(사용자 확인)
                //   7 빙결 — 같은 자리에 사람에겐 `mobsor_delay` 를 쓴다
                //   8 어둠의각인 — 프라보보다 강한 저주, 저주와 칸이 따로다(사용자 확인)
                case "magic":
                    switch (Arg(a, 0))
                    {
                        case 1: return Afflict(Find(a, 1), new Curse(Curse.Slot, Arg(a, 4)), Arg(a, 3));
                        case 2: return Afflict(Find(a, 1), new debuff_sleep(), Arg(a, 3));
                        case 5: return Afflict(Find(a, 1), new MagicGuard(), Arg(a, 3));
                        case 6: return Afflict(Find(a, 1), new debuff_beagsuain(), Arg(a, 3));
                        case 7: return Afflict(Find(a, 1), new debuff_frozen(), Arg(a, 3));
                        case 8: return Afflict(Find(a, 1), new Curse(Curse.Mark, Arg(a, 4)), Arg(a, 3));
                        default: return Unknown(name + " " + Arg(a, 0));
                    }
                case "magic_exist":
                    switch (Arg(a, 0))
                    {
                        case 1: return Find(a, 1)?.HasDebuff(Curse.Slot) == true ? 1 : 0;
                        case 2: return Find(a, 1)?.HasDebuff("sleep") == true ? 1 : 0;
                        case 5: return Find(a, 1)?.HasDebuff(MagicGuard.Slot) == true ? 1 : 0;
                        case 6: return Find(a, 1)?.HasDebuff("beag suain") == true ? 1 : 0;
                        case 7: return Find(a, 1)?.HasDebuff("frozen") == true ? 1 : 0;
                        case 8: return Find(a, 1)?.HasDebuff(Curse.Mark) == true ? 1 : 0;
                        default: return Unknown(name + " " + Arg(a, 0));
                    }

                // 기다림은 템플릿 쿨다운이 맡는다 — 하데스가 스크립트 뒤에 템플릿 값으로 덮어쓴다.
                case "skill_delay":
                    return 0;

                // 표적을 찾은 뒤라 살아 있다. 5.99 는 `if(@target && get_mobdie){end;}` 로 쓴다.
                case "get_mobdie":
                    return 0;

                // 걸기와 확인을 한 명령으로 쓴다: 긴 시간은 걸고(`enare(@target, 150)`), 1·0 은 걸려 있나 본다
                // (`if(enare(@myid,1)) set @dam, @dam + 1500;`). 공격 기술이 그 결과로 피해를 더한다.
                case "enare":
                case "suenare":
                case "focus":
                case "horrama":
                case "hprecovery":
                case "phoenix":
                case "sosusin":
                    return State(Find(a, 0), name, Arg(a, 1));
                // 신의축복 — 에나르마와 같은 칸(0x156)을 파티에 건다. `god_bless 애니메이션, 초`.
                case "god_bless":
                    foreach (var member in Party())
                    {
                        Grant(member, "enare", Arg(a, 1));
                        _me.Show(Scope.NearbyAislings, new ServerFormat29((uint) _me.Serial, (uint) member.Serial,
                            0, (ushort) Arg(a, 0), 100));
                    }
                    return 1;
                // 리베라토 — 걸린 효과를 지운다. 저주 같은 디버프는 남는다(사용자 확인). 5.99 는 에나르마 두 칸을 지운다.
                case "reberato":
                case "reberato2":
                case "mob_reberato":
                {
                    var who = Find(a, 0);
                    if (who == null)
                        return 0;
                    foreach (var key in States.Keys.Where(k => k.Item1 == who.Serial).ToList())
                        States.TryRemove(key, out _);
                    return 1;
                }
                // 코마 — 하데스의 빈사(skulled). 코마디아가 풀어 준다(사용자 확인).
                case "get_coma":
                case "get_state1":
                    return (Find(a, 0) as Aisling)?.Skulled == true ? 1 : 0;
                case "set_coma":
                case "set_state1":
                case "coma_delay":
                    return Arg(a, 1) == 0 ? 0 : Unknown(name + " 켜기");
                case "del_coma":
                    return Find(a, 0)?.RemoveDebuff("skulled") == true ? 1 : 0;
                // 센스·센스몬스터·품뒤져보기 — 앞에 선 대상의 정보를 보여 준다(사용자 확인).
                case "sense_user":
                case "sense_monster":
                case "sense_item":
                    return Sense(name);
                // 몇 초 무적. 하데스 `dion` 이 같은 일을 한다.
                case "immortal":
                {
                    var who = Find(a, 0) ?? _me;
                    var buff = new buff_dion();
                    if (who.HasBuff(buff.Name))
                    {
                        (who as Aisling)?.Client.SendMessage(0x02, "이미 걸려있습니다.");
                        return 0;
                    }
                    buff.Timer.Tick = buff.Length - (int) Arg(a, 1);
                    buff.OnApplied(who, buff);
                    return 1;
                }
                // "사용불가 지역입니다" 검사. 하데스에는 그런 지역이 없다.
                case "get_solo":
                    return 0;
                case "get_pk": return _me.Map.Flags.HasFlag(MapFlags.PlayerKill) ? 1 : 0;
                case "time": return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                // 5.99 는 민첩과 같은 칸(0xA7)을 읽는다.
                case "get_body": return Who(a, 0).Dex;
                // 암살 — 몬스터 등 뒤로 간다.
                case "get_mobside": return Find(a, 0)?.Direction ?? 0;
                case "get_xs1": return Find(a, 0)?.XPos ?? 0;
                case "get_ys1": return Find(a, 0)?.YPos ?? 0;

                // 풀기 — 디소루마(빙결) · 디나르콜리(수면) · 일루메나(실명). `group_` 은 파티 전체, 인자는 애니메이션.
                case "mobsor_end": return Find(a, 0)?.RemoveDebuff("frozen") == true ? 1 : 0;
                case "mobnar_end": return Find(a, 0)?.RemoveDebuff("sleep") == true ? 1 : 0;
                case "set_delstrabismus": return Find(a, 0)?.RemoveDebuff("blind") == true ? 1 : 0;
                case "group_mobsor_end":
                case "group_mobnar_end":
                    foreach (var member in Party())
                    {
                        member.RemoveDebuff(name == "group_mobsor_end" ? "frozen" : "sleep");
                        _me.Show(Scope.NearbyAislings, new ServerFormat29((uint) _me.Serial, (uint) member.Serial,
                            0, (ushort) Arg(a, 0), 100));
                    }
                    return 1;
                // 딜루메니 — 사람에게 거는 실명.
                case "set_strabismus": return Afflict(Find(a, 0), new debuff_blind(), Arg(a, 1));

                // 효과 크기가 엔진 안에 있는 상태들 — 걸고 확인만 한다.
                //   silence 침묵 · defens 완전방어 · dell 델리스펠라스 · sokup_delay 속성강화 · set_rest 휴식(회복 1.5배)
                case "silence":
                    if (Find(a, 0) is Sprite hushed && Has(hushed, "silence"))
                    {
                        _me.Client.SendMessage(0x02, "이미 사일런스가 걸려있습니다.");
                        return 0;
                    }
                    return State(Find(a, 0), name, Arg(a, 1));
                case "defens":
                case "dell":
                case "sokup_delay":
                case "set_rest":
                    return State(Find(a, 0) ?? _me, name, Math.Max(2, Arg(a, 1)));
                // 슈페이아움 — 파티에 건다. `iaum_delay 애니메이션, 초`.
                case "iaum_delay":
                    foreach (var member in Party())
                        Grant(member, name, Arg(a, 1));
                    return 1;

                // 소지품 — 표창던지기가 투척용표창을 쓴다.
                case "item_exist":
                    return (Who(a, 0) as Aisling)?.Inventory?.Items.Values
                        .Where(i => i?.Template?.Name == Text(a, 1)).Sum(i => Math.Max(1, (int) i.Stacks)) ?? 0;
                case "item_del":
                {
                    var left = Math.Max(1, Arg(a, 1));
                    foreach (var item in _me.Inventory.Items.Values.Where(i => i?.Template?.Name == Text(a, 0)).ToList())
                    {
                        var take = (int) Math.Min(left, Math.Max(1, (int) item.Stacks));
                        _me.Inventory.RemoveRange(_me.Client, item, take);
                        left -= take;
                        if (left <= 0)
                            break;
                    }
                    return 1;
                }
                // 적갑옷해체·적무기해체 — 정면 캐릭터의 갑옷·무기를 벗겨 그 사람의 가방으로 보낸다(사용자 확인).
                // 스스로 벗을 때 하데스가 쓰는 길(`RemoveFromExisting`)을 그대로 탄다.
                case "arm_del":
                case "weapon_del":
                {
                    if (!(Find(a, 0) is Aisling bare) || bare.Serial == _me.Serial)
                        return 0;
                    var slot = name == "arm_del" ? ItemSlots.Armor : ItemSlots.Weapon;
                    var worn = bare.EquipmentManager?.Equipment;
                    if (worn == null || !worn.ContainsKey(slot) || worn[slot]?.Item == null)
                        return 0;
                    return bare.EquipmentManager.RemoveFromExisting(slot) ? 1 : 0;
                }

                // 다라밀공 — 말을 한다.
                case "user_say":
                    _me.Client.SendMessage(Scope.NearbyAislings, 0x00, $"{_me.Username}: {Text(a, 1)}");
                    return 0;

                default:
                    return Unknown(name);
            }
        }

        private V Unknown(string name)
        {
            if (Missing.TryAdd(name, 0))
                ServerContext.Logger($"[5.99] 아직 옮기지 않은 명령: {name}");
            return 0;
        }

        private V State(Sprite who, string state, long seconds)
        {
            if (who == null)
                return 0;
            if (seconds <= 1)
                return Has(who, state) ? 1 : 0;
            if (Grant(who, state, seconds))
                return 1;
            (who as Aisling)?.Client.SendMessage(0x02, "이미 걸려있습니다.");
            return 0;
        }

        private static long Arg(V[] a, int i) => i < a.Length ? a[i].Num : 0;
        private static string Text(V[] a, int i) => i < a.Length ? a[i].ToString() : "";

        private Sprite Find(V[] a, int i)
        {
            var serial = Arg(a, i);
            if (serial == 0)
                return null;
            return serial == _me.Serial ? _me : _me.GetObject(_me.Map, s => s.Serial == serial, Living);
        }

        /// <summary>인자가 없으면 나. 5.99 는 <c>get_xs()</c> 와 <c>get_xs(@myid)</c> 를 섞어 쓴다.</summary>
        private Sprite Who(V[] a, int i) => (i < a.Length ? Find(a, i) : null) ?? _me;

        private static long Kind(Sprite s) => s is Monster ? 1 : s is Mundane ? 2 : s is Aisling ? 3 : 0;

        private T At<T>(long x, long y) where T : Sprite =>
            _me.GetObjects(_me.Map, s => s.XPos == x && s.YPos == y && s is T && s.Serial != _me.Serial, Living)
                .OfType<T>().FirstOrDefault();

        private T Front<T>(Sprite who) where T : Sprite
        {
            var (dx, dy) = who.Direction switch { 0 => (0, -1), 1 => (1, 0), 2 => (0, 1), _ => (-1, 0) };
            return _me.GetObjects(_me.Map, s => s.XPos == who.XPos + dx && s.YPos == who.YPos + dy && s is T, Living)
                .OfType<T>().FirstOrDefault(s => s.Serial != who.Serial);
        }

        private IEnumerable<Aisling> Party() =>
            _me.PartyMembers?.Where(m => m != null && m.Map == _me.Map) ?? new[] { _me };

        private V Move(long x, long y)
        {
            if (_me.Map.IsWall((int) x, (int) y))
                return 0;
            _me.XPos = (int) x;
            _me.YPos = (int) y;
            _me.Client.Refresh();
            return 1;
        }

        private static V SetHealth(Sprite who, long value)
        {
            if (who == null)
                return 0;
            who.CurrentHp = (int) Math.Max(1, Math.Min(value, who.MaximumHp));
            (who as Aisling)?.Client.SendStats(StatusFlags.StructB);
            return 1;
        }

        private static V SetMana(Sprite who, long value)
        {
            if (who == null)
                return 0;
            who.CurrentMp = (int) Math.Max(0, Math.Min(value, who.MaximumMp));
            (who as Aisling)?.Client.SendStats(StatusFlags.StructB);
            return 1;
        }

        /// <summary>5.99 저주. 하데스 cradh 넷(방어 +20~+50)과 같은 일을 5.99 의 세기로 한다.</summary>
        public sealed class Curse : debuff_cursed
        {
            public const string Slot = "5.99 저주";
            public const string Mark = "5.99 각인";
            private readonly int _amount;

            public Curse() : this(Slot, 0)
            {
            }

            public Curse(string slot, long amount) : base(slot, 120, 82)
            {
                _amount = (int) amount;
            }

            public override void OnApplied(Sprite affected, Debuff debuff)
            {
                affected.BonusAc += _amount;
                base.OnApplied(affected, debuff);
            }

            public override void OnEnded(Sprite affected, Debuff debuff)
            {
                affected.BonusAc -= _amount;
                base.OnEnded(affected, debuff);
            }
        }

        /// <summary>칸의눈. 마법 방어 20% — 하데스 마법 방어(Mr)는 0~70 백분율이다.</summary>
        public sealed class MagicGuard : Debuff
        {
            public const string Slot = "5.99 칸의눈";

            public MagicGuard()
            {
                Name = Slot;
                Length = 30;
                Icon = 0;
            }

            public override void OnApplied(Sprite affected, Debuff debuff)
            {
                affected.BonusMr = (byte) (affected.BonusMr + 20);
                base.OnApplied(affected, debuff);
            }

            public override void OnEnded(Sprite affected, Debuff debuff)
            {
                affected.BonusMr = (byte) System.Math.Max(0, affected.BonusMr - 20);
                base.OnEnded(affected, debuff);
            }
        }

        /// <summary>
        /// 공격력. 5.99 서버(Novaonline.exe `0x45d4xx`)가 장비·능력치가 바뀔 때마다 캐릭터 칸 0x8C 에 적는 값이다.
        ///
        ///   장비마다 (최소공격력1 + 최대공격력1) / 2 의 합 · 주먹단련(Lev1) 이 있으면 +10 · 힘 × 10
        ///   · 무도가면 레벨 20 이하 +25 · 50 이하 +70 · 80 이하 +210 · 그 위 +260
        ///   · 에나르마 ×1.3 · 수페라에나르마 ×1.6 · 피닉스모드 +30% · 소수신공 +40%
        ///
        /// 5.99 는 포만도가 50 이하면 75%, 25 이하면 절반으로 깎는다 — 하데스에 포만도가 없어 뺐다.
        /// </summary>
        public static long AttackPower(Aisling who)
        {
            long power = Equipped(who).Sum(item => (item.Template.DmgMin + item.Template.DmgMax) / 2);
            if (who.SpellBook.Has("주먹단련(Lev1)"))
                power += 10;
            power += who.Str * 10;
            if (who.Path == Class.Monk)
                power += who.ExpLevel <= 20 ? 25 : who.ExpLevel <= 50 ? 70 : who.ExpLevel <= 80 ? 210 : 260;

            power = Boost(who, power);
            if (Has(who, "phoenix"))
                power += power / 10 * 3;
            if (Has(who, "sosusin"))
                power += power / 10 * 4;
            return power;
        }

        /// <summary>
        /// 마법 공격력. 공격력과 같은 곳(캐릭터 칸 0x98)에서 적는다 — 장비마다 (최소공격력2 + 최대공격력2) / 2 의 합
        /// · 지능 × 10 · 에나르마 ×1.3 · 수페라에나르마 ×1.6. 하데스 장비에는 마법 대미지 칸이 없어 장비분은 0 이다.
        /// </summary>
        public static long MagicPower(Aisling who) => Boost(who, who.Int * 10);

        /// <summary>
        /// 치명타 확률(%). 캐릭터 칸 0x9C — 장비 크리티컬의 합 · 명중률향상(Lev1) +10 · (Lev2) +20, 100 에서 자르고
        /// 민첩 ÷ 11 을 더한다. 하데스 장비에는 크리티컬 칸이 없다.
        /// </summary>
        public static long Critical(Aisling who)
        {
            long chance = 0;
            if (who.SpellBook.Has("명중률향상(Lev1)"))
                chance += 10;
            if (who.SpellBook.Has("명중률향상(Lev2)"))
                chance += 20;
            return Math.Min(100, chance) + who.Dex / 11;
        }

        /// <summary>5.99 상태를 건다. 이미 걸려 있으면 거절한다.</summary>
        public static bool Grant(Sprite who, string state, long seconds)
        {
            var key = (who.Serial, state);
            if (States.TryGetValue(key, out var until) && until > DateTime.UtcNow)
                return false;
            States[key] = DateTime.UtcNow.AddSeconds(seconds);
            return true;
        }

        public static bool Has(Sprite who, string state) =>
            States.TryGetValue((who.Serial, state), out var until) && until > DateTime.UtcNow;

        private static long Boost(Sprite who, long power)
        {
            if (Has(who, "enare"))
                power = power / 10 * 13;
            if (Has(who, "suenare"))
                power = power / 10 * 16;
            return power;
        }

        private static IEnumerable<Item> Equipped(Aisling who) =>
            who.EquipmentManager?.Equipment?.Values.Where(slot => slot?.Item?.Template != null).Select(slot => slot.Item)
            ?? Enumerable.Empty<Item>();

        private V Sense(string kind)
        {
            var target = kind == "sense_monster" ? (Sprite) Front<Monster>(_me) : Front<Aisling>(_me);
            if (target == null)
                return 0;
            if (kind == "sense_item" && target is Aisling pocket)
            {
                var items = pocket.Inventory?.Items.Values.Where(i => i?.Template != null).Select(i => i.Template.Name) ?? new string[0];
                _me.Client.SendMessage(0x02, $"{pocket.Username}: {string.Join(", ", items)}");
                return 1;
            }
            var named = target is Aisling person ? person.Username : (target as Monster)?.Template?.Name;
            var level = target is Aisling leveled ? $" Lv{leveled.ExpLevel}" : "";
            _me.Client.SendMessage(0x02, $"{named}{level} 체력 {target.CurrentHp}/{target.MaximumHp} 마력 {target.CurrentMp}/{target.MaximumMp}");
            return 1;
        }

        /// <summary>하데스 디버프는 길이가 클래스에 박혀 있다. 남은 시간이 `Length - Tick` 이라 Tick 을 당긴다.</summary>
        private static V Afflict(Sprite target, Debuff debuff, long seconds)
        {
            if (target == null || target.HasDebuff(debuff.Name))
                return 0;
            debuff.Timer.Tick = debuff.Length - (int) seconds;
            debuff.OnApplied(target, debuff);
            return 1;
        }
    }
}
