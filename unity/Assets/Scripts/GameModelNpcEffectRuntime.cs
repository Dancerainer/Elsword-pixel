using System;
namespace ElswordPixel {
    public sealed partial class GameModel {
        bool ReflectNpcEffect(NpcEffectInstance effect,EffectPose pose){
            if(effect.reflected||!effect.definition.reflectable||actionCommand==null||actionTiming==null||actionTiming.command==null||charging||player.hp<=0)return false;
            foreach(var window in actionTiming.hitWindows){
                if(!window.damageDefined||player.clock<window.start||player.clock>window.end)continue;
                var g=GeometryAt(player.clock,actionTiming);if(g==null)continue;var parts=window.enabledAttackBoxes;if(parts==null||parts.Length==0)parts=defaultSwordBoxes;
                foreach(string part in parts){
                    float ax,ay,bx,by;Segment(g,part,player.x,player.y,out ax,out ay,out bx,out by);float dx=bx-ax,dy=by-ay,length=dx*dx+dy*dy;
                    for(int i=0;i<effect.definition.baseSpheres.Length;i+=4){float x,y,z,r;NpcEffectSphere(effect,pose,i,out x,out y,out z,out r);float depth=Math.Max(0,Math.Abs(z)-9.8f);if(depth>r)continue;r=(float)Math.Sqrt(r*r-depth*depth)+g.radius;
                        float u=length<.00001f?0:Clamp(((x-ax)*dx+(y-ay)*dy)/length,0,1),ex=x-ax-u*dx,ey=y-ay-u*dy;if(ex*ex+ey*ey>r*r)continue;
                        effect.reflected=true;effect.facing=player.facing;effect.dirX=player.facing;effect.dirY=0;RemoveNpcSourceEffectRotationV18(effect);OnAlteraCannonBigReflectedV18(effect);TraceNpcEffect("reflect",effect);return true;
                    }
                }
            }return false;
        }
        void EndNpcEffect(NpcEffectInstance effect,string reason,float at){
            if(effect.ended)return;OnMine2SourceEffectEndV20(effect);RemoveAlteraRavenCannonBladeV18(effect);effect.ended=true;npcEffectLiveCount--;EndNpcEffectSounds(effect);TraceNpcEffect(reason,effect,0,at);
#if UNITY_EDITOR
            BelowCannonRetireDiagnosticR786(effect,reason,at);
#endif
            
            string child=reason=="contact"?effect.definition.contactChild:effect.definition.deathChild;
            if(!string.IsNullOrEmpty(child))SpawnNpcEffect(effect.owner,npcEffectDefinitions[child],null,at,effect);
            if(reason!="contact")EmitNpcSourceDeathChildrenV18(effect,at);
            RemoveNpcSourceEffectRotationV18(effect);
        }
        void ContactNpcEffect(NpcEffectInstance effect,EffectPose pose,float at){if(TryContactDragonFireShamanV20(effect,pose,at))return;if(TryContactAlteraRavenMissileRainV18(effect,pose,at))return;if(TryContactAlteraRavenCannonBladeV18(effect,pose,at))return;if(TryContactAlteraRavenMaximumCombatV18(effect,pose,at))return;
            var d=effect.definition;var damage=NpcEffectDamageViewBelowLaserV20(effect);if(damage.physical<=0&&damage.magic<=0&&string.IsNullOrEmpty(damage.contactChild))
#if UNITY_EDITOR
            { BelowCannonContactDiagnosticR786(effect,"no-damage-definition",at,damage.physical,damage.magic);
#endif
            return;
#if UNITY_EDITOR
            }
#endif
            
            if(effect.age<damage.damageFrom)
#if UNITY_EDITOR
            { BelowCannonContactDiagnosticR786(effect,"before-damage-window",at,damage.physical,damage.magic);
#endif
            return;
#if UNITY_EDITOR
            }
#endif
            
            if(damage.damageUntil>0&&effect.age>damage.damageUntil)
#if UNITY_EDITOR
            { BelowCannonContactDiagnosticR786(effect,"after-damage-window",at,damage.physical,damage.magic);
#endif
            return;
#if UNITY_EDITOR
            }
#endif
            
            if(!effect.reflected&&at<effect.nextPlayerHit)
#if UNITY_EDITOR
            { BelowCannonContactDiagnosticR786(effect,"existing-repeat-hit-wait",at,damage.physical,damage.magic);
#endif
            return;
#if UNITY_EDITOR
            }
#endif
            
            for(int i=0;i<d.baseSpheres.Length;i+=4){float x,y,z,r;NpcEffectSphere(effect,pose,i,out x,out y,out z,out r);if(r<=0)
#if UNITY_EDITOR
            { BelowCannonContactDiagnosticR786(effect,"existing-sphere-radius-nonpositive",at,damage.physical,damage.magic,i,x,y,z,r);
#endif
            continue;
#if UNITY_EDITOR
            }
#endif
            
                if(!effect.reflected){
                    float px=Clamp(x,player.x-9.8f,player.x+9.8f),py=Clamp(y,player.y,player.y+42),pz=Clamp(z,-9.8f,9.8f);
#if UNITY_EDITOR
                    float belowDistanceSquaredR989=(px-x)*(px-x)+(py-y)*(py-y)+(pz-z)*(pz-z),belowRadiusSquaredR989=r*r;bool belowRejectedR989=belowDistanceSquaredR989>belowRadiusSquaredR989;
                    BelowCannonWindowGeometryR989(effect,at,i,x,y,z,r,px,py,pz,belowDistanceSquaredR989,belowRadiusSquaredR989,belowRejectedR989);
                    if(belowRejectedR989)
#else
                    if((px-x)*(px-x)+(py-y)*(py-y)+(pz-z)*(pz-z)>r*r)
#endif
#if UNITY_EDITOR
            { BelowCannonContactDiagnosticR786(effect,"existing-sphere-distance-rejected",at,damage.physical,damage.magic,i,x,y,z,r,true,px,py,pz);
#endif
            continue;
#if UNITY_EDITOR
            }
#endif
            
                    if(!string.IsNullOrEmpty(damage.contactChild)){
#if UNITY_EDITOR
            BelowCannonContactDiagnosticR786(effect,"existing-contact-child-branch",at,damage.physical,damage.magic,i,x,y,z,r,true,px,py,pz);
#endif
            if(IsCastleGuidedMissile(effect)){effect.x=x;effect.y=y;effect.z=z;}
            EndNpcEffect(effect,"contact",at);return;}
                    // Only the original two initial Below Cannons may retain pre-round (0,1) damage.
                    if(BelowCannonFractionalScopeR901(effect)){
                        float raw=effect.physicalAttack*damage.physical+effect.magicAttack*damage.magic;
                        if(!Finite(raw))throw new InvalidOperationException("R901 nonfinite scoped Cannon damage");
                        if(raw>0&&raw<1){
                            var fractional=ApplyBelowCannonFractionalHitR901(raw,effect.x-effect.facing,damage.reaction,damage.launch);
#if UNITY_EDITOR
                            BelowCannonFractionalContactDiagnosticR901(effect,at,damage.physical,damage.magic,i,x,y,z,r,px,py,pz,fractional);
#endif
                            if(fractional.appliedHpLoss>0){effect.nextPlayerHit=damage.repeat?at+damage.repeatGap:float.PositiveInfinity;if(!PlayerHasSuperArmor&&player.state!=CombatState.Knockdown&&player.state!=CombatState.GettingUp)player.vx=effect.facing*damage.push;hitstop=Math.Max(hitstop,damage.stopDefender);if(effect.owner!=null&&effect.owner.hp>0)effect.owner.hitPause=Math.Max(effect.owner.hitPause,damage.stopAttacker);TraceNpcFractionalHitR901(effect,fractional,at);EmitNpcHitChild(effect,at,px,py,pz);ApplyNpcPlayerStatus(d,at);}return;
                        }
                    }
                    if(CargoMissileFractionalScopeR1473(effect)){
                        float raw=effect.physicalAttack*damage.physical+effect.magicAttack*damage.magic;
                        if(!Finite(raw))throw new InvalidOperationException("Nonfinite original Cargo missile damage");
                        if(raw>0&&raw<1){
                            var fractional=ApplyBelowCannonFractionalHitR901(raw,effect.x-effect.facing,damage.reaction,damage.launch);
                            if(fractional.appliedHpLoss>0){OnCoreHardPlayerDamageV21();OnPlainOriginalFxPlayerDamageR1304();effect.nextPlayerHit=damage.repeat?at+damage.repeatGap:float.PositiveInfinity;if(!PlayerHasSuperArmor&&player.state!=CombatState.Knockdown&&player.state!=CombatState.GettingUp)player.vx=effect.facing*damage.push;hitstop=Math.Max(hitstop,damage.stopDefender);if(effect.owner!=null&&effect.owner.hp>0)effect.owner.hitPause=Math.Max(effect.owner.hitPause,damage.stopAttacker);TraceNpcFractionalHitR901(effect,fractional,at);EmitNpcHitChild(effect,at,px,py,pz);ApplyNpcPlayerStatus(d,at);ApplyPlainOriginalFxPlayerStatusR1304(effect,at);}return;
                        }
                    }
                    int dealt=ApplyEnemyAttackToPlayer(CoreHardOutgoingAttackDamageR1417(effect.owner,(int)Math.Round(effect.physicalAttack*damage.physical+effect.magicAttack*damage.magic)),effect.x-effect.facing,damage.reaction,damage.launch);
#if UNITY_EDITOR
            BelowCannonContactDiagnosticR786(effect,"actual-ApplyHitToPlayer-return",at,damage.physical,damage.magic,i,x,y,z,r,true,px,py,pz,true,dealt);
#endif
            
                    if(dealt>0){effect.nextPlayerHit=damage.repeat?at+damage.repeatGap:float.PositiveInfinity;if(!PlayerHasSuperArmor&&player.state!=CombatState.Knockdown&&player.state!=CombatState.GettingUp)player.vx=effect.facing*damage.push;hitstop=Math.Max(hitstop,damage.stopDefender);if(effect.owner!=null&&effect.owner.hp>0)effect.owner.hitPause=Math.Max(effect.owner.hitPause,damage.stopAttacker);TraceNpcEffect("hit-player",effect,dealt,at);EmitNpcHitChild(effect,at,px,py,pz);ApplyNpcPlayerStatus(d,at);ApplyPlainOriginalFxPlayerStatusR1304(effect,at);}return;
                }
                float depth=Math.Max(0,Math.Abs(z)-9.8f);if(depth>r)continue;float projected=(float)Math.Sqrt(Math.Max(0,r*r-depth*depth));
                foreach(var enemy in enemies){float next;if(!enemy.active||enemy.dormant||(enemy.passive&&!CoreHardBuffDeviceDamageableR1417(enemy))||enemy.hp<=0||effect.nextEnemyHit.TryGetValue(enemy,out next)&&at<next||!EnemyCircleIntersects(enemy,x,y,projected))continue;
                    if(!string.IsNullOrEmpty(damage.contactChild)){EndNpcEffect(effect,"contact",at);return;}
                    int dealt=ApplySourceHitFromXR1417(enemy,(int)Math.Round(effect.physicalAttack*damage.physical+effect.magicAttack*damage.magic),damage.reaction,damage.push,damage.launch,0,0,damage.stopDefender,(CoreHardSupportScopeR1417()||NpcSourceStandardGuardActorR1419(enemy))?effect.x-effect.facing:player.x);
                    if(dealt>0){effect.nextEnemyHit[enemy]=damage.repeat?at+damage.repeatGap:float.PositiveInfinity;TraceNpcEffect("hit-enemy",effect,dealt,at);EmitNpcHitChild(effect,at,x,y,z);}
                }
            }
            ContactNpcEffectLines(effect,pose,at);
        }
        void TickNpcEffects(){TickCoreHardClosureWorldV21();
            TickNpcLakeVisualOwnershipV18();
            TickAlteraAirshipIntroV18();
            TickDragonIntroV20();
            FlushAlteraDeferredCannonGates();
            TickNpcPlayerStatuses();
            FlushNpcSummonRequests();
            for(int i=npcAttachedSets.Count-1;i>=0;i--){
                var set=npcAttachedSets[i];float age=elapsed-set.bornAt;
                while(set.next<set.definition.emissions.Length&&set.definition.emissions[set.next].start<=age+1e-6f){var e=set.definition.emissions[set.next++];if(!string.IsNullOrEmpty(e.ownerState)&&(set.owner.hp<=0||set.owner.npcState!=e.ownerState))continue;SpawnNpcEffect(set.owner,npcEffectDefinitions[e.effect],e,set.bornAt+e.start);}
                if(set.next>=set.definition.emissions.Length)npcAttachedSets.RemoveAt(i);
            }
            // A child added by contact/expiry runs in this same packet only
            // for the time remaining after its own exact source birth time.
            for(int i=0;i<npcEffects.Count;i++){
                var effect=npcEffects[i];if(effect.ended)continue;if(TickCoreHardLocalCasterV21(effect))continue;
                // BAT declares all three DELETE_EFFECT_SET flags. Cancel its
                // attached contact volume on motion exit, reaction or death;
                // cancellation is disposal, so it does not emit a death child.
                if(effect.emission!=null&&!string.IsNullOrEmpty(effect.emission.ownerState)&&(effect.owner.hp<=0||effect.owner.npcState!=effect.emission.ownerState)){
                    effect.ended=true;npcEffectLiveCount--;EndNpcEffectSounds(effect);TraceNpcEffect("owner-state-end",effect);continue;
                }
                float targetAge=Math.Min(effect.lifetime,Math.Max(0,elapsed-effect.bornAt));
                bool first=effect.updatedAt==effect.bornAt&&effect.age==0;
                if(first){var p=EffectAt(effect.definition,0);if(!ReflectNpcEffect(effect,p))ContactNpcEffect(effect,p,effect.bornAt);}
                while(!effect.ended&&effect.age<targetAge-1e-7f){
                    float step=Math.Min(1f/120,targetAge-effect.age),fromAge=effect.age;var from=EffectAt(effect.definition,effect.age);TurnNpcEffect(effect,step);effect.age+=step;var to=EffectAt(effect.definition,effect.age);
                    if(effect.emission!=null&&effect.emission.follow){NpcAttachment(effect.owner,effect.emission,effect.bornAt+effect.age,out effect.x,out effect.y,out effect.z);effect.facing=effect.owner.facing;}
                    else if(TryMoveAlteraRavenCannonBladeV18(effect,fromAge,effect.age)){}else if(TryMoveAlteraRavenMissileRainV18(effect,fromAge,effect.age)){}else if(effect.definition.projectilePhysics)MoveNpcProjectile(effect,fromAge,effect.age);
                    else{float distance=to.move-from.move;if(!MoveBelowCannonPortGuidanceR863(effect,distance)&&!MoveCastleGuidedMissile(effect,distance,step)){effect.x+=effect.dirX*distance;effect.y+=effect.dirY*distance;}}
                    if(effect.ended)break;
                    effect.alpha=to.alpha;AdvanceNpcEffectSounds(effect,fromAge,effect.age);if(!ReflectNpcEffect(effect,to))ContactNpcEffect(effect,to,effect.bornAt+effect.age);
                }
                effect.updatedAt=elapsed;
                if(!effect.ended&&effect.age>=effect.lifetime-1e-6f)EndNpcEffect(effect,"expire",effect.bornAt+effect.lifetime);
            }
            for(int i=npcEffects.Count-1;i>=0;i--)if(npcEffects[i].ended){RemoveCoreHardCasterV21(npcEffects[i]);RemoveNpcSourceEffectRotationV18(npcEffects[i]);npcEffects.RemoveAt(i);}
        }
    }
}
