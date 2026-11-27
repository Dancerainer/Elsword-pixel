using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
    public sealed partial class DemoScene {
        const float SwordRibbonLifetime=.14f;
        const float SwordRibbonEdgeFrames=2;
        bool showCollisionReview;
        Material combatLineMaterial;
        GameObject swordRibbonObject;
        Mesh swordRibbonMesh;
        MeshRenderer swordRibbonRenderer;
        readonly List<LineRenderer> collisionLines=new List<LineRenderer>();
        struct SwordRibbonSample {public Vector3 bladeBase,bladeTip;public float born;public int strip;}
        readonly List<SwordRibbonSample> swordRibbonSamples=new List<SwordRibbonSample>(64);
        readonly List<Vector3> swordRibbonVertices=new List<Vector3>(128);
        readonly List<Color> swordRibbonColors=new List<Color>(128);
        readonly List<Vector2> swordRibbonUV=new List<Vector2>(128);
        readonly List<int> swordRibbonTriangles=new List<int>(384);
        readonly List<float> swordRibbonTimes=new List<float>(64);
        string swordRibbonClip="";
        Vector2 swordRibbonActor;
        float swordRibbonClock,swordRibbonAge,swordRibbonModelTime,swordRibbonPreviousAge;
        int swordRibbonFacing,swordRibbonRoom=-1,swordRibbonStrip,swordRibbonWindow=-1;
        bool swordRibbonObserved;
        RoomDefinition swordRibbonRoomData;

        Material CombatMaterial(){
            if(!combatLineMaterial){combatLineMaterial=new Material(Shader.Find("Sprites/Default"));combatLineMaterial.name="Combat ribbon / vertex fade";combatLineMaterial.mainTexture=Texture2D.whiteTexture;}
            return combatLineMaterial;
        }
        LineRenderer CombatLine(string name,float width,Color color){
            var obj=new GameObject(name);obj.transform.SetParent(effectsRoot,false);
            var line=obj.AddComponent<LineRenderer>();line.sharedMaterial=CombatMaterial();line.useWorldSpace=true;
            line.startWidth=line.endWidth=width;line.startColor=line.endColor=color;line.sortingOrder=44;
            line.numCapVertices=0;line.numCornerVertices=0;return line;
        }
        void EnsureSwordRibbon(){
            if(swordRibbonObject)return;
            swordRibbonObject=new GameObject("Sword motion ribbon");swordRibbonObject.transform.SetParent(effectsRoot,false);
            swordRibbonMesh=new Mesh{name="World-space source blade history"};swordRibbonMesh.MarkDynamic();
            swordRibbonObject.AddComponent<MeshFilter>().sharedMesh=swordRibbonMesh;
            swordRibbonRenderer=swordRibbonObject.AddComponent<MeshRenderer>();swordRibbonRenderer.sharedMaterial=CombatMaterial();swordRibbonRenderer.sortingOrder=44;swordRibbonRenderer.enabled=false;
        }
        // An empty enabledAttackBoxes list means the original default Sword box,
        // just as it does in the model. Foot-only and effect-only windows are excluded.
        float SwordWindowPadding(AnimationData clip){
            // Command animation follows through outside its damage frames.
            return clip.command!=null && clip.fps>0 ? SwordRibbonEdgeFrames/clip.fps : 0;
        }
        int SwordWindow(AnimationData clip,float clock){
            float padding=SwordWindowPadding(clip);
            if(clip.hits==null)return -1;
            for(int i=0;i<clip.hits.Length;i++){
                var hit=clip.hits[i];if(!hit.damageDefined||clock<hit.start-padding-.000001f||clock>hit.end+padding+.000001f)continue;
                if(hit.enabledAttackBoxes==null||hit.enabledAttackBoxes.Length==0)return i;
                foreach(string box in hit.enabledAttackBoxes)if(string.Equals(box,"Sword",StringComparison.OrdinalIgnoreCase))return i;
            }
            return -1;
        }
        bool BladeGeometry(AnimationData clip,float clock,out Vector2 bladeBase,out Vector2 bladeTip){
            bladeBase=bladeTip=Vector2.zero;if(clip.frames==null||clip.frames.Length==0||clip.fps<=0)return false;
            float local=clip.loop&&clip.duration>0?Mathf.Repeat(clock,clip.duration):Mathf.Max(0,clock);
            float frame=local*clip.fps;int a=Mathf.Clamp(Mathf.FloorToInt(frame),0,clip.frames.Length-1),b=Mathf.Min(a+1,clip.frames.Length-1);float blend=b==a?0:Mathf.Clamp01(frame-a);
            if(clip.loop&&a==clip.frames.Length-1&&clip.frames.Length>1){b=0;float span=clip.duration-a/clip.fps;blend=span>.000001f?Mathf.Clamp01((local-a/clip.fps)/span):0;}
            var first=clip.frames[a].attackGeometry;var second=clip.frames[b].attackGeometry;
            if(first==null||second==null||first.weaponBase==null||first.weaponTip==null||second.weaponBase==null||second.weaponTip==null||first.weaponBase.Length<2||first.weaponTip.Length<2||second.weaponBase.Length<2||second.weaponTip.Length<2)return false;
            bladeBase=Vector2.Lerp(new Vector2(first.weaponBase[0],first.weaponBase[1]),new Vector2(second.weaponBase[0],second.weaponBase[1]),blend);
            Vector2 from=new Vector2(first.weaponTip[0]-first.weaponBase[0],first.weaponTip[1]-first.weaponBase[1]);
            Vector2 to=new Vector2(second.weaponTip[0]-second.weaponBase[0],second.weaponTip[1]-second.weaponBase[1]);
            if(blend<=.00001f)bladeTip=bladeBase+from;
            else if(blend>=.99999f)bladeTip=bladeBase+to;
            else{
                float start=Mathf.Atan2(from.y,from.x)*Mathf.Rad2Deg,end=Mathf.Atan2(to.y,to.x)*Mathf.Rad2Deg;
                float angle=(start+Mathf.DeltaAngle(start,end)*blend)*Mathf.Deg2Rad;
                bladeTip=bladeBase+new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*Mathf.Lerp(from.magnitude,to.magnitude,blend);
            }
            return (bladeTip-bladeBase).sqrMagnitude>.0001f;
        }
        void AppendSwordSample(AnimationData clip,float clock,Vector2 actor,int facing,float born){
            int window=SwordWindow(clip,clock);Vector2 bladeBase,bladeTip;
            if(window<0||!BladeGeometry(clip,clock,out bladeBase,out bladeTip)){swordRibbonWindow=-1;return;}
            if(window!=swordRibbonWindow){swordRibbonStrip++;swordRibbonWindow=window;}
            // Store world coordinates now: later actor travel never drags an old cut.
            Vector3 a=actorsRoot.TransformPoint(new Vector3(actor.x+facing*bladeBase.x,actor.y+bladeBase.y,0));
            Vector3 b=actorsRoot.TransformPoint(new Vector3(actor.x+facing*bladeTip.x,actor.y+bladeTip.y,0));
            if(swordRibbonSamples.Count>0){var last=swordRibbonSamples[swordRibbonSamples.Count-1];if(last.strip==swordRibbonStrip&&(last.bladeBase-a).sqrMagnitude<.000001f&&(last.bladeTip-b).sqrMagnitude<.000001f)return;}
            swordRibbonSamples.Add(new SwordRibbonSample{bladeBase=a,bladeTip=b,born=born,strip=swordRibbonStrip});
            if(swordRibbonSamples.Count>128)swordRibbonSamples.RemoveAt(0);
        }
        void CollectBladeSubsteps(AnimationData clip,float from,float to){
            float fps=clip.fps;
            if(fps>0){
                int first=Mathf.FloorToInt(from*fps),end=Mathf.CeilToInt(to*fps);
                for(int frame=first;frame<end;frame++){
                    // Slow poses need one edge. A fast cut can rotate over 100 degrees
                    // in one source frame. Tessellate only that part of the arc.
                    float start=frame/fps,finish=(frame+1)/fps;
                    int steps=1;
                    if(BladeGeometry(clip,start,out var baseA,out var tipA)&&BladeGeometry(clip,finish,out var baseB,out var tipB))
                        steps=Mathf.Max(1,Mathf.CeilToInt(Vector2.Angle(tipA-baseA,tipB-baseB)/16));
                    for(int n=1;n<=steps;n++){
                        float t=Mathf.Lerp(start,finish,(float)n/steps);
                        if(t>from&&t<to)swordRibbonTimes.Add(t);
                    }
                }
            }
            swordRibbonTimes.Add(to);
        }
        void UpdateSwordRibbon(){
            EnsureSwordRibbon();if(!game.paused&&game.hitstop<=0)swordRibbonAge+=Time.unscaledDeltaTime;
            float clock=RenderPlayerClock;Vector2 actor=RenderPlayerPosition;int facing=game.player.facing;
            bool reset=swordRibbonRoom!=game.room||game.CurrentRoom!=swordRibbonRoomData||game.elapsed+.0001f<swordRibbonModelTime
                ||(swordRibbonObserved&&(actor-swordRibbonActor).sqrMagnitude>160*160);
            if(reset){swordRibbonSamples.Clear();swordRibbonObserved=false;swordRibbonWindow=-1;swordRibbonStrip++;}
            for(int i=swordRibbonSamples.Count-1;i>=0;i--)if(swordRibbonAge-swordRibbonSamples[i].born>=SwordRibbonLifetime)swordRibbonSamples.RemoveAt(i);
            Library library;AnimationData clip=null;
            bool hasClip=libraries.TryGetValue(catalog.hero.id,out library)&&library.clips.TryGetValue(game.player.clip,out clip);
            bool discontinuity=!swordRibbonObserved||game.player.clip!=swordRibbonClip||facing!=swordRibbonFacing||clock+.00001f<swordRibbonClock;
            if(discontinuity){swordRibbonWindow=-1;swordRibbonStrip++;}
            if(!game.paused&&hasClip&&clip.hits!=null&&clip.hits.Length>0){
                // Frozen animation clocks generate no samples, even at 120 Hz rendering.
                if(discontinuity)AppendSwordSample(clip,clock,actor,facing,swordRibbonAge);
                else if(clock>swordRibbonClock+.000001f){
                    swordRibbonTimes.Clear();float from=Mathf.Max(swordRibbonClock,clock-SwordRibbonLifetime);
                    CollectBladeSubsteps(clip,from,clock);
                    // Retain exact contact edges; a short source window must not
                    // vanish merely because its endpoints fall between 60 Hz ticks.
                    float padding=SwordWindowPadding(clip);
                    if(clip.hits!=null)foreach(var hit in clip.hits){
                        float start=hit.start-padding,end=hit.end+padding;
                        if(start>from&&start<clock)swordRibbonTimes.Add(start);
                        if(end>from&&end<clock)swordRibbonTimes.Add(end);
                    }
                    swordRibbonTimes.Sort();float previous=-1;
                    foreach(float sample in swordRibbonTimes){if(sample-previous<.000001f)continue;previous=sample;float u=Mathf.Clamp01((sample-swordRibbonClock)/(clock-swordRibbonClock));AppendSwordSample(clip,sample,Vector2.Lerp(swordRibbonActor,actor,u),facing,Mathf.Lerp(swordRibbonPreviousAge,swordRibbonAge,u));}
                }
            }
            swordRibbonPreviousAge=swordRibbonAge;swordRibbonRoomData=game.CurrentRoom;swordRibbonObserved=true;swordRibbonClip=game.player.clip;swordRibbonClock=clock;swordRibbonActor=actor;swordRibbonFacing=facing;swordRibbonRoom=game.room;swordRibbonModelTime=game.elapsed;
            BuildSwordRibbonMesh();
        }
        void BuildSwordRibbonMesh(){
            swordRibbonVertices.Clear();swordRibbonColors.Clear();swordRibbonUV.Clear();swordRibbonTriangles.Clear();
            for(int i=0;i<swordRibbonSamples.Count;i++){
                var sample=swordRibbonSamples[i];float fade=1-Mathf.Clamp01((swordRibbonAge-sample.born)/SwordRibbonLifetime);fade*=fade;
                // Leave the fingers clear and keep a thin rim of light along the cutting tip.
                Vector3 hilt=swordRibbonObject.transform.InverseTransformPoint(ProjectNativePoint(sample.bladeBase));
                Vector3 tip=swordRibbonObject.transform.InverseTransformPoint(ProjectNativePoint(sample.bladeTip));
                swordRibbonVertices.Add(Vector3.Lerp(hilt,tip,.25f));swordRibbonVertices.Add(Vector3.Lerp(hilt,tip,.9f));swordRibbonVertices.Add(tip);
                swordRibbonColors.Add(C(0xffbd79,.025f*fade));swordRibbonColors.Add(C(0xffda9e,.39f*fade));swordRibbonColors.Add(C(0xfff4d3,.8f*fade));
                swordRibbonUV.Add(Vector2.zero);swordRibbonUV.Add(Vector2.right*.9f);swordRibbonUV.Add(Vector2.right);
                if(i>0&&sample.strip==swordRibbonSamples[i-1].strip){
                    int p=(i-1)*3;
                    for(int band=0;band<2;band++,p++){
                        swordRibbonTriangles.Add(p);swordRibbonTriangles.Add(p+1);swordRibbonTriangles.Add(p+3);
                        swordRibbonTriangles.Add(p+3);swordRibbonTriangles.Add(p+1);swordRibbonTriangles.Add(p+4);
                    }
                }
            }
            if(swordRibbonMesh.vertexCount!=0)swordRibbonMesh.Clear();swordRibbonRenderer.enabled=swordRibbonTriangles.Count>0;
            if(!swordRibbonRenderer.enabled)return;
            swordRibbonMesh.SetVertices(swordRibbonVertices);swordRibbonMesh.SetColors(swordRibbonColors);swordRibbonMesh.SetUVs(0,swordRibbonUV);swordRibbonMesh.SetTriangles(swordRibbonTriangles,0);swordRibbonMesh.RecalculateBounds();
        }
        void RenderCombatGeometry(){
            UpdateSwordRibbon();int visible=showCollisionReview?game.ActiveHitBoxes.Count:0;
            while(collisionLines.Count<visible)collisionLines.Add(CombatLine("Active collision volume",.7f,C(0x6bffb7,.9f)));
            for(int i=0;i<collisionLines.Count;i++){
                var line=collisionLines[i];line.enabled=i<visible;if(i>=visible)continue;var box=game.ActiveHitBoxes[i];line.positionCount=5;
                line.SetPosition(0,ProjectNativePoint(new Vector3(box.left,box.bottom,0)));line.SetPosition(1,ProjectNativePoint(new Vector3(box.right,box.bottom,0)));line.SetPosition(2,ProjectNativePoint(new Vector3(box.right,box.top,0)));line.SetPosition(3,ProjectNativePoint(new Vector3(box.left,box.top,0)));line.SetPosition(4,ProjectNativePoint(new Vector3(box.left,box.bottom,0)));
            }
        }
        void OnDestroy(){DisposeContactClips();DisposeGlassCurtain();DisposeMonsterPixelViewsV53();DisposeCombatFeedback();CancelMobileRoomWarmup();DisposeBendersViewsV36();DisposeBattleControls();DisposeBeastsV45();DisposeSourceReactionInkV45();DisposeBatBPaintV41();DisposeRubenForestPaintV40();try{if(plainProductSessionR1320)DisposePlainProductR1320(false);}finally{
            try{if(battleProductSessionR1325)DisposeBattleProductR1325(false);}
            finally{
            try{if(cargoProductSessionR1288)DisposeCargoProductR1288(false);}
            finally{
            try{if(mine2ProductSessionR1233)DisposeMine2ProductR1233(false);}
            finally{
            try{if(factoryProductSessionR1179)DisposeFactoryProductR1179(false);}
            finally{
            try{if(coreProductSessionR1147)DisposeCoreHardProductR1147(false);}
            finally{
            try{DisposeBelowPlayerCandidateV21(false);}
            finally{
#if UNITY_EDITOR
            DisposeCoreHardControlledEntryV21();
#endif
            ClearCoreKingWholeLibraryV21();
#if UNITY_EDITOR
            DisposeForestPreview();
#endif
            DisposeNativeWorld();
            DisposeNpcSteamVisual();
            DisposeOutskirtsNpcVisual();
            DisposePresentation();if(swordRibbonMesh)Destroy(swordRibbonMesh);if(swordRibbonObject)Destroy(swordRibbonObject);
            foreach(var line in collisionLines)if(line)Destroy(line.gameObject);if(combatLineMaterial)Destroy(combatLineMaterial);
        }
        
            }
        
            }
                    }
        
            }
            }
        }
            }
    }
}
