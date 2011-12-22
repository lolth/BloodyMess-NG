using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Logic.POI;

namespace BloodyMess
{
    class DeathKnight : CombatRoutine
    {
        private string vNum = "v0.9.2";
        public override sealed string Name { get { return "Joystick's BloodyMess PVP " + vNum; } }
        public override WoWClass Class { get { return WoWClass.DeathKnight; } }
        private static LocalPlayer Me { get { return ObjectManager.Me; } }
        private bool BloodPresenceSwitch = false;

        private BloodyMessForm BloodyMessConfig;


        private bool DisableMovement
        {
            get { return BloodyMessConfig.Settings.DisableMovement; }
        }
        private bool DisableTargeting
        {
            get { return BloodyMessConfig.Settings.DisableTargeting; }
        }
        private bool BloodPresence
        {
            get { return BloodyMessConfig.Settings.BloodPresence; }
        }
        private bool FrostPresence
        {
            get { return BloodyMessConfig.Settings.FrostPresence; }
        }
        private bool UnholyPresence
        {
            get { return BloodyMessConfig.Settings.UnholyPresence; }
        }
        private bool UseMindFreeze
        {
            get { return BloodyMessConfig.Settings.UseMindFreeze; }
        }
        private bool UseStrangulate
        {
            get { return BloodyMessConfig.Settings.UseStrangulate; }
        }
        private bool UseDeathGripInterrupt
        {
            get { return BloodyMessConfig.Settings.UseDeathGripInterrupt; }
        }
        private bool UseDeathGrip
        {
            get { return BloodyMessConfig.Settings.UseDeathGrip; }
        }
        private bool UseERW
        {
            get { return BloodyMessConfig.Settings.UseERW; }
        }
        private bool UseDRW
        {
            get { return BloodyMessConfig.Settings.UseDRW; }
        }
        private bool UseBoneShield
        {
            get { return BloodyMessConfig.Settings.UseBoneShield; }
        }
        private bool UseHorn
        {
            get { return BloodyMessConfig.Settings.UseHorn; }
        }
        private int LichbornePercent
        {
            get { return BloodyMessConfig.Settings.LichbornePercent; }
        }
        private int DeathCoilPercent
        {
            get { return BloodyMessConfig.Settings.DeathCoilPercent; }
        }
        private int VampiricBloodPercent
        {
            get { return BloodyMessConfig.Settings.VampiricBloodPercent; }
        }
        private int RuneTapPercent
        {
            get { return BloodyMessConfig.Settings.RuneTapPercent; }
        }
        private int AMSPercent
        {
            get { return BloodyMessConfig.Settings.AMSPercent; }
        }
        private int IBFPercent
        {
            get { return BloodyMessConfig.Settings.IBFPercent; }
        }
        private int DeathStrikePercent
        {
            get { return BloodyMessConfig.Settings.DeathStrikePercent; }
        }

