using System;
namespace ElswordPixel {
 public sealed partial class GameModel {
  // The original line3.start == line2.end and line2.start == line1.end.
  // The frozen 2D projection stores these three source charts at different Xs.
  // Crossing a shared native endpoint changes charts, never native scenery.
  RoomDefinition cargoProjectionRoomR1479;
  bool cargoProjectionBridgeR1479;
  int cargoProjectionChartSerialR1479;
  public int CargoProjectionChartSerialR1479 {get{return cargoProjectionChartSerialR1479;}}
  public bool CargoProjectionBridgeActiveR1479 {get{return CargoProjectionChartsScopeR1479()&&EncounterIndex==0&&cargoProjectionBridgeR1479&&ReferenceEquals(cargoProjectionRoomR1479,CurrentRoom);}}
  void ClearCargoProjectionChartsR1479(){
   if(cargoProjectionRoomR1479!=null||cargoProjectionBridgeR1479)cargoProjectionChartSerialR1479++;
   cargoProjectionRoomR1479=null;cargoProjectionBridgeR1479=false;
  }
  bool CargoProjectionChartsScopeR1479(){
   return cargoSourceV21!=null&&NativeTerrain&&room==0&&CurrentRoom.id=="Dungeon_Besma_Air_Ship_Normal-0"&&HasEncounters&&CurrentRoom.encounters.Length==2;
  }
  void CargoProjectionLinesR1479(out GroundSurface incoming,out GroundSurface bridge,out GroundSurface outgoing){
   incoming=Surface(1);bridge=Surface(2);outgoing=Surface(3);
   // Exact authored projection 4a2a6520 / native line map 6ac47072. These
   // checks reject a different room/geometry instead of guessing a floor.
   if(incoming==null||bridge==null||outgoing==null||!incoming.platform||bridge.platform||outgoing.platform||
    Math.Abs(incoming.x1-3070.4077876f)>.002f||Math.Abs(incoming.x2-3210.4077876f)>.002f||Math.Abs(incoming.y2-435.0828f)>.002f||
    Math.Abs(bridge.x1-2507.0148183f)>.002f||Math.Abs(bridge.x2-2718.8921285f)>.002f||Math.Abs(bridge.y1-435.0828f)>.002f||Math.Abs(bridge.y2-502.2828f)>.002f||
    Math.Abs(outgoing.x1-149.6f)>.002f||Math.Abs(outgoing.y1-502.2828f)>.002f)
    throw new InvalidOperationException("Exact original Cargo line3/2/1 projection endpoints required");
   if(cargoProjectionRoomR1479!=null&&!ReferenceEquals(cargoProjectionRoomR1479,CurrentRoom))ClearCargoProjectionChartsR1479();
   cargoProjectionRoomR1479=CurrentRoom;
  }
  bool TryCargoProjectionLinkMoveR1479(Fighter actor,float wanted){
   if(!ReferenceEquals(actor,player)||!CargoProjectionChartsScopeR1479()||EncounterIndex!=0)return false;
   GroundSurface one,two,three;CargoProjectionLinesR1479(out one,out two,out three);bool grounded=NativeGrounded(player);
   bool airborne=player.state==CombatState.Airborne||player.state==CombatState.Hurt;
   if(!cargoProjectionBridgeR1479&&player.groundLine==three.id&&player.x>=three.x1-.02f&&wanted<three.x1&&(grounded||airborne&&player.y>=three.y1-.02f)){
    if(Math.Abs(ClipPlayerBodyMovement(player.x,player.y,three.x1,grounded?three.y1:player.y)-three.x1)>.001f)return false;
    float remainder=three.x1-wanted;float mapped=Math.Max(two.x1,two.x2-remainder);
    mapped=ClipPlayerBodyMovement(two.x2,grounded?two.y2:player.y,mapped,grounded?two.Y(mapped):player.y);
    cargoProjectionBridgeR1479=true;cargoProjectionChartSerialR1479++;
    player.x=mapped;if(grounded)player.y=two.Y(mapped);player.groundLine=two.id;return true;
   }
   if(cargoProjectionBridgeR1479&&player.groundLine==two.id&&player.x<=two.x2+.02f&&wanted>two.x2&&(grounded||airborne&&player.y>=two.y2-.02f)){
    if(Math.Abs(ClipPlayerBodyMovement(player.x,player.y,two.x2,grounded?two.y2:player.y)-two.x2)>.001f)return false;
    float remainder=wanted-two.x2;float mapped=Math.Min(three.x2,three.x1+remainder);
    mapped=ClipPlayerBodyMovement(three.x1,grounded?three.y1:player.y,mapped,grounded?three.Y(mapped):player.y);
    cargoProjectionBridgeR1479=false;cargoProjectionChartSerialR1479++;
    player.x=mapped;if(grounded)player.y=three.Y(mapped);player.groundLine=three.id;return true;
   }
   return false;
  }
  bool TryCargoProjectionEncounterClampR1479(Fighter actor,float wanted,out float result){
   result=wanted;if(!ReferenceEquals(actor,player)||!cargoProjectionBridgeR1479||!CargoProjectionChartsScopeR1479()||EncounterIndex!=0)return false;
   GroundSurface one,two,three;CargoProjectionLinesR1479(out one,out two,out three);result=Clamp(wanted,two.x1,two.x2);return true;
  }
  void ReconcileCargoProjectionNavigationGoalR1479(ref float x,ref float y){
   if(!CargoProjectionChartsScopeR1479()||EncounterIndex!=0||!gateOpen||Math.Abs(x-CurrentEncounter.exitX)>.25f)return;
   GroundSurface one,two,three;CargoProjectionLinesR1479(out one,out two,out three);
   // Reach the actual lower line3 start first; ordinary NativeNavigate owns
   // any necessary drop/jump. Once on bridge, walk LEFT to its real curtain.
   x=cargoProjectionBridgeR1479?two.x1:three.x1+.025f;y=cargoProjectionBridgeR1479?two.y1:three.y1;
  }
  void ReconcileCargoOriginalCurtainExitR1479(EncounterDefinition definition,ref float x){
   if(!CargoProjectionChartsScopeR1479()||EncounterIndex!=0||!ReferenceEquals(definition,CurrentEncounter))return;
   GroundSurface one,two,three;CargoProjectionLinesR1479(out one,out two,out three);x=two.x1;
  }
  bool CargoOriginalCurtainContactR1479(){
   if(!CargoProjectionChartsScopeR1479()||EncounterIndex!=0)return true;
   GroundSurface one,two,three;CargoProjectionLinesR1479(out one,out two,out three);
   return cargoProjectionBridgeR1479&&Grounded(player)&&player.groundLine==two.id&&Math.Abs(player.x-two.x1)<.001f;
  }
  void ReconcileCargoLinkedEncounterEntryR1479(int previous,int next){
   if(!CargoProjectionChartsScopeR1479()||previous!=0||next!=1)return;
   GroundSurface one,two,three;CargoProjectionLinesR1479(out one,out two,out three);
   if(!cargoProjectionBridgeR1479||player.groundLine!=two.id||Math.Abs(player.x-two.x1)>.001f||Math.Abs(player.y-two.y1)>.002f)
    throw new InvalidOperationException("Cargo original curtain line2.start must precede line1.end entry");
   // The original curtain endpoint is exactly line1.end in native XYZ.
   // Preserve HP, MP, action/source clocks, velocities, and the same model.
   player.x=one.x2;player.y=one.y2;player.groundLine=one.id;
   cargoProjectionBridgeR1479=false;cargoProjectionChartSerialR1479++;
  }
 }
}
