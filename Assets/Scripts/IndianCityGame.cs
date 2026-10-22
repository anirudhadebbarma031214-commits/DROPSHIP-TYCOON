using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class IndianCityGame : MonoBehaviour
{
    CharacterController controller; Transform player; Camera cam;
    TextMeshProUGUI hud, notice, questText;
    float stamina=100, messageTimer; int cash=10000, blessings, deliveries, diyas;
    Vector2 moveInput; bool sprint, driving, diwali=true;
    readonly List<Vector3> fastTravel=new List<Vector3>();
    readonly Color[] palette={new Color(.92f,.63f,.32f),new Color(.38f,.68f,.83f),new Color(.91f,.82f,.55f),new Color(.55f,.73f,.49f),new Color(.82f,.53f,.67f)};
    void Start(){Application.targetFrameRate=60;BuildWorld();BuildPlayer();BuildUI();ResetDaily();Refresh("Welcome to Naya Nagar! Explore, work, shop and celebrate Diwali.");}
    void Update(){
        if(controller==null)return; Vector2 input=moveInput;
#if UNITY_EDITOR || UNITY_STANDALONE
        input+=new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical")); sprint=Input.GetKey(KeyCode.LeftShift);
        if(Input.GetKeyDown(KeyCode.E))Interact();
#endif
        input=Vector2.ClampMagnitude(input,1);bool running=sprint&&stamina>1&&input.sqrMagnitude>.01f&&!driving;
        float spd=driving?10f:(running?8f:4.5f);
        stamina=running?Mathf.Max(0,stamina-23*Time.deltaTime):Mathf.Min(100,stamina+(input.sqrMagnitude<.01f?18:10)*Time.deltaTime);
        Vector3 dir=player.forward*input.y+player.right*input.x;dir.y=0;
        if(dir.sqrMagnitude>.01f&&!driving)player.rotation=Quaternion.Slerp(player.rotation,Quaternion.LookRotation(dir),12*Time.deltaTime);
        controller.Move((dir.normalized*spd+Vector3.down*9f)*Time.deltaTime);
        if(driving)player.position+=player.forward*input.y*spd*Time.deltaTime;
        cam.transform.position=Vector3.Lerp(cam.transform.position,player.position+new Vector3(0,5.2f,-7.5f),8*Time.deltaTime);cam.transform.LookAt(player.position+Vector3.up*1.4f);
        if(messageTimer>0)messageTimer-=Time.deltaTime;RefreshHUD();
    }
    void BuildWorld(){
        RenderSettings.ambientLight=new Color(.72f,.72f,.72f);RenderSettings.fog=true;RenderSettings.fogColor=new Color(.66f,.82f,.91f);RenderSettings.fogDensity=.003f;
        GameObject sunObj=new GameObject("Sunny Indian City Lighting");Light sun=sunObj.AddComponent<Light>();sun.type=LightType.Directional;sun.intensity=1.15f;sunObj.transform.rotation=Quaternion.Euler(48,-35,0);
        Make("Grassland",PrimitiveType.Plane,new Vector3(0,-.15f,0),new Vector3(12,1,12),new Color(.35f,.65f,.32f));
        Make("Main Road",PrimitiveType.Cube,new Vector3(0,.01f,0),new Vector3(14,.12f,150),new Color(.22f,.24f,.26f));
        Make("Cross Road",PrimitiveType.Cube,new Vector3(0,.02f,0),new Vector3(150,.12f,14),new Color(.22f,.24f,.26f));
        for(int i=-6;i<=6;i++){Make("Road lane",PrimitiveType.Cube,new Vector3(0,.09f,i*10),new Vector3(.12f,.02f,4),Color.white);Make("Road lane",PrimitiveType.Cube,new Vector3(i*10,.09f,0),new Vector3(4,.02f,.12f),Color.white);}
        for(int x=-5;x<=5;x++)for(int z=-5;z<=5;z++){if(Mathf.Abs(x)<=1||Mathf.Abs(z)<=1)continue;Vector3 p=new Vector3(x*10,0,z*10);float h=UnityEngine.Random.Range(3f,8f);
            Make("Indian City Building",PrimitiveType.Cube,p+Vector3.up*h/2,new Vector3(7,h,7),palette[UnityEngine.Random.Range(0,palette.Length)]);
            Make("Flat Roof",PrimitiveType.Cube,p+Vector3.up*(h+.08f),new Vector3(7.1f,.18f,7.1f),new Color(.86f,.78f,.62f));
            for(int wy=1;wy<Mathf.FloorToInt(h/1.6f);wy++)for(int wx=-1;wx<=1;wx++)Make("Window",PrimitiveType.Cube,p+new Vector3(wx*1.7f,wy*1.6f,3.54f),new Vector3(.8f,.65f,.08f),new Color(.2f,.65f,.8f));}
        Landmark("CITY MALL 1",new Vector3(-24,0,24),new Color(.92f,.36f,.22f),3.5f);Landmark("CITY MALL 2",new Vector3(24,0,24),new Color(.26f,.55f,.87f),3.5f);Landmark("CITY MALL 3",new Vector3(24,0,-24),new Color(.65f,.35f,.82f),3.5f);
        Landmark("CENTRAL STADIUM",new Vector3(-35,0,-35),new Color(.3f,.65f,.32f),5f);Landmark("MANDIR TEMPLE",new Vector3(35,0,-35),new Color(1f,.67f,.18f),3f);Landmark("BUS AUTO STAND",new Vector3(-35,0,35),new Color(.18f,.68f,.62f),2.5f);
        string[] food={"Pizza Corner","Burger Hub","Noodle House","Chai Stall","Mithai Bhandar","Dosa Point","Biryani House","Ice Cream Shop","Street Momos","Juice Bar"};
        for(int i=0;i<food.Length;i++){Vector3 p=new Vector3((i%5-2)*13,0,(i/5==0?48:-48));Make(food[i],PrimitiveType.Cube,p+Vector3.up*1.3f,new Vector3(5,2.6f,4),palette[i%palette.Length]);Label(food[i],p+Vector3.up*3.1f);}
        for(int i=0;i<6;i++){Vector3 p=new Vector3(-48+i*19,0,0);fastTravel.Add(p);Make("Bus Stop",PrimitiveType.Cylinder,p+Vector3.up*.7f,new Vector3(1.2f,1.4f,1.2f),new Color(.1f,.65f,.8f));}
        for(int i=0;i<18;i++){Vector3 p=new Vector3(UnityEngine.Random.Range(-60,60),.25f,UnityEngine.Random.Range(-60,60));if(Mathf.Abs(p.x)<10||Mathf.Abs(p.z)<10)continue;Make("Diwali Diya",PrimitiveType.Sphere,p,new Vector3(.35f,.12f,.35f),new Color(1f,.48f,.04f));}
        Make("Starter Scooter",PrimitiveType.Cube,new Vector3(3,.55f,5),new Vector3(1.1f,.8f,2),new Color(.9f,.12f,.12f));Make("Personal Car",PrimitiveType.Cube,new Vector3(6,.7f,5),new Vector3(2,1.2f,3.5f),new Color(.1f,.35f,.9f));
    }
    void Landmark(string title,Vector3 p,Color c,float s){Make(title,PrimitiveType.Cube,p+Vector3.up*s,new Vector3(10,s*2,8),c);Make(title+" roof",PrimitiveType.Cube,p+Vector3.up*(s*2+.1f),new Vector3(10.5f,.25f,8.5f),new Color(.95f,.8f,.55f));Label(title,p+Vector3.up*(s*2+1.1f));}
    GameObject Make(string n,PrimitiveType type,Vector3 p,Vector3 s,Color c){GameObject g=GameObject.CreatePrimitive(type);g.name=n;g.transform.position=p;g.transform.localScale=s;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.material.color=c;return g;}
    void Label(string text,Vector3 p){GameObject g=new GameObject(text+" Sign");g.transform.position=p;TextMesh tm=g.AddComponent<TextMesh>();tm.text=text;tm.fontSize=48;tm.characterSize=.12f;tm.anchor=TextAnchor.MiddleCenter;tm.color=Color.white;}
    void BuildPlayer(){GameObject go=new GameObject("Player Avatar");go.transform.position=new Vector3(0,1,0);player=go.transform;controller=go.AddComponent<CharacterController>();controller.height=1.8f;controller.radius=.35f;controller.center=new Vector3(0,.9f,0);
        GameObject body=Make("Avatar Body",PrimitiveType.Capsule,player.position+Vector3.up*.85f,new Vector3(.7f,.85f,.7f),new Color(.18f,.5f,.95f));body.transform.SetParent(player,true);
        GameObject head=Make("Avatar Head",PrimitiveType.Sphere,player.position+Vector3.up*1.8f,new Vector3(.48f,.48f,.48f),new Color(.86f,.62f,.43f));head.transform.SetParent(player,true);
        GameObject cg=new GameObject("Third Person Camera");cam=cg.AddComponent<Camera>();cam.fieldOfView=65;cg.AddComponent<AudioListener>();cam.transform.position=player.position+new Vector3(0,5,-7);cam.transform.LookAt(player.position+Vector3.up);}
    void BuildUI(){
        if(FindObjectOfType<EventSystem>()==null)new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));
        GameObject co=new GameObject("City HUD",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));Canvas c=co.GetComponent<Canvas>();c.renderMode=RenderMode.ScreenSpaceOverlay;CanvasScaler cs=co.GetComponent<CanvasScaler>();cs.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;cs.referenceResolution=new Vector2(1080,1920);
        hud=Text(co.transform,"",26,TextAlignmentOptions.TopLeft,new Vector2(.02f,.98f),new Vector2(430,220));questText=Text(co.transform,"",23,TextAlignmentOptions.TopLeft,new Vector2(.98f,.98f),new Vector2(420,210));notice=Text(co.transform,"",28,TextAlignmentOptions.Center,new Vector2(.5f,.62f),new Vector2(850,100));
        Button(co.transform,"SPRINT",()=>sprint=true,new Vector2(.84f,.19f),new Vector2(180,95));Button(co.transform,"WALK",()=>sprint=false,new Vector2(.84f,.1f),new Vector2(180,85));
        Button(co.transform,"◀",()=>moveInput=new Vector2(-1,0),new Vector2(.12f,.13f),new Vector2(100,95));Button(co.transform,"▶",()=>moveInput=new Vector2(1,0),new Vector2(.32f,.13f),new Vector2(100,95));Button(co.transform,"▲",()=>moveInput=new Vector2(0,1),new Vector2(.22f,.2f),new Vector2(100,95));Button(co.transform,"▼",()=>moveInput=new Vector2(0,-1),new Vector2(.22f,.07f),new Vector2(100,85));Button(co.transform,"RELEASE MOVE",()=>moveInput=Vector2.zero,new Vector2(.22f,.28f),new Vector2(200,75));
        Button(co.transform,"INTERACT",Interact,new Vector2(.72f,.3f),new Vector2(210,95));Button(co.transform,"JOB / QUEST",DoJob,new Vector2(.72f,.4f),new Vector2(230,90));Button(co.transform,"TRAVEL",Travel,new Vector2(.72f,.5f),new Vector2(200,90));Button(co.transform,"SHOP",Shop,new Vector2(.72f,.6f),new Vector2(200,90));Button(co.transform,"DIWALI",Festival,new Vector2(.72f,.7f),new Vector2(200,90));
    }
    TextMeshProUGUI Text(Transform p,string value,float size,TextAlignmentOptions align,Vector2 anchor,Vector2 dims){GameObject g=new GameObject("HUD Text",typeof(TextMeshProUGUI));g.transform.SetParent(p,false);var t=g.GetComponent<TextMeshProUGUI>();t.text=value;t.fontSize=size;t.color=Color.white;t.alignment=align;t.textWrappingMode=TextWrappingModes.Normal;RectTransform r=t.rectTransform;r.anchorMin=anchor;r.anchorMax=anchor;r.pivot=anchor;r.anchoredPosition=Vector2.zero;r.sizeDelta=dims;return t;}
    void Button(Transform p,string label,Action action,Vector2 anchor,Vector2 size){GameObject g=new GameObject(label,typeof(Image),typeof(Button));g.transform.SetParent(p,false);RectTransform r=g.GetComponent<RectTransform>();r.anchorMin=anchor;r.anchorMax=anchor;r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=Vector2.zero;r.sizeDelta=size;g.GetComponent<Image>().color=new Color(.04f,.28f,.42f,.92f);g.GetComponent<Button>().onClick.AddListener(()=>action());GameObject tg=new GameObject("Label",typeof(TextMeshProUGUI));tg.transform.SetParent(g.transform,false);var t=tg.GetComponent<TextMeshProUGUI>();t.text=label;t.fontSize=25;t.alignment=TextAlignmentOptions.Center;t.color=Color.white;RectTransform tr=t.rectTransform;tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=Vector2.zero;tr.offsetMax=Vector2.zero;}
    void Refresh(string msg){notice.text=msg;messageTimer=4;RefreshHUD();}
    void RefreshHUD(){if(hud!=null)hud.text="NAYA NAGAR\n₹ "+cash.ToString("N0")+"\nSTAMINA "+Mathf.RoundToInt(stamina)+"%\nDELIVERIES "+deliveries+"\nBLESSINGS "+blessings+"\n"+(diwali?"DIWALI NIGHT":"CITY DAY");
        if(notice!=null&&messageTimer<=0)notice.text="";if(questText!=null)questText.text="DAILY QUESTS\n"+(deliveries>=3?"✓":"□")+" Deliver 3 orders\n"+(blessings>0?"✓":"□")+" Visit a Mandir\n"+(PlayerPrefs.GetInt("city_shop",0)>0?"✓":"□")+" Buy from a shop\n"+(diwali?"DIWALI: Light 10 diyas":"Explore the city");}
    void DoJob(){cash+=450;deliveries++;PlayerPrefs.SetInt("city_deliveries",deliveries);if(deliveries>=3){cash+=1500;Refresh("Delivery complete! +₹450. Quest bonus ₹1,500!");}else Refresh("Delivery job complete! +₹450. "+(3-deliveries)+" more for bonus.");}
    void Shop(){if(cash<120){Refresh("You need ₹120 for snacks.");return;}cash-=120;stamina=Mathf.Min(100,stamina+35);PlayerPrefs.SetInt("city_shop",1);Refresh("Kirana snack bought for ₹120. Stamina restored!");}
    void Interact(){if(Vector3.Distance(player.position,new Vector3(35,0,-35))<18){blessings++;PlayerPrefs.SetInt("city_temple",1);stamina=100;Refresh("Mandir blessing received! Stamina restored.");}
        else if(Vector3.Distance(player.position,new Vector3(3,0,5))<8||Vector3.Distance(player.position,new Vector3(6,0,5))<8){driving=!driving;Refresh(driving?"Vehicle mode enabled!":"You left your vehicle.");}
        else if(diwali){diyas++;cash+=50;if(diyas>=10){cash+=1000;Refresh("Diwali quest complete! Bonus ₹1,000.");}else Refresh("Diya lit! +₹50 ("+diyas+"/10)");}else Refresh("Explore landmarks, shops or jobs.");}
    void Travel(){if(fastTravel.Count==0)return;Vector3 p=fastTravel[UnityEngine.Random.Range(0,fastTravel.Count)];controller.enabled=false;player.position=new Vector3(p.x,1,p.z);controller.enabled=true;Refresh("Fast travel to public bus / auto stand.");}
    void Festival(){diwali=!diwali;RenderSettings.ambientLight=diwali?new Color(.28f,.22f,.42f):new Color(.72f,.72f,.72f);Refresh(diwali?"Cool Diwali Update enabled! Light diyas and distribute mithai.":"Daytime city mode enabled.");}
    void ResetDaily(){string today=DateTime.Now.ToString("yyyyMMdd");if(PlayerPrefs.GetString("city_daily_day","")!=today){PlayerPrefs.SetString("city_daily_day",today);PlayerPrefs.SetInt("city_deliveries",0);PlayerPrefs.SetInt("city_temple",0);PlayerPrefs.SetInt("city_shop",0);PlayerPrefs.Save();}deliveries=PlayerPrefs.GetInt("city_deliveries",0);}
}