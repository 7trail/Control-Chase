using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class KeyBindController : MonoBehaviour
{
    // Token: 0x060000EC RID: 236 RVA: 0x00005F98 File Offset: 0x00004198
    private void Start()
    {
        this.codes = new List<KeyCode>();
        foreach (char c in this.alphaC)
        {
            this.alpha.Add(c.ToString());
        }
        for (int i = 0; i < 20; i++)
        {
            this.alpha.Add("Joystick1Button" + i);
        }
        foreach (string value in this.alpha)
        {
            this.codes.Add((KeyCode)Enum.Parse(typeof(KeyCode), value));
        }
    }

    // Token: 0x060000ED RID: 237 RVA: 0x00006088 File Offset: 0x00004288
    private void Update()
    {
        this.ticks++;
        if (this.ticks > 1500)
        {
            this.ticks = 0;
            this.awaiting.SetActive(false);
        }
        if (this.currentKey != null)
        {
            for (int i = 0; i < this.codes.Count; i++)
            {
                if (Input.GetKeyDown(this.codes[i]) && this.ticks > 15)
                {
                    GameInputManager.SetKeyMap(this.currentKey.name, this.codes[i]);
                    this.awaiting.SetActive(false);
                    this.currentKey.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = this.codes[i].ToString();
                    this.currentKey = null;
                    
                }
            }
        }
    }

    // Token: 0x060000EE RID: 238 RVA: 0x00006133 File Offset: 0x00004333
    public void SetKey(GameObject clicked)
    {
        if (this.currentKey == null)
        {
            this.currentKey = clicked;
            this.awaiting.SetActive(true);
            this.ticks = 0;
        }
    }

    // Token: 0x040000ED RID: 237
    public GameObject awaiting;

    // Token: 0x040000EE RID: 238
    private GameObject currentKey;

    // Token: 0x040000EF RID: 239
    private List<string> alpha = new List<string>
    {
        "Mouse0",
        "Mouse1",
        "Mouse2",
        "LeftShift",
        "Alpha1",
        "Alpha2",
        "Alpha3",
        "Alpha4",
        "Alpha5",
        "Alpha6",
        "Alpha7",
        "Alpha8",
        "Alpha9",
        "Alpha0",
        "Space",
        "Tab"
    };

    // Token: 0x040000F0 RID: 240
    private List<KeyCode> codes = new List<KeyCode>();

    // Token: 0x040000F1 RID: 241
    private int ticks;

    // Token: 0x040000F2 RID: 242
    private List<char> alphaC = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().ToList<char>();
}



public static class GameInputManager
{
    // Token: 0x060000F1 RID: 241 RVA: 0x0000624E File Offset: 0x0000444E
    public static string Join<T>(this IEnumerable<T> source, string separator)
    {
        return string.Join<T>(separator, source);
    }

    // Token: 0x060000F2 RID: 242 RVA: 0x00006257 File Offset: 0x00004457
    public static void RebindAxis(string axis, string bind)
    {
        GameInputManager.axisBinds[GameInputManager.axes.IndexOf(axis)] = bind;
    }

    // Token: 0x060000F3 RID: 243 RVA: 0x0000626B File Offset: 0x0000446B
    public static void SaveAxes()
    {
        PlayerPrefs.SetString("Axes", GameInputManager.axes.Join("-"));
        PlayerPrefs.Save();
    }

    // Token: 0x060000F4 RID: 244 RVA: 0x0000628C File Offset: 0x0000448C
    static GameInputManager()
    {
        //KeyCode[] array = new KeyCode[12];
        //GameInputManager.defaults = array;
        GameInputManager.axes = new List<string>
        {
            "1-H",
            "1-V"
        };
        GameInputManager.axisBinds = new string[]
        {
            "Horizontal",
            "Vertical"
        };
        GameInputManager.invert = new List<string>();
        if (PlayerPrefs.HasKey("KeyBinds"))
        {
            try
            {
                KeyCode[] array2 = JsonUtility.FromJson<KeysHolder>(PlayerPrefs.GetString("KeyBinds")).keys.ToArray();
                if (array2.Length == GameInputManager.defaults.Length)
                {
                    GameInputManager.defaults = array2;
                }
            }
            catch
            {
            }
            try
            {
                List<string> list = PlayerPrefs.GetString("Axes", "").Split(new char[]
                {
                    '-'
                }).ToList<string>();
                if (list.Count == GameInputManager.axes.Count)
                {
                    GameInputManager.axes = list;
                }
            }
            catch
            {
            }
        }
        GameInputManager.InitializeDictionary();
    }

