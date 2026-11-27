using System;
using System.Collections.Generic;

namespace ElswordPixel
{
    public sealed partial class GameModel
    {
        struct RegionPoint { internal float x,y,z; internal RegionPoint(float x,float y,float z=0){this.x=x;this.y=y;this.z=z;} }
        sealed class RegionNative { internal string name;internal object owner; }
        sealed class RegionVisual {internal bool alive=true;internal RegionPoint position;}
        static readonly object RegionUnknown=new object();
        public bool TraceRegionNpcCalls;
        public readonly HashSet<string> RegionUnsupportedCalls=new HashSet<string>();
        object RegionMissing(string operation){
            if(TraceRegionNpcCalls)RegionUnsupportedCalls.Add(operation);
            return RegionUnknown;
        }
        sealed class RegionTable : Dictionary<object,object>{}
        readonly List<Enemy> regionSummons=new List<Enemy>();
        static float RegionNumber(object x){return x is float?(float)x:x is int?(int)x:Single.NaN;}
        static bool RegionTruth(object x){return x!=null&&!(x is bool&&!(bool)x)&&x!=RegionUnknown;}
        static object RegionPrimitive(int index){return index==0?null:(object)(index==2);}
        static string RegionSymbol(object v){return v is string?(string)v:v is RegionNative?((RegionNative)v).name:"";}
        static object RegionArg(object[] args,int ix){return ix>=0&&ix<args.Length?args[ix]:RegionUnknown;}
        static int RegionId(object v){float n=RegionNumber(v);return Single.IsNaN(n)?-1:(int)n;}
        static float RegionValue(Dictionary<int,float> map,int key){float n;return map.TryGetValue(key,out n)?n:0;}
        static bool RegionEqual(object x,object y){return Equals(x,y)||!Single.IsNaN(RegionNumber(x))&&RegionNumber(x)==RegionNumber(y);}
        RegionPoint RegionPosition(object f) {
            var fighter=f as Fighter;
            return fighter!=null?new RegionPoint(fighter.x/.28f,fighter.y/.28f):new RegionPoint(0,0);
        }
        public bool RequestRegionNpcState(Enemy e,string state)
        {
            RegionActorMechanics spec;
            if(e==null||String.IsNullOrEmpty(state)||!regionActors.TryGetValue(e.kind,out spec)||!spec.nodes.ContainsKey(state))return false;
            if(e.regionActor==null)TickRegionNpc(e,0);
            if(e.regionActor==null)return false;
            e.regionActor.request=state;
            return true;
        }

