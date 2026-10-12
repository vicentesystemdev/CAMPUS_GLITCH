using UnityEngine;
namespace CampusGlitch {
// Animated sprites remain separate from scene collision and mission logic.
public sealed class CGAnimatedProp:MonoBehaviour {
 public string effectName="";public int frameCount=4;public float fps=8f;
 public bool pingPong=false;
 SpriteRenderer target;
 Sprite[] frames;
 void Awake(){target=GetComponent<SpriteRenderer>();}
 public void Configure(string name,int count,float framesPerSecond=8f){
   effectName=name;frameCount=count;fps=framesPerSecond;
   target=GetComponent<SpriteRenderer>();frames=null;EnsureFrames();
 }
 void EnsureFrames(){
  if(frames!=null||string.IsNullOrEmpty(effectName))return;
  frames=new Sprite[frameCount];
  for(int i=0;i<frameCount;i++)frames[i]=Resources.Load<Sprite>("CG/Effects/"+effectName+"_"+i.ToString("00"));
 }
 void Update(){
  if(target==null)return;EnsureFrames();if(frames==null||frames.Length==0)return;
  int ix=(int)(Time.time*fps)%frames.Length;
  if(pingPong&&frames.Length>2){int cycle=2*frames.Length-2;ix=(int)(Time.time*fps)%cycle;if(ix>=frames.Length)ix=cycle-ix;}
  if(frames[ix]!=null)target.sprite=frames[ix];
 }
}
}
