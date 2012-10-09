using System.IO;
using System;
using Styx;
using Styx.Helpers;
using Styx.Common;

namespace BloodyMess
{
    public class BloodyMessSettings : Styx.Helpers.Settings
    {
        public BloodyMessSettings()
            : base(Path.Combine(BloodyMess.DeathKnight.baseFolder, string.Format("BloodyMessSettings_{0}.xml", StyxWoW.Me.Name)))
        { }

        [Setting, DefaultValue(true)]
        public bool BloodPresence { get; set; }

        [Setting, DefaultValue(false)]
        public bool DisableTargeting { get; set; }

        [Setting, DefaultValue(false)]
        public bool DisableMovement { get; set; }

        [Setting, DefaultValue(false)]
        public bool FrostPresence { get; set; }

        [Setting, DefaultValue(false)]
        public bool UnholyPresence { get; set; }

        [Setting, DefaultValue(true)]
        public bool UseMindFreeze { get; set; }

        [Setting, DefaultValue(true)]
        public bool UseStrangulate { get; set; }

        [Setting, DefaultValue(true)]
        public bool UseDeathGripInterrupt { get; set; }

        [Setting, DefaultValue(true)]
        public bool UseDeathGrip { get; set; }

        [Setting, DefaultValue(true)]
        public bool UseERW { get; set; }

        [Setting, DefaultValue(true)]
        public bool UseDRW { get; set; }

        [Setting, DefaultValue(true)]
        public bool UsePath { get; set; }

        [Setting, DefaultValue(true)]
        public bool UseBloodTap { get; set; }

        [Setting, DefaultValue(true)]
        public bool UseBoneShield { get; set; }

        [Setting, DefaultValue(true)]
        public bool UseHorn { get; set; }

        [Setting, DefaultValue(50)]
        public int LichbornePercent { get; set; }

        [Setting, DefaultValue(90)]
        public int DeathCoilPercent { get; set; }

        [Setting, DefaultValue(60)]
        public int VampiricBloodPercent { get; set; }

        [Setting, DefaultValue(70)]
        public int DeathSiphonPercent { get; set; }

        [Setting, DefaultValue(60)]
        public int RuneTapPercent { get; set; }

        [Setting, DefaultValue(50)]
        public int AMSPercent { get; set; }

        [Setting, DefaultValue(50)]
        public int IBFPercent { get; set; }

        [Setting, DefaultValue(90)]
        public int DeathStrikePercent { get; set; }

        [Setting, DefaultValue(1)]
        public int RevisionNumber { get; set; }

    }
}
