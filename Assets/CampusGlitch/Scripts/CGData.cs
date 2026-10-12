using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;

namespace CampusGlitch {
[Serializable] public class CGProgress {
    public int latestUnlocked=0;
    public int[] stars=new int[10];
    public bool[] completed=new bool[10];
    public bool purpleUnlocked=false;
    public bool purpleEquipped=false;
    public bool useOriginalWolf=false;
    public int dataVersion=2;
    public bool extraUnlocked=false;
    public float masterVolume=0.85f;
    public bool reduceFlash=false;
    public bool subtitles=true;
    public string[] actionKeys={"A", "D", "Space", "LeftShift", "J", "E", "Q", "Escape"};
    public string[] padBindings={"dpadLeft","dpadRight","buttonSouth","rightTrigger","buttonWest","buttonNorth","rightShoulder","startButton"};
}
public static class CGStore {
    public static CGProgress Data;
    static string FileName {get {return System.IO.Path.Combine(Application.persistentDataPath,"campus_glitch_save_v1.json");}}
    public static void Load(){
      if(Data!=null)return;
      try { Data=JsonUtility.FromJson<CGProgress>(System.IO.File.ReadAllText(FileName)); } catch { Data=new CGProgress(); }
      if(Data==null || Data.completed==null || Data.completed.Length!=10)Data=new CGProgress();
      // Migration of old saves: previous demo selected the quadruped by default.
      if(Data.dataVersion<2){Data.useOriginalWolf=false;Data.dataVersion=2;}
      if(Data.actionKeys==null || Data.actionKeys.Length!=8)Data.actionKeys=new CGProgress().actionKeys;
      if(Data.padBindings==null || Data.padBindings.Length!=8)Data.padBindings=new CGProgress().padBindings;
    }
    public static void Save(){Load();try{
      string path=FileName; System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
      string tmp=path+".tmp";System.IO.File.WriteAllText(tmp,JsonUtility.ToJson(Data,true));
      if(System.IO.File.Exists(path)){System.IO.File.Copy(path,path+".bak",true);System.IO.File.Delete(path);}
      System.IO.File.Move(tmp,path);
    }catch(Exception e){Debug.LogError("No se pudo guardar el progreso: "+e.Message);}}
    public static void Reset(){Data=new CGProgress();Save();}
}
public static class CGControl {
    public static readonly string[] Names={"Izquierda","Derecha","Saltar","Correr","Atacar","Interactuar","Habilidad","Pausar"};
    static bool KeyHeld(int n){CGStore.Load(); if(Keyboard.current==null)return false;
      try {Key k=(Key)Enum.Parse(typeof(Key),CGStore.Data.actionKeys[n],true);return Keyboard.current[k].isPressed;}catch{return false;}}
    static bool KeyTap(int n){CGStore.Load();if(Keyboard.current==null)return false;
      try {Key k=(Key)Enum.Parse(typeof(Key),CGStore.Data.actionKeys[n],true);return Keyboard.current[k].wasPressedThisFrame;}catch{return false;}}
    public static bool PadHeld(int n){CGStore.Load();if(Gamepad.current==null)return false;return PadHeldName(CGStore.Data.padBindings[n]);}
    static bool PadHeldName(string name){var p=Gamepad.current;if(p==null)return false;
      switch(name){case "dpadLeft":return p.dpad.left.isPressed;case "dpadRight":return p.dpad.right.isPressed;
      case "dpadUp":return p.dpad.up.isPressed;case "dpadDown":return p.dpad.down.isPressed;
      case "buttonSouth":return p.buttonSouth.isPressed;case "buttonNorth":return p.buttonNorth.isPressed;
      case "buttonWest":return p.buttonWest.isPressed;case "buttonEast":return p.buttonEast.isPressed;
      case "leftShoulder":return p.leftShoulder.isPressed;case "rightShoulder":return p.rightShoulder.isPressed;
      case "startButton":return p.startButton.isPressed;case "selectButton":return p.selectButton.isPressed;
      case "leftStickButton":return p.leftStickButton.isPressed;case "rightStickButton":return p.rightStickButton.isPressed;
      case "leftTrigger":return p.leftTrigger.ReadValue()>.65f;case "rightTrigger":return p.rightTrigger.ReadValue()>.65f;
      default:return false;}}
    public static bool PadTapped(int n){CGStore.Load();var p=Gamepad.current;if(p==null)return false;
      string name=CGStore.Data.padBindings[n];if(name=="leftTrigger"||name=="rightTrigger")return false;
      ButtonControl b=PadButton(name);return b!=null&&b.wasPressedThisFrame;}
    static ButtonControl PadButton(string n){var p=Gamepad.current;if(p==null)return null;
      switch(n){case "dpadLeft":return p.dpad.left;case "dpadRight":return p.dpad.right;
      case "dpadUp":return p.dpad.up;case "dpadDown":return p.dpad.down;
      case "buttonSouth":return p.buttonSouth;case "buttonNorth":return p.buttonNorth;case "buttonWest":return p.buttonWest;
      case "buttonEast":return p.buttonEast;case "leftShoulder":return p.leftShoulder;case "rightShoulder":return p.rightShoulder;
      case "startButton":return p.startButton;case "selectButton":return p.selectButton;
      case "leftStickButton":return p.leftStickButton;case "rightStickButton":return p.rightStickButton;
      default:return null;}}
    public static string NextGamepadButton(){string[] names={"buttonSouth","buttonWest","buttonNorth","buttonEast","leftShoulder","rightShoulder","startButton","selectButton","dpadLeft","dpadRight","dpadUp","dpadDown","leftStickButton","rightStickButton"};
      foreach(string n in names){ButtonControl b=PadButton(n);if(b!=null&&b.wasPressedThisFrame)return n;}return null;}
    public static float Axis(){float x=(KeyHeld(1)?1:0)-(KeyHeld(0)?1:0);if(Gamepad.current!=null){float v=Gamepad.current.leftStick.ReadValue().x;if(Mathf.Abs(v)>0.22f)x=v; if(PadHeld(0))x=-1; if(PadHeld(1))x=1;}return x;}
    public static bool HoldRun(){return KeyHeld(3) || PadHeld(3);}
    public static bool Jump(){return KeyTap(2) || PadTapped(2);}
    public static bool Attack(){return KeyTap(4) || PadTapped(4);}
    public static bool Interact(){return KeyTap(5) || PadTapped(5);}
    public static bool Ability(){return KeyTap(6) || PadTapped(6);}
    public static bool Pause(){return KeyTap(7) || (Keyboard.current!=null&&Keyboard.current.escapeKey.wasPressedThisFrame) ||PadTapped(7);}
    public static bool Confirm(){return (Keyboard.current!=null&&(Keyboard.current.enterKey.wasPressedThisFrame||Keyboard.current.spaceKey.wasPressedThisFrame)) || (Gamepad.current!=null&&Gamepad.current.buttonSouth.wasPressedThisFrame);}
    public static bool Back(){return (Keyboard.current!=null&&Keyboard.current.escapeKey.wasPressedThisFrame)||(Gamepad.current!=null&&Gamepad.current.buttonEast.wasPressedThisFrame);}
    public static bool Down(){return (Keyboard.current!=null&&(Keyboard.current.downArrowKey.wasPressedThisFrame||Keyboard.current.sKey.wasPressedThisFrame))||(Gamepad.current!=null&&Gamepad.current.dpad.down.wasPressedThisFrame);}
    public static bool Up(){return (Keyboard.current!=null&&(Keyboard.current.upArrowKey.wasPressedThisFrame||Keyboard.current.wKey.wasPressedThisFrame))||(Gamepad.current!=null&&Gamepad.current.dpad.up.wasPressedThisFrame);}
    public static Key? NextKeyboardKey(){if(Keyboard.current==null)return null;foreach(KeyControl key in Keyboard.current.allKeys)if(key.wasPressedThisFrame && key.keyCode!=Key.None)return key.keyCode;return null;}
}
[Serializable] public class CGLevelSpec {
    public string name,key,objective,npc,dialogue,boss;
    public string[] props;
    public string quiz,option1,option2,option3;
    public int right;
}
public static class CGLevels {
 public static readonly CGLevelSpec[] All={
   Level("Exterior / acceso principal","01_Exterior","Recupera los datos del acceso y habla con Guardia U.","Guardia_U","Bienvenido a UNIFRANZ. Aprende haciendo y recupera la primera señal.","",new[]{"ExteriorLightPole","UniversityGlassEntrance","UrbanBench","ExteriorPlanter_Long","CampusTotem","SecurityBooth"},"¿Qué necesitas para entrar?","Una credencial","Un libro","Un micrófono",0),
   Level("Planta baja / lobby","02_PlantaBaja","Visita el lobby, recoge las señales y restaura la credencial.","Recepcionista","La recepción conecta a estudiantes con los servicios del campus.","",new[]{"WaitingChairs_Blue","MissionBoard_Lobby","AdmissionDesk","CafeteriaStool","RoundCafeTable"},"¿Qué área ayuda a orientar estudiantes?","Recepción","Sala de rayos X","Techo",0),
   Level("Mezanine / conexión","03_Mezanine","Reactiva la red de oficinas del mezanine.","Director","ARCA, Cajas y Dirección necesitan recuperar la conexión.","",new[]{"AdminReception_ARCA","QueueTicketMachine","CareerDirectorDoor","FacultyDirectory","PaymentCounter_Cajas"},"¿Qué organiza el recorrido entre áreas?","Una baranda","Señalética y accesos","Un enemigo",1),
   Level("Piso 1 / laboratorios de salud","04_Piso1_Salud","Recupera tres muestras digitales para el laboratorio.","Docente","Observa los equipos y restaura los registros de investigación.","",new[]{"NutritionKitchenSink","MedicalStool","WhiteHealthFloor","ExperimentFlask","LabCurtain_Blue","Anatomy_Skeleton"},"¿Qué sirve para observar muestras pequeñas?","Microscopio","Mesa de juegos","Router",0),
   Level("Piso 2 / Ingeniería de Sistemas","05_Piso2_Sistemas","Recupera los nodos y vuelve a activar la red.","Noemi_Moller","Los sistemas fallan; ordena la información y restaura la red.","",new[]{"SystemsComputerDesk","ServerRack_Double","Router_Active","HackTerminal","SystemsPC_DualMonitor","Switch_Network"},"¿Qué dispositivo comunica redes?","Proyector","Router","Silla",1),
   Level("Piso 4 / aulas y pasillos","06_Piso4","Resuelve el código de decisiones y vence a Secuencias.","Director","Estudia el patrón del enemigo: primero avisa, luego ataca.","Boss_Secuencias",new[]{"PsychologySofa","ClassroomDoorNumber","NoticeBoard","JusticeEmblem","ProjectorCeiling","GesellMirror_OneWay"},"Secuencia 2, 4, 6 ... ¿qué sigue?","7","10","8",2),
   Level("Piso 5 / Biblioteca e Innova","07_Piso5_Biblioteca","Organiza las ideas para abrir el acceso a la terraza.","Unifranzito","¡Innova! Aquí puedes aprender y compartir con tus compañeros.","",new[]{"StudyLamp","BookshelfDouble","BibliotecaBookcase_Tall","InnovaGlassModule","BibliotecaStudyTable_Long","LibraryCheckpoint"},"¿Dónde buscarías información académica?","Biblioteca","Estacionamiento","Entrada principal",0),
   Level("Terraza / lounge / solarium","08_Terraza_Lounge","Recupera el enlace exterior y supera el reto de lógica.","Estudiante","La vista de La Paz nos recuerda lo que estamos restaurando.","Boss_IF_ELSE",new[]{"TerraceGreenFloor","PalletChair","TerraceRailGuard","LoungeSofa_Brown","TieredGreenSofa","RoofBlueSteelGirder"},"Una condición IF verdadera ejecuta...","La rama ELSE","La rama IF","Ninguna",1),
   Level("Clínica odontológica / núcleo final","09_Odontologia","Restaura el núcleo del campus y vence a Core Glitch.","Noemi_Moller","La última llave de acceso está junto al núcleo digital.","Core_Glitch",new[]{"DentistryXRayMachine","DentistryBlueChair","ConsultorioSign","DentalUnit","DentistryMonitor","DentalAccessDoor"},"¿Qué protege el acceso digital?","Compartir contraseñas","Usar credenciales seguras","Desactivar todo",1),
   Level("EXTRA / subsuelo de anatomía","EXTRA_Subsuelo","Investiga el pasillo secreto de Osteoteca y Anatomía.","Docente","Aquí se guardan pistas adicionales del campus.","",new[]{"AnatomySink","DentalGiantModel","AnatomyBrain_Model","MedicalWasteBin_Red","BoneDisplayCabinet","AnfiteatroBleachers"},"¿Dónde se estudian los huesos?","Osteoteca","Terraza","Game room",0)
 };
 static CGLevelSpec Level(string n,string k,string o,string npc,string d,string b,string[] p,string quiz,string a,string c,string e,int right){return new CGLevelSpec{name=n,key=k,objective=o,npc=npc,dialogue=d,boss=b,props=p,quiz=quiz,option1=a,option2=c,option3=e,right=right};}
 public static string Scene(int i){return i==9?"SC_Extra_Subsuelo":"SC_Level"+(i+1).ToString("00");}
}
public static class CGArt {
  public static Sprite Get(string path){return Resources.Load<Sprite>("CG/"+path);}
  public static Sprite Biped(bool purple,int f){return Get("Playable/"+(purple?"Purple_":"Red_")+Mathf.Clamp(f,0,20).ToString("00"));}
  // Three original pixel-art animations supplied by the team. Default is Classic.
  public static Sprite Player(bool purple,int frame,bool original){
    if(original)return Get("Playable/"+(purple?"ClassicPurple_":"Classic_")+Mathf.Clamp(frame,0,16).ToString("00"));
    return Biped(purple,frame);
  }
  public static Sprite NPC(string kind,int f){return Get("02_Animados/NPCs/"+kind+"/"+(f<4?"IDLE_":"HABLAR_")+(f%4).ToString("00"));}
  public static Sprite Enemy(string kind,int f){if(kind=="Boss_Secuencias"||kind=="Boss_IF_ELSE"||kind=="Core_Glitch")return Get("02_Animados/Jefes/"+kind+"/IDLE_"+(f%4).ToString("00"));return Get("02_Animados/Enemigos/"+kind+"/IDLE_"+(f%4).ToString("00"));}
  public static Sprite Prop(CGLevelSpec spec,string name){return Get("01_Nuevos_Modulares/"+spec.key+"/"+name);}
}
}
