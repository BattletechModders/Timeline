using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using BattleTech.UI;
using Harmony;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global

namespace Timeline.Features
{
    public class TextScroller
    {
        private static Camera ScrollerCamera;
        private static GameObject Scroller;
        private static float StartTime;
        private static float ReadingTime;

        #region TestingHotkeys

        //private static string string1 = "It was August 3049 when communications with outlying regions of the Inner Sphere first started mysteriously being cut.\n<br>What we knew was limited. An extremely advanced and previously unknown enemy, with futuristic weapons, was cleaning up garrisons and even small realms wholesale.\n<br>What we know now, that we didn't know then, was that the clans had arrived and with them 'mech combat was to be forever changed.\n<br>Until the Inner Sphere adapted to this deadly new threat, their losses would be enormous.";
        //private static string string2 = "Tukayyid was one of the bloodiest battles of the invasion, and with its victory the Inner Sphere gained a line in the sand below which the Clans would not invade.\n\nComstar's defence of the Inner Sphere and active role in politics, has the clans deep thrust into the tender nether-regions of the Inner Sphere halted, for now.\n\nThey call for a peace summit to try and unify the Inner Sphere more strongly against the clan aggression. Such overtures are, however, rebuffed.  Largely due to the depth and complexity of their prior clandestine behaviour.\n\nMeanwhile, technological development continues at a staggering rate. Clan tech is now much more readily being rushed from the front lines into the arms of awaiting house scientists, who develop the soon to be iconic Bushwacker Battlemech.";
        //private static string string3 = "A short but deadly war renders the Federated Commonwealth in twain and with clan pressure continuing unabated, entire regions of the Inner Sphere are plunged into leaderless chaos.\n\nNevertheless, the technological progress of the period continues, with the development and deployment of new Inner Sphere Omni 'Mech designs. Further new designs based on Clan 'Mechs are fielded, including the Rakshasa and the Black Hawk-KU.";
        //
        //
        //public class Updater : MonoBehaviour
        //{
        //    void Update()
        //    {
        //        if (Input.GetKeyDown(KeyCode.Alpha1)) CreateScroller(string1);
        //        if (Input.GetKeyDown(KeyCode.Alpha2)) CreateScroller(string2);
        //        if (Input.GetKeyDown(KeyCode.Alpha3)) CreateScroller(string3);
        //    }
        //}
        //
        //[HarmonyPatch(typeof(MainMenu), "Init")]
        //public static class Foo
        //{
        //    public static void Postfix(MainMenu __instance)
        //    {
        //        __instance.gameObject.AddComponent<Updater>();
        //    }
        //}

        #endregion

        // translates along local Y axis
        public class MoveUp : MonoBehaviour
        {
            private TextMeshPro textMeshPro;
            private const float speed = 0.015f;

            void Start()
            {
                textMeshPro = GetComponent<TextMeshPro>();
            }

            void Update()
            {
                if (ReadingTime + StartTime - Time.time < 0)
                {
                    StartCoroutine(FadeText.FadeOutText(textMeshPro));
                }

                // allow skipping via whatever keys
                var abortKeys = new[]
                {
                    KeyCode.Escape,
                    KeyCode.Space,
                    KeyCode.Mouse0,
                    KeyCode.Return
                };

                if (abortKeys.Any(Input.GetKeyDown))
                {
                    Destroy(GameObject.Find("Scroller Camera"));
                    Destroy(GameObject.Find("Scroller"));
                }
                else
                {
                    transform.Translate(0, speed, 0);
                }
            }
        }

        public class FadeText : MonoBehaviour
        {
            private static float fadeSpeed = 0.25f;

            internal static IEnumerator FadeOutText(TextMeshPro text)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
                while (text.color.a > 0)
                {
                    text.color = new Color(
                        text.color.r, text.color.g, text.color.b, text.color.a - Time.deltaTime * fadeSpeed);
                    yield return null;
                }

                Destroy(text);
                yield return new WaitForSeconds(1.25f);
                Destroy(GameObject.Find("Scroller Camera"));
                Destroy(GameObject.Find("Scroller"));
            }
        }

        internal static void CreateScroller(string text)
        {
            try
            {
                // it makes no sense but this const has a different
                // effect via the MainMenu test keys than when triggered
                // by the Timeline event selection
                // eg in testing it takes 55-60s for proper effect
                // in use it's more like 75s
                const float secondsPerCharacter = 0.175f;
                // approximate time to read instead of measuring distance
                // text fades after this time
                ReadingTime = text.Length * secondsPerCharacter;

                // found this .depth and .layer stuff online somewhere, maybe extreme values
                ScrollerCamera = new GameObject("Scroller Camera")
                    .AddComponent<Camera>();
                ScrollerCamera.depth = 100f;
                ScrollerCamera.transform.Translate(0, 20, 40);
                ScrollerCamera.backgroundColor = Color.black;
                ScrollerCamera.cullingMask = 1 << 30;

                Scroller = new GameObject("Scroller");
                Scroller.gameObject.AddComponent<MoveUp>();
                Scroller.gameObject.layer = 30;

                var scrollText = Scroller.AddComponent<TextMeshPro>();
                StartTime = Time.time;
                scrollText.font = UnityEngine.Resources
                    .FindObjectsOfTypeAll<TMP_FontAsset>()
                    .First(f => f.name == "Proxima Nova Semibold SDF");
                scrollText.alignment = TextAlignmentOptions.TopJustified;
                // tilt it and position it offscreen
                scrollText.transform.Rotate(Vector3.right, 80);
                scrollText.transform.Translate(0, 56, 0);
                scrollText.fontSize = 40f;

                var scrollRt = scrollText.GetComponentInChildren<RectTransform>();
                scrollText.SetText(text);
                scrollRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 45);
            }
            catch (Exception e)
            {
                Main.HBSLog?.LogException(e);
            }
        }
    }
}
