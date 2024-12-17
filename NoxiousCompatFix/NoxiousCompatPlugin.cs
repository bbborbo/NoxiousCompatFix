using System;
using BepInEx;
using BepInEx.Configuration;
using RoR2;

namespace NoxiousCompatFix
{
    [BepInPlugin(guid, modName, version)]
    public class NoxiousCompatPlugin : BaseUnityPlugin
    {
        public const string guid = "com." + teamName + "." + modName;
        public const string teamName = "RiskOfBrainrot";
        public const string modName = "NoxiousCompatFix";
        public const string version = "1.0.2";
        internal static ConfigFile CustomConfigFile { get; set; }

        void Awake()
        {
            CustomConfigFile = new ConfigFile(Paths.ConfigPath + $"\\{guid}.cfg", true);

            On.RoR2.BuffCatalog.SetBuffDefs += NoxiousThornDebuffCompat;
        }

        private void NoxiousThornDebuffCompat(On.RoR2.BuffCatalog.orig_SetBuffDefs orig, BuffDef[] newBuffDefs)
        {
            foreach (BuffDef buffDef in newBuffDefs)
            {
                if (buffDef.isDebuff || buffDef.isDOT)
                {
                    bool isCompatWithNox = !buffDef.flags.HasFlag(BuffDef.Flags.ExcludeFromNoxiousThorns);
                    bool wantCompat = isCompatWithNox;
                    switch (buffDef.name)
                    {
                        case "bdLunarDetonationCharge":
                            wantCompat = true;
                            break;
                    }
                    wantCompat = CustomConfigFile.Bind<bool>("Noxious Thorn Compatibility", 
                        $"Should Nox Transfer Debuff: {buffDef.name}", wantCompat,
                        $"Vanilla value is {isCompatWithNox}. Set to TRUE for Noxious Thorn to transfer this debuff on kill.").Value;

                    if (wantCompat != isCompatWithNox)
                    {
                        //if buff is compatible with nox, remove it
                        if (isCompatWithNox)
                        {
                            buffDef.flags |= BuffDef.Flags.ExcludeFromNoxiousThorns;
                        }
                        else //if buff is not compatible with nox, add it
                        {
                            buffDef.flags &= ~BuffDef.Flags.ExcludeFromNoxiousThorns;
                        }
                    }
                }
            }
            orig(newBuffDefs);
        }
    }
}
