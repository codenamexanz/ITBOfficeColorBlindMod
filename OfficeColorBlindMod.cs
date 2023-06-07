using System.IO;
using MelonLoader;
using UnityEngine;
using static UnityEngine.Object;
using UnityEditor;
using Il2Cpp;
using HarmonyLib;
using System.Reflection;
using Il2CppMirror;
using UnityEngine.Playables;

namespace OfficeColorBlindMod
{
    /*
     * OfficeColorBlindMod - Allows changing the 8 colors in Office Level
     */
    public class OfficeColorBlindMod : MelonMod
    {
        private static bool debug = false;
        
        #region Preferences
        private static MelonPreferences_Category category;
        private static MelonPreferences_Entry<Color32> blue;
        private static MelonPreferences_Entry<Color32> brown;
        private static MelonPreferences_Entry<Color32> gray;
        private static MelonPreferences_Entry<Color32> green;
        private static MelonPreferences_Entry<Color32> pink;
        private static MelonPreferences_Entry<Color32> red;
        private static MelonPreferences_Entry<Color32> white;
        private static MelonPreferences_Entry<Color32> yellow;
        private static MelonPreferences_Entry<Color32> colorVariable;
        private static MelonPreferences_Entry<Color32> colorChanger;
        #endregion

        #region normColors
        private static Color32 normBlue = new Color32(0, 117, 253, 255);
        private static Color32 normBrown = new Color32(118, 74, 59, 255);
        private static Color32 normGray = new Color32(106, 106, 106, 255);
        private static Color32 normGreen = new Color32(101, 197, 44, 255);
        private static Color32 normPink = new Color32(220, 0, 255, 255);
        private static Color32 normRed = new Color32(192, 32, 32, 255);
        private static Color32 normWhite = new Color32(255, 255, 255, 255);
        private static Color32 normYellow = new Color32(229, 216, 70, 255);
        #endregion

        #region colorButtons
        private static bool blueButton = true;
        private static bool brownButton = false;
        private static bool grayButton = false;
        private static bool greenButton = false;
        private static bool pinkButton = false;
        private static bool redButton = false;
        private static bool whiteButton = false;
        private static bool yellowButton = false;
        #endregion

        #region rgb
        private static byte rValue;
        private static byte gValue;
        private static byte bValue;
        #endregion

        public override void OnInitializeMelon()
        {
            category = MelonPreferences.CreateCategory("OfficeColorBlind Mod");
            blue = category.CreateEntry<Color32>("Blue", normBlue);
            brown = category.CreateEntry<Color32>("Brown", normBrown);
            gray = category.CreateEntry<Color32>("Gray", normGray);
            green = category.CreateEntry<Color32>("Green", normGreen);
            pink = category.CreateEntry<Color32>("Pink", normPink);
            red = category.CreateEntry<Color32>("Red", normRed);
            white = category.CreateEntry<Color32>("White", normWhite);
            yellow = category.CreateEntry<Color32>("Yellow", normYellow);
            colorVariable = blue;
            rValue = colorVariable.Value.r;
            gValue = colorVariable.Value.g;
            bValue = colorVariable.Value.b;
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName == "MainLevel")
            {
                MelonEvents.OnGUI.Unsubscribe(DrawColorBlindOptions);
                ChangeColors();
            }

            if (sceneName == "MainMenu")
            {
                MelonEvents.OnGUI.Subscribe(DrawColorBlindOptions, 100);
            }
        }

        public static MelonPreferences_Entry<Color32> GetColor(string label)
        {
            switch (label)
            {
                case "Blue":
                    return blue;
                case "Brown":
                    return brown;
                case "Gray":
                    return gray;
                case "Green":
                    return green;
                case "Pink":
                    return pink;
                case "Red":
                    return red;
                case "White":
                    return white;
                case "Yellow":
                    return yellow;
                case "RGB(0, 117, 253)":
                    return blue;
                case "RGB(118, 74, 59)":
                    return brown;
                case "RGB(106, 106, 106)":
                    return gray;
                case "RGB(101, 197, 44)":
                    return green;
                case "RGB(220, 0, 255)":
                    return pink;
                case "RGB(192, 32, 32)":
                    return red;
                case "RGB(255, 255, 255)":
                    return white;
                default:
                    return yellow;
            }
        }

        public void CreateButton(string label, ref bool boolVariable, int xColumn, int yRow, ref Color32 color)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

            if (boolVariable)
            {
                buttonStyle.normal.background = Texture2D.whiteTexture;
                buttonStyle.normal.textColor = Color.black;
            }
            else
            {
                buttonStyle.normal.background = GUI.skin.button.normal.background;
                buttonStyle.normal.textColor = GUI.skin.button.normal.textColor;
            }