    // Token: 0x060000F5 RID: 245 RVA: 0x000065E4 File Offset: 0x000047E4
    private static void InitializeDictionary()
    {
        GameInputManager.keyMapping = new Dictionary<string, KeyCode>();
        for (int i = 0; i < GameInputManager.keyMaps.Length; i++)
        {
            GameInputManager.keyMapping.Add(GameInputManager.keyMaps[i], GameInputManager.defaults[i]);
        }
    }

    // Token: 0x060000F6 RID: 246 RVA: 0x00006625 File Offset: 0x00004825
    public static float GetAxis(string name)
    {
        return Input.GetAxis(GameInputManager.axisBinds[GameInputManager.axes.IndexOf(name)]) * (float)(GameInputManager.invert.Contains(name) ? -1 : 1);
    }

    // Token: 0x060000F7 RID: 247 RVA: 0x00006650 File Offset: 0x00004850
    public static void SetKeyMap(string keyMap, KeyCode key)
    {
        if (!GameInputManager.keyMapping.ContainsKey(keyMap))
        {
            throw new ArgumentException("Invalid KeyMap in SetKeyMap: " + keyMap);
        }
        GameInputManager.keyMapping[keyMap] = key;
        string text = JsonUtility.ToJson(new KeysHolder
        {
            keys = GameInputManager.keyMapping.Values.ToList<KeyCode>()
        });
        Debug.Log(text);
        PlayerPrefs.SetString("KeyBinds", text);
        PlayerPrefs.Save();
    }

    // Token: 0x060000F8 RID: 248 RVA: 0x000066BD File Offset: 0x000048BD
    public static KeyCode GetKeyMap(string keyMap)
    {
        if (!GameInputManager.keyMapping.ContainsKey(keyMap))
        {
            throw new ArgumentException("Invalid KeyMap in SetKeyMap: " + keyMap);
        }
        return GameInputManager.keyMapping[keyMap];
    }

    // Token: 0x060000F9 RID: 249 RVA: 0x000066E8 File Offset: 0x000048E8
    public static bool GetButtonDown(string keyMap)
    {
        return Input.GetButtonDown(keyMap) || GameInputManager.GetKeyDown(keyMap);
    }

    // Token: 0x060000FA RID: 250 RVA: 0x000066FA File Offset: 0x000048FA
    public static bool GetButton(string keyMap)
    {
        return Input.GetButton(keyMap) || GameInputManager.GetKey(keyMap);
    }

    // Token: 0x060000FB RID: 251 RVA: 0x0000670C File Offset: 0x0000490C
    public static bool GetKeyDown(string keyMap)
    {
        return Input.GetKeyDown(GameInputManager.keyMapping[keyMap]);
    }

    // Token: 0x060000FC RID: 252 RVA: 0x0000671E File Offset: 0x0000491E
    public static bool GetKey(string keyMap)
    {
        return Input.GetKey(GameInputManager.keyMapping[keyMap]);
    }

    public static bool GetKeyUp(string keyMap)
    {
        return Input.GetKeyUp(GameInputManager.keyMapping[keyMap]);
    }

    // Token: 0x040000F4 RID: 244
    public static Dictionary<string, KeyCode> keyMapping;

    // Token: 0x040000F5 RID: 245
    private static string[] keyMaps = new string[]
    {
        "1-Fire1",
        "2-Fire1",
        "3-Fire1",
        "4-Fire1",
        "1-Fire2",
        "2-Fire2",
        "3-Fire2",
        "4-Fire2",
        "1-Fire3",
        "2-Fire3",
        "3-Fire3",
        "4-Fire3",
    };

    // Token: 0x040000F6 RID: 246
    private static KeyCode[] defaults = new KeyCode[] {
        KeyCode.Mouse0,
        KeyCode.Joystick2Button0,
        KeyCode.Joystick3Button0,
        KeyCode.Joystick4Button0,
        KeyCode.Mouse1,
        KeyCode.Joystick2Button1,
        KeyCode.Joystick3Button1,
        KeyCode.Joystick4Button1,
        KeyCode.Mouse2,
        KeyCode.Joystick2Button2,
        KeyCode.Joystick3Button2,
        KeyCode.Joystick4Button2,
    };

    // Token: 0x040000F7 RID: 247
    public static List<string> axes;

    // Token: 0x040000F8 RID: 248
    public static string[] axisBinds;

    // Token: 0x040000F9 RID: 249
    public static List<string> invert;
}
[Serializable]
public class KeysHolder
{
    // Token: 0x040000F3 RID: 243
    public List<KeyCode> keys;
}