        object RunRegionFunction(Enemy e,RegionActorState a,string name)
        {
            if(String.IsNullOrEmpty(name))return null;
            RegionFunction fn;
            if(!a.spec.code.TryGetValue(name,out fn))return null;
            return EvalRegionCode(e,a,fn,new object[]{this,this,e},0);
        }
        object EvalRegionCode(Enemy e,RegionActorState a,RegionFunction fn,object[] args,int depth)
        {
            if(fn.name=="IsNearLineEndPos" && e.kind=="dark_nephilim" && VelderChaseRoom)
                return NearVelderChaseEnd(e,RegionNumber(RegionArg(args,1)));
            if(depth>8||fn.op==null)return RegionUnknown;
            var r=new object[Math.Max(8,fn.frame+5)];
            Array.Copy(args,r,Math.Min(args.Length,r.Length));
            int pc=0,top=0,remaining=Math.Max(64,fn.op.Length*5);
            while(pc>=0&&pc<fn.op.Length&&remaining-->0){
                uint w=(uint)fn.op[pc++];
                int op=(int)(w&255),ra=(int)(w>>8)&255,d=(int)(w>>16),b=d>>8,c=d&255;
                if(ra>=r.Length)return RegionUnknown;
                if(op<=0x0b){
                    object x=r[ra],y;
                    if(op<=5)y=d<r.Length?r[d]:RegionUnknown;
                    else if(op<=7)y=d<fn.constants.Length?fn.constants[d]:RegionUnknown;
                    else if(op<=9)y=d<fn.numbers.Length?(object)fn.numbers[d]:RegionUnknown;
                    else y=RegionPrimitive(d);
                    if(x==RegionUnknown||y==RegionUnknown)return RegionUnknown;
                    bool match;
                    switch(op){
                        case 0:match=RegionNumber(x)<RegionNumber(y);break;
                        case 1:match=RegionNumber(x)>=RegionNumber(y);break;
                        case 2:match=RegionNumber(x)<=RegionNumber(y);break;
                        case 3:match=RegionNumber(x)>RegionNumber(y);break;
                        default:match=(op%2==0)==RegionEqual(x,y);break;
                    }
                    if(!match)pc++;
                }else if(op>=0x1a&&op<=0x28){
                    object x,y;
                    if(op<=0x1e){x=r[b];y=fn.numbers[c];}
                    else if(op<=0x23){x=fn.numbers[c];y=r[b];}
                    else{x=r[b];y=r[c];}
                    int kind=(op-0x1a)%5;
                    if(x is RegionPoint&&y is RegionPoint&&kind<=1){
                        var l=(RegionPoint)x;var h=(RegionPoint)y;float sign=kind==0?1:-1;
                        r[ra]=new RegionPoint(l.x+sign*h.x,l.y+sign*h.y,l.z+sign*h.z);
                    }else{
                        float l=RegionNumber(x),h=RegionNumber(y);
                        r[ra]=kind==0?l+h:kind==1?l-h:kind==2?l*h:kind==3?l/h:l%h;
                    }
                }else switch(op){
                    case 0x0c:r[ra]=r[d];break;
                    case 0x0d:r[ra]=!RegionTruth(r[d]);break;
                    case 0x0e:r[ra]=-RegionNumber(r[d]);break;
                    case 0x12:case 0x13:if(r[d]==RegionUnknown)return RegionUnknown;if(RegionTruth(r[d])!=(op==0x12))pc++;break;
                    case 0x14:r[ra]=fn.constants[d];break;
                    case 0x16:r[ra]=(float)(short)d;break;
                    case 0x17:r[ra]=fn.numbers[d];break;
                    case 0x18:r[ra]=RegionPrimitive(d);break;
                    case 0x19:for(int i=ra;i<=d&&i<r.Length;i++)r[i]=null;break;
                    case 0x2a:{
                        var joined=new System.Text.StringBuilder();
                        for(int i=b;i<=c;i++)joined.Append(Convert.ToString(r[i],System.Globalization.CultureInfo.InvariantCulture));
                        r[ra]=joined.ToString();break;
                    }
                    case 0x2b:case 0x2c:{
                        var callee=r[ra] as RegionNative;
                        if(callee==null)return RegionUnknown;
                        int argc=op==0x2b?Math.Min(top-ra-1,r.Length-ra-1):Math.Min(c-1,r.Length-ra-1);
                        argc=Math.Max(0,argc);
                        var actual=new object[argc];Array.Copy(r,ra+1,actual,0,argc);
                        RegionFunction nested;
                        object result=a.spec.code.TryGetValue(callee.name,out nested)?
                            EvalRegionCode(e,a,nested,actual,depth+1):CallRegionNative(e,a,callee,actual);
                        if(callee.name=="ipairs"&&b>=4&&actual.Length>0){r[ra]=callee;r[ra+1]=actual[0];r[ra+2]=0f;}
                        else if(b!=1)r[ra]=result;
                        if(b==0)top=ra+1;
                        break;
                    }
                    case 0x2f:{
                        var table=r[ra-2] as RegionTable;object value;
                        int index=RegionId(r[ra-1])+1;
                        if(table!=null&&table.TryGetValue((float)index,out value)){r[ra]=(float)index;r[ra+1]=value;}
                        else r[ra]=null;
                        break;
                    }
                    case 0x3a:case 0x3b:{
                        var table=new RegionTable();
                        if(op==0x3b&&fn.tables!=null)foreach(var template in fn.tables)if(template.index==d)
                            foreach(var item in template.items)table[(float)item.key]=item.text!=null?(object)item.text:item.value;
                        r[ra]=table;
                        break;
                    }
                    case 0x3c:{
                        string k=fn.constants[d];object v;
                        r[ra]=a.globals.TryGetValue(k,out v)?v:k=="TRUE"?(object)true:k=="FALSE"?(object)false:new RegionNative{name=k};
                        break;
                    }
                    case 0x3d:a.globals[fn.constants[d]]=r[ra];break;
                    case 0x3e:case 0x40:{
                        var table=r[b] as RegionTable;
                        object key=op==0x40?(object)(float)c:r[c],v;
                        r[ra]=key!=null&&table!=null&&table.TryGetValue(key,out v)?v:null;break;
                    }
                    case 0x3f:{
                        string field=fn.constants[c];object target=r[b];
                        if(target is RegionPoint&&(field=="x"||field=="y"||field=="z")){
                            var p=(RegionPoint)target;r[ra]=field=="x"?p.x:field=="y"?p.y:p.z;
                        }else if(target is RegionTable){object v;r[ra]=((RegionTable)target).TryGetValue(field,out v)?v:null;}
                        else if(target is RegionNative&&(field.StartsWith("NUI_",StringComparison.Ordinal)||field.StartsWith("GAI_CMD_",StringComparison.Ordinal)||field.StartsWith("RT_",StringComparison.Ordinal)))r[ra]=field;
                        else r[ra]=new RegionNative{name=field,owner=target};
                        break;
                    }
                    case 0x41:case 0x43:{
                        var table=r[b] as RegionTable;object key=op==0x43?(object)(float)c:r[c];
                        if(table!=null&&key!=null)table[key]=r[ra];break;
                    }
                    case 0x42:{
                        string field=fn.constants[c];
                        if(r[b] is RegionTable){((RegionTable)r[b])[field]=r[ra];break;}
                        if(r[b] is RegionPoint){
                            var p=(RegionPoint)r[b];float val=RegionNumber(r[ra]);
                            if(field=="x")p.x=val;else if(field=="y")p.y=val;else if(field=="z")p.z=val;
                            r[b]=p;
                        }
                        break;
                    }
                    case 0x47:return null;
                    case 0x48:return r[ra];
                    case 0x49:case 0x4b:{
                        float first=RegionNumber(r[ra]),limit=RegionNumber(r[ra+1]),step=RegionNumber(r[ra+2]);
                        if(op==0x4b)first+=step;
                        r[ra]=r[ra+3]=first;
                        bool inside=step>=0?first<=limit:first>=limit;
                        if((op==0x49&&!inside)||(op==0x4b&&inside))pc+=d-32768;
                        break;
                    }
                    case 0x4e:if(r[ra]!=null){r[ra-1]=r[ra];pc+=d-32768;}break;
                    case 0x51:break;
                    case 0x54:pc+=d-32768;break;
                    default:return RegionMissing("opcode-"+op.ToString("x2")+" "+fn.name);
                }
            }
            return RegionUnknown;
        }
        object CallRegionNative(Enemy e,RegionActorState a,RegionNative call,object[] raw)
        {
            // Colon calls pass the receiver as argument zero. Global helper functions do not.
            var library=call.owner as RegionNative;
            int offset=call.owner==null||library!=null&&(library.name=="math"||library.name=="table"||library.name=="os")?0:1;
            object p=RegionArg(raw,offset),q=RegionArg(raw,offset+1),third=RegionArg(raw,offset+2);
            int key=RegionId(p);float n=RegionNumber(p);
            Fighter subject=call.owner as Fighter??e;
            var subjectEnemy=subject as Enemy;
            RegionActorState local=subjectEnemy!=null&&subjectEnemy.regionActor!=null?subjectEnemy.regionActor:a;
            object stageValue;
            if(VelderChaseRoom && TryVelderStageNative(call.name,subject,p,q,third,out stageValue))return stageValue;
            if(call.name=="PlaySound_LUA" || call.name=="Play2DSound_LUA" || call.name=="StopSound_LUA") {
                QueueRegionScriptSound(call.name,e,a.node?.id,raw);
                return null;
            }
            if(TryRegionalBindings(call.name,e,subject,local,p,q,third,RegionArg(raw,offset+3),out stageValue))return stageValue;
            if(TryRegionInteraction(e,local,call.name,call.owner,raw,offset,out stageValue))return stageValue;
            switch(call.name){
                case "GetUnitUID":return call.owner==player?1000000f:call.owner is Enemy?RegionUnitId((Enemy)call.owner):0f;
                case "GetFollowHitter":return (float)local.stick;
                case "SetGAccel":if(Finite(n))local.gravityOverride=n*.28f;return null;
                case "SetReloadAccel":if(Finite(n))local.reloadOverride=n*.28f;return null;
                case "GetNowHP":case "GetNowHP_LUA":return subject.hp;
                case "GetMaxHP":case "GetMaxHP_LUA":return subject.maxHp;
                case "SetNowHP":case "SetNowHP_LUA":if(!Single.IsNaN(n))ApplyCombatHealth(subject,Clamp(n,0,subject.maxHp));return null;
                case "GetNowMP":return subject==player?mana:local.mp;
                case "GetMaxMP":return subject==player?MaxMana:local.spec.maxMp;
                case "SetNowMP":
                    if(!Single.IsNaN(n)){if(subject==player)mana=Clamp(n,0,MaxMana);else local.mp=Clamp(n,0,local.spec.maxMp);}
                    return null;
                case "GetInt_LUA":return RegionValue(local.ints,key);
                case "SetInt_LUA":if(key>=0)local.ints[key]=RegionNumber(q);return null;
                case "GetFlag_LUA":return local.flags.Contains(key);
                case "SetFlag_LUA":if(key>=0){if(RegionTruth(q))local.flags.Add(key);else local.flags.Remove(key);}return null;
                case "SetAllFlag_LUA":if(!RegionTruth(p))local.flags.Clear();return null;
                case "GetStateTime":return local.time;
                case "GetNowAnimTime_LUA":return local.MotionTime;
                case "AnimTimer_LUA":return local.MotionTime>=n;
                case "AnimEventTimer_LUA":return local.PreviousMotionTime<n&&local.MotionTime>=n;
                case "GetTimerElapsedTime":return local.age-RegionValue(local.timers,key);
                case "SetTimerRestart":local.timers[key]=local.age;return null;
                case "GetGameTime":return elapsed;
                case "GetHardLevel":return 1f;
                case "EventTimer":return local.prev<n&&local.time>=n;
                case "GetAtkPhysic_LUA":return local.physicalMultiplier;
                case "SetAtkPhysic_LUA":if(n>0)local.physicalMultiplier=n;return null;
                case "SetPassiveSpeed":if(!Single.IsNaN(n))local.passiveX=n*.28f;return null;
                case "HasTarget":return player.hp>0;
                case "GetUserUnitListSize":return player.hp>0?1f:0f;
                case "GetNPCUnitNum":return (float)enemies.Count;
                case "GetTeam":return subject==player?0f:1f;
                case "GetFarthestUserUnitPos_LUA":return RegionPosition(player);
                case "SetCloaking":local.hidden=true;return null;
                case "SetUnCloaking":local.hidden=false;return null;
                case "SetAIEnable":if(subjectEnemy!=null)subjectEnemy.aiEnabled=RegionTruth(p);return null;
                case "SetTargetUnit":case "FlushCreateNPCReq":case "randomseed":return null;
                case "D3DXVECTOR2":return new RegionPoint(n,RegionNumber(q));
                case "GetReflectMagic_LUA":return local.node.reflectTo>local.node.reflectFrom&&
                    local.MotionTime>=local.node.reflectFrom&&local.MotionTime<=local.node.reflectTo;
                case "GetGameLevel":return 0f;
                case "AddReverseGravityRegion":
                    if(p is RegionPoint&&RegionNumber(q)>0&&RegionNumber(third)>0){
                        var pos=(RegionPoint)p;
                        regionGravityX=pos.x*.28f;
                        regionGravityTop=pos.y*.28f+Math.Max(0,RegionNumber(RegionArg(raw,offset+3)))*.28f;
                        regionGravityRadius=RegionNumber(q)*.28f;
                        regionGravityUntil=elapsed+RegionNumber(third);
                    }
                    return null;
                case "ipairs":return null;
                case "sort":{
                    var table=p as RegionTable;if(table==null)return null;
                    var items=new List<RegionTable>();
                    for(int i=1;i<=table.Count;i++){object v;if(table.TryGetValue((float)i,out v)&&v is RegionTable)items.Add((RegionTable)v);}
                    items.Sort((l,r)=>RegionNumber(l["density"]).CompareTo(RegionNumber(r["density"])));
                    for(int i=0;i<items.Count;i++)table[(float)(i+1)]=items[i];return null;
                }
                case "D3DXVECTOR3":return new RegionPoint(n,RegionNumber(q),RegionNumber(third));
                case "GetMyUnit":return player;
                case "SetCanNotInputTime_LUA":
                    if(subject==player&&n>=0)regionInputUntil=Math.Max(regionInputUntil,elapsed+n);
                    return null;
                case "SetShow":local.hidden=!RegionTruth(p);return null;
                case "SetShowGage":local.hiddenGauge=!RegionTruth(p);return null;
                case "IsHost":return true;
                case "GetRandVal":return (float)(int)(SkillRandom()*32767);
                case "random":return Single.IsNaN(n)?(object)SkillRandom():!Single.IsNaN(RegionNumber(q))?
                    (float)(int)(n+SkillRandom()*(RegionNumber(q)-n+1)):(float)(1+(int)(SkillRandom()*n));
                case "time":return (float)(int)elapsed;
                case "GetIsRight":return subject.facing>0;
                case "SetIsRight":subject.facing=RegionTruth(p)?1:-1;return null;
                case "GetDirVector":return new RegionPoint(subject.facing,0);
                case "GetLandPosition_LUA":
                    if(library!=null&&library.name=="LineMap"&&p is RegionPoint)return RegionLand((RegionPoint)p);
                    goto case "GetPos";
                case "GetPos":
                    return call.owner is RegionVisual?((RegionVisual)call.owner).position:RegionPosition(subject);
                case "GetBonePos_LUA":{
                    var point=RegionBonePoint(subjectEnemy??e,RegionSymbol(p));
                    return new RegionPoint(point.x/.28f,point.y/.28f);
                }
                case "GetRotateDegree":return subject.facing>0?0f:180f;
                case "GetLineMap":return new RegionNative{name="LineMap",owner=e};
                case "GetNPCStartPos":case "GetStartPosition":{
                    if((call.name=="GetNPCStartPos"||library!=null&&library.name=="LineMap")&&a.spec.landmarks!=null)
                        foreach(var room in a.spec.landmarks)if(room.room==CurrentRoom.id)
                            foreach(var pos in room.points)if(pos.index==key)return new RegionPoint(pos.x/.28f,pos.y/.28f);
                    return subjectEnemy!=null?new RegionPoint(subjectEnemy.spawnX/.28f,subjectEnemy.y/.28f):RegionPosition(subject);
                }
                case "GetNearestUserUnitInSpecificRange_LUA":{
                    var pos=p is RegionPoint?(RegionPoint)p:RegionPosition(e);
                    var target=RegionPosition(player);
                    float dist=(float)Math.Sqrt((pos.x-target.x)*(pos.x-target.x)+(pos.y-target.y)*(pos.y-target.y));
                    return player.hp>0&&dist>=RegionNumber(q)&&dist<=RegionNumber(third)?player:null;
                }
                case "GetNearestGUUser":return local.linked;
                case "SetNearestGUUser":local.linked=p as Fighter;return null;
                case "GetTargetUser":case "GetNearestUserUnit_LUA":return RegionTarget(e);
                case "GetUserUnit":return player.hp>0?(object)player:null;
                
                case "GetNearestUserUnitPos_LUA":case "GetNearestUserPos":case "GetFocusUnitPos_LUA":return RegionPosition(player);
                case "IsMoveableState_LUA":return subject.state==CombatState.Normal||subject.state==CombatState.Airborne;
                case "IsRightTarget_LUA":{
                    var x=p is RegionPoint?(RegionPoint)p:RegionPosition(p as Fighter??subject);
                    var y=q is RegionPoint?(RegionPoint)q:RegionPosition(player);
                    return y.x>=x.x;
                }
                case "GetDist_LUA":case "GetDistance_LUA":case "GetDistanceFrom":case "GetXDistanceFrom":{
                    if(call.name=="GetXDistanceFrom"&&Finite(n))return Math.Abs(subject.x/.28f-n);
                    var x=p is RegionPoint?(RegionPoint)p:RegionPosition(subject);
                    var y=q is RegionPoint?(RegionPoint)q:p is Fighter?RegionPosition(p):p is RegionPoint?RegionPosition(subject):RegionPosition(player);
                    float dx=x.x-y.x,dy=x.y-y.y;
                    return call.name=="GetXDistanceFrom"?Math.Abs(dx):(float)Math.Sqrt(dx*dx+dy*dy);
                }
                case "GetVector_LUA":{object v;return local.vectors.TryGetValue(key,out v)?v:new RegionPoint();}
                case "SetVector_LUA":if(key>=0&&q is RegionPoint)local.vectors[key]=q;return null;
                case "GetNowStateID":
                    // The Spriggan scripts compare the hidden shadow and casting body to numeric engine state IDs.
                    if(local.node.id=="SPRIGGAN_BIG_HIDE")return 7f;
                    if(local.node.id=="SPRIGGAN_SMALL_SPECIAL_ATTACK")return 6f;
                    return local.node.id;
                case "ClearHitUnitList_LUA":local.hitWindows.Clear();local.nextContact.Clear();return null;
                case "SetDamageData_LUA":{
                    string table=RegionSymbol(p);
                    if(local.node.damageTables!=null)foreach(var value in local.node.damageTables)if(value.id==table){local.damageTable=value;break;}
                    return null;
                }
                case "GetStateID":case "GetStateID_LUA":return RegionSymbol(p);
                case "GetStartState":return local.spec.start;
                case "GetNearNPCUnitUID_LUA":{
                    string kind="";foreach(var arg in raw)if(RegionSymbol(arg).StartsWith("NUI_",StringComparison.Ordinal))kind=RegionSymbol(arg);
                    Enemy nearest=null;float best=Single.PositiveInfinity;
                    foreach(var other in enemies)if(other.active&&!other.dormant&&other.hp>0&&other.sourceNPC==kind) {
                        float distance=Math.Abs(other.x-subject.x);if(distance<best){best=distance;nearest=other;}
                    }
                    return nearest==null?0f:RegionUnitId(nearest);
                }
                case "GetNPCUnitByUID":return p is Enemy?p:enemies.Find(other=>RegionUnitId(other)==n);
                case "KillNPC":{
                    foreach(var other in enemies)if(other.active&&other.sourceNPC==RegionSymbol(p))other.hp=0;
                    return null;
                }
                case "GetKeyCode":return subjectEnemy!=null?(float)subjectEnemy.sourceKeyCode:0f;
                case "GetConsultNpc":{
                    string kind=RegionSymbol(p);
                    if(!kind.StartsWith("NUI_",StringComparison.Ordinal))kind=RegionSymbol(q);
                    Enemy best=null;float distance=Single.PositiveInfinity;
                    foreach(var other in enemies){
                        if(other==subject||other.hp<=0||!other.active||other.dormant||other.sourceNPC!=kind)continue;
                        float delta=Math.Abs(other.x-subject.x);if(delta<distance){best=other;distance=delta;}
                    }
                    return best;
                }
                case "GetGroupAICommand_LUA":return local.command;
                case "SetGroupAISelfCommandTransit_LUA":local.selfCommand=RegionTruth(p);return null;
                case "SetGroupAICommand_LUA":case "COMMAND_SKILL_LUA":
                    local.command=RegionSymbol(p);local.selfCommand=true;return null;
                case "COMMAND_FORCED_MOVE_TARGET_LUA":
                    local.command="GAI_CMD_FORCED_MOVE";local.commander=p as Enemy;local.selfCommand=true;return null;
                case "COMMAND_ATTACK_MOVE_LUA":local.command=null;return null;
                case "GetHandle":return call.owner is RegionVisual?call.owner:subject;
                case "SetNowHit":if(subject==player&&!RegionTruth(p))a.lastPlayerHit=PlayerImpactSerial;return null;
                case "GetNowHit":return subject==player&&PlayerImpactSerial>a.lastPlayerHit;
                case "GetRealDamage":return subject==player?regionLastPlayerDamage:0f;
                case "GetSpeedX":return subject.vx/.28f;
                case "GetSpeedY":return subject.vy/.28f;
                case "SetSpeedX":if(!Single.IsNaN(n))subject.vx=n*.28f;return null;
                case "SetSpeedY":if(!Single.IsNaN(n)){subject.vy=n*.28f;if(subject.vy>0)subject.groundLine=-1;}return null;
                case "SetAnimSpeed_LUA":
                    if(subject==player&&n>0)regionLinkerAnimation=n;else if(n>0)local.animSpeed=n;return null;
                case "SetPosition":case "SetPositionForce":case "SetLandPosition":
                    if(call.owner is RegionVisual&&p is RegionPoint)((RegionVisual)call.owner).position=(RegionPoint)p;
                    if(call.owner is Fighter&&p is RegionPoint){var pos=(RegionPoint)p;if(!Single.IsNaN(pos.x)&&!Single.IsNaN(pos.y)){subject.x=Clamp(pos.x*.28f,8,RoomWidth-8);subject.y=pos.y*.28f;subject.groundLine=-1;if(q is bool)subject.facing=(bool)q?1:-1;}}
                    return null;
                case "GetUserUnitNum_LUA":case "LiveUserUnitNum":return player.hp>0?1f:0f;
                case "LiveActiveNPCNum":return (float)enemies.FindAll(x=>x.hp>0&&x.active&&!x.dormant).Count;
                case "LiveNPCNumType_LUA":case "GetNPCUnitByType":case "GetKeyCodeNPC":{
                    string npc=RegionSymbol(p);int number=0;
                    foreach(var other in enemies)if(other.hp>0&&other.active&&!other.dormant&&
                            (call.name=="GetKeyCodeNPC"?other.sourceKeyCode==key:other.sourceNPC==npc)){
                        if(call.name!="LiveNPCNumType_LUA")return other;
                        number++;
                    }
                    if(call.name=="GetNPCUnitByType")foreach(var other in enemies)if(other.encounterIndex==e.encounterIndex&&other.sourceNPC==npc)return other;
                    return call.name=="LiveNPCNumType_LUA"?(object)(float)number:null;
                }
                case "StateChange_LUA":{
                    string next=RegionSymbol(p);
                    if(subjectEnemy!=null&&subjectEnemy!=e)RequestRegionNpcState(subjectEnemy,next);
                    else if(local.spec.nodes.ContainsKey(next))local.request=next;
                    return null;
                }
                case "CreateNPCReq_LUA":case "CreateNPCReq_LUA2":case "PushCreateNPCReq_Lua":case "CreateNPCReqAlly":case "CreateNPCReqElite":{
                    string npc=RegionSymbol(p);
                    if(npc.Length==0 && call.name=="CreateNPCReqElite" && IsHamelSurvivalSpawner(e))npc=HamelSurvivalNpc(key);
                    RegionActorMechanics spec;
                    if(!npc.StartsWith("NUI_",StringComparison.Ordinal)||!regionActors.TryGetValue(npc.Substring(4).ToLowerInvariant(),out spec))return null;
                    var pos=RegionPosition(e);
                    int facing=e.facing;
                    // NPC id, difficulty, elite, world position, facing, key code, enabled.
                    object arg=RegionArg(raw,offset+3);if(arg is RegionPoint)pos=(RegionPoint)arg;
                    arg=RegionArg(raw,offset+4);if(arg is bool)facing=(bool)arg?1:-1;
                    if(Single.IsNaN(pos.x)||Single.IsNaN(pos.y))return null;
                    object spawnOptions=RegionArg(raw,offset+5);
                    int keyCode=spawnOptions is RegionPoint?(int)((RegionPoint)spawnOptions).y:RegionId(spawnOptions);
                    var child=new Enemy{sourceNPC=npc,kind=spec.id,x=Clamp(pos.x*.28f,8,RoomWidth-8),y=pos.y*.28f,
                        facing=facing,hp=spec.boss?480:110,maxHp=spec.boss?480:110,active=true,aiEnabled=true,countsForClear=false,
                        nativeDepth=e.nativeDepth,encounterIndex=e.encounterIndex,sourceKeyCode=Math.Max(0,keyCode)};
                    if(call.name=="CreateNPCReqAlly"){child.regionalAlly=true;child.passive=true;}
                    child.spawnX=child.x;regionSummons.Add(child);return child;
                }
                case "IsSprigganBigHideState":
                    return !enemies.Exists(x=>x.kind=="spriggan_big"&&x.active&&x.hp>0&&x.npcState!="SPRIGGAN_BIG_HIDE");
                case "GetDamageEffect":case "GetDamageEffect_LUA":case "GetEffectSet":case "GetEffectSet_LUA":case "GetMajorXMeshPlayer":
                case "GetMajorParticle":case "GetMinorParticle":case "GetXSkinAnim":case "GetX2Camera":case "GetCamera":
                    return new RegionNative{name=call.name,owner=e};
                case "GameUnitCreateSequence_LUA":return new RegionVisual{position=RegionPosition(e)};
                case "GetMajorParticle_LUA":case "GetMajorMeshPlayer_LUA":case "GetMajorMeshPlayerHandle":{
                    object obj;string k=(call.name=="GetMajorParticle_LUA"?"particle":"mesh")+key;
                    return local.visuals.TryGetValue(k,out obj)?obj:null;
                }
                case "SetMajorParticle_LUA":case "SetMajorMeshPlayer_LUA":{
                    string k=(call.name=="SetMajorParticle_LUA"?"particle":"mesh")+key;
                    local.visuals[k]=q;return null;
                }
                case "ClearMajorParticle_LUA":local.visuals.Remove("particle"+key);return null;
                case "ClearMajorMeshPlayer_LUA":local.visuals.Remove("mesh"+key);return null;
                case "SetAutoDie":if(call.owner is RegionVisual)((RegionVisual)call.owner).alive=false;return null;
                case "DestroyInstance":if(p is RegionVisual)((RegionVisual)p).alive=false;return null;
                case "IsLiveInstance":return p is RegionSpellInstance?!((RegionSpellInstance)p).stopped:p is RegionVisual&&((RegionVisual)p).alive;
                case "CreateInstance_LUA":case "CreateInstance_LUA2":{
                    int idIndex=-1;
                    for(int i=offset;i<raw.Length;i++)if(raw[i] is string&&a.spec.projectiles.ContainsKey((string)raw[i])){idIndex=i;break;}
                    if(idIndex>=0){
                        RegionPoint pos=RegionPosition(e);
                        for(int i=idIndex+1;i<raw.Length;i++)if(raw[i] is RegionPoint){pos=(RegionPoint)raw[i];break;}
                        return AddRegionSpell(e,(string)raw[idIndex],pos.x*.28f,pos.y*.28f);
                    }
                    return new RegionVisual{position=RegionPosition(e)};
                }
                case "PlayEffectSet_LUA":case "PlayEffectSet":case "PlayEffectSetWithCustomPos_LUA":{
                    foreach(object arg in raw)if(arg is string) {
                        string set=(string)arg;
                        RegionPoint? origin=null;
                        if(call.name=="PlayEffectSetWithCustomPos_LUA")
                            foreach(object at in raw)if(at is RegionPoint){origin=(RegionPoint)at;break;}
                        ScheduleRegionSet(e,set,origin);RegionEffectCue(e,set);break;
                    }
                    return null;
                }
                default:return RegionMissing(call.name);
            }
        }
        void FlushRegionSummons()
        {
            if(regionSummons.Count==0)return;
            enemies.AddRange(regionSummons);
            regionSummons.Clear();
        }
    }
}
