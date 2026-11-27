using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel
{
    public sealed partial class DemoScene
    {
        sealed class RecoveryDropView
        {
            public GameObject root;public SpriteRenderer shadow,bottle;public TextMesh label;
        }
        Sprite recoveryHpBottle,recoveryMpBottle;
        TextMesh fullRecoveryNotice;
        float fullNoticeUntil;
        readonly List<RecoveryDropView> recoveryDropViews=new List<RecoveryDropView>();
        int recoveryViewWorld=-1;
        public int RecoveryPickupSoundPlays {get;private set;}
        public int RecoveryPickupMissingSounds {get;private set;}

        SpriteRenderer RecoveryRect(Transform parent,string name,float x,float y,float width,float height,uint color,int order=47)
        {
            var rect=WorldSprite(name,parent,white,order,new Vector3(x,y,0));
            rect.transform.localScale=new Vector3(width/white.bounds.size.x,height/white.bounds.size.y,1);
            rect.color=C(color);return rect;
        }
        TextMesh RecoveryText(Transform parent,string name,int order)
        {
            var go=new GameObject(name);go.transform.SetParent(parent,false);var text=go.AddComponent<TextMesh>();
            text.font=pixelFont?pixelFont:font;text.fontSize=32;text.characterSize=1.45f;text.anchor=TextAnchor.MiddleCenter;
            var renderer=text.GetComponent<MeshRenderer>();renderer.sharedMaterial=text.font.material;renderer.sortingOrder=order;
            return text;
        }
        RecoveryDropView CreateRecoveryDropView()
        {
            var root=new GameObject("Recovery item");root.transform.SetParent(effectsRoot,false);
            var shadow=RecoveryRect(root.transform,"Ground shadow",0,1,12,2,0x182132,43);
            shadow.color=C(0x182132,.5f);
            var bottle=WorldSprite("Pixel recovery bottle",root.transform,recoveryHpBottle,49,Vector3.zero);
            bottle.transform.localScale=Vector3.one*(21f/recoveryHpBottle.bounds.size.y);
            var label=RecoveryText(root.transform,"Recovery item prompt",52);label.transform.localPosition=new Vector3(0,31,0);
            return new RecoveryDropView{root=root,shadow=shadow,bottle=bottle,label=label};
        }
        void ClearRecoveryDropViews()
        {
            foreach(var view in recoveryDropViews)if(view.root)Destroy(view.root);
            recoveryDropViews.Clear();recoveryViewWorld=game.RecoveryDropWorldSerial;
            fullNoticeUntil=0;
            if(fullRecoveryNotice)fullRecoveryNotice.gameObject.SetActive(false);
        }
        void RenderRecoveryDrops()
        {
            if(recoveryViewWorld!=game.RecoveryDropWorldSerial)ClearRecoveryDropViews();
            while(recoveryDropViews.Count<game.recoveryDrops.Count)recoveryDropViews.Add(CreateRecoveryDropView());
            for(int i=0;i<recoveryDropViews.Count;i++){
                var view=recoveryDropViews[i];view.root.SetActive(i<game.recoveryDrops.Count);if(i>=game.recoveryDrops.Count)continue;
                var item=game.recoveryDrops[i];bool hp=item.resource=="HP",near=Math.Abs(game.player.x-item.x)<85&&Math.Abs(game.player.y-item.y)<48;
                view.root.transform.localPosition=new Vector3(Mathf.Round(item.x),Mathf.Round(item.y),0);
                view.bottle.transform.localPosition=new Vector3(0,11+(item.grounded?Mathf.Round(2+Mathf.Sin(item.age*4)*2):0),0);
                view.bottle.sprite=hp?recoveryHpBottle:recoveryMpBottle;
                npcEffectDepths[view.bottle.transform]=item.nativeDepth;
                npcEffectDepths[view.shadow.transform]=item.nativeDepth;
                npcEffectDepths[view.label.transform]=item.nativeDepth;
                string gain=item.percent>0?item.percent.ToString("0")+"%":item.amount.ToString("0");
                view.label.text=near?(game.RecoveryResourceFull(item)?item.resource+" FULL\nMOVE TO COLLECT":item.resource+" +"+gain+"\nMOVE TO COLLECT"):item.resource;
                view.label.color=C(hp?0xff9baeu:0x9adfffu);
            }
            RecoveryPickupEvent pickup;
            while(game.TryDequeueRecoveryPickup(out pickup)){
                if(pickup.worldSerial!=game.RecoveryDropWorldSerial)continue;
                if(pickup.amount==0) {
                    if(!fullRecoveryNotice)fullRecoveryNotice=RecoveryText(effectsRoot,"Recovery collected",52);
                    fullRecoveryNotice.text=pickup.resource+" PICKED UP";
                    fullRecoveryNotice.color=C(0xffebad);
                    fullNoticeUntil=game.elapsed+.75f;
                }
                if(audioClips.ContainsKey("getitem_common")){
                    int before=AudioPlayCount;PlayNativeSound(new TimedSound{sound="getitem_common",volume=.50f},70);
                    if(AudioPlayCount>before)RecoveryPickupSoundPlays++;
                }else{RecoveryPickupMissingSounds++;if(RecoveryPickupMissingSounds==1)Debug.LogWarning("Recovery pickup SFX missing: Pixel/audio/getitem_common");}
            }
            if(fullRecoveryNotice){
                bool visible=game.elapsed<fullNoticeUntil;
                fullRecoveryNotice.gameObject.SetActive(visible);
                if(visible)fullRecoveryNotice.transform.localPosition=new Vector3(game.player.x,game.player.y+55+(fullNoticeUntil-game.elapsed)*-8,0);
            }
        }
    }
}
