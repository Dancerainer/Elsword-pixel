using System;
using System.Collections.Generic;
namespace ElswordPixel {
 // Owned display timing only: it cannot modify the GameModel or emit damage.
 public sealed class CoreHardOwnedVisualFieldV21 {
  public readonly int serial;public readonly string field;public readonly float started,window;
  public readonly bool forceDieOnHit;public bool killed {get;internal set;}
  internal CoreHardOwnedVisualFieldV21(int owner,string name,float at,float duration,bool force){serial=owner;field=name;started=at;window=duration;forceDieOnHit=force;}
 }
 public sealed class CoreHardOwnedVisualFieldClockV21 {
  readonly Dictionary<string,CoreHardOwnedVisualFieldV21> fields=new Dictionary<string,CoreHardOwnedVisualFieldV21>(StringComparer.Ordinal);
  readonly Dictionary<int,List<float>> hits=new Dictionary<int,List<float>>();
  public int Count{get{return fields.Count;}}
  public void Clear(){fields.Clear();hits.Clear();}
  static bool Finite(float x){return !float.IsNaN(x)&&!float.IsInfinity(x);}
  public bool TryStart(int serial,string field,float bornAt,float eventTime,float window,bool forceDieOnHit,float now,bool parentLive,out CoreHardOwnedVisualFieldV21 value){
   value=null;if(serial<=0||string.IsNullOrEmpty(field)||!Finite(bornAt)||!Finite(eventTime)||eventTime<0||!Finite(window)||window<0||!Finite(now))throw new ArgumentException("Invalid exact owned Core visual field");
   float at=bornAt+eventTime;if(!parentLive||now+1e-6f<at)return false;string key=serial+"/"+field;
   if(fields.TryGetValue(key,out value))return false;
   value=new CoreHardOwnedVisualFieldV21(serial,field,at,window,forceDieOnHit);fields.Add(key,value);
   List<float> observed;if(forceDieOnHit&&hits.TryGetValue(serial,out observed))foreach(float hit in observed)if(hit+1e-6f>=at){value.killed=true;break;}
   return true;
  }
  public void SuccessfulSourceHit(int serial,float at){
   if(serial<=0||!Finite(at))throw new ArgumentException("Invalid successful source HIT clock");
   List<float> list;if(!hits.TryGetValue(serial,out list)){list=new List<float>();hits.Add(serial,list);}if(!list.Contains(at))list.Add(at);
   foreach(var p in fields){var f=p.Value;if(f.serial==serial&&f.forceDieOnHit&&at+1e-6f>=f.started)f.killed=true;}
  }
  public void RetireAbsentOwners(ISet<int> live){
   if(live==null)throw new ArgumentNullException("live");var remove=new List<string>();foreach(var p in fields)if(!live.Contains(p.Value.serial)){p.Value.killed=true;remove.Add(p.Key);}foreach(string key in remove)fields.Remove(key);
   var ended=new List<int>();foreach(int serial in hits.Keys)if(!live.Contains(serial))ended.Add(serial);foreach(int serial in ended)hits.Remove(serial);
  }
 }
}