            if (GUI.Button(new Rect(Screen.width - 405f + xColumn * 200f, 790f + yRow * 35f, 190f, 30f), label, buttonStyle))
            {
                if (boolVariable)
                {
                    SetAllButtonsFalse();
                } 
                else
                {
                    SetAllButtonsFalse();
                    boolVariable = true;
                    colorVariable = GetColor(label);
                    if (debug) MelonLogger.Msg(System.ConsoleColor.Cyan, "colorVariable swapped to " + label);
                    rValue = colorVariable.Value.r;
                    gValue = colorVariable.Value.g;
                    bValue = colorVariable.Value.b;
                }

                if (debug)
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, label + " Button Down");
                    MelonLogger.Msg(System.ConsoleColor.Green, label + " bool changed to " + boolVariable);
                }
            }

            if (boolVariable)
            {
                if (GUI.Button(new Rect(Screen.width - 405f, 965f, 390f, 30f), "Reset Color"))
                {

                    colorVariable.Value = color;
                    rValue = colorVariable.Value.r;
                    gValue = colorVariable.Value.g;
                    bValue = colorVariable.Value.b;
                    if (debug) MelonLogger.Msg(System.ConsoleColor.Magenta, "Reset " + label + " to " + colorVariable.Value.ToString());
                }
            }
        }

        public void DrawColorBlindOptions()
        {
            GUI.Box(new Rect(Screen.width - 410f, 770f, 400f, 300f), "Color Options");
            CreateButton("Blue", ref blueButton, 0, 0, ref normBlue);
            CreateButton("Brown", ref brownButton, 1, 0, ref normBrown);
            CreateButton("Gray", ref grayButton, 0, 1, ref normGray);
            CreateButton("Green", ref greenButton, 1, 1, ref normGreen);
            CreateButton("Red", ref redButton, 0, 2, ref normRed);
            CreateButton("Pink", ref pinkButton, 1, 2, ref normPink);
            CreateButton("White", ref whiteButton, 0, 3, ref normWhite);
            CreateButton("Yellow", ref yellowButton, 1, 3, ref normYellow);

            rValue = byte.Parse(GUI.TextField(new Rect(Screen.width - 405f, 930f, 125f, 30f), rValue.ToString()));
            gValue = byte.Parse(GUI.TextField(new Rect(Screen.width - 272f, 930f, 125f, 30f), gValue.ToString()));
            bValue = byte.Parse(GUI.TextField(new Rect(Screen.width - 140f, 930f, 125f, 30f), bValue.ToString()));

            colorVariable.Value = new Color32(rValue, gValue, bValue, 255);

            Texture2D backgroundTexture = new Texture2D(1, 1);
            backgroundTexture.SetPixel(0, 0, colorVariable.Value);
            backgroundTexture.Apply();
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.background = backgroundTexture;
            GUI.Box(new Rect(Screen.width - 405f, 1000f, 390f, 65f), "", boxStyle);
        }

        public static void SetAllButtonsFalse()
        {
            blueButton = false;
            brownButton = false;
            grayButton = false;
            greenButton = false;
            pinkButton = false;
            redButton = false;
            whiteButton = false;
            yellowButton = false;
        }

        public static void ChangeColors()
        {
            int i = 0;
            GameObject officeLevel = GameObject.Find("__BACKROOMS OFFICE (LEVEL3)");
            GameObject systems = officeLevel.transform.FindChild("SYSTEMS").gameObject;
            ComputersScreenPuzzle puzzleInstance = systems.GetComponent<ComputersScreenPuzzle>();
            if (puzzleInstance != null)
            {
                GameObject postits = GameObject.Find("PostitsPapers");
                if (postits != null)
                {
                    foreach (Il2CppSystem.Collections.Generic.KeyValuePair<int, Il2Cpp.ComputersScreenPuzzle.SyncComputerInfo> kvp in puzzleInstance.syncComputerInfos)
                    {
                        int currentIndex = kvp.Key;
                        if (debug) MelonLogger.Msg(System.ConsoleColor.Cyan, "Index is " + kvp.Key);
                        Material computerScreen = puzzleInstance.computers[currentIndex].baseMaterial;
                        if (computerScreen != null)
                        {
                            Color32 color = computerScreen.color;
                            string colorString = $"RGB({color.r.ToString()}, {color.g.ToString()}, {color.b.ToString()})";
                            GameObject postit = postits.transform.GetChild(i).gameObject;
                            MeshRenderer meshRenderer = postit.GetComponent<MeshRenderer>();
                            if (debug) MelonLogger.Msg(System.ConsoleColor.Green, colorString);
                            if (meshRenderer != null)
                            {
                                colorChanger = GetColor(colorString);
                                if (debug) MelonLogger.Msg(System.ConsoleColor.Magenta, "Color is " + colorChanger.DisplayName.ToString());
                                if (meshRenderer.material != null)
                                {
                                    computerScreen.color = colorChanger.Value;
                                    meshRenderer.material.color = colorChanger.Value;
                                }
                            }
                        }
                        i++;
                    }
                }
            }
        }
    }
}