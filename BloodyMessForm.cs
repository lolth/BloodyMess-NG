using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using Styx;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace BloodyMess
{
    public partial class BloodyMessForm : Form
    {
        public BloodyMessSettings Settings;
        public BloodyMessForm()
        {
            InitializeComponent();
            if (Settings == null)
            {
                Settings = new BloodyMessSettings();
                Settings.Load();
            }
        }
        private void UpdateSettingsFromGUI()
        {
            Settings.UseERW = checkBox1.Checked;
            Settings.UseDRW = checkBox2.Checked;
            Settings.UseStrangulate = checkBox3.Checked;
            Settings.UseMindFreeze = checkBox4.Checked;
            Settings.UseDeathGripInterrupt = checkBox5.Checked;
            Settings.UseBoneShield = checkBox6.Checked;
            Settings.UseHorn = checkBox7.Checked;
            Settings.DisableMovement = checkBox8.Checked;
            Settings.DisableTargeting = checkBox9.Checked;
            Settings.UseDeathGrip = checkBox10.Checked;
            Settings.UsePath = checkBox11.Checked;
            Settings.UseBloodTap = checkBox12.Checked;

            Settings.VampiricBloodPercent = (int)numericUpDown1.Value;
            Settings.LichbornePercent = (int)numericUpDown2.Value;
            Settings.DeathCoilPercent = (int)numericUpDown3.Value;
            Settings.RuneTapPercent = (int)numericUpDown4.Value;
            Settings.AMSPercent = (int)numericUpDown5.Value;
            Settings.IBFPercent = (int)numericUpDown6.Value;
            Settings.DeathStrikePercent = (int)numericUpDown7.Value;

            Settings.BloodPresence = radioButton1.Checked;
            Settings.FrostPresence = radioButton2.Checked;
            Settings.UnholyPresence = radioButton3.Checked;
        }
        private void UpdateGUIFromSettings()
        {
            checkBox1.Checked = Settings.UseERW;
            checkBox2.Checked = Settings.UseDRW;
            checkBox3.Checked = Settings.UseStrangulate;
            checkBox4.Checked = Settings.UseMindFreeze;
            checkBox5.Checked = Settings.UseDeathGripInterrupt;
            checkBox6.Checked = Settings.UseBoneShield;
            checkBox7.Checked = Settings.UseHorn;
            checkBox8.Checked = Settings.DisableMovement;
            checkBox9.Checked = Settings.DisableTargeting;
            checkBox10.Checked = Settings.UseDeathGrip;
            checkBox11.Checked = Settings.UsePath;
            checkBox12.Checked = Settings.UseBloodTap;

            numericUpDown1.Value = Settings.VampiricBloodPercent;
            numericUpDown2.Value = Settings.LichbornePercent;
            numericUpDown3.Value = Settings.DeathCoilPercent;
            numericUpDown4.Value = Settings.RuneTapPercent;
            numericUpDown5.Value = Settings.AMSPercent;
            numericUpDown6.Value = Settings.IBFPercent;
            numericUpDown7.Value = Settings.DeathStrikePercent;

            radioButton1.Checked = Settings.BloodPresence;
            radioButton2.Checked = Settings.FrostPresence;
            radioButton3.Checked = Settings.UnholyPresence;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateSettingsFromGUI();
            Settings.Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Updater.CheckForUpdate();
        }

        public void Form_Load(object sender, EventArgs eArgs)
        {
            UpdateGUIFromSettings();
        }
    }
}