        public override bool WantButton
        {
            get
            {
                return true;
            }
        }
        public override void OnButtonPress()
        {
            if (BloodyMessConfig != null)
                BloodyMessConfig.ShowDialog();
            else
            {
                BloodyMessConfig = new BloodyMessForm();
                BloodyMessConfig.ShowDialog();
            }
        }
        public override void Initialize()
        {
            BloodyMessConfig = new BloodyMessForm();
            Logging.Write(Color.White, "Joystick's BloodyMess PVP Started");
        }
        public override bool NeedRest { get { return false; } }
        public override void Rest()
        {
        }
        public override void Pull()
        {
            if (!DisableTargeting)
            {
                GetTarget();
                if (Me.CurrentTarget == null || !Me.GotTarget || !Me.CurrentTarget.Attackable || Me.Stunned || Me.Fleeing || Me.Dead)
                    return;
                GetFace();
            }
            else
            {
                if (Me.Mounted)
                    return;
            }
            if (!DisableMovement)
            {
                GetMelee();
            }
            if (CheckHealth())
            {
                if (MustHeal())
                    return;
            }
            if (InterruptRotation())
                return;
            if (CheckBuffs())
            {
                if (MustBuff())
                    return;
            }
            if (!SpellManager.GlobalCooldown)
            {
                if (DiseasesRotation())
                    return;
                if (UseCooldowns())
                    return;
                if (Me.CurrentTarget.Distance > 4)
                {
                    if (RangedRotation())
                        return;
                }
                else if (Me.CurrentTarget.Distance <= 4)
                {
                    if (MeleeRotation())
                        return;
                }
            }

        }
        public override bool NeedPullBuffs { get { return false; } }
        public override void PullBuff() { }
        public override bool NeedPreCombatBuffs { get { return false; } }
        public override void PreCombatBuff()
        {
        }
        public override bool NeedCombatBuffs { get { return false; } }
        public override void CombatBuff()
        {
        }
        public override bool NeedHeal { get { return false; } }
        public override void Heal()
        {

        }
        public void HandleFalling() { }
        public override void Combat()
        {
            if (!DisableTargeting)
            {
                GetTarget();
                if (Me.CurrentTarget == null || !Me.GotTarget || !Me.CurrentTarget.Attackable || Me.Stunned || Me.Fleeing || Me.Dead)
                    return;
                GetFace();
            }
            else
            {
                if (Me.Mounted)
                    return;
            }
            if (!DisableMovement)
            {
                GetMelee();
            }
            if (CheckHealth())
            {
                if (MustHeal())
                    return;
            }
            if (InterruptRotation())
                return;
            if (CheckBuffs())
            {
                if (MustBuff())
                    return;
            }
            if (!SpellManager.GlobalCooldown)
            {
                if (DiseasesRotation())
                    return;
                if (UseCooldowns())
                    return;
                if (Me.GotTarget)
                {
                    if (Me.CurrentTarget.Distance > 4)
                    {
                        if (RangedRotation())
                            return;
                    }
                    else if (Me.CurrentTarget.Distance <= 4)
                    {
                        if (MeleeRotation())
                            return;
                    }
                }
            }
        }
        private void AutoAttack()
        {
            if (!Me.IsAutoAttacking)
            {
                Lua.DoString("StartAttack()");
            }

        }
        private bool CanCast(string spellName)
        {
            if (SpellManager.CanCast(spellName))
                return true;
            else
            {
                return false;
            }
        }
        private bool CanBuff(string spellName)
        {
            if (SpellManager.CanBuff(spellName))
                return true;
            else
            {
                return false;
            }
        }
        private void Cast(string spellName)
        {
            Logging.Write(Color.Yellow, "[BloodyMess] Casting " + spellName);
            if (Me.GotTarget)
                SpellManager.Cast(spellName);
        }
        private void Buff(string spellName)
        {
            Logging.Write(Color.Green, "[BloodyMess] Buffing " + spellName);
            SpellManager.Buff(spellName);
        }
        private void CastMe(string spellName)
        {
            Logging.Write(Color.Yellow, "[BloodyMess] Casting " + spellName + " on Myself");
            SpellManager.Cast(spellName, Me);
        }
        private void Interrupt(String spellName)
        {
            Logging.Write(Color.Red, "[BloodyMess] Interrupting " + Me.CurrentTarget + "'s " + Me.CurrentTarget.CastingSpell.ToString() + " with " + spellName);
            if (Me.GotTarget)
                SpellManager.Cast(spellName);
        }
        private bool CCTC(string spellName)
        {
            if (CanCast(spellName) && Me.GotTarget)
            {
                Cast(spellName);
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CCTCMe(string spellName)
        {
            if (CanCast(spellName))
            {
                CastMe(spellName);
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CCBC(string spellName)
        {
            if (CanBuff(spellName))
            {
                Buff(spellName);
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CCIC(string spellName)
        {
            if (CanCast(spellName) && Me.GotTarget)
            {
                Interrupt(spellName);
                return true;
            }
            else
            {
                return false;
            }
        }
        private void GetMelee()
        {
            if (Me.GotTarget)
            {
                if (Me.CurrentTarget.Distance > 1 && !Me.IsCasting && Styx.BotManager.Current.Name != "LazyRaider")
                {
                    Navigator.MoveTo(Me.CurrentTarget.Location);
                }
                else if (Me.CurrentTarget.Distance <= 1 && Styx.BotManager.Current.Name != "LazyRaider")
                {
                    Navigator.PlayerMover.MoveStop();
                }
            }
        }
        private void GetFace()
        {
            if (Me.GotTarget)
            {
                if (!Me.IsFacing(Me.CurrentTarget.Location))
                    Me.CurrentTarget.Face();
            }
        }
        private bool DiseasesRotation()
        {
            if (Me.GotTarget)
            {

                if (!Me.CurrentTarget.HasAura("Frost Fever") && !Me.CurrentTarget.HasAura("Blood Plague"))
                {
                    if (Me.CurrentTarget.Distance < 30)
                    {
                        if (CCTC("Outbreak"))
                            return true;
                    }
                    if (Me.CurrentTarget.Distance < 20)
                    {
                        if (CCTC("Icy Touch"))
                            return true;
                    }

                }
                if (!Me.CurrentTarget.HasAura("Frost Fever"))
                {
                    if (Me.CurrentTarget.Distance < 30)
                    {
                        if (CCTC("Outbreak"))
                            return true;
                    }
                    if (Me.CurrentTarget.Distance < 20)
                    {
                        if (CCTC("Icy Touch"))
                            return true;
                    }
                }
                if (!Me.CurrentTarget.HasAura("Blood Plague"))
                {
                    if (Me.CurrentTarget.Distance < 30)
                    {
                        if (CCTC("Outbreak"))
                            return true;
                    }
                    if (Me.CurrentTarget.Distance < 4)
                    {
                        if (CCTC("Plague Strike"))
                            return true;
                    }
                }
            }

            return false;
        }
        private void GetTarget()
        {
            if (!Me.GotTarget)
            {
                FindTarget();
            }
            else
            {
                if (Me.CurrentTarget.IsPet)
                {
                    (Me.CurrentTarget.CreatedByUnit).Target();
                    BotPoi.Current = new BotPoi(Me.CurrentTarget, PoiType.Kill);
                    WoWMovement.ConstantFace(Me.CurrentTargetGuid);
                }

                if (Me.CurrentTarget.Distance > 30 || !Me.CurrentTarget.IsAlive || !Me.CurrentTarget.Attackable
                    || Me.CurrentTarget.HasAura("Spirit of Redemption") || Me.CurrentTarget.HasAura("Ice Block"))
                {
                    Me.ClearTarget();
                    FindTarget();
                }
            }
        }
        private void FindTarget()
        {
            if (Styx.BotManager.Current.Name != "BG Bot [Beta]" && Styx.BotManager.Current.Name != "PvP")
                return;
            WoWPlayer newTarget = ObjectManager.GetObjectsOfType<WoWPlayer>(false, false).
                Where(p => p.IsHostile && !p.IsTotem && !p.IsPet && !p.Dead && p.DistanceSqr <= (10 * 10) && !p.Mounted
                    && !p.HasAura("Ice Block") && !p.HasAura("Spirit of Redemption")).
                OrderBy(u => u.HealthPercent).FirstOrDefault();
            if (newTarget != null)
            {
                newTarget.Target();
                BotPoi.Current = new BotPoi(Me.CurrentTarget, PoiType.Kill);
                WoWMovement.ConstantFace(Me.CurrentTargetGuid);
            }

        }
        private bool InterruptRotation()
        {
            if (Me.GotTarget)
            {
                if (Me.CurrentTarget.CanInterruptCurrentSpellCast && Me.CurrentTarget.IsCasting)
                {
                    if (!SpellManager.GlobalCooldown)
                    {
                        if (Me.CurrentTarget.Distance < 30 && UseStrangulate)
                        {
                            if (CCIC("Strangulate"))
                                return true;
                        }
                    }
                    if (Me.CurrentTarget.Distance < 4 && UseMindFreeze)
                    {
                        if (CCIC("Mind Freeze"))
                            return true;
                    }
                    if (Me.CurrentTarget.Distance < 30 && UseDeathGripInterrupt)
                    {
                        if (CCIC("Death Grip"))
                            return true;
                    }
                }
            }
            return false;
        }
        private bool RangedRotation()
        {
            if (Me.GotTarget)
            {
                if (Me.CurrentTarget.Distance < 30 && UseDeathGrip && !IsPVPBoss())
                {
                    if (CCTC("Death Grip"))
                        return true;
                }
                if (Me.CurrentTarget.Distance < 30)
                {
                    if (CCTC("Death Coil"))
                        return true;
                }
                if (Me.CurrentTarget.Distance < 20)
                {
                    if (CCTC("Chains of Ice"))
                        return true;
                }
            }
            return false;
        }
        private bool MeleeRotation()
        {
            if (Me.GotTarget)
            {
                if (!Me.CurrentTarget.HasAura("Necrotic Strike"))
                {
                    if (CCTC("Necrotic Strike"))
                        return true;
                }
                if (CCTC("Death Strike"))
                    return true;
                if (CCTC("Heart Strike"))
                    return true;
                if (CCTC("Necrotic Strike"))
                    return true;
                if (CCTC("Death Coil"))
                    return true;
            }

            return false;
        }
        private bool UseCooldowns()
        {
            if (UseERW)
            {
                if (CCTC("Empower Rune Weapon"))
                    return true;
            }
            if (UseDRW)
            {
                if (CCTC("Dancing Rune Weapon"))
                    return true;
            }

            return false;
        }
        private bool CheckHealth()
        {
            if (Me.HealthPercent < LichbornePercent && CanCast("Lichborne"))
            {
                return true;
            }
            else if (Me.HealthPercent < DeathCoilPercent && CanCast("Death Coil") && Me.HasAura("Lichborne") && !SpellManager.GlobalCooldown)
            {
                return true;
            }
            else if (Me.HealthPercent < IBFPercent && CanCast("Icebound Fortitude"))
            {
                return true;
            }
            else if (Me.HealthPercent < AMSPercent && CanCast("Anti-Magic Shell"))
            {
                return true;
            }
            else if (Me.HealthPercent < VampiricBloodPercent && CanCast("Vampiric Blood"))
            {
                return true;
            }
            else if ((Me.HealthPercent < RuneTapPercent || (Me.HasAura("Will of the Necropolis") && Me.HealthPercent < RuneTapPercent)) && CanCast("Rune Tap"))
            {
                return true;
            }
            else if (Me.HealthPercent < DeathStrikePercent && CanCast("Death Strike") && Me.CurrentTarget.Distance < 4 && !SpellManager.GlobalCooldown)
            {
                return true;
            }

            return false;
        }
        private bool MustHeal()
        {
            if (Me.HealthPercent < VampiricBloodPercent)
            {
                if (CCTC("Vampiric Blood"))
                    return true;
            }
            if (Me.HealthPercent < LichbornePercent && CanBuff("Lichborne"))
            {
                if (CCTC("Lichborne"))
                    return true;
            }
            if (Me.HasAura("Lichborne") && Me.HealthPercent < DeathCoilPercent && !SpellManager.GlobalCooldown)
            {
                if (CCTCMe("Death Coil"))
                    return true;
            }
            if (Me.HealthPercent < RuneTapPercent || (Me.HasAura("Will of the Necropolis") && Me.HealthPercent < RuneTapPercent))
            {
                if (CCTC("Rune Tap"))
                    return true;
            }
            if (Me.HealthPercent < AMSPercent)
            {
                if (CCTC("Anti-Magic Shell"))
                    return true;
            }
            if (Me.HealthPercent < IBFPercent)
            {
                if (CCTC("Icebound Fortitude"))
                    return true;
            }
            if (Me.CurrentTarget.Distance < 4 && !SpellManager.GlobalCooldown && Me.HealthPercent < DeathStrikePercent)
            {
                if (CCTC("Death Strike"))
                    return true;
            }

            return false;
        }
        private bool CheckBuffs()
        {
            if ((!Me.HasAura("Bone Shield") && UseBoneShield)
             || ((!Me.HasAura("Horn of Winter") && !Me.HasAura("Battle Shout")) && UseHorn)
             || ((!Me.HasAura("Blood Presence") || !BloodPresenceSwitch) && BloodPresence)
             || (!Me.HasAura("Frost Presence") && FrostPresence)
             || (!Me.HasAura("Unholy Presence") && UnholyPresence))
            {
                return true;
            }
            else
                return false;
        }
        private bool MustBuff()
        {
            if (!Me.HasAura("Bone Shield") && UseBoneShield)
            {
                if (CCTC("Bone Shield"))
                    return true;
            }
            if ((!Me.HasAura("Horn of Winter") && !Me.HasAura("Battle Shout")) && UseHorn)
            {
                if (CCTC("Horn of Winter"))
                    return true;
            }
            if ((!Me.HasAura("Blood Presence") || !BloodPresenceSwitch) && BloodPresence)
            {
                if (CCTC("Blood Presence"))
                {
                    BloodPresenceSwitch = true;
                    return true;
                }
            }
            if (!Me.HasAura("Frost Presence") && FrostPresence)
            {
                if (CCTC("Frost Presence"))
                {
                    BloodPresenceSwitch = false;
                    return true;
                }
            }
            if (!Me.HasAura("Unholy Presence") && UnholyPresence)
            {
                if (CCTC("Unholy Presence"))
                {
                    BloodPresenceSwitch = false;
                    return true;
                }
            }

            return false;
        }

        private bool IsPVPBoss()
        {
            if (Me.CurrentTarget.Name.ToString().Equals("Drek'Thar")
             || Me.CurrentTarget.Name.ToString().Equals("Vanndar Stormpike")
             || Me.CurrentTarget.Name.ToString().Equals("Captain Balinda Stonehearth")
             || Me.CurrentTarget.Name.ToString().Equals("Captain Galvangar")
             || Me.CurrentTarget.Name.ToString().Equals("High Commander Halford Wyrmbane")
             || Me.CurrentTarget.Name.ToString().Equals("Overlord Agmar"))
            {
                return true;
            }


            return false;
        }

    }
}