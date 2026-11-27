using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ElswordPixel {
    // A deliberate planar assembly, not a reconstruction of per-fragment 3D
    // intersection. The source-depth review and original-mesh GPU comparison
    // establish closed cannons behind the shell and a guarded front gate. The
    // shell's baked back wall cannot be a 3D hole: put the vulnerable core and
    // source-SHOW eyes in front, and expose only the cannon's original Open /
    // Attack / Close animation there. Closed Wait cannons stay behind the shell.
    // Unique internal orders prevent atlas/material allocation from choosing the
    // visible component. The group's external order remains the original carrier.
    internal static class FortPixelLayerV54 {
        internal static bool TryOrder(string npc,string kind,string motion,out int order){
            order=-1;
            bool cannonOpen=motion=="Open"||motion=="Attack"||motion=="Close";
            switch(kind){
                case "cannon-fort-parasite-bottom-eye":order=4;break;
                case "cannon-fort-parasite-bottom-cannon":order=cannonOpen?6:0;break;
                case "cannon-fort-parasite-top-eye":order=5;break;
                case "cannon-fort-parasite-top-cannon":order=cannonOpen?7:1;break;
                case "cannon-fort-parasite-body":order=3;break;
                case "cannon-fort-parasite-body-shell":order=2;break;
                case "cannon-fort-parasite-body-armor":order=8;break;
                default:return false;
            }
            string expected="NUI_"+kind.Replace('-','_').ToUpperInvariant();
            if(!String.Equals(npc,expected,StringComparison.Ordinal)){order=-1;return false;}
            return true;
        }
    }
    public sealed partial class DemoScene {
        readonly Dictionary<int,SortingGroup> fortPixelGroupsV54=new Dictionary<int,SortingGroup>();
        readonly HashSet<int> fortPixelGroupsUsedV54=new HashSet<int>();
        void BeginFortPixelSortingV54(){fortPixelGroupsUsedV54.Clear();}
        void ApplyFortPixelSortingV54(MonsterSelectionV53 selected,SpriteRenderer renderer){
            if(Dungeon.id!="altera-below-tunnel"||!FortPixelLayerV54.TryOrder(selected.actor.sourceNPC,selected.actor.kind,selected.motion,out int order))return;
            int key=selected.actor.encounterIndex;
            if(!fortPixelGroupsV54.TryGetValue(key,out var group)||!group){var node=new GameObject("Fort source assembly / "+key){layer=renderer.gameObject.layer};node.transform.SetParent(actorsRoot,false);group=node.AddComponent<SortingGroup>();fortPixelGroupsV54[key]=group;}
            fortPixelGroupsUsedV54.Add(key);group.sortingLayerID=selected.carrier.sortingLayerID;group.sortingOrder=selected.carrier.sortingOrder;
            var actor=selected.actor;var root=actorsRoot.TransformPoint(new Vector3(actor.x,actor.y,0));group.transform.position=NativeWorldActive?ProjectNativePointWithEye(root,actor.nativeDepth,out _):root;
            renderer.transform.SetParent(group.transform,true);renderer.sortingLayerID=selected.carrier.sortingLayerID;renderer.sortingOrder=order;renderer.spriteSortPoint=SpriteSortPoint.Pivot;
        }
        void EndFortPixelSortingV54(){
            var remove=new List<int>();foreach(var pair in fortPixelGroupsV54)if(!fortPixelGroupsUsedV54.Contains(pair.Key)){if(pair.Value)DestroyFortPixelGroupV54(pair.Value.gameObject);remove.Add(pair.Key);}foreach(int key in remove)fortPixelGroupsV54.Remove(key);
        }
        static void DestroyFortPixelGroupV54(GameObject node){if(Application.isPlaying)UnityEngine.Object.Destroy(node);else UnityEngine.Object.DestroyImmediate(node);}
        void DisposeFortPixelSortingV54(){foreach(var group in fortPixelGroupsV54.Values)if(group)DestroyFortPixelGroupV54(group.gameObject);fortPixelGroupsV54.Clear();fortPixelGroupsUsedV54.Clear();}
    }
}
