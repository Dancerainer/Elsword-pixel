using UnityEngine;
namespace ElswordPixel {
    public sealed partial class DemoScene {
        void SelectStartupDungeon(){
            var args=System.Environment.GetCommandLineArgs();
            for(int i=0;i<args.Length;i++){
                string value=null;var arg=args[i];
                if(arg=="--dungeon"&&i+1<args.Length)value=args[++i];
                else if(arg.StartsWith("--dungeon=",System.StringComparison.Ordinal))value=arg.Substring(10);
                if(value==null)continue;
                if(!int.TryParse(value,out int index)||index<0||index>=catalog.dungeons.Length)continue;
                selectedDungeon=previewDungeon=index;
                if(mapDungeons.Count>0)SelectRuntimeDungeonOnMap(index);
                Debug.Log("STARTUP_DUNGEON index="+index+" id="+catalog.dungeons[index].id+" rooms="+catalog.dungeons[index].count);
            }
        }
        int smokeFeature;
        float featureStarted,featureCameraJumpStartTime;
        bool featureJumpCaptured,featureScrollCaptured,featureDownCaptured,featureTerrainCaptured,featureCameraJumpStarted,featureCameraJumpCaptured,featureCameraJumpFinished,featureCameraJumpAirborne;
        // Native-player acceptance drives the same menu actions and jump handler
        // used by keyboard/touch. It never edits HP or skips combat to reach clear.
        bool SmokeFeatures(out Input input){
            input=new Input{skill=-1};
            if(smokeFeature==0){SetMenu("loadout");MenuAction("slot:3");MenuAction("select-id:sonic-blade");MenuAction("equip-selected");featureStarted=wallClock;smokeFeature=1;return true;}
            if(smokeFeature==1){
                if(wallClock-featureStarted>.65f){Capture("05-loadout");smokeFeature=11;}return true;
            }
            if(smokeFeature==11){SetMenu("guide");featureStarted=wallClock;smokeFeature=2;return true;}
            if(smokeFeature==2){
                if(wallClock-featureStarted>.65f){Capture("06-commands");smokeFeature=12;}return true;
            }
            if(smokeFeature==12){SetMenu("");Action("jump");input=ReadInput();smokeFeature=3;return true;}
            if(smokeFeature==3){
                if(!featureJumpCaptured&&game.player.y>game.PlayerGroundY+40){featureJumpCaptured=true;Capture("07-jump");}
                if(featureJumpCaptured&&game.Grounded(game.player)){smokeFeature=4;}
                return true;
            }
            // Exercise vertical follow before combat starts chaining attacks.
            // Wait for the ordinary landing to finish, then use a real dash
            // and jump input. No actor position or camera position is injected.
            if(smokeFeature==4&&!featureCameraJumpStarted){
                if(game.Grounded(game.player)&&!game.Busy()&&game.player.clip=="idle"){
                    input.jump=true;input.dash=true;input.move=1;featureCameraJumpStarted=true;featureCameraJumpStartTime=wallClock;
                }
                return true;
            }
            if(smokeFeature==4&&featureCameraJumpStarted&&!featureCameraJumpFinished){
                if(!game.Grounded(game.player)){
                    featureCameraJumpAirborne=true;
                    // Free dash-jump uses the source 2.2x gravity and peaks
                    // around 79 px; its camera test must fit that real arc.
                    if(!featureCameraJumpCaptured&&game.player.y-game.PlayerGroundY>70&&worldCamera.transform.localPosition.y>138){featureCameraJumpCaptured=true;Capture("11-camera-jump");}
                    input.move=1;input.dash=true;return true;
                }
                if(!featureCameraJumpAirborne&&wallClock-featureCameraJumpStartTime<.5f){input.move=1;input.dash=true;return true;}
                featureCameraJumpFinished=true;smokeFeature=5;
            }
            if(!featureScrollCaptured&&worldCamera.transform.localPosition.x>400){featureScrollCaptured=true;Capture("08-scroll");}
            if(!featureTerrainCaptured&&game.PlayerGroundY>=70&&game.Grounded(game.player)&&worldCamera.transform.localPosition.y>140){featureTerrainCaptured=true;Capture("10-terrain");}
            if(!featureDownCaptured){foreach(var enemy in game.enemies)if(enemy.state==CombatState.Knockdown){featureDownCaptured=true;Capture("09-knockdown");break;}}
            return false;
        }
    }
}
