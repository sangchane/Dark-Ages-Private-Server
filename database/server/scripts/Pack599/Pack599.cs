using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Darkages.Network.Object;
using Darkages.Scripting;
using Darkages.Systems;
using Darkages.Templates;
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

        // `a.ToString() + b` 라고 쓰면 C# 이 문자열 더하기가 아니라 이 연산자를 다시 골라(글자가 V 로 바뀐다)
        // 끝없이 불러 서버가 죽는다. 둘 다 글자로 바꿔 더한다.
        public static V operator +(V a, V b) => a.IsText || b.IsText ? string.Concat(a.ToString(), b.ToString()) : a._n + b._n;
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

        /// <summary>괴물이 쓴 마법이면 그 괴물. 이때 `get_myid` 는 맞는 사람이다(`Mob_Spell.txt`).</summary>
        private readonly Sprite _actor;

        public Pack599(Sprite sprite, Sprite chosen)
        {
            _me = sprite as Aisling;
            _chosen = chosen;
        }

        private Pack599(Aisling victim, Sprite monster)
        {
            _me = victim;
            _actor = monster;
        }

        /// <summary>
        /// 괴물 마법(`Monster_이름`). 5.99 는 이 블록을 **맞는 사람 쪽에서** 돌린다 — `get_myid` 가 맞는 사람이고,
        /// 쓴 괴물은 `object_name`·`get_object_id`·`get_last_object_xs` 로 부른다. 피해를 주는 쪽은 괴물이다.
        /// </summary>
        public static Pack599 ForMonster(Sprite monster, Sprite target) => new Pack599(target as Aisling, monster);

        public bool Ready => _me != null && !_me.Dead;

        /// <summary>하데스의 기술 수련. 5.99 에는 없지만 기술 레벨이 오르는 길이 이것뿐이다.</summary>
        public void Train(Skill skill)
        {
            if (skill.Level < skill.Template.MaxLevel)
                _me.Client.TrainSkill(skill);
        }

        /// <summary>
        /// 캐릭터에 남는 스크립트 값(`#이름`·`$이름`). 없으면 0. 수로 읽히는 것은 수로 돌려준다 — 글자로 두면 `#EG + 200` 이
        /// 이어붙이기가 된다.
        /// </summary>
        public V this[string name]
        {
            get => _me?.PackVariables != null && _me.PackVariables.TryGetValue(name, out var kept)
                ? long.TryParse(kept, out var number) ? (V) number : (V) kept
                : 0;
            set
            {
                if (_me == null)
                    return;
                // 고친 사본으로 통째로 바꿔 끼운다. 자동 저장이 같은 캐릭터를 직렬화하며 이 사전을 훑는 중에 그 자리를
                // 고치면 "열거 중 변경" 예외가 난다 — 바꿔 끼우면 저장은 옛 사전을 끝까지 읽는다.
                var kept = _me.PackVariables == null
                    ? new Dictionary<string, string>()
                    : new Dictionary<string, string>(_me.PackVariables);
                kept[name] = value.ToString();
                _me.PackVariables = kept;
            }
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

                // ── 배우기 (NPC 스크립트 — PackNpc) ───────────────────────
                // `…2` 는 2차 직업 기술에만 쓰인다(Npc_Skill.txt, get_class_sub() == 2) — 2차 칸에 넣는 판으로 보이지만
                // 하데스 기술책에는 그런 칸 구분이 없어 같은 것으로 둔다.
                case "skill_exist": return _me.SkillBook.Skills.Values.Any(s => s?.Template?.Name == Text(a, 0)) ? 1 : 0;
                case "skill_add":
                case "skill_add2":
                    return ServerContext.GlobalSkillTemplateCache.ContainsKey(Text(a, 0))
                        ? Skill.GiveTo(_me.Client, Text(a, 0)) ? 1 : 0
                        : Unknown($"skill_add {Text(a, 0)}");
                case "spell_add":
                case "spell_add2":
                    return ServerContext.GlobalSpellTemplateCache.ContainsKey(Text(a, 0))
                        ? Spell.GiveTo(_me.Client, Text(a, 0)) ? 1 : 0
                        : Unknown($"spell_add {Text(a, 0)}");
                case "skill_del":
                case "skill_del2":
                {
                    var skill = _me.SkillBook.Skills.Values.FirstOrDefault(s => s?.Template?.Name == Text(a, 0));
                    if (skill == null)
                        return 0;
                    _me.SkillBook.Remove(skill.Slot);
                    _me.Client.Send(new ServerFormat2D(skill.Slot));
                    return 1;
                }
                case "spell_del":
                {
                    var spell = _me.SpellBook.Spells.Values.FirstOrDefault(s => s?.Template?.Name == Text(a, 0));
                    if (spell == null)
                        return 0;
                    _me.SpellBook.Remove(spell.Slot);
                    _me.Client.Send(new ServerFormat18(spell.Slot));
                    return 1;
                }

                // ── 이동·주기 (NPC 스크립트) ──────────────────────────────
                case "warp":
                {
                    var area = ServerContext.GlobalMapCache.Values.FirstOrDefault(map => map.Name == Text(a, 0));
                    if (area == null)
                        return Unknown($"warp {Text(a, 0)}");
                    _me.Client.TransitionToMap(area, new Position((int) Arg(a, 1), (int) Arg(a, 2)));
                    return 1;
                }
                case "item_add":
                {
                    if (!ServerContext.GlobalItemTemplateCache.TryGetValue(Text(a, 0), out var template))
                        return Unknown($"item_add {Text(a, 0)}");
                    var count = Math.Max(1, Arg(a, 1));
                    if (template.CanStack)
                    {
                        // 한 묶음 한도(MaxStack)를 넘는 개수는 여러 묶음으로 준다 — 한 묶음으로 자르면 나머지가 말없이 사라진다.
                        var perStack = Math.Max((int) template.MaxStack, 1);
                        for (var left = count; left > 0; left -= perStack)
                        {
                            var item = Item.Create(_me, template);
                            item.Stacks = (ushort) Math.Min(left, perStack);
                            item.GiveTo(_me, false);
                        }
                    }
                    else
                    {
                        for (var i = 0; i < count; i++)
                            Item.Create(_me, template).GiveTo(_me, false);
                    }

                    return 1;
                }
                case "get_sex": return (Who(a, 0) as Aisling) is { } person ? (long) person.Gender : 0;

                // ── 겉모습 (아이템 스크립트 — 염색약) ─────────────────────
                case "set_haircolor":
                    _me.HairColor = (byte) Arg(a, 0);
                    _me.Client.UpdateDisplay();
                    return 0;

                // ── 금화·단계 ─────────────────────────────────────────────
                case "get_money": return _me.GoldPoints;
                case "money_del":
                    _me.GoldPoints = (int) Math.Max(0, _me.GoldPoints - Arg(a, 0));
                    _me.Client.SendStats(StatusFlags.StructC);
                    return 0;
                // 5.99 의 전직 단계 0 1차 · 1 승급 · 2 2차 — 하데스 ClassStage 의 Class·Master·Dedicated 와 차례가 같다.
                case "get_class_sub": return (long) _me.Stage;
                case "get_ability": return _me.AbpLevel;

                case "get_att_damage": return Who(a, 0) is Aisling hitter ? AttackPower(hitter) : 0;
                case "get_mgc_damage": return Who(a, 0) is Aisling caster ? MagicPower(caster) : 0;

                // ── 보이기·들리기 ─────────────────────────────────────────
                case "effect":
                {
                    var target = Find(a, 0) ?? _me;
                    // 0x29 는 첫 그림을 맞는 쪽에, 둘째 그림을 쓴 쪽에 그린다 — 하데스는 인자 이름이 거꾸로다
                    // (`CasterEffect` 가 첫 자리). 5.99 `effect @대상, 쓴쪽그림, 대상그림, 속도` 를 그 순서로 보낸다.
                    _me.Show(Scope.NearbyAislings, new ServerFormat29((uint) (_actor ?? _me).Serial, (uint) target.Serial,
                        (ushort) Arg(a, 2), (ushort) Arg(a, 1), (ushort) Math.Max(1, Arg(a, 3))));
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

                // ── 개인 던전(map_create 사본 — Systems/Instances) ────────────────────
                // `map_create 번호, 이름, 보일이름, 가로, 세로, 음악, 단계, 세부단계, "팩맵파일"`. 사본마다 1초에 한 번
                // 던전 스크립트(Dungeon__Script)가 그 안 사람마다 돈다(PackRoutine).
                case "map_create":
                {
                    var original = PackMap(Text(a, 8));
                    if (original == null)
                        return Unknown($"map_create {Text(a, 8)}");
                    var area = Instances.Create(original, Text(a, 1), Text(a, 2), (int) Arg(a, 5), (int) Arg(a, 6), (int) Arg(a, 7));
                    if (area != null && (area.Scripts == null || area.Scripts.Count == 0))
                        area.Scripts = ScriptManager.Load<AreaScript>("PACK_Dungeon__Script", area);
                    return area?.Id ?? 0;
                }
                // `warp_create 종류, 출발맵, x, y, 도착맵, x, y, 최소레벨, 최대레벨, 막힘` — 막힘 1 은 괴물을 다 잡아야 지나간다.
                case "warp_create":
                {
                    var from = MapNamed(Text(a, 1));
                    var to = MapNamed(Text(a, 4));
                    if (from == null || to == null || Arg(a, 0) != 0)
                        return Unknown($"warp_create {Arg(a, 0)} {Text(a, 1)} → {Text(a, 4)}");
                    Instances.AddWarp(new WarpTemplate
                    {
                        Name = $"warp {from.Name}({Arg(a, 2)},{Arg(a, 3)}) to {to.Name}({Arg(a, 5)},{Arg(a, 6)})",
                        ActivationMapId = from.Id,
                        Activations = new List<Warp> { new Warp { AreaId = from.Id, Location = new Position((int) Arg(a, 2), (int) Arg(a, 3)) } },
                        To = new Warp { AreaId = to.Id, Location = new Position((int) Arg(a, 5), (int) Arg(a, 6)) },
                        WarpType = WarpType.Map,
                        LevelRequired = (byte) Math.Clamp(Arg(a, 7), 1, 99),
                        LevelMaximum = (byte) (Arg(a, 8) >= 99 ? 0 : Math.Clamp(Arg(a, 8), 1, 98)),
                        RequiresClear = Arg(a, 9) == 1
                    });
                    return 1;
                }
                // `group_warp 맵, x, y` — 같은 맵에 선 그룹원도 함께.
                case "group_warp":
                {
                    var area = MapNamed(Text(a, 0));
                    if (area == null)
                        return Unknown($"group_warp {Text(a, 0)}");
                    foreach (var member in Party().ToList())
                        member.Client.TransitionToMap(area, new Position((int) Arg(a, 1), (int) Arg(a, 2)));
                    return 1;
                }
                // `mob_spawn3 괴물, 맵, 최소공격력, 최대공격력, 체력, 마릿수` — 맵 안 빈 칸 아무 데나. 공격력 둘은 튜토리얼 팜팻(1, 2)이
                // 정의의 공격력과 같아 그렇게 읽었다. 하데스 괴물 피해는 식이 정하므로 체력만 쓴다.
                case "mob_spawn3":
                {
                    var area = MapNamed(Text(a, 1));
                    var template = ServerContext.GlobalMonsterTemplateCache.FirstOrDefault(t => t.Name == Text(a, 0));
                    if (area == null || template == null)
                        return Unknown($"mob_spawn3 {Text(a, 0)} @ {Text(a, 1)}");
                    for (var i = 0; i < Arg(a, 5); i++)
                    {
                        // 괴물 생성은 빈 칸을 몇 번만 찍어 보고 못 찾으면 null 이다 — 좁은 방에서는 마릿수가 모자란다. 5.99 는 적은 수를 다 세운다.
                        Monster monster = null;
                        for (var attempt = 0; attempt < 20 && monster == null; attempt++)
                            monster = Monster.Create(template, area);
                        if (monster == null)
                            continue;
                        if (Arg(a, 4) > 0)
                        {
                            monster._MaximumHp = (int) Arg(a, 4);
                            monster.CurrentHp = (int) Arg(a, 4);
                        }
                        _me.AddObject(monster);
                    }
                    return 1;
                }
                case "mob_clear": return Instances.Clear<Monster>(MapNamed(Text(a, 0)));
                case "item_clear": return Instances.Clear<Item>(MapNamed(Text(a, 0))) + Instances.Clear<Money>(MapNamed(Text(a, 0)));
                case "get_map_stage": return Who(a, 0).Map?.Stage ?? 0;
                case "get_map_sub_stage": return Who(a, 0).Map?.SubStage ?? 0;
                case "map_objmob":
                {
                    var map = Who(a, 0).Map;
                    return map == null ? 0 : _me.GetObjects<Monster>(map, m => m.CurrentHp > 0 && m.CurrentMapId == map.Id).Count();
                }
                case "get_clear_time": return Who(a, 0).Map is { } cleared ? (long) (DateTime.UtcNow - cleared.CreatedAt).TotalSeconds : 0;
                case "get_kill_mob": return Who(a, 0).Map?.Kills ?? 0;
                case "exp_add":
                    Monster.DistributeExperience(_me, Arg(a, 0));
                    // 경험치만 올리고 알리지 않아 화면(과 시험)이 다음 능력치 알림까지 옛 값을 보였다 — 괴물 보상(GenerateRewards)처럼 알린다.
                    _me.UpdateStats();
                    return 1;

                // ── 피해·회복·마력 ────────────────────────────────────────
                case "damaged":
                case "char_damaged":
                case "char_damaged2":
                case "char_damaged3":
                {
                    var target = Find(a, 0);
                    if (target == null || (_actor == null && target.Serial == _me.Serial) || !target.Attackable)
                        return 0;
                    if (target is Aisling && name == "damaged")
                        return 0;
                    target.ApplyDamage(_actor ?? _me, (int) Math.Min(int.MaxValue, Math.Max(0, Arg(a, 1))), (byte) 0);
                    return 0;
                }
                case "set_vital": return SetHealth(_me, Arg(a, 0));
                case "set_vita": return SetHealth(Find(a, 0), Arg(a, 1));
                // 5.99 `set_body` 16·32 = 산 몸(남·여), 64 = 유령. 하데스의 유령은 AislingFlags.Ghost 라, 뮤레칸·빛의이아가 산 몸을 입힐 때
                // 유령을 푼다. 체력은 앞의 set_vita 가 정한 대로 두고(하데스 Revive 는 가득 채운다), 멈췄던 회복만 다시 돌린다.
                case "set_body":
                {
                    if (Find(a, 0) is not Aisling who)
                        return 0;
                    if ((Arg(a, 1) == 16 || Arg(a, 1) == 32) && who.Dead)
                    {
                        who.Flags = AislingFlags.Normal;
                        who.Client.HpRegenTimer.Disabled = false;
                        who.Client.MpRegenTimer.Disabled = false;
                        who.Client.SendStats(StatusFlags.All);
                    }
                    return 1;
                }
                case "group_hill":
                    foreach (var member in Party())
                    {
                        SetHealth(member, member.CurrentHp + Arg(a, 0));
                        if (Arg(a, 1) > 0)
                            _me.Show(Scope.NearbyAislings, new ServerFormat29((uint) _me.Serial,
                                (uint) member.Serial, (ushort) Arg(a, 1), 0, 100));
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
                //   10 완전방어 — 결계 괴물이 "완전방어!" 하고 제게 거는 무적(하데스 dion)
                case "magic":
                    switch (Arg(a, 0))
                    {
                        case 1: return Afflict(Find(a, 1), new Curse(Curse.Slot, Arg(a, 4)), Arg(a, 3));
                        case 2: return Afflict(Find(a, 1), new debuff_sleep(), Arg(a, 3));
                        case 5: return Afflict(Find(a, 1), new MagicGuard(), Arg(a, 3));
                        case 6: return Afflict(Find(a, 1), new debuff_beagsuain(), Arg(a, 3));
                        case 7: return Afflict(Find(a, 1), new debuff_frozen(), Arg(a, 3));
                        case 8: return Afflict(Find(a, 1), new Curse(Curse.Mark, Arg(a, 4)), Arg(a, 3));
                        case 10: return Shield(Find(a, 1), Arg(a, 3));
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
                        case 10: return Find(a, 1)?.HasBuff("dion") == true ? 1 : 0;
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
                            (ushort) Arg(a, 0), 0, 100));
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
                // ── 혼수(빈사) ─────────────────────────────────────────────
                // 5.99 서버(Novaonline.exe) 역어셈블: set_coma 는 캐릭터 0x6D 칸(혼수), coma_delay 는 0xF1 칸(남은 초)에 쓰고, 서버가 1초마다
                // 그 초를 줄이며 그림 24 와 아이콘 89 를 보낸다(0x46f677). 0 이 되도록 살아나지 못하면 `__COMA_END__` 가 죽인다.
                // 하데스 빈사 디버프(debuff_reeping — 아이콘 89 · 그림 24, 끝나면 죽음)가 같은 것이라 그것을 건다.
                // set_state · set_state1 은 보이는 상태 칸(0xF0 — 1 혼수 모습 · 0 보통)에 쓰고 주변에 모습을 다시 보낸다(0x462ec1).
                // 혼수 모습은 빈사 디버프가 보내므로 따로 할 일이 없다. 5.99 스크립트는 0 과 1 만 쓴다.
                case "get_coma":
                case "get_state1":
                    return (Find(a, 0) as Aisling)?.Skulled == true ? 1 : 0;
                case "set_coma":
                {
                    var target = Find(a, 0);
                    if (target == null)
                        return 0;
                    if (Arg(a, 1) == 0)
                        return target.RemoveDebuff("skulled", true) ? 1 : 0;
                    return Afflict(target, new debuff_reeping(), ServerContext.Config.SkullLength);
                }
                case "coma_delay":
                {
                    if (Find(a, 0) is not { } target || Arg(a, 1) <= 0 || !target.Debuffs.TryGetValue("skulled", out var coma))
                        return 0;
                    // 이미 도는 혼수를 늘리지 않는다. 오솔길 함정(Dungeon__Script)은 터진 뒤 자리를 다시 적지 않아(5.99 Dungeon.txt 도 같다)
                    // 혼수로 서 있는 동안에도 1초마다 1/5 로 다시 터지고, 그때마다 12초로 되감겨 혼수가 끝나지 않았다(PoteDungeonTests).
                    if (coma.Length - coma.Timer.Tick <= Arg(a, 1))
                        return 0;
                    coma.Timer.Tick = coma.Length - (int) Arg(a, 1);
                    return 1;
                }
                case "set_state":
                case "set_state1":
                    return 1;
                // 하데스 RemoveDebuff("skulled") 는 취소 표시가 없으면 지우지 않는다(죽음을 건너뛰지 못하게) — 살려 내는 것이므로 취소로 지운다.
                case "del_coma":
                    return Find(a, 0)?.RemoveDebuff("skulled", true) == true ? 1 : 0;
                // 센스·센스몬스터·품뒤져보기 — 앞에 선 대상의 정보를 보여 준다(사용자 확인).
                case "sense_user":
                case "sense_monster":
                case "sense_item":
                    return Sense(name);
                // 몇 초 무적. 하데스 `dion` 이 같은 일을 한다.
                case "immortal":
                {
                    var who = Find(a, 0) ?? _me;
                    if (who.HasBuff("dion"))
                    {
                        (who as Aisling)?.Client.SendMessage(0x02, "이미 걸려있습니다.");
                        return 0;
                    }
                    return Shield(who, Arg(a, 1));
                }
                // ── 괴물 마법 ────────────────────────────────────────────
                case "object_name": return (_actor as Monster)?.Template?.Name ?? "";
                case "get_object_id": return _actor?.Serial ?? 0;
                case "get_last_object_xs": return _actor?.XPos ?? 1000;
                case "get_last_object_ys": return _actor?.YPos ?? 1000;
                // 괴물이 말한다 — `mob_say2 괴물, 0, 0, "메테오."`.
                case "mob_say2":
                    Find(a, 0)?.Show(Scope.NearbyAislings, new ServerFormat0D
                    {
                        Serial = (int) Arg(a, 0),
                        Type = 0x00,
                        Text = $"{(Find(a, 0) as Monster)?.Template?.Name}: {Text(a, 3)}"
                    });
                    return 0;
                // 자르반·엘리멘탈의 메테오 — 맞는 사람 파티에서 가장 큰 최대 체력을 나눈 만큼 파티 전체를 친다.
                case "group_bighp": return Party().Select(m => (long) m.MaximumHp).DefaultIfEmpty(0).Max();
                case "group_damaged2":
                    foreach (var member in (Find(a, 0) as Aisling)?.PartyMembers?.Where(m => m?.Map == _me.Map) ??
                                           new[] { Find(a, 0) as Aisling })
                        member?.ApplyDamage(_actor ?? _me, (int) Math.Min(int.MaxValue, Math.Max(0, Arg(a, 1))), (byte) 0);
                    return 0;

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
                            (ushort) Arg(a, 0), 0, 100));
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

        private static V Shield(Sprite who, long seconds)
        {
            if (who == null || who.HasBuff("dion"))
                return 0;
            var buff = new buff_dion();
            buff.Timer.Tick = buff.Length - (int) seconds;
            buff.OnApplied(who, buff);
            return 1;
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

        private static Area MapNamed(string name) =>
            ServerContext.GlobalMapCache.Values.FirstOrDefault(map => map.Name == name);

        /// <summary>
        /// 팩 스크립트가 적는 맵 파일(`db/maps/default/maps/lod4612.map`) → 서버 맵. 표는 `tools/pack-import/import.py --kind maps` 가
        /// `static/pack599-mapfiles.tsv` 로 쓴다(팩 파일 경로 · 서버 맵 번호).
        /// </summary>
        private static Area PackMap(string file)
        {
            if (PackMapFiles == null)
            {
                var table = new Dictionary<string, int>();
                var path = Path.Combine(ServerContext.StoragePath, "static", "pack599-mapfiles.tsv");
                if (File.Exists(path))
                    foreach (var line in File.ReadAllLines(path))
                    {
                        var cells = line.Split('\t');
                        if (cells.Length == 2 && int.TryParse(cells[1], out var id))
                            table[cells[0]] = id;
                    }
                PackMapFiles = table;
            }

            return PackMapFiles.TryGetValue(file, out var number) && ServerContext.GlobalMapCache.TryGetValue(number, out var area)
                ? area
                : null;
        }

        private static Dictionary<string, int> PackMapFiles;

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
