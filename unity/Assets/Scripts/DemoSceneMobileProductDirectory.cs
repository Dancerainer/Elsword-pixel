using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        [Serializable] sealed class MobileProductDirectory { public CandidateDungeonArt[] routes; }
        readonly Dictionary<string,CandidateDungeonArt> mobileProductPreviews = new Dictionary<string,CandidateDungeonArt>(StringComparer.Ordinal);
        readonly Dictionary<string,string> mobileProductFailures = new Dictionary<string,string>(StringComparer.Ordinal);
        readonly HashSet<string> mobileInstalledProducts = new HashSet<string>(StringComparer.Ordinal);
        bool mobileProductEntryRunning;
        object preparedMobileIndex;

        bool UseMobileProductDirectory { get { return Application.isPlaying && Application.platform == RuntimePlatform.Android; } }

        void AddMobileProductPreviews() {
            var asset = Resources.Load<TextAsset>("Pixel/ui/dungeon-preview/products");
            var directory = asset ? JsonUtility.FromJson<MobileProductDirectory>(asset.text) : null;
            if (directory?.routes == null || directory.routes.Length != 8)
                throw new InvalidOperationException("Missing mobile dungeon directory");
            mobileProductPreviews.Clear();
            foreach (var row in directory.routes) {
                if (row == null || string.IsNullOrEmpty(row.id) || row.count <= 0 || string.IsNullOrEmpty(row.sourceGraph) ||
                    Array.IndexOf(MapRegions,row.region) < 0 || mapDungeons.Exists(other => other.id == row.id))
                    throw new InvalidOperationException("Invalid mobile dungeon preview");
                row.runtimeIndex = -1;
                row.runtimeAdmitted = false;
                mobileProductPreviews.Add(row.id,row);
                mapDungeons.Add(row);
            }
        }

        void RenderMobileProductStatus(CandidateDungeonArt row) {
            if (row == null || !mobileProductPreviews.TryGetValue(row.id,out var preview)) return;
            if (mobileProductFailures.TryGetValue(row.id,out var failure)) {
                TextValue("Dungeon Enter Text","CONTENT UNAVAILABLE");
                TextValue("Dungeon Status",failure);
                images["Dungeon Enter"].color=C(0x293944);
            } else if (ReferenceEquals(row,preview)) {
                TextValue("Dungeon Enter Text","ENTER DUNGEON");
                TextValue("Dungeon Status","");
                images["Dungeon Enter"].color=C(0x476b61);
            }
        }

        bool TryStartMobileProductEntry() {
            if (!UseMobileProductDirectory || openMenu != "dungeons" || mapSelected < 0 || mapSelected >= mapDungeons.Count)
                return false;
            var row=mapDungeons[mapSelected];
            if (row.region != mapRegion || !IsReleaseDungeon(row.runtimeIndex,row.id)) return false;
            if (!mobileProductEntryRunning && !mobileProductFailures.ContainsKey(row.id))
                StartCoroutine(EnterMobileProduct(row));
            return true;
        }

        bool mobileCancelEntry;
        IEnumerator EnterMobileProduct(CandidateDungeonArt row) {
            mobileProductEntryRunning=true;
            mobileCancelEntry=false;
            var window=ShowMobileLoadingWindow("LOADING DUNGEON");
            try {
                yield return null;
                yield return null;
                if (mobileCancelEntry) yield break;
                if (openMenu != "dungeons" || mapSelected < 0 || mapSelected >= mapDungeons.Count || !ReferenceEquals(mapDungeons[mapSelected],row)) yield break;
                // Warm the first visible monster poses while this loading overlay owns input.
                yield return WarmHeroOpeners();
                yield return WarmEntryMonsters(row);
                if(mobileCancelEntry || openMenu!="dungeons" || mapSelected<0 || mapSelected>=mapDungeons.Count ||
                   !ReferenceEquals(mapDungeons[mapSelected],row)){ResetMonsterStream();yield break;}
                EnterSelectedDungeonNow();
                while (belowPlayerLoadingV21 || Mine2ProductSessionLoadingR1233 || FactoryProductSessionLoadingR1179 ||
                       CargoProductSessionLoadingR1288 || BattleProductSessionLoadingR1325 || PlainProductSessionLoadingR1320 ||
                       (coreProductSessionR1147 && forestPreviewLoading)) {
                    if (mobileCancelEntry) {
                        if (belowPlayerLoadingV21) EndBelowPlayerCandidateV21();
                        else SetMenu("dungeons");
                        yield break;
                    }
                    yield return null;
                }
            } finally {
                preparedMobileIndex=null;
                mobileProductEntryRunning=false;
                CloseMobileLoadingWindow(window);
                if(openMenu=="dungeons")RenderMenus();
            }
        }

        bool ResolveMobileProduct(CandidateDungeonArt preview) {
            // The preview is display-only. Replace it only with the exact row from the
            // ordinary registrar, preserving its reference identity and menu position.
            int slot=mapSelected;
            mapDungeons.RemoveAt(slot);
            CandidateDungeonArt registered=null;
            try {
                switch(preview.id) {
                    case "altera-below-tunnel": AppendBelowProductMapRouteR1130(""); break;
                    case "altera-core-hard": AppendCoreProductMapRouteR1143(""); break;
                    case "altera-nasod-factory": AppendFactoryProductMapRouteR1179(""); break;
                    case "bethma-richie-mine": AppendMine2ProductMapRouteR1233(""); break;
                    case "altera-plain":
                    case "altera-plain-recycle": AppendPlainProductMapRoutesR1320("",preview.sourceGraph); break;
                    case "bethma-cargo-airship": AppendCargoProductMapRouteR1288(""); break;
                    case "altera-battle-airship": AppendBattleProductMapRouteR1325(""); break;
                }
                registered=mapDungeons.Find(row=>row.id==preview.id);
                if (registered != null) mapDungeons.Remove(registered);
                if (registered == null || registered.region!=preview.region || registered.sourceGraph!=preview.sourceGraph ||
                    registered.count!=preview.count || registered.combatZones!=preview.combatZones) {
                    mobileProductFailures[preview.id]="CONTENT UNAVAILABLE";
                    return false;
                }
                return true;
            } catch (Exception error) {
                mobileProductFailures[preview.id]="CONTENT UNAVAILABLE";
                mapDungeons.RemoveAll(row=>row.id==preview.id);
                Debug.LogException(error);
                return false;
            } finally {
                mapDungeons.Insert(slot,mobileProductFailures.ContainsKey(preview.id) || registered==null ? preview : registered);
                mapSelected=slot;
            }
        }

        // Desktop validates when registering the row. Android registers only after
        // Enter is tapped and hands the index directly to the same validating host.
        void CheckMenuProductIndex(object index,Action validate) {
            if (mobileProductEntryRunning) preparedMobileIndex=index;
            else validate();
        }

        T TakeMobileProductIndex<T>(Func<T> create) where T:class {
            if (!(preparedMobileIndex is T index)) return create();
            preparedMobileIndex=null;
            return index;
        }

        void CheckHostProductFiles(string id,Action validate) {
            if (!UseMobileProductDirectory) { validate(); return; }
            if (mobileInstalledProducts.Contains(id)) return;
            try {
                validate();
                mobileInstalledProducts.Add(id);
            } catch {
                mobileProductFailures[id]="CONTENT NOT INSTALLED";
                throw;
            }
        }
    }
}
