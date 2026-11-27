using UnityEngine;
using UnityEngine.UI;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        [System.Serializable] sealed class HamelBubble { public bool big; public float[] position; }
        Shader hamelAlphaShader, hamelAdditiveShader;
        Shader HamelBackdropShader(int blend) {
            if (blend == 2) {
                if (!hamelAdditiveShader) hamelAdditiveShader = Resources.Load<Shader>("Pixel/Shaders/SourceFactorOpaqueDestinationV27");
                return hamelAdditiveShader;
            }
            if (!hamelAlphaShader) hamelAlphaShader = Resources.Load<Shader>("Pixel/Shaders/WaterwaySourceAlphaV26");
            return hamelAlphaShader;
        }

        void BuildHamelBubbleVents(NativeScene scene) {
            if (scene.bubbles == null || scene.bubbles.Length == 0) return;
            var texture = Resources.Load<Texture2D>("Pixel/hamel/textures/resiam_underwater_bubble_effect");
            var material = new Material(Shader.Find("Sprites/Default")) { mainTexture = texture };
            nativeObjects.Add(material);
            foreach (var vent in scene.bubbles) {
                bool big = vent.big;
                var go = new GameObject(big ? "Pa_DeepSea_Bouble_Big" : "Pa_DeepSea_Bouble_Small");
                go.layer = 28;
                go.transform.SetParent(nativeRoot, false);
                go.transform.localPosition = NativeV(vent.position);
                var system = go.AddComponent<ParticleSystem>();
                system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                var main = system.main;
                main.duration = 8;
                main.startLifetime = 8;
                main.maxParticles = 50;
                main.startSpeed = 0;
                main.startSize = big ? new ParticleSystem.MinMaxCurve(36, 52) : new ParticleSystem.MinMaxCurve(25, 40);
                main.startColor = big ? new Color(.6f,.75f,1) : new Color(.55f,.85f,1);
                var shape = system.shape;
                shape.shapeType = ParticleSystemShapeType.Box;
                shape.scale = new Vector3(200,20,300);
                var velocity = system.velocityOverLifetime;
                velocity.enabled = true;
                velocity.x = velocity.z = new ParticleSystem.MinMaxCurve(0,0);
                velocity.y = big ? new ParticleSystem.MinMaxCurve(180,320) : new ParticleSystem.MinMaxCurve(150,300);
                var force = system.forceOverLifetime;
                force.enabled = true;
                force.x = force.z = new ParticleSystem.MinMaxCurve(-50,50);
                force.y = new ParticleSystem.MinMaxCurve(130,130);
                var color = system.colorOverLifetime;
                color.enabled = true;
                var gradient = new Gradient();
                gradient.SetKeys(new[] {new GradientColorKey(Color.white,0),new GradientColorKey(Color.white,1)},
                    new[] {new GradientAlphaKey(0,0),new GradientAlphaKey(big?1:.9f,big?.025f:.05f),
                           new GradientAlphaKey(big?.61f:.55f,1)});
                color.color = gradient;
                var size = system.sizeOverLifetime;
                size.enabled = true;
                size.size = new ParticleSystem.MinMaxCurve(1,big
                    ? new AnimationCurve(new Keyframe(0,1),new Keyframe(.375f,2.5f),new Keyframe(1,1.84f))
                    : AnimationCurve.Linear(0,1,1,.627f));
                var emission = system.emission;
                emission.rateOverTime = big ? 2 : 3;
                system.GetComponent<ParticleSystemRenderer>().sharedMaterial = material;
                system.Play();
            }
        }

        Image hamelAirTrack, hamelAirFill;
        Text hamelAirLabel;
        int lastWaterEntry;

        void RenderHamelAir() {
            bool show = adventureStarted && game.IsUnderWater && !game.victory;
            if (!show) {
                if (hamelAirTrack) hamelAirTrack.gameObject.SetActive(false);
                return;
            }
            if (!hamelAirTrack) {
                hamelAirTrack = Panel("Oxygen Track", new Rect(88, 377, 138, 13), C(0x163745,.94f));
                hamelAirFill = Panel("Oxygen Fill", new Rect(0, 0, 138, 13), C(0x3389ac), hamelAirTrack.transform);
                hamelAirLabel = Label("Oxygen", "OXYGEN", new Rect(4, 0, 128, 13), 10, Color.white,
                    TextAnchor.MiddleLeft, hamelAirTrack.transform);
            }
            hamelAirTrack.gameObject.SetActive(string.IsNullOrEmpty(openMenu));
            hamelAirFill.rectTransform.sizeDelta = new Vector2(138 * game.Oxygen * .01f, 13);
            hamelAirFill.color = game.Oxygen <= 20 ? C(0xb94952) : C(0x3389ac);
            hamelAirLabel.text = "OXYGEN  " + Mathf.CeilToInt(game.Oxygen) + "%";
            if (lastWaterEntry != game.WaterEntrySerial) {
                lastWaterEntry = game.WaterEntrySerial;
                string sound = "Pixel/hamel/sounds/water_diving";
                if (!audioClips.ContainsKey(sound)) {
                    var clip = Resources.Load<AudioClip>(sound);
                    if (clip) audioClips[sound] = clip;
                }
                PlayNativeSound(new TimedSound { sound = sound, volume = .38f }, 140);
            }
        }
    }
}
