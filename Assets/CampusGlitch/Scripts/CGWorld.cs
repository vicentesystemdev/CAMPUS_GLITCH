using UnityEngine;
namespace CampusGlitch {
public class CGWorld : MonoBehaviour {
  public CGGame game; public CGLevelSpec spec;public int index;
  public Transform root;public CGHero hero;public CGEnemy boss;
  public float spawnX=2,checkpointX=2;
  public bool checkpoint=false;
  Texture2D solidTex;
  static string TilePath(int level){
    string[] tiles={"Floor_ExteriorStone","Floor_GrayVinyl","Floor_Technical","Floor_WhiteClinic","Floor_Technical","Floor_GrayVinyl","Floor_WoodGray","Floor_GreenTurf","Floor_WhiteClinic","Floor_WhiteClinic"};
    return "01_Nuevos_Modulares/00_Global/Arquitectura/"+tiles[level];
  }
  public void Setup(CGGame g,int level){game=g;index=level;spec=CGLevels.All[level];root=new GameObject("CG_Level"+level+"_Root").transform;root.SetParent(transform);
    Camera cam=Camera.main;
    if(cam==null){GameObject cg=new GameObject("Gameplay_Camera");cg.tag="MainCamera";cam=cg.AddComponent<Camera>();cam.orthographic=true;cam.orthographicSize=5.65f;cg.AddComponent<AudioListener>();}
    cam.orthographic=true;cam.orthographicSize=5.7f;cam.clearFlags=CameraClearFlags.SolidColor;cam.transform.rotation=Quaternion.identity;cam.transform.position=new Vector3(8f,2.2f,-10f);cam.backgroundColor=new Color(.055f,.10f,.18f);
    CreateBackground();
    AddArchitecture();
    AddAmbientDecorations();
    for(int x=0;x<73;x+=1){ if(x==20||x==44)continue;Platform(x+0.5f,-.5f,1f,1f, false); }
    // elevated paths provide alternate routes, with collisions independent of sprites
    // Different traversable platform sequences for each zone, with reasonable jump distances.
    float[][] routes = {
      new float[]{11,17,27,32,44,50,60},
      new float[]{14,22,29,36,44,55,64},
      new float[]{9,15,24,33,42,51,60},
      new float[]{14,19,27,37,45,53,61},
      new float[]{8,16,25,34,44,53,63},
      new float[]{12,20,29,39,48,58,65},
      new float[]{14,23,29,37,46,54,62},
      new float[]{7,15,22,31,42,50,60},
      new float[]{11,18,27,36,43,52,62},
      new float[]{10,19,26,34,43,52,62}
    };
    foreach(float x in routes[index]) Platform(x,1.7f+(x%3)*.17f,2.6f,.32f,true);
    foreach(float x in new float[]{19+index%4,43+index%5,59+index%3}) Platform(x,3.12f,2.0f,.28f,true);
    if(index==2||index==6||index==7){
       foreach(float x in new float[]{28,40,54})Platform(x,4.42f,2.2f,.28f,true);
    }
    if(index==9)foreach(float x in new float[]{18,28,38,48,58})Platform(x,4.0f,2.7f,.28f,true);
    for(int x=5;x<68;x+=4+(index%3)){
      int pidx=(x/4+index)%spec.props.Length;
      string p=spec.props[pidx];
      Prop(p,x,1.1f+(x%5==0?.06f:0),1.25f+(pidx%3)*.17f,false);
    }
    Prop("UNIFRANZ_Sign",2,3.8f,1.8f,false);
    PlaceSign("NIVEL "+(index==9?"EXTRA":(index+1).ToString())+" / "+spec.name, 3.7f,3.8f);
    for(int n=0;n<3;n++){int x=14+n*17;var item=new GameObject("Fragmento_Datos_"+(n+1));item.transform.SetParent(root);item.transform.position=new Vector3(x,n==1?3.35f:1.1f,0);item.AddComponent<CGPickup>().Setup(game,n);}
    SpawnNPC(8,spec.npc);
    SpawnAssistant();
    foreach(float x in new[]{18f,30f,51f})SpawnEnemy(x,index>=4 && x==51?"Centinela":"Bug_Terrestre",false);
    if(index>=2)SpawnEnemy(38,"Bug_Saltador",false);
    if(!string.IsNullOrEmpty(spec.boss))SpawnEnemy(61,spec.boss,true);
    Prop("CheckpointPillar",35,1.1f,1.8f,true,"01_Nuevos_Modulares/Global_Gameplay/CheckpointPillar");
    Prop("TerminalHacking",40,1.05f,1.7f,true,"01_Nuevos_Modulares/Global_Gameplay/TerminalHacking");
    Prop("PortalTransition",69,1.2f,2.0f,true,"01_Nuevos_Modulares/Global_Gameplay/PortalTransition");
    var objectPlayer=new GameObject("PLAYER_Lobo_Guardian");objectPlayer.transform.SetParent(root);objectPlayer.transform.position=new Vector3(2,0,0);hero=objectPlayer.AddComponent<CGHero>();hero.Setup(game);
    var cameraFollow=cam.gameObject.GetComponent<CGCamera>(); if(cameraFollow==null)cameraFollow=cam.gameObject.AddComponent<CGCamera>();cameraFollow.follow=hero.transform;
  }
  void CreateBackground(){Sprite sky=CGArt.Get("Backgrounds/Background_"+index.ToString("00"));if(sky==null)return;
    for(int j=0;j<5;j++){GameObject obj=new GameObject("Fondo_Parallax_"+j);obj.transform.SetParent(root);var sr=obj.AddComponent<SpriteRenderer>();sr.sprite=sky;sr.sortingOrder=-100+j;obj.transform.position=new Vector3(j*19,3f,0);
     float w=sr.sprite.bounds.size.x;obj.transform.localScale=new Vector3(25/Mathf.Max(.1f,w),12/Mathf.Max(.1f,sr.sprite.bounds.size.y),1);
     CGParallax par=obj.AddComponent<CGParallax>();par.cameraTransform=Camera.main!=null?Camera.main.transform:null;par.startX=j*19;par.factor=.12f;
    }
  }
  void Platform(float x,float y,float w,float h,bool raised){var obj=new GameObject(raised?"Plataforma_Elevada":"Ground");obj.transform.SetParent(root);obj.transform.position=new Vector3(x,y,0);
    GameObject visual=new GameObject("Arte_Tile_2D");visual.transform.SetParent(obj.transform,false);
    var sr=visual.AddComponent<SpriteRenderer>();sr.sortingOrder=raised?5:6;
    sr.sprite=CGArt.Get(raised?"01_Nuevos_Modulares/00_Global/Arquitectura/Platform_Thick":TilePath(index));
    if(sr.sprite!=null)visual.transform.localScale=new Vector3(w/Mathf.Max(.01f,sr.sprite.bounds.size.x),h/Mathf.Max(.01f,sr.sprite.bounds.size.y),1);
    else {sr.sprite=MakeSolid(raised?new Color(.35f,.4f,.46f):new Color(.45f,.47f,.52f));visual.transform.localScale=new Vector3(w,h,1);}
    // Collider dimensions must be world units, never derived from sprite pixel size.
    BoxCollider2D col=obj.AddComponent<BoxCollider2D>();col.size=new Vector2(w,h);obj.AddComponent<CGSurface>();
  }
  // All elements are real SpriteRenderers in side view; objects and collision geometry remain separate.
  void Piece(Transform parent,string id,string asset,float x,float y,float width,float height,int sorting) {
    Sprite sprite=CGArt.Get(asset);
    if(sprite==null)return;
    GameObject part=new GameObject(id);part.transform.SetParent(parent,false);
    part.transform.localPosition=new Vector3(x,y,0);
    SpriteRenderer sr=part.AddComponent<SpriteRenderer>();sr.sprite=sprite;sr.sortingOrder=sorting;
    part.transform.localScale=new Vector3(width/Mathf.Max(.01f,sprite.bounds.size.x),height/Mathf.Max(.01f,sprite.bounds.size.y),1);
  }
  void AddArchitecture(){
    Transform backdrop=new GameObject("Arquitectura_2D_Modular").transform;
    backdrop.SetParent(root,false);
    string prefix="Environment/L"+index.ToString("00")+"_";
    // Interior levels have modular architecture rather than an exterior panorama pasted behind them.
    for(int block=0;block<12;block++){
      float x=block*6.4f;
      Piece(backdrop,"ARTE_ESPECIFICO_"+spec.key+"_"+block,"Themes/Level"+index.ToString("00")+"_Panel"+(block%6),x+2.2f,3.7f,6.4f,5.8f,-31);
      if(block%4==2 && index!=6 && index!=7) {
        Piece(backdrop,"Ventanal_"+block,prefix+"Window",x+2.2f,3.0f,3.0f,2.8f,-24);
        Piece(backdrop,"Iluminacion_"+block,prefix+"Light",x+4.9f,5.0f,.95f,.95f,-8);
      } else {
        Piece(backdrop,"Acceso_Modular_"+block,prefix+"Door",x+2.1f,1.1f,1.75f,2.75f,-20);
        Piece(backdrop,"Bandera_Campus_"+block,prefix+"Banner",x+5.1f,2.65f,1.15f,2.2f,-7);
      }
      if(block%2==0)Piece(backdrop,"Pilar_"+block,prefix+"Column",x+5.95f,2.80f,.52f,5.55f,-19);
      Piece(backdrop,"Zocalo_"+block,prefix+"FloorCap",x+2.2f,.0f,6.4f,.45f,-12);
      if(block%3==0 && index!=9)Piece(backdrop,"Maceta_"+block,prefix+"Plant",x+5.6f,.78f,1.0f,1.15f,9);
    }
    // Elevated decorative balconies: foregrounds and collisions are separate from these visuals.
    for(int x=10;x<67;x+=13){Piece(backdrop,"Pasamanos_Elevado_"+x,prefix+"Rail",x,2.70f,3.2f,.42f,8);}
  }
  void AddAmbientDecorations() {
    Transform extras=new GameObject("Efectos_Ambientales_2D").transform;
    extras.SetParent(root,false);
    string prefix="Environment/L"+index.ToString("00")+"_";
    for(int x=6;x<70;x+=9){
      GameObject fx=new GameObject("Brillo_Digital_"+x);fx.transform.SetParent(extras,false);
      fx.transform.position=new Vector3(x,3.6f,0);
      var sr=fx.AddComponent<SpriteRenderer>();sr.sprite=CGArt.Get(prefix+"Glow");sr.sortingOrder=18;
      fx.transform.localScale=Vector3.one*.3f;
      var flicker=fx.AddComponent<CGAmbientGlow>();flicker.seed=x;
    }
  }
  static Sprite fallSprite;
  static Sprite MakeSolid(Color c){if(fallSprite!=null)return fallSprite;Texture2D t=new Texture2D(1,1);t.SetPixel(0,0,c);t.Apply();fallSprite=Sprite.Create(t,new Rect(0,0,1,1),new Vector2(.5f,.5f),1);return fallSprite;}
  public void Prop(string name,float x,float y,float height,bool interactive,string fixedPath=null){Sprite s=!string.IsNullOrEmpty(fixedPath)?CGArt.Get(fixedPath):CGArt.Prop(spec,name);
    if(s==null)return;
    GameObject go=new GameObject("PROP_"+name);go.transform.SetParent(root);go.transform.position=new Vector3(x,y,0);SpriteRenderer sr=go.AddComponent<SpriteRenderer>();sr.sprite=s;sr.sortingOrder=interactive?16:3;
    float aspect=s.bounds.size.x/Mathf.Max(.01f,s.bounds.size.y);float visibleH=Mathf.Max(0.55f,height);go.transform.localScale=new Vector3(visibleH/Mathf.Max(.01f,s.bounds.size.y),visibleH/Mathf.Max(.01f,s.bounds.size.y),1);
    if(interactive){
      var inter=go.AddComponent<CGMarker>();inter.game=game;inter.kind=name;
      string clip=name=="CheckpointPillar"?"Checkpoint_Active":name=="TerminalHacking"?"Terminal_Screen":name=="PortalTransition"?"Portal_Transition":"";
      if(!string.IsNullOrEmpty(clip)){
         GameObject animated=new GameObject("EFECTO_ANIMADO_"+clip);
         animated.transform.SetParent(go.transform,false);
         animated.transform.localPosition=new Vector3(0,.13f,0);
         animated.transform.localScale=Vector3.one*.62f;
         SpriteRenderer overlay=animated.AddComponent<SpriteRenderer>();overlay.sortingOrder=20;
         CGAnimatedProp frames=animated.AddComponent<CGAnimatedProp>();
         frames.Configure(clip,clip=="Terminal_Screen"?4:5,8f);
      }
    }
  }
  void SpawnNPC(float x,string kind){GameObject npc=new GameObject("NPC_"+kind);npc.transform.SetParent(root);npc.transform.position=new Vector3(x,1.02f,0);SpriteRenderer s=npc.AddComponent<SpriteRenderer>();s.sortingOrder=15;s.sprite=CGArt.NPC(kind,0);var anim=npc.AddComponent<CGFrameAnimator>();anim.view=s;anim.npcKind=kind;
    npc.transform.localScale=new Vector3(1.35f,1.35f,1);var actor=npc.AddComponent<CGMarker>();actor.kind="NPC";actor.game=game;}
  void SpawnAssistant(){GameObject npc=new GameObject("Asistente_Digital");npc.transform.SetParent(root);npc.transform.position=new Vector3(4,2f);var sr=npc.AddComponent<SpriteRenderer>();sr.sortingOrder=18;var an=npc.AddComponent<CGFrameAnimator>();an.assistant=true;an.view=sr;npc.transform.localScale=Vector3.one*.85f;
    var follow=npc.AddComponent<CGAssistantFollow>();follow.game=game;}
  void SpawnEnemy(float x,string name,bool isBoss){GameObject en=new GameObject("ENEMY_"+name);en.transform.SetParent(root);en.transform.position=new Vector3(x,isBoss?0.65f:0.45f,0);CGEnemy enemy=en.AddComponent<CGEnemy>();enemy.Setup(game,name,isBoss);if(isBoss)boss=enemy;}
  void PlaceSign(string text,float x,float y){GameObject board=new GameObject("SENAL_"+text);board.transform.SetParent(root);board.transform.position=new Vector3(x,y,0);var sr=board.AddComponent<SpriteRenderer>();sr.sprite=CGArt.Get("01_Nuevos_Modulares/00_Global/Arquitectura/DirectionalArrows");sr.sortingOrder=8;}
}
public class CGAssistantFollow:MonoBehaviour {
 public CGGame game;
 void Update(){if(game==null||game.Hero==null)return;Vector3 target=game.Hero.transform.position+new Vector3(-1.2f,2.05f+0.19f*Mathf.Sin(Time.time*3),0);transform.position=Vector3.Lerp(transform.position,target,Time.deltaTime*3f);}
}
public class CGMarker:MonoBehaviour {public CGGame game;public string kind;}
}

namespace CampusGlitch {
public class CGAmbientGlow:UnityEngine.MonoBehaviour {
 public float seed;
 UnityEngine.SpriteRenderer sr;
 void Awake(){sr=GetComponent<UnityEngine.SpriteRenderer>();}
 void Update(){if(sr==null)return;
    float alpha=.35f+.27f*UnityEngine.Mathf.Sin(UnityEngine.Time.time*2f+seed);
    sr.color=new UnityEngine.Color(1,1,1,alpha);
    transform.localScale=UnityEngine.Vector3.one*(.28f+.035f*UnityEngine.Mathf.Sin(UnityEngine.Time.time*3.7f+seed));
 }
}
}
