using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IndianCityGame : MonoBehaviour
{
    CharacterController playerController;
    Transform player;
    Camera gameCamera;
    TextMeshProUGUI status, quests;
    int rupees = 10000, deliveries, blessings;
    float stamina = 100f;
    Vector2 move;
    bool sprinting, festival = true;
    readonly List<Vector3> stops = new List<Vector3>();

    void Start()
    {
        Application.targetFrameRate = 60;
        RenderSettings.ambientLight = Color.gray;
        GameObject sun = new GameObject("City Sun");
        sun.AddComponent<Light>().type = LightType.Directional;
        sun.transform.rotation = Quaternion.Euler(45, -30, 0);
        Cube("Green Ground", new Vector3(0,-.2f,0), new Vector3(150,.3f,150), new Color(.3f,.6f,.3f));
        Cube("Main Road", Vector3.zero, new Vector3(14,.12f,150), Color.gray);
        Cube("Cross Road", Vector3.zero, new Vector3(150,.12f,14), Color.gray);
        Color[] colors = { Color.cyan, Color.yellow, new Color(1,.4f,.3f), new Color(.6f,.4f,1) };
        for (int x=-5; x<=5; x++) for (int z=-5; z<=5; z++)
        {
            if (Mathf.Abs(x)<2 || Mathf.Abs(z)<2) continue;
            float h = UnityEngine.Random.Range(3f,8f);
            Cube("City Building", new Vector3(x*10,h/2,z*10), new Vector3(7,h,7), colors[UnityEngine.Random.Range(0,colors.Length)]);
        }
        Landmark("MALL 1", new Vector3(-24,0,24), Color.red);
        Landmark("MALL 2", new Vector3(24,0,24), Color.blue);
        Landmark("MALL 3", new Vector3(24,0,-24), Color.magenta);
        Landmark("STADIUM", new Vector3(-35,0,-35), Color.green);
        Landmark("MANDIR", new Vector3(35,0,-35), new Color(1,.6f,.1f));
        string[] shops = {"Pizza Corner","Burger Hub","Noodle House","Chai Stall","Mithai Bhandar","Dosa Point","Biryani House","Ice Cream","Street Momos","Juice Bar"};
        for (int i=0;i<shops.Length;i++) Landmark(shops[i],new Vector3((i%5-2)*13,0,i<5?48:-48),colors[i%colors.Length]);
        for (int i=0;i<6;i++) stops.Add(new Vector3(-48+i*19,1,0));
        Cube("Scooter",new Vector3(3,.5f,5),new Vector3(1,1,2),Color.red);
        Cube("Car",new Vector3(6,.6f,5),new Vector3(2,1.2f,3.5f),Color.blue);
        GameObject avatar = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        avatar.name = "Player Avatar"; avatar.transform.position = new Vector3(0,1,0); player = avatar.transform;
        playerController = avatar.AddComponent<CharacterController>();
        playerController.height=1.8f; playerController.center=new Vector3(0,.9f,0);
        gameCamera = new GameObject("Third Person Camera").AddComponent<Camera>();
        gameCamera.transform.position=player.position+new Vector3(0,5,-7);
        BuildUI(); Refresh("Welcome to Naya Nagar!");
    }
    void Update()
    {
        Vector2 input=move;
#if UNITY_EDITOR || UNITY_STANDALONE
        input += new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
        sprinting=Input.GetKey(KeyCode.LeftShift);
#endif
        input=Vector2.ClampMagnitude(input,1);
        bool run=sprinting&&stamina>0&&input.sqrMagnitude>.01f;
        stamina=Mathf.Clamp(stamina+(run?-23:input.sqrMagnitude<.01f?18:10)*Time.deltaTime,0,100);
        Vector3 dir=player.forward*input.y+player.right*input.x; dir.y=0;
        if(dir.sqrMagnitude>.01f) player.rotation=Quaternion.Slerp(player.rotation,Quaternion.LookRotation(dir),10*Time.deltaTime);
        playerController.Move((dir.normalized*(run?7.5f:4.5f)+Vector3.down*9)*Time.deltaTime);
        gameCamera.transform.position=Vector3.Lerp(gameCamera.transform.position,player.position+new Vector3(0,5,-7),8*Time.deltaTime);
        gameCamera.transform.LookAt(player.position+Vector3.up);
        status.text="NAYA NAGAR\n₹ "+rupees.ToString("N0")+"\nSTAMINA "+Mathf.RoundToInt(stamina)+"%\nDELIVERIES "+deliveries+"\n"+(festival?"DIWALI EVENT":"CITY DAY");
        quests.text="DAILY QUESTS\n"+(deliveries>=3?"✓":"□")+" Deliver 3 orders\n"+(blessings>0?"✓":"□")+" Visit Mandir\n"+(festival?"Light Diwali diyas":"Explore city");
    }
    void Cube(string n,Vector3 p,Vector3 s,Color c){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.position=p;g.transform.localScale=s;g.GetComponent<Renderer>().material.color=c;}
    void Landmark(string n,Vector3 p,Color c){Cube(n,p+Vector3.up*2,new Vector3(9,4,7),c);GameObject sign=new GameObject(n+" Sign");sign.transform.position=p+Vector3.up*4.5f;TextMesh t=sign.AddComponent<TextMesh>();t.text=n;t.fontSize=36;t.characterSize=.12f;t.anchor=TextAnchor.MiddleCenter;}
    void BuildUI()
    {
        if(FindObjectOfType<EventSystem>()==null)new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));
        GameObject g=new GameObject("HUD",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
        Canvas canvas=g.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler=g.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1080,1920);
        status=MakeText(g.transform,new Vector2(.02f,.98f),new Vector2(420,230),TextAlignmentOptions.TopLeft);
        quests=MakeText(g.transform,new Vector2(.98f,.98f),new Vector2(400,180),TextAlignmentOptions.TopRight);
        AddButton(g.transform,"◀",()=>move=new Vector2(-1,0),new Vector2(.12f,.15f));
        AddButton(g.transform,"▶",()=>move=new Vector2(1,0),new Vector2(.32f,.15f));
        AddButton(g.transform,"▲",()=>move=new Vector2(0,1),new Vector2(.22f,.22f));
        AddButton(g.transform,"▼",()=>move=new Vector2(0,-1),new Vector2(.22f,.07f));
        AddButton(g.transform,"STOP",()=>move=Vector2.zero,new Vector2(.22f,.29f));
        AddButton(g.transform,"SPRINT",()=>sprinting=!sprinting,new Vector2(.82f,.18f));
        AddButton(g.transform,"JOB +₹450",()=>{rupees+=450;deliveries++;},new Vector2(.78f,.4f));
        AddButton(g.transform,"SHOP ₹120",()=>{if(rupees>=120){rupees-=120;stamina=Mathf.Min(100,stamina+35);}},new Vector2(.78f,.5f));
        AddButton(g.transform,"TRAVEL",()=>{Vector3 p=stops[UnityEngine.Random.Range(0,stops.Count)];playerController.enabled=false;player.position=p;playerController.enabled=true;},new Vector2(.78f,.6f));
        AddButton(g.transform,"DIWALI",()=>{festival=!festival;RenderSettings.ambientLight=festival?new Color(.3f,.25f,.45f):Color.gray;},new Vector2(.78f,.7f));
        AddButton(g.transform,"MANDIR BLESSING",()=>{blessings++;stamina=100;rupees+=100;},new Vector2(.78f,.3f));
    }
    TextMeshProUGUI MakeText(Transform p,Vector2 anchor,Vector2 size,TextAlignmentOptions align){GameObject g=new GameObject("HUD Text",typeof(TextMeshProUGUI));g.transform.SetParent(p,false);TextMeshProUGUI t=g.GetComponent<TextMeshProUGUI>();t.fontSize=25;t.color=Color.white;t.alignment=align;RectTransform r=t.rectTransform;r.anchorMin=anchor;r.anchorMax=anchor;r.pivot=anchor;r.anchoredPosition=Vector2.zero;r.sizeDelta=size;return t;}
    void AddButton(Transform p,string label,Action action,Vector2 anchor){GameObject g=new GameObject(label,typeof(Image),typeof(Button));g.transform.SetParent(p,false);RectTransform r=g.GetComponent<RectTransform>();r.anchorMin=anchor;r.anchorMax=anchor;r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=Vector2.zero;r.sizeDelta=new Vector2(210,90);g.GetComponent<Image>().color=new Color(.03f,.25f,.4f,.95f);g.GetComponent<Button>().onClick.AddListener(()=>action());GameObject tgo=new GameObject("Label",typeof(TextMeshProUGUI));tgo.transform.SetParent(g.transform,false);TextMeshProUGUI t=tgo.GetComponent<TextMeshProUGUI>();t.text=label;t.fontSize=23;t.color=Color.white;t.alignment=TextAlignmentOptions.Center;RectTransform tr=t.rectTransform;tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=Vector2.zero;tr.offsetMax=Vector2.zero;}
}
