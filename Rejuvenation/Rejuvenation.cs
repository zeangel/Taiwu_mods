﻿using Harmony12;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace Rejuvenation
{
    public class Settings : UnityModManager.ModSettings
    {
        public int rejuvenatedAge = 0;
        public int rejuvenationAge = 6;
        public int changedGongFaFLevel = 0;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

    }

    public static class Main
    {
        public static bool enabled;
        public static UnityModManager.ModEntry.ModLogger Logger;

        public static Settings setting;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            setting = Settings.Load<Settings>(modEntry);

            Logger = modEntry.Logger;

            modEntry.OnToggle = OnToggle;

            modEntry.OnGUI = OnGUI;

            //if (enabled)
            {
                string resdir = System.IO.Path.Combine(modEntry.Path, "Data");
                Logger.Log(" resdir :" + resdir);
                BaseResourceMod.Main.registModResDir(modEntry, resdir);
            }

            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            return true;
        }

        public static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (!value)
                return false;

            enabled = value;

            return true;
        }

        public static void DoRemoveChooseGongFa()
        {
            int key = DateFile.instance.MianActorID();
            DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0] = 0;
            DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][1] = 0;
            DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][2] = 0;
            DateFile.instance.gongFaBookPages.Remove(150369);
            DateFile.instance.RemoveMainActorEquipGongFa(150369);
        }

        public static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            bool cond = (DateFile.instance == null || DateFile.instance.actorsDate == null || !DateFile.instance.actorsDate.ContainsKey(DateFile.instance.mianActorId));
            if (cond)
            {
                GUILayout.Label("未加载存档！");
                return;
            }
            if (!DateFile.instance.gongFaDate.ContainsKey(150369))
            {
                GUILayout.Label("增量数据未正常加载！");
                return;
            }

            //DateFile.instance.ChangeActorGongFa(DateFile.instance.mianActorId, 150369, 0, 0, 0, true);

            if (!DateFile.instance.actorGongFas[DateFile.instance.mianActorId].ContainsKey(150369))
            {
                if (DateFile.instance.actorGongFas[DateFile.instance.mianActorId].ContainsKey(150002))
                {
                    int readPageNum = 0;
                    int[] bookPages = (!DateFile.instance.gongFaBookPages.ContainsKey(150002)) ? new int[10] : DateFile.instance.gongFaBookPages[150002];
                    for (int i = 0; i < 10; i++)
                    {
                        if (bookPages[i] != 0)
                        {
                            readPageNum++;
                        }
                    }
                    GUILayout.Label("须得「沛然诀」大成，方能领悟「天长地久不老长春功」。");
                    GUILayout.BeginHorizontal("Box");
                    GUILayout.Label("修习进度：" + DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150002][0].ToString() + "%");
                    GUILayout.Label("研读进度：" + readPageNum.ToString() + "/10");
                    GUILayout.EndHorizontal();
                }
                else
                {
                    int[] allQi = DateFile.instance.GetActorAllQi(DateFile.instance.mianActorId);
                    GUILayout.Label("须以上乘内功为根基，方能修炼「天长地久不老长春功」。");
                    GUILayout.BeginHorizontal("Box");
                    GUILayout.Label("当前内力：" + (allQi[0] + allQi[1] + allQi[2] + allQi[3] + allQi[4]).ToString() + "/500");
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                //GUILayout.BeginHorizontal("Box");
                //GUILayout.Label("年龄：");
                //DateFile.instance.actorsDate[DateFile.instance.mianActorId][11] = GUILayout.TextField(DateFile.instance.actorsDate[DateFile.instance.mianActorId][11]);
                //GUILayout.Label("修习进度：");
                //DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0] = int.Parse(GUILayout.TextField(DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0].ToString()));
                //GUILayout.Label("研读进度：");
                //DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][1] = int.Parse(GUILayout.TextField(DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][1].ToString()));
                //DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][2] = int.Parse(GUILayout.TextField(DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][2].ToString()));
                //GUILayout.EndHorizontal();

                GUILayout.Label("修炼「天长地久不老长春功」有所成者，有返老还童之能，届时功力散去，有伐毛洗髓之效。");
                GUILayout.BeginHorizontal("Box");
                GUILayout.Label("修习进度：" + DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0].ToString() + "%"
                    + (setting.rejuvenatedAge == 0
                    && DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0] >= 25 && DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0] < 100
                    && DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0] / 25 * 30 + 6 < int.Parse(DateFile.instance.actorsDate[DateFile.instance.mianActorId][11])
                    ? " -1%/时节" : "        "));
                GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
                GUILayout.Label("触发年龄：");
                GUILayout.FlexibleSpace();
                string[] ageList = { "不触发", "36岁", "66岁", "96岁" };
                setting.rejuvenatedAge = GUILayout.Toolbar(setting.rejuvenatedAge, ageList);
                setting.rejuvenatedAge = Mathf.Min(DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0] / 25, setting.rejuvenatedAge);
                GUILayout.EndHorizontal();
                GUILayout.EndHorizontal();

                GUILayout.Label("修炼者内功造诣越高，对功法的控制力越强。");
                GUILayout.BeginHorizontal("Box");
                GUILayout.Label("研读进度："
                    + "正" + DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][1].ToString()
                    + " 逆" + DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][2].ToString());
                GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
                GUILayout.Label("返老还童后正逆练：");
                if (GUILayout.Button("<", GUILayout.Width(30)) && DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][2] - setting.changedGongFaFLevel > 0)
                {
                    Main.setting.changedGongFaFLevel++;
                }
                GUILayout.Label("正" + (DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][1] + setting.changedGongFaFLevel).ToString()
                    + " 逆" + (DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][2] - setting.changedGongFaFLevel).ToString());
                if (GUILayout.Button(">", GUILayout.Width(30)) && DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][1] + setting.changedGongFaFLevel > 0)
                {
                    Main.setting.changedGongFaFLevel--;
                }
                Main.setting.changedGongFaFLevel = Mathf.Min(Main.setting.changedGongFaFLevel, DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][2]);
                Main.setting.changedGongFaFLevel = Mathf.Max(Main.setting.changedGongFaFLevel, -1 * DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][1]);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
                GUILayout.Label("返老还童后年龄：");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("<", GUILayout.Width(30)) && Main.setting.rejuvenationAge > 6)
                {
                    Main.setting.rejuvenationAge--;
                }
                GUILayout.Label("  " + Main.setting.rejuvenationAge.ToString() + "  ");
                if (GUILayout.Button(">", GUILayout.Width(30)))
                {
                    Main.setting.rejuvenationAge++;
                }
                Main.setting.rejuvenationAge = Mathf.Min(Main.setting.rejuvenationAge,
                    6 + DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][1] + DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][2]);
                GUILayout.EndHorizontal();
                GUILayout.EndHorizontal();

                GUILayout.Label("返老还童之时，需以「血露」调和阴阳，方可避免走火入魔。");
                GUILayout.BeginHorizontal("Box");
                int bloodNum = 0;
                for (int i = 0; i < 9; i++)
                {
                    if (DateFile.instance.actorItemsDate[DateFile.instance.mianActorId].ContainsKey(21 + i))
                    {
                        bloodNum += DateFile.instance.actorItemsDate[DateFile.instance.mianActorId][21 + i];
                    }
                }
                GUILayout.Label("持有血露：" + bloodNum.ToString() + "/30");
                GUILayout.EndHorizontal();
            }
        }
    }

    [HarmonyPatch(typeof(UIDate), "TrunChange")]
    public static class UIDate_TrunChange_Patch
    {
        private static void Prefix()
        {
            if (!Main.enabled)
            {
                return;
            }

            if (!DateFile.instance.actorGongFas[DateFile.instance.mianActorId].ContainsKey(150369))
            {
                int[] allQi = DateFile.instance.GetActorAllQi(DateFile.instance.mianActorId);
                if (allQi[0] + allQi[1] + allQi[2] + allQi[3] + allQi[4] >= 500 && DateFile.instance.GetActorValue(DateFile.instance.mianActorId, 601, true) - int.Parse(DateFile.instance.GetActorDate(DateFile.instance.mianActorId, 601, true)) >= 100)
                {
                    DateFile.instance.ChangeActorGongFa(DateFile.instance.mianActorId, 150369, 50, 0, 0, false);
                }
                if (DateFile.instance.actorGongFas[DateFile.instance.mianActorId].ContainsKey(150002))
                {
                    int[] bookPages = (!DateFile.instance.gongFaBookPages.ContainsKey(150002)) ? new int[10] : DateFile.instance.gongFaBookPages[150002];
                    for (int i = 0; i < 10; i++)
                    {
                        if (bookPages[i] == 0)
                        {
                            return;
                        }
                    }
                    if (DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150002][0] == 100)
                    {
                        DateFile.instance.ChangeActorGongFa(DateFile.instance.mianActorId, 150369, 0, 1, 0, true);
                    }
                }
            }
            else
            {
                if (Main.setting.rejuvenatedAge == 0)
                {
                    if (DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0] >= 25 && DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0] < 100
                        && DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0] / 25 * 30 < int.Parse(DateFile.instance.actorsDate[DateFile.instance.mianActorId][11]))
                    {
                        DateFile.instance.RemoveMainActorEquipGongFa(150369);
                        DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0]--;
                    }
                }
                else
                {
                    Main.setting.rejuvenatedAge = Mathf.Min(DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0] / 25, Main.setting.rejuvenatedAge);
                    Main.setting.rejuvenationAge = Mathf.Min(Main.setting.rejuvenationAge, 6 + DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][1] + DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][2]);
                    Main.setting.changedGongFaFLevel = Mathf.Min(Main.setting.changedGongFaFLevel, DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][2]);
                    Main.setting.changedGongFaFLevel = Mathf.Max(Main.setting.changedGongFaFLevel, -1 * DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][1]);


                    if (int.Parse(DateFile.instance.actorsDate[DateFile.instance.mianActorId][11]) >= Main.setting.rejuvenatedAge * 30 + 6)
                    {
                        int[] gongFa = new int[] {
                            DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0],
                            DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][1] + Main.setting.changedGongFaFLevel,
                            DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][2] - Main.setting.changedGongFaFLevel};

                        Main.setting.changedGongFaFLevel = 0;

                        DateFile.instance.RemoveMainActorEquipGongFa(150369);
                        DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0] = 0;
                        DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][1] = 0;
                        DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][2] = 0;


                        DateFile.instance.actorsDate[DateFile.instance.mianActorId][11] = Main.setting.rejuvenationAge.ToString();


                        for (int i = 0; i < 6; i++)
                        {
                            if (Random.Range(1, 101) <= gongFa[0])
                            {
                                int num = Random.Range(0, 6);
                                DateFile.instance.actorsDate[DateFile.instance.mianActorId][61 + num] = (int.Parse(DateFile.instance.actorsDate[DateFile.instance.mianActorId][61 + num]) + 1).ToString();
                            }
                        }
                        for (int i = 0; i < 16; i++)
                        {
                            if (Random.Range(1, 101) <= gongFa[0])
                            {
                                int num = Random.Range(0, 16);
                                DateFile.instance.actorsDate[DateFile.instance.mianActorId][501 + num] = (int.Parse(DateFile.instance.actorsDate[DateFile.instance.mianActorId][501 + num]) + 1).ToString();
                            }
                        }
                        for (int i = 0; i < 14; i++)
                        {
                            if (Random.Range(1, 101) <= gongFa[0])
                            {
                                int num = Random.Range(0, 14);
                                DateFile.instance.actorsDate[DateFile.instance.mianActorId][601 + num] = (int.Parse(DateFile.instance.actorsDate[DateFile.instance.mianActorId][601 + num]) + 1).ToString();
                            }
                        }


                        int exp = DateFile.instance.GetActorValue(DateFile.instance.mianActorId, 601, true) - int.Parse(DateFile.instance.GetActorDate(DateFile.instance.mianActorId, 601, true));
                        int expNeed = 10;
                        while (exp >= expNeed)
                        {
                            exp -= expNeed;
                            expNeed += 10;
                            if (DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][2] < gongFa[2])
                            {
                                DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][2]++;
                            }
                            else
                            {
                                DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][1]++;
                            }
                        }
                        int readPageNum = 0;
                        int[] bookPages = (!DateFile.instance.gongFaBookPages.ContainsKey(150002)) ? new int[10] : DateFile.instance.gongFaBookPages[150002];
                        for (int i = 0; i < 10; i++)
                        {
                            if (bookPages[i] == 0)
                            {
                                readPageNum++;
                            }
                        }
                        if (DateFile.instance.actorGongFas[DateFile.instance.mianActorId].ContainsKey(150002))
                        {
                            if (readPageNum == 10 && DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150002][0] == 100)
                            {
                                if (DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][2] < gongFa[2])
                                {
                                    DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][2]++;
                                }
                                else
                                {
                                    DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][1]++;
                                }
                            }
                        }


                        int bloodNeed = 30;
                        for (int i = 0; i < 9; i++)
                        {
                            if (DateFile.instance.actorItemsDate[DateFile.instance.mianActorId].ContainsKey(21 + i) && bloodNeed > 0)
                            {
                                if (bloodNeed >= DateFile.instance.actorItemsDate[DateFile.instance.mianActorId][21 + i])
                                {
                                    bloodNeed -= DateFile.instance.actorItemsDate[DateFile.instance.mianActorId][21 + i];
                                    DateFile.instance.actorItemsDate[DateFile.instance.mianActorId][21 + i] = 0;
                                }
                                else
                                {
                                    DateFile.instance.actorItemsDate[DateFile.instance.mianActorId][21 + i] -= bloodNeed;
                                    bloodNeed = 0;
                                }
                            }
                        }
                        for (int i = 1, j = 0; i <= bloodNeed; i++)
                        {
                            List<string> list2 = new List<string>();
                            if (i <= 5 / 2)
                            {
                                j = 1;
                                list2 = new List<string>(DateFile.instance.bodyInjuryDate[9][11].Split(new char[] { '|' }));
                            }
                            else if (i <= 5)
                            {
                                j = 2;
                                list2 = new List<string>(DateFile.instance.bodyInjuryDate[9][12].Split(new char[] { '|' }));
                            }
                            else
                            {
                                j = 3;
                                list2 = new List<string>(DateFile.instance.bodyInjuryDate[9][13].Split(new char[] { '|' }));
                            }
                            int id = int.Parse(list2[Random.Range(0, list2.Count)]);
                            int num10 = i * i;
                            ActorMenu.instance.ChangeMianQi(DateFile.instance.mianActorId, j * num10, 5);
                            DateFile.instance.AddInjury(DateFile.instance.mianActorId, id, num10, false);
                        }
                    }
                }
            }
        }
    }
}